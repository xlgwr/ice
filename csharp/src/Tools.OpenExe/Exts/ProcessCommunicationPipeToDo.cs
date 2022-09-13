using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tools.OpenExe.Exts
{
    public class ProcessCommunicationPipeToDo
    {
        public const string pipname = "ToolsOpenExeProcessCommunicationPipe";
        NamedPipeServerStream namedPipeServerStream;
        NamedPipeClientStream namedPipeClientStream;
        SynchronizationContext synchronizationContext;
        public Action<string, bool, int> changeNoticeQueueAction { get; set; }
        public Action<string> isShowToDoAction { get; set; }
        public bool _isRun = true;
        CancellationTokenSource cancellationTokenSource;
        CancellationToken cancellationToken;
        public void changeNoticeQueue(string msg, bool ishow = true, int type = 1)
        {
            changeNoticeQueueAction?.Invoke(msg, ishow, type);
        }
        public ProcessCommunicationPipeToDo()
        {
            synchronizationContext = new SynchronizationContext();
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
        }
        ~ProcessCommunicationPipeToDo()
        {
            _isRun = false;
            cancellationTokenSource.Cancel();
            namedPipeServerStream?.Dispose();
            namedPipeClientStream?.Dispose();
        }
        public void ClientStreamDispose()
        {
            _isRun = false;
            cancellationTokenSource.Cancel();
            namedPipeClientStream?.Dispose();
            // Thread.Sleep((3000);
        }
        #region server
        public void ServerDisconnectAndInit()
        {
            Task.Factory.StartNew(() =>
            {
                lock (namedPipeServerStream)
                {
                    namedPipeServerStream?.Disconnect();
                    Thread.Sleep(1000);
                    ServerWaitForConnectionAsyncToDo();
                }
            });
        }
        public void ServerDisposeAndInit()
        {
            Task.Factory.StartNew(() =>
            {
                lock (namedPipeServerStream)
                {
                    namedPipeServerStream?.Disconnect();
                    namedPipeServerStream?.Dispose();
                    ServerInit();
                }
            });
        }
        public void ServerInit(string tmppipname = pipname)
        {
            try
            {
                ///定义一个命名管道，第一个参数是管道名称，第二个参数代表是输入类型还是输出类型 还是输入输出类型,以及设置最大的服务器实例，设置传输类型，以及开启可以异步的进行读取和写入
                namedPipeServerStream = new NamedPipeServerStream(tmppipname, PipeDirection.InOut, 10, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                ServerWaitForConnectionAsyncToDo();

            }
            catch (Exception ex)
            {
                changeNoticeQueue(ex.Message);
            }
        }
        public void ServerWaitForConnectionAsyncToDo()
        {
            try
            {
                if (!_isRun || cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                //异步等待客户端链接，如果上面的Options不是Asynchronous 异步则会报错
                namedPipeServerStream.WaitForConnectionAsync().ContinueWith(s =>
                {
                    var cts = synchronizationContext;
                    //刷新UI 告知有客户端链接
                    cts.Send(new System.Threading.SendOrPostCallback(b =>
                    {
                        changeNoticeQueue("Client Is Connected;");
                    }), null);

                    ServerStreamReadAsync();

                }, cancellationToken);
            }
            catch (Exception ex)
            {
                changeNoticeQueue(ex.Message);
            }
        }
        public void ServerStreamReadAsync()
        {
            if (!_isRun || cancellationToken.IsCancellationRequested)
            {
                return;
            }
            var cts = synchronizationContext;
            var valByte = new byte[1024];
            //异步读取客户端发送的消息
            namedPipeServerStream.ReadAsync(valByte, 0, valByte.Length).ContinueWith(m =>
             {
                 var val = valByte;
                 var str = Encoding.UTF8.GetString(val).Trim('\0');
                 if (!str.IsNullOrWhiteSpace())
                 {
                     cts.Send(new System.Threading.SendOrPostCallback(b =>
                     {
                         changeNoticeQueue($"Server Receive Val:{str}");
                         isShowToDoAction?.Invoke(str);

                     }), null);
                 }
                 // Thread.Sleep(1000);
                 //ServerStreamReadAsync();
             });

        }
        public void SendClient(string message)
        {
            if (namedPipeServerStream == null)
            {
                return;
            }
            //命名管道发送消息到客户端
            var data = Encoding.UTF8.GetBytes(message);
            //发送消息到客户端
            namedPipeServerStream.WriteAsync(data, 0, data.Length);
            changeNoticeQueue($"Server Send Val:{message}");
        }

        #endregion
        #region client
        void CheckIsConnectServer()
        {
            int count = 3;
            while (!namedPipeClientStream.IsConnected && count > 0)
            {
                count--;
                Thread.Sleep(1000);
            }

            if (!namedPipeClientStream.IsConnected)
            {
                ClientWaitForConnectionAsyncToDo();
            }
        }
        public void ClientInit(string currpipname = pipname)
        {
            try
            {
                //初始化
                if (namedPipeClientStream != null)
                {
                    //namedPipeClientStream.Dispose();
                    // Thread.Sleep((2000);
                    return;
                }

                var cts = synchronizationContext;
                //定义管道对象,如果需要是网络之间通信.替换为服务端的服务器名称和pipeName
                namedPipeClientStream = new NamedPipeClientStream(".", currpipname, PipeDirection.InOut, PipeOptions.Asynchronous);

                ClientWaitForConnectionAsyncToDo();

                CheckIsConnectServer();
            }
            catch (Exception ex)
            {
                changeNoticeQueue(ex.Message);
            }
        }
        public void ClientWaitForConnectionAsyncToDo()
        {
            try
            {
                if (!_isRun || cancellationToken.IsCancellationRequested)
                {
                    namedPipeClientStream.Dispose();
                    return;
                }
                //异步链接服务端
                var cts = synchronizationContext;
                namedPipeClientStream.ConnectAsync().ContinueWith(s =>
                {
                    var cs = cts;
                    cs.Send(new System.Threading.SendOrPostCallback(b =>
                    {
                        changeNoticeQueue($"Server Is Connected;");
                    }), null);

                    ClientStreamReadAsyncToDo();
                });
            }
            catch (Exception ex)
            {
                changeNoticeQueue(ex.Message);
            }
        }
        public void ClientStreamReadAsyncToDo()
        {
            if (!_isRun || cancellationToken.IsCancellationRequested)
            {
                namedPipeClientStream.Dispose();
                return;
            }

            var cts = synchronizationContext;
            var valByte = new byte[1024];
            //异步等待收到服务端发送的消息 然后更新到UI
            namedPipeClientStream.ReadAsync(valByte, 0, valByte.Length).ContinueWith((sb) =>
            {
                try
                {

                    var val = valByte;
                    var str = Encoding.UTF8.GetString(val).Trim('\0');
                    if (!str.IsNullOrWhiteSpace())
                    {
                        cts.Send(new System.Threading.SendOrPostCallback(b =>
                        {
                            changeNoticeQueue($"Client Receive Val:{str}");
                        }), null);
                    }
                    Thread.Sleep(1000);
                    ClientStreamReadAsyncToDo();
                }
                catch (Exception ex)
                {

                }

            }, cancellationToken);
        }
        public void SendServer(string message, string currpipname = pipname)
        {
            lock (this)
            {
                ClientInit(pipname);

                try
                {
                    //命名管道发送消息到客户端
                    var data = Encoding.UTF8.GetBytes(message);
                    //发送消息到客户端
                    namedPipeClientStream.WriteAsync(data, 0, data.Length);
                    changeNoticeQueue($"Client Send Val:{message}");
                }
                catch (Exception ex)
                {
                    changeNoticeQueue(ex.Message);
                }


            }
        }
        #endregion
    }
}
