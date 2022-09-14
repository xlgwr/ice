using Fluent;
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
using System.Windows.Shapes;
using Solomon.Log;
using SlmCommunicator;
using SlmCommunicator.Client;

namespace Tools.OpenExe
{
    using AvalonDock.Layout;
    using ControlzEx.Theming;
    using Fluent;
    using System.Collections.Concurrent;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;
    using Forms = System.Windows.Forms;
    using System.Windows.Threading;
    using Tools.OpenExe.Exts;
    using Tools.OpenExe.UserControlComm;
    using Tools.OpenExe.VM;
    using System.Runtime.InteropServices;
    using Tools.OpenExe.ICEApp.ServerToDo;
    using System.Threading;
    using Tools.OpenExe.ICEApp;
    using AvalonDock.Layout.Serialization;
    using System.Reflection;
    using Tools.OpenExe.ICEApp.ClientToDo;
    using Tools.OpenExe.QueueToDo;

    /// <summary>
    /// MainRibbonWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainRibbonWindow
    {
        #region 参数
        private readonly MainViewModel viewModel;

        public bool isLoad = false;
        private ObservableCollection<StackPanel> _allExeFile;
        private ObservableCollection<CheckBoxFlag> _allExeFileData;
        private StackPanel[] _allExeFile_bak;
        private ObservableCollection<CheckBoxFlag> _allProjectFile;
        private CollectionViewSource collectionViewSource;
        private static Process[] _getCurrAllProcess;
        private string currPath;
        private List<string> currPathes;
        public string OpenExeConfigPath;

        public string projectFilePath { get; set; }
        public string projectFileConfigPath { get; set; }

        public List<string> otherFilterDef { get; private set; }
        public List<string> logFilterDef { get; private set; }

        public DispatcherTimer dispatcherTimer { get; set; }
        public string version { get; set; }
        public Dictionary<string, string> cacheMd5File = new Dictionary<string, string>();
        public bool isInitOver = false;
        public List<string> listLogpaht { get; set; } = new List<string>();
        int NummberLayoutDocument { get; set; }
        private Forms.NotifyIcon notifyIcon { get; set; }
        public OpenExeConfig currConfig;
        public MainRibbonWindow currMain;
        public readonly object _sendLock = new object();
        #region log
        public void changeNoticeQueue(string msg, bool ishow = true, int type = 1)
        {
            currMain?.Dispatcher.Invoke(() =>
            {
                logShow?.changeNoticeQueue(msg, ishow, type);
            });
        }
        #endregion
        #endregion
        public MainRibbonWindow()
        {
            try
            {
                string logPath = string.Format("{0}Log\\{1}", System.AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToString("yyyyMMdd"));
                LogHelper.Init(logPath, 50);

                loadConfig();

                App.loadDic(this, CheckOneExe);

                this.InitializeComponent();

                currMain = this;

                this.viewModel = new MainViewModel();
                this.DataContext = this.viewModel;

                initNotifyIcon();

                initFirst();

                initThread();

                //注册事件
                this.Loaded += MainRibbonWindow_Loaded;
                this.Closing += MainRibbonWindow_Closing;
                this.StateChanged += MainRibbonWindow_StateChanged;

            }
            finally
            {
                isInitOver = true;
            }
        }

        private void initThread()
        {
            MsgIdCheckClientQueue.GetInstance().StartThread();

            StartThread();
        }

        ~MainRibbonWindow()
        {
            try
            {
                isStopSend = true;
                MsgIdCheckClientQueue.GetInstance().Dispose();

                mutex?.Close();
                //TestServerHelper.GetInstance().Dispose();
                //initClient.GetInstanceLazy.DisconnectServer();
                //LogHelper.GetInstance().Dispose();
            }
            catch (Exception ex)
            {

            }
        }
        public System.Threading.Mutex mutex;
        /// <summary>
        /// 检测是否当前程序已打开
        /// </summary>
        /// <returns></returns>
        public bool CheckOneExe()
        {
            //one check 
            mutex = new System.Threading.Mutex(true, App.filenameMd5, out bool ret);
            if (!ret)
            {
                mutex.Close();
                Thread.Sleep(500);
                mutex = new System.Threading.Mutex(true, App.filenameMd5, out ret);

            }

            if (!ret)
            {
                try
                {
                    SendServer(App.currExeName);
                    ClientStreamDispose();
                    if (currConfig.ReOpenNotice)
                    {
                        //logShow.changeNoticeQueue($"当前目录程序已打开,不要浪费内存哦，请点确认后退出，显示已开的程序。");
                        MessageBox.Show($"当前目录程序已打开,不要浪费内存哦，请看右下角图标。", "", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                    App.app.Shutdown();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    mutex.Close();
                    Environment.Exit(0);
                }
                return false;
            }
            return ret;

        }

        private void MainRibbonWindow_StateChanged(object sender, EventArgs e)
        {
            //if (this.WindowState == WindowState.Minimized)
            //{
            //不让在任务栏显示。
            //this.ShowInTaskbar = false;
            //this.Visibility = System.Windows.Visibility.Hidden;
            //}
        }
        #region init NotifyIcon
        void initNotifyIcon()
        {
            this.notifyIcon = new Forms.NotifyIcon();
            this.notifyIcon.BalloonTipText = "一键启动多功能工具V8......";
            this.notifyIcon.ShowBalloonTip(2000);
            this.notifyIcon.Text = "一键启动多功能工具V8......";
            this.notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            this.notifyIcon.Visible = true;
            //打开菜单项
            System.Windows.Forms.ToolStripMenuItem open = new System.Windows.Forms.ToolStripMenuItem("打开");
            open.Click += new EventHandler(Show);
            //退出菜单项
            System.Windows.Forms.ToolStripMenuItem exit = new System.Windows.Forms.ToolStripMenuItem("直接退出");
            exit.Click += new EventHandler(Close);
            //关联托盘控件
            System.Windows.Forms.ToolStripItem[] childen = new System.Windows.Forms.ToolStripMenuItem[] { open, exit };
            notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.AddRange(childen);

            this.notifyIcon.MouseDoubleClick += notifyIcon_MouseClick;

        }

        /// <inheritdoc />
        protected override void OnInitialized(EventArgs e)
        {
            CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.CloseWindowCommand,
                (s, args) => Microsoft.Windows.Shell.SystemCommands.CloseWindow((Window)args.Parameter)));
            CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.MaximizeWindowCommand,
                (s, args) => Microsoft.Windows.Shell.SystemCommands.MaximizeWindow((Window)args.Parameter)));
            CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.MinimizeWindowCommand,
                (s, args) => Microsoft.Windows.Shell.SystemCommands.MinimizeWindow((Window)args.Parameter)));
            CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.RestoreWindowCommand,
                (s, args) => Microsoft.Windows.Shell.SystemCommands.RestoreWindow((Window)args.Parameter)));
            //Debug.Assert(this.Owner != null);
            base.OnInitialized(e);
        }

        public void isToShow(string flag = "tools")
        {
            App._mainRibbonWindow.Dispatcher.Invoke(delegate
            {
                //如果此窗口是隐藏模式就显示。
                if (App._mainRibbonWindow.Visibility == System.Windows.Visibility.Hidden || !this.IsActive)
                {
                    App._mainRibbonWindow.Visibility = System.Windows.Visibility.Visible;
                    if (App._mainRibbonWindow.WindowState == WindowState.Minimized)
                    {
                        Microsoft.Windows.Shell.SystemCommands.RestoreWindow(this);
                    }
                }
                //将窗口至于前台并激活。
                App._mainRibbonWindow.Activate();
            });
            if (flag == App.currExeName)
            {
                App._processCommunicationPipeToDo.ServerDisconnectAndInit();
                changeNoticeQueue("不得了，有同样的我，激活了我。。。。");
            }
        }
        private void Show(object sender, EventArgs e)
        {
            isToShow();
        }
        private void notifyIcon_MouseClick(object sender, Forms.MouseEventArgs e)
        {
            //如果点击了鼠标左键。
            if (e.Button == Forms.MouseButtons.Left)
            {

                //如果隐藏就显示，否则就隐藏。
                if (this.Visibility == System.Windows.Visibility.Visible || this.IsActive)
                {
                    //不让在任务栏显示。
                    //this.ShowInTaskbar = false;
                    this.Hide();
                }
                else
                {
                    isToShow();
                }
            }
        }

        private void Close(object sender, EventArgs e)
        {
            Closingtodo();
            App.app.Shutdown();
            Environment.Exit(0);
        }
        #endregion
        public void Closingtodo()
        {
            mutex?.Close();
            logShow._runShow = false;
            autoSave(currPathes);
            //TestServerHelper.GetInstance().Dispose();
            //initClient.GetInstanceLazy.DisconnectServer();
            //LogHelper.GetInstance().Dispose();
        }
        private void MainRibbonWindow_Closing(object sender, CancelEventArgs e)
        {
            if (ckCloseToHide.IsChecked.GetValueOrDefault())
            {
                //不让在任务栏显示。
                //this.ShowInTaskbar = false;
                this.Hide();
                e.Cancel = true;
                return;
            }
            var result = MessageBox.Show("确认是否退出，否：最小化到右下角任务栏中。。", "提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    this.Hide();
                    Closingtodo();
                    App.app.Shutdown();
                    Environment.Exit(0);
                    break;
                case MessageBoxResult.No:
                    this.Hide();
                    break;
                case MessageBoxResult.Cancel:
                default:
                    break;
            }
            e.Cancel = true;
        }

        private async void MainRibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await initLoad();
            mainwindows.Topmost = true;
            IsTop(mainwindows.Topmost);
        }
        #region windows todo
        void initFirst()
        {
            try
            {
                this.version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

                otherFilterDef = new List<string>();
                _allExeFile = new ObservableCollection<StackPanel>();
                _allExeFileData = new ObservableCollection<CheckBoxFlag>();
                _allProjectFile = new ObservableCollection<CheckBoxFlag>();
                collectionViewSource = new CollectionViewSource() { Source = _allExeFile };
                collectionViewSource.View.CurrentChanged += View_CurrentChanged;
                allLst.Items.Clear();
                allLst.ItemsSource = null;
                allLst.ItemsSource = collectionViewSource.View;
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;


                currPath = currPath.IsNullOrWhiteSpace() ? AppDomain.CurrentDomain.BaseDirectory : currPath;
                this.Title = this.version + "->当前目录:" + currPath;
                lblVersion.Content = $"V:{this.version}";
                selectDateTime.Value = DateTime.Now;

                initFirstOverHold();

                initIceServeFirst();

                initComboBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        async Task<bool> SetCurrPath(string tmppath)
        {
            this.Dispatcher.Invoke(() =>
            {
                currPath = tmppath;
                Title = $"{this.version}->当前目录:{currPath}";
                logShow.projectFilePath = currPath;

            });
            await LoadData("", true);
            return true;
        }
        void IsTop(bool flag)
        {
            try
            {

                if (flag)
                {
                    btntop.Content = "最顶";
                    btntop.ToolTip = "当前为最顶";
                    btntop.Foreground = Brushes.Red;
                    //btntop.Icon = new Uri("pack://application:,,,/Tools.OpenExe;component/Images/isTop.png", UriKind.Absolute);
                }
                else
                {
                    btntop.Content = "置顶";
                    btntop.ToolTip = "当前可置顶";
                    btntop.Foreground = Brushes.Black;
                    //btntop.Icon = new Uri("pack://application:,,,/Tools.OpenExe;component/Images/isUp.png", UriKind.Absolute);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void autoSave(List<string> tmpPaths, bool isSet = false)
        {
            if (isSet)
            {
                currPathes.Clear();
                currPathes.AddRange(tmpPaths);
            }
            var toJson = new OpenExeConfig()
            {
                configPath = currPath,
                toolconfigFilter = filterName.Text,
                configPaths = tmpPaths,
                toolconfigGetName = GetNameExt.Text,
                dellogpath = txtlogPath.Text,
                txtfixurl = txtfixurl.Text,
                CloseToHide = ckCloseToHide.IsChecked.GetValueOrDefault(),
                ReOpenNotice = ckReOpenNotice.IsChecked.GetValueOrDefault(),
                IceServerIp = txtServerIP.Text,
                IceServerPort = txtServerPort.Text,
                IceClientType = txtClientType.Text,
                IceClientId = txtClientId.Text,
                IceClientIp = txtClientIP.Text,
                IceClientPort = txtClientPort.Text,
                IceClientFilterNo = txtFilterServerNo.Text,
                copyPathList = copytoWin.GetPaths().HasItem() ? copytoWin.GetPaths() : currConfig.copyPathList
            };
            OpenExeConfigPath.FilePathSaveContent(toJson.toJsonStr(), Encoding.UTF8);

        }
        void loadConfig()
        {
            OpenExeConfigPath = AppDomain.CurrentDomain.BaseDirectory + "\\OpenExeConfig.json";

            var getContent = OpenExeConfigPath.FilePathGetContent(Encoding.UTF8);
            if (getContent.IsNotNullOrWhiteSpace())
            {
                currConfig = getContent.JsonTo<OpenExeConfig>();
            }
            else
            {
                currConfig = new OpenExeConfig();
            }
        }
        async Task initLoad()
        {
            try
            {
                loadConfig();
                //init value
                currPath = currConfig.configPath.IsNotNullOrWhiteSpace() ? currConfig.configPath : currPath;
                filterName.Text = currConfig.toolconfigFilter.IsNotNullOrWhiteSpace() ? currConfig.toolconfigFilter : filterName.Text;
                GetNameExt.Text = currConfig.toolconfigGetName.IsNotNullOrWhiteSpace() ? currConfig.toolconfigGetName : GetNameExt.Text;
                txtlogPath.Text = currConfig.dellogpath.IsNotNullOrWhiteSpace() ? currConfig.dellogpath : txtlogPath.Text;
                txtfixurl.Text = currConfig.txtfixurl.IsNotNullOrWhiteSpace() ? currConfig.txtfixurl : txtfixurl.Text;
                currPathes = currConfig.configPaths;
                ckCloseToHide.IsChecked = currConfig.CloseToHide;
                ckReOpenNotice.IsChecked = currConfig.ReOpenNotice;

                txtServerIP.Text = currConfig.IceServerIp.IsNotNullOrWhiteSpace() ? currConfig.IceServerIp : txtServerIP.Text;
                txtServerPort.Text = currConfig.IceServerPort.IsNotNullOrWhiteSpace() ? currConfig.IceServerPort : txtServerPort.Text;


                txtClientIP.Text = currConfig.IceClientIp.IsNotNullOrWhiteSpace() ? currConfig.IceClientIp : txtClientIP.Text;
                txtClientPort.Text = currConfig.IceClientPort.IsNotNullOrWhiteSpace() ? currConfig.IceClientPort : txtClientPort.Text;

                txtClientId.Text = currConfig.IceClientId.IsNotNullOrWhiteSpace() ? currConfig.IceClientId : txtClientId.Text;
                txtClientType.Text = currConfig.IceClientType.IsNotNullOrWhiteSpace() ? currConfig.IceClientType : txtClientType.Text;

                txtFilterServerNo.Text = currConfig.IceClientFilterNo.IsNotNullOrWhiteSpace() ? currConfig.IceClientFilterNo : txtFilterServerNo.Text;

                copytoWin.SetPaths(currConfig.copyPathList);

                if (!currPath.FileDirectoryExists())
                {
                    currPath = AppDomain.CurrentDomain.BaseDirectory;
                }
                if (!currPathes.HasItem())
                {
                    currPathes = new List<string>() { currPath };
                }
                this.Title = this.version + "->当前目录:" + currPath;
                projectFilePath = System.IO.Path.Combine(currPath, "方案");
                projectFilePath.FileDirectoryCreateDirectory();
                projectFileConfigPath = System.IO.Path.Combine(currPath, "方案", "Config");
                projectFileConfigPath.FileDirectoryCreateDirectory();

                var tmpTaskProject = initProject();

                //init logshow
                logShow.projectFilePath = projectFilePath;
                logShow.initProjectFunc = initProject;


                string toolconfigFilter = AppDomain.CurrentDomain.BaseDirectory + "\\toolconfigFilter.txt";
                var toFilter = toolconfigFilter.FilePathGetContent(Encoding.UTF8);

                if (toFilter.IsNotNullOrWhiteSpace() && filterName.Text.IsNullOrWhiteSpace())
                {
                    filterName.Text = toFilter;
                }
                string toolconfigGetName = AppDomain.CurrentDomain.BaseDirectory + "\\toolconfigGetNameExt.txt";
                var toFilterGetName = toolconfigGetName.FilePathGetContent(Encoding.UTF8);
                if (toFilterGetName.IsNotNullOrWhiteSpace() && GetNameExt.Text.IsNullOrWhiteSpace())
                {
                    GetNameExt.Text = toFilterGetName;
                }

                await tmpTaskProject;

                projectList.ItemsSource = _allProjectFile;
                projectList.DisplayMemberPath = "Name";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
        private void ZoomSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //var textFormattingMode = e.NewValue > 1.0 || Math.Abs(e.NewValue - 1.0) < double.Epsilon ? TextFormattingMode.Ideal : TextFormattingMode.Display;
            //TextOptions.SetTextFormattingMode(this, textFormattingMode);
        }

        #region Top Button      
        private void btnIsTop_Click(object sender, RoutedEventArgs e)
        {
            mainwindows.Topmost = !mainwindows.Topmost;
            IsTop(mainwindows.Topmost);
        }

        private async void btnLoadData_Click(object sender, RoutedEventArgs e)
        {
            await LoadData();
        }
        private void btnSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                autoSave(currPathes);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void btnRunExe_Click(object sender, RoutedEventArgs e)
        {
            var btn2 = sender as Button;
            btn2.IsEnabled = false;
            this.Cursor = Cursors.Wait;

            try
            {
                ShowLayoutDocumentPane("document1");
                var AllToStart = _allExeFileData.Where(a => a.IsChecked == true);
                int toI = 1;
                foreach (var item in AllToStart)
                {
                    changeNotice($"第{toI}/{AllToStart.Count()}个：{item.Content}:开始启动。");

                    await RunProcess(item.Content);

                    toI++;
                }
                changeNotice($"{AllToStart.Count()}个全部启动完成。");
                //刷新当前进程信
                _getCurrAllProcess = Process.GetProcesses();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btn2.IsEnabled = true;
                this.Cursor = Cursors.Arrow;

            }
        }
        private async void btnStopExe_Click(object sender, RoutedEventArgs e)
        {
            var btn3 = sender as Button;
            btn3.IsEnabled = false;
            this.Cursor = Cursors.Wait;
            try
            {
                ShowLayoutDocumentPane("document1");
                var isCloseByName = ckCloseByName.IsChecked.GetValueOrDefault();
                var AllToStop = _allExeFileData.Where(a => a.IsChecked);
                var exitProcessName = AllToStop.Select(a => new { filename = a.Content, a = a.Content.GetFileNameWithoutExtension(), b = a.IsDelLog && (a.IsShowDelLog == Visibility.Visible) ? a.Content.GetDirectoryName() : "" });

                //始启化 
                _getCurrAllProcess = Process.GetProcesses().ToList().Where(a => exitProcessName.Where(x => x.a.Contains(a.ProcessName)).Any()).ToArray();

                int toI = 1;
                foreach (var item in exitProcessName)
                {
                    changeNotice($"第{toI}/{AllToStop.Count()}个：{item.a}:开始关闭。");
                    await Task.Delay(1);
                    var listLogToDel = GetLogDelPath(item.b);
                    await KillProcess(item.filename, listLogToDel, isCloseByName);
                    await Task.Delay(1);
                    toI++;
                }
                changeNotice($"{AllToStop.Count()}个全部关闭完成。");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btn3.IsEnabled = true;
                this.Cursor = Cursors.Arrow;
            }
        }
        private void btnSetTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var flag = selectDateTime.Value.SetLocalTimeByDateTime();
                if (!flag)
                {
                    System.Windows.Forms.MessageBox.Show("修改当前电脑时间失败：" + selectDateTime.Value.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GetName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private async void ckIsHowRemoveLog_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(10);
            try
            {
                ShowLayoutDocumentPane("document1");
                var currcheck = ckIsShowRemoveLog;
                currcheck.IsEnabled = false;
                updateShowDelCheckBox();
                currcheck.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ckRemoveLog.IsEnabled = true;
            }

        }
        void updateShowDelCheckBox()
        {
            ShowLayoutDocumentPane("document1");
            foreach (var item in _allExeFile)
            {
                var model = item.DataContext.GetTModel<ViewModelNotify>();
                model.TrySetMember<CheckBoxFlag, Visibility>((a) => a.IsShowDelLog, ckIsShowRemoveLog.IsChecked.GetValueOrDefault() ? Visibility.Visible : Visibility.Collapsed);
            }
        }
        private async void ckRemoveLog_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(10);
            try
            {
                ShowLayoutDocumentPane("document1");
                if (!ckIsShowRemoveLog.IsChecked.GetValueOrDefault())
                {
                    ckIsShowRemoveLog.IsChecked = true;
                    updateShowDelCheckBox();
                }
                var currcheck = ckRemoveLog;
                currcheck.IsEnabled = false;
                foreach (var item in _allExeFile)
                {
                    var model = item.DataContext.GetTModel<ViewModelNotify>();
                    var currdata = model.GetT<CheckBoxFlag>();
                    if (currdata.IsChecked)
                    {
                        model.TrySetMember<CheckBoxFlag, bool>((a) => a.IsDelLog, currcheck.IsChecked.GetValueOrDefault());
                    }
                }
                currcheck.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                ckRemoveLog.IsEnabled = true;
            }

        }

        private void projectList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            initLoadProjcetByFilePath();
            sortBySelectCheck(false);
        }
        #endregion
        #region menu click
        #region menu fun
        async Task StartProcess()
        {
            if (allLst.SelectedItem == null)
            {
                return;
            }
            var item = allLst.SelectedItem.GetT<StackPanel, ViewModelNotify, CheckBoxFlag>();

            string path = item.Content;

            changeNotice($"{path}:开始启动。");

            await RunProcess(path);

            changeNotice($"{path}启动完成。");

            //刷新当前进程信
            _getCurrAllProcess = Process.GetProcesses();
        }
        async Task KillExit(bool isCloseByName = false)
        {
            if (allLst.SelectedItem == null)
            {
                return;
            }
            //刷新当前进程信
            _getCurrAllProcess = Process.GetProcesses();

            var item = allLst.SelectedItem.GetT<StackPanel, ViewModelNotify, CheckBoxFlag>();

            string path = item.Content.ToString();

            changeNotice($"{path}:开始关闭。");

            await KillProcess(path, null, isCloseByName);

            changeNotice($"{path}关闭完成。");
        }
        #endregion
        private void allLst_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;

                if (allLst.SelectedItem == null)
                {
                    return;
                }
                var model = allLst.SelectedItem.GetTModel<StackPanel, ViewModelNotify>();
                var tmpCheckflag = model.GetT<CheckBoxFlag>();
                model.TrySetMember<CheckBoxFlag, bool>((a) => a.IsChecked, !tmpCheckflag.IsChecked);
                if (!tmpCheckflag.IsChecked)
                {
                    model.TrySetMember<CheckBoxFlag, bool>((a) => a.IsDelLog, false);

                }
                changeNoticeQueue(tmpCheckflag.toJsonStr());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }
        private async void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                await StartProcess();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void MenuItem_Click_23(object sender, RoutedEventArgs e)
        {
            try
            {
                var isCloseByName = ckCloseByName.IsChecked.GetValueOrDefault();
                await KillExit(isCloseByName);
                await Task.Delay(1000);
                await StartProcess();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                if (allLst.SelectedItem == null)
                {
                    return;
                }
                var isCloseByName = ckCloseByName.IsChecked.GetValueOrDefault();
                await KillExit(isCloseByName);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            try
            {
                if (allLst.SelectedItem == null)
                {
                    return;
                }

                //刷新当前进程信
                _getCurrAllProcess = Process.GetProcesses();

                var item = allLst.SelectedItem.GetT<StackPanel, ViewModelNotify, CheckBoxFlag>();


                var isCloseByName = true;// ckCloseByName.IsChecked.GetValueOrDefault();
                string path = item.Content;
                string currdir = path.GetDirectoryName();

                changeNotice($"{path}:开始关闭并删除log。");
                var listLogToDel = GetLogDelPath(currdir);
                await KillProcess(path, listLogToDel, isCloseByName);

                changeNotice($"{path}关闭完成并删除log。");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void OpenFloder()
        {
            try
            {
                if (allLst.SelectedItem == null)
                {
                    return;
                }
                var getcurrItem = allLst.SelectedItem.GetT<StackPanel, ViewModelNotify, CheckBoxFlag>();
                FileGet.OpenFolderAndSelectFile(getcurrItem.Content);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFloder();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (allLst.SelectedItem == null)
                {
                    return;
                }
                var getcurrItem = allLst.SelectedItem.GetT<StackPanel, ViewModelNotify, CheckBoxFlag>();
                var fileName = getcurrItem.Content.GetFileNameWithoutExtension();
                if (fileName.isNull())
                {
                    return;
                }
                if (otherFilterDef.Contains(fileName))
                {
                    otherFilterDef.Remove(fileName);
                }
                else
                {
                    otherFilterDef.Add(fileName);
                }
                filterName.Text = string.Join(",", otherFilterDef.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                projectFilePath = System.IO.Path.Combine(currPath, "方案");
                projectFilePath.FileDirectoryCreateDirectory();

                var toSendPackage = _allExeFileData.Where(a => a.IsChecked);
                var toJson = toSendPackage.Select(a => new CheckBoxFlag() { Content = a.Content, IsChecked = true, IsDelLog = a.IsDelLog }).ToList().toJsonStr();

                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "json方案文件 (*.json)|*.json";
                saveFileDialog.Title = "保存当前选择方案";
                saveFileDialog.InitialDirectory = projectFilePath;
                var saveName = currPath.Replace(@"\", "`").Replace(@":", "`").Replace(@"``", "`").Split('`');

                var saveNameLast = saveName[saveName.Length - 1];
                if (saveName.Length > 2)
                {
                    saveNameLast = saveName[0] + "_m_" + saveName[saveName.Length - 2] + "_" + saveName[saveName.Length - 1];
                }
                saveFileDialog.FileName = $"方案_{saveNameLast}_" + DateTime.Now.Date.ToString("yyyyMMdd");
                if ((bool)(saveFileDialog.ShowDialog()))
                {
                    //获得文件路径
                    var localFilePath = saveFileDialog.FileName.ToString();
                    localFilePath.FilePathSaveContent(toJson, Encoding.UTF8);
                }
                await initProject();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            try
            {
                projectFilePath = System.IO.Path.Combine(currPath, "方案");
                projectFilePath.FileDirectoryCreateDirectory();

                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Filter = "json选择方案 (*.json)|*.json";
                openFileDialog.Title = "选择方案";
                openFileDialog.InitialDirectory = projectFilePath;
                if ((bool)openFileDialog.ShowDialog())
                {
                    string tmpPath = openFileDialog.FileName;
                    loadProjectByFilePath(tmpPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItem_Click_md5(object sender, RoutedEventArgs e)
        {
            try
            {
                projectFilePath = System.IO.Path.Combine(currPath, "方案Md5");
                projectFilePath.FileDirectoryCreateDirectory();

                var toSendPackage = _allExeFileData.Where(a => a.IsChecked);

                if (!toSendPackage.HasItem())
                {
                    MessageBox.Show("当前没有勾选文件.");
                    return;
                }
                StringBuilder tosaveCsv = new StringBuilder();
                //头
                tosaveCsv.AppendLine("文件名,版本号,MD5校验,dll文件MD5校验,备注");

                foreach (var item in toSendPackage)
                {
                    var md5all = GetMD5HashFromFileDit(item.Content);

                    tosaveCsv.AppendLine($"{item.Name},{item.Fileversion ?? "0"},{item.VersionmsgMd5},{md5all},{item.Content}");
                }

                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "csv方案Md5校验文件 (*.csv)|*.csv";
                saveFileDialog.Title = "保存当前选择方案Md5校验文件";
                saveFileDialog.InitialDirectory = projectFilePath;
                var saveName = currPath.Replace(@"\", "`").Replace(@":", "`").Replace(@"``", "`").Split('`');

                var saveNameLast = saveName[saveName.Length - 1];
                if (saveName.Length > 2)
                {
                    saveNameLast = saveName[0] + "_m_" + saveName[saveName.Length - 2] + "_" + saveName[saveName.Length - 1];
                }
                saveFileDialog.FileName = $"测试环境Md5_{saveNameLast}_" + DateTime.Now.Date.ToString("yyyyMMdd");
                if ((bool)(saveFileDialog.ShowDialog()))
                {
                    //获得文件路径
                    var localFilePath = saveFileDialog.FileName.ToString();
                    localFilePath.FilePathSaveContent(tosaveCsv.ToString(), Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItem_Click_md5_check(object sender, RoutedEventArgs e)
        {
            try
            {
                projectFilePath = currPath;// System.IO.Path.Combine(currPath, "方案Md5");
                projectFilePath.FileDirectoryCreateDirectory();

                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Filter = "csv方案Md5校验文件 (*.csv)|*.csv";
                openFileDialog.Title = "选择Md5校验文件";
                openFileDialog.InitialDirectory = projectFilePath;
                if ((bool)openFileDialog.ShowDialog())
                {
                    string tmpPath = openFileDialog.FileName;
                    CheckMD5fileCsv(tmpPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        Dictionary<string, LayoutDocument> allCopyWindows = new Dictionary<string, LayoutDocument>();
        private void MenuItem_Click_Copyto_new(object sender, RoutedEventArgs e)
        {
            try
            {
                if (allLst.SelectedItem == null)
                {
                    return;
                }
                var allSelect = _allExeFileData.Where(a => a.IsChecked);
                var getcurrItem = allLst.SelectedItem.GetT<StackPanel, ViewModelNotify, CheckBoxFlag>();

                var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                if (NummberLayoutDocument <= 0)
                {
                    NummberLayoutDocument = firstDocumentPane.Children.Count;
                }
                NummberLayoutDocument++;
                var tmpId = $"CopyDcoument{NummberLayoutDocument}";
                LayoutDocument doc;
                if (allCopyWindows.ContainsKey(tmpId))
                {
                    doc = allCopyWindows[tmpId];
                }
                else
                {
                    var tmptitle = $"目录复制{NummberLayoutDocument}";
                    doc = new LayoutDocument
                    {
                        Title = tmptitle,
                        ContentId = tmpId
                    };
                    allCopyWindows[tmpId] = doc;
                }

                var tmpcopytoWin = new CopyTo_UC();
                initCopyTo(tmpcopytoWin, allSelect, getcurrItem);
                doc.Content = tmpcopytoWin;
                firstDocumentPane.Children.Add(doc);
                addToMenu(doc);
                doc.IsActive = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void addToMenu(LayoutDocument doc)
        {
            if (doc == null)
            {
                return;
            }
            var menu = new MenuItem() { Header = doc.Title, Tag = doc };
            menu.Click += openmenudoc;
            menulist.Items.Add(menu);
        }
        private void openmenudoc(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem item = sender as MenuItem;

                LayoutDocument doc = item.Tag as LayoutDocument;

                ShowAndAddLayoutDocumentPane(doc);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }
        void RemoveToMenu(string title)
        {
            try
            {

                if (title.IsNullOrWhiteSpace())
                {
                    return;
                }
                //menulist.Dispatcher.Invoke(() =>
                {
                    for (int i = 0; i < menulist.Items.Count; i++)
                    {
                        MenuItem item = menulist.Items[i] as MenuItem;
                        if (item == null)
                        {
                            continue;
                        }
                        if (item.Header.NullToStr() == title)
                        {
                            menulist.Items.RemoveAt(i);
                            NummberLayoutDocument--;
                            break;
                        }
                    }
                }
                //);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        void initCopyTo(CopyTo_UC tmpcopytoWin, IEnumerable<CheckBoxFlag> allSelect, CheckBoxFlag getcurrItem)
        {
            tmpcopytoWin.SetDestFilePath(getcurrItem.Content.NullToStr());
            tmpcopytoWin.currPath = currPath;
            if (allSelect.HasItem())
            {
                foreach (var item in allSelect)
                {
                    if (item.Content == getcurrItem.Content)
                    {
                        continue;
                    }
                    tmpcopytoWin.SetCopyToFilePath(item.Content.NullToStr());
                    break;
                }

            }
        }
        private void MenuItem_Click_Copyto(object sender, RoutedEventArgs e)
        {
            //复制到另一目录
            try
            {
                if (allLst.SelectedItem == null)
                {
                    return;
                }
                ShowLayoutDocumentPane("document2");

                var allSelect = _allExeFileData.Where(a => a.IsChecked);

                var getcurrItem = allLst.SelectedItem.GetT<StackPanel, ViewModelNotify, CheckBoxFlag>();

                initCopyTo(copytoWin, allSelect, getcurrItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItem_Click_top(object sender, RoutedEventArgs e)
        {
            mainwindows.Topmost = !mainwindows.Topmost;
            IsTop(mainwindows.Topmost);
        }
        #region Log Search

        private void ckLogSearch_Click(object sender, RoutedEventArgs e)
        {
            var btn = ((Button)sender);
            this.Cursor = Cursors.Wait;
            try
            {
                ShowLayoutDocumentPane("document1");
                btn.IsEnabled = false;

                if (txtFilter.Text.IsNullOrWhiteSpace() && txtFilterNo.Text.IsNullOrWhiteSpace())
                {
                    return;
                }

                var toSendPackage = _allExeFileData.Where(a => a.IsChecked);

                if (!toSendPackage.HasItem())
                {
                    MessageBox.Show("当前没有勾选文件.");
                    return;
                }
                bool isFind = false;
                StringBuilder sb = new StringBuilder();
                foreach (var item in toSendPackage)
                {
                    var result = item.Content.FilePathGetAllLineSearchAll(txtFilter.Text, txtFilterNo.Text, "", Encoding.UTF8, txtTopCount.Text.TryToInt());
                    if (result.Count > 0)
                    {
                        sb.Clear();
                        sb.AppendLine($"{item.Content}:");
                        foreach (var tmpstr in result)
                        {
                            sb.AppendLine($"行号[{tmpstr.Key}]->{tmpstr.Value}");
                        }
                        changeNoticeQueue(sb.ToString(), true, 1);
                        isFind = true;
                    }
                    else
                    {
                        //currck.IsChecked = false;
                        //item.Tag = false;
                        //(item.Children[0] as CheckBox).IsChecked = false;
                    }
                }
                if (!isFind)
                {
                    changeNoticeQueue("没有查到任何文件符合！", true, 2);
                }

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
        private async void fixLogSearch_Click(object sender, RoutedEventArgs e)
        {
            var btn = ((Button)sender);
            this.Cursor = Cursors.Wait;
            try
            {
                ShowLayoutDocumentPane("document1");
                btn.IsEnabled = false;

                var toSendPackage = _allExeFileData.Where(a => a.IsChecked);

                if (!toSendPackage.HasItem())
                {
                    MessageBox.Show("当前没有勾选文件.");
                    return;
                }
                var allTextPath = toSendPackage.Select(a => a.Content).Distinct().ToList();
                bool isFind = false;

                logShow.Clear();
                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("\n[tag]->计数");
                        var result = new Dictionary<string, int>();
                        int currCount = 0;

                        Regex regex = new Regex(@"\d+=");
                        foreach (var item in allTextPath)
                        {
                            currCount = item.ToString().FilePathGetAllLineSearchAll(result, Encoding.UTF8, regex);
                            changeNoticeQueue($"找到:{currCount}个,{item}.");
                        }
                        if (result.Count > 0)
                        {
                            var tmpresult = new Dictionary<int, int>();
                            foreach (var item in result)
                            {
                                var tmpkey = item.Key.TrimEnd('=').TryToInt();
                                tmpresult[tmpkey] = item.Value;
                            }
                            var toPrint = tmpresult.OrderBy(a => a.Key);
                            foreach (var tmpstr in toPrint)
                            {
                                sb.AppendLine($"[{tmpstr.Key}=]->{tmpstr.Value}");
                            }
                            changeNoticeQueue(sb.ToString());
                            isFind = true;
                        }
                        if (!isFind)
                        {
                            changeNoticeQueue("没有查到任何文件符合！", true, 2);
                        }

                    }
                    catch (Exception ex)
                    {
                        changeNoticeQueue(ex.Message, true, 2);
                    }
                });
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

        private async void iceBinPackage_Click(object sender, RoutedEventArgs e)
        {
            var btn = ((Button)sender);
            this.Cursor = Cursors.Wait;
            try
            {
                ShowLayoutDocumentPane("document1");
                btn.IsEnabled = false;

                var toSendPackage = _allExeFileData.Where(a => a.IsChecked);

                if (!toSendPackage.HasItem())
                {
                    MessageBox.Show("当前没有勾选文件.");
                    return;
                }
                var allTextPath = toSendPackage.Select(a => a.Content).Distinct().ToList();

                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        changeNoticeQueue("\n");
                        changeNoticeQueue("*****解包类型 Item1: 1,9:SlmOrderBody 2,10:SlmOrderJourBody 3:SendOrderStatusBody 4,11:SlmMatchBody 5:PushOrderStateBody 6:PushMatchStateBody  7:SlmPositionRow 8:SlmCloseRow 12:Deposits", true, 7);
                        PackageOrderType currType;
                        StringBuilder stringBuilder = new StringBuilder();

                        CommunicatorClientHelper communicatorClientHelper = new CommunicatorClientHelper();
                        var iOs = communicatorClientHelper.GetInstanceOs();
                        var v = communicatorClientHelper.GetEncodingVersion();
                        foreach (var item in allTextPath)
                        {
                            try
                            {
                                //解包
                                changeNoticeQueue($"开始解包:{item}", true, 1);
                                var tmpPackageBytes = LogHelper.GetInstance().ReadByteFileByPathDic(item);
                                stringBuilder.Clear();
                                foreach (var item2 in tmpPackageBytes)
                                {
                                    foreach (var item3 in item2.Value)
                                    {
                                        stringBuilder.AppendLine((new Tuple<PackageOrderType, object>(item2.Key, item2.Key.GetIceObject(item3, iOs, v))).toJsonStr());
                                    }
                                }
                                var tmpOtherFileName = item.GetDirectoryName() + "\\" + $"{item.GetFileNameWithoutExtension()}_解包.txt";
                                tmpOtherFileName.FilePathSaveContent(stringBuilder.ToString(), Encoding.UTF8);
                                changeNoticeQueue($"解包完成,另存为:{tmpOtherFileName}", true, 6);
                            }
                            catch (Exception ex)
                            {
                                changeNoticeQueue($"解包失败{item},下一个", true, 2);
                                changeNoticeQueue(ex.Message, true, 2);
                            }
                        }
                        changeNoticeQueue($"*****全部解包完成,共{allTextPath.Count}个", true, 6);

                    }
                    catch (Exception ex)
                    {
                        changeNoticeQueue(ex.Message, true, 2);
                    }
                });
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
        #endregion
        #region listbox to do

        private void addCustomerFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.Filter = "所有文件 (*.*)|*.*";
                openFileDialog.Title = "所有文件";
                openFileDialog.InitialDirectory = currPath;
                if ((bool)openFileDialog.ShowDialog())
                {
                    string tmpPath = openFileDialog.FileName;

                    if (!tmpPath.FileExists())
                    {
                        MessageBox.Show($"文件不存在：{tmpPath}");
                        return;
                    }

                    FileInfo fileInfo = new FileInfo(tmpPath);
                    var stackPanel = GetStack(fileInfo, _allExeFile.Count + 1);
                    _allExeFile.Add(stackPanel.Item1);
                    _allExeFileData.Add(stackPanel.Item2);
                    changeNotice($"添加临时文件成功：{tmpPath}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in _allExeFile)
            {
                var model = item.DataContext.GetTModel<ViewModelNotify>();
                model.TrySetMember<CheckBoxFlag, bool>((a) => a.IsChecked, cbAll.IsChecked);
                //if (ckIsShowRemoveLog.IsChecked.GetValueOrDefault())
                {
                    model.TrySetMember<CheckBoxFlag, bool>((a) => a.IsDelLog, cbAll.IsChecked);
                }
            }
            projectList.SelectedIndex = -1;
        }

        private async void sortBySelect_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(10);
            sortBySelectCheck();
        }
        private async void SearchFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (sender == null || _allExeFile_bak == null)
                {
                    return;
                }
                await Task.Delay(10);
                var send = sender as TextBox;
                _allExeFile.Clear();

                IEnumerable<CheckBoxFlag> listFilte = _allExeFile_bak.Select(a => a.GetT<StackPanel, ViewModelNotify, CheckBoxFlag>());
                //清除选择标记
                foreach (var item in listFilte)
                {
                    item.IsChecked = false;
                    item.IsDelLog = false;
                }
                if (send.Text.IsNotNullOrWhiteSpace() && _allExeFile_bak.Length > 0)
                {
                    listFilte = listFilte.Where(a => a.Content.Contains(send.Text, StringComparison.InvariantCultureIgnoreCase));
                }

                var count = 1;
                foreach (var item in listFilte)
                {
                    item.IsChecked = cbAll.IsChecked.GetValueOrDefault();
                    item.IsDelLog = cbAll.IsChecked.GetValueOrDefault();

                    item.Index = count;
                    var stackPanel = GetStack(item);
                    _allExeFile.Add(stackPanel);
                    count++;
                }

                if (listFilte.Count() == _allExeFile_bak.Length)
                {
                    initLoadProjcetByFilePath();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
        #endregion
        #region com method
        async Task LoadData(string fileExt = "", bool isProject = false)
        {
            var currbtn = btnLoadData;
            try
            {
                if (isLoad)
                {
                    return;
                }
                ShowLayoutDocumentPane("document1");

                isLoad = true;
                currbtn.IsEnabled = false;
                this.Cursor = Cursors.Wait;
                bool isDelFlag = ckRemoveLog.IsChecked.GetValueOrDefault();

                //lock (this)
                {
                    await LoadExeFile(fileExt, isDelFlag);
                }

                projectList.SelectedIndex = -1;
                projectFilePath = System.IO.Path.Combine(currPath, "方案");
                if (isProject)
                {
                    await initProject();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
                currbtn.IsEnabled = true;
                isLoad = false;
            }

        }
        public async Task initProject()
        {
            try
            {
                var toNotnames = new List<string>();
                string getFileName = ".json";
                FileGet.allGetFiles.Clear();
                await FileGet.getFileAsync(projectFilePath, getFileName, toNotnames);

                var getAllExe = FileGet.allGetFiles;
                _allProjectFile.Clear();
                foreach (var item in getAllExe)
                {
                    _allProjectFile.Add(new CheckBoxFlag() { IsDelLog = false, Content = item.FullName, Name = item.Name });
                }
            }
            catch (Exception ex)
            {
                changeNotice(ex.Message);
            }
        }
        private async Task LoadExeFile(string fileExt = "", bool isDel = false)
        {
            _allExeFile.Clear();
            _allExeFileData.Clear();
            otherFilterDef.Clear();
            string currName = Process.GetCurrentProcess().MainModule.ModuleName;//获得当前执行的exe的文件名。 

            var toNotnames = new List<string>() { currName, "Tools.OpenExe" };
            if (filterName.Text.IsNullOrWhiteSpace())
            {
                filterName.Text = "updater,upgrade,other";
            }
            otherFilterDef = filterName.Text.Split(",;，".ToCharArray()).ToList<string>();
            if (otherFilterDef.Any())
            {
                toNotnames.AddRange(otherFilterDef);
            }
            var getFileName = fileExt.IsNullOrWhiteSpace() ? this.GetNameExt.Text : fileExt;
            if (getFileName.Length <= 0)
            {
                getFileName = ".exe";
            }

            FileGet.allGetFiles.Clear();
            await FileGet.getFileAsync(currPath, getFileName, toNotnames.Distinct().ToList());
            var getAllExe = FileGet.allGetFiles;
            int number = 1;
            foreach (var item in getAllExe)
            {
                try
                {
                    var checkExist = _allExeFileData.Where(a => a.Content == item.FullName).Count() > 0;
                    if (!checkExist)
                    {
                        var stackPanel = GetStack(item, number, isDel);
                        _allExeFile.Add(stackPanel.Item1);
                        _allExeFileData.Add(stackPanel.Item2);
                        number++;
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            _allExeFile_bak = new StackPanel[_allExeFile.Count];
            _allExeFile.CopyTo(_allExeFile_bak, 0);


            changeNotice($"加载[{getFileName}]文件数：{_allExeFile.Count}");
        }
        public Tuple<StackPanel, CheckBoxFlag> GetStack(FileInfo item, int number, bool isDel = false)
        {
            var fileversion = item.FullName.GetFileVerion();
            var versionmsg = $"{fileversion?.FileVersion ?? "0"}";
            var versionmsgMd5 = GetMD5HashFromFile(item.FullName);


            //实例对象
            CheckBoxFlag checkBoxFlag = new CheckBoxFlag { Id = number, Index = number, IsChecked = true, IsDelLog = isDel, IsShowDelLog = ckIsShowRemoveLog.IsChecked.GetValueOrDefault() ? Visibility.Visible : Visibility.Collapsed, Content = item.FullName, Name = item.Name, Fileversion = versionmsg, VersionmsgMd5 = versionmsgMd5 };

            var stackPanel = GetStack(checkBoxFlag);
            return new Tuple<StackPanel, CheckBoxFlag>(stackPanel, checkBoxFlag);
        }

        public StackPanel GetStack(CheckBoxFlag checkBoxFlag)
        {
            //Notify代理
            ViewModelNotify currModel = new ViewModelNotify() { Model = new DynamicProxy(checkBoxFlag) };
            //记数
            var labelNum = new Label();
            //checkbox
            var stackPanel = new StackPanel() { Orientation = Orientation.Horizontal, DataContext = currModel };
            var tmpcheckBoxDel = new CheckBox() { ToolTip = "删除log" };
            var tmpcheckBoxMain = new CheckBox() { ToolTip = checkBoxFlag.Name };

            //binding
            labelNum.SetBinding(Label.ContentProperty, new Binding("Model.Index") { Mode = BindingMode.TwoWay });
            tmpcheckBoxDel.SetBinding(CheckBox.IsCheckedProperty, new Binding("Model.IsDelLog") { Mode = BindingMode.TwoWay });
            tmpcheckBoxDel.SetBinding(CheckBox.VisibilityProperty, new Binding("Model.IsShowDelLog") { Mode = BindingMode.TwoWay });
            tmpcheckBoxMain.SetBinding(CheckBox.IsCheckedProperty, new Binding("Model.IsChecked") { Mode = BindingMode.TwoWay });
            stackPanel.SetBinding(StackPanel.TagProperty, new Binding("Model.IsChecked") { Mode = BindingMode.TwoWay });

            //add
            stackPanel.Children.Add(labelNum);
            stackPanel.Children.Add(tmpcheckBoxDel);
            stackPanel.Children.Add(tmpcheckBoxMain);

            //textshow
            TextBlock contentFullname = new TextBlock() { Text = checkBoxFlag.Content, ToolTip = checkBoxFlag.Name, Foreground = Brushes.Black, VerticalAlignment = VerticalAlignment.Center };
            TextBlock labelVersion = new TextBlock() { Text = checkBoxFlag.Fileversion, Margin = new Thickness(5, 0, 0, 0), Foreground = Brushes.Red, VerticalAlignment = VerticalAlignment.Center };
            TextBlock labelMd5 = new TextBlock() { Text = checkBoxFlag.VersionmsgMd5Short, ToolTip = checkBoxFlag.VersionmsgMd5, Margin = new Thickness(5, 0, 0, 0), Foreground = Brushes.Black, VerticalAlignment = VerticalAlignment.Center };

            stackPanel.Children.Add(contentFullname);
            stackPanel.Children.Add(labelVersion);
            stackPanel.Children.Add(labelMd5);

            return stackPanel;
        }

        private void CheckMD5fileCsv(string tmpPath)
        {
            //当前选择文件
            var toSendPackage = _allExeFile.Where(a => a.GetT<StackPanel, ViewModelNotify, CheckBoxFlag>().IsChecked);

            if (!toSendPackage.HasItem())
            {
                MessageBox.Show("当前没有勾选文件.");
                return;
            }

            var getContent = tmpPath.FilePathGetContentAllLine(Encoding.UTF8);
            //csv校验文件
            var getCsvFileCheckMd5 = new VersionCheckMd5Lst();
            getCsvFileCheckMd5.loadLine(getContent);

            if (!getCsvFileCheckMd5.HasItem())
            {
                MessageBox.Show($"校验文件无记录：{tmpPath}.");
                return;
            }

            //当前选择文件md5
            var getCurrTocheckList = new VersionCheckMd5LstCheck();
            foreach (var item in toSendPackage)
            {
                var currck = item.GetT<StackPanel, ViewModelNotify, CheckBoxFlag>();

                var md5all = GetMD5HashFromFileDit(currck.Content);

                var listStr = $"{currck.Name},{currck.Fileversion ?? "0"},{currck.VersionmsgMd5},{md5all},{currck.Content}";
                getCurrTocheckList.loadLine(listStr, item);
            }
            if (!getCurrTocheckList.HasItem())
            {
                MessageBox.Show($"当前选择文件无生成版本及md5校验");
                return;
            }
            //验测，当前选择是否为，csv校验文件版本及记录
            var msg = "";
            string errormsg = "";
            foreach (var item in getCurrTocheckList)
            {
                var currStack = item.StackPanel;

                VersionCheckMd5 getExitCheck;

                getCsvFileCheckMd5.cacheDic.TryGetValue(item.md5, out getExitCheck);

                if (getExitCheck == null || (getExitCheck != null && item.md5All.IsNotNullOrWhiteSpace() && getExitCheck.md5All != item.md5All))
                {
                    if (getExitCheck != null && getExitCheck.md5All != item.md5All)
                    {
                        msg = "dll对应md5不一样。";
                        currStack.Background = Brushes.Yellow;
                    }
                    else
                    {
                        var listName = getCsvFileCheckMd5.Where(a => a.name == item.name);
                        msg = "";
                        if (listName.HasItem())
                        {
                            foreach (var a in listName)
                            {
                                msg += $"{a.name}:{a.version}_{a.md5.Substring(a.md5.Length - 5)}\n";
                            }

                        }
                        currStack.Background = Brushes.LightPink;
                    }

                    currStack.ToolTip = $"当前验证不成功，{msg}";
                    errormsg += item.name + ",";
                }
                else
                {

                    currStack.Background = Brushes.LightGreen;
                    currStack.ToolTip = null;
                }

            }
            //csv中有，没有选择，提示
            var allSelectMd5 = getCurrTocheckList.Select(a => a.md5);
            var diffEXE = getCsvFileCheckMd5.cacheDic.Values.Where(a => !allSelectMd5.Contains(a.md5)).ToList();

            if (diffEXE.HasItem())
            {
                errormsg += $",还有【{diffEXE.Select(b => b.name).Aggregate((a, b) => { return a + "," + b; })}】未校验。";
            }
            //error
            if (!errormsg.IsNullOrWhiteSpace())
            {
                MessageBox.Show($"Error:校验不通过:{errormsg}");
            }
            else
            {
                MessageBox.Show($"{getCurrTocheckList.Count()}个程序校验全部通过");
            }

        }
        /// <summary>
        /// 获取所有文件夹文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetMD5HashFromFileDit(string fileName, string filteExt = ".dll")
        {
            try
            {
                var dirFilePath = System.IO.Path.GetDirectoryName(fileName);

                FileGet.allGetFiles.Clear();
                FileGet.getFile(dirFilePath, filteExt, null, false);

                StringBuilder sb = new StringBuilder();
                string md5str = string.Empty;
                var allFileSort = FileGet.allGetFiles.OrderBy(a => a.FullName);
                if (!allFileSort.HasItem())
                {
                    return "";
                }
                foreach (var item in allFileSort)
                {
                    md5str = GetMD5HashFromFile(item.FullName);
                    sb.Append($",{md5str}");
                }
                md5str = sb.ToString().MD5Encrypt();
                return md5str;
            }
            catch (Exception ex)
            {
                return "GetMD5HashFromFileDit() fail,error:" + ex.Message;
            }
        }
        public string GetMD5HashFromFile(string fileName)
        {
            try
            {
                var md5file = fileName.GetMD5HashFromFile();
                cacheMd5File[fileName] = md5file;
                return md5file;
            }
            catch (Exception ex)
            {
                return "GetMD5HashFromFile() fail,error:" + ex.Message;
            }
        }
        public void changeNotice(string msg)
        {
            try
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {

                    try
                    {
                        this.lblmsg.Content = msg;
                    }
                    catch { }
                }));
            }
            catch { }
        }
        #endregion
        #region init project
        private void sortBySelectCheck(bool isSort = true)
        {
            ScrollToTop(allLst);
            if (collectionViewSource.SortDescriptions.Count > 0)
            {
                collectionViewSource.SortDescriptions.Clear();
                if (isSort)
                {
                    return;
                }
            }
            collectionViewSource.SortDescriptions.Add(new SortDescription("Tag", ListSortDirection.Descending));
        }
        void ScrollToTop(ListBox listBox)
        {
            Decorator decorator = (Decorator)VisualTreeHelper.GetChild(listBox, 0);
            ScrollViewer scrollViewer = (ScrollViewer)decorator.Child;
            scrollViewer.ScrollToTop();
        }
        void ScrollToEnd(ListBox listBox)
        {
            Decorator decorator = (Decorator)VisualTreeHelper.GetChild(listBox, 0);
            ScrollViewer scrollViewer = (ScrollViewer)decorator.Child;
            scrollViewer.ScrollToEnd();
        }
        private async void View_CurrentChanged(object sender, EventArgs e)
        {

            var data = sender as ListCollectionView;
            if (data.Count > 0)
            {
                await updateSortNum(data);
            }
        }
        async Task updateSortNum(ListCollectionView data)
        {
            await Task.Delay(10);
            int number = 1;
            foreach (StackPanel item in data)
            {
                var model = item.DataContext.GetTModel<ViewModelNotify>();
                model.TrySetMember<CheckBoxFlag, int>((a) => a.Index, number);
                number++;
            }
        }
        void initLoadProjcetByFilePath()
        {
            try
            {
                ShowLayoutDocumentPane("document1");
                if (projectList.SelectedIndex > -1)
                {
                    var currItem = projectList.SelectedItem as CheckBoxFlag;
                    string tmpPath = currItem.Content;
                    loadProjectByFilePath(tmpPath);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void loadProjectByFilePath(string tmpPath)
        {
            ShowLayoutDocumentPane("document1");
            ScrollToTop(allLst);
            var getContent = tmpPath.FilePathGetContent(Encoding.UTF8);
            var getlistCheckList = getContent.JsonTo<List<CheckBoxFlag>>();
            foreach (var item in _allExeFile)
            {
                var model = item.DataContext.GetTModel<ViewModelNotify>();
                var currDataFlag = model.GetT<CheckBoxFlag>();

                var isExist = getlistCheckList.Where(a => a.Content == currDataFlag.Content).FirstOrDefault();

                if (isExist != null)
                {
                    model.TrySetMember<CheckBoxFlag, bool>((a) => a.IsChecked, true);
                    model.TrySetMember<CheckBoxFlag, bool>((a) => a.IsDelLog, isExist.IsDelLog);
                }
                else
                {
                    model.TrySetMember<CheckBoxFlag, bool>((a) => a.IsChecked, false);
                    model.TrySetMember<CheckBoxFlag, bool>((a) => a.IsDelLog, false);
                }
            }
        }
        #endregion
        #region Process to do
        public async Task RunProcess(string path)
        {
            if (!path.EndsWith("exe", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            await Task.Delay(1);
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(path);
            info.WorkingDirectory = System.IO.Path.GetDirectoryName(path);
            System.Diagnostics.Process.Start(info);

            await Task.CompletedTask;
        }
        public async Task KillProcess(string fileName, List<string> delPath = null, bool isCloseByName = false)//关闭线程
        {
            try
            {
                if (!_getCurrAllProcess.HasItem())
                {
                    FileDirectoryDelete(delPath);
                    return;
                }
                foreach (Process p in _getCurrAllProcess)
                {
                    try
                    {
                        string toKillProcessName = isCloseByName ? System.IO.Path.GetFileNameWithoutExtension(fileName) : fileName;
                        bool isToClose = isCloseByName ? p.ProcessName.Equals(toKillProcessName) : p.MainModule.FileName.Equals(toKillProcessName);
                        if (isToClose)
                        {
                            await Task.Factory.StartNew(() =>
                            {
                                var currp = p;
                                try
                                {
                                    currp.Kill();
                                    currp.CloseMainWindow();
                                    currp.Close();
                                    currp.WaitForExit(100);
                                    currp.Dispose();
                                    FileDirectoryDelete(delPath);
                                }
                                catch (Exception ex)
                                {
                                    //currp.CloseMainWindow();
                                }
                            });

                            changeNotice($"{fileName}:关闭完成。");
                        }
                        else
                        {
                            FileDirectoryDelete(delPath);
                        }

                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
                await Task.Delay(1);
            }
            catch (Win32Exception e)
            {
                // MessageBox.Show(e.Message.ToString());   // process was terminating or can't be terminated - deal with it
            }
            catch (InvalidOperationException e)
            {
                //  MessageBox.Show(e.Message.ToString()); // process has already exited - might be able to let this one go
            }
            catch (Exception e)
            {
                //  MessageBox.Show(e.Message.ToString());
            }
        }
        public void FileDirectoryDelete(List<string> delPath)
        {
            try
            {
                if (delPath == null)
                {
                    return;
                }
                foreach (var item in delPath)
                {
                    item.FileDirectoryDelete();
                }

            }
            catch (Exception ex)
            {

            }
        }
        public List<string> GetLogDelPath(string rootpath)
        {

            var listLogToDel = new List<string>() { };
            if (rootpath.IsNullOrWhiteSpace())
            {
                return listLogToDel;
            }
            try
            {
                if (listLogpaht.Count <= 0)
                {
                    initlistLogpaht();
                }
                foreach (var tmplog in listLogpaht)
                {
                    var tmplogpath = System.IO.Path.Combine(rootpath, tmplog);
                    listLogToDel.Add(tmplogpath);
                }
                return listLogToDel;

            }
            catch (Exception ex)
            {
                return listLogToDel;
            }
        }
        public void initlistLogpaht()
        {
            listLogpaht = txtlogPath.Text.SplitTrim(',');
            if (!listLogpaht.HasItem())
            {
                listLogpaht = new List<string>() { "log" };
            }
        }


        #endregion

        #region 切换目录
        private ObservableCollection<string> _cacheListBinding = new ObservableCollection<string>();
        public ConcurrentDictionary<string, string> _allFileDit = new ConcurrentDictionary<string, string>();
        public Func<string, Task<bool>> SetCurrPathAction { get; set; }
        public Action<List<string>, bool> AutoSaveAction { get; set; }
        /// <summary>
        /// 切换目录
        /// </summary>
        public void initFirstOverHold()
        {
            allfileslstbox.ItemsSource = _cacheListBinding;

        }
        public void ClosingToUpdateData()
        {
            AutoSaveAction?.Invoke(_allFileDit.Keys.ToList(), true);
        }
        public void initDic(List<string> files)
        {
            foreach (var item in files)
            {
                _allFileDit[item] = item;
            }
            initData();
        }
        void initData()
        {
            _cacheListBinding.Clear();
            foreach (var item in _allFileDit)
            {
                _cacheListBinding.Add(item.Key);
            }
        }
        private void btnselectFileDataR_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.Description = "请选择文件路径";
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                if (!currPath.IsNullOrWhiteSpace())
                {
                    dialog.SelectedPath = currPath;
                }
                if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                txtfilePathData.Text = dialog.SelectedPath;
                if (_allFileDit.ContainsKey(currPath))
                {
                    return;
                }
                _allFileDit[currPath] = currPath;
                initData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void addtoList()
        {
            try
            {
                if (txtfilePathData.Text.Trim().Trim('\\').FileDirectoryExists())
                {
                    var tmpcurrPath = txtfilePathData.Text.Trim().Trim('\\');
                    if (_allFileDit.ContainsKey(tmpcurrPath))
                    {
                        return;
                    }
                    _allFileDit[tmpcurrPath] = tmpcurrPath;
                    initData();
                }
                else
                {
                    MessageBox.Show($"当前目录不存在:{txtfilePathData.Text}");
                    txtfilePathData.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            addtoList();
        }

        private async void btnSet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (allfileslstbox.SelectedItem != null)
                {
                    var dd = allfileslstbox.SelectedItem.NullToStr();
                    var result = await SetCurrPathAction?.Invoke(dd);
                    Title = $"系统设置->当前目录:{currPath}";
                    MessageBox.Show($"设置当前目录：{dd} 成功。");
                }
                else
                {
                    MessageBox.Show("请选择一条目录");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (allfileslstbox.SelectedItem != null)
                {
                    var dd = allfileslstbox.SelectedItem.NullToStr();
                    _allFileDit.TryRemove(dd, out _);
                    initData();
                }
                else
                {
                    MessageBox.Show("请选择一条目录");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void allfileslstbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;

                if (allfileslstbox.SelectedItem != null)
                {
                    var dd = allfileslstbox.SelectedItem.NullToStr();
                    var result = await SetCurrPathAction?.Invoke(dd);
                    this.Backstage.IsOpen = false;
                }
                else
                {
                    MessageBox.Show("请选择一条目录");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void txtfilePathData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                addtoList();
            }
        }

        private void allfileslstbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                if (allfileslstbox.SelectedItem != null)
                {
                    var dd = allfileslstbox.SelectedItem.NullToStr();
                    txtfilePathData.Text = dd;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void Backstage_IsOpenChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                //init action
                SetCurrPathAction = SetCurrPath;
                AutoSaveAction = autoSave;
                initDic(currPathes);
            }
            else
            {
                ClosingToUpdateData();
            }
        }





        #endregion

        #region AvalonDock
        #region FocusedElement

        /// <summary>
        /// FocusedElement Dependency Property
        /// </summary>
        public static readonly DependencyProperty FocusedElementProperty =
            DependencyProperty.Register("FocusedElement", typeof(string), typeof(MainWindow),
                new FrameworkPropertyMetadata((IInputElement)null));

        /// <summary>
        /// Gets or sets the FocusedElement property.  This dependency property 
        /// indicates ....
        /// </summary>
        public string FocusedElement
        {
            get => (string)GetValue(FocusedElementProperty);
            set => SetValue(FocusedElementProperty, value);
        }

        #endregion
        private void DockManager_DocumentClosing(object sender, AvalonDock.DocumentClosingEventArgs e)
        {
            if (MessageBox.Show($"您确定要关闭选项卡【{e.Document?.Title}】吗?", "提示", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                e.Cancel = true;

            }
            else
            {
                RemoveToMenu(e.Document?.Title);
            }
            switch (e.Document?.ContentId)
            {
                case "AutoHide2LogIceServer":
                case "AutoHide2LogPushIceServer":
                    isLondLayoutPushOne = true;
                    break;
                case "AutoHide2LogIceClient":
                case "AutoHide2LogPushIceClient":
                    isLondLayoutSendOne = true;
                    break;
                default:
                    break;
            }
        }

        private void OnLayoutRootPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var activeContent = ((LayoutRoot)sender).ActiveContent;
            if (e.PropertyName == "ActiveContent")
            {
                Debug.WriteLine(string.Format("ActiveContent-> {0}", activeContent));
            }
        }

        #region right tool bar fun 
        private void btnShowSearch_Click(object sender, RoutedEventArgs e)
        {
            ShowLayoutDocumentPane("document1");
            ShowLayoutAnchorable("AutoHide2Content");
        }
        private void btnOpenDic_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    path = Directory.GetParent(path).ToString();
                }
                var openfile = System.IO.Path.Combine(path, @"AppData\Local\Yintech");

                if (!System.IO.Directory.Exists(openfile))
                {
                    openfile = System.IO.Path.Combine(path, @"AppData\Local");
                }
                FileGet.OpenFolder(openfile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
        #region comm fun
        /// <summary>
        /// 显示LayoutAnchorable
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        LayoutAnchorable ShowLayoutAnchorable(string contentId)
        {
            var listDoc = dockManager.Layout.Descendents().OfType<LayoutAnchorable>();
            var toolWindow1 = listDoc.FirstOrDefault(a => a.ContentId == contentId);
            if (toolWindow1 == null)
            {
                return null;
            }
            if (toolWindow1.IsHidden)
                toolWindow1.Show();
            else if (toolWindow1.IsVisible)
                toolWindow1.IsActive = true;
            else
                toolWindow1.AddToLayout(dockManager, AnchorableShowStrategy.Bottom | AnchorableShowStrategy.Most);

            return toolWindow1;
        }

        LayoutContent ShowLayoutDocumentPane(string contentId)
        {
            var listDoc = dockManager.Layout.Descendents().OfType<LayoutDocument>();
            var toolWindow1 = listDoc.FirstOrDefault(a => a.ContentId == contentId);
            if (toolWindow1 == null)
            {
                return null;
            }
            if (toolWindow1.IsVisible)
                toolWindow1.IsActive = true;

            return toolWindow1;
        }
        void ShowAndAddLayoutDocumentPane(string contentId, string title, Control control)
        {
            var listDoc = dockManager.Layout.Descendents().OfType<LayoutDocument>();
            var toolWindow1 = listDoc.FirstOrDefault(a => a.ContentId == contentId);
            if (toolWindow1 == null)
            {
                var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                LayoutDocument layoutDocument = new LayoutDocument()
                {
                    CanClose = true,
                    ContentId = contentId,
                    Title = title,
                    Content = control
                };
                firstDocumentPane.Children.Add(layoutDocument);
                layoutDocument.IsActive = true;
            }
            else
            {
                if (toolWindow1.IsVisible)
                    toolWindow1.IsActive = true;
            }

        }
        void ShowAndAddLayoutDocumentPane(LayoutDocument doc)
        {
            if (doc == null)
            {
                return;
            }
            var listDoc = dockManager.Layout.Descendents().OfType<LayoutDocument>();
            var toolWindow1 = listDoc.FirstOrDefault(a => a.ContentId == doc.ContentId);
            if (toolWindow1 == null)
            {
                var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
                firstDocumentPane.Children.Add(doc);
                doc.IsActive = true;
            }
            else
            {
                if (toolWindow1.IsVisible)
                    toolWindow1.IsActive = true;
            }

        }


        #endregion

        #endregion

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(delegate
            {
                //PostMessages();
                //SendMessage(App.currExeName);
                SendServer($"Client Test.{Title}");
                SendClient($"Server Test.{Title}");

            });
        }
        public void SendServer(string message)
        {
            App._processCommunicationPipeToDo?.SendServer(message);
        }
        public void ClientStreamDispose()
        {
            App._processCommunicationPipeToDo?.ClientStreamDispose();
        }
        public void SendClient(string message)
        {
            App._processCommunicationPipeToDo?.SendClient(message);
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void openSelectFload_Click(object sender, RoutedEventArgs e)
        {
            OpenFloder();
        }
        #region ICE Server To Do


        private void btnServerRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnServerRun.IsEnabled = false;
                if (txtServerIP.Text.isNull() || txtServerIP.Text.isNull())
                {
                    return;
                }
                TestServerHelper.GetInstance().ActiveList = txtActiveAndsandby.Text.Split(',').ToList();
                TestServerHelper.GetInstance().ActiveListTrade = txtTradeActiveAndsandby.Text.Split(',').ToList();
                initServer();
            }
            catch (Exception ex)
            {
                btnServerRun.IsEnabled = true;
                MessageBox.Show(ex.Message);
            }
            finally
            {
                _startICEServer = true;
            }
        }

        private void btnServerStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                initAvalonEdit();

                ShowAndAddLayoutDocumentPane("AutoHide2LogIceServer", "ICE Server 收到消息", logShowIceServer);

                btnServerRun.IsEnabled = true;

                //停止推送前置线程
                FrontPushHelper.GetInstance().Dispose();

                //停止交易服务
                TestServerHelper.GetInstance().Dispose();

                changeNoticeServerICEAction?.Invoke("***服务已关闭...", true, 1);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                _startICEServer = false;
            }
        }
        bool isLondLayoutPushOne = true;
        private void btnServerPush_Click(object sender, RoutedEventArgs e)
        {
            btnPushTodo();
        }
        void btnPushTodo()
        {
            try
            {
                btnServerPush.IsEnabled = false;

                initAvalonEdit();

                ShowAndAddLayoutDocumentPane("AutoHide2LogIceServer", "ICE Server 收到消息", logShowIceServer);
                ShowAndAddLayoutDocumentPane("AutoHide2LogPushIceServer", "ICE Server 推送消息", logShowPushIceServer);

                if (isLondLayoutPushOne)
                {
                    LoadLayoutByText("ICEServer布局");
                    isLondLayoutPushOne = false;
                }

                if (!_startICEServer)
                {
                    changeNoticeServerICE("**当前服务未启动，无法推送", 2);
                    return;
                }

                //加载默认发包  
                changeNoticeServerICE("*****************ICE 服务 推送消息****************");

                if (isInitNumServer.IsChecked.GetValueOrDefault())
                {
                    FrontPushHelper.GetInstance().InitDicAddNum();
                }

                PushDataByMType(logShowPushIceServer.Text());


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btnServerPush.IsEnabled = true;
            }
        }
        private void isShowMsg_Click(object sender, RoutedEventArgs e)
        {
            TestServerHelper.GetInstance().isShowAllMsg = isShowMsg.IsChecked.GetValueOrDefault();
        }

        private void isPushAsync_Click(object sender, RoutedEventArgs e)
        {
            TestServerHelper.GetInstance().isPushAsync = isPushAsync.IsChecked.GetValueOrDefault();
        }
        #region Server to do method
        public Action<string, bool, int> changeNoticeServerICEAction { get; set; }
        public void changeNoticeServerICE(string msg, int type = 1, bool isshow = true)
        {
            changeNoticeServerICEAction?.Invoke(msg, isshow, type);
        }
        private bool _startICEServer = false;
        AvalonEditUserControl logShowIceServer;
        AvalonEditUserControl logShowPushIceServer;
        AvalonEditUserControl logShowIceClient;
        AvalonEditUserControl logShowSendIceClient;
        void initIceServeFirst()
        {
            txtClientType.ToolTip = txtPushType.ToolTip = @"    TRADE_CLIENT = 1, //交易客户端
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
                                        TRANSFER_SERVER = 25,  //中转服务
                                        TRANSFER_SERVER = 26,  //持久服务
                                        TRANSFER_SERVER = 28,  //仲裁服务
                                        TRANSFER_SERVER = 29,  //内存服务
                        ";

        }
        void initAvalonEdit()
        {
            if (logShowIceServer == null)
            {
                logShowIceServer = new AvalonEditUserControl();

                //显示日志
                changeNoticeServerICEAction = logShowIceServer.changeNoticeQueue;
                FrontPushHelper.GetInstance().changeNoticeServerICEAction = logShowIceServer.changeNoticeQueue;
            }

            if (logShowPushIceServer == null)
            {
                logShowPushIceServer = new AvalonEditUserControl();
                logShowPushIceServer.InitEditToolBar("IceServer");
                logShowPushIceServer.AddSendTodo("推送", btnPushTodo);
                logShowPushIceServer.changeNoticeOtherAction = changeNoticeServerICEAction;

                //清空
                logShowPushIceServer.comboBoxSelectedIndex(0);
                logShowPushIceServer.SetSyntaxHighlighting("Json");
            }
        }
        public void initServer()
        {
            initAvalonEdit();

            ShowAndAddLayoutDocumentPane("AutoHide2LogIceServer", "ICE Server 收到消息", logShowIceServer);

            changeNoticeServerICE("*****************ICE 服务 测试****************");
            //注册回调
            App._ActionAll.OnFrontServerStateEvent = onServerStateEvent;
            App._ActionAll.OnQuotConnectionStatusEvent = onQuotStateEvent;
            App._ActionAll.OnSystemMessageICEServerEvent = (m, i) =>
            {
                changeNoticeServerICEAction?.Invoke(m, true, i);
            };
            //启动推送线程
            FrontPushHelper.GetInstance().StartThread();

            //连接路由
            //connectRoute(txtRoute_IP.Text, txtRoute_Port.Text, txtRouteName.Text);

            //启动
            startTradeService(txtServerIP.Text.Trim(), txtServerPort.Text.Trim());
        }
        private void startTradeService(string ip = "0.0.0.0", string port = "10000")
        {
            //启动交易服务 
            //初始化server
            changeNoticeServerICEAction?.Invoke($"{ip}:{port}初始化服务...", true, 1);
            TestServerHelper.GetInstance().actionMsg = (msg) =>
            {
                changeNoticeServerICEAction?.Invoke(msg, true, 1);
            };
            TestServerHelper.GetInstance().AddClientType = (clienttype, isadd) =>
            {
                AddClientType(clienttype, isadd);
            };
            TestServerHelper.GetInstance().Init(ip, port);
            _startICEServer = true;
        }
        #region State event 
        private void onServerStateEvent(bool conn)
        {

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
        #endregion
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
        #endregion
        #region push by server
        public void PushDataByMType(string jsonStr)
        {
            try
            {
                var toPushPackage = jsonStr.JsonTo<ComDto>();

                if (toPushPackage == null)
                {
                    changeNoticeServerICE($"## 输入错数据消息包。。。{jsonStr}", 2);
                    return;
                }

                switch (toPushPackage.MType)
                {
                    case emMType.Base:
                        PushDataOld(jsonStr);
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
                changeNoticeServerICE(ex.Message, 2);
            }
        }
        #region changeconnectid
        void ChangeConnectID(CacheTradeDBDto2 toPushPackage)
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
                changeNoticeServerICE(ex.Message, 2);
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
        #endregion
        public void PushDataOld(string jsonStr)
        {
            try
            {
                var toPushPackage = jsonStr.JsonTo<ResponsePackageDTO>();
                ChangeConnectID(toPushPackage);
                changeNoticeServerICE($"isPushAsync:{isPushAsync.IsChecked.GetValueOrDefault()},测试(旧)，开始推送。。。");
                var getListType = txtPushType.Text.SplitTrim(',');
                foreach (var item in getListType)
                {
                    FrontPushHelper.GetInstance().AddData(toPushPackage, item.TryToInt());
                }
            }
            catch (Exception ex)
            {
                changeNoticeServerICE(ex.Message, 2);
            }
        }
        public void PushDataList(string jsonStr)
        {
            try
            {
                var toPushPackage = jsonStr.JsonTo<ResponsePackageOrderDto>();
                ChangeConnectID(toPushPackage);
                changeNoticeServerICE($"isPushAsync:true,测试(List)，开始推送。。。");
                var getListType = txtPushType.Text.SplitTrim(',');
                var isStream = new Tuple<bool, int, int, bool>(ckIsICEStreamToPush.IsChecked.GetValueOrDefault(), txtServerNumber.Text.TryToInt(1), txtDataSize.Text.TryToInt(1), false);
                foreach (var item in getListType)
                {
                    FrontPushHelper.GetInstance().AddData(toPushPackage, item.TryToInt(), isStream);
                }
            }
            catch (Exception ex)
            {
                changeNoticeServerICE(ex.Message, 2);
            }
        }
        public void PushDataCacheDB(string jsonStr)
        {
            try
            {
                var toPushPackage = jsonStr.JsonTo<PackageCacheDBDto>();
                ChangeConnectID(toPushPackage);
                changeNoticeServerICE($"isPushAsync:true,测试(CacheDB)，开始推送。。。");
                var getListType = txtPushType.Text.SplitTrim(',');
                foreach (var item in getListType)
                {
                    FrontPushHelper.GetInstance().AddData(toPushPackage, item.TryToInt(), null);
                }
            }
            catch (Exception ex)
            {
                changeNoticeServerICE(ex.Message, 2);
            }
        }
        public void PushCacheTradeDBDto(string jsonStr)
        {
            try
            {
                var toPushPackage = jsonStr.JsonTo<CacheTradeDBDto2>();
                ChangeConnectID(toPushPackage);
                changeNoticeServerICE($"isPushAsync:true,测试(CacheTradeDB)，开始推送。。。");
                var getListType = txtPushType.Text.SplitTrim(',');
                foreach (var item in getListType)
                {
                    FrontPushHelper.GetInstance().AddData(toPushPackage, item.TryToInt(), null);
                }
            }
            catch (Exception ex)
            {
                changeNoticeServerICE(ex.Message, 2);
            }
        }
        #endregion

        #endregion

        #region layout set

        private void Button_ConfigLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                initAvalonEdit();
                ShowAndAddLayoutDocumentPane("AutoHide2LogIceServer", "ICE Server 收到消息", logShowIceServer);
                ShowAndAddLayoutDocumentPane("AutoHide2LogPushIceServer", "ICE Server 推送消息", logShowPushIceServer);

                initAvalonEditClient();
                ShowAndAddLayoutDocumentPane("AutoHide2LogIceClient", "ICE Client 收到消息", logShowIceClient);
                ShowAndAddLayoutDocumentPane("AutoHide2LogPushIceClient", "ICE Client 发送消息", logShowSendIceClient);

                var currbtn = (Fluent.Button)sender;
                LoadLayoutByText(currbtn.Header.ToNotNullString());

                isLondLayoutPushOne = false;
                isLondLayoutSendOne = false;
            }
            catch (Exception ex)
            {
                changeNoticeQueue(ex.Message, true, 2);
            }
        }
        private void btnSaveLayout_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "布局文件 (*.config)|*.config|所有文件 (*.*)|*.*";
                saveFileDialog.Title = "保存当前布局";
                saveFileDialog.InitialDirectory = projectFileConfigPath;
                saveFileDialog.FileName = $"Man_" + DateTime.Now.Date.ToString("yyyyMMdd");
                if ((bool)(saveFileDialog.ShowDialog()))
                {
                    //获得文件路径
                    var serializer = new XmlLayoutSerializer(dockManager);
                    using (var stream = new StreamWriter(saveFileDialog.FileName))
                        serializer.Serialize(stream);

                    layoutConfigDic[saveFileDialog.FileName.GetFileNameWithoutExtension()] = saveFileDialog.FileName;
                    UpdateCombox();
                    changeNoticeQueue($"保存当前布局成功，Path:{saveFileDialog.FileName}。", true, 7);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void layoutComb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var cb = sender as ComboBox;
                if (cb.SelectedItem != null)
                {
                    var dd = (KeyValuePair<string, string>)cb.SelectedItem;
                    if (dd.Value.IsNullOrWhiteSpace())
                    {
                        return;
                    }
                    var serializer = new XmlLayoutSerializer(dockManager);
                    //serializer.LayoutSerializationCallback += (s, args) =>
                    //    {
                    //        var prevContent = currentContentsList.FirstOrDefault(c => c.ContentId == args.Model.ContentId);
                    //        if (prevContent != null)
                    //            args.Content = prevContent.Content;
                    //    };
                    using (var stream = new StreamReader(dd.Value))
                        serializer.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void LoadLayoutByText(string keyName)
        {

            if (!layoutConfigDic.ContainsKey(keyName))
            {
                return;
            }
            var serializer = new XmlLayoutSerializer(dockManager);
            var filePath = layoutConfigDic[keyName];
            //serializer.LayoutSerializationCallback += (s, args) =>
            //    {
            //        var prevContent = currentContentsList.FirstOrDefault(c => c.ContentId == args.Model.ContentId);
            //        if (prevContent != null)
            //            args.Content = prevContent.Content;
            //    };
            using (var stream = new StreamReader(filePath))
                serializer.Deserialize(stream);
        }
        Dictionary<string, string> layoutConfigDic = new Dictionary<string, string>();
        Dictionary<string, string> layoutConfigExecutingAssemblyDic = new Dictionary<string, string>();

        public Dictionary<string, string> getAssemblyConfig()
        {
            var result = new Dictionary<string, string>();
            try
            {
                Assembly executingAssembly = Assembly.GetExecutingAssembly();
                var list = executingAssembly.GetManifestResourceNames();

                var assemblyName = new AssemblyName(executingAssembly.FullName);

                string replaceNameStr = $"{assemblyName.Name}.Config.";
                foreach (var item in list)
                {
                    if (System.IO.Path.GetExtension(item) != ".config")
                    {
                        continue;
                    }
                    using (var stream = executingAssembly.GetManifestResourceStream(item))
                    {
                        if (stream == null) continue;
                        var assemblyRawBytes = new byte[stream.Length];
                        stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                        var getContent = stream.StreamReadString(Encoding.UTF8);
                        result[item.Replace(replaceNameStr, "").GetFileNameWithoutExtension()] = getContent;
                    }

                }
            }
            catch (Exception ex)
            {
                return result;

            }
            return result;

        }

        public void loadlayoutConfigDic()
        {
            if (projectFileConfigPath.IsNullOrWhiteSpace())
            {
                projectFileConfigPath = System.IO.Path.Combine(currPath, "方案", "Config");
                projectFileConfigPath.FileDirectoryCreateDirectory();

            }
            //var currSample = getAssemblyJson();

            var toNotnames = new List<string>();
            string getFileName = ".config";
            FileGet.allGetFiles.Clear();
            FileGet.getFile(projectFileConfigPath, getFileName, toNotnames);
            var getAllExe = FileGet.allGetFiles;

            foreach (var item in getAllExe)
            {
                layoutConfigDic.Add(item.Name.GetFileNameWithoutExtension(), item.FullName);
            }

        }
        void initComboBox(int allSize = 50)
        {
            layoutConfigDic["  "] = "";

            loadlayoutConfigDic();

            if (layoutConfigDic.Count <= 1 && layoutConfigDic.ContainsKey("  "))
            {
                if (layoutConfigExecutingAssemblyDic.Count <= 0)
                {
                    layoutConfigExecutingAssemblyDic = getAssemblyConfig();
                }

                foreach (var item in layoutConfigExecutingAssemblyDic)
                {
                    var tmpPath = projectFileConfigPath + $"//{item.Key}.config";
                    tmpPath.FilePathSaveContentStreamWriter(item.Value, Encoding.UTF8);
                    layoutConfigDic[item.Key] = tmpPath;
                }
            }

            layoutComb.ItemsSource = layoutConfigDic;
            layoutComb.DisplayMemberPath = "Key";
            layoutComb.SelectedValuePath = "Value";
        }
        void UpdateCombox(int allSize = 50)
        {
            if (layoutConfigDic.Keys.Count > 0)
            {
                layoutComb.ItemsSource = layoutConfigDic;
                layoutComb.Items.Refresh();
            }
        }
        #endregion

        #region ICE Client To Do
        public Action<string, bool, int> changeNoticeClientICEAction { get; set; }
        public bool toStopClient { get; set; }
        public void changeNoticeClientICE(string msg, int type = 1, bool isshow = true)
        {
            changeNoticeClientICEAction?.Invoke(msg, isshow, type);
        }
        void InitClientICE()
        {

        }
        void initAvalonEditClient()
        {
            if (logShowIceClient == null)
            {
                logShowIceClient = new AvalonEditUserControl();

                //显示日志
                changeNoticeClientICEAction = logShowIceClient.changeNoticeQueue;
            }

            if (logShowSendIceClient == null)
            {
                logShowSendIceClient = new AvalonEditUserControl();
                logShowSendIceClient.InitEditToolBar("IceClient");
                logShowSendIceClient.AddSendTodo("发送", btnSendTodoClient);
                logShowSendIceClient.changeNoticeOtherAction = changeNoticeClientICEAction;

                MsgIdCheckClientQueue.GetInstance().logShowActionAll = changeNoticeClientICEAction;
                //清空
                logShowSendIceClient.comboBoxSelectedIndex(0);
                logShowSendIceClient.SetSyntaxHighlighting("Json");
            }
        }

        private void btnClientRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtClientIP.Text.IsNullOrWhiteSpace() || txtClientPort.Text.IsNullOrWhiteSpace())
                {
                    return;
                }

                initAvalonEditClient();

                ShowAndAddLayoutDocumentPane("AutoHide2LogIceClient", "ICE Client 收到消息", logShowIceClient);


                var currbtn = ((Button)sender);
                currbtn.IsEnabled = false;

                var curr = this;
                //this.ip = $"获取:{txtClientIP.Text},{txtClientPort.Text}";
                initClient.GetInstanceLazy.ConnectAction = (a) =>
                {
                    curr.Dispatcher.Invoke(() =>
                    {
                        toStopClient = currbtn.IsEnabled = a == SlmCommunicator.Client.ConnectStateClient.CONNECT_OK ? false : true;
                        var success = a == SlmCommunicator.Client.ConnectStateClient.CONNECT_OK ? "成功" : "失败";
                        changeNoticeClientICE($"***连接IP:{txtClientIP.Text},{txtClientPort.Text}{success}");
                    });
                };
                ClientType clientType = ClientType.CONTER_CLIENT;
                ClientType.TryParse(txtClientType.Text, out clientType);
                char isOnewey = ckOnewey.IsChecked.GetValueOrDefault() ? 't' : 'o';
                bool isZip = ckZip.IsChecked.GetValueOrDefault();
                initClient.GetInstanceLazy.actionMsg = changeNoticeClientICEFroAction;
                initClient.GetInstanceLazy.ConnectServer(txtClientId.Text.Trim(), txtClientIP.Text.Trim(), txtClientPort.Text.Trim(), clientType, isOnewey, isZip);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void changeNoticeClientICEFroAction(string msg)
        {
            changeNoticeClientICE($"*** {msg}");
        }
        private void btnClientStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                initClient.GetInstanceLazy.DisconnectServer();
                toStopClient = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {

                btnClientRun.IsEnabled = true;
            }

        }
        public bool isLondLayoutSendOne { get; set; } = true;
        private void btnSendClient_Click(object sender, RoutedEventArgs e)
        {
            btnSendTodoClient();
        }
        void btnSendTodoClient()
        {
            //lock (currMain)
            {
                var currbtn = btnSendClient;
                try
                {
                    currbtn.IsEnabled = false;

                    initAvalonEditClient();

                    ShowAndAddLayoutDocumentPane("AutoHide2LogIceClient", "ICE Client 收到消息", logShowIceClient);
                    ShowAndAddLayoutDocumentPane("AutoHide2LogPushIceClient", "ICE Client 发送消息", logShowSendIceClient);

                    if (isLondLayoutSendOne)
                    {
                        LoadLayoutByText("ICEClient布局");
                        isLondLayoutSendOne = false;
                    }


                    sendMessageClient();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    currbtn.IsEnabled = true;
                }

            }

        }

        void sendMessageClient()
        {
            lock (_sendLock)
            {
                currMain.Dispatcher.Invoke(() =>
                {
                    //加载默认发包  
                    changeNoticeClientICE("*****************ICE 客户端 发送消息****************");

                    if (toStopClient)
                    {
                        changeNoticeClientICE("**当前服务未连接，无法发送", 2);

                        if (istoSend)
                        {
                            istoSend = false;

                            ckAutoSend.IsChecked = false;

                        }

                        return;
                    }


                    var listTo = new List<ComDto>();

                    var jsonStr = logShowSendIceClient.Text();
                    var toSendPackage = jsonStr.JsonTo<ComDto>();
                    int num = 1;
                    decimal takeTime = 1;
                    bool AddSendTime = ckAddSendTime.IsChecked.GetValueOrDefault();
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
                            toSendPackage = PushDataCacheDB2(jsonStr);
                            toSendPackage.MType = emMType.CacheDB;
                            break;
                        case emMType.TradeDB:
                            toSendPackage = PushCacheTradeDBDto2(jsonStr);
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
                        if (toStopClient)
                        {
                            changeNoticeClientICE($"**##**停止成功，目前第：{i}/{num}个.");
                            break;
                        }
                        changeNoticeClientICE($"**##**开始发送第：{i}/{num}个.");
                        if (ckAddOne.IsChecked.GetValueOrDefault())
                        {
                            toSendPackage.ServiceNo++;
                        }
                        toSendPackage.IsAddSendTime = AddSendTime;
                        toSendPackage.MsgId = Guid.NewGuid().ToString();
                        if (istoSend && toSendPackage.MType != emMType.Base)
                        {
                            MsgIdCheckClientQueue.GetInstance().AddDataDic(toSendPackage.MsgId, $"{toSendPackage.ServiceNo}");
                        }
                        SendRequest(toSendPackage);
                        sendcount++;
                    }
                    stopwatch.Stop();
                    changeNoticeClientICE($"**##**已全部发送完成：{sendcount}/{num}个.发送总时间：{stopwatch.Elapsed}");
                });
            }
        }
        private void isClientShowMsg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                initClient.GetInstanceLazy.isShowAllMsg = isShowMsg.IsChecked.GetValueOrDefault();
                if (initClient.GetInstanceLazy.isShowAllMsg)
                {
                    initFilterData();
                }
                //isSyntaxHighlighting = !initClient.GetInstanceLazy.isShowAllMsg;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
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
            changeNoticeClientICE("\n**过滤服务号**\n");
            changeNoticeClientICE(initClient.GetInstanceLazy.filterServernos.toJsonStr());
            changeNoticeClientICE("\n");
        }
        #region ToDo Client
        public async Task SendRequest(ComDto toSendPackage)
        {
            SendRequestToDo(toSendPackage, true);
            await Task.Delay(5);
        }
        public void SendRequestToDo(ComDto toSendPackage, bool isshow = true, string keyComm = "")
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            bool isShowMsg = isClientShowMsg.IsChecked.GetValueOrDefault();
            initClient.GetInstanceLazy.isShowAllMsg = isShowMsg;

            bool isAsync = ckIsAsync.IsChecked.GetValueOrDefault();
            bool isAddKey = ckAddKey.IsChecked.GetValueOrDefault();
            bool isInitAdd = ckInitAddNum.IsChecked.GetValueOrDefault();
            bool IsListCacheTrade = ckIsListCacheTrade.IsChecked.GetValueOrDefault();
            bool isUseICEStream = ckIsICEStreamToSend.IsChecked.GetValueOrDefault();
            bool isSaveICEStream = ckIsSaveStream.IsChecked.GetValueOrDefault();
            if (isInitAdd)
            {
                initClient.GetInstanceLazy.InitDicAddNum();
            }
            int num = 1;
            int numPackage = 1;
            int.TryParse(txtNumber.Text, out num);
            int.TryParse(txtNumberSend.Text, out numPackage);
            var getPackage = initClient.GetInstanceLazy.SendRequest(toSendPackage, keyComm, isAsync, new Tuple<bool, int, int, bool, bool, bool>(isAddKey, num, numPackage, IsListCacheTrade, isUseICEStream, isSaveICEStream));
            stopwatch.Stop();
            changeNoticeClientICE($"**获取:{txtClientIP.Text},{txtClientPort.Text}服务号:{toSendPackage.ServiceNo},耗时：{stopwatch.Elapsed},内容如下：\n{getPackage.toJsonStr()}", 1, isshow);

        }
        public PackageRequestDTO PushDataOrderList(string jsonStr, string title = "List")
        {
            try
            {
                var toPushPackage = jsonStr.JsonTo<PackageRequestDTO>();

                changeNoticeClientICE($"isSendAsync:true,测试({title})，开始发送。。。");

                return toPushPackage;
            }
            catch (Exception ex)
            {
                changeNoticeClientICE(ex.Message, 2);
            }
            return null;
        }
        public PackageCacheDBDto PushDataCacheDB2(string jsonStr)
        {
            try
            {
                var toPushPackage = jsonStr.JsonTo<PackageCacheDBDto>();

                changeNoticeClientICE($"isSendAsync:true,测试(CacheDB)，开始发送。。。");

                return toPushPackage;
            }
            catch (Exception ex)
            {
                changeNoticeClientICE(ex.Message, 2);
            }
            return null;
        }
        public CacheTradeDBDto PushCacheTradeDBDto2(string jsonStr)
        {
            try
            {
                var toPushPackage = jsonStr.JsonTo<CacheTradeDBDto>();

                changeNoticeClientICE($"isSendAsync:true,测试(CacheTradeDB)，开始发送。。。");

                return toPushPackage;
            }
            catch (Exception ex)
            {
                changeNoticeClientICE(ex.Message, 2);
            }
            return null;
        }




        #endregion

        #endregion

        private void ckAddSendTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TestServerHelper.GetInstance().isAddTime = ckAddSendTime.IsChecked.GetValueOrDefault();
                changeNoticeServerICE($"ICE服务端回报Head是否带上时间：{TestServerHelper.GetInstance().isAddTime}");
                changeNoticeClientICE($"ICE客户端回报Head是否带上时间：{TestServerHelper.GetInstance().isAddTime}");
            }
            catch (Exception ex)
            {
                changeNoticeServerICE(ex.Message);
            }
        }
        private void ckAutoSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                initClient.GetInstanceLazy.isAddTime = ckAutoSend.IsChecked.GetValueOrDefault();
                timetoSleep = txtTimeAutoSend.Text.TryToInt(3);
                istoSend = ckAutoSend.IsChecked.GetValueOrDefault();
                changeNoticeClientICE($"ICE客户端订时发送功能开关：{istoSend}");
            }
            catch (Exception ex)
            {
                changeNoticeClientICE(ex.Message);
            }
        }
        #region auto time tosend client
        bool istoSend = false;
        bool isStopSend = false;
        int timetoSleep = 3;
        private void workProcessToSendToDo()
        {

            SpinWait spinWait = new SpinWait();
            while (!isStopSend)
            {
                try
                {
                    if (!istoSend)
                    {
                        spinWait.SpinOnce();
                        continue;
                    }
                    sendMessageClient();

                    timetoSleep = timetoSleep < 1 ? 3 : timetoSleep;
                    Thread.Sleep(timetoSleep*1000);

                }
                catch (Exception ex)
                {
                    LogHelper.WriteErrorLog($"workProcessToSendToDo:{ex}");
                }
            }
        }
        public void StartThread()
        {
            Thread checkThread = new Thread(workProcessToSendToDo);
            checkThread.IsBackground = true;
            checkThread.Name = "workProcessToSendToDo";
            checkThread.Start();
        }
        #endregion


    }
}
