using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Tools.OpenExe
{
    /// <summary>
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
       
        public SettingWindow()
        {
            InitializeComponent();

            initFirst();
            this.Closing += SettingWindow_Closing;
        }

        private void SettingWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            settingPage.Closing();
        }

        public void initFirst()
        {
            this.Topmost = true;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
       
    }
}
