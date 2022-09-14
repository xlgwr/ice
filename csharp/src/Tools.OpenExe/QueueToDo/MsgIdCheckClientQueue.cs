using Solomon.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tools.OpenExe.QueueToDo
{
    /// <summary>
    /// 消息号回报反馈确认
    /// </summary>
    public class MsgIdCheckClientQueue
    {
        public const string logName = "QueueMsgIdCheckClient";
        #region 属性变量
        private static MsgIdCheckClientQueue _instance = null;
        private static object _lock = new object();
        private bool _start = false;
        public static bool IsRuning = false;
        public Action<string, bool, int> logShowActionAll { get; set; }
        /// <summary>
        /// msgid初启化，记录时间
        /// </summary>
        private ConcurrentDictionary<string, DateTime> _commandTimeDic = new ConcurrentDictionary<string, DateTime>();
        /// <summary>
        /// msgid回报，标记
        /// </summary>
        private ConcurrentDictionary<string, bool> _commandCallBackDic = new ConcurrentDictionary<string, bool>();
        /// <summary>
        /// 计数，超3次，移除并报错处理
        /// </summary>
        private ConcurrentDictionary<string, int> _commandCountDic = new ConcurrentDictionary<string, int>();

        /// <summary>
        /// 默认超时为
        /// 10秒 s
        /// 10*1000 ms
        /// </summary>
        public static int intTimeOutTimeForOffset = 10 * 1000;
        #endregion

        #region 构造方法
        public MsgIdCheckClientQueue()
        {
        }

        public static MsgIdCheckClientQueue GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new MsgIdCheckClientQueue();
                        IsRuning = true;
                    }
                }
            }
            return _instance;
        }

        ~MsgIdCheckClientQueue()
        {
            stopThread();
        }
        #endregion

        #region 私有方法
        private void logShowAction(string msg, int type, bool isshow = true)
        {
            logShowActionAll?.Invoke(msg, isshow, type);
        }
        private void stopThread()
        {
            _start = false;
        }

        private void workProcess()
        {
            _start = true;
            SpinWait spinWait = new SpinWait();
            while (_start)
            {
                try
                {
                    //处理消息
                    var comm = getCommand();
                    if (comm.HasItem())
                    {
                        processCommand(comm);
                    }
                    else
                    {
                        spinWait.SpinOnce();
                    }

                }
                catch (Exception ex)
                {
                    LogHelper.WriteErrorLog($"{logName}:{ex}");
                }
            }
        }
        /// <summary>
        /// 处理超时msgid
        /// </summary>
        /// <param name="param"></param>
        private void processCommand(List<string> param)
        {
            foreach (var item in param)
            {
                //查看是否已回报
                if (!_commandCallBackDic.ContainsKey(item))
                {
                    //未回报直接报错
                    var msg = $"已超时[{intTimeOutTimeForOffset}ms]回报,MsgId:{item}";
                    LogHelper.WriteLog($"{logName}TimeOut", msg);
                    logShowAction(msg, 2);
                }

                //丢异常中去
                _commandCountDic.TryAdd(item, 0);

                //并移除标记字典
                _commandTimeDic.TryRemove(item, out _);
                _commandCallBackDic.TryRemove(item, out _);
            }
        }
        /// <summary>
        /// 获取超时msgid
        /// </summary>
        /// <returns></returns>
        private List<string> getCommand()
        {
            var dateTimeNow = DateTime.Now;
            var tmpListKey = _commandTimeDic.Where(a => (dateTimeNow - a.Value).TotalMilliseconds > intTimeOutTimeForOffset).Select(a => a.Key).ToList();
            return tmpListKey;
        }

        #endregion

        #region 公有方法
        /// <summary>
        /// 异步处理
        /// 处理完后
        /// 直接推送给对应connectid客户端
        /// </summary>
        /// <param name="pushData"></param>
        /// <param name="ConnectId"></param>
        public void AddDataDic(string msgid, string desc)
        {
            if (msgid.IsNullOrWhiteSpace())
            {
                LogHelper.WriteLog($"{logName}Error", $"AddDataDic Msgid:不能为空。");
                return;
            }
            _commandTimeDic.TryAdd(msgid, DateTime.Now);
        }
        /// <summary>
        /// 回报记录，返回对应的msgid
        /// </summary>
        /// <param name="msgid"></param>
        public void CallBackAddDataDic(string msgid, string msgnotice)
        {
            try
            {
                if (msgid.IsNullOrWhiteSpace())
                {
                    LogHelper.WriteLog($"{logName}Error", $"CallBackAddDataDic Msgid:不能为空");
                    return;
                }

                var msg = $"收到回报:{msgid},{msgnotice}";
                LogHelper.WriteLog($"{logName}Success", msg);

                if (!_commandTimeDic.ContainsKey(msgid))
                {
                    LogHelper.WriteLog($"{logName}Success", $"Msgid:{msgid} 当前不存在记录时间，{msgnotice}");
                    //记录回报标记：
                    _commandCallBackDic[msgid] = true;
                    return;
                }

                //增量查询回报处理检测 
                _commandCallBackDic[msgid] = true;

                //输出耗时
                var diffTime = DateTime.Now - _commandTimeDic[msgid];
                if (diffTime.TotalMilliseconds > 100)
                {
                    logShowAction($"耗时超100ms:msgid:{msgid}", 2);
                }
                LogHelper.WriteLog($"{logName}SuccessTimeUse", $"Msgid:{msgid},{msgnotice},UseTime:{diffTime.TotalMilliseconds}ms");


            }
            catch (Exception ex)
            {
                LogHelper.WriteLog($"{logName}Error", ex.toJsonStr());
            }
        }
        /// <summary>
        /// 主切备，清理回报确认数据
        /// </summary>
        public void ClearDataByTran()
        {
            logShowAction($"备启用，备切【主】，开始清理回报确认数据...", 1);

            _commandTimeDic.Clear();
            _commandCallBackDic.Clear();

            logShowAction($"备启用，备切【主】，完成清理回报确认数据...", 1);

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
            checkThread.Name = "MsgIdCheckClientQueue";
            checkThread.Start();
        }
        #endregion
    }
}
