using SlmCommunicator;
using Solomon.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Test.PackageServer.Model;

namespace Test.PackageServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _appExit = false;
        private bool _startServer = false;
        public DispatcherTimer dispatcherTimer { get; set; }
        public DispatcherTimer dispatcherTimerList { get; set; }
        public string version { get; set; }

        public MainWindow()
        {
            //注册回调
            initAction();

            InitializeComponent();

            this.version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            this.Title += "->" + version;

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new System.EventHandler(OnTimedEvent);
            dispatcherTimerList = new DispatcherTimer();
            dispatcherTimerList.Tick += new System.EventHandler(OnTimedListEvent);
            //dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1);

            string logPath = string.Format("{0}Log\\{1}", System.AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("yyyyMMdd"));
            App._ActionAll.OnSystemMessageEvent("日志路径：" + logPath, 1);
            int err = LogHelper.Init(logPath, 500);
            if (err == -1)
            {
                App._ActionAll.OnSystemMessageEvent("初始日志失败！", 2);
            }

            //初始化log
            CvsLogHelper.GetInstance().Init(DateTime.Now.ToString("yyyyMMdd"));

            initPushText();

            BatchPushDemoHelper.GetInstance().StartThread();
            this.Closing += MainWindow_Closing;



        }
        void initAction()
        {
            //注册回调
            App._ActionAll.OnFrontServerStateEvent = onServerStateEvent;
            App._ActionAll.ExitEvent = onExit;
            App._ActionAll.OnQuotConnectionStatusEvent = onQuotStateEvent;
            App._ActionAll.OnSystemMessageEvent = changeNotice;
        }

        private void initPushText()
        {
            var ddic = new Dictionary<int, List<string>>();
            ddic[1] = new List<string>() { "1", "1", "a" };
            ddic[2] = new List<string>() { "2", "2", "b" };
            var dd = new ResponsePackageDTO()
            {
                ClientId = "7777",
                ErrorInfo = "",
                ErrorNo = 0,
                ServiceNo = 1000,
                DataBody = ddic
            };
            var tmpMsg = dd.toJsonStr();
            rtRecPush.Document.Blocks.Clear();
            rtRecPush.AppendText(tmpMsg);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            BatchPushDemoHelper.GetInstance().Dispose();
        }
        public void init()
        {

            //启动推送线程
            FrontPushHelper.GetInstance().StartThread();

            //连接路由
            //connectRoute(txtRoute_IP.Text, txtRoute_Port.Text, txtRouteName.Text);

            bool iszip = ckZip.IsChecked.GetValueOrDefault();
            //启动
            startTradeService(txtIP.Text.Trim(), txtPort.Text.Trim(), iszip);
        }

        private void startTradeService(string ip = "0.0.0.0", string port = "10000", bool iszip = false)
        {
            //启动交易服务 
            //初始化server
            App._ActionAll.OnSystemMessageEvent("初始化服务...", 1);
            TestServerHelper.GetInstance().actionMsg = (msg) =>
            {
                changeNotice(msg);
            };
            TestServerHelper.GetInstance().AddClientType = (clienttype, isadd) =>
            {
                AddClientType(clienttype, isadd);
            };
            TestServerHelper.GetInstance().Init(ip, port, iszip);
            _startServer = true;
        }

        #region 连接状态
        private void onServerStateEvent(bool conn)
        {

        }

        private void onFrontConnectEvent(bool state, ClientInfo info)
        {
            if (state)
            {
                App._ActionAll.OnSystemMessageEvent(string.Format("前置{0}已连接。", info.ClientId), 1);
            }
            else
            {
                App._ActionAll.OnSystemMessageEvent(string.Format("前置{0}已断开连接。", info.ClientId), 2);
            }
        }

        private void onQuotStateEvent(bool conn)
        {
            if (conn)
            {
                //设置信号灯
                Console.WriteLine(conn);
            }
            else
            {
                //设置信号灯
                Console.WriteLine(conn);
            }
        }

        private void onExit()
        {
            _appExit = true;
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                this.Close();
            }));
        }
        #endregion
        public void changeNotice(string msg, short type)
        {
            changeNotice(msg);
        }
        public void changeNotice(SystemMessage message)
        {
            changeNotice(message.MessageInfo);
        }
        public void AddClientType(string clienttype, bool isadd)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    var getListType = txtPushType.Text.SplitTrim(',');
                    if (!getListType.Contains(clienttype))
                    {
                        getListType.Add(clienttype);
                        txtPushType.Text = string.Join(",", getListType);
                        return;
                    }
                    if (!isadd)
                    {
                        getListType.Remove(clienttype);
                        txtPushType.Text = string.Join(",", getListType);
                        return;
                    }
                }
                catch { }
            }));
        }
        public async void changeNotice(string msg)
        {
            await Task.Delay(10);
            await Task.Factory.StartNew(() =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {

                    try
                    {
                        if (rtRec.Document.Blocks.Count > 10000)
                        {
                            rtRec.Document.Blocks.Clear();
                        }
                        rtRec.ScrollToEnd();
                        rtRec.AppendText($"\n*{DateTime.Now}->{msg}");
                    }
                    catch { }
                }));

            });
            await Task.Delay(10);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnConnect.IsEnabled = false;
                if (txtPort.Text.isNull() || txtIP.Text.isNull())
                {
                    return;
                }
                TestServerHelper.GetInstance().ActiveList = txtActiveAndsandby.Text.Split(',').ToList();
                TestServerHelper.GetInstance().ActiveListTrade = txtActiveAndsandbyTrade.Text.Split(',').ToList();
                init();
            }
            catch (Exception ex)
            {
                btnConnect.IsEnabled = true;
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            btnConnect.IsEnabled = true;

            //停止推送前置线程
            FrontPushHelper.GetInstance().Dispose();

            //停止交易服务
            TestServerHelper.GetInstance().Dispose();

            App._ActionAll.OnSystemMessageEvent("***服务已关闭...", 1);
        }

        private async void btnClear_Click(object sender, RoutedEventArgs e)
        {
            rtRec.Document.Blocks.Clear();
            GC.Collect(0);
            await Task.Delay(10);

        }
        private string GetText(RichTextBox richTextBox)
        {
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            return textRange.Text;
        }
        private void OnTimedEvent(object sender, EventArgs e)
        {
            PushDataOld();
        }
        public void PushDataOld()
        {
            try
            {
                var toPushPackage = GetText(rtRecPush).JsonTo<ResponsePackageDTO>();
                ChangeConnectID(toPushPackage);
                changeNotice($"isPushAsync:{isPushAsync.IsChecked.GetValueOrDefault()},测试(旧)，开始推送。。。");
                var getListType = txtPushType.Text.SplitTrim(',');
                foreach (var item in getListType)
                {
                    FrontPushHelper.GetInstance().AddData(toPushPackage, item.TryToInt());
                }
            }
            catch (Exception ex)
            {
                changeNotice(ex.Message);
            }
        }
        public void PushDataList(string jsonStr)
        {
            try
            {
                var toPushPackage = jsonStr.JsonTo<ResponsePackageOrderDto>();
                ChangeConnectID(toPushPackage);
                changeNotice($"isPushAsync:true,测试(List)，开始推送。。。");
                var getListType = txtPushType.Text.SplitTrim(',');
                foreach (var item in getListType)
                {
                    FrontPushHelper.GetInstance().AddData(toPushPackage, item.TryToInt());
                }
            }
            catch (Exception ex)
            {
                changeNotice(ex.Message);
            }
        }

        public void PushDataByMType()
        {
            try
            {
                var jsonStr = GetText(rtRecPush);
                var toPushPackage = jsonStr.JsonTo<ComDto>();


                if (toPushPackage == null)
                {
                    changeNotice($"## 输入错数据消息包。。。{jsonStr}");
                    return;
                }

                switch (toPushPackage.MType)
                {
                    case emMType.Base:
                        PushDataOld();
                        break;
                    case emMType.OrderList:
                        PushDataList(jsonStr);
                        break;
                    case emMType.CacheDB:
                        PushDataCacheDB(jsonStr);
                        break;
                    case emMType.TradeDB:
                        PushCacheTradeDBDto(jsonStr);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                changeNotice(ex.Message);
            }
        }
        public void PushDataCacheDB(string jsonStr)
        {
            try
            {
                var toPushPackage = jsonStr.JsonTo<PackageCacheDBDto>();
                ChangeConnectID(toPushPackage);
                changeNotice($"isPushAsync:true,测试(CacheDB)，开始推送。。。");
                var getListType = txtPushType.Text.SplitTrim(',');
                foreach (var item in getListType)
                {
                    FrontPushHelper.GetInstance().AddData(toPushPackage, item.TryToInt());
                }
            }
            catch (Exception ex)
            {
                changeNotice(ex.Message);
            }
        }
        public void PushCacheTradeDBDto(string jsonStr)
        {
            try
            {
                var toPushPackage = jsonStr.JsonTo<CacheTradeDBDto>();
                ChangeConnectID(toPushPackage);
                changeNotice($"isPushAsync:true,测试(CacheTradeDB)，开始推送。。。");
                var getListType = txtPushType.Text.SplitTrim(',');
                foreach (var item in getListType)
                {
                    FrontPushHelper.GetInstance().AddData(toPushPackage, item.TryToInt());
                }
            }
            catch (Exception ex)
            {
                changeNotice(ex.Message);
            }
        }
        void ChangeConnectID(CacheTradeDBDto toPushPackage)
        {
            switch (toPushPackage.ServiceNo)
            {
                case (int)ArbittrationSeverServiceNo.RequestActiveStandbySwitch:
                case (int)ArbittrationSeverServiceNo.RequestServerIdentify:
                    var firstValue = toPushPackage.PackageData.DataBody.First();
                    var bodyInfoList = firstValue.Value[0].JsonTo<List<CommunicationClientInfo>>();
                    var allClients = TestServerHelper.GetInstance().allClientInfo;
                    foreach (var item in bodyInfoList)
                    {
                        if (allClients.ContainsKey(item.KeyIDAndType))
                        {
                            item.ConnectionId = allClients[item.KeyIDAndType].ConnectionId;
                        }
                    }
                    firstValue.Value[0] = bodyInfoList.toJsonStr();
                    break;
                default:
                    break;
            }
        }
        void ChangeConnectID(PackageCacheDBDto toPushPackage)
        {
            switch (toPushPackage.ServiceNo)
            {
                case (int)ArbittrationSeverServiceNo.RequestActiveStandbySwitch:
                case (int)ArbittrationSeverServiceNo.RequestServerIdentify:
                    var firstValue = toPushPackage.DataBody.First();
                    var bodyInfoList = firstValue.Value[0].JsonTo<List<CommunicationClientInfo>>();
                    var allClients = TestServerHelper.GetInstance().allClientInfo;
                    foreach (var item in bodyInfoList)
                    {
                        if (allClients.ContainsKey(item.KeyIDAndType))
                        {
                            item.ConnectionId = allClients[item.KeyIDAndType].ConnectionId;
                        }
                    }
                    firstValue.Value[0] = bodyInfoList.toJsonStr();
                    break;
                default:
                    break;
            }
        }
        void ChangeConnectID(ResponsePackageOrderDto toPushPackage)
        {
            try
            {


            }
            catch (Exception ex)
            {
                changeNotice(ex.Message);
            }
        }
        void ChangeConnectID(ResponsePackageDTO toPushPackage)
        {
            switch (toPushPackage.ServiceNo)
            {
                case (int)ArbittrationSeverServiceNo.RequestActiveStandbySwitch:
                case (int)ArbittrationSeverServiceNo.RequestServerIdentify:
                    var firstValue = toPushPackage.DataBody.First();
                    var bodyInfoList = firstValue.Value[0].JsonTo<List<CommunicationClientInfo>>();
                    var allClients = TestServerHelper.GetInstance().allClientInfo;
                    foreach (var item in bodyInfoList)
                    {
                        if (allClients.ContainsKey(item.KeyIDAndType))
                        {
                            item.ConnectionId = allClients[item.KeyIDAndType].ConnectionId;
                        }
                    }
                    firstValue.Value[0] = bodyInfoList.toJsonStr();
                    break;
                default:
                    break;
            }
        }

        List<ResponsePackageOrder> tolist = new List<ResponsePackageOrder>();
        private void OnTimedListEvent(object sender, EventArgs e)
        {
            PushData();
        }
        public void PushData()
        {
            if (!tolist.HasItem())
            {
                int toPad = DataSize.Text.TryToInt();
                for (int i = 0; i < toPad; i++)
                {
                    string[][] pushData = new string[10][];
                    string s = "".PadRight(3, '0');
                    //pushData[0] = new string[1000000];           
                    pushData[0] = new string[1];
                    pushData[0][0] = s;

                    ResponsePackageOrder rp = new ResponsePackageOrder();
                    rp.DataBody = pushData;
                    rp.ErrorNo = 0;
                    rp.ErrorInfo = "PushData Test";
                    rp.ServiceNo = 10000 + i;
                    rp.SlmOrderBody = new SlmOrderRow() { AccountNo = "111" + i };
                    rp.PushMatchStateBody = new PushMatchStateRow() { AccountNo = "111" + i };
                    rp.PushOrderStateBody = new PushOrderStateRow() { BrokerOrderJourNo = "11" + i };
                    tolist.Add(rp);
                }
            }
            changeNotice($"isPushAsync:{isPushAsync.IsChecked.GetValueOrDefault()},测试，开始推送。。。");
            var getListType = txtPushType.Text.SplitTrim(',');
            foreach (var item in getListType)
            {
                //批量推送到客户端
                TestServerHelper.GetInstance().SendResponsePackageList(tolist.ToArray(), item.TryToInt());
            }
        }
        private void btnSendPush_Click(object sender, RoutedEventArgs e)
        {
            btnSendPush.IsEnabled = false;
            int mimilliseconds = int.Parse(txtPushTime.Text);
            int batchsize = int.Parse(DataSize.Text);
            BatchPushDemoHelper.GetInstance().bathcSize = batchsize;
            // FrontPushHelper.intTimeSleep = mimilliseconds;		
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, mimilliseconds);
            dispatcherTimer.Start();
        }

        private void btnSendPush_Stop_Click(object sender, RoutedEventArgs e)
        {
            btnSendPush.IsEnabled = true;
            dispatcherTimer.Stop();
            btnSendPush_CopyList.IsEnabled = true;
            dispatcherTimerList.Stop();
        }

        private void btnSendPush_CopyList_Click(object sender, RoutedEventArgs e)
        {
            btnSendPush_CopyList.IsEnabled = false;
            int mimilliseconds = int.Parse(txtPushTime.Text);
            int batchsize = int.Parse(DataSize.Text);
            BatchPushDemoHelper.GetInstance().bathcSize = batchsize;
            // FrontPushHelper.intTimeSleep = mimilliseconds;		
            dispatcherTimerList.Interval = new TimeSpan(0, 0, 0, 0, mimilliseconds);
            dispatcherTimerList.Start();
        }

        private void isShowMsg_Click(object sender, RoutedEventArgs e)
        {
            TestServerHelper.GetInstance().isShowAllMsg = isShowMsg.IsChecked.GetValueOrDefault();
        }

        private void isPushAsync_Click(object sender, RoutedEventArgs e)
        {
            TestServerHelper.GetInstance().isPushAsync = isPushAsync.IsChecked.GetValueOrDefault();
        }

        private void isPushOver_Click(object sender, RoutedEventArgs e)
        {
            TestServerHelper.GetInstance().isPushOver = isPushOver.IsChecked.GetValueOrDefault();
            TestServerHelper.GetInstance().ClientTypeList = new List<int>();
            int batchsize = int.Parse(DataSize.Text);
            BatchPushDemoHelper.GetInstance().bathcSize = batchsize;
            var getListType = txtPushType.Text.SplitTrim(',');
            foreach (var item in getListType)
            {
                TestServerHelper.GetInstance().ClientTypeList.Add(item.ToInt());
            }

        }

        private void btnSendPush_Copy_Click(object sender, RoutedEventArgs e)
        {
            PushDataByMType();
        }

        private void btnSendPush_Cache_Click(object sender, RoutedEventArgs e)
        {
            PushDataByMType();
        }
    }
}
