
using Ice;
using SlmCommunicator;
using SlmCommunicator.Client;
using Solomon.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Test.PackageClient.Exts;
using Communicator = SlmCommunicator.Client.CommunicatorClientHelper;
using ConnectState = SlmCommunicator.Client.ConnectStateClient;

namespace Test.PackageClient
{
    /// <summary>
    /// 入参数类
    /// </summary>
    public class PackageRequestDTO : ComDto
    {
        public string MsgId { get; set; }
        public string ServiceName { get; set; }
        public string ClientId { get; set; }
        public string UUId { get; set; }
        public List<string> Head { get; set; }
        public List<string> DataBody { get; set; }
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
    public class initClient
    {
        #region 参数 
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

        #endregion

        public initClient()
        {
            //StartToQueueToDo();
        }
        ~initClient() { }

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

            if (isAsyn)
            {
                bool rp2 = currC.SendPackageAsyn(package.ServiceNo, package.Head?.ToArray(), package.DataBody?.ToArray());
                return rp2;
            }
            ResponsePackage rp = currC.SendPackage(package.ServiceNo, package.Head?.ToArray(), package.DataBody?.ToArray()).PackageData;
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

        public object SendRequest(ComDto package, string keyCommunicator = "", bool isAsyn = false, Tuple<bool, int, int> isAdd = null)
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
            pushFData.Type = type;
            //实体类发送类
            RequestPackage pushData = new RequestPackage();
            pushData.UUID = uuid;
            pushData.ClientId = curr.ClientId;
            pushData.ServiceNo = curr.ServiceNo;
            pushData.Head = curr.Head?.ToArray();
            pushData.DataBody = curr.DataBody?.ToArray();

            pushFData.PushData = pushData;

            return pushFData;
        }
        private SendCommData UpdateData(PackageRequestDTO curr, int type, string uuid = "")
        {
            var pushFData = new SendCommData();
            pushFData.DtoData.MType = curr.MType;
            pushFData.Type = type;
            //实体类发送类
            RequestPackageOrder pushData = new RequestPackageOrder();
            pushData.UUID = uuid;
            pushData.ClientId = curr.ClientId;
            pushData.MsgId = curr.MsgId ?? Guid.NewGuid().ToString();
            pushData.ServiceNo = curr.ServiceNo;
            pushData.Head = curr.Head?.ToArray();
            var arrbody = new string[1][] { curr.DataBody?.ToArray() };
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
            if (!curr.DataBody.HasItem())
            {
                return pushFData;
            }
            pushFData.DtoData.MType = curr.MType;
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
        private SendCommData UpdateData(CacheTradeDBDto model, int type, string uuid = "")
        {
            var pushFData = new SendCommData();
            var curr = model.PackageData;
            if (!curr.DataBody.HasItem())
            {
                return pushFData;
            }
            pushFData.DtoData.MType = model.MType;
            pushFData.ServiceNo = curr.ServiceNo;
            pushFData.Type = type;
            //
            CacheTradeDB pushData = model.Copy();
            //
            pushData.PackageData.UUID = uuid;

            pushFData.PushData = pushData;

            return pushFData;
        }
        private object SendPackage(SendCommData data, Communicator currC, Tuple<bool, int, int> isAdd = null)
        {
            actionMsg?.Invoke($"发送数据：{data.toJsonStr()}");
            switch (data.DtoData.MType)
            {
                case emMType.Base:
                    var currSend = data.PushData as RequestPackage;
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
        public void BatchAddToSend(RequestPackageOrder pushData, Communicator currC, Tuple<bool, int, int> isAdd)
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
            actionMsg?.Invoke($"RequestPackageOrder,主键是否递增：{isAdd.Item1},发送次数：{isAdd.Item2},合包数：{isAdd.Item3}");

            int addNum = 1;
            for (int i = 0; i < isAdd.Item2; i++)
            {
                RequestPackageOrder[] requestPackageOrders = new RequestPackageOrder[isAdd.Item3];
                for (int j = 0; j < isAdd.Item3; j++)
                {
                    requestPackageOrders[j] = pushData.Copy(isAdd.Item1, addNumDic);
                    addNum++;
                }
                currC.SendPackageListAsyn(requestPackageOrders);
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
        public void BatchAddToSend(CacheTradeDB pushData, Communicator currC, Tuple<bool, int, int> isAdd)
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
                tmpPackage.DataBody = new string[1][] { item.DataBody?.ToArray() };
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
        public void ConnectServer(string clientId, string ip, string port, ClientType type, char isOneWay = 'o', bool iszip = false)
        {
            c = new Communicator();
            clientType = (int)type;
            if (type == ClientType.QUOT_SERVER)
            {
                c.RegRecQuotEvent(OnRecQuotEventHandler);
                c.RegResponseEvent(OnReceiveResponse);
                c.RegResponseListEvent(OnReceiveListResponse);

                c.Init(clientId, ip, port, 'c', type, QuotConnectionEventHandler, iszip);
            }
            else
            {
                c.RegResponseEvent(OnReceiveResponse);
                c.RegResponseListEvent(OnReceiveListResponse);
                c.RegReceivePackageCacheBaseEvent(OnReceivePackageCacheBase);
                c.RegReceivePackageCacheTradeEvent(OnReceivePackageCacheTrade);

                c.Init(clientId, ip, port, isOneWay, type, QuotConnectionEventHandler, iszip);

            }
        }
        //public void StreamTest()
        //{
        //    Ice.InputStream inS;
        //    Ice.OutputStream outS;

        //    bool[] arr = { true, false, true, false };
        //    outS = new Ice.OutputStreamI(c.GetCommunicator());
        //    Ice.BoolSeqHelper.write(outS, arr);
        //    var data = outS.finished();
        //    inS = new Ice.InputStreamI(c.GetCommunicator(), data, false);
        //    var arr2 = Ice.BoolSeqHelper.read(inS);

        //}
        //public byte[] readByteSeq(SlmOrderRow requestPackageOrderLog)
        //{
        //    var instance = GetInstanceOs();
        //    IceInternal.BasicStream os__ = new IceInternal.BasicStream(instance, instance.defaultsAndOverrides().defaultEncoding);
        //    SlmCommunicator.SlmOrderRow.write__(os__, requestPackageOrderLog);
        //    var dd = os__.getBuffer();
        //    return dd.b.rawBytes();
        //}
        //public SlmOrderRow GetSlmOrderRow(byte[] readByteSeq)
        //{
        //    var result = new SlmOrderRow();
        //    var instance = GetInstanceOs();
        //    IceInternal.BasicStream os__ = new IceInternal.BasicStream(instance, instance.defaultsAndOverrides().defaultEncoding, readByteSeq);
        //    SlmCommunicator.SlmOrderRow.read__(os__, result);
        //    return result;
        //}

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

            }
            else
            {
                newComm.Init(clientId, ip, port, 'o', type, QuotConnectionEventDicHandler);
                newComm.RegResponseEvent(OnReceiveResponse);
                newComm.RegResponseListEvent(OnReceiveListResponse);
                newComm.RegReceivePackageCacheBaseEvent(OnReceivePackageCacheBase);
                newComm.RegReceivePackageCacheTradeEvent(OnReceivePackageCacheTrade);

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
                    var msg = $"收到服务号：{responsePackagePackage.ServiceNo},databodysize:{responsePackagePackage.DataBody?.Length}";
                    actionMsg?.Invoke(msg);
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
                    var msg = $"收到批量包数：{responsePackagePackage.Length}";
                    actionMsg?.Invoke(msg);
                    LogHelper.WriteLog("ReceiveList", msg);
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
        private PackageCacheDB OnReceivePackageCacheTrade(CacheTradeDB responsePackagePackage)
        {
            var result = new PackageCacheDB() { ClientId = c.GetClientId(), UUID = c.GetUUID(), ServiceNo = responsePackagePackage.PackageData.ServiceNo, MsgId = responsePackagePackage.PackageData.MsgId, DataBody = new string[1][] { new string[] { "1" } }, ErrorInfo = "OnReceivePackageCacheTrade Success Test.PackageClient" };
            Thread checkThread = new Thread(() =>
            {
                var currTask = Task.Factory.StartNew(() =>
                {

                    var msg = $"OnReceivePackageCacheTrade 收到服务号：{responsePackagePackage.PackageData.ServiceNo},databodysize:{responsePackagePackage.toJsonStr()?.Length}";
                    actionMsg?.Invoke(msg);
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

        private PackageCacheDB OnReceivePackageCacheBase(PackageCacheDB responsePackagePackage)
        {
            Thread checkThread = new Thread(() =>
            {
                var currTask = Task.Factory.StartNew(() =>
                {
                    var msg = $"OnReceivePackageCacheBase 收到服务号：{responsePackagePackage.ServiceNo},databodysize:{responsePackagePackage.toJsonStr()?.Length}";
                    actionMsg?.Invoke(msg);
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


    }
}
