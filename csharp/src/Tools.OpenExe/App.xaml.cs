using ControlzEx.Theming;
using Solomon.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tools.OpenExe.Exts;
using Tools.OpenExe.ICEApp;

namespace Tools.OpenExe
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 当前运行名
        /// </summary>
        public const string currExeName = "Tools.OpenExe";
        public static MainRibbonWindow _mainRibbonWindow;
        public static ActionAll _ActionAll = new ActionAll();
        public static List<string> dlllist = new List<string>();
        public static ProcessCommunicationPipeToDo _processCommunicationPipeToDo;
        /// <summary>
        /// 当前exe filepath
        /// </summary>
        public static string filename;
        public static string filenameMd5;
        public static App app;
        [STAThread]
        protected static void Main()
        {
            try
            {


                app = new App();

                AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
                //当类型(Type)通过反射加载失败的时候会触发TypeResolve事件，这里注册TypeResolve事件的处理函数为CurrentDomain_TypeResolve
                //AppDomain.CurrentDomain.TypeResolve += CurrentDomain_TypeResolve;

                //
                App.Current.DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                //Task线程内未捕获异常处理事件
                TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
                Application.Current.Resources.Clear();

                app.InitializeComponent();

                AppInit();

                app.Run();

                StartupCheck();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        }

        public static void AppInit()
        {
            try
            {

                //AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad; ;
                filename = $"{System.AppContext.BaseDirectory}{currExeName}.exe";
                filenameMd5 = filename.MD5Encrypt();
                _processCommunicationPipeToDo = new ProcessCommunicationPipeToDo();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static Assembly CurrentDomain_TypeResolve(object sender, ResolveEventArgs args)
        {
            throw new NotImplementedException();
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            var dd = 2;
        }
        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            try
            {
                var executingAssembly = Assembly.GetExecutingAssembly();
                var assemblyName = new AssemblyName(args.Name);

                var path = assemblyName.Name + ".dll";
                if (assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture) == false)
                    path = $@"{assemblyName.CultureInfo}\{path}";

                using (var stream = executingAssembly.GetManifestResourceStream(path))
                {
                    if (stream == null) return null;
                    var assemblyRawBytes = new byte[stream.Length];
                    stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                    dlllist.Add(path);
                    return Assembly.Load(assemblyRawBytes);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// Fluent.dll
        /// ControlzEx.dll
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isLoad"></param>
        void SaveLoadDll(string path = "Fluent.dll", bool isLoad = false)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            using (var stream = executingAssembly.GetManifestResourceStream(path))
            {
                if (stream == null) return;
                var assemblyRawBytes = new byte[stream.Length];
                stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                if (isLoad)
                {
                    Assembly.Load(assemblyRawBytes);
                }
                stream.StreamToFile(path);
            }
        }
        /// <summary>
        /// 初始化样式资源
        /// </summary>
        public static void loadDic(MainRibbonWindow mainRibbonWindow, Func<bool> funcToDo)
        {
            _mainRibbonWindow = mainRibbonWindow;
            _processCommunicationPipeToDo.changeNoticeQueueAction = mainRibbonWindow.changeNoticeQueue;
            _processCommunicationPipeToDo.isShowToDoAction = mainRibbonWindow.isToShow;

            if (!funcToDo.Invoke())
            {
                app.Shutdown();
                App.Current.Shutdown();
                Environment.Exit(0);
                return;
            }
            _processCommunicationPipeToDo.ServerInit();

            System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
            ThemeManager.Current.SyncTheme();

            Application.Current.Resources.Clear();

            var rd = new ResourceDictionary();
            rd.Source = new Uri("pack://application:,,,/Fluent;component/Themes/Generic.xaml");

            var rd2 = new ResourceDictionary();
            rd2.Source = new Uri("pack://application:,,,/Fluent;component/Themes/Themes/Light.Green.xaml");

            Application.Current.Resources.MergedDictionaries.Add(rd);
            Application.Current.Resources.MergedDictionaries.Add(rd2);
        }

        private static void StartupCheck()
        {


            /**
             * 当前用户是管理员的时候，直接启动应用程序
             * 如果不是管理员，则使用启动对象启动程序，以确保使用管理员身份运行
             */
            //获得当前登录的Windows用户标示
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            //判断当前登录用户是否为管理员
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                //创建启动对象
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = filename;
                //设置启动动作,确保以管理员身份运行
                startInfo.Verb = "runas";
                try
                {
                    //启动一个新的
                    System.Diagnostics.Process.Start(startInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"非管理员启动，无法修改系统时间，其它功能正常。{ex.Message}，{filename}");
                    //Run(new MainRibbonWindow());
                }
                finally
                {
                    app.Shutdown();
                    Application.Current.Shutdown();
                    Environment.Exit(0);
                }
            }
            else
            {

                //管理员直接启动
                //Run(new MainRibbonWindow());
                //App.Current.Shutdown();

            }
            //启动
        }

        static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.Exception, e.ToString());
            LogHelper.Error("ErrorLog [UnobservedTaskException]" + str);

        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is System.Exception)
            {
                string str = GetExceptionMsg(e.ExceptionObject as Exception, e.ToString());
                LogHelper.Error("ErrorLog [CurrentDomain_Unhandle]" + str);
            }
        }

        static void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.Exception, e.ToString());
            LogHelper.Error("ErrorLog [App_Dispatcher_Unhandle]" + str);
            e.Handled = true;
        }

        /// <summary>
        /// 生成自定义异常消息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="backStr">备用异常消息：当ex为null时有效</param>
        /// <returns>异常字符串文本</returns>
        static string GetExceptionMsg(Exception ex, string backStr)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("****************************异常信息****************************");
            sb.AppendLine("【出现时间】：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            if (ex != null)
            {
                sb.AppendLine("【异常类型】：" + ex.GetType().Name);
                sb.AppendLine("【异常信息】：" + ex.Message);
                sb.AppendLine("【堆栈调用】：" + ex.StackTrace);
                sb.AppendLine("【异常方法】：" + ex.TargetSite);
            }
            else
            {
                sb.AppendLine("【未处理异常】：" + backStr);
            }
            sb.AppendLine("***************************************************************");

            return sb.ToString();
        }

    }
}
