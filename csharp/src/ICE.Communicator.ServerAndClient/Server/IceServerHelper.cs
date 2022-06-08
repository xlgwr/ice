using IceInternal;
using Solomon.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace SlmCommunicator.Server
{
    class IceServerHelper
    {
        #region 属性变量
        private static IceServerHelper _instance = null;
        private static object _lock = new object();
        private Ice.Communicator _communicator = null;
        private string _ip = string.Empty;
        private string _port = string.Empty;
        private bool _isReady = false;
        public int ACM_TIME_OUT = 60; //60秒 心跳间隔
        #region ssl
        private bool _enableSSL = false;
        private string _sslPath = string.Empty;
        private string _pemName = string.Empty;
        private string _p12Name = string.Empty;
        private string _sslPass = string.Empty;
        #endregion

        #endregion

        #region 构造方法
        public IceServerHelper()
        {

        }

        public static IceServerHelper GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new IceServerHelper();
                    }
                }
            }
            return _instance;
        }

        ~IceServerHelper()
        {

        }
        #endregion

        #region 私有方法
        Ice.InitializationData initData;
        Instance _communicatorInstance;
        private void workThread()
        {
            string endPointStr = string.Empty;
            try
            {
                ACM_TIME_OUT = ACM_TIME_OUT <= 0 ? 60 : ACM_TIME_OUT;

                initData = new Ice.InitializationData();
                initData.properties = Ice.Util.createProperties();
                initData.properties.setProperty("Ice.ACM.Client.Close", "4");
                initData.properties.setProperty("Ice.ACM.Client.Heartbeat", "3");
                initData.properties.setProperty("Ice.ACM.Client.Timeout", ACM_TIME_OUT.ToString());
                initData.properties.setProperty("Ice.ACM.Server.Close", "4");
                initData.properties.setProperty("Ice.ACM.Server.Heartbeat", "3");
                initData.properties.setProperty("Ice.ACM.Server.Timeout", ACM_TIME_OUT.ToString());
                initData.properties.setProperty("Ice.RetryIntervals", "-1");
                initData.properties.setProperty("Ice.MessageSizeMax", "2097152");//最大2GB 2097152KB;
                //initData.properties.setProperty("Ice.Override.Timeout", "5000");
                //tcp,upd缓存 字节
                initData.properties.setProperty("Ice.TCP.RcvSize", "10240000");
                initData.properties.setProperty("Ice.TCP.SndSize", "10240000");
                initData.properties.setProperty("Ice.UDP.RcvSize", "10240000");
                initData.properties.setProperty("Ice.UDP.SndSize", "10240000");
                //服务器线程设置
                //initData.properties.setProperty("Ice.ThreadPool.Client.Size", "4");
                //initData.properties.setProperty("Ice.ThreadPool.Client.SizeMax", "4");
                ////initData.properties.setProperty("Ice.ThreadPool.Client.SizeWarn", "5");

                //initData.properties.setProperty("Ice.ThreadPool.Server.Size", "4");
                //initData.properties.setProperty("Ice.ThreadPool.Server.SizeMax", "4");
                ////initData.properties.setProperty("Ice.ThreadPool.Server.SizeWarn", "5");
                if (_enableSSL)//启用安全加密的通信
                {
                    initData.properties.setProperty("Ice.Plugin.IceSSL", "IceSSL:IceSSL.PluginFactory");
                    initData.properties.setProperty("IceSSL.DefaultDir", _sslPath);
                    initData.properties.setProperty("IceSSL.CAs", _pemName);
                    initData.properties.setProperty("IceSSL.CertFile", _p12Name);
                    initData.properties.setProperty("IceSSL.Password", _sslPass);
                }

                initData.dispatcher = (System.Action action, Ice.Connection connection) =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.IceError(ex, "IceServerHelper.workThread");
                    }
                };

                _communicator = Ice.Util.initialize(initData);
                string needZip = string.Empty;
                if (System.Configuration.ConfigurationManager.AppSettings["NeedZip"] == "1")
                {
                    needZip = " -z";
                }
                endPointStr = string.Format("tcp -h {0} -p {1}{2}", _ip, _port, needZip);
                if (_enableSSL) //如果启用SSL
                {
                    endPointStr = string.Format("ssl -h {0} -p {1}{2}", _ip, _port, needZip);
                }
                Ice.ObjectAdapter adapter = _communicator.createObjectAdapterWithEndpoints("PackageSender", endPointStr);
                //Ice.ObjectAdapter adapter = _communicator.createObjectAdapter("PackageSender");
                //PackageSenderPtr senderPtr = new PackageSenderI(m_sessionManager, m_connCallback);
                //Ice::Identity id = m_communicator->stringToIdentity("packageSender");
                //adapter->add(senderPtr, id);
                //adapter->activate();
                //m_connCallback->OnConnectChangeEvent(CONNECT_OK);
                //m_communicator->waitForShutdown();
                adapter.add(new PackageSenderServerI(), _communicator.stringToIdentity("packageSender"));
                adapter.activate();
                EventServerHelper.GetInstance().OnConnectChangeEvent(ConnectStateServer.Connect_Ready);

                LogHelper.WriteSystemLog($"Client ICE:{GetIPStr()}");
                _communicator.waitForShutdown();
                _communicator?.Dispose();
                _communicator = null;
            }
            catch (Ice.Exception ex)
            {
                LogHelper.IceError(ex, "workThread", endPointStr);
                EventServerHelper.GetInstance().OnConnectChangeEvent(ConnectStateServer.Connect_Fail);
            }
        }
        #endregion

        #region 公有方法
        public int Init(string ip, string port)
        {
            _ip = ip;
            _port = port;
            Thread initThread = new Thread(workThread);
            initThread.IsBackground = true;
            initThread.Name = "initIceThread";
            initThread.Start();
            return 0;
        }

        public int SSLInit(string ip, string port, string sslPath, string pemName, string p12Name, string caPass)
        {
            _ip = ip;
            _port = port;
            _enableSSL = true;
            _sslPath = sslPath;
            _pemName = pemName;
            _p12Name = p12Name;
            _sslPass = caPass;

            Thread initThread = new Thread(workThread);
            initThread.IsBackground = true;
            initThread.Name = "initIceThread";
            initThread.Start();
            return 0;
        }

        public void Dispose()
        {
            try
            {
                if (_communicator == null)
                {
                    return;
                }
                _communicator.shutdown();
                EventServerHelper.GetInstance().OnConnectChangeEvent(ConnectStateServer.Connect_Fail);
            }
            catch (Exception ex)
            {
                LogHelper.IceError(ex, "Dispose");
            }
        }
        public string GetIPStr()
        {
            return $"{_ip}:{_port} ACM_TIME_OUT：{ACM_TIME_OUT}";
        }
        //public byte[] readByteSeq(SlmOrderRow requestPackageOrderLog)
        //{
        //    BasicStream os__ = new BasicStream(GetInstanceOs(), new Ice.EncodingVersion(3, 6));
        //    SlmCommunicator.SlmOrderRow.write__(os__, requestPackageOrderLog);
        //    var dd = os__.getBuffer();
        //    return dd.b.rawBytes();
        //}
        //public SlmOrderRow GetSlmOrderRow(byte[] readByteSeq)
        //{
        //    var result = new SlmOrderRow();
        //    BasicStream os__ = new BasicStream(GetInstanceOs(), new Ice.EncodingVersion(3, 6), readByteSeq);
        //    SlmCommunicator.SlmOrderRow.read__(os__, result);
        //    return result;
        //}
        public Instance GetInstanceOs()
        {
            if (initData == null)
            {
                initData = new Ice.InitializationData();
            }
            if (_communicatorInstance == null)
            {
                _communicatorInstance = new Instance(_communicator, initData);
            }
            return _communicatorInstance;
        }
        #endregion
    }
}
