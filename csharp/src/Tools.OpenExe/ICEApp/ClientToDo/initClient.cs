using Ice;
using SlmCommunicator;
using SlmCommunicator.Client;
using Solomon.Log;
using Solomon.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tools.OpenExe.QueueToDo;
using Communicator = SlmCommunicator.Client.CommunicatorClientHelper;
using ConnectState = SlmCommunicator.Client.ConnectStateClient;

namespace Tools.OpenExe.ICEApp.ClientToDo
{
    public class initClient
    {
        #region 参数 
        public string logPrefixFileName = "Tools.OpenExe.ICEApp.ClientToDo";
        public static readonly object _lock = new object();
        public Action<ConnectState> ConnectAction { get; set; }
        ConnectState _state = ConnectState.CONNECT_FAIL;
        private static readonly Lazy<initClient> _instanceLazy =
               new Lazy<initClient>(() => new initClient());
        public Action<string> actionMsg { get; set; }
        public int clientType { get; set; }

        public enum CONNECT_STATE
        {
            DISCONNECT = 0,
            CONNECTED = 1,// 连接成功，验证通过
            CONNECTED_AUTO_RE = 2, // 自动重新连接 
        }
        #region 服务设置
        public const int QUOT_TYPE = 0;
        #endregion
        public delegate void ClientConnectionChange(int type, CONNECT_STATE state);
        public event ClientConnectionChange ClientConnectionChangeEvent;
        static Communicator c;
        public Dictionary<string, Communicator> CommunicatorDics = new Dictionary<string, Communicator>();
        public Dictionary<string, ConnectState> ConnectStateDics = new Dictionary<string, ConnectState>();
        /// <summary>
        /// 用于收集，收到的回报
        /// </summary>
        public Queue<ResponsePackageEx> QueueresponsePackages = new Queue<ResponsePackageEx>();
        public bool isShowAllMsg { get; set; }
        public List<int> filterServernos { get; set; } = new List<int>();
        public bool isAutoCheckTime { get; set; } = false;

        #endregion

        public initClient()
        {
            //StartToQueueToDo();
        }
        ~initClient()
        {
            DisconnectServer();
        }

        /// <summary>
        /// 使用Lazy创建单例，默认是线程安全的,获取对应单例
        /// </summary>
        /// <value></value>
        public static initClient GetInstanceLazy
        {
            get
            {
                return _instanceLazy.Value;
            }
        }

        public object SendRequest(PackageRequestDTO package, string keyCommunicator = "", bool isAsyn = false)
        {
            Communicator currC;
            if (keyCommunicator.IsNotNullOrWhiteSpace())
            {
                currC = CommunicatorDics.GetValue(keyCommunicator, null);
            }
            else
            {
                currC = c;
                if (_state != ConnectState.CONNECT_OK)
                {
                    //throw new Exception("服务器没有连接成功");
                    actionMsg?.Invoke($"服务器没有连接成功");
                    return "";
                }
            }
            if (currC == null)
            {
                actionMsg?.Invoke($"服务器没有连接成功");
                return "";
            }

            //if (!currC.InitPackage())
            //{
            //    //throw new Exception("服务器忙，稍后再试");
            //    Console.WriteLine("服务器忙，稍后再试");
            //    return "";
            //}
            string[][] arrbody = null;
            if (package.DataBody.HasItem())
            {
                arrbody = new string[package.DataBody.Count][];
                var first = 0;
                foreach (var item in package.DataBody.Values)
                {
                    arrbody[first] = item.ToArray();
                    first++;
                }
            }

            if (isAsyn)
            {
                bool rp2 = currC.SendPackageAsyn(package.ServiceNo, package.Head?.ToArray(), arrbody?.FirstOrDefault());
                return rp2;
            }
            ResponsePackage rp = currC.SendPackage(package.ServiceNo, package.Head?.ToArray(), arrbody?.FirstOrDefault()).PackageData;
            var rpEx = new ResponsePackageEx(rp);
            var result = new
            {
                Main = rpEx.PackageData,
                Head = rpEx.PackageData.Head,
                DataBody = rpEx.PackageData.DataBody
            };
            package = null;
            return rp;
        }

        public object SendRequest(ComDto package, string keyCommunicator = "", bool isAsyn = false, Tuple<bool, int, int, bool, bool, bool> isAdd = null)
        {
            Communicator currC;
            if (keyCommunicator.IsNotNullOrWhiteSpace())
            {
                currC = CommunicatorDics.GetValue(keyCommunicator, null);
            }
            else
            {
                currC = c;
                if (_state != ConnectState.CONNECT_OK)
                {
                    //throw new Exception("服务器没有连接成功");
                    actionMsg?.Invoke($"服务器没有连接成功");
                    return "";
                }
            }
            if (currC == null)
            {
                actionMsg?.Invoke($"服务器没有连接成功2");
                return "";
            }

            //if (!currC.InitPackage())
            //{
            //    //throw new Exception("服务器忙，稍后再试");
            //    Console.WriteLine("服务器忙，稍后再试");
            //    return "";
            //}

            if (package == null)
            {
                return "";
            }
            var pushData = new SendCommData();
            switch (package.MType)
            {
                case emMType.Base:
                    pushData = UpdateDataBase(package as PackageRequestDTO, clientType, currC.GetUUID());
                    break;
                case emMType.OrderList:
                    pushData = UpdateData(package as PackageRequestDTO, clientType, currC.GetUUID());
                    break;
                case emMType.CacheDB:
                    pushData = UpdateData(package as PackageCacheDBDto, clientType, currC.GetUUID());
                    break;
                case emMType.TradeDB:
                    pushData = UpdateData(package as CacheTradeDBDto, clientType, currC.GetUUID());
                    break;
                default:
                    break;
            }

            var rp = SendPackage(pushData, currC, isAdd);
            return rp;
        }
        private SendCommData UpdateDataBase(PackageRequestDTO curr, int type, string uuid = "")
        {
            var pushFData = new SendCommData();
            pushFData.DtoData.MType = curr.MType;
            pushFData.DtoData.MsgId = curr.MsgId;
            pushFData.Type = type;
            //实体类发送类
            RequestPackage pushData = new RequestPackage();
            pushData.UUID = uuid;
            pushData.ClientId = curr.ClientId;
            pushData.ServiceNo = curr.ServiceNo;
            pushData.Head = curr.Head?.ToArray();

            if (curr.IsAddSendTime || curr.MsgId.IsNotNullOrWhiteSpace())
            {
                pushData.Head = new string[] { DateTime.Now.toFormatfff(), curr.MsgId };
            }

            string[][] arrbody = null;
            if (curr.DataBody.HasItem())
            {
                arrbody = new string[curr.DataBody.Count][];
                var first = 0;
                foreach (var item in curr.DataBody.Values)
                {
                    arrbody[first] = item.ToArray();
                    first++;
                }
            }
            pushData.DataBody = arrbody?.FirstOrDefault();

            pushFData.PushData = pushData;

            return pushFData;
        }
        private SendCommData UpdateData(PackageRequestDTO curr, int type, string uuid = "")
        {
            var pushFData = new SendCommData();
            pushFData.DtoData.MType = curr.MType;
            pushFData.DtoData.MsgId = curr.MsgId;
            pushFData.Type = type;
            //实体类发送类
            RequestPackageOrder pushData = new RequestPackageOrder();
            pushData.UUID = uuid;
            pushData.ClientId = curr.ClientId;
            pushData.MsgId = curr.MsgId ?? Guid.NewGuid().ToString();
            pushData.ServiceNo = curr.ServiceNo;
            pushData.Head = curr.Head?.ToArray();
            if (curr.IsAddSendTime)
            {
                pushData.Head = new string[] { DateTime.Now.toFormatfff() };
            }
            string[][] arrbody = null;
            if (curr.DataBody.HasItem())
            {
                arrbody = new string[curr.DataBody.Count][];
                var first = 0;
                foreach (var item in curr.DataBody.Values)
                {
                    arrbody[first] = item.ToArray();
                    first++;
                }
            }
            pushData.DataBody = arrbody;
            pushData.SlmOrderBody = curr.SlmOrderBody;
            pushData.PushMatchStateBody = curr.PushMatchStateBody;
            pushData.PushOrderStateBody = curr.PushOrderStateBody;
            pushData.SlmMatchBody = curr.SlmMatchBody?.ToArray();
            pushData.SendOrderStatusBody = curr.SendOrderStatusBody;
            pushData.SlmOrderJourBody = curr.SlmOrderJourBody;

            pushFData.PushData = pushData;

            return pushFData;
        }

        private SendCommData UpdateData(PackageCacheDBDto curr, int type, string uuid = "")
        {
            var pushFData = new SendCommData();

            pushFData.DtoData.MType = curr.MType;
            pushFData.DtoData.MsgId = curr.MsgId;
            pushFData.Type = type;
            //实体类发送类
            PackageCacheDB pushData = new PackageCacheDB();
            pushData.UUID = uuid;
            pushData.ClientId = curr.ClientId;
            pushData.MsgId = curr.MsgId ?? Guid.NewGuid().ToString();
            pushData.ServiceNo = curr.ServiceNo;
            pushData.ErrorNo = curr.ErrorNo;
            pushData.ErrorInfo = curr.ErrorInfo;
            pushData.Head = curr.Head?.ToArray();
            if (curr.IsAddSendTime)
            {
                pushData.Head = new string[] { DateTime.Now.toFormatfff() };
            }
            if (curr.DataBody.HasItem())
            {
                var arrbody = new string[curr.DataBody.Count][];
                var first = 0;
                foreach (var item in curr.DataBody.Values)
                {
                    arrbody[first] = item.ToArray();
                    first++;
                }
                pushData.DataBody = arrbody;
            }

            pushFData.PushData = pushData;

            return pushFData;
        }
        private SendCommData UpdateData(CacheTradeDBDto model, int type, string uuid = "")
        {
            var pushFData = new SendCommData();
            var curr = model.PackageData;
            if (!curr.DataBody.HasItem())
            {
                return pushFData;
            }
            if (curr.IsAddSendTime)
            {
                curr.Head = new List<string> { DateTime.Now.toFormatfff() };
            }
            pushFData.DtoData.MType = model.MType;
            pushFData.DtoData.MsgId = model.MsgId;
            pushFData.ServiceNo = curr.ServiceNo;
            pushFData.Type = type;
            //
            CacheTradeDB pushData = model.Copy();
            //
            pushData.PackageData.UUID = uuid;

            pushFData.PushData = pushData;

            return pushFData;
        }

        private object SendPackage(SendCommData data, Communicator currC, Tuple<bool, int, int, bool, bool, bool> isAdd = null)
        {
            actionMsg?.Invoke($"发送数据：{DateTime.Now.toFormatfff()},{data.toJsonStr()}");
            switch (data.DtoData.MType)
            {
                case emMType.Base:
                    var currSend = data.PushData as RequestPackage;
                    if (isAutoCheckTime)
                    {
                        MsgIdCheckClientQueue.GetInstance().AddDataDic(data.DtoData.MsgId, $"{data.DtoData.MType}");
                    }
                    return currC.SendPackageAsyn(currSend.ServiceNo, currSend.Head, currSend.DataBody);
                    break;
                case emMType.OrderList:
                    BatchAddToSend(data.PushData as RequestPackageOrder, currC, isAdd);
                    break;
                case emMType.TradeDB:
                    BatchAddToSend(data.PushData as CacheTradeDB, currC, isAdd);
                    break;
                case emMType.CacheDB:
                    return currC.SendPackageCacheBaseAsync(data.PushData as PackageCacheDB);
                    break;
                default:
                    break;
            }
            return false;
        }
        /// <summary>
        /// isadd
        /// numSend
        /// numPackage
        /// </summary>
        /// <param name="pushData"></param>
        /// <param name="currC"></param>
        /// <param name="isAdd"></param>
        public void BatchAddToSend(RequestPackageOrder pushData, Communicator currC, Tuple<bool, int, int, bool, bool, bool> isAdd)
        {
            if (pushData == null)
            {
                actionMsg?.Invoke($"RequestPackageOrder 为null");
                return;
            }
            if (isAdd == null)
            {
                actionMsg?.Invoke($"RequestPackageOrder isAdd 为null");
                return;
            }
            actionMsg?.Invoke($"RequestPackageOrder,主键是否递增：{isAdd.Item1},发送次数：{isAdd.Item2},合包数：{isAdd.Item3},是否合包：{isAdd.Item4},是否使用icestream：{isAdd.Item5}");

            int addNum = 1;
            for (int i = 0; i < isAdd.Item2; i++)
            {
                //是否使用ICEStream
                if (isAdd.Item5)
                {
                    StreamPackage[] requestPackageOrders = new StreamPackage[isAdd.Item3];
                    var iOs = currC.GetInstanceOs();
                    var v = currC.GetEncodingVersion();
                    for (int j = 0; j < isAdd.Item3; j++)
                    {
                        requestPackageOrders[j] = pushData.CopyStream(isAdd.Item1, addNumDic, iOs, v);
                        addNum++;
                    }
                    if (isAutoCheckTime)
                    {
                        MsgIdCheckClientQueue.GetInstance().AddDataDic(pushData.MsgId, $"{pushData.ServiceNo}");
                    }
                    currC.SendStreamPackageListAsync(requestPackageOrders);
                    //是否保存字节流

                    if (isAdd.Item6)
                    {
                        PackageOrderType getType = PackageOrderType.Unkow;
                        string PackageOrderTypeStr = MsgTypeForLog.PackageOrderType;
                        string fileName = $"RequestPackageOrder";
                        foreach (var item in requestPackageOrders)
                        {
                            getType = item.DatasDic[PackageOrderTypeStr].GetValue<PackageOrderType>();
                            fileName = $"CacheTradeDB{getType}";
                            LogHelper.GetInstance().WriteOneBytelog(item.DataStreamBody[0], fileName, getType);
                        }
                        actionMsg?.Invoke($"Warn:RequestPackageOrder 写字节流文件：{fileName}");
                    }
                }
                else
                {
                    RequestPackageOrder[] requestPackageOrders = new RequestPackageOrder[isAdd.Item3];
                    for (int j = 0; j < isAdd.Item3; j++)
                    {
                        requestPackageOrders[j] = pushData.Copy(isAdd.Item1, addNumDic);
                        addNum++;
                    }
                    if (isAutoCheckTime)
                    {
                        MsgIdCheckClientQueue.GetInstance().AddDataDic(pushData.MsgId, $"{pushData.ServiceNo}");
                    }
                    currC.SendPackageListAsyn(requestPackageOrders);
                }

            }
        }

        public Dictionary<int, int> addNumDic = new Dictionary<int, int>
        {
            {1,1 },
            {2,1 },
            {3,1 },
            {4,1 },
            {5,1 },
            {6,1 }
        };
        public Dictionary<int, int> addNumListDic = new Dictionary<int, int>
        {
            {1,1 },
            {2,1 },
            {3,1 },
            {4,1 }
        };
        /// <summary>
        /// 初始化计数
        /// </summary>
        public void InitDicAddNum()
        {
            addNumDic = new Dictionary<int, int>
            {
            {1,1 },
            {2,1 },
            {3,1 },
            {4,1 },
            {5,1 },
            {6,1 }
             };
            addNumListDic = new Dictionary<int, int>
        {
            {1,1 },
            {2,1 },
            {3,1 },
            {4,1 }
        };
        }
        public void BatchAddToSend(CacheTradeDB pushData, Communicator currC, Tuple<bool, int, int, bool, bool, bool> isAdd)
        {
            if (pushData == null)
            {
                actionMsg?.Invoke($"RequestPackageOrder 为null");
                return;
            }
            if (isAdd == null)
            {
                actionMsg?.Invoke($"RequestPackageOrder isAdd 为null");
                return;
            }
            actionMsg?.Invoke($"CacheTradeDB,主键是否递增：{isAdd.Item1},发送次数：{isAdd.Item2},合包数：{isAdd.Item3}");

            if (!isAdd.Item1)
            {
                currC.SendPackageCacheTradeAsync(pushData);
                return;
            }

            for (int i = 0; i < isAdd.Item2; i++)
            {
                //是否使用ICEStream
                if (isAdd.Item5)
                {
                    StreamPackage[] sendDataes = new StreamPackage[isAdd.Item3];
                    var iOs = currC.GetInstanceOs();
                    var v = currC.GetEncodingVersion();

                    for (int j = 0; j < isAdd.Item3; j++)
                    {
                        var item = pushData.Clone() as CacheTradeDB;

                        item.PackageData = pushData.PackageData.Clone() as PackageCacheDB;
                        item.PackageData.MsgId = Guid.NewGuid().ToString();

                        if (pushData.Orders.HasItem())
                        {
                            item.Orders = new OrderCacheDB[] { pushData.Orders.First().Copy(isAdd.Item1, addNumDic[1]) };
                            addNumDic[1]++;
                        }
                        if (pushData.Orderjours.HasItem())
                        {
                            item.Orderjours = new OrderjourCacheDB[] { pushData.Orderjours.First().Copy(isAdd.Item1, addNumDic[2]) };
                            addNumDic[2]++;
                        }
                        if (pushData.Trades.HasItem())
                        {
                            item.Trades = new TradeCacheDB[] { pushData.Trades.First().Copy(isAdd.Item1, addNumDic[3]) };
                            addNumDic[3]++;
                        }
                        if (pushData.Deposits.HasItem())
                        {
                            item.Deposits = new DepositCacheDB[] { pushData.Deposits.First().Copy(isAdd.Item1, addNumDic[4]) };
                            addNumDic[4]++;
                        }

                        sendDataes[j] = item.GetStreamPackage(iOs, v);
                    }

                    currC.SendStreamPackageListAsync(sendDataes, "send");

                    //是否保存字节流
                    if (isAdd.Item6)
                    {
                        PackageOrderType getType = PackageOrderType.Unkow;
                        string PackageOrderTypeStr = MsgTypeForLog.PackageOrderType;
                        string fileName = string.Empty;
                        foreach (var item in sendDataes)
                        {
                            getType = item.DatasDic[PackageOrderTypeStr].GetValue<PackageOrderType>();
                            fileName = $"CacheTradeDB{getType}";
                            LogHelper.GetInstance().WriteOneBytelog(item.DataStreamBody[0], fileName, getType);
                        }
                        actionMsg?.Invoke($"Warn:RequestPackageOrder 写字节流文件：{fileName}");
                    }
                    continue;
                }
                //是否合包
                if (!isAdd.Item4)
                {
                    CacheTradeDB sendData = pushData.Clone() as CacheTradeDB;
                    sendData.PackageData = pushData.PackageData.Clone() as PackageCacheDB;

                    sendData.Orders = pushData.Orders.HasItem() ? new OrderCacheDB[isAdd.Item3] : null;
                    sendData.Orderjours = pushData.Orderjours.HasItem() ? new OrderjourCacheDB[isAdd.Item3] : null;
                    sendData.Trades = pushData.Trades.HasItem() ? new TradeCacheDB[isAdd.Item3] : null;
                    sendData.Deposits = pushData.Deposits.HasItem() ? new DepositCacheDB[isAdd.Item3] : null;

                    for (int j = 0; j < isAdd.Item3; j++)
                    {
                        if (pushData.Orders.HasItem())
                        {
                            sendData.Orders[j] = pushData.Orders.First().Copy(isAdd.Item1, addNumDic[1]);
                            addNumDic[1]++;
                        }
                        if (pushData.Orderjours.HasItem())
                        {
                            sendData.Orderjours[j] = pushData.Orderjours.First().Copy(isAdd.Item1, addNumDic[2]);
                            addNumDic[2]++;
                        }
                        if (pushData.Trades.HasItem())
                        {
                            sendData.Trades[j] = pushData.Trades.First().Copy(isAdd.Item1, addNumDic[3]);
                            addNumDic[3]++;
                        }
                        if (pushData.Deposits.HasItem())
                        {
                            sendData.Deposits[j] = pushData.Deposits.First().Copy(isAdd.Item1, addNumDic[4]);
                            addNumDic[4]++;
                        }
                    }
                    sendData.PackageData.MsgId = Guid.NewGuid().ToString();
                    currC.SendPackageCacheTradeAsync(sendData);

                }
                else
                {

                    //else
                    {

                        CacheTradeDB[] sendDataes = new CacheTradeDB[isAdd.Item3];
                        for (int j = 0; j < isAdd.Item3; j++)
                        {
                            sendDataes[j] = pushData.Clone() as CacheTradeDB;
                            var item = sendDataes[j];

                            item.PackageData = pushData.PackageData.Clone() as PackageCacheDB;
                            item.PackageData.MsgId = Guid.NewGuid().ToString();

                            if (pushData.Orders.HasItem())
                            {
                                item.Orders = new OrderCacheDB[] { pushData.Orders.First().Copy(isAdd.Item1, addNumDic[1]) };
                                addNumDic[1]++;
                            }
                            if (pushData.Orderjours.HasItem())
                            {
                                item.Orderjours = new OrderjourCacheDB[] { pushData.Orderjours.First().Copy(isAdd.Item1, addNumDic[2]) };
                                addNumDic[2]++;
                            }
                            if (pushData.Trades.HasItem())
                            {
                                item.Trades = new TradeCacheDB[] { pushData.Trades.First().Copy(isAdd.Item1, addNumDic[3]) };
                                addNumDic[3]++;
                            }
                            if (pushData.Deposits.HasItem())
                            {
                                item.Deposits = new DepositCacheDB[] { pushData.Deposits.First().Copy(isAdd.Item1, addNumDic[4]) };
                                addNumDic[4]++;
                            }
                        }
                        currC.SendPackageCacheTradeListAsync(sendDataes);
                    }
                }
            }
        }
        public object SendRequestList(List<PackageRequestDTO> packages, string keyCommunicator = "", bool isAsyn = false, bool islog = false)
        {
            Communicator currC;
            if (keyCommunicator.IsNotNullOrWhiteSpace())
            {
                currC = CommunicatorDics.GetValue(keyCommunicator, null);
            }
            else
            {
                currC = c;
                if (_state != ConnectState.CONNECT_OK)
                {
                    //throw new Exception("服务器没有连接成功");
                    actionMsg?.Invoke($"服务器没有连接成功");
                    return null;
                }
            }
            if (currC == null)
            {
                actionMsg?.Invoke($"服务器没有连接成功");
                return null;
            }

            //if (!currC.InitPackage())
            //{
            //    //throw new Exception("服务器忙，稍后再试");
            //    Console.WriteLine("服务器忙，稍后再试");
            //    return "";
            //}
            var requestPackages = new List<RequestPackageOrder>();

            foreach (var item in packages)
            {

                var tmpPackage = new RequestPackageOrder();
                tmpPackage.ClientId = currC.GetClientId();
                tmpPackage.UUID = currC.GetUUID();
                tmpPackage.MsgId = item.MsgId ?? Guid.NewGuid().ToString();
                tmpPackage.ServiceNo = item.ServiceNo;
                tmpPackage.Head = item.Head?.ToArray();
                string[][] arrbody = null;
                if (item.DataBody.HasItem())
                {
                    arrbody = new string[item.DataBody.Count][];
                    var first = 0;
                    foreach (var item2 in item.DataBody.Values)
                    {
                        arrbody[first] = item2.ToArray();
                        first++;
                    }
                }
                tmpPackage.DataBody = arrbody;
                tmpPackage.SlmOrderBody = item.SlmOrderBody;
                tmpPackage.PushMatchStateBody = item.PushMatchStateBody;
                tmpPackage.PushOrderStateBody = item.PushOrderStateBody;
                tmpPackage.SlmMatchBody = item.SlmMatchBody.ToArray();
                tmpPackage.SendOrderStatusBody = item.SendOrderStatusBody;
                tmpPackage.SlmOrderJourBody = item.SlmOrderJourBody;

                requestPackages.Add(tmpPackage);
            }
            object rp2;
            if (!islog)
            {
                rp2 = isAsyn ? currC.SendPackageListAsyn(requestPackages.ToArray()) : currC.SendPackageList(requestPackages.ToArray());
                packages = null;
                return rp2;
            }
            rp2 = currC.SendLogPackageListAsync(requestPackages.ToArray(), 0);
            packages = null;
            return rp2;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="type"></param>
        /// <param name="isOneWay"></param>
        public void ConnectServer(string clientId, string ip, string port, ClientType type, char isOneWay = 'o', bool iszip = false, int ACM_TIME_OUT = 60)
        {
            c = new Communicator();
            clientType = (int)type;
            if (type == ClientType.QUOT_SERVER)
            {
                c.RegRecQuotEvent(OnRecQuotEventHandler);
                c.RegResponseEvent(OnReceiveResponse);
                c.RegResponseListEvent(OnReceiveListResponse);

                c.Init(clientId, ip, port, 'c', type, QuotConnectionEventHandler, iszip, ACM_TIME_OUT);
            }
            else
            {
                c.RegResponseEvent(OnReceiveResponse);
                c.RegResponseListEvent(OnReceiveListResponse);
                c.RegReceivePackageCacheBaseEvent(OnReceivePackageCacheBase);
                c.RegReceivePackageCacheTradeEvent(OnReceivePackageCacheTrade);
                c.RegReceiveStreamPackageEvent(OnReceiveStreamPackageEvent);
                c.RegReceivePackageCacheTradeListEvent(OnReceivePackageCacheTradeList);
                c.RegReceiveStreamPackageListEvent(OnReceiveStreamListPackageEvent);

                c.RegReceivePackageCacheTradeListAsynCallBackEvent(OnReceivePackageCacheTradeListAsynCallBackEvent);
                c.RegSendPackageListAsynCallBackEvent(ReceivePackageListAsynCallBackEvent);

                c.Init(clientId, ip, port, isOneWay, type, QuotConnectionEventHandler, iszip, ACM_TIME_OUT);

            }
        }



        public void StreamTest()
        {
            Ice.InputStream inS;
            Ice.OutputStream outS;

            bool[] arr = { true, false, true, false };
            outS = new Ice.OutputStream(c.GetCommunicator());
            Ice.BoolSeqHelper.write(outS, arr);
            var data = outS.finished();
            inS = new Ice.InputStream(c.GetCommunicator(), data);
            var arr2 = Ice.BoolSeqHelper.read(inS);

        }
        public byte[] readByteSeq(SlmOrderRow requestPackageOrderLog)
        {
            var instance = GetInstanceOs();
            OutputStream os__ = new OutputStream(instance, instance.defaultsAndOverrides().defaultEncoding);
            SlmCommunicator.SlmOrderRow.ice_write(os__, requestPackageOrderLog);
            var dd = os__.getBuffer();
            return dd.b.rawBytes();
        }
        public SlmOrderRow GetSlmOrderRow(byte[] readByteSeq)
        {
            var instance = GetInstanceOs();
            var os__ = new InputStream(instance, instance.defaultsAndOverrides().defaultEncoding, readByteSeq);
            var result = SlmCommunicator.SlmOrderRow.ice_read(os__);
            return result;
        }

        private IceInternal.Instance GetInstanceOs()
        {
            IceInternal.Instance instance = IceInternal.Util.getInstance(c.GetCommunicator());
            return instance;
        }
        private Ice.EncodingVersion GetEncodingVersion()
        {
            return c.GetObjectPrx().ice_getEncodingVersion();
        }

        /// <summary>
        /// 批量连接服务
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="type"></param>
        public void ConnectServerDic(string clientId, string ip, string port, ClientType type)
        {
            clientType = (int)type;
            var newComm = new Communicator();
            if (type == ClientType.QUOT_SERVER)
            {
                newComm.Init(clientId, ip, port, 'c', type, QuotConnectionEventDicHandler);
                newComm.RegRecQuotEvent(OnRecQuotEventHandler);
                newComm.RegResponseEvent(OnReceiveResponse);
                newComm.RegResponseListEvent(OnReceiveListResponse);
                newComm.RegReceivePackageCacheTradeListEvent(OnReceivePackageCacheTradeList);

            }
            else
            {
                newComm.Init(clientId, ip, port, 'o', type, QuotConnectionEventDicHandler);
                newComm.RegResponseEvent(OnReceiveResponse);
                newComm.RegResponseListEvent(OnReceiveListResponse);
                newComm.RegReceivePackageCacheBaseEvent(OnReceivePackageCacheBase);
                newComm.RegReceivePackageCacheTradeEvent(OnReceivePackageCacheTrade);
                newComm.RegReceivePackageCacheTradeListEvent(OnReceivePackageCacheTradeList);

            }
            CommunicatorDics[clientId] = newComm;
        }

        private void OnRecQuotEventHandler(byte type, NewFlashData[] nfArray, DeepData[] dArray)
        {
            Thread checkThread = new Thread(() =>
            {
                var currTask = Task.Factory.StartNew(() =>
                {
                    var result = new { type, nfArray, dArray };
                    var msg = result.toJsonStr();
                    Console.WriteLine(msg);
                    actionMsg?.Invoke(msg);
                });
            });
            checkThread.IsBackground = true;
            checkThread.Start();
        }
        private void OnReceiveResponse(ResponsePackage responsePackagePackage)
        {
            Thread checkThread = new Thread(() =>
            {
                var currTask = Task.Factory.StartNew(() =>
                {
                    ResponsePackageEx result = new ResponsePackageEx(responsePackagePackage);
                    //QueueresponsePackages.Enqueue(result);
                    var msg = $"收到服务号：{DateTime.Now.toFormatfff()},{responsePackagePackage.ServiceNo},databodysize:{responsePackagePackage.DataBody?.Length}";
                    actionMsg?.Invoke(msg);
                    if (isAutoCheckTime)
                    {
                        string msgid = responsePackagePackage.Head.TryGetArrayByIndex(1);
                        if (msgid.IsNotNullOrWhiteSpace())
                        {
                            MsgIdCheckClientQueue.GetInstance().CallBackAddDataDic(msgid, "ResponsePackage");
                        }
                    }
                    if (isShowAllMsg && !filterServernos.Contains(responsePackagePackage.ServiceNo))
                    {
                        //if (responsePackagePackage.DataBody?.Length > 1000)
                        //{
                        //    actionMsg?.Invoke($"{responsePackagePackage.DataBody.Take(1000).toJsonStr()}");
                        //}
                        //else
                        //{
                        actionMsg?.Invoke($"{responsePackagePackage.toJsonStr()}");
                        //}
                    }
                    responsePackagePackage = null;
                });
            });
            checkThread.IsBackground = true;
            checkThread.Start();

        }
        private int OnReceiveListResponse(ResponsePackageOrder[] responsePackagePackage)
        {
            Thread checkThread = new Thread(() =>
            {
                var currTask = Task.Factory.StartNew(() =>
                {
                    //QueueresponsePackages.Enqueue(result);
                    var msg = $"收到批量包数：{DateTime.Now.toFormatfff()},{responsePackagePackage.Length}";
                    actionMsg?.Invoke(msg);
                    LogHelper.WriteLog("ReceiveList", msg);
                    if (responsePackagePackage.Length > 0 && isAutoCheckTime)
                    {
                        foreach (var item in responsePackagePackage)
                        {
                            MsgIdCheckClientQueue.GetInstance().CallBackAddDataDic(item.MsgId, "ResponsePackageOrder");
                        }
                    }
                    if (isShowAllMsg && !filterServernos.Contains(responsePackagePackage.First().ServiceNo))
                    {
                        GetInfo(responsePackagePackage);
                        msg = responsePackagePackage.toJsonStr();
                        actionMsg?.Invoke($"{msg}");
                        LogHelper.WriteLog("ReceiveList", msg);
                    }
                    responsePackagePackage = null;
                });
            });
            checkThread.IsBackground = true;
            checkThread.Start();
            return 0;
        }
        /// <summary>
        /// SendPackageListAsyn 异步回报结果处理
        /// </summary>
        /// <param name="responsePackagePackage"></param>
        protected virtual void ReceivePackageListAsynCallBackEvent(ResponsePackageOrderBackMsg responsePackagePackage)
        {
            Thread checkThread = new Thread(() =>
            {
                var currTask = Task.Factory.StartNew(() =>
                {
                    //QueueresponsePackages.Enqueue(result);
                    var msg = $"收到批量,{DateTime.Now.toFormatfff()},回报：{responsePackagePackage.toJsonStr()}";
                    actionMsg?.Invoke(msg);
                    LogHelper.WriteLog("ReceiveListCallBack", msg);
                    if (responsePackagePackage.MsgId.IsNotNullOrWhiteSpace() && isAutoCheckTime)
                    {
                        MsgIdCheckClientQueue.GetInstance().CallBackAddDataDic(responsePackagePackage.MsgId, "ResponsePackageOrderBackMsg");
                    }
                    if (isShowAllMsg && !filterServernos.Contains(responsePackagePackage.ServiceNo))
                    {
                        msg = responsePackagePackage.toJsonStr();
                        actionMsg?.Invoke($"{msg}");
                        LogHelper.WriteLog("ReceiveListCallBack", msg);
                    }
                    responsePackagePackage = null;
                });
            });
            checkThread.IsBackground = true;
            checkThread.Start();
            return;
        }
        protected virtual void OnReceivePackageCacheTradeListAsynCallBackEvent(CacheTradeDB responsePackagePackage)
        {
            Thread checkThread = new Thread(() =>
            {
                var currTask = Task.Factory.StartNew(() =>
                {
                    var msg = $"收到{DateTime.Now.toFormatfff()},CacheTradeDB,回报：{responsePackagePackage.toJsonStr()}";
                    actionMsg?.Invoke(msg);
                    LogHelper.WriteLog("ReceiveCacheTradeDBCallBack", msg);
                    if (isAutoCheckTime)
                    {
                        MsgIdCheckClientQueue.GetInstance().CallBackAddDataDic(responsePackagePackage.PackageData.MsgId, "CacheTradeDB");
                    }
                    if (isShowAllMsg && !filterServernos.Contains(responsePackagePackage.PackageData.ServiceNo))
                    {
                        msg = responsePackagePackage.toJsonStr();
                        actionMsg?.Invoke($"{msg}");
                        LogHelper.WriteLog("ReceiveCacheTradeDBCallBack", msg);
                    }
                    responsePackagePackage = null;
                });
            });
            checkThread.IsBackground = true;
            checkThread.Start();
            return;
        }
        public void GetInfo(ResponsePackageOrder[] items)
        {
            foreach (var item in items)
            {
                if (item.SlmOrderBody?.OrderNo <= 0 || item.SlmOrderBody.AccountNo.IsNullOrWhiteSpace())
                {
                    item.SlmOrderBody = null;
                }
                if (item.SlmOrderJourBody?.OrderNo <= 0 || item.SlmOrderJourBody.AccountNo.IsNullOrWhiteSpace())
                {
                    item.SlmOrderJourBody = null;
                }
                if (item.PushMatchStateBody?.OrderNo <= 0 || item.PushMatchStateBody.AccountNo.IsNullOrWhiteSpace())
                {
                    item.PushMatchStateBody = null;
                }
                if (item.PushOrderStateBody?.OrderNo <= 0 || item.PushOrderStateBody.OrderJourNo.IsNullOrWhiteSpace())
                {
                    item.PushOrderStateBody = null;
                }
                if (item.SendOrderStatusBody?.OrderNo <= 0 || item.SendOrderStatusBody.AccountNo.IsNullOrWhiteSpace())
                {
                    item.SendOrderStatusBody = null;
                }
            }
        }
        #region CacheTrade
        private PackageCacheDB OnReceivePackageCacheTradeList(CacheTradeDB[] responsePackagePackage)
        {
            var result = new PackageCacheDB() { ClientId = c.GetClientId(), UUID = c.GetUUID(), ServiceNo = (int)CacheSeverServiceNo.TE_All_DEFAUL, DataBody = new string[1][] { new string[] { "1" } }, ErrorInfo = $"OnReceivePackageCacheTradeList Success Test.PackageClient Length:{responsePackagePackage?.Length}" };
            var msg = $"OnReceivePackageCacheTradeList  Length:{responsePackagePackage?.Length}";
            actionMsg?.Invoke(msg);

            if (isShowAllMsg)
            {
                foreach (var item in responsePackagePackage)
                {
                    OnReceivePackageCacheTradeToDo(item);
                }
            }

            return result;

        }
        public PackageCacheDB OnReceivePackageCacheTradeToDo(CacheTradeDB responsePackagePackage)
        {
            lock (this)
            {
                var result = new PackageCacheDB() { ClientId = c.GetClientId(), UUID = c.GetUUID(), ServiceNo = responsePackagePackage.PackageData.ServiceNo, MsgId = responsePackagePackage.PackageData.MsgId, DataBody = new string[1][] { new string[] { "1" } }, ErrorInfo = "OnReceivePackageCacheTrade Success Test.PackageClient" };
                Thread checkThread = new Thread(() =>
                {
                    var currTask = Task.Factory.StartNew(() =>
                    {

                        var msg = $"OnReceivePackageCacheTrade {DateTime.Now.toFormatfff()},收到服务号：{responsePackagePackage.PackageData.ServiceNo},databodysize:{responsePackagePackage.toJsonStr()?.Length}";
                        actionMsg?.Invoke(msg);
                        if (isAutoCheckTime)
                        {
                            MsgIdCheckClientQueue.GetInstance().CallBackAddDataDic(responsePackagePackage.PackageData.MsgId, "CacheTradeDB");
                        }
                        if (isShowAllMsg && !filterServernos.Contains(responsePackagePackage.PackageData.ServiceNo))
                        {
                            actionMsg?.Invoke($"{responsePackagePackage.toJsonStr()}");
                        }
                        responsePackagePackage = null;
                    });
                });
                checkThread.IsBackground = true;
                checkThread.Start();
                return result;
            }

        }
        private PackageCacheDB OnReceivePackageCacheTrade(CacheTradeDB responsePackagePackage)
        {
            return OnReceivePackageCacheTradeToDo(responsePackagePackage);
        }
        private StreamPackage OnReceiveStreamListPackageEvent(StreamPackage[] param, string msgName)
        {
            Thread checkThread = new Thread(() =>
            {
                var currTask = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var msg = param.toJsonStr();
                        actionMsg?.Invoke($"收到{DateTime.Now.toFormatfff()},StreamPackage合包条数：{param.Length},msgName:{msgName},DataStreamBody:{param[0].DataStreamBody[0].Length},内容大小：{msg.GetBytesOfString(2)}KB");
                        if (isAutoCheckTime)
                        {
                            foreach (var item in param)
                            {
                                MsgIdCheckClientQueue.GetInstance().CallBackAddDataDic(item.MsgId, "StreamPackage[]");
                            }
                        }
                        if (isShowAllMsg)
                        {
                            msg = GetIceObjectFromStream(param).toJsonStr();
                            actionMsg?.Invoke($"收到{DateTime.Now.toFormatfff()},数据：{msg}");
                            LogHelper.WriteLog("onSendStreamPackageList", msg);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        actionMsg?.Invoke($"Size:{param.Length},收到数据成功：处理出错。{ex}");
                        LogHelper.WriteErrorLog(ex);
                    }
                });
            });
            checkThread.IsBackground = true;
            checkThread.Start();
            return new StreamPackage() { ServiceNo = (int)CacheSeverServiceNo.TE_All_DEFAUL, ErrorInfo = "Test.PackageServer onSendStreamPackageList OK Success", DatasDic = param[0].DatasDic, DataStreamBody = param[0].DataStreamBody };
        }
        private StreamPackage OnReceiveStreamPackageEvent(StreamPackage responsePackagePackage)
        {
            return OnOnReceiveStreamPackageToDo(responsePackagePackage);
        }

        private StreamPackage OnOnReceiveStreamPackageToDo(StreamPackage responsePackagePackage)
        {
            lock (this)
            {
                var result = new StreamPackage() { ClientId = c.GetClientId(), UUID = c.GetUUID(), ServiceNo = responsePackagePackage.ServiceNo, MsgId = responsePackagePackage.MsgId, DataBody = new string[1][] { new string[] { "1" } }, ErrorInfo = "OnOnReceiveStreamPackageToDo Success Test.PackageClient" };
                Thread checkThread = new Thread(() =>
                {
                    var currTask = Task.Factory.StartNew(() =>
                    {
                        string msg = string.Empty;
                        try
                        {
                            msg = $"OnOnReceiveStreamPackageToDo 收到{DateTime.Now.toFormatfff()},服务号：{responsePackagePackage.ServiceNo},DataStreamBody:{responsePackagePackage.DataStreamBody?.Length},databodysize:{responsePackagePackage.toJsonStr()?.Length}";

                            actionMsg?.Invoke(msg);

                            if (isAutoCheckTime)
                            {
                                MsgIdCheckClientQueue.GetInstance().CallBackAddDataDic(responsePackagePackage.MsgId, "StreamPackage");
                            }
                            if (isShowAllMsg && !filterServernos.Contains(responsePackagePackage.ServiceNo))
                            {
                                if (responsePackagePackage.ErrorNo == 0)
                                {
                                    msg = GetICEObject(responsePackagePackage).toJsonStr();
                                }
                                else
                                {
                                    msg = responsePackagePackage.toJsonStr();
                                }
                                actionMsg?.Invoke($"{msg}");
                            }
                            responsePackagePackage = null;

                        }
                        catch (System.Exception ex)
                        {
                            actionMsg?.Invoke($"Error:{msg},{ex}");
                        }
                    });
                });
                checkThread.IsBackground = true;
                checkThread.Start();
                return result;
            }
        }
        private List<object> GetIceObjectFromStream(StreamPackage[] streams)
        {
            var tmpObject = new List<object>();
            foreach (var item in streams)
            {
                tmpObject.Add(GetICEObject(item));
            }
            return tmpObject;
        }
        public Tuple<string, object> GetICEObject(StreamPackage stream)
        {
            var baseMsg = stream.toJsonNoByteStr();
            object tmpobj = null;

            var iOs = c.GetInstanceOs();
            var v = c.GetEncodingVersion();
            if (stream.DatasDic.ContainsKey(MsgTypeForLog.PackageOrderType))
            {
                var getType = stream.DatasDic[MsgTypeForLog.PackageOrderType].GetValue<PackageOrderType>();
                tmpobj = getType.GetIceObject(stream.DataStreamBody[0], iOs, v);
                return new Tuple<string, object>(baseMsg, tmpobj);
            }
            else
            {
                return new Tuple<string, object>(baseMsg, "当前对象未处理转换");
            }

            var resunt = new Tuple<string, object>(baseMsg, tmpobj);
            return resunt;
        }

        private PackageCacheDB OnReceivePackageCacheBase(PackageCacheDB responsePackagePackage)
        {
            Thread checkThread = new Thread(() =>
            {
                var currTask = Task.Factory.StartNew(() =>
                {
                    var msg = $"OnReceivePackageCacheBase 收到{DateTime.Now.toFormatfff()},服务号：{responsePackagePackage.ServiceNo},databodysize:{responsePackagePackage.toJsonStr()?.Length}";
                    actionMsg?.Invoke(msg);
                    if (isAutoCheckTime)
                    {
                        MsgIdCheckClientQueue.GetInstance().CallBackAddDataDic(responsePackagePackage.MsgId, "PackageCacheDB");
                    }
                    if (isShowAllMsg)
                    {
                        actionMsg?.Invoke($"{responsePackagePackage.toJsonStr()}");
                    }
                    responsePackagePackage = null;
                });
            });
            checkThread.IsBackground = true;
            checkThread.Start();
            var result = new PackageCacheDB() { ClientId = c.GetClientId(), UUID = c.GetUUID(), ServiceNo = responsePackagePackage.ServiceNo, MsgId = responsePackagePackage.MsgId, DataBody = new string[1][] { new string[] { "1" } }, ErrorInfo = "OnReceivePackageCacheBase Success Test.PackageClient" };
            return result;
        }
        #endregion

        private StreamPackage OnReceiveStreamPackage(StreamPackage responsePackagePackage)
        {
            throw new NotImplementedException();
        }
        public void StartToQueueToDo()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        if (QueueresponsePackages.Count <= 0)
                        {
                            Thread.Sleep(10);
                            continue;
                        }

                        var toGet = QueueresponsePackages.Dequeue();
                        if (toGet == null)
                        {
                            Thread.Sleep(10);
                            continue;
                        }

                        var msg = toGet.toJsonStr();
                        Console.WriteLine(msg);
                        //actionMsg?.Invoke(msg);

                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        //actionMsg?.Invoke(ex.Message);
                        Thread.Sleep(10);
                    }
                }
            });
        }
        private void QuotConnectionEventHandler(string uuid, ConnectState state)
        {
            Console.WriteLine("QuotConnectionEventHandler");
            Console.WriteLine(state);
            _state = state;
            ConnectAction?.Invoke(state);

        }
        private void QuotConnectionEventDicHandler(string uuid, ConnectState state)
        {
            Console.WriteLine("QuotConnectionEventHandler");
            Console.WriteLine(state);
            ConnectAction?.Invoke(state);
        }

        public void DisconnectServer(bool isAllDic = false)
        {

            if (isAllDic)
            {
                foreach (var item in CommunicatorDics)
                {
                    item.Value.Dispose();
                }
            }
            else
            {
                if (c != null)
                {
                    c.Dispose();
                }
                _state = ConnectState.CONNECT_FAIL;

            }
            ConnectAction = null;
        }
        /// <summary>
        /// 生成流文件
        /// </summary>
        /// <param name="fileName"></param>
        public void GenBinStream(string fileName = "TestStreamBin.bin")
        {
            if (c == null)
            {
                c = new Communicator();
            }

            var iOs = c.GetInstanceOs();
            var v = c.GetEncodingVersion();

            var orderbin = new SlmOrderRow() { MsgNum = addNumDic[1] * 10, AccountNo = "Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001", RemarkCus = DateTime.Now.toFormatfff() };
            var tradebin = new SlmMatchRow() { MsgNum = addNumDic[1] * 10, AccountNo = "Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001", Info1 = DateTime.Now.toFormatfff() };

            StreamBinFilePackage[] saveBin = new StreamBinFilePackage[2];
            saveBin[0] = new StreamBinFilePackage() { PackageOrderType = (byte)PackageOrderType.SlmOrderBody, DataStreamBody = orderbin.readByteSeq(iOs, v) };
            saveBin[1] = new StreamBinFilePackage() { PackageOrderType = (byte)PackageOrderType.SlmMatchBody, DataStreamBody = tradebin.readByteSeq(iOs, v) };


            byte[] getBinToSave = saveBin[0].readByteSeq(iOs, v).Concat(saveBin[1].readByteSeq(iOs, v)).ToArray();

            string filePath = System.Environment.CurrentDirectory + "\\" + fileName;
            getBinToSave.SaveLogFile(filePath);
            actionMsg?.Invoke($"生成流文件:{filePath}: {saveBin.Length}");

        }

        /// <summary>
        /// 生成日志流文件
        /// </summary>
        /// <param name="fileName"></param>
        public void GenBinStreamLog(string fileName = "TestStreamBinLog")
        {
            if (c == null)
            {
                c = new Communicator();
            }

            var iOs = c.GetInstanceOs();
            var v = c.GetEncodingVersion();

            var orderbin = new SlmOrderRow() { MsgNum = addNumDic[1] * 10, AccountNo = "Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001Test001", RemarkCus = DateTime.Now.toFormatfff() };


            byte[] getBinToSave = orderbin.readByteSeq(iOs, v);

            LogHelper.GetInstance().WriteOneBytelog(getBinToSave, fileName, PackageOrderType.SlmOrderBody);

            actionMsg?.Invoke($"生成流文件:{fileName}: {getBinToSave.Length}");

        }


        /// <summary>
        /// 解码流文件
        /// </summary>
        /// <param name="fileName"></param>
        public void GetBinStream(string fileName = "TestStreamBin.bin", bool isUseOne = true)
        {
            if (c == null)
            {
                c = new Communicator();
            }
            var iOs = c.GetInstanceOs();
            var v = c.GetEncodingVersion();

            var tmpByte = fileName.GetbytesByFilePath();

            List<object> list = new List<object>();
            if (isUseOne)
            {
                int offset = 0;
                while (offset < tmpByte.Length)
                {
                    var getDataStreamBodySize = GetSize(tmpByte.GetCopyArray(offset + 1, 2));
                    var getNextLength = getDataStreamBodySize.Item1 ? getDataStreamBodySize.Item2 + 1 + 1 : getDataStreamBodySize.Item2 + 1 + 2;
                    var getBin = tmpByte.GetCopyArray(offset, getNextLength).GetStreamBinFilePackage(iOs, v);
                    var orderPackageOrderType = (PackageOrderType)getBin.PackageOrderType;
                    var iceObject = orderPackageOrderType.GetIceObject(getBin.DataStreamBody, iOs, v);
                    list.Add(iceObject);

                    offset += (getNextLength);
                    actionMsg?.Invoke($"当前包：{orderPackageOrderType} offset:{offset}, size:{getBin.DataStreamBody.Length + 2}");
                }
                actionMsg?.Invoke($"解码流文件:{fileName}: {list.Count}");
                actionMsg?.Invoke(list.toJsonStr());
                list.Clear();
                return;
            }
            //解法2
            //var treamloglist = tmpByte.GetStreamBinFilePackageLogBin(iOs, v);
            //foreach (var getBin in treamloglist)
            //{
            //    var orderPackageOrderType = (PackageOrderType)getBin.PackageOrderType;
            //    var iceObject = GetIceObject(orderPackageOrderType, getBin.DataStreamBody);
            //    list.Add(iceObject);
            //}

            //解法3
            var treamloglist = tmpByte.GetStreamPackageFromLogBin(iOs, v);
            foreach (var getBin in treamloglist)
            {
                var orderPackageOrderType = (PackageOrderType)getBin.Key;
                foreach (var item in getBin.Value)
                {
                    var iceObject = orderPackageOrderType.GetIceObject(item, iOs, v);
                    list.Add(iceObject);
                }

            }

            actionMsg?.Invoke($"解码流文件2:{fileName}: {list.Count}");
            actionMsg?.Invoke(list.toJsonStr());
        }
        /// <summary>
        /// isbyte +1
        /// isInt +2
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        public Tuple<bool, int> GetSize(byte[] head)
        {
            try
            {
                byte b = System.Buffer.GetByte(head.Take(1).ToArray(), 0); ;
                if (b == byte.MaxValue)
                {
                    int @int = BitConverter.ToInt32(head.Take(2).ToArray(), 0);
                    if (@int < 0)
                    {
                        throw new UnmarshalOutOfBoundsException();
                    }

                    return new Tuple<bool, int>(false, @int);
                }

                return new Tuple<bool, int>(true, b);
            }
            catch (InvalidOperationException ex__)
            {
                throw new UnmarshalOutOfBoundsException(ex__);
            }

        }
    }

    #region other class
    /// <summary>
    /// 入参数类
    /// </summary>
    public class PackageRequestDTO : ComDto
    {

        public string ServiceName { get; set; }
        public string ClientId { get; set; }
        public string UUId { get; set; }
        public List<string> Head { get; set; }
        public Dictionary<int, List<string>> DataBody { get; set; }
        public SlmOrderRow SlmOrderBody { get; set; }
        public PushMatchStateRow PushMatchStateBody { get; set; }
        public PushOrderStateRow PushOrderStateBody { get; set; }
        public SendOrderStatusRow SendOrderStatusBody { get; set; }
        public SlmOrderJourRow SlmOrderJourBody { get; set; }
        public List<SlmMatchRow> SlmMatchBody { get; set; }
        public PackageRequestDTO Copy()
        {
            var listSlms = new List<SlmMatchRow>();
            if (this.SlmMatchBody != null)
            {
                foreach (var item in this.SlmMatchBody)
                {
                    listSlms.Add((SlmMatchRow)item.Clone());
                }
            }
            var result = new PackageRequestDTO()
            {
                ClientId = this.ClientId,
                UUId = this.UUId,
                DataBody = this.DataBody,
                Head = this.Head,
                ServiceName = this.ServiceName,
                MsgId = this.MsgId ?? Guid.NewGuid().ToString(),
                ServiceNo = this.ServiceNo,
                SlmOrderBody = (SlmOrderRow)this.SlmOrderBody?.Clone(),
                PushMatchStateBody = (PushMatchStateRow)this.PushMatchStateBody?.Clone(),
                PushOrderStateBody = (PushOrderStateRow)this.PushOrderStateBody?.Clone(),
                SendOrderStatusBody = (SendOrderStatusRow)this.SendOrderStatusBody?.Clone(),
                SlmOrderJourBody = (SlmOrderJourRow)this.SlmOrderJourBody?.Clone(),
                SlmMatchBody = listSlms
            };
            return result;
        }

    }
    public class SendCommData
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
    #endregion
}
