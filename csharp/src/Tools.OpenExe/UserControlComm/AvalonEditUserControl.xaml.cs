using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Search;
using Microsoft.Win32;
using Solomon.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Xml;
using Tools.OpenExe.Exts;
using Tools.OpenExe.UserControlComm.BraceFoldings;

namespace Tools.OpenExe.UserControlComm
{
    /// <summary>
    /// AvalonEditUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class AvalonEditUserControl : UserControl, INotifyPropertyChanged
    {
        public Func<Task> initProjectFunc { get; set; }
        public string titleForLog { get; set; } = "";
        public string projectFilePath { get; set; }
        #region Action
        /// <summary>
        /// 显示消息
        /// </summary>
        public Action<string, bool, int> changeNoticeOtherAction { get; set; }
        #endregion
        #region 参数设置
        /// <summary>
        /// 单行超N个字符，不语法高亮
        /// </summary>
        const int MaxLengToSyntaxHightlighting = 10_0000;
        /// <summary>
        /// 超过100w个字符，清空显示
        /// </summary>
        const int MaxLengAll = 100_0000;
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
        #endregion
        public AvalonEditUserControl()
        {
            initAvalonEditBefore();

            InitializeComponent();

            tbToolEdit.Visibility = Visibility.Collapsed;

            this.DataContext = this;

            var msg = $"\n####小提示：\n**不包含：可输入多个分号或逗号隔开，如：123;abc;ddd\n**##特殊显示内容查询：a&b|c&d|e|f&g&h&i";
            initAvalonEditEnd(msg);

            initThread();

            rtRec.Text = App.dlllist.toJsonStr() + "\n";
        }
        ~AvalonEditUserControl()
        {
            _runShow = false;
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
        /// <summary>
        /// 清空显示日志
        /// </summary>
        public void Clear()
        {
            rtRec.Text = "";
        }

        public void InitEditToolBar(string title, string syntax = "Json", int ind = 0)
        {
            titleForLog = title;
            ShowtbToolEdit();
            comboBoxSelectedIndex(ind);
            SetSyntaxHighlighting(syntax);
        }
        public void AddSendTodo(string title, Action action)
        {
            btnRunLog.ToolTip = title;
            btnMenuLog.Header = title;
            btnRunLog.Visibility = Visibility.Visible;
            btnMenuLog.Visibility = Visibility.Visible;
            btnToDoAction = action;
        }
        public void ShowtbToolEdit()
        {
            tbToolEdit.Visibility = Visibility.Visible;
        }
        public void SetSyntaxHighlighting(string syntax = "Json")
        {
            rtRec.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition(syntax);
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                rtRec.Dispatcher.Invoke(() =>
                {
                    rtRec.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition(syntax);
                });
            });
        }
        public void comboBoxSelectedIndex(int ind = 0)
        {
            if (comboBoxDefault != null)
            {
                if (comboBoxDefault.Items.Count > ind)
                {
                    comboBoxDefault.SelectedIndex = ind;
                }
            }
        }
        /// <summary>
        /// 获取肉容
        /// </summary>
        public string Text()
        {
            return rtRec.Text;
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
        public bool _runShow = true;
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
                            sb.AppendLine($"{DateTimeNowAll}->{msg}");
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
                    LogHelper.WriteLog($"Error{titleForLog}Log", msg);
                    rtRec.ScrollToEnd();
                    break;
                case 3:
                    msg = $"Warn:{msg}";
                    LogHelper.Warn(msg);
                    rtRec.ScrollToEnd();
                    break;
                case 6:
                    msg = $"Success:{msg}";
                    LogHelper.WriteLog($"System{titleForLog}Log", msg);
                    break;
                case 7:
                    msg = $"Notice:{msg}";
                    LogHelper.WriteLog($"System{titleForLog}Log", msg);
                    break;
                default:
                    LogHelper.WriteLog($"System{titleForLog}Log", msg);
                    break;
            }
            if (ishow)
            {
                _commandLogQueue.Enqueue(msg);
            }
        }

        void CheckisSyntaxHighlighting()
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
                if ((rtRec.Text.Length + msg.Length) > MaxLengToSyntaxHightlighting)
                {
                    rtRec.AppendText($"\n***************显示语法高亮失效处理:字符长度超过：{MaxLengToSyntaxHightlighting}*********************\n");
                    if (rtRec.SyntaxHighlighting != null)
                    {
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
                    CheckisSyntaxHighlighting();

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
        #endregion
        #region check box tool
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
            if (HighlightingManager.Instance.HighlightingDefinitions.Where(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).Any())
            {
                return;
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
        void initAvalonEditEnd(string title)
        {
            try
            {

                rtRec.AppendText($"...{title}\n");
                rtRec.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("log");
                //rtSend.SyntaxHighlighting = customHighlighting;
                //initial highlighting now set by XAML

                SearchPanel.Install(rtRec);

                //单行超长文本的性能优化
                //rtSend.TextArea.TextView.ElementGenerators.Add(new TruncateLongLines());
                rtRec.TextArea.TextView.ElementGenerators.Add(new TruncateLongLines());

                DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
                foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
                foldingUpdateTimer.Tick += delegate { UpdateFoldings(); };
                foldingUpdateTimer.Start();

                loadFixFileToButton();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }
        #endregion

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            rtRec.Text = "";
        }
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var toJson = rtRec.Text;

                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                var jsonLog = btnMenuLog.Visibility == Visibility.Visible ? "json文件 (*.json)|*.json|log文件 (*.log)|*.log|" : "log文件 (*.log)|*.log|json文件 (*.json)|*.json|";
                saveFileDialog.Filter = $"{jsonLog}txt文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
                saveFileDialog.Title = "保存当前内容";
                saveFileDialog.InitialDirectory = projectFilePath;
                saveFileDialog.FileName = $"Search_" + DateTime.Now.Date.ToString("yyyyMMdd");
                if ((bool)(saveFileDialog.ShowDialog()))
                {
                    //获得文件路径
                    var localFilePath = saveFileDialog.FileName.ToString();
                    localFilePath.FilePathSaveContent(toJson, Encoding.UTF8);

                    if (btnMenuLog.Visibility == Visibility.Visible)
                    {
                        fixDic[localFilePath.GetFileNameWithExtension()] = rtRec.Text;
                        UpdateCombox();
                        changeNotice($"另存文件成功：{localFilePath}");
                    }

                }
                if (initProjectFunc != null)
                {
                    await initProjectFunc?.Invoke();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #region Tool edit to do
        public void changeNotice(string msg, int type = 1, bool show = true)
        {
            changeNoticeOtherAction?.Invoke(msg, show, type);
        }
        private void openFileClick(object sender, RoutedEventArgs e)
        {

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;
            dlg.InitialDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "方案", "消息头");
            if (dlg.ShowDialog() ?? false)
            {
                currentFileName = dlg.FileName;
                dlg.Filter = "消息文件 (*.json)|*.json|所有文件 (*.*)|*.*";
                rtRec.Load(currentFileName);
                rtRec.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(System.IO.Path.GetExtension(currentFileName));
                changeNotice($"打开文件成功：{currentFileName}");
            }
        }
        string currentFileName;
        private void saveFileClick(object sender, RoutedEventArgs e)
        {
            if (currentFileName.IsNullOrWhiteSpace())
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "消息文件 (*.json)|*.json|所有文件 (*.*)|*.*";
                dlg.DefaultExt = ".json";

                //dlg.InitialDirectory =  System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "方案", "消息头");
                //dlg.FileName = @"Man" + currFileNameDefaule + "_" + DateTime.Now.ToyyyyMMddHHmmssExt();

                if (dlg.ShowDialog() ?? false)
                {
                    currentFileName = dlg.FileName;
                    fixDic[currFileNameDefaule] = rtRec.Text;
                    UpdateCombox();
                }
                else
                {
                    return;
                }
            }
            currentFileName.FilePathSaveContentStreamWriter(rtRec.Text, Encoding.UTF8);
            changeNotice($"保存文件成功：{currentFileName}");

        }


        private void saveFileAsClick(object sender, RoutedEventArgs e)
        {

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "消息文件 (*.json)|*.json|所有文件 (*.*)|*.*";

            //dlg.DefaultExt = ".fix";
            dlg.InitialDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "方案", "消息头");
            dlg.FileName = $"{_allProjectFile.Count + 1}_Man_" + currFileNameDefaule.GetFileNameWithoutExtension() + "_" + DateTime.Now.ToyyyyMMddHHmmssExt();

            if (dlg.ShowDialog() ?? false)
            {
                dlg.FileName.FilePathSaveContentStreamWriter(rtRec.Text, Encoding.UTF8);
                fixDic[dlg.FileName.GetFileNameWithExtension()] = rtRec.Text;
                UpdateCombox();
                changeNotice($"另存文件成功：{dlg.FileName}");

            }
            else
            {
                return;
            }
        }

        #endregion

        #region tool bar folding
        public string currFileNameDefaule { get; set; }
        public ComboBox comboBoxDefault { get; set; }
        FoldingManager foldingManager;
        object foldingStrategy;
        void UpdateFoldings()
        {
            if (foldingStrategy is BraceFoldingStrategy)
            {
                ((BraceFoldingStrategy)foldingStrategy).UpdateFoldings(foldingManager, rtRec.Document);
            }
            if (foldingStrategy is XmlFoldingStrategy)
            {
                ((XmlFoldingStrategy)foldingStrategy).UpdateFoldings(foldingManager, rtRec.Document);
            }
        }
        private void HighlightingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (rtRec.SyntaxHighlighting == null)
            {
                foldingStrategy = null;
            }
            else
            {
                switch (rtRec.SyntaxHighlighting.Name)
                {
                    case "XML":
                        foldingStrategy = new XmlFoldingStrategy();
                        rtRec.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        break;
                    case "C#":
                    case "C++":
                    case "PHP":
                    case "Java":
                        rtRec.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(rtRec.Options);
                        foldingStrategy = new BraceFoldingStrategy();
                        break;
                    default:
                        rtRec.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        foldingStrategy = null;
                        break;
                }
            }
            if (foldingStrategy != null)
            {
                if (foldingManager == null)
                    foldingManager = FoldingManager.Install(rtRec.TextArea);
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
        //public string projectFilePath { get; set; }
        public Dictionary<string, string> GetJsonSample()
        {
            var result = new Dictionary<string, string>();
            try
            {
                var currA = typeof(MainWindow).Assembly;
                result["0默认样例"] = currA.GetPropertiesResources("0默认样例").toJsonStr();
            }
            catch (Exception ex)
            {
                return result;

            }
            return result;
        }
        public Dictionary<string, string> getAssemblyJson()
        {
            var result = new Dictionary<string, string>();
            try
            {
                Assembly executingAssembly = Assembly.GetExecutingAssembly();
                var list = executingAssembly.GetManifestResourceNames();

                var assemblyName = new AssemblyName(executingAssembly.FullName);

                string replaceNameStr = $"{assemblyName.Name}.消息头.";
                foreach (var item in list)
                {
                    if (System.IO.Path.GetExtension(item) != ".json")
                    {
                        continue;
                    }
                    using (var stream = executingAssembly.GetManifestResourceStream(item))
                    {
                        if (stream == null) continue;
                        var assemblyRawBytes = new byte[stream.Length];
                        stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                        var getContent = stream.StreamReadString(Encoding.UTF8);
                        result[item.Replace(replaceNameStr, "")] = getContent;
                    }

                }
            }
            catch (Exception ex)
            {
                return result;

            }
            return result;

        }
        public void loadFixFileToButton()
        {
            if (projectFilePath.IsNullOrWhiteSpace())
            {
                projectFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "方案", "消息头");
                projectFilePath.FileDirectoryCreateDirectory();

            }
            var currSample = getAssemblyJson();

            var toNotnames = new List<string>();
            string getFileName = ".json";
            FileGet.allGetFiles.Clear();
            FileGet.getFile(projectFilePath, getFileName, toNotnames);
            var getAllExe = FileGet.allGetFiles;
            _allProjectFile.Clear();
            int sizeCount = 1;
            var tmpallProjectFile = new Dictionary<int, Tuple<string, string>>();

            foreach (var item in getAllExe)
            {
                _allProjectFile.Add(sizeCount, new Tuple<string, string>(item.Name, item.FullName.FilePathGetContent(Encoding.UTF8)));
                sizeCount++;
            }
            Dictionary<string, bool> defaultKe = new Dictionary<string, bool>();
            foreach (var item in _allProjectFile)
            {
                if (currSample.ContainsKey(item.Value.Item1))
                {
                    defaultKe.Add(item.Value.Item1, true);
                    continue;
                }
            }

            foreach (var item in currSample)
            {
                if (defaultKe.ContainsKey(item.Key))
                {
                    continue;
                }
                _allProjectFile.Add(sizeCount, new Tuple<string, string>(item.Key, item.Value));
                sizeCount++;
            }
            sizeCount--;
            initToolBar(sizeCount);
            initComboBox();

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
                tbToolEdit.Children.Add(button);
            }
        }
        public Dictionary<string, string> fixDic = new Dictionary<string, string>();
        void initComboBox(int allSize = 50)
        {
            //if (allSize <= 5)
            //{
            //    return;
            //} 
            fixDic.Clear();
            foreach (var item in _allProjectFile)
            {
                if (item.Key > allSize)
                {
                    continue;
                }
                fixDic.TryAdd(item.Value.Item1, item.Value.Item2);
            }
            if (fixDic.Keys.Count > 0)
            {
                comboBoxDefault = new ComboBox() { ToolTip = "当前目录【消息头】下所有json文件,可手动新增,重启，自动加载。" };
                comboBoxDefault.ItemsSource = fixDic;
                comboBoxDefault.DisplayMemberPath = "Key";
                comboBoxDefault.SelectedValuePath = "Value";
                comboBoxDefault.Width = 50;
                comboBoxDefault.SelectionChanged += ComboBox_SelectionChanged;
                tbToolEdit.Children.Add(comboBoxDefault);
            }
        }
        void UpdateCombox(int allSize = 50)
        {
            if (fixDic.Keys.Count > 0)
            {
                comboBoxDefault.ItemsSource = fixDic;
                comboBoxDefault.Items.Refresh();
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var cb = sender as ComboBox;
                if (cb.SelectedItem != null)
                {
                    var dd = (KeyValuePair<string, string>)cb.SelectedItem;
                    rtRec.Text = dd.Value;
                    currFileNameDefaule = dd.Key;
                    currentFileName = string.Empty;
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
                rtRec.Text = getFixContent.Item2;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
        #region todo 
        public Action btnToDoAction { get; set; }
        private void btnToDo_Click(object sender, RoutedEventArgs e)
        {
            btnToDoAction?.Invoke();
        }

        private void btnRunLog_Click(object sender, RoutedEventArgs e)
        {
            btnToDoAction?.Invoke();
        }
        #endregion
    }
}
