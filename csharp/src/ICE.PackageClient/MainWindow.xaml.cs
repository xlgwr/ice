using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Threading;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Solomon.Log;
using System.Collections.Concurrent;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Folding;
using Test.PackageClient.BraceFoldings;
using ICSharpCode.AvalonEdit.CodeCompletion;
using Test.PackageClient.CompletionDatas;
using Microsoft.Win32;
using Test.PackageClient.Exts;
using ICSharpCode.AvalonEdit.Search;
using System.IO;
using System.Xml;
using System.ComponentModel;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Document;

using Test.PackageClient;


using SlmCommunicator;
using SlmCommunicator.Client;
using Communicator = SlmCommunicator.Client.CommunicatorClientHelper;
using ConnectState = SlmCommunicator.Client.ConnectStateClient;
using PackageRequestDTO = Test.PackageClient.PackageRequestDTO;

namespace ICE.PackageClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        MainWindow _curr;
        ObservableCollection<string> AllAccountId = new ObservableCollection<string>();
        public string ip { get; set; }

        public string OpenConfigPath;
        public string version { get; set; }
        public MainWindow()
        {

            initAvalonEditBefore();

            InitializeComponent();

            this.DataContext = this;

            initAvalonEditEnd();

            loadFixFileToButton();

            this.Closing += MainWindow_Closing;
            this.Loaded += MainWindow_Loaded;

            initFirst();

            initThread();

            initDllContent();
        }

        private void initDllContent()
        {
            rtSend.AppendText("\n当前依赖项dll:\n");
            App.getAssembly();
            rtSend.AppendText(App.dllDic.toJsonStr());
            rtSend.AppendText("\n");

        }
        #region Notify
        public event PropertyChangedEventHandler PropertyChanged;

        public void notifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                OpenConfigPath = AppDomain.CurrentDomain.BaseDirectory + "\\TestPackageClientConfig.json";

                string logPath = string.Format("{0}Log\\{1}", System.AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("yyyyMMdd"));
                LogHelper.Init(logPath, 50, 10);

                //初始化文件part记数
                LogHelper.GetInstance().GetInitFilePart(logPath, true);

                _curr = this;
                //toConfig
                var getContent = OpenConfigPath.FilePathGetContent(Encoding.UTF8);
                if (getContent.IsNotNullOrWhiteSpace())
                {
                    var cf = getContent.JsonTo<TestPackageClientConfig>();
                    txtClientID.Text = cf.txtClientID.IsNotNullOrWhiteSpace() ? cf.txtClientID : txtClientID.Text;
                    txtClientType.Text = cf.txtClientType.IsNotNullOrWhiteSpace() ? cf.txtClientType : txtClientType.Text;
                    txtIP.Text = cf.txtIP.IsNotNullOrWhiteSpace() ? cf.txtIP : txtIP.Text;
                    txtPort.Text = cf.txtPort.IsNotNullOrWhiteSpace() ? cf.txtPort : txtPort.Text;
                    txtFilterServerNo.Text = cf.txtFilterServerNo.IsNotNullOrWhiteSpace() ? cf.txtFilterServerNo : txtFilterServerNo.Text;
                }

                this.Title += "->" + version;

                initFilterData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var toJson = new TestPackageClientConfig()
            {
                txtClientID = txtClientID.Text.Trim(),
                txtClientType = txtClientType.Text.Trim(),
                txtIP = txtIP.Text.Trim(),
                txtPort = txtPort.Text.Trim(),
                txtFilterServerNo = txtFilterServerNo.Text.Trim()
            };
            OpenConfigPath.FilePathSaveContent(toJson.toJsonStr(), Encoding.UTF8);
        }

        public string defaultJsonMsg = @"欢迎来到ICE客户端消息测试工具！";
        /// <summary>
        /// 启动线程相关
        /// </summary>
        private void initThread()
        {
            //log show
            startThread();
        }
        public void initFirst()
        {
            try
            {
                this.SizeChanged += MainWindow_SizeChanged;
                this.StateChanged += MainWindow_StateChanged;
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                //this.ResizeMode = ResizeMode.NoResize;
                this.rtSend.Text = "";
                this.rtRec.Document.Text = "";
                this.rtSend.AppendText(defaultJsonMsg);
                btnMove.Content = @"<-|||->";
                cmbAllId.ItemsSource = AllAccountId;

                txtClientType.ToolTip = @"TRADE_CLIENT = 1, //交易客户端
                                        RISK_CLIENT = 2,  //风控客户端
                                        CONTER_CLIENT = 3,//柜台客户端
                                        TRADE_API = 4,   //交易api
                                        TRADE_FRONT = 5,  //交易前置
                                        TRADE_SERVER = 6, //交易服务器
                                        TRADE_GATEWAY = 7,//交易网关
                                        QUOT_SERVER = 8,  //行情服务器
                                        QUOT_CONVERGE = 9,//行情汇聚
                                        QUOT_GATEWAY = 10,//行情网关                         
                                        CONTER_FRONT = 11,//管理前置
                                        SETTLE_FRONT = 12,//结算前置
                                        FORCE_CLOSE_CLIENT = 13,//强平客户端
                                        SETTLE_CLIENT = 14,//结算客户端
                                        MONITOR = 15,     //监控服务
                                        CONFIRM_SERVER = 16,    //认证服务
                                        NEWS_SERVER = 17,       //消息服务
                                        HISTORY_SERVER = 18,    //历史查询服务
                                        SMS_SERVER = 19,        //短信服务
                                        RISK_PARAM_SERVER = 20,   //风控参数服务
                                        RISK_DATA_SERVER = 21,    //风控数据服务
                                        FORCE_CALC_SERVER = 22,   //强平计算服务
                                        REIS_TRIGGER_SERVER = 23,  //风控触发服务
                                        FORCE_QUERY_SERVER = 24,  //强平查询
                                        TRANSFER_SERVER = 25,  //中转服务";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {

        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtPort.Text.IsNullOrWhiteSpace() || txtIP.Text.IsNullOrWhiteSpace())
                {
                    return;
                }
                var curr = this;
                this.ip = $"获取:{txtIP.Text},{txtPort.Text}";
                initClient.GetInstanceLazy.ConnectAction = (a) =>
                {
                    curr.Dispatcher.Invoke(() =>
                    {
                        toStop = ((Button)sender).IsEnabled = a == ConnectState.CONNECT_OK ? false : true;
                        var success = a == ConnectState.CONNECT_OK ? "成功" : "失败";
                        changeNotice($"***连接IP:{txtIP.Text},{txtPort.Text}{success}");
                    });
                };
                ClientType clientType = ClientType.CONTER_CLIENT;
                ClientType.TryParse(txtClientType.Text, out clientType);
                char isOnewey = ckOnewey.IsChecked.GetValueOrDefault() ? 't' : 'o';
                bool isZip = ckZip.IsChecked.GetValueOrDefault();
                initClient.GetInstanceLazy.actionMsg = (m) =>
                {
                    curr.Dispatcher.Invoke(() =>
                    {
                        changeNotice($"***{isOnewey},收到数据:{m}");
                    });
                };
                initClient.GetInstanceLazy.ConnectServer(txtClientID.Text.Trim(), txtIP.Text.Trim(), txtPort.Text.Trim(), clientType, isOnewey, isZip);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            initClient.GetInstanceLazy.DisconnectServer();
            btnConnect.IsEnabled = true;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            rtRec.Document.Text = "";
            GC.Collect(0);
        }

        private void rtSend_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        public bool toStop { get; set; }
        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnSend.IsEnabled = false;
                toStop = false;
                rtRec.Document.Text = "";
                _curr = this;
                List<PackageRequestDTO> listTo = new List<PackageRequestDTO>();
                var toSendPackage = GetText(rtSend).JsonTo<PackageRequestDTO>();
                int num = 1;
                decimal takeTime = 1;
                int.TryParse(txtNumber.Text, out num);
                decimal.TryParse(txtSendTime.Text, out takeTime);

                double databodySize = 10;//M
                double.TryParse(txtSendSize.Text, out databodySize);
                if (ckSizeTest.IsChecked.GetValueOrDefault())
                {
                    toSendPackage.DataBody.Clear();
                    string getonewM = "0".GetOneM();
                    for (int i = 0; i < databodySize; i++)
                    {
                        toSendPackage.DataBody.Add(getonewM);
                    }
                }

                int sendcount = 0;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                for (int i = 0; i < num; i++)
                {
                    if (toStop)
                    {
                        changeNotice($"**##**停止成功，目前第：{i}/{num}个.");
                        break;
                    }
                    changeNotice($"**##**开始发送第：{i}/{num}个.");
                    if (ckAddOne.IsChecked.GetValueOrDefault())
                    {
                        toSendPackage.ServiceNo++;
                    }
                    await SendRequest(toSendPackage);
                    await Task.Delay((int)takeTime * 1000);
                    sendcount++;
                }
                stopwatch.Stop();
                changeNotice($"**##**已全部发送完成：{sendcount}/{num}个.发送总时间：{stopwatch.Elapsed}");
                rtRec.Focus();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btnSend.IsEnabled = true;
            }
        }
        public async Task SendRequest(PackageRequestDTO toSendPackage)
        {
            SendRequestToDo(toSendPackage, true);
            await Task.Delay(5);
        }
        public async Task SendRequest(ComDto toSendPackage)
        {
            SendRequestToDo(toSendPackage, true);
            await Task.Delay(5);
        }
        public void SendRequestToDo(PackageRequestDTO toSendPackage, bool isshow = true, string keyComm = "")
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            bool isAsync = ckIsAsync.IsChecked.GetValueOrDefault();
            var getPackage = initClient.GetInstanceLazy.SendRequest(toSendPackage, keyComm, isAsync);
            stopwatch.Stop();
            changeNotice($"**获取:{txtIP.Text},{txtPort.Text}服务号:{toSendPackage.ServiceNo},耗时：{stopwatch.Elapsed},内容如下：\n{getPackage.toJsonStr()}", isshow);

        }
        public void SendRequestToDo(List<PackageRequestDTO> toSendPackage, bool isshow = true, bool isAsync = false, bool islogTest = false, string keyComm = "")
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var getPackage = initClient.GetInstanceLazy.SendRequestList(toSendPackage, keyComm, isAsync, islogTest);
            stopwatch.Stop();
            changeNotice($"**isAsync:{isAsync},{_curr.ip},发送数:{toSendPackage.Count},耗时：{stopwatch.Elapsed},内容如下：\n{getPackage.toJsonStr()}", isshow);

        }
        public void SendRequestToDo(ComDto toSendPackage, bool isshow = true, string keyComm = "")
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            bool isAsync = ckIsAsync.IsChecked.GetValueOrDefault();
            bool isAddKey = ckAddKey.IsChecked.GetValueOrDefault();
            bool isInitAdd = ckInitAddNum.IsChecked.GetValueOrDefault();
            if (isInitAdd)
            {
                initClient.GetInstanceLazy.InitDicAddNum();
            }
            int num = 1;
            int numPackage = 1;
            int.TryParse(txtNumber.Text, out num);
            int.TryParse(txtNumberSend.Text, out numPackage);
            var getPackage = initClient.GetInstanceLazy.SendRequest(toSendPackage, keyComm, isAsync, new Tuple<bool, int, int>(isAddKey, num, numPackage));
            stopwatch.Stop();
            changeNotice($"**获取:{txtIP.Text},{txtPort.Text}服务号:{toSendPackage.ServiceNo},耗时：{stopwatch.Elapsed},内容如下：\n{getPackage.toJsonStr()}", isshow);

        }
        private string GetText(RichTextBox richTextBox)
        {
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            return textRange.Text;
        }
        private string GetText(TextEditor richTextBox)
        {
            return richTextBox.Text;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            this.rtSend.Text = "";
            this.rtSend.AppendText(defaultJsonMsg);
            initDllContent();
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var toSendPackage = GetText(rtSend);
                var toJson = toSendPackage.JsonTo<PackageRequestDTO>();
                var toSendPackageServiceNo = toJson.ServiceNo;
                var toSendPackageServiceName = toJson.ServiceName.ToNotNullString();

                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "json消息文件 (*.json)|*.json";
                saveFileDialog.Title = "保存son消息文件";
                saveFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + @"消息头";
                saveFileDialog.FileName = @"package_" + toSendPackageServiceNo + "_" + toSendPackageServiceName + "_" + DateTime.Now.ToyyyyMMddHHmmssExt();
                if ((bool)(saveFileDialog.ShowDialog()))
                {
                    //获得文件路径
                    var localFilePath = saveFileDialog.FileName.ToString();
                    localFilePath.FilePathSaveContent(toSendPackage, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Filter = "json消息文件 (*.json)|*.json";
                openFileDialog.Title = "加载json消息文件";
                openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + @"消息头";
                if ((bool)openFileDialog.ShowDialog())
                {
                    string tmpPath = openFileDialog.FileName;
                    this.rtSend.Text = "";
                    this.rtSend.AppendText(tmpPath.FilePathGetContent(Encoding.UTF8));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExitBat_Click(object sender, RoutedEventArgs e)
        {
            toStop = true;
            btnSend.IsEnabled = true;
        }

        private void btnSendDo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnSendDo.IsEnabled = false;
                this.Cursor = Cursors.Wait;

                toStop = false;
                rtRec.Document.Text = "";
                _curr = this;
                List<PackageRequestDTO> listTo = new List<PackageRequestDTO>();
                var toSendPackage = GetText(rtSend).JsonTo<PackageRequestDTO>();
                int num = 1;
                int numPackage = 1;
                decimal takeTime = 1;
                int.TryParse(txtNumber.Text, out num);
                int.TryParse(txtNumberSend.Text, out numPackage);
                decimal.TryParse(txtSendTime.Text, out takeTime);

                int sendcount = 0;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                for (int i = 0; i < num; i++)
                {
                    if (toStop)
                    {
                        //changeNotice($"**##**停止成功，目前第：{i}/{num}个.");
                        break;
                    }
                    toSendPackage = GetText(rtSend).JsonTo<PackageRequestDTO>();
                    //changeNotice($"**##**开始发送第：{i}/{num}个.", true); 
                    if (ckAddOne.IsChecked.GetValueOrDefault())
                    {
                        toSendPackage.ServiceNo++;
                    }
                    toSendPackage.DataBody[0] = i.ToString();
                    var tmpCopy = new string[toSendPackage.DataBody.Count];
                    toSendPackage.DataBody.CopyTo(tmpCopy);
                    for (int y = 0; y < numPackage - 1; y++)
                    {
                        toSendPackage.DataBody.AddRange(tmpCopy);
                    }

                    SendRequestToDo(toSendPackage, false);
                    sendcount++;
                }
                stopwatch.Stop();
                //var checkQueue = initClient.GetInstanceLazy.QueueresponsePackages;
                var msg = $"**##**已全部发送完成：{sendcount}/{num}个.发送总时间：{stopwatch.Elapsed}";
                changeNotice(msg);
                rtRec.Focus();
                MessageBox.Show(msg);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
                btnSendDo.IsEnabled = true;
            }
        }

        private void btnGen_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            try
            {
                btn.IsEnabled = false;
                this.Cursor = Cursors.Wait;

                if (txtPort.Text.IsNullOrWhiteSpace() || txtIP.Text.IsNullOrWhiteSpace())
                {
                    return;
                }

                int num = 1;
                int.TryParse(txtclientNums.Text, out num);
                int Starnum = 1000;
                int.TryParse(txtStartClient.Text, out Starnum);

                for (int i = 0; i < num; i++)
                {
                    var curr = this;
                    string currId = $"{Starnum + i}";
                    //initClient.GetInstanceLazy.ConnectAction = (a) =>
                    //{
                    //    curr.Dispatcher.Invoke(() =>
                    //    {
                    //        toStop = ((Button)sender).IsEnabled = a == SlmCommunicator.ConnectState.CONNECT_OK ? false : true;
                    //        var success = a == SlmCommunicator.ConnectState.CONNECT_OK ? "成功" : "失败";
                    //        changeNotice($"***连接IP:{txtIP.Text},{txtPort.Text}{success}");
                    //    });
                    //};
                    //initClient.GetInstanceLazy.actionMsg += (m) =>
                    //{
                    //    curr.Dispatcher.Invoke(() =>
                    //    {
                    //        changeNotice($"***收到数据:{m}");
                    //    });
                    //};
                    Thread.Sleep(1);
                    initClient.GetInstanceLazy.ConnectServerDic(currId, txtIP.Text.Trim(), txtPort.Text.Trim(), ClientType.CONTER_CLIENT);

                }
                AllAccountId.Clear();
                foreach (var item in initClient.GetInstanceLazy.CommunicatorDics.Keys)
                {
                    AllAccountId.Add(item);
                };
                cmbAllId.SelectedIndex = -1;

                changeNotice($"***生成连接数:{num}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
                btn.IsEnabled = true;
            }
        }

        private void btnSend2_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            try
            {
                btn.IsEnabled = false;
                this.Cursor = Cursors.Wait;

                string getCountId = cmbAllId.Text;
                if (cmbAllId.Text.IsNullOrWhiteSpace())
                {
                    cmbAllId.Focus();
                    return;
                }
                toStop = false;
                rtRec.Document.Text = "";
                _curr = this;
                List<PackageRequestDTO> listTo = new List<PackageRequestDTO>();
                var toSendPackage = GetText(rtSend).JsonTo<PackageRequestDTO>();
                int num = 1;
                decimal takeTime = 1;
                int.TryParse(txtNumber.Text, out num);
                decimal.TryParse(txtSendTime.Text, out takeTime);

                int sendcount = 0;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                for (int i = 0; i < num; i++)
                {
                    if (toStop)
                    {
                        //changeNotice($"**##**停止成功，目前第：{i}/{num}个.");
                        break;
                    }
                    if (ckAddOne.IsChecked.GetValueOrDefault())
                    {
                        toSendPackage.ServiceNo++;
                    }
                    //changeNotice($"**##**开始发送第：{i}/{num}个.", true);
                    SendRequestToDo(toSendPackage, true, getCountId);
                    sendcount++;
                }
                stopwatch.Stop();
                var msg = $"**##**已全部发送完成：{sendcount}/{num}个.发送总时间：{stopwatch.Elapsed}";
                changeNotice(msg);
                rtRec.Focus();
                MessageBox.Show(msg);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btn.IsEnabled = true;
                this.Cursor = Cursors.Arrow;
            }
        }

        private void btnCloseAll_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            try
            {
                btn.IsEnabled = false;
                this.Cursor = Cursors.Wait;

                initClient.GetInstanceLazy.DisconnectServer(true);

                AllAccountId.Clear();
                cmbAllId.SelectedIndex = -1;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
                btn.IsEnabled = true;
            }
        }

        private async void btnSendDoBatch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Delay(10);
                btnSendDoBatch.IsEnabled = false;
                this.Cursor = Cursors.Wait;

                toStop = false;
                rtRec.Document.Text = "";
                _curr = this;
                List<PackageRequestDTO> listTo = new List<PackageRequestDTO>();
                int num = 1;
                int numPackage = 1;
                decimal takeTime = 1;
                int.TryParse(txtNumber.Text, out num);
                int.TryParse(txtNumberSend.Text, out numPackage);
                decimal.TryParse(txtSendTime.Text, out takeTime);

                var toSendPackage = GetText(rtSend).JsonTo<PackageRequestDTO>();
                bool bckAddOne = ckAddOne.IsChecked.GetValueOrDefault();
                bool bisShowMsg = isShowMsg.IsChecked.GetValueOrDefault();
                bool isAsync = ckIsAsync.IsChecked.GetValueOrDefault();
                bool isLogTest = ckIsLogTest.IsChecked.GetValueOrDefault();
                int sendcount = 0;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                int ServiceNo = 0;
                var tasktmp = Task.Factory.StartNew(() =>
                {
                    for (int i = 0; i < num; i++)
                    {
                        if (toStop)
                        {
                            //changeNotice($"**##**停止成功，目前第：{i}/{num}个.");
                            break;
                        }
                        var listTmpSendPacke = new List<PackageRequestDTO>();
                        for (int y = 0; y < numPackage; y++)
                        {
                            //toSendPackage.SlmOrderBody = new SlmCommunicator.SlmOrderRow() { AccountNo = "110" };
                            //toSendPackage.PushMatchStateBody = new SlmCommunicator.PushMatchStateRow() { AccountNo = "110" };
                            //toSendPackage.PushOrderStateBody = new SlmCommunicator.PushOrderStateRow() { BrokerOrderJourNo = "100" };
                            //toSendPackage.SendOrderStatusBody = new SlmCommunicator.SendOrderStatusRow() { AccountNo = "111010" };
                            //toSendPackage.SlmOrderJourBody = new SlmCommunicator.SlmOrderJourRow() { AccountNo = "1023434" };
                            //toSendPackage.SlmMatchBody = new List<SlmCommunicator.SlmMatchRow>() { new SlmCommunicator.SlmMatchRow() { AccountNo = "1134234" } };

                            var currtmp = toSendPackage.Copy();
                            //changeNotice($"**##**开始发送第：{i}/{num}个.", true); 
                            if (bckAddOne)
                            {
                                if (ServiceNo <= 0)
                                {
                                    ServiceNo = currtmp.ServiceNo;
                                }
                                currtmp.ServiceNo = ServiceNo;
                                ServiceNo++;
                            }
                            listTmpSendPacke.Add(currtmp);
                        }
                        SendRequestToDo(listTmpSendPacke, bisShowMsg, isAsync, isLogTest);

                        sendcount++;
                    }
                });
                await tasktmp;
                stopwatch.Stop();
                //var checkQueue = initClient.GetInstanceLazy.QueueresponsePackages;
                var msg = $"**##**已全部发送完成：{sendcount}/{num}个.发送总时间：{stopwatch.Elapsed}";
                changeNotice(msg);
                rtRec.Focus();
                //MessageBox.Show(msg);
                await Task.Delay(10);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
                btnSendDoBatch.IsEnabled = true;
            }

        }

        private void isShowMsg_Click(object sender, RoutedEventArgs e)
        {
            initClient.GetInstanceLazy.isShowAllMsg = isShowMsg.IsChecked.GetValueOrDefault();
            if (initClient.GetInstanceLazy.isShowAllMsg)
            {
                initFilterData();
            }
            isSyntaxHighlighting = !initClient.GetInstanceLazy.isShowAllMsg;
        }
        void initFilterData()
        {
            var getList = txtFilterServerNo.Text.SplitTrim(',');
            initClient.GetInstanceLazy.filterServernos.Clear();
            foreach (var item in getList)
            {
                var intFilter = item.TryToInt();
                if (!initClient.GetInstanceLazy.filterServernos.Contains(intFilter))
                {
                    initClient.GetInstanceLazy.filterServernos.Add(intFilter);
                }
            }

            //显示
            rtRec.AppendText("\n**过滤服务号**\n");
            rtRec.AppendText(initClient.GetInstanceLazy.filterServernos.toJsonStr());
            rtRec.AppendText("\n");
        }

        private async void btnSendDoCache_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnSendDoCache.IsEnabled = false;
                toStop = false;
                rtRec.Document.Text = "";
                _curr = this;
                var listTo = new List<ComDto>();
                var jsonStr = GetText(rtSend);
                var toSendPackage = jsonStr.JsonTo<ComDto>();
                int num = 1;
                decimal takeTime = 1;
                //int.TryParse(txtNumber.Text, out num);
                //decimal.TryParse(txtSendTime.Text, out takeTime);


                switch (toSendPackage.MType)
                {
                    case emMType.Base:
                        toSendPackage = PushDataOrderList(jsonStr, "Base");
                        toSendPackage.MType = emMType.Base;
                        break;
                    case emMType.OrderList:
                        toSendPackage = PushDataOrderList(jsonStr, "OrderList");
                        toSendPackage.MType = emMType.OrderList;
                        break;
                    case emMType.CacheDB:
                        toSendPackage = PushDataCacheDB(jsonStr);
                        toSendPackage.MType = emMType.CacheDB;
                        break;
                    case emMType.TradeDB:
                        toSendPackage = PushCacheTradeDBDto(jsonStr);
                        toSendPackage.MType = emMType.TradeDB;
                        break;
                    default:
                        break;
                }
                int sendcount = 0;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                for (int i = 0; i < num; i++)
                {
                    if (toStop)
                    {
                        changeNotice($"**##**停止成功，目前第：{i}/{num}个.");
                        break;
                    }
                    changeNotice($"**##**开始发送第：{i}/{num}个.");
                    if (ckAddOne.IsChecked.GetValueOrDefault())
                    {
                        toSendPackage.ServiceNo++;
                    }
                    await SendRequest(toSendPackage);
                    await Task.Delay((int)takeTime * 1000);
                    sendcount++;
                }
                stopwatch.Stop();
                changeNotice($"**##**已全部发送完成：{sendcount}/{num}个.发送总时间：{stopwatch.Elapsed}");
                rtRec.Focus();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btnSendDoCache.IsEnabled = true;
            }
        }
        public PackageRequestDTO PushDataOrderList(string jsonStr, string title = "List")
        {
            try
            {
                var toPushPackage = jsonStr.JsonTo<PackageRequestDTO>();

                changeNotice($"isPushAsync:true,测试({title})，开始推送。。。");

                return toPushPackage;
            }
            catch (Exception ex)
            {
                changeNotice(ex.Message);
            }
            return null;
        }
        public PackageCacheDBDto PushDataCacheDB(string jsonStr)
        {
            try
            {
                var toPushPackage = jsonStr.JsonTo<PackageCacheDBDto>();

                changeNotice($"isPushAsync:true,测试(CacheDB)，开始推送。。。");

                return toPushPackage;
            }
            catch (Exception ex)
            {
                changeNotice(ex.Message);
            }
            return null;
        }
        public CacheTradeDBDto PushCacheTradeDBDto(string jsonStr)
        {
            try
            {
                var toPushPackage = jsonStr.JsonTo<CacheTradeDBDto>();

                changeNotice($"isPushAsync:true,测试(CacheTradeDB)，开始推送。。。");

                return toPushPackage;
            }
            catch (Exception ex)
            {
                changeNotice(ex.Message);
            }
            return null;
        }

        #region log show
        public void startThread()
        {
            Thread checkThread = new Thread(FlashQueueShow2);
            checkThread.IsBackground = true;
            checkThread.Name = "FlashQueueShow2";
            checkThread.Start();
        }
        int totalsize = 1000;
        private ConcurrentQueue<string> _commandLogQueue = new ConcurrentQueue<string>();
        StringBuilder sb = new StringBuilder();
        string msg = string.Empty;
        bool _runShow = true;
        public void FlashQueueShow2()
        {
            _runShow = true;
            while (_runShow)
            {
                try
                {

                    while (!_commandLogQueue.IsEmpty)
                    {
                        if (totalsize <= 0)
                        {
                            break;
                        }
                        if (_commandLogQueue.TryDequeue(out msg))
                        {
                            sb.AppendLine($"{LogHelper.GetInstance().DateTimeNowAll}->{msg}");
                        }
                        totalsize--;
                    }
                    if (sb.Length > 0)
                    {
                        AddToShowLog(sb.ToString());
                        sb.Clear();
                        totalsize = 1000;
                    }
                }
                catch (Exception ex)
                {
                    rtRec.AppendText(ex.Message);
                }
                Thread.Sleep(1000);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="type">1:info,2:error</param>
        public void changeNotice(string msg, bool ishow = true, int type = 1)
        {
            switch (type)
            {
                case 2:
                    msg = $"Error:{msg}";
                    LogHelper.WriteErrorLog(msg);
                    break;
                case 3:
                    LogHelper.WriteLog("adminLog", msg);
                    break;
                default:
                    LogHelper.WriteSystemLog(msg);
                    break;
            }
            LogHelper.WriteLog("SplitLineDemo", msg, true, true);
            if (ishow)
            {
                _commandLogQueue.Enqueue(msg);
            }
        }
        void CheckisSyntaxHighlighting(string msgToShow)
        {
            if (rtRec.Text.Length >= MaxLengAll)
            {
                int toClearLine = (rtRec.Document.Lines.Count / 3) * 2;
                int beforeLength = rtRec.Text.Length;
                ClearBefore(toClearLine);
                rtRec.AppendText($"\n***************显示清理前：{toClearLine}行，总字符长度超过：{MaxLengAll}->{beforeLength},现在长度：{rtRec.Text.Length}*********************\n");
            }

            if (!isSyntaxHighlighting)
            {
                if (rtRec.SyntaxHighlighting != null)
                {
                    rtRec.SyntaxHighlighting = null;
                }
            }
            else
            {

                if (msgToShow.Length > MaxLengToSyntaxHightlighting)
                {
                    if (rtRec.SyntaxHighlighting != null)
                    {
                        rtRec.AppendText($"\n***************显示语法高亮失效处理:字符长度超过：{MaxLengToSyntaxHightlighting}*********************\n");
                        rtRec.SyntaxHighlighting = null;
                    }
                    isSyntaxHighlighting = false;
                }
            }
        }
        /// <summary>
        /// 清更前多少行
        /// </summary>
        /// <param name="Number"></param>
        void ClearBefore(int offsetRemove)
        {
            if (offsetRemove > 0)
            {
                for (int i = 0; i < offsetRemove; i++)
                {
                    var offsetLine = rtRec.Document.GetLineByOffset(i);
                    rtRec.Document.Remove(offsetLine);
                }
                rtRec.Text = rtRec.Text.TrimStart('\n', '\r');
            }
        }
        private async void AddToShowLog(string msg, bool ishow = true, bool isClear = false)
        {

            if (!ishow)
            {
                return;
            }
            await Task.Delay(10);
            await rtRec.Dispatcher.BeginInvoke(new Action(() =>
            {
                Thread.Sleep(0);
                try
                {

                    CheckisSyntaxHighlighting(msg);

                    if (!isClear)
                    {
                        rtRec.AppendText(msg);
                    }
                    else
                    {
                        rtRec.Document.Text = "";
                        rtRec.AppendText(msg);
                    }
                    var offsetRemove = rtRec.Document.Lines.Count - cmbLogLine.Text.TryToInt(100);
                    ClearBefore(offsetRemove);

                    if (rtRec.Document.Lines.Count > 50 && isScrollToEnd)
                    {
                        rtRec.ScrollToEnd();
                    }
                }
                catch { }
            }));
        }
        #region Tool Set
        /// <summary>
        /// 单行超N个字符，不语法高亮
        /// </summary>
        const int MaxLengToSyntaxHightlighting = 10_0000;
        /// <summary>
        /// 超过100w个字符，清空1/2行
        /// </summary>
        const int MaxLengAll = 1000_0000;
        bool _isScrollToEnd = true;
        public bool isScrollToEnd
        {
            get
            {
                return _isScrollToEnd;
            }
            set
            {
                _isScrollToEnd = value;
                notifyPropertyChanged("isScrollToEnd");
            }
        }


        bool _isSyntaxHighlighting = true;
        public bool isSyntaxHighlighting
        {
            get
            {
                return _isSyntaxHighlighting;
            }
            set
            {
                _isSyntaxHighlighting = value;
                notifyPropertyChanged("isSyntaxHighlighting");
            }
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            rtRec.ScrollToEnd();
            isScrollToEnd = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            isScrollToEnd = false;
        }


        private void CheckBox_Checked_S(object sender, RoutedEventArgs e)
        {
            if (rtRec.Text.Length <= MaxLengToSyntaxHightlighting)
            {
                rtRec.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("log");
            }
        }

        private void CheckBox_Unchecked_S(object sender, RoutedEventArgs e)
        {
            rtRec.SyntaxHighlighting = null;
        }
        #endregion
        #endregion

        #region edit
        void initAvalonEditBefore()
        {
            try
            {

                //
                LoadCustomXshd("Test.PackageClient.Log.xshd", "log", new string[] { ".log", ".txt" });

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private static void LoadCustomXshd(string path, string name, string[] exts)
        {
            // Load our custom highlighting definition
            IHighlightingDefinition customHighlighting;
            using (Stream s = typeof(MainWindow).Assembly.GetManifestResourceStream(path))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting(name, exts, customHighlighting);
        }
        public class TruncateLongLines : VisualLineElementGenerator
        {
            const int maxLength = 2000;
            const string ellipsis = "...";
            const int charactersAfterEllipsis = 100;
            public override int GetFirstInterestedOffset(int startOffset)
            {
                DocumentLine line = CurrentContext.VisualLine.LastDocumentLine;
                if (line.Length > maxLength)
                {
                    int ellipsisOffset = line.Offset + maxLength - charactersAfterEllipsis - ellipsis.Length;
                    if (startOffset <= ellipsisOffset)
                        return ellipsisOffset;
                }
                return -1;
            }
            public override VisualLineElement ConstructElement(int offset)
            {
                return new FormattedTextElement(ellipsis, CurrentContext.VisualLine.LastDocumentLine.EndOffset - offset - charactersAfterEllipsis);
            }
        }
        void initAvalonEditEnd()
        {
            try
            {

                rtRec.AppendText($"...\n");
                //rtSend.TextArea.SelectionBorder = null;
                rtSend.WordWrap = true;
                //rtRec.WordWrap = true;
                rtSend.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Json");
                rtRec.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("log");
                //rtSend.SyntaxHighlighting = customHighlighting;
                //initial highlighting now set by XAML

                //单行超长文本的性能优化
                //rtSend.TextArea.TextView.ElementGenerators.Add(new TruncateLongLines());
                rtRec.TextArea.TextView.ElementGenerators.Add(new TruncateLongLines());

                rtSend.TextArea.TextEntering += textEditor_TextArea_TextEntering;
                rtSend.TextArea.TextEntered += textEditor_TextArea_TextEntered;
                SearchPanel.Install(rtSend);
                SearchPanel.Install(rtRec);

                DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
                foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
                foldingUpdateTimer.Tick += delegate { UpdateFoldings(); };
                foldingUpdateTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }
        FoldingManager foldingManager;
        object foldingStrategy;
        void UpdateFoldings()
        {
            if (foldingStrategy is BraceFoldingStrategy)
            {
                ((BraceFoldingStrategy)foldingStrategy).UpdateFoldings(foldingManager, rtSend.Document);
            }
            if (foldingStrategy is XmlFoldingStrategy)
            {
                ((XmlFoldingStrategy)foldingStrategy).UpdateFoldings(foldingManager, rtSend.Document);
            }
        }
        CompletionWindow completionWindow;

        void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            return;
            if (e.Text == ".")
            {
                // open code completion after the user has pressed dot:
                completionWindow = new CompletionWindow(rtSend.TextArea);
                // provide AvalonEdit with the data:
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                data.Add(new MyCompletionData("Item1"));
                data.Add(new MyCompletionData("Item2"));
                data.Add(new MyCompletionData("Item3"));
                data.Add(new MyCompletionData("Another item"));
                completionWindow.Show();
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
            }
        }

        void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // do not set e.Handled=true - we still want to insert the character that was typed
        }

        private void openFileClick(object sender, RoutedEventArgs e)
        {

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            if (dlg.ShowDialog() ?? false)
            {
                currentFileName = dlg.FileName;
                dlg.Filter = "消息文件 (*.json)|*.json|所有文件 (*.*)|*.*";
                rtSend.Load(currentFileName);
                rtSend.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(System.IO.Path.GetExtension(currentFileName));
                changeNotice($"打开文件成功：{currentFileName}");
            }
        }
        string currentFileName;
        private void saveFileClick(object sender, RoutedEventArgs e)
        {
            if (currentFileName == null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "消息文件 (*.json)|*.json|所有文件 (*.*)|*.*";
                dlg.DefaultExt = ".json";
                if (dlg.ShowDialog() ?? false)
                {
                    currentFileName = dlg.FileName;
                }
                else
                {
                    return;
                }
            }
            rtSend.Save(currentFileName);
            changeNotice($"保存文件成功：{currentFileName}");

        }


        private void saveFileAsClick(object sender, RoutedEventArgs e)
        {

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "消息文件 (*.json)|*.json|所有文件 (*.*)|*.*";
            dlg.InitialDirectory = projectFilePath;
            //dlg.DefaultExt = ".fix";
            if (dlg.ShowDialog() ?? false)
            {
                rtSend.Save(dlg.FileName);
                changeNotice($"另存文件成功：{dlg.FileName}");

            }
            else
            {
                return;
            }
        }

        private void HighlightingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (rtSend.SyntaxHighlighting == null)
            {
                foldingStrategy = null;
            }
            else
            {
                switch (rtSend.SyntaxHighlighting.Name)
                {
                    case "XML":
                        foldingStrategy = new XmlFoldingStrategy();
                        rtSend.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        break;
                    case "C#":
                    case "C++":
                    case "PHP":
                    case "Java":
                        rtSend.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(rtSend.Options);
                        foldingStrategy = new BraceFoldingStrategy();
                        break;
                    default:
                        rtSend.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        foldingStrategy = null;
                        break;
                }
            }
            if (foldingStrategy != null)
            {
                if (foldingManager == null)
                    foldingManager = FoldingManager.Install(rtSend.TextArea);
                UpdateFoldings();
            }
            else
            {
                if (foldingManager != null)
                {
                    FoldingManager.Uninstall(foldingManager);
                    foldingManager = null;
                }
            }
        }

        #endregion
        #region 自定义按键及下拉框
        public Dictionary<int, Tuple<string, string>> _allProjectFile = new Dictionary<int, Tuple<string, string>>();
        public string projectFilePath { get; set; }

        void loadFixFileToButton()
        {
            if (projectFilePath.IsNullOrWhiteSpace())
            {
                projectFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "消息头");
                projectFilePath.FileDirectoryCreateDirectory();

            }
            var toNotnames = new List<string>();
            string getFileName = ".json";
            FileGet.allGetFiles.Clear();
            FileGet.getFile(projectFilePath, getFileName, toNotnames);
            var getAllExe = FileGet.allGetFiles;
            _allProjectFile.Clear();
            int sizeCount = 1;
            foreach (var item in getAllExe)
            {
                _allProjectFile.Add(sizeCount, new Tuple<string, string>(item.Name, item.FullName.FilePathGetContent(Encoding.UTF8)));
                sizeCount++;
            }
            sizeCount--;
            initToolBar(sizeCount);
            initComboBox(sizeCount);

        }
        void initToolBar(int btnsize = 5)
        {
            if (btnsize > 5)
            {
                btnsize = 5;
            }
            for (int i = 1; i <= btnsize; i++)
            {
                string imageName = $"{i}.png";
                Button button = new Button();
                var imageSource = new BitmapImage(new Uri($"/images/{imageName}", UriKind.Relative));
                Image image = new Image() { Source = imageSource, Height = 16 };
                button.Content = image;
                button.ToolTip = _allProjectFile[i].Item1;
                button.Tag = i;
                button.Click += toolbarFixLoad;
                tbTool.Items.Add(button);
            }
        }
        void initComboBox(int allSize = 5)
        {
            //if (allSize <= 5)
            //{
            //    return;
            //}
            Dictionary<string, string> fixDic = new Dictionary<string, string>();
            foreach (var item in _allProjectFile)
            {
                //if (item.Key <= 5)
                //{
                //    continue;
                //}
                fixDic.TryAdd(item.Value.Item1, item.Value.Item2);
            }
            if (fixDic.Keys.Count > 0)
            {
                ComboBox comboBox = new ComboBox() { ToolTip = "当前目录【消息头】下所有json文件,可手动新增,重启，自动加载。" };
                comboBox.ItemsSource = fixDic;
                comboBox.DisplayMemberPath = "Key";
                comboBox.SelectedValuePath = "Value";
                comboBox.Width = 50;
                comboBox.SelectionChanged += ComboBox_SelectionChanged;
                tbTool.Items.Add(comboBox);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var cb = sender as ComboBox;
                if (cb.SelectedValue != null)
                {
                    rtSend.Text = cb.SelectedValue.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolbarFixLoad(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = sender as Button;
                int flag = btn.Tag.TryToInt();

                var getFixContent = _allProjectFile[flag];
                rtSend.Text = getFixContent.Item2;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        Point pointMove = new Point();
        private void btnMove_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.SizeWE;
                ((UIElement)e.Source).CaptureMouse();
                Point pp = Mouse.GetPosition(e.Source as FrameworkElement);//WPF方法
                Point ppp = (e.Source as FrameworkElement).PointToScreen(pp);//WPF方法
                //changeNotice("----------down---------------");
                //changeNotice(pp.toJsonStr());
                //changeNotice(ppp.toJsonStr());
                pointMove = ppp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnMove_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {

                Point pp = Mouse.GetPosition(e.Source as FrameworkElement);//WPF方法
                Point ppp = (e.Source as FrameworkElement).PointToScreen(pp);//WPF方法
                //changeNotice("----------up---------------");
                //changeNotice(pp.toJsonStr());
                //changeNotice(ppp.toJsonStr());
                double diffx = ppp.X - pointMove.X;
                var width = leftCol.ActualWidth + diffx;
                width = width + 110 >= this.ActualWidth ? this.ActualWidth - 110 : width;
                width = width <= 0 ? leftCol.ActualWidth : width;
                leftCol.Width = new GridLength(width);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ((UIElement)e.Source).ReleaseMouseCapture();
                this.Cursor = Cursors.Arrow;
            }
        }
    }
}
