using Ice;
using Solomon.Log;
using Solomon.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SlmCommunicator.Client
{
    /// <summary>
    /// 通信客户端帮助类
    /// </summary>
    public class CommunicatorClientHelper
    {
        #region 属性变量
        //参数列表 for client
        private Dictionary<string, string> _param = new Dictionary<string, string>();
        //参数列表 for server
        private List<string> _paramEx = new List<string>();
        //ice通信对象
        private IceClient _ice = null;
        //事件通知对象
        private EventClientHelper _eventHelper = null;

        private bool _reConnect = true;

        /// <summary>
        /// 用于输出日志显示
        /// </summary>
        public Action<string, string, int> LogMsgAction { get; set; }
        #endregion

        #region 构造方法
        public CommunicatorClientHelper()
        {
            _eventHelper = new EventClientHelper();
        }

        public CommunicatorClientHelper(bool reConnect)
        {
            _reConnect = reConnect;
            _eventHelper = new EventClientHelper();
        }

        ~CommunicatorClientHelper()
        {
            Dispose();
        }
        #endregion

        #region 私有方法

        #endregion

        #region 公有方法
        /// <summary>
        /// 初始化通信
        /// </summary>
        /// <param name="clientId">登录id</param>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口号</param>
        /// <param name="type">通信种类</param>
        /// <param name="mode">通信模式 one way or two way</param>
        /// <param name="handler">连接状态回调</param>
        /// <returns>初始化结果 0成功</returns>
        public int Init(string clientId, string ip, string port, char mode, ConnectionEventClientHandler handler, bool isZip = false, int ACM_TIME_OUT = 60)
        {
            LogHelper.WriteSystemLog($"**Init:{clientId},{ip},{port},{mode}");
            //注册连接事件
            _eventHelper.RegConnectionEvent(handler);
            _ice = new IceClient(mode, _eventHelper);
            _ice.LogMsgAction = LogMsgAction;
            _ice.SetNeedZip(isZip);
            _ice.SetServerReConnect(_reConnect);
            //启动ice连接
            _ice.ACM_TIME_OUT = ACM_TIME_OUT;
            return _ice.Init(clientId, ip, port);
        }

        /// <summary>
        /// 初始化SSL通信
        /// </summary>
        /// <param name="clientId">登录id</param>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口号</param>
        /// <param name="type">通信种类</param>
        /// <param name="mode">通信模式 one way or two way</param>
        /// <param name="handler">连接状态回调</param>
        /// <param name="type">客户端类型</param>
        /// <param name="sslPath">ssl路径</param>
        /// <param name="pemName">pem文件名</param>
        /// <param name="p12Name">p12文件名</param>
        /// <param name="caPass">ca密码</param>
        /// <returns>初始化结果 0成功</returns>
        public int SSLInit(string clientId, string ip, string port, char mode, ClientType type, ConnectionEventClientHandler handler, string sslPath, string pemName, string p12Name, string caPass, string dllName, bool isZip = false, int ACM_TIME_OUT = 60)
        {
            //去掉路径最后的斜杠
            string tmp = sslPath;
            while (tmp.Substring(tmp.Length - 1, 1) == "\\" || tmp.Substring(tmp.Length - 1, 1) == "/")
            {
                tmp = tmp.Substring(0, tmp.Length - 1);
            }

            //检查ssl参数
            if (!System.IO.Directory.Exists(sslPath) || !System.IO.File.Exists(tmp + "\\" + pemName) || !System.IO.File.Exists(tmp + "\\" + p12Name))
            {
                return -1;
            }

            //注册连接事件
            _eventHelper.RegConnectionEvent(handler);
            _ice = new IceClient(mode, type, _eventHelper);
            _ice.LogMsgAction = LogMsgAction;
            _ice.SetNeedZip(isZip);
            _ice.SetServerReConnect(_reConnect);
            //启动ice连接
            _ice.ACM_TIME_OUT = ACM_TIME_OUT;
            return _ice.SSLInit(clientId, ip, port, sslPath, pemName, p12Name, caPass, dllName);
        }

        /// <summary>
        /// 初始化通信
        /// </summary>
        /// <param name="clientId">登录id</param>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口号</param>
        /// <param name="type">通信种类</param>
        /// <param name="mode">通信模式 one way or two way</param>
        /// <param name="type">客户端类型</param>
        /// <param name="handler">连接状态回调</param>
        /// <returns></returns>
        public int Init(string clientId, string ip, string port, char mode, ClientType type, ConnectionEventClientHandler handler, bool isZip = false, int ACM_TIME_OUT = 60)
        {
            //注册连接事件
            _eventHelper.RegConnectionEvent(handler);
            _ice = new IceClient(mode, type, _eventHelper);
            _ice.LogMsgAction = LogMsgAction;
            _ice.SetNeedZip(isZip);
            _ice.SetServerReConnect(_reConnect);
            //启动ice连接
            _ice.ACM_TIME_OUT = ACM_TIME_OUT;
            return _ice.Init(clientId, ip, port);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public async void Dispose()
        {
            if (_ice != null)
            {
                _ice.SetNormalClose();
                //关闭ice
                await _ice.Close();

                _ice = null;
            }
        }

        /// <summary>
        /// 释放资源
        /// 断开不自动重连
        /// </summary>
        public void DisposeSetNormalClose()
        {
            if (_ice != null)
            {
                _ice.SetNormalClose();
                //关闭ice
                _ice.Close(true);

                _ice = null;
            }
        }
        //todo  王圣平：此处可能存在问题，多线程的时候，有的线程返回false 导致获取不到数据。如果使用多线程需要优化这个类的方法
        /// <summary>
        /// 判断是否能发送服务调用 
        /// </summary>
        /// <returns></returns>
        public bool InitPackage()
        {
            //清空参数
            _param.Clear();
            _paramEx.Clear();
            return true;

        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="param"></param>
        public bool AddParameter(string name, string param)
        {
            try
            {
                _param.Add(name.ToLower(), param);
            }
            catch //(System.Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="param"></param>
        public bool AddParameter(string param)
        {
            try
            {
                _paramEx.Add(param);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 发送同步调用请求
        /// </summary>
        /// <param name="serviceNo"></param>
        /// <param name="param"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public ResponsePackageEx SendPackage(int serviceNo)
        {
            try
            {
                if (_ice == null)
                {
                    return null;
                }

                ResponsePackage rspPackage = null;
                if (_paramEx.Count > 0) //for server
                {
                    rspPackage = _ice.SendPackage(serviceNo, null, _paramEx.ToArray());
                }
                else
                {
                    var lisKeys = new List<string>();
                    if (_param.Keys.Contains(PagingInfoDicHelper.HeaderName))
                    {
                        var getPageInfo = _param[PagingInfoDicHelper.HeaderName];
                        _param.Remove(PagingInfoDicHelper.HeaderName);
                        lisKeys = _param.Keys.ToList();
                        lisKeys.Add(getPageInfo);
                    }
                    else
                    {
                        lisKeys = _param.Keys.ToList();
                    }
                    rspPackage = _ice.SendPackage(serviceNo, lisKeys.ToArray(), _param.Values.ToArray());
                }

                ResponsePackageEx result = new ResponsePackageEx(rspPackage);

                return result;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "SendPackage", serviceNo.toJsonStr());
                return null;
            }
        }
        /// <summary>
        /// 发送同步调用请求
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="serviceNo"></param>
        /// <param name="param"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public ResponsePackageEx SendPackageEx(string clientId, int serviceNo)
        {
            try
            {
                if (_ice == null)
                {
                    return null;
                }

                ResponsePackage rspPackage = null;
                if (_paramEx.Count > 0) //for server
                {
                    rspPackage = _ice.SendPackage(clientId, serviceNo, null, _paramEx.ToArray());
                }
                else
                {
                    rspPackage = _ice.SendPackage(clientId, serviceNo, _param.Keys.ToArray(), _param.Values.ToArray());
                }

                ResponsePackageEx result = new ResponsePackageEx(rspPackage);

                return result;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "SendPackageEx", serviceNo.toJsonStr());
                return null;
            }

        }
        public bool SendPackageAsyn(int serviceNo)
        {
            var result = false;
            try
            {

                if (_ice == null)
                {
                    return result;
                }

                Ice.AsyncResult<Callback_PackageSender_SendPackage> rspPackage = null;
                var keys = _param.Keys.ToArray();
                var values = _param.Values.ToArray();

                if (_paramEx.Count > 0) //for server
                {
                    keys = null;
                    values = _paramEx.ToArray();
                }

                rspPackage = _ice.SendPackageAsync(serviceNo, keys, values).whenCompleted((ResponsePackage rsp) =>
                {
                    _eventHelper.OnReceivePackageEvent(rsp);

                }, (Ice.Exception excb) =>
                {
                    _eventHelper.OnReceivePackageEvent(new ResponsePackage() { ServiceNo = serviceNo, ErrorNo = 1001, ErrorInfo = excb.toJsonStr() });
                    LogHelper.Error(excb, "通信失去连接", $"SendPackageAsyn:{serviceNo},{keys.toJsonStr()},{values.toJsonStr()}");
                    //throw excb;
                });
                if (rspPackage != null)
                {
                    result = rspPackage.isSent();
                }
                return result;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "SendPackageAsyn", serviceNo.toJsonStr());
                return result;
            }

        }
        public bool SendPackageAsyn(int serviceNo, string[] keys, string[] values)
        {
            var result = false;
            try
            {
                if (_ice == null)
                {
                    return result;
                }
                var rspPackage = _ice.SendPackageAsync(serviceNo, keys, values);
                if (rspPackage == null)
                {
                    return false;
                }
                else
                {
                    result = true;
                }

                rspPackage.whenCompleted((ResponsePackage rsp) =>
                {
                    _eventHelper.OnReceivePackageEvent(rsp);

                }, (Ice.Exception excb) =>
                {
                    string[][] body = new string[][] { values };
                    LogHelper.Error(excb, "通信失去连接", $"SendPackageAsyn:{serviceNo},{keys.toJsonStr()},{values.toJsonStr()}");
                    _eventHelper.OnReceivePackageEvent(new ResponsePackage() { ServiceNo = serviceNo, ErrorNo = 1001, ErrorInfo = excb.toJsonStr(), DataBody = body });
                    //throw excb;
                });
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "SendPackageAsyn", values.toJsonStr());
            }
            return result;
        }
        public string GetClientId()
        {
            return _ice?.GetClientId();
        }
        public string GetUUID()
        {
            return _ice?.GetUUID();
        }
        /// <summary>
        /// 批量同步发送
        /// </summary>
        /// <param name="requestPackages"></param>
        /// <returns></returns>
        public ResponsePackageOrderBackMsg SendPackageList(RequestPackageOrder[] requestPackages)
        {
            var result = new ResponsePackageOrderBackMsg() { ErrorNo = -1, ErrorInfo = "SendPackageList: system error" };

            try
            {
                if (_ice == null || requestPackages == null)
                {
                    return result;
                }
                var rspPackage = _ice.SendPackageList(requestPackages);

                result = rspPackage;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "SendPackageList", $"SendPackageList:{requestPackages.toJsonStr()}");
            }
            return result;
        }
        /// <summary>
        /// 批量异步步发送
        /// </summary>
        /// <param name="requestPackages"></param>
        /// <returns></returns>
        public ResponsePackageOrderBackMsg SendPackageListAsyn(RequestPackageOrder[] requestPackages)
        {
            var result = new ResponsePackageOrderBackMsg() { ErrorNo = -1, ErrorInfo = "SendPackageListAsyn: system error" };

            try
            {
                if (_ice == null || requestPackages == null)
                {
                    return result;
                }
                var rspPackage = _ice.SendPackageListAsync(requestPackages);
                if (rspPackage == null)
                {
                    return result;
                }
                else
                {
                    var sendflag = rspPackage.isSent();
                    result.ErrorNo = sendflag ? 0 : -1;
                    result.ErrorInfo = sendflag ? "SendPackageListAsyn send success." : "SendPackageListAsyn send error";
                }

                rspPackage.whenCompleted((rsp) =>
                {
                    LogHelper.WriteLog("whenCompleted_SendPackageListAsyn", $"Count:{requestPackages.Length},result:{rsp?.ServiceNo},Clientid:{rsp?.ClientId},MsgId:{rsp?.MsgId},UUID:{rsp?.UUID},{rsp?.ErrorInfo}");
                    _eventHelper.OnSendPackageListAsynCallBackEvent(rsp);

                }, (Ice.Exception excb) =>
                {
                    LogHelper.Error(excb, "通信失去连接", $"SendPackageListAsyn:{requestPackages.Length}");
                    ResponsePackageOrder[] resps = new ResponsePackageOrder[requestPackages.Length];
                    for (int i = 0; i < requestPackages.Length; i++)
                    {
                        resps[i] = new ResponsePackageOrder()
                        {
                            ServiceNo = requestPackages[i].ServiceNo,
                            Head = requestPackages[i].Head,
                            ClientId = requestPackages[i].ClientId,
                            UUID = requestPackages[i].UUID,
                            MsgId = requestPackages[i].MsgId,
                            DataBody = null,
                            SlmOrderBody = requestPackages[i].SlmOrderBody,
                            PushMatchStateBody = requestPackages[i].PushMatchStateBody,
                            PushOrderStateBody = requestPackages[i].PushOrderStateBody,
                            SendOrderStatusBody = requestPackages[i].SendOrderStatusBody,
                            SlmOrderJourBody = requestPackages[i].SlmOrderJourBody,
                            SlmMatchBody = requestPackages[i].SlmMatchBody,
                            ErrorNo = 1001,
                        };
                    }
                    _eventHelper.OnReceivePackageListEvent(resps);
                    //throw excb;
                });
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "SendPackageList", $"SendPackageList:{requestPackages.toJsonStr()}");
            }
            return result;

        }

        public bool SendPackageAsyn(int serviceNo, string[] keys, string[] values, Action<ResponsePackageEx> backAction)
        {
            var result = false;
            try
            {
                if (_ice == null)
                {
                    return result;
                }
                var rspPackage = _ice.SendPackageAsync(serviceNo, keys, values);
                if (rspPackage == null)
                {
                    return false;
                }

                rspPackage.whenCompleted((ResponsePackage rsp) =>
                {
                    ResponsePackageEx rr = new ResponsePackageEx(rsp);
                    //_eventHelper.OnReceivePackageEvent(rsp);
                    backAction(rr);
                }, (Ice.Exception excb) =>
                {
                    string[][] body = new string[][] { values };
                    LogHelper.Error(excb, "通信失去连接", $"SendPackageAsyn:{serviceNo},{keys.toJsonStr()},{values.toJsonStr()}");
                    _eventHelper.OnReceivePackageEvent(new ResponsePackage() { ServiceNo = serviceNo, ErrorNo = 1001, ErrorInfo = excb.toJsonStr(), DataBody = body });
                    //throw excb;
                });
                //rspPackage.waitForCompleted();
                result = true;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "SendPackageAsyn", values.toJsonStr());
            }
            return result;
        }

        public ResponsePackageEx SendPackage(int serviceNo, string[] keys, string[] values)
        {
            ResponsePackageEx result = null;
            try
            {
                if (_ice == null)
                {
                    return null;
                }

                ResponsePackage rspPackage = null;
                rspPackage = _ice.SendPackage(serviceNo, keys, values);

                result = new ResponsePackageEx(rspPackage);

                return result;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "SendPackage", values.toJsonStr());
                return result;

            }
        }
        #region cache 发送功能
        #region 同步
        public CacheTradeDB SendPackageCacheTrade(CacheTradeDB requestPackages)
        {
            CacheTradeDB rspPackage = null;
            try
            {
                if (_ice == null)
                {
                    return null;
                }

                if (requestPackages.PackageData == null)
                {
                    LogHelper.Error("CacheTradeDB PackageData 不能为null");
                    return null;
                }
                rspPackage = _ice.SendPackageCacheTrade(requestPackages);

                return rspPackage;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "SendPackageCacheTrade", requestPackages.toJsonStr());
                return rspPackage;

            }
        }
        public PackageCacheDB SendPackageCacheBase(PackageCacheDB requestPackages)
        {
            PackageCacheDB rspPackage = null;
            try
            {
                if (_ice == null)
                {
                    return null;
                }
                rspPackage = _ice.SendPackageCacheBase(requestPackages);
                return rspPackage;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "SendPackageCacheBase", requestPackages.toJsonStr());
                return rspPackage;

            }
        }
        #endregion
        #region async

        public bool SendPackageCacheTradeAsync(CacheTradeDB requestPackages)
        {
            var result = false;
            try
            {
                if (_ice == null)
                {
                    return result;
                }
                if (requestPackages.PackageData == null)
                {
                    LogHelper.Error("CacheTradeDB PackageData 不能为null");
                    return false;
                }
                var rspPackage = _ice.SendPackageCacheTradeAsync(requestPackages);
                if (rspPackage == null)
                {
                    return false;
                }

                rspPackage.whenCompleted((CacheTradeDB rsp) =>
                {
                    _eventHelper.OnReceivePackageCacheTradeEvent(rsp);

                }, (Ice.Exception excb) =>
                {
                    LogHelper.Error(excb, "通信失去连接", $"SendPackageCacheTradeAsync:{requestPackages.toJsonStr()}");
                    _eventHelper.OnReceivePackageEvent(new ResponsePackage() { ServiceNo = requestPackages.PackageData.ServiceNo, ErrorNo = 1001, ErrorInfo = excb.toJsonStr(), DataBody = requestPackages.PackageData.DataBody });

                });
                result = true;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "SendPackageCacheTradeAsync", requestPackages.toJsonStr());
            }
            return result;
        }
        public bool SendPackageCacheBaseAsync(PackageCacheDB requestPackages)
        {
            var result = false;
            try
            {
                if (_ice == null)
                {
                    return result;
                }
                var rspPackage = _ice.SendPackageCacheBaseAsync(requestPackages);
                if (rspPackage == null)
                {
                    return false;
                }

                rspPackage.whenCompleted((PackageCacheDB rsp) =>
                {
                    _eventHelper.OnReceivePackageCacheDBEvent(rsp);

                }, (Ice.Exception excb) =>
                {
                    LogHelper.Error(excb, "通信失去连接", $"SendPackageCacheBaseAsync:{requestPackages.toJsonStr()}");
                    _eventHelper.OnReceivePackageEvent(new ResponsePackage() { ServiceNo = requestPackages.ServiceNo, ErrorNo = 1001, ErrorInfo = excb.toJsonStr(), DataBody = requestPackages.DataBody });

                });
                result = true;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "SendPackageCacheBaseAsync", requestPackages.toJsonStr());
            }
            return result;
        }

        #endregion
        #endregion
        #region 日志发送功能
        /// <summary>
        /// log同步发送
        /// </summary>
        /// <param name="requestPackages"></param>
        /// <returns></returns>
        public int SendLogPackage(RequestPackageLog requestPackages)
        {
            var result = -1;
            if (_ice == null || requestPackages == null)
            {
                return result;
            }
            try
            {
                return _ice.SendLogPackage(requestPackages);
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "SendLogPackageAsync", $"SendLogPackageAsync:{requestPackages.toJsonStr()}");
            }
            return result;
        }

        /// <summary>
        /// log异步发送
        /// </summary>
        /// <param name="requestPackages"></param>
        /// <returns></returns>
        public int SendLogPackageAsync(RequestPackageLog requestPackages)
        {
            var result = -1;
            if (_ice == null || requestPackages == null)
            {
                return result;
            }
            try
            {
                var rspPackage = _ice.SendLogPackageAsync(requestPackages);
                if (rspPackage == null)
                {
                    return result;
                }
                else
                {
                    result = 0;
                }

                rspPackage.whenCompleted((rsp) =>
                {
                    //LogHelper.WriteFrontReceiveLog($"SendLogPackageAsync send Count:{requestPackages.Length}, result:{rsp}");

                }, (Ice.Exception excb) =>
                {
                    LogHelper.Error(excb, "通信失去连接", $"SendLogPackageAsync:{requestPackages.toJsonStr()}");
                    //throw excb;
                });
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "SendLogPackageAsync", $"SendLogPackageAsync:{requestPackages.toJsonStr()}");
            }
            return result;
        }
        /// <summary>
        /// log批量异步发送
        /// </summary>
        /// <param name="requestPackages"></param>
        /// <returns></returns>
        public int SendLogPackageListAsync(RequestPackageOrderLog[] requestPackages, Action<RequestPackageOrderLog[]> actionErrorTodo = null)
        {
            var result = -1;
            if (_ice == null || requestPackages == null)
            {
                return result;
            }
            try
            {
                var rspPackage = _ice.SendLogPackageListAsync(requestPackages);
                if (rspPackage == null)
                {
                    return result;
                }
                else
                {
                    result = 0;
                }

                rspPackage.whenCompleted((rsp) =>
                {
                    //LogHelper.WriteFrontReceiveLog($"SendLogPackageListAsync send Count:{requestPackages.Length}, result:{rsp}");

                }, (Ice.Exception excb) =>
                {
                    LogHelper.Error(excb, "通信失去连接", $"SendLogPackageListAsync Length:{requestPackages.Length}");
                    actionErrorTodo?.Invoke(requestPackages);
                    //throw excb;
                });
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "SendLogPackageListAsync", $"SendLogPackageListAsync Length:{requestPackages.Length}");
            }
            return result;
        }

        /// <summary>
        /// log批量异步发送 RequestPackageOrder
        /// </summary>
        /// <param name="requestPackages"></param>
        /// <returns></returns>
        public int SendLogPackageListAsync(RequestPackageOrder[] requestPackages, int mstype)
        {
            var result = -1;
            if (_ice == null || requestPackages == null)
            {
                return result;
            }
            try
            {
                var rspPackage = _ice.SendLogPackageListAsync(requestPackages, mstype);
                if (rspPackage == null)
                {
                    return result;
                }
                else
                {
                    result = 0;
                }

                rspPackage.whenCompleted((rsp) =>
                {
                    //LogHelper.WriteFrontReceiveLog($"SendLogPackageListAsync send Count:{requestPackages.Length}, result:{rsp}");

                }, (Ice.Exception excb) =>
                {
                    LogHelper.Error(excb, "通信失去连接", $"SendLogPackageListAsync RequestPackageOrder:{requestPackages.Length}");
                    //throw excb;
                });
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "SendLogPackageListAsync", $"SendLogPackageListAsync RequestPackageOrder:{requestPackages.Length}");
            }
            return result;
        }

        /// <summary>
        /// log批量异步发送 ResponsePackageOrder
        /// </summary>
        /// <param name="requestPackages"></param>
        /// <returns></returns>
        public int SendLogPackageListAsync(ResponsePackageOrder[] requestPackages, int mstype)
        {
            var result = -1;
            if (_ice == null || requestPackages == null)
            {
                return result;
            }
            try
            {
                var rspPackage = _ice.SendLogPackageListAsync(requestPackages, mstype);
                if (rspPackage == null)
                {
                    return result;
                }
                else
                {
                    result = 0;
                }

                rspPackage.whenCompleted((rsp) =>
                {
                    //LogHelper.WriteFrontReceiveLog($"SendLogPackageListAsync send Count:{requestPackages.Length}, result:{rsp}");

                }, (Ice.Exception excb) =>
                {
                    LogHelper.Error(excb, "通信失去连接", $"SendLogPackageListAsync ResponsePackageOrder:{requestPackages.Length}");
                    //throw excb;
                });
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex, "SendLogPackageListAsync", $"SendLogPackageListAsync ResponsePackageOrder:{requestPackages.Length}");
            }
            return result;
        }
        #endregion


        /// <summary>
        /// 注册收包回调事件
        /// </summary>
        /// <param name="call"></param>
        public void RegResponseEvent(ReceivePackageClientEventHandler call)
        {
            _eventHelper.RegReceivePackageEvent(call);
        }
        /// <summary>
        /// 注册收包list回调事件
        /// </summary>
        /// <param name="call"></param>
        public int RegResponseListEvent(ReceivePackageListClientEventHandler call)
        {
            return _eventHelper.RegReceivePackageListEvent(call);
        }
        /// <summary>
        /// 注册 SendPackageListAsyn 发送回包 异步回调
        /// </summary>
        /// <param name="call"></param>
        public void RegSendPackageListAsynCallBackEvent(SendPackageListAsynCallBackEventHandler call)
        {
            _eventHelper.RegSendPackageListAsynCallBackEvent(call);
        }
        /// <summary>
        /// 注册收包CacheBase回调事件
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        public int RegReceivePackageCacheBaseEvent(ReceivePackageCacheDBClientEventHandler call)
        {
            return _eventHelper.RegReceivePackageCacheDBEvent(call);
        }
        /// <summary>
        /// 注册收包CacheTrade回调事件
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        public int RegReceivePackageCacheTradeEvent(ReceivePackageCacheTradeClientEventHandler call)
        {
            return _eventHelper.RegReceivePackageCacheTradeEvent(call);
        }
        /// <summary>
        /// 连接成功后,第一次断线重连时,失败后,调用投票功能
        /// </summary>
        /// <param name="call"></param>
        public void RegRequestStartVoteEvent(RequestStartVoteHandler call)
        {
            _eventHelper.RegRequestStartVoteEvent(call);
        }
        /// <summary>
        /// 设置自动重连开关
        /// </summary>
        /// <param name="isAuto"></param>
        public void SetAutoConnect(bool isAuto)
        {
            if (_ice != null)
            {
                _ice.SetAutoConnect(isAuto);
            }
        }

        /// <summary>
        /// 注册接收行情回调事件
        /// </summary>
        /// <param name="call"></param>
        public void RegRecQuotEvent(RecQuotEventClientHandler call)
        {
            _eventHelper.RegRecQuotEvent(call);
        }

        public Dictionary<string, string> GetParam()
        {
            return _param;
        }

        public List<string> GetParamEx()
        {
            return _paramEx;
        }
        public Ice.Communicator GetCommunicator()
        {
            return _ice?.GetCommunicator();
        }
        public Ice.ObjectPrx GetObjectPrx()
        {
            return _ice?.GetObjectPrx();
        }
        #endregion

        #region exts       
        public ResponsePackage SendPackage(RequestPackage requestPackage)
        {
            if (_ice == null)
            {
                return null;
            }
            var result = _ice.SendPackage(requestPackage);
            return result;
        }
        public void ice_ping()
        {
            try
            {
                _ice.ice_ping();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }
        #endregion
    }
}
