using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;
using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml;
using Tools.OpenExe.Exts;
using CheckBox = System.Windows.Controls.CheckBox;
using Cursors = System.Windows.Input.Cursors;
using MessageBox = System.Windows.MessageBox;

namespace Tools.OpenExe
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<StackPanel> _allExeFile;
        private StackPanel[] _allExeFile_bak;
        private ObservableCollection<CheckBoxFlag> _allProjectFile;
        private CollectionViewSource collectionViewSource;
        private static Process[] _getCurrAllProcess;
        private string currPath;
        private List<string> currPathes;
        public string OpenExeConfigPath;

        public string projectFilePath { get; set; }

        public List<string> otherFilterDef { get; private set; }
        public List<string> logFilterDef { get; private set; }

        public DispatcherTimer dispatcherTimer { get; set; }
        public string version { get; set; }
        public Dictionary<string, string> cacheMd5File = new Dictionary<string, string>();
        public bool isInitOver = false;

        public MainWindow()
        {
            try
            {

                initAvalonEditBefore();

                InitializeComponent();

                var msg = $"\n####小提示：\n**不包含：可输入多个分号或逗号隔开，如：123;abc;ddd\n**##特殊显示内容查询：a&b|c&d|e|f&g&h&i";
                initAvalonEditEnd(msg);

                this.version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

                initThread();

                initFirst();

                rtRec.Text = App.dlllist.toJsonStr();
            }
            finally
            {
                isInitOver = true;
            }
        }
        void initFirst()
        {
            try
            {
                btnMove.Content = @"<-|||->";

                OpenExeConfigPath = AppDomain.CurrentDomain.BaseDirectory + "\\OpenExeConfig.json";

                otherFilterDef = new List<string>();
                _allExeFile = new ObservableCollection<StackPanel>();
                _allProjectFile = new ObservableCollection<CheckBoxFlag>();
                collectionViewSource = new CollectionViewSource() { Source = _allExeFile };
                allLst.Items.Clear();
                allLst.ItemsSource = null;
                allLst.ItemsSource = collectionViewSource.View;
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                this.Topmost = true;
                IsTop(this.Topmost);
                this.Loaded += MainWindow_Loaded;
                this.Closing += MainWindow_Closing;

                currPath = currPath.IsNullOrWhiteSpace() ? AppDomain.CurrentDomain.BaseDirectory : currPath;
                this.Title = this.version + "->当前目录:" + currPath;

                selectDateTime.Value = DateTime.Now;

                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(OnTimedEvent);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
                dispatcherTimer.Start();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void OnTimedEvent(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                //selectDateTime.Value = DateTime.Now;
            });
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            _runShow = false;
            autoSave(currPathes);
            Application.Current.Shutdown();
            Environment.Exit(0);
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
                dellogpath = txtlogPath.Text
            ,
                txtfixurl = txtfixurl.Text
            };
            OpenExeConfigPath.FilePathSaveContent(toJson.toJsonStr(), Encoding.UTF8);
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                var getContent = OpenExeConfigPath.FilePathGetContent(Encoding.UTF8);
                if (getContent.IsNotNullOrWhiteSpace())
                {
                    var tmppenExeConfig = getContent.JsonTo<OpenExeConfig>();
                    currPath = tmppenExeConfig.configPath.IsNotNullOrWhiteSpace() ? tmppenExeConfig.configPath : currPath;
                    filterName.Text = tmppenExeConfig.toolconfigFilter.IsNotNullOrWhiteSpace() ? tmppenExeConfig.toolconfigFilter : filterName.Text;
                    GetNameExt.Text = tmppenExeConfig.toolconfigGetName.IsNotNullOrWhiteSpace() ? tmppenExeConfig.toolconfigGetName : GetNameExt.Text;
                    txtlogPath.Text = tmppenExeConfig.dellogpath.IsNotNullOrWhiteSpace() ? tmppenExeConfig.dellogpath : txtlogPath.Text;
                    txtfixurl.Text = tmppenExeConfig.txtfixurl.IsNotNullOrWhiteSpace() ? tmppenExeConfig.txtfixurl : txtfixurl.Text;
                    currPathes = tmppenExeConfig.configPaths;
                }

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

                var tmpTaskProject = initProject();

               

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
        public bool isLoad = false;
        async Task LoadData(string fileExt = "", bool isProject = false)
        {
            var currbtn = btnLoadData;
            try
            {
                if (isLoad)
                {
                    return;
                }
                isLoad = true;
                currbtn.IsEnabled = false;
                this.Cursor = Cursors.Wait;

                //lock (this)
                {
                    await LoadExeFile(fileExt);
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
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await LoadData();
        }
        private async Task LoadExeFile(string fileExt = "")
        {
            _allExeFile.Clear();
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
            foreach (var item in getAllExe)
            {
                try
                {
                    var checkExist = _allExeFile.Where(a => ((CheckBox)a.Children[1]).Content.ToString() == item.FullName).Count() > 0;
                    if (!checkExist)
                    {
                        var stackPanel = GetStack(item);
                        _allExeFile.Add(stackPanel);
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
        public StackPanel GetStack(FileInfo item)
        {
            var fileversion = GetFileVerion(item.FullName);
            var versionmsgMd5 = GetMD5HashFromFile(item.FullName);

            var versionmsg = $"{fileversion?.FileVersion ?? "0"}";
            var versionmsg2 = $"{versionmsgMd5.Substring(versionmsgMd5.Length - 5)}";

            var stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            stackPanel.Children.Add(new CheckBox() { ToolTip = "删除log" });
            var lstbotx = new CheckBox() { Content = item.FullName, IsChecked = true, ToolTip = $"{item.Name}", Tag = fileversion };
            stackPanel.Children.Add(lstbotx);
            TextBlock labelVersion = new TextBlock() { Text = versionmsg, Tag = versionmsgMd5, Margin = new Thickness(5, 0, 0, 0), Foreground = Brushes.Red };

            TextBlock labelMd5 = new TextBlock() { Text = versionmsg2, Tag = versionmsgMd5, Margin = new Thickness(5, 0, 0, 0), Foreground = Brushes.Black };
            stackPanel.Children.Add(labelVersion);
            stackPanel.Children.Add(labelMd5);

            stackPanel.Tag = true;
            lstbotx.Click += Lstbotx_Checked;

            return stackPanel;
        }

        private void Lstbotx_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                var currbtn = sender as CheckBox;
                var currSP = currbtn.Parent as StackPanel;
                currSP.Tag = currbtn.IsChecked;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public FileVersionInfo GetFileVerion(string filename)
        {
            System.Diagnostics.FileVersionInfo myFileVersionInfo = null;
            try
            {

                myFileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return myFileVersionInfo;
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
        /// <summary>
        /// 打开进程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Btn2_Click(object sender, RoutedEventArgs e)
        {
            btn2.IsEnabled = false;
            this.Cursor = Cursors.Wait;

            try
            {
                var AllToStart = _allExeFile.Select(a => (CheckBox)a.Children[1]).Where(a => a.IsChecked.GetValueOrDefault() == true);
                int toI = 1;
                foreach (var item in AllToStart)
                {
                    string path = item.Content.ToString();

                    changeNotice($"第{toI}/{AllToStart.Count()}个：{path}:开始启动。");

                    await RunProcess(path);

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
        public List<string> listLogpaht { get; set; } = new List<string>();
        public void initlistLogpaht()
        {
            listLogpaht = txtlogPath.Text.SplitTrim(',');
            if (!listLogpaht.HasItem())
            {
                listLogpaht = new List<string>() { "log" };
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
        /// <summary>
        /// 关闭进程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Btn3_Click(object sender, RoutedEventArgs e)
        {
            btn3.IsEnabled = false;
            this.Cursor = Cursors.Wait;
            try
            {
                var isCloseByName = ckCloseByName.IsChecked.GetValueOrDefault();
                var AllToStop = _allExeFile.Where(a => (a.Children[1] as CheckBox).IsChecked.GetValueOrDefault() == true);
                var exitProcessName = AllToStop.Select(a => new { filename = (a.Children[1] as CheckBox).Content.ToString(), a = System.IO.Path.GetFileNameWithoutExtension((a.Children[1] as CheckBox).Content.ToString()), b = (a.Children[0] as CheckBox).IsChecked.GetValueOrDefault() ? System.IO.Path.GetDirectoryName((a.Children[1] as CheckBox).Content.ToString()) : "" });

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
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in _allExeFile)
            {
                ((CheckBox)item.Children[0]).IsChecked = cbAll.IsChecked;
                ((CheckBox)item.Children[1]).IsChecked = cbAll.IsChecked;
                item.Tag = cbAll.IsChecked;
            }
            projectList.SelectedIndex = -1;
        }
        private void CheckBox_Checked2(object sender, RoutedEventArgs e)
        {
            var currcheck = sender as CheckBox;
            foreach (var item in _allExeFile)
            {
                if (((CheckBox)item.Children[1]).IsChecked.GetValueOrDefault())
                {
                    ((CheckBox)item.Children[0]).IsChecked = currcheck.IsChecked;
                }
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var getcurrItem = ((StackPanel)allLst.SelectedItem).Children[1] as CheckBox;
                FileGet.OpenFolderAndSelectFile(getcurrItem.Content.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private async void ConfigUpdate_Click(object sender, RoutedEventArgs e)
        {
            configUpdate.IsEnabled = false;
            this.Cursor = Cursors.Wait;

            try
            {
                var AllToStart = _allExeFile.Select(a => (CheckBox)a.Children[1]).Where(a => a.IsChecked.GetValueOrDefault() == true);
                int toI = 1;
                foreach (var item in AllToStart)
                {
                    string path = item.Content.ToString();
                    if (!path.EndsWith("config", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    changeNotice($"第{toI}/{AllToStart.Count()}个：{path}:开始更新。");
                    await Task.Delay(1);
                    var getContent = path.FilePathGetContent(Encoding.UTF8);
                    var getEndContent = getContent.Replace("http://gitlab.yintech.net/tc/FTrade", "http://gitlab.yintech.net/fdzq/FTrade");
                    path.FilePathSaveContent(getEndContent, Encoding.UTF8);
                    toI++;
                }
                changeNotice($"{AllToStart.Count()}个全部更新完成。");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                configUpdate.IsEnabled = true;
                this.Cursor = Cursors.Arrow;

            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

            try
            {
                var getcurrItem = ((StackPanel)allLst.SelectedItem).Children[1] as CheckBox;
                var fileName = System.IO.Path.GetFileNameWithoutExtension(getcurrItem.ToolTip.NullToStr());
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
                //_allExeFile.Remove(getcurrItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }

        async Task StartProcess()
        {
            var item = ((StackPanel)allLst.SelectedItem).Children[1] as CheckBox;

            string path = item.Content.ToString();

            changeNotice($"{path}:开始启动。");

            await RunProcess(path);

            changeNotice($"{path}启动完成。");

            //刷新当前进程信
            _getCurrAllProcess = Process.GetProcesses();
        }
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        async Task KillExit(bool isCloseByName = false)
        {
            //刷新当前进程信
            _getCurrAllProcess = Process.GetProcesses();

            var item = ((StackPanel)allLst.SelectedItem).Children[1] as CheckBox;

            string path = item.Content.ToString();// System.IO.Path.GetFileNameWithoutExtension(item.Content.ToString());

            changeNotice($"{path}:开始关闭。");

            await KillProcess(path, null, isCloseByName);

            changeNotice($"{path}关闭完成。");
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                var isCloseByName = ckCloseByName.IsChecked.GetValueOrDefault();
                await KillExit(isCloseByName);

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

                var toSendPackage = _allExeFile.Where(a => (a.Children[1] as CheckBox).IsChecked.GetValueOrDefault() == true);
                var toJson = toSendPackage.Select(a => new CheckBoxFlag() { Content = (a.Children[1] as CheckBox).Content.ToString(), IsChecked = true, IsDelLog = (a.Children[0] as CheckBox).IsChecked.GetValueOrDefault() }).ToList().toJsonStr();

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
        public void loadProjectByFilePath(string tmpPath)
        {
            var getContent = tmpPath.FilePathGetContent(Encoding.UTF8);
            var getlistCheckList = getContent.JsonTo<List<CheckBoxFlag>>();
            foreach (var item in _allExeFile)
            {
                var checkboxPath = item.Children[1] as CheckBox;
                var checkboxDelflag = item.Children[0] as CheckBox;
                var filepath = checkboxPath.Content.ToString();
                var isExist = getlistCheckList.Where(a => a.Content == filepath).FirstOrDefault();
                item.Tag = false;
                if (isExist != null)
                {
                    checkboxPath.IsChecked = true;
                    checkboxDelflag.IsChecked = isExist.IsDelLog;
                    item.Tag = true;

                }
                else
                {
                    checkboxPath.IsChecked = false;
                    checkboxDelflag.IsChecked = false;
                }
            }
        }
        void initLoadProjcetByFilePath()
        {
            try
            {
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
        private void projectList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            initLoadProjcetByFilePath();
            sortBySelectCheck(false);
        }
        void selectPath()
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            if (!currPath.IsNullOrWhiteSpace())
            {

                dialog.SelectedPath = currPath;
            }

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                currPath = dialog.SelectedPath;
            }
            this.Title = $"{this.version}->当前目录:{currPath}";
        }
        private void btnSelectPath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var win = new SettingWindow();
                win.settingPage.SetCurrPathAction = SetCurrPathAction;
                win.settingPage.AutoSaveAction = autoSave;
                win.settingPage.initDic(currPathes);
                win.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        private void SearchFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (sender == null || _allExeFile_bak == null)
                {
                    return;
                }
                string versionmsg = "";
                string versionmsgmd5 = "";

                var send = (TextBox)sender;
                if (send.Text.IsNotNullOrWhiteSpace() && _allExeFile_bak.Length > 0)
                {
                    var listFilte = _allExeFile_bak.Select(a => (CheckBox)a.Children[1]).Where(a => a.Content.NullToStr().ToLower().Contains(send.Text.ToLower()));
                    _allExeFile.Clear();
                    foreach (var item in listFilte)
                    {
                        var fileversion = item.Tag as FileVersionInfo;

                        var filepathAll = item.Content.ToString();
                        versionmsgmd5 = cacheMd5File.ContainsKey(filepathAll) ? cacheMd5File[filepathAll] : GetMD5HashFromFile(filepathAll);

                        versionmsg = $"{fileversion?.FileVersion ?? "0"}";

                        var stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                        stackPanel.Children.Add(new CheckBox() { ToolTip = "删除log" });
                        var lstbotx = new CheckBox() { Content = item.Content, IsChecked = true, ToolTip = item.ToolTip, Tag = item.Tag };
                        stackPanel.Children.Add(lstbotx);
                        TextBlock labelVersion = new TextBlock() { Text = versionmsg, Tag = versionmsgmd5, Margin = new Thickness(5, 0, 0, 0), Foreground = Brushes.Red };
                        stackPanel.Children.Add(labelVersion);
                        TextBlock labelMd5 = new TextBlock() { Text = versionmsgmd5.Substring(versionmsgmd5.Length - 5), Tag = versionmsgmd5, Margin = new Thickness(5, 0, 0, 0), Foreground = Brushes.Black };
                        stackPanel.Children.Add(labelMd5);
                        stackPanel.Tag = true;
                        lstbotx.Click += Lstbotx_Checked;
                        _allExeFile.Add(stackPanel);
                    }

                }
                else
                {
                    _allExeFile.Clear();
                    foreach (var item in _allExeFile_bak)
                    {
                        var currck = item.Children[1] as CheckBox;
                        var textBlock = item.Children[2] as TextBlock;

                        var fileversion = currck.Tag as FileVersionInfo;
                        versionmsgmd5 = textBlock.Tag.ToString();

                        var stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
                        stackPanel.Children.Add(new CheckBox() { ToolTip = "删除log" });
                        var lstbotx = new CheckBox() { Content = currck.Content, IsChecked = true, ToolTip = currck.ToolTip, Tag = fileversion };
                        stackPanel.Children.Add(lstbotx);
                        TextBlock labelVersion = new TextBlock() { Text = textBlock.Text, Tag = textBlock.Tag, Margin = new Thickness(5, 0, 0, 0), Foreground = Brushes.Red };
                        stackPanel.Children.Add(labelVersion);
                        TextBlock labelMd5 = new TextBlock() { Text = versionmsgmd5.Substring(versionmsgmd5.Length - 5), Tag = versionmsgmd5, Margin = new Thickness(5, 0, 0, 0), Foreground = Brushes.Black };
                        stackPanel.Children.Add(labelMd5);
                        stackPanel.Tag = true;
                        lstbotx.Click += Lstbotx_Checked;
                        _allExeFile.Add(stackPanel);
                    }
                    initLoadProjcetByFilePath();

                }
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
                //刷新当前进程信
                _getCurrAllProcess = Process.GetProcesses();

                var item = allLst.SelectedItem as StackPanel;

                var isCloseByName = ckCloseByName.IsChecked.GetValueOrDefault();
                string path = (item.Children[1] as CheckBox).Content.ToString();
                string currdir = System.IO.Path.GetDirectoryName((item.Children[1] as CheckBox).Content.ToString());

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

        private void btnSetTime_Copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    path = Directory.GetParent(path).ToString();
                }
                var openfile = System.IO.Path.Combine(path, @"AppData\Local\Yintech");
                FileGet.OpenFolder(openfile);
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

                var toSendPackage = _allExeFile.Where(a => (a.Children[1] as CheckBox).IsChecked.GetValueOrDefault() == true);

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
                    var currck = item.Children[1] as CheckBox;
                    var textBlock = item.Children[2] as TextBlock;
                    var fileversion = currck.Tag as FileVersionInfo;


                    var md5all = GetMD5HashFromFileDit(currck.Content.ToString());

                    tosaveCsv.AppendLine($"{currck.ToolTip},{fileversion?.FileVersion ?? "0"},{textBlock.Tag},{md5all},{currck.Content}");
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

        private void MenuItem_Click_top(object sender, RoutedEventArgs e)
        {
            this.Topmost = !this.Topmost;
            IsTop(this.Topmost);
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

        private void CheckMD5fileCsv(string tmpPath)
        {
            //当前选择文件
            var toSendPackage = _allExeFile.Where(a => (a.Children[1] as CheckBox).IsChecked.GetValueOrDefault() == true);

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
            var getCurrTocheckList = new VersionCheckMd5Lst();
            var cacheCheckBox = new Dictionary<string, StackPanel>();
            foreach (var item in toSendPackage)
            {
                var currck = item.Children[1] as CheckBox;
                var textBlock = item.Children[2] as TextBlock;
                var fileversion = currck.Tag as FileVersionInfo;

                cacheCheckBox.TryAdd(textBlock.Tag.ToString(), item);

                var md5all = GetMD5HashFromFileDit(currck.Content.ToString());

                var listStr = $"{currck.ToolTip},{fileversion?.FileVersion ?? "0"},{textBlock.Tag},{md5all},{currck.Content}";
                getCurrTocheckList.loadLine(listStr);
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
                var currStack = cacheCheckBox[item.md5];

                VersionCheckMd5 getExitCheck;

                getCsvFileCheckMd5.cacheDic.TryGetValue(item.md5, out getExitCheck);

                if (getExitCheck == null || (getExitCheck != null && item.md5All.IsNotNullOrWhiteSpace() && getExitCheck.md5All != item.md5All))
                {
                    if ((getExitCheck != null && getExitCheck.md5All != item.md5All))
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
                MessageBox.Show($"校验全部通过。");
            }

        }
        private void sortBySelect_Click(object sender, RoutedEventArgs e)
        {
            sortBySelectCheck();
        }
        private void sortBySelectCheck(bool isSort = true)
        {
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
        async Task<bool> SetCurrPathAction(string tmppath)
        {
            this.Dispatcher.Invoke(() =>
            {
                currPath = tmppath;
                Title = $"{this.version}->当前目录:{currPath}";

            });
            await LoadData("", true);
            return true;
        }
        void IsTop(bool flag)
        {
            if (flag)
            {
                btntop.Content = "最顶";
                btntop.Background = Brushes.GreenYellow;
            }
            else
            {
                btntop.Content = "置顶";
                btntop.Background = Brushes.Transparent;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Topmost = !this.Topmost;
            IsTop(this.Topmost);
        }
        #region log show
        /// <summary>
        /// 启动线程相关
        /// </summary>
        private void initThread()
        {
            //log show
            startThread();
        }

        /// <summary>
        /// 全局时间变量 每10毫秒更新一次
        /// yyyy-MM-dd HH:mm:ss fff
        /// </summary>
        public string DateTimeNowAll { get; private set; }
        public string DateTimeNowHHmmSSfffAll { get; private set; }
        private void workProcess2()
        {
            _runShow = true;
            while (_runShow)
            {
                try
                {
                    var currTime = DateTime.Now;
                    this.DateTimeNowAll = currTime.ToString("yyyy-MM-dd HH:mm:ss fff");
                    this.DateTimeNowHHmmSSfffAll = currTime.ToString("HH:mm:ss.fff");
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Thread.Sleep(10);
            }

        }
        public void startThread()
        {
            Thread TimeThread = new Thread(workProcess2);
            TimeThread.IsBackground = true;
            TimeThread.Name = "TimeThread";
            TimeThread.Start();

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
                            sb.AppendLine($"{DateTimeNowHHmmSSfffAll}->{msg}");
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
        public void changeNoticeQueue(string msg, bool ishow = true, int type = 1)
        {
            switch (type)
            {
                case 2:
                    msg = $"Error:{msg}";
                    //LogHelper.WriteErrorLog(msg);
                    break;
                case 3:
                    //LogHelper.WriteLog("adminLog", msg);
                    break;
                default:
                    //LogHelper.WriteSystemLog(msg);
                    break;
            }
            if (ishow)
            {
                _commandLogQueue.Enqueue(msg);
            }
        }
        private void AddToShowLog(string msg, bool ishow = true, bool isClear = false)
        {

            if (!ishow)
            {
                return;
            }
            SpinWait spinWait = new SpinWait();
            rtRec.Dispatcher.BeginInvoke(new Action(() =>
            {
                spinWait.SpinOnce();
                try
                {
                    if (rtRec.Document.Lines.Count > 10000)
                    {
                        rtRec.Document.Text = "";
                    }
                    if (!isClear)
                    {
                        rtRec.AppendText(msg);
                    }
                    else
                    {
                        rtRec.Document.Text = "";
                        rtRec.AppendText(msg);
                    }
                    rtRec.ScrollToEnd();
                }
                catch { }
            }));
        }
        #endregion

        #region edit
        void initAvalonEditBefore(string assemblyName = "Tools.OpenExe")
        {
            try
            {
                //
                LoadCustomXshd($"{assemblyName}.Log.xshd", "log", new string[] { ".log", ".txt" });

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
        void initAvalonEditEnd(string title)
        {
            try
            {

                rtRec.AppendText($"...{title}\n");
                rtRec.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("log");
                //rtSend.SyntaxHighlighting = customHighlighting;
                //initial highlighting now set by XAML

                SearchPanel.Install(rtRec);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }
        #endregion
        #region 可变大小
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
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            rtRec.Text = "";
        }

        #endregion

        private void ckLogSearch_Click(object sender, RoutedEventArgs e)
        {
            var btn = ((Button)sender);
            this.Cursor = Cursors.Wait;
            try
            {
                btn.IsEnabled = false;

                if (txtFilter.Text.IsNullOrWhiteSpace() && txtFilterNo.Text.IsNullOrWhiteSpace())
                {
                    return;
                }

                var toSendPackage = _allExeFile.Where(a => (a.Children[1] as CheckBox).IsChecked.GetValueOrDefault() == true);

                if (!toSendPackage.HasItem())
                {
                    MessageBox.Show("当前没有勾选文件.");
                    return;
                }
                bool isFind = false;
                StringBuilder sb = new StringBuilder();
                foreach (var item in toSendPackage)
                {
                    var currck = item.Children[1] as CheckBox;
                    var textBlock = item.Children[2] as TextBlock;
                    var fileversion = currck.Tag as FileVersionInfo;

                    var result = currck.Content.ToString().FilePathGetAllLineSearchAll(txtFilter.Text, txtFilterNo.Text, "", Encoding.UTF8, txtTopCount.Text.TryToInt());
                    if (result.Count > 0)
                    {
                        sb.Clear();
                        sb.AppendLine($"{currck.Content}:");
                        foreach (var tmpstr in result)
                        {
                            sb.AppendLine($"行号[{tmpstr.Key}]->{tmpstr.Value}");
                        }
                        changeNoticeQueue(sb.ToString());
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

        private async void GetName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                if (!isInitOver || e.AddedItems.Count <= 0)
                {
                    return;
                }
                var fileExt = (e.AddedItems[0] as ListBoxItem).Content.NullToStr();
                await LoadData(fileExt);
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

        private async void fixLogSearch_Click(object sender, RoutedEventArgs e)
        {
            var btn = ((Button)sender);
            this.Cursor = Cursors.Wait;
            try
            {
                btn.IsEnabled = false;


                var toSendPackage = _allExeFile.Where(a => (a.Children[1] as CheckBox).IsChecked.GetValueOrDefault() == true);

                if (!toSendPackage.HasItem())
                {
                    MessageBox.Show("当前没有勾选文件.");
                    return;
                }
                var allTextPath = toSendPackage.Select(a => (a.Children[1] as CheckBox).Content.ToString()).Distinct().ToList();
                bool isFind = false;

                rtRec.Text = "";
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

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var toJson = rtRec.Text;

                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "log文件 (*.log)|*.log|txt文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
                saveFileDialog.Title = "保存当前内容";
                saveFileDialog.InitialDirectory = projectFilePath;
                saveFileDialog.FileName = $"Search_" + DateTime.Now.Date.ToString("yyyyMMdd");
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
        #region 自动检测fix协议差异功能
        Dictionary<string, bool> isDiffSetDic = new Dictionary<string, bool>();
        bool isDiffRun = false;
        string projectFilePathFix { get; set; }
        string projectFilePathFixDiff { get; set; }
        int sleepTime = 30 * 60 * 1000;
        private async void fixLogAutoCheck_Click(object sender, RoutedEventArgs e)
        {
            var btn = ((Button)sender);
            try
            {
                btn.IsEnabled = false;
                isDiffRun = true;
                projectFilePathFix = System.IO.Path.Combine(projectFilePath, "Fix");
                projectFilePathFix.FileDirectoryCreateDirectory();

                var txturl = txtfixurl.Text;

                var currDate = DateTime.Now.ToString("yyyyMMdd");
                isDiffSetDic[currDate] = false;
                await Task.Factory.StartNew(() =>
                 {
                     changeNoticeQueue($"**Fix自动检测运行成功：{currDate}");
                     StartDiff(txturl, currDate);
                 });
                btn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // btn.IsEnabled = true;
            }

            await Task.Delay(10);
        }
        void StartDiff(string url, string currDate)
        {
            try
            {
                string preDate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
                while (isDiffRun)
                {

                    currDate = DateTime.Now.ToString("yyyyMMdd");
                    if (isDiffSetDic.ContainsKey(currDate) && isDiffSetDic[currDate])
                    {
                        changeNoticeQueue($"**Fix自动检测运行中：当日{currDate}已处理，等待{sleepTime / 1000}秒继续，到下一日开始新的对比");
                        Thread.Sleep(sleepTime);
                        continue;
                    }
                    try
                    {
                        isDiffSetDic[currDate] = false;
                        //下载处理,今天
                        var currFixFile = System.IO.Path.Combine(projectFilePathFix, $"{currDate}{url.CreateFileName()}");
                        url.DownloadFile(currFixFile);
                        if (!currFixFile.FileExists())
                        {
                            changeNoticeQueue($"当日文件不存在，等待3秒继续：{currFixFile}");
                            Thread.Sleep(3000);
                            continue;
                        }
                        //昨天
                        preDate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
                        string preDateFixFile = System.IO.Path.Combine(projectFilePathFix, $"{preDate}{url.CreateFileName()}");

                        if (!preDateFixFile.FileExists())
                        {
                            changeNoticeQueue($"昨日文件不存在,取消当日对比：{preDateFixFile}");
                            isDiffSetDic[currDate] = true;
                            continue;
                        }
                        //两文件md5
                        var currmd5 = currFixFile.GetMD5HashFromFile();
                        var precurrmd5 = preDateFixFile.GetMD5HashFromFile();
                        if (currmd5 == precurrmd5)
                        {
                            changeNoticeQueue($"昨日，今日文件一样，无需对比,取消当日对比：\n{currFixFile}\n{preDateFixFile}");
                            isDiffSetDic[currDate] = true;
                            continue;
                        }
                        //fc命令
                        var getResult = "FC".RunCmd(currFixFile, preDateFixFile).Split(Environment.NewLine.ToCharArray());
                        var resultDiff = $"\n###Fix对比差异********{currDate}:{preDate}****************\n{string.Join(Environment.NewLine, getResult.Skip(8).Take(getResult.Length - 11))}";
                        changeNoticeQueue(resultDiff);
                        var currFixFileDiff = System.IO.Path.Combine(projectFilePathFix, $"{currDate}_{preDate}_diff_{url.CreateFileName()}.txt");
                        currFixFileDiff.FilePathSaveContent(resultDiff, Encoding.UTF8);
                        changeNoticeQueue($"###Fix对比差异结果保存：{currFixFileDiff}");
                        isDiffSetDic[currDate] = true;

                    }
                    catch (Exception ex)
                    {
                        changeNoticeQueue(ex.Message);
                        Thread.Sleep(3000);
                        continue;
                    }

                }
            }
            catch (Exception ex)
            {
                changeNoticeQueue(ex.Message);
                fixLogDiffCheck.IsEnabled = true;
            }
        }
        #endregion

        private void fixLogAutoStop_Click(object sender, RoutedEventArgs e)
        {
            isDiffRun = false;
            fixLogDiffCheck.IsEnabled = true;
        }

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
                    var stackPanel = GetStack(fileInfo);
                    _allExeFile.Add(stackPanel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItem_Click_Copyto(object sender, RoutedEventArgs e)
        {
            //复制到另一目录
            try
            {
                var allSelect = _allExeFile.Where(a => (a.Children[1] as CheckBox).IsChecked.GetValueOrDefault() == true);

                var getcurrItem = ((StackPanel)allLst.SelectedItem).Children[1] as CheckBox;
                CopyToWindow copyToWindow = new CopyToWindow();
                copyToWindow.copytoWin.SetDestFilePath(getcurrItem.Content.NullToStr());
                copyToWindow.copytoWin.currPath = currPath;
                if (allSelect.HasItem())
                {
                    foreach (var item in allSelect)
                    {
                        var currck = item.Children[1] as CheckBox;
                        if (currck.Content == getcurrItem.Content)
                        {
                            continue;
                        }
                        copyToWindow.copytoWin.SetCopyToFilePath(currck.Content.NullToStr());
                        break;
                    }

                }
                this.Topmost = false;
                copyToWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
       
        private void NewWindows_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NewMainWindow win = new NewMainWindow();
                win.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void NewWindows2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainRibbonWindow win = new MainRibbonWindow();
                win.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
