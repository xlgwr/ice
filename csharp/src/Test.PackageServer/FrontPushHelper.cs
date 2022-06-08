using SlmCommunicator;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test.PackageServer
{
    class PushFrontData
    {
        public string ClientId { get; set; }
        public int ErrorNo { get; set; }
        public string ErrorInfo { get; set; }
        public int ServiceNo { get; set; }
        public string[] Head { get; set; }
        public string[][] DataBody { get; set; }
        public int Type { get; set; }
        public ComDto DtoData { get; set; } = new ComDto();
        public object PushData { get; set; }
    }

    /// <summary>
    /// 负责推送消息给前置
    /// </summary>
    public class FrontPushHelper
    {
        #region 属性变量
        private static FrontPushHelper _instance = null;
        private static object _lock = new object();
        private bool _start = false;
        public static bool IsRuning = false;
        private ConcurrentQueue<PushFrontData> _commandQueue = new ConcurrentQueue<PushFrontData>();
        public static int intTimeSleep = 100;
        #endregion

        #region 构造方法
        public FrontPushHelper()
        {
            //注册消息
            App._ActionAll.SendDataToFrontEventHandler = AddData;
        }

        public static FrontPushHelper GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new FrontPushHelper();
                        IsRuning = true;
                    }
                }
            }
            return _instance;
        }

        ~FrontPushHelper()
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
                //处理消息
                PushFrontData comm = getCommand();
                if (comm != null)
                {
                    processCommand(comm);
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        private void processCommand(PushFrontData data)
        {
            switch (data.DtoData.MType)
            {
                case emMType.Base:
                    TestServerHelper.GetInstance().SendResponsePackage(data.ClientId, data.ServiceNo, data.ErrorNo, data.ErrorInfo, data.DataBody, data.Type);
                    break;
                case emMType.OrderList:
                    TestServerHelper.GetInstance().SendResponsePackageList(data.PushData as ResponsePackageOrder[], data.Type);
                    break;
                case emMType.CacheDB:
                    TestServerHelper.GetInstance().SendPackageCacheBaseAsync(data.PushData as PackageCacheDB, data.Type);
                    break;
                case emMType.TradeDB:
                    TestServerHelper.GetInstance().SendPackageCacheTradeAsync(data.PushData as CacheTradeDB, data.Type);
                    break;
                default:
                    break;
            }
        }

        private PushFrontData getCommand()
        {
            PushFrontData data = null;
            _commandQueue.TryDequeue(out data);
            return data;
        }

        #endregion

        #region 公有方法
        public void AddData(string clientId, int serviceNo, int errNo, string errInfo, string[][] data, int type)
        {
            PushFrontData pushData = new PushFrontData();
            pushData.ClientId = clientId;
            pushData.ServiceNo = serviceNo;
            pushData.ErrorNo = errNo;
            pushData.ErrorInfo = errInfo;
            pushData.DataBody = data;
            pushData.Type = type;
            _commandQueue.Enqueue(pushData);
        }

        public void AddData(ResponsePackageDTO curr, int type)
        {
            if (!curr.DataBody.HasItem())
            {
                return;
            }
            PushFrontData pushData = new PushFrontData();
            pushData.ClientId = curr.ClientId;
            pushData.ServiceNo = curr.ServiceNo;
            pushData.ErrorNo = curr.ErrorNo;
            pushData.ErrorInfo = curr.ErrorInfo;
            pushData.Head = curr.Head?.ToArray();
            var arrbody = new string[curr.DataBody.Count][];
            var first = 0;
            foreach (var item in curr.DataBody.Values)
            {
                arrbody[first] = item.ToArray();
                first++;
            }
            pushData.DataBody = arrbody;
            pushData.Type = type;
            _commandQueue.Enqueue(pushData);
        }
        public void AddData(ComDto curr, int type)
        {
            if (curr == null)
            {
                return;
            }
            PushFrontData pushData = new PushFrontData();
            switch (curr.MType)
            {
                case emMType.Base:
                    break;
                case emMType.OrderList:
                    pushData = UpdateData(curr as ResponsePackageOrderDto, type);
                    break;
                case emMType.CacheDB:
                    pushData = UpdateData(curr as PackageCacheDBDto, type);
                    break;
                case emMType.TradeDB:
                    pushData = UpdateData(curr as CacheTradeDBDto, type);
                    break;
                default:
                    break;
            }
            _commandQueue.Enqueue(pushData);
        }
        private PushFrontData UpdateData(ResponsePackageOrderDto curr, int type)
        {
            PushFrontData pushFData = new PushFrontData();
            if (!curr.DataBody.HasItem())
            {
                return pushFData;
            }
            pushFData.DtoData.MType = curr.MType;
            pushFData.Type = type;
            pushFData.ServiceNo = curr.ServiceNo;

            List<ResponsePackageOrder> pushlist = new List<ResponsePackageOrder>();
            //实体类发送类
            ResponsePackageOrder pushData = new ResponsePackageOrder();

            pushData.ClientId = curr.ClientId;
            pushData.ServiceNo = curr.ServiceNo;
            pushData.MsgId = curr.MsgId;
            pushData.Head = curr.Head?.ToArray();
            var arrbody = new string[curr.DataBody.Count][];
            var first = 0;
            foreach (var item in curr.DataBody.Values)
            {
                arrbody[first] = item.ToArray();
                first++;
            }
            pushData.DataBody = arrbody;

            pushData.SlmOrderBody = curr.SlmOrderBody;
            pushData.SlmOrderJourBody = curr.SlmOrderJourBody;
            pushData.SlmMatchBody = curr.SlmMatchBody?.ToArray();
            pushData.PushMatchStateBody = curr.PushMatchStateBody;
            pushData.SendOrderStatusBody = curr.SendOrderStatusBody;

            pushlist.Add(pushData);

            pushFData.PushData = pushlist.ToArray();

            return pushFData;
        }
        private PushFrontData UpdateData(PackageCacheDBDto curr, int type)
        {
            PushFrontData pushFData = new PushFrontData();
            if (!curr.DataBody.HasItem())
            {
                return pushFData;
            }
            pushFData.DtoData.MType = curr.MType;
            pushFData.ServiceNo = curr.ServiceNo;
            pushFData.Type = type;
            //实体类发送类
            PackageCacheDB pushData = new PackageCacheDB();
            pushData.ClientId = curr.ClientId;
            pushData.ServiceNo = curr.ServiceNo;
            pushData.ErrorNo = curr.ErrorNo;
            pushData.ErrorInfo = curr.ErrorInfo;
            pushData.Head = curr.Head?.ToArray();
            var arrbody = new string[curr.DataBody.Count][];
            var first = 0;
            foreach (var item in curr.DataBody.Values)
            {
                arrbody[first] = item.ToArray();
                first++;
            }
            pushData.DataBody = arrbody;

            pushFData.PushData = pushData;

            return pushFData;
        }
        private PushFrontData UpdateData(CacheTradeDBDto model, int type)
        {
            PushFrontData pushFData = new PushFrontData();
            var curr = model.PackageData;
            if (!curr.DataBody.HasItem())
            {
                return pushFData;
            }
            pushFData.DtoData.MType = model.MType;
            pushFData.ServiceNo = curr.ServiceNo;
            pushFData.Type = type;
            //实体类发送类
            PackageCacheDB pushParmData = new PackageCacheDB();
            pushParmData.ClientId = curr.ClientId;
            pushParmData.ServiceNo = curr.ServiceNo;
            pushParmData.ErrorNo = curr.ErrorNo;
            pushParmData.ErrorInfo = curr.ErrorInfo;
            pushParmData.Head = curr.Head?.ToArray();
            var arrbody = new string[curr.DataBody.Count][];
            var first = 0;
            foreach (var item in curr.DataBody.Values)
            {
                arrbody[first] = item.ToArray();
                first++;
            }
            pushParmData.DataBody = arrbody;
            //
            CacheTradeDB pushData = new CacheTradeDB();
            pushData.PackageData = pushParmData;

            pushData.Orders = model.Orders?.ToArray();
            pushData.Orderjours = model.Orderjours?.ToArray();
            pushData.Trades = model.Trades?.ToArray();
            pushData.Positions = model.Positions?.ToArray();
            pushData.PrePositions = model.PrePositions?.ToArray();
            pushData.Deposits = model.Deposits?.ToArray();

            //
            pushFData.PushData = pushData;

            return pushFData;
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
            checkThread.Name = "FrontPushThread";
            checkThread.Start();
        }
        #endregion
    }
}
