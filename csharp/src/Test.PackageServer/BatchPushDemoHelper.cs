using SlmCommunicator;
using Solomon.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test.PackageServer
{
    class PushDemoData
    {
        public ResponsePackageOrder RequestPackage { get; set; }
        public int Type { get; set; }
    }

    /// <summary>
    /// 负责推送消息给xx
    /// </summary>
    public class BatchPushDemoHelper
    {
        #region 属性变量
        private static BatchPushDemoHelper _instance = null;
        private static object _lock = new object();
        private bool _start = false;
        public static bool IsRuning = false;
        private ConcurrentQueue<PushDemoData> _commandQueue = new ConcurrentQueue<PushDemoData>();
        public int intTimeSleep = 100;//毫秒
        public bool isAsync = false;
        public int bathcSize = 1000;//合包数
        #endregion

        #region 构造方法
        public BatchPushDemoHelper()
        {
            //注册消息
            //TradeEvent.GetInstance().RegSendDataToFrontEvent(AddData);
        }

        public static BatchPushDemoHelper GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new BatchPushDemoHelper();
                        IsRuning = true;
                    }
                }
            }
            return _instance;
        }

        ~BatchPushDemoHelper()
        {
            stopThread();
        }
        #endregion

        #region 私有方法
        private void stopThread()
        {
            _start = false;
        }

        private void workProcess()
        {
            _start = true;
            while (_start)
            {
                Thread.Sleep(intTimeSleep);

                Dictionary<int, List<ResponsePackageOrder>> currDicPush = new Dictionary<int, List<ResponsePackageOrder>>();
                try
                {
                    for (int i = 0; i < bathcSize; i++)
                    {
                        //处理消息
                        PushDemoData comm = getCommand();
                        if (comm != null)
                        {
                            if (currDicPush.ContainsKey(comm.Type))
                            {
                                currDicPush[comm.Type].Add(comm.RequestPackage);
                            }
                            else
                            {
                                currDicPush[comm.Type] = new List<ResponsePackageOrder>() { comm.RequestPackage };
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (currDicPush.Count > 0)
                    {
                        foreach (var item in currDicPush)
                        {
                            processCommand(item.Value.ToArray(), item.Key);
                        }
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }

                }
                catch (Exception ex)
                {
                    LogHelper.WriteErrorLog(ex);
                }

            }
        }

        private int processCommand(ResponsePackageOrder[] rps, int type)
        {
            var result = TestServerHelper.GetInstance().SendResponsePackageList(rps, type);
            return result;

        }

        private PushDemoData getCommand()
        {
            PushDemoData data = null;
            _commandQueue.TryDequeue(out data);
            return data;
        }

        #endregion

        #region 公有方法
        public void AddData(ResponsePackageOrder requestPackage, int type)
        {
            PushDemoData pushData = new PushDemoData();
            pushData.RequestPackage = requestPackage;
            pushData.Type = type;
            _commandQueue.Enqueue(pushData);
        }
        public void AddData(string clientId, int serviceNo, int errNo, string errInfo, object data, int type)
        {
            PushDemoData pushData = new PushDemoData();
            ResponsePackageOrder requestPackage = new ResponsePackageOrder();

            requestPackage.ClientId = clientId;
            requestPackage.ServiceNo = serviceNo;
            requestPackage.ErrorNo = errNo;
            requestPackage.ErrorInfo = errInfo;
            switch (serviceNo)
            {
                case 940001:
                case 910001:
                    requestPackage.SlmOrderBody = data as SlmOrderRow;
                    break;
                case 930002:
                    requestPackage.PushMatchStateBody = data as PushMatchStateRow;
                    break;
                case 930001:
                    requestPackage.PushOrderStateBody = data as PushOrderStateRow;
                    break;
                case 920001:
                    requestPackage.SendOrderStatusBody = data as SendOrderStatusRow;
                    break;
                case 920005:
                    requestPackage.SlmOrderJourBody = data as SlmOrderJourRow;
                    break;
                case 920002:
                    requestPackage.SlmMatchBody = data as SlmMatchRow[];
                    break;
                default:
                    requestPackage.DataBody = data as string[][];
                    break;
            }
            pushData.RequestPackage = requestPackage;
            pushData.Type = type;
            _commandQueue.Enqueue(pushData);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            stopThread();
        }

        public void StartThread()
        {
            Thread checkThread = new Thread(workProcess);
            checkThread.IsBackground = true;
            checkThread.Name = "BatchPushDemoThread";
            checkThread.Start();
        }
        #endregion
    }
}
