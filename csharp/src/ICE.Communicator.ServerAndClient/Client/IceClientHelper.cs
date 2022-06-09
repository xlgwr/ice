
using Ice;
using Solomon.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Exception = Ice.Exception;


namespace SlmCommunicator.Client
{
    public class ConnectionClientEvent
    {
        private EventClientHelper _eventHelper;
        public string _uuid { get; set; }

        public EventClientHelper EventObj
        {
            get { return _eventHelper; }
        }

        public ConnectionClientEvent(EventClientHelper eventHelper)
        {
            _eventHelper = eventHelper;
        }

        public void closed(Connection con)
        {
            LogHelper.WriteLog("HeartBeat", "Client Closed: " + con.ice_toString_());
            _eventHelper.OnConnectionEvent(_uuid, ConnectStateClient.CONNECT_FAIL);

        }

        public void heartbeat(Connection con)
        {
            LogHelper.WriteLog("HeartBeat", "Client: " + con.getInfo().toJsonStr());

        }
    }

    class IceClient
    {
        #region 属性变量
        private Ice.Communicator _communicator = null;
        private Ice.ObjectPrx prx = null;
        private PackageSenderPrx _server;
        private PackageSenderPrx _serverOne;
        private Ice.ObjectAdapter _clientServer;
        private bool _isReady = false;
        private string _clientId = string.Empty;
        private string _ip = string.Empty;
        private string _port = string.Empty;
        private bool NeedZip = false;

        //t：twoway，两条tcp通道，服务端和客户端发送各占用一条通道
        //o:  oneway,  服务端和客户端发送都是一条tcp通道
        //p:是批量发送的
        //c:是udp协议
        private char _mode = 'o'; //one way or two way
        private string _uuid = string.Empty;
        private ConnectionClientEvent _event = null;
        private EventClientHelper _eventHelper;
        public int ACM_TIME_OUT = 60; //60秒 //ACM心跳间隔
        private int CONN_TIME_OUT { get; set; } = 2000; //2秒 //连接超时
        private int INVOCAION_TIME_OUT { get; set; } = 10 * 1000; //10秒 //调用超时
        /// <summary>
        /// 是否启用连接成功后的重连机制
        /// </summary>
        private bool _isAutoConnect = true;
        private bool _isNormalClose = false;
        private bool _stopConnect = false;
        private PackageReceiverPrx _receiver;
        private Thread _heartBeatThread;
        private string _hostIp = string.Empty;
        private string _proxyStr = string.Empty;
        private ClientType _type = ClientType.UNKNOW;
        /// <summary>
        /// 是否启用连接成功前的重连机制
        /// </summary>
        private bool _serverReConnect = true;
        private int errorAll = 3;
        #region SSL
        private bool _enableSSL = false;
        private string _sslPath = string.Empty;
        private string _pemName = string.Empty;
        private string _p12Name = string.Empty;
        private string _sslPass = string.Empty;
        private string _dllName = "IceSSL";
        #endregion

        #endregion
        #region 回调方法
        /// <summary>
        /// 用于输出日志显示
        /// </summary>
        public Action<string, string, int> LogMsgAction { get; set; }
        #endregion
        #region 构造方法
        public IceClient(char mode, EventClientHelper eventHelper)
        {
            _mode = mode;
            if (_mode == 't')
            {
                _isAutoConnect = true;
            }

            _eventHelper = eventHelper;
            _event = new ConnectionClientEvent(_eventHelper);
            _eventHelper.RegConnectionEvent(connectionChange);
        }

        public IceClient(char mode, ClientType type, EventClientHelper eventHelper)
        {
            if (type == ClientType.SETTLE_CLIENT)
            {
                INVOCAION_TIME_OUT = INVOCAION_TIME_OUT * 1000;
                CONN_TIME_OUT = CONN_TIME_OUT * 100;
            }
            if (type == ClientType.CONTER_CLIENT)
            {
                INVOCAION_TIME_OUT = INVOCAION_TIME_OUT * 100;
                SetNeedZip(true);
            }
            if (type == ClientType.TRADE_CLIENT)
            {
                INVOCAION_TIME_OUT = INVOCAION_TIME_OUT * 25;
                CONN_TIME_OUT = CONN_TIME_OUT * 5;
                SetNeedZip(true);
            }
            if (type == ClientType.CONTER_CLIENT)
            {
                INVOCAION_TIME_OUT = INVOCAION_TIME_OUT * 100;
            }
            if (type == ClientType.RISK_CLIENT)
            {
                INVOCAION_TIME_OUT = INVOCAION_TIME_OUT * 6;
                CONN_TIME_OUT = CONN_TIME_OUT * 5;
            }

            if (type == ClientType.TRADE_SERVER)
            {
                INVOCAION_TIME_OUT = INVOCAION_TIME_OUT * 50;
                CONN_TIME_OUT = CONN_TIME_OUT * 5;
            }
            _mode = mode;
            _type = type;
            if (_mode == 't')
            {
                _isAutoConnect = true;
            }
            _eventHelper = eventHelper;
            _event = new ConnectionClientEvent(_eventHelper);
            _eventHelper.RegConnectionEvent(connectionChange);
        }

        ~IceClient()
        {

        }
        #endregion

        #region 私有方法
        private string getHostIp()
        {

            ///获取本地的IP地址
            string AddressIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }

            return AddressIP;
        }
        public string GetIPStr()
        {
            return $"{_clientId} {_uuid} {_ip}:{_port} CONN_TIME_OUT:{CONN_TIME_OUT}ms INVOCAION_TIME_OUT:{INVOCAION_TIME_OUT}ms NeedZip:{NeedZip} ACM_TIME_OUT:{ACM_TIME_OUT}";
        }
        private void workThread()
        {
            try
            {
                ACM_TIME_OUT = ACM_TIME_OUT <= 0 ? 60 : ACM_TIME_OUT;
#if DEBUG
                //CONN_TIME_OUT = 300 * 60 * 1000;//30分
                //INVOCAION_TIME_OUT = 30 * 60 * 1000;//30分
#endif
                Ice.InitializationData initData = new Ice.InitializationData();
                initData.properties = Ice.Util.createProperties();
                initData.properties.setProperty("Ice.ACM.Client.Close", "4");
                initData.properties.setProperty("Ice.ACM.Client.Heartbeat", "3");
                initData.properties.setProperty("Ice.ACM.Client.Timeout", ACM_TIME_OUT.ToString());
                initData.properties.setProperty("Ice.ACM.Server.Close", "4");
                initData.properties.setProperty("Ice.ACM.Server.Heartbeat", "3");
                initData.properties.setProperty("Ice.ACM.Server.Timeout", ACM_TIME_OUT.ToString());
                initData.properties.setProperty("Ice.RetryIntervals", "-1");
                initData.properties.setProperty("Ice.MessageSizeMax", "2097152");//最大2GB 2097152KB
                //initData.properties.setProperty("Ice.Override.Timeout", "500000");

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
                    initData.properties.setProperty("Ice.Plugin.IceSSL", _dllName + ":IceSSL.PluginFactory");
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
                    catch (Ice.Exception ex)
                    {
                        LogHelper.Error(ex);
                    }
                };

                _communicator = Ice.Util.initialize(initData);
                if (_communicator == null)
                {
                    LogHelper.Warn($"workThread: initialize error {GetIPStr()}");
                    _eventHelper.OnConnectionEvent(_uuid, ConnectStateClient.CONNECT_FAIL);
                    _isReady = false;
                    return;
                }

                var ipPattern = "((25[0-5]|2[0-4]\\d|((1\\d{2})|([1-9]?\\d))).){3}(25[0-5]|2[0-4]\\d|((1\\d{2})|([1-9]?\\d)))";
                if (Regex.IsMatch(_ip, ipPattern) == false
                    || Regex.IsMatch(_port, "^[1-9]\\d*$") == false)
                {
                    LogHelper.Error($"无效的配置 ip地址:【{_ip}】,服务端口:【{_port}】");
                    return;
                }
                string NeedZipFlag = string.Empty;
                if (NeedZip)
                {
                    NeedZipFlag = " -z";
                }
                if (_enableSSL)
                {
                    _proxyStr = string.Format("packageSender:ssl -h {0} -p {1}{2}", _ip, _port, NeedZipFlag);
                }
                else
                {
                    _proxyStr = string.Format("packageSender:tcp -h {0} -p {1}{2}", _ip, _port, NeedZipFlag);
                }

                prx = _communicator.stringToProxy(_proxyStr).ice_timeout(CONN_TIME_OUT).ice_invocationTimeout(INVOCAION_TIME_OUT).ice_twoway().ice_secure(false);
                if (_enableSSL)
                {
                    prx = prx.ice_secure(true);
                }

                //建立第一个服务端的代理
                while (!connectServer(prx))
                {
                    if (_stopConnect)
                    {
                        return;
                    }
                    _eventHelper.OnConnectionEvent(_uuid, ConnectStateClient.CONNECT_FAIL);
                    _isReady = false;
                    if (!_serverReConnect) //是否启用重连
                    {
                        return;
                    }
                    Thread.Sleep(3000);
                }

                if (_communicator == null)
                {
                    LogHelper.Warn($"workThread: _communicator is null {GetIPStr()}");
                    _eventHelper.OnConnectionEvent(_uuid, ConnectStateClient.CONNECT_FAIL);
                    _isReady = false;
                    return;
                }

                //_server.ice_getConnection().setACM(30, Ice.ACMClose.CloseOff, Ice.ACMHeartbeat.HeartbeatAlways);

                if (_mode == 't')
                {
                    string endPointStr = string.Format("tcp -h {0}", getHostIp());
                    Ice.ObjectAdapter adapter = _communicator.createObjectAdapterWithEndpoints("PackageReceive", endPointStr);
                    adapter.add(new PackageReceiveClientI(_event), _communicator.stringToIdentity("packageReceive"));
                    adapter.activate();
                    //建立客户端的服务
                    _receiver = PackageReceiverPrxHelper.uncheckedCast(adapter.createProxy(_communicator.stringToIdentity("packageReceive")).ice_timeout(CONN_TIME_OUT).ice_invocationTimeout(INVOCAION_TIME_OUT).ice_twoway().ice_secure(false));
                    Ice.Identity id = new Identity();
                    _uuid = Guid.NewGuid().ToString("N");
                    id.name = _uuid;
                    id.category = _clientId;
                    _server.RegClientExt(_receiver, id, (int)_type);
                }
                else if (_mode == 'o')
                {
                    //启动客户端Proxy
                    _clientServer = _communicator.createObjectAdapter("");
                    Ice.Identity ident = new Ice.Identity();
                    _uuid = Guid.NewGuid().ToString("N");
                    ident.name = _uuid;
                    ident.category = _clientId;
                    _event._uuid = _uuid;
                    _clientServer.add(new PackageReceiveClientI(_event), ident);
                    _clientServer.activate();
                    _server.ice_getConnection().setAdapter(_clientServer);
                    //设置连接消息回调
                    _server.ice_getConnection().setCallback(_event);
                    _server.RegClient(ident, (int)_type);
                }
                else if (_mode == 'p')
                {
                    //新建一个oneway的连接
                    Ice.ObjectPrx prxOne = _communicator.stringToProxy(_proxyStr).ice_timeout(CONN_TIME_OUT).ice_invocationTimeout(INVOCAION_TIME_OUT).ice_twoway().ice_secure(false);
                    _serverOne = PackageSenderPrxHelper.checkedCast(prxOne);
                    //启动客户端Proxy
                    ObjectAdapter _clientServerOne = _communicator.createObjectAdapter("");
                    Ice.Identity ident = new Ice.Identity();
                    _uuid = Guid.NewGuid().ToString("N");
                    ident.name = _uuid;
                    ident.category = _clientId;
                    _event._uuid = _uuid;
                    _clientServerOne.add(new PackageReceiveClientI(_event), ident);
                    _clientServerOne.activate();
                    _serverOne.ice_getConnection().setAdapter(_clientServerOne);
                    _serverOne.RegClient(ident, (int)_type);
                }
                else if (_mode == 'c') //交易客户端行情用
                {
                    //启动客户端Proxy
                    _clientServer = _communicator.createObjectAdapter("");
                    //建立主身份信息
                    Ice.Identity ident = new Ice.Identity();
                    _uuid = Guid.NewGuid().ToString("N");
                    ident.name = _uuid;
                    ident.category = _clientId;
                    //建立行情身份信息
                    Ice.Identity quotIdent = new Ice.Identity();
                    quotIdent.name = Guid.NewGuid().ToString("N");
                    //加入身份到Adapter
                    _event._uuid = _uuid;
                    _clientServer.add(new PackageReceiveClientI(_event), ident);
                    _clientServer.add(new QuotPackageI(_event), quotIdent);
                    _clientServer.activate();
                    _server.ice_getConnection().setAdapter(_clientServer);
                    //设置连接消息回调
                    _server.ice_getConnection().setCallback(_event);
                    _server.RegQuotClient(ident, quotIdent, (int)_type);
                }

                LogHelper.WriteSystemLog($"Client ICE:{GetIPStr()}");
                //注册连接回调
                //_server.ice_getConnection().setCallback(_event);
                //发送连接成功的消息
                _eventHelper.OnConnectionEvent(_uuid, ConnectStateClient.CONNECT_OK);
                _isReady = true;
                _communicator?.waitForShutdown();
                _communicator?.Dispose();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "workThread");
                _isReady = false;
                _eventHelper.OnConnectionEvent(_uuid, ConnectStateClient.CONNECT_FAIL);
            }
        }

        private bool connectServer(Ice.ObjectPrx prx)
        {
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                _server = PackageSenderPrxHelper.checkedCast(prx);
                return true;
            }
            catch (Exception ex)
            {
                //最多打3次错误
                if (errorAll > 0)
                {
                    stopwatch.Stop();
                    LogHelper.Warn($"workThread: checkedCast {GetIPStr()},UseTime:{stopwatch.ElapsedMilliseconds}ms", "connectServer", "", ex);
                    errorAll--;
                }
            }
            finally
            {
                stopwatch.Stop();
            }
            return false;
        }

        private RequestPackage packageData(int serviceNo, string[] head, string[] data)
        {
            RequestPackage reqData = new RequestPackage();
            reqData.ServiceNo = serviceNo;
            reqData.Head = head;
            reqData.DataBody = data;
            reqData.ClientId = _clientId;
            reqData.UUID = _uuid;
            return reqData;
        }

        private RequestPackage packageData(string clientId, int serviceNo, string[] head, string[] data)
        {
            RequestPackage reqData = new RequestPackage();
            reqData.ServiceNo = serviceNo;
            reqData.Head = head;
            reqData.DataBody = data;
            reqData.ClientId = clientId;
            reqData.UUID = _uuid;
            return reqData;
        }

        private void connectionChange(string uuid, ConnectStateClient state)
        {
            //判断是否启动心跳线程
            if (_server != null && _isAutoConnect && state == ConnectStateClient.CONNECT_FAIL && !_isNormalClose)
            {
                _stopConnect = false;
                _heartBeatThread = new Thread(heartBeatProcess);
                _heartBeatThread.IsBackground = true;
                _heartBeatThread.Start();
            }
            //成功后,激活自动重联发送投票功能
            if (state == ConnectStateClient.CONNECT_OK)
            {
                isRequestStartVote = true;
            }

        }
        #region 投票功能
        /// <summary>
        /// 连接成功后,第一次断线重连时,失败后,调用投票功能
        /// </summary>
        private bool isRequestStartVote = false;
        #endregion

        private void heartBeatProcess()
        {
            int err = 0;
            while (!_stopConnect)
            {
                try
                {
                    LogMsgAction?.Invoke("heartBeatProcess", $"开始断线重连:{GetIPStr()}", 1);
                    _server.ice_ping();
                    if (this._isNormalClose)
                    {
                        LogMsgAction?.Invoke("heartBeatProcess", $"_isNormalClose is True,无需自动重连!{GetIPStr()}", 1);
                        return;
                    }
                    if (_mode == 't')//两条tcp
                    {
                        Ice.Identity id = new Identity();
                        id.name = _uuid;
                        id.category = _clientId;
                        err = _server.RegClientExt(_receiver, id, (int)_type);
                        //注册连接回调
                        _event._uuid = _uuid;
                        _server.ice_getConnection().setCallback(_event);
                    }
                    else if (_mode == 'o')//一条tcp
                    {
                        //启动客户端Proxy
                        _clientServer = _communicator.createObjectAdapter("");
                        Ice.Identity ident = new Ice.Identity();
                        ident.name = _uuid;
                        ident.category = _clientId;
                        _event._uuid = _uuid;
                        _clientServer.add(new PackageReceiveClientI(_event), ident);
                        _clientServer.activate();
                        _server.ice_getConnection().setAdapter(_clientServer);
                        //设置连接消息回调
                        _server.ice_getConnection().setCallback(_event);
                        err = _server.RegClient(ident, (int)_type);
                    }
                    else if (_mode == 'p')
                    {
                        //新建一个oneway的连接
                        Ice.ObjectPrx prxOne = _communicator.stringToProxy(_proxyStr).ice_timeout(CONN_TIME_OUT).ice_invocationTimeout(INVOCAION_TIME_OUT).ice_twoway().ice_secure(false);
                        _serverOne = PackageSenderPrxHelper.checkedCast(prxOne);
                        //启动客户端Proxy
                        ObjectAdapter _clientServerOne = _communicator.createObjectAdapter("");
                        Ice.Identity ident = new Ice.Identity();
                        _uuid = Guid.NewGuid().ToString("N");
                        ident.name = _uuid;
                        ident.category = _clientId;
                        _event._uuid = _uuid;
                        _clientServerOne.add(new PackageReceiveClientI(_event), ident);
                        _clientServerOne.activate();
                        _serverOne.ice_getConnection().setAdapter(_clientServerOne);
                        err = _serverOne.RegClient(ident, (int)_type);
                    }
                    else if (_mode == 'c') //交易客户端行情用
                    {
                        //启动客户端Proxy
                        _clientServer = _communicator.createObjectAdapter("");
                        //建立主身份信息
                        Ice.Identity ident = new Ice.Identity();
                        _uuid = Guid.NewGuid().ToString("N");
                        ident.name = _uuid;
                        ident.category = _clientId;
                        //建立行情身份信息
                        Ice.Identity quotIdent = new Ice.Identity();
                        quotIdent.name = Guid.NewGuid().ToString("N");
                        //加入身份到Adapter
                        _event._uuid = _uuid;
                        _clientServer.add(new PackageReceiveClientI(_event), ident);
                        _clientServer.add(new QuotPackageI(_event), quotIdent);
                        _clientServer.activate();
                        _server.ice_getConnection().setAdapter(_clientServer);
                        //设置连接消息回调
                        _server.ice_getConnection().setCallback(_event);
                        err = _server.RegQuotClient(ident, quotIdent, (int)_type);
                    }
                    if (err == 0 || err == 1)
                    {
                        if (this._isNormalClose)
                        {
                            LogMsgAction?.Invoke("heartBeatProcess", $"_isNormalClose is True,无需发送OnConnectionEvent回调!{GetIPStr()}", 1);
                            return;
                        }
                        //发送连接成功的消息
                        _eventHelper.OnConnectionEvent(_uuid, ConnectStateClient.CONNECT_RE);
                        _eventHelper.OnConnectionEvent(_uuid, ConnectStateClient.CONNECT_OK);
                        _stopConnect = true;
                    }

                    LogMsgAction?.Invoke("heartBeatProcess", $"自动断线重连成功:{GetIPStr()}", 1);

                }
                catch (Exception ex)
                {
                    if (isRequestStartVote)
                    {
                        _eventHelper.OnRequestStartVoteEvent(KeyIDAndType);
                        isRequestStartVote = false;
                    }
                    LogHelper.IceError(ex, $"heartBeatProcess:{GetIPStr()}");
                    Thread.Sleep(1000);

                }
            }
            _heartBeatThread = null;
        }
        #endregion

        #region 公有方法
        public int Init(string clientId, string ip, string port)
        {
            _ip = ip;
            _port = port;
            _clientId = clientId;
            Thread initThread = new Thread(workThread);
            initThread.IsBackground = true;
            initThread.Name = "initIceThread";
            initThread.Start();

            return 0;
        }
        public string GetClientId()
        {
            return _clientId;
        }
        /// <summary>
        ///  $"{ClientId}:{ClientType}";
        ///   7777:88
        /// </summary>
        /// <returns></returns>
        public string KeyIDAndType
        {
            get { return $"{_clientId}:{_type}"; }
        }
        public string GetUUID()
        {
            return _uuid;
        }
        public void SetNeedZip(bool iszip = false)
        {
            NeedZip = iszip;
        }
        public void ice_ping()
        {
            try
            {
                _server.ice_ping();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public int SSLInit(string clientId, string ip, string port, string path, string pem, string p12, string pass, string dllName)
        {
            //安全加密只支持一条tcp连接方式
            if (_mode != 'o' && _mode != 'c')
            {
                return -1;
            }
            _ip = ip;
            _port = port;
            _clientId = clientId;
            _enableSSL = true;
            _sslPath = path;
            _pemName = pem;
            _p12Name = p12;
            _sslPass = pass;
            if (!string.IsNullOrEmpty(dllName))
            {
                _dllName = dllName;
            }

            Thread initThread = new Thread(workThread);
            initThread.IsBackground = true;
            initThread.Name = "initIceThread";
            initThread.Start();

            return 0;
        }

        public async Task Close()
        {
            try
            {
                _stopConnect = true;
                ConnectionClose connectionClose = ConnectionClose.Forcefully;

                if (_server != null)
                {
                    try
                    {
                        var dd = _server.ice_pingAsync(_server.ice_getContext());
                        await dd.WaitAsync(new TimeSpan(0, 0, 2));
                        if (dd.IsCompletedSuccessfully)
                        {
                            connectionClose = ConnectionClose.Gracefully;
                        }
                        _server.ice_getConnection().close(connectionClose);
                    }
                    catch (System.Exception ex)
                    {
                        _server.ice_getConnection().close(ConnectionClose.Forcefully);
                        LogHelper.Error(ex, "IceClientClose1", GetIPStr());
                    }
                    _server = null;
                }
                if (_serverOne != null)
                {
                    try
                    {
                        var dd = _serverOne.ice_pingAsync(_serverOne.ice_getContext());
                        await dd.WaitAsync(new TimeSpan(0, 0, 2));
                        if (dd.IsCompletedSuccessfully)
                        {
                            connectionClose = ConnectionClose.Gracefully;
                        }
                        _serverOne.ice_getConnection().close(connectionClose);
                    }
                    catch (System.Exception ex)
                    {
                        _serverOne.ice_getConnection().close(ConnectionClose.Forcefully);
                        LogHelper.Error(ex, "IceClientClose2", GetIPStr());

                    }
                    _serverOne = null;
                }
                if (_communicator != null)
                {
                    _communicator.shutdown();
                    _communicator = null;
                }
                if (_clientServer != null)
                {
                    _clientServer.destroy();
                    _clientServer = null;
                }
            }
            catch (Ice.Exception ex)
            {
                LogHelper.Error(ex, "IceClientClose3", GetIPStr());
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "IceClientClose4", GetIPStr());
            }
        }
        public void Close(bool isForce)
        {
            try
            {
                _stopConnect = true;

                if (_server != null)
                {
                    //_server.ice_getConnection().close(isForce);
                    _server = null;
                }
                if (_serverOne != null)
                {
                    //_serverOne.ice_getConnection().close(true);
                    _serverOne = null;
                }
                if (_communicator != null)
                {
                    _communicator.shutdown();
                    _communicator = null;
                }
                if (_clientServer != null)
                {
                    _clientServer.destroy();
                    _clientServer = null;
                }
            }
            catch (Ice.Exception ex)
            {
                LogHelper.Error(ex, "IceClientClose5", GetIPStr());
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "IceClientClose6", GetIPStr());
            }
        }
        public ResponsePackage SendPackage(RequestPackage requestPackage)
        {
            try
            {
                if (_server == null)
                {
                    ResponsePackage package = new ResponsePackage();
                    package.ErrorInfo = "通信对象初始化失败。";
                    package.ErrorNo = 1000;
                    return package;
                }
                UpdateParamUUID(requestPackage);
                return _server.SendPackage(requestPackage);
            }
            catch (Ice.Exception ex)
            {

                ResponsePackage package = new ResponsePackage();
                //InnerException，这个对象有时会为null，导致无法结束
                if (ex.InnerException != null)
                {
                    package.ErrorInfo = ex.InnerException.Message + "通信失去连接。";
                }
                else
                {
                    package.ErrorInfo = ex.Message + "通信失去连接。";
                }
                package.ErrorNo = 1001;
                LogHelper.Error(ex, "通信失去连接", $"SendPackage:{requestPackage.ServiceNo}");
                return package;
            }
            catch (System.Exception ex)
            {
                ResponsePackage package = new ResponsePackage();
                package.ErrorInfo = ex.ToString();
                package.ErrorNo = 1002;
                return package;
            }
        }
        public ResponsePackage SendPackage(int serviceNo, string[] head, string[] data)
        {
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                if (_server == null)
                {
                    ResponsePackage package = new ResponsePackage();
                    package.ErrorInfo = "通信对象初始化失败。";
                    package.ErrorNo = 1000;
                    return package;
                }
                return _server.SendPackage(packageData(serviceNo, head, data));
            }
            catch (Ice.Exception ex)
            {

                ResponsePackage package = new ResponsePackage();
                //InnerException，这个对象有时会为null，导致无法结束
                if (ex.InnerException != null)
                {
                    package.ErrorInfo = ex.InnerException.Message + "通信错误。";
                }
                else
                {
                    package.ErrorInfo = ex.Message + "通信错误。";
                }
                package.ErrorNo = 1001;
                LogHelper.Error(ex, "通信错误", $"SendPackage:{serviceNo}");
                return package;
            }
            catch (System.Exception ex)
            {
                ResponsePackage package = new ResponsePackage();
                package.ErrorInfo = ex.ToString();
                package.ErrorNo = 1002;
                return package;
            }
            finally
            {
                stopwatch.Stop();
                if (stopwatch.ElapsedMilliseconds > 300)
                {
                    LogHelper.WriteLog("SendPackageTimeUse", $"{serviceNo}:{stopwatch.ElapsedMilliseconds}ms");
                }
            }
        }
        public Ice.AsyncResult<SlmCommunicator.Callback_PackageSender_SendPackage> SendPackageAsync(int serviceNo, string[] head, string[] data)
        {
            try
            {
                if (_server == null)
                {
                    return null;
                }
                return _server.begin_SendPackage(packageData(serviceNo, head, data));
            }
            catch (Ice.Exception ex)
            {

                ResponsePackage package = new ResponsePackage();
                //InnerException，这个对象有时会为null，导致无法结束
                if (ex.InnerException != null)
                {
                    package.ErrorInfo = ex.InnerException.Message + "通信失去连接。";
                }
                else
                {
                    package.ErrorInfo = ex.Message + "通信失去连接。";
                }
                package.ErrorNo = 1001;
                LogHelper.Error(ex, "通信失去连接", $"SendPackage:{serviceNo},{package.ErrorNo},{package.ErrorInfo}");
                return null;
            }
            catch (System.Exception ex)
            {
                ResponsePackage package = new ResponsePackage();
                package.ErrorInfo = ex.ToString();
                package.ErrorNo = 1002;
                LogHelper.Error(ex, "通信失去连接", $"SendPackage:{serviceNo},{package.ErrorNo},{package.ErrorInfo}");
                return null;
            }
        }
        /// <summary>
        /// 批量异步发送
        /// </summary>
        /// <param name="requestPackages"></param>
        /// <returns></returns>
        public Ice.AsyncResult<SlmCommunicator.Callback_PackageSender_SendPackageList> SendPackageListAsync(RequestPackageOrder[] requestPackages)
        {
            try
            {
                if (_server == null)
                {
                    LogHelper.WriteErrorLog("通信失去连接-1,_server为null");
                    return null;
                }
                UpdateParamUUID(requestPackages);
                return _server.begin_SendPackageList(requestPackages);
            }
            catch (Ice.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1001", $"SendPackageListAsync:{requestPackages.Length}");
                return null;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1002", $"SendPackageListAsync:{requestPackages.Length}");
                return null;
            }
        }
        /// <summary>
        /// 同步发送log
        /// </summary>
        /// <param name="requestPackages"></param>
        /// <returns></returns>
        public int SendLogPackage(RequestPackageLog requestPackages)
        {
            try
            {
                if (_server == null)
                {
                    LogHelper.WriteErrorLog("通信失去连接-1,_server为null");
                    return -1;
                }
                UpdateParamUUID(requestPackages);
                return _server.SendLogPackage(requestPackages);
            }
            catch (Ice.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1001", $"SendLogPackageAsync:{requestPackages.toJsonStr()}");
                return -2;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1002", $"SendLogPackageAsync:{requestPackages.toJsonStr()}");
                return -3;
            }
        }
        /// <summary>
        /// 批量异步发送log
        /// </summary>
        /// <param name="requestPackages"></param>
        /// <returns></returns>
        public Ice.AsyncResult<SlmCommunicator.Callback_PackageSender_SendLogPackage> SendLogPackageAsync(RequestPackageLog requestPackages)
        {
            try
            {
                if (_server == null)
                {
                    LogHelper.WriteErrorLog("通信失去连接-1,_server为null");
                    return null;
                }
                UpdateParamUUID(requestPackages);
                return _server.begin_SendLogPackage(requestPackages);
            }
            catch (Ice.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1001", $"SendLogPackageAsync:{requestPackages.toJsonStr()}");
                return null;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1002", $"SendLogPackageAsync:{requestPackages.toJsonStr()}");
                return null;
            }
        }
        /// <summary>
        /// 批量异步发送log list
        /// </summary>
        /// <param name="requestPackages"></param>
        /// <returns></returns>
        public Ice.AsyncResult<SlmCommunicator.Callback_PackageSender_SendLogPackageList> SendLogPackageListAsync(RequestPackageOrderLog[] requestPackages)
        {
            try
            {
                if (_server == null)
                {
                    LogHelper.WriteErrorLog("通信失去连接-1,_server为null");
                    return null;
                }
                UpdateParamUUID(requestPackages);
                return _server.begin_SendLogPackageList(requestPackages);
            }
            catch (Ice.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1001", $"begin_SendLogPackageList:{requestPackages.Length}");
                return null;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1002", $"begin_SendLogPackageList:{requestPackages.Length}");
                return null;
            }
        }
        /// <summary>
        /// 批量异步发送log list RequestPackageOrder
        /// </summary>
        /// <param name="requestPackages"></param>
        /// <returns></returns>
        public Ice.AsyncResult<SlmCommunicator.Callback_PackageSender_SendLogRequestPackageList> SendLogPackageListAsync(RequestPackageOrder[] requestPackages, int mstype)
        {
            try
            {
                if (_server == null)
                {
                    LogHelper.WriteErrorLog("通信失去连接-1,_server为null");
                    return null;
                }
                UpdateParamUUID(requestPackages);
                return _server.begin_SendLogRequestPackageList(requestPackages, mstype);
            }
            catch (Ice.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1001", $"begin_SendLogRequestPackageList:{requestPackages.Length}");
                return null;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1002", $"begin_SendLogRequestPackageList:{requestPackages.Length}");
                return null;
            }
        }
        /// <summary>
        /// 批量异步发送log list ResponsePackageOrder
        /// </summary>
        /// <param name="responsePackages"></param>
        /// <returns></returns>
        public Ice.AsyncResult<SlmCommunicator.Callback_PackageSender_SendLogResponsePackageList> SendLogPackageListAsync(ResponsePackageOrder[] responsePackages, int mstype)
        {
            try
            {
                if (_server == null)
                {
                    LogHelper.WriteErrorLog("通信失去连接-1,_server为null");
                    return null;
                }
                UpdateParamUUID(responsePackages);
                return _server.begin_SendLogResponsePackageList(responsePackages, mstype);
            }
            catch (Ice.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1001", $"begin_SendLogResponsePackageList:{responsePackages.Length}");
                return null;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1002", $"begin_SendLogResponsePackageList:{responsePackages.Length}");
                return null;
            }
        }
        /// <summary>
        /// 批量同步发送
        /// </summary>
        /// <param name="requestPackages"></param>
        /// <returns></returns>
        public ResponsePackageOrderBackMsg SendPackageList(RequestPackageOrder[] requestPackages)
        {
            try
            {
                if (_server == null)
                {
                    LogHelper.WriteErrorLog("通信失去连接-1,_server为null");
                    return new ResponsePackageOrderBackMsg() { ErrorInfo = "SendPackageList通信失去连接-1,_server为null", ErrorNo = -1 };
                }

                UpdateParamUUID(requestPackages);
                return _server.SendPackageList(requestPackages);
            }
            catch (Ice.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1001", $"SendPackageList:{requestPackages.toJsonStr()}");
                return new ResponsePackageOrderBackMsg() { ErrorInfo = "SendPackageList通信失去连接1001", ErrorNo = 1001 };
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1002", $"SendPackageList:{requestPackages.toJsonStr()}");
                return new ResponsePackageOrderBackMsg() { ErrorInfo = "SendPackageList通信失去连接1002", ErrorNo = 1002 };
            }
        }

        public ResponsePackage SendPackage(string clientId, int serviceNo, string[] head, string[] data)
        {
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                if (_server == null)
                {
                    ResponsePackage package = new ResponsePackage();
                    package.ErrorInfo = "通信对象初始化失败。";
                    package.ErrorNo = 1000;
                    return package;
                }
                return _server.SendPackage(packageData(clientId, serviceNo, head, data));
            }
            catch (Ice.Exception ex)
            {
                ResponsePackage package = new ResponsePackage();
                package.ErrorInfo = "通信失去连接。";
                package.ErrorNo = 1001;
                LogHelper.Error(ex, $"通信失去连接。SendPackage:{serviceNo}", ex.InnerException.Message);
                return package;
            }
            catch (System.Exception ex)
            {
                ResponsePackage package = new ResponsePackage();
                package.ErrorInfo = ex.ToString();
                package.ErrorNo = 1002;
                return package;
            }
            finally
            {
                stopwatch.Stop();
                if (stopwatch.ElapsedMilliseconds > 300)
                {
                    LogHelper.WriteLog("SendPackageTimeUse", $"{serviceNo}:{stopwatch.ElapsedMilliseconds}ms,Length:{data?.Length}");
                }
            }
        }

        #region cache send async
        #region 同步
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="requestPackages"></param>
        /// <returns></returns>
        public PackageCacheDB SendPackageCacheBase(PackageCacheDB requestPackages)
        {
            try
            {
                if (_server == null)
                {
                    LogHelper.WriteErrorLog("通信失去连接-1,_server为null");
                    return null;
                }
                UpdateParamUUID(requestPackages);
                return _server.SendPackageCacheBase(requestPackages);
            }
            catch (Ice.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1001", $"SendPackageCacheBase:{requestPackages?.ServiceNo}");
                return null;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1002", $"SendPackageCacheBase:{requestPackages?.ServiceNo}");
                return null;
            }
        }
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="requestPackages"></param>
        /// <returns></returns>
        public CacheTradeDB SendPackageCacheTrade(CacheTradeDB requestPackages)
        {
            try
            {
                if (_server == null)
                {
                    LogHelper.WriteErrorLog("通信失去连接-1,_server为null");
                    return null;
                }
                UpdateParamUUID(requestPackages);
                return _server.SendPackageCacheTrade(requestPackages);
            }
            catch (Ice.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1001", $"SendPackageCacheTrade:{requestPackages?.PackageData?.ServiceNo}");
                return null;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1002", $"SendPackageCacheTrade:{requestPackages?.PackageData?.ServiceNo}");
                return null;
            }
        }
        #endregion
        #region 异步

        /// <summary>
        /// 异步发送
        /// </summary>
        /// <param name="requestPackages"></param>
        /// <returns></returns>
        public Ice.AsyncResult<SlmCommunicator.Callback_PackageSenderCacheDB_SendPackageCacheBase> SendPackageCacheBaseAsync(PackageCacheDB requestPackages)
        {
            try
            {
                if (_server == null)
                {
                    LogHelper.WriteErrorLog("通信失去连接-1,_server为null");
                    return null;
                }
                UpdateParamUUID(requestPackages);
                return _server.begin_SendPackageCacheBase(requestPackages);
            }
            catch (Ice.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1001", $"SendPackageCacheBaseAsync:{requestPackages?.ServiceNo}");
                return null;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1002", $"SendPackageCacheBaseAsync:{requestPackages?.ServiceNo}");
                return null;
            }
        }
        /// <summary>
        /// 异步发送
        /// </summary>
        /// <param name="requestPackages"></param>
        /// <returns></returns>
        public Ice.AsyncResult<SlmCommunicator.Callback_PackageSenderCacheDB_SendPackageCacheTrade> SendPackageCacheTradeAsync(CacheTradeDB requestPackages)
        {
            try
            {
                if (_server == null)
                {
                    LogHelper.WriteErrorLog("通信失去连接-1,_server为null");
                    return null;
                }
                UpdateParamUUID(requestPackages);
                return _server.begin_SendPackageCacheTrade(requestPackages);
            }
            catch (Ice.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1001", $"SendPackageCacheBaseAsync:{requestPackages?.PackageData?.ServiceNo}");
                return null;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "通信失去连接1002", $"SendPackageCacheBaseAsync:{requestPackages?.PackageData?.ServiceNo}");
                return null;
            }
        }
        #endregion
        #region UUID update

        /// <summary>
        /// 更新参数中的uuid及clientid
        /// </summary>
        /// <param name="package"></param>
        public void UpdateParamUUID(CacheTradeDB package)
        {
            if (package == null || package.PackageData == null)
            {
                return;
            }
            package.PackageData.UUID = _uuid;
            package.PackageData.ClientId = _clientId;
        }
        public void UpdateParamUUID(PackageCacheDB package)
        {
            if (package == null)
            {
                return;
            }
            package.UUID = _uuid;
            package.ClientId = _clientId;
        }
        public void UpdateParamUUID(RequestPackageOrder package)
        {
            if (package == null)
            {
                return;
            }
            package.UUID = _uuid;
            package.ClientId = _clientId;
        }
        public void UpdateParamUUID(RequestPackageOrder[] package)
        {
            if (package == null)
            {
                return;
            }
            foreach (var item in package)
            {
                UpdateParamUUID(item);
            }
        }
        public void UpdateParamUUID(ResponsePackageOrder package)
        {
            if (package == null)
            {
                return;
            }
            package.UUID = _uuid;
            package.ClientId = _clientId;
        }
        public void UpdateParamUUID(ResponsePackageOrder[] package)
        {
            if (package == null)
            {
                return;
            }
            foreach (var item in package)
            {
                UpdateParamUUID(item);
            }
        }
        public void UpdateParamUUID(RequestPackageOrderLog package)
        {
            if (package == null)
            {
                return;
            }
            package.UUID = _uuid;
            package.ClientId = _clientId;
        }
        public void UpdateParamUUID(RequestPackageOrderLog[] package)
        {
            if (package == null)
            {
                return;
            }
            foreach (var item in package)
            {
                UpdateParamUUID(item);
            }
        }
        public void UpdateParamUUID(RequestPackageLog package)
        {
            if (package == null)
            {
                return;
            }
            package.UUID = _uuid;
            package.ClientId = _clientId;
        }
        public void UpdateParamUUID(RequestPackage package)
        {
            if (package == null)
            {
                return;
            }
            package.UUID = _uuid;
            package.ClientId = _clientId;
        }
        #endregion
        #endregion
        public bool IsReady()
        {
            return _isReady;
        }

        public void SetNormalClose()
        {
            this._isNormalClose = true;
        }

        public void SetAutoConnect(bool isAuto)
        {
            this._isAutoConnect = isAuto;
        }

        public void SetServerReConnect(bool reConn)
        {
            _serverReConnect = reConn;
        }
        public Ice.Communicator GetCommunicator()
        {
            return _communicator;
        }
        public ObjectPrx GetObjectPrx()
        {
            return prx;
        }
        #endregion
    }

}
