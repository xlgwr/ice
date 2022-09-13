using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Tools.OpenExe.Exts
{
    class HwndSourceHookDemo
    {
        #region HwndSourceHook 进程通信
        private string msgstr = "Tools.OpenExe";
        private int msg;
        public Action<string, bool, int> changeNoticeQueueAction { get; set; }

        public void changeNoticeQueue(string msg, bool ishow = true, int type = 1)
        {
            changeNoticeQueueAction?.Invoke(msg, ishow, type);
        }
        /// <summary>
        /// 激活显示
        /// </summary>
        public const int HWND_BROADCAST = 0xffff;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool PostMessage(int hhwnd, uint msg, IntPtr wparam, IntPtr lparam);

        #region Sendmessage
        const int WM_COPYDATA = 0x004A; // 固定数值，不可更改 public struct copydatastruct         

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData; // 任意值
            public int cbData; //指定LpData内存区域的字节数 
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;//发送给目标窗口所在进程的数话
        }
        #endregion
        /// <summary>
        /// 导出SendMessage函数
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="Msg"></param>
        /// <param name=""></param>
        /// <param name=""></param>
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT IParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint RegisterWindowMessage(string lpString);

        /// <summary>
        /// 发送信息
        /// </summary>
        public void PostMessages()
        {
            msg = (int)RegisterWindowMessage(msgstr);
            if (msg == 0)
            {
                MessageBox.Show(Marshal.GetLastWin32Error().ToString());
            }
            var dd = PostMessage(HWND_BROADCAST, (uint)msg, IntPtr.Zero, IntPtr.Zero);

        }

        public void SendMessage(string processName, string sendmsg = "start", string title = "Tools.OpenExe")
        {
            // 枚举进程
            Process[] procs = Process.GetProcesses().ToList().Where(a => a.ProcessName.Contains(processName, StringComparison.InvariantCultureIgnoreCase)).ToArray();
            foreach (Process p in procs)
            {
                if (p.ProcessName.Equals(processName))
                {
                    // 获款目标进程句柄
                    IntPtr hWnd = GetProcessWindows(p.Id);// p.MainWindowHandle;
                    if (hWnd.ToInt32() == 0)
                    {
                        hWnd = FindWindow(null, title);
                        changeNoticeQueue($"SendMessage hwnd:{hWnd.ToInt32()},FindWindow:{title}");
                    }
                    //封装消息
                    byte[] sarr = System.Text.Encoding.Default.GetBytes(sendmsg);

                    int len = sarr.Length;
                    COPYDATASTRUCT cds2;
                    cds2.dwData = (IntPtr)0;
                    cds2.cbData = len + 1;
                    cds2.lpData = sendmsg;

                    changeNoticeQueue($"SendMessage hwnd:{hWnd.ToInt32()},WM_COPYDATA:{WM_COPYDATA},{sendmsg}");

                    // 发送消息 
                    SendMessage(hWnd, WM_COPYDATA, IntPtr.Zero, ref cds2);
                }
            }

        }
        /// <summary>
        /// 根据窗口标题查找窗体
        /// </summary>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        static extern IntPtr FindWindowEx(IntPtr parentWindow, IntPtr previousChildWindow, string windowClass, string windowTitle);
        [DllImport("user32.dll")]
        static extern IntPtr GetWindowThreadProcessId(IntPtr window, out int process);
        [DllImport("user32.dll")]
        static extern bool IsWindowVisible(IntPtr hWnd);
        private static IntPtr GetProcessWindows(int process)
        {
            IntPtr pLast = IntPtr.Zero;
            do
            {
                pLast = FindWindowEx(IntPtr.Zero, pLast, null, null);
                int iProcess_;
                GetWindowThreadProcessId(pLast, out iProcess_);
                if (iProcess_ == process)
                {
                    if (IsWindowVisible(pLast))
                        return pLast;
                }
            } while (pLast != IntPtr.Zero);

            return pLast;
        }


        /// <summary>
        /// https://docs.microsoft.com/zh-cn/windows/win32/menurc/wm-syscommand?redirectedfrom=MSDN
        /// SC_MAXIMIZE 0xF030 最大化窗口。SC_MINIMIZE 0xF020 最小化窗口。SC_RESTORE 0xF120 将窗口还原到其正常位置和大小。
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:/* WM_GETMINMAXINFO */
                    //    WmGetMinMaxInfo(hwnd, lParam);
                    //    handled = true;
                    changeNoticeQueue($"hwnd:{hwnd.ToInt32()},msg:{msg}");
                    break;
                case WM_COPYDATA:
                    ActionSendMessageTodo(lParam);
                    break;
                default: break;
            }
            if (this.msg == msg)
            {
                changeNoticeQueue($"{msg}:todo:postmessage");
            }
            return (System.IntPtr)0;
        }
        /// <summary>
        /// 接收SendMessage功能
        /// </summary>
        /// <param name="lParam"></param>
        public void ActionSendMessageTodo(IntPtr lParam)
        {
            // 接收封装的消息
            COPYDATASTRUCT cds = (COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(COPYDATASTRUCT));
            // 获取消息内容                                                        
            string rece = cds.lpData;
            // 自定义行为   
            if (rece == "start")
            {
                changeNoticeQueue($"WM_COPYDATA:{WM_COPYDATA}:todo:{rece}");
                //isToShow();
            }
        }

        //protected override void OnSourceInitialized(EventArgs e)
        //{
        //    base.OnSourceInitialized(e);
        //    System.Windows.Interop.HwndSource hs = PresentationSource.FromVisual(this) as System.Windows.Interop.HwndSource;
        //    hs.AddHook(new System.Windows.Interop.HwndSourceHook(WndProc));
        //}

        #endregion
    }
}
