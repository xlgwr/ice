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
using Tools.OpenExe.Exts;

namespace Tools.OpenExe.UserControlComm
{
    /// <summary>
    /// CopyTo_UC.xaml 的交互逻辑
    /// </summary>
    public partial class CopyTo_UC : UserControl
    {

        public string currPath { get; set; }
        public CopyTo_UC()
        {
            InitializeComponent();

            initfirst();
        }
        void initfirst()
        {
            //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public void SetDestFilePath(string path)
        {
            txtSource.Text = path.Substring(0, path.LastIndexOf('\\'));
        }
        public void SetCopyToFilePath(string path)
        {
            txtDestination.Text = path.Substring(0, path.LastIndexOf('\\'));
        }
        public void SetPaths(List<string> result)
        {
            if (!result.HasItem())
            {
                return;
            }
            if (result.Count > 1)
            {
                txtSource.Text = result[0];
                txtDestination.Text = result[1];
            }
            else if (result.Count == 1)
            {
                txtSource.Text = result[0];
            }
        }
        public List<string> GetPaths()
        {
            var result = new List<string>() { };
            if (!txtSource.Text.IsNullOrWhiteSpace())
            {
                result.Add(txtSource.Text.Trim());
            }
            if (!txtDestination.Text.IsNullOrWhiteSpace())
            {
                result.Add(txtDestination.Text.Trim());
            }
            return result;
        }
        private async void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Delay(10);
                this.Cursor = Cursors.Wait;
                string destFilePath = txtDestination.Text.Trim();
                string sourceFilepath = txtSource.Text.Trim();
                if (destFilePath == sourceFilepath || destFilePath.IsNullOrWhiteSpace() || sourceFilepath.IsNullOrWhiteSpace())
                {
                    txtDestination.Focus();
                    return;
                }
                if (!sourceFilepath.FileDirectoryExists())
                {
                    MessageBox.Show($"文件目录不存在{sourceFilepath}");
                    return;
                }
                if (!destFilePath.FileDirectoryExists())
                {
                    destFilePath.FileDirectoryCreateDirectory();
                }

                changeNotice(DateTime.Now.ToString(), true);
                //　XCOPY source [destination] [/Y][/-Y] [/A | /M] [/D[:date]] [/P] [/S [/E]] [/W][/C] [/I] [/Q] [/F] [/L] [/H] [/R] [/T] [/U][/K] [/N]
                changeNotice($"复制:{txtSource.Text.Trim()}");
                changeNotice($"到:{txtDestination.Text.Trim()}");
                changeNotice($"开始处理。。。。 \n");

                var getResultAsync = await "XCOPY".RunCmdAsync(txtSource.Text.Trim(), txtDestination.Text.Trim(), "/S", "/E", "/H", "/R", "/Y", "/D");
                var getResult = getResultAsync.Split(Environment.NewLine.ToCharArray());
                changeNotice(string.Join(Environment.NewLine, getResult.Skip(7).Take(getResult.Length - 10)));
                //lblmsg.Text += string.Join(Environment.NewLine, getResult);
                changeNotice("复制成功");

                //System.Windows.Forms.MessageBox.Show("复制增量文件更新成功");
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

        private void btnSeletFilePath_Click(object sender, RoutedEventArgs e)
        {
            SelectPath(txtSource);
        }

        private void btnSelectOtherFilePath_Click(object sender, RoutedEventArgs e)
        {
            SelectPath(txtDestination);

        }

        public void SelectPath(TextBox textBox)
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

                textBox.Text = dialog.SelectedPath;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void changeNotice(string msg, bool isclear = false)
        {
            lblmsg.Dispatcher.Invoke(() =>
            {
                if (isclear)
                {
                    lblmsg.Text = msg;
                }
                else
                {
                    lblmsg.Text += "\n" + msg;
                }
            });
        }
        private async void btnPull_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Delay(10);
                btnPull.IsEnabled = false;
                this.Cursor = Cursors.Wait;
                string destFilePath = txtDestination.Text.Trim();
                string sourceFilepath = txtSource.Text.Trim();
                if (destFilePath == sourceFilepath || sourceFilepath.IsNullOrWhiteSpace())
                {
                    txtDestination.Focus();
                    return;
                }

                if (MessageBox.Show($"是否要Pull!{sourceFilepath}", "提示", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }

                if (!sourceFilepath.FileDirectoryExists())
                {
                    MessageBox.Show($"文件目录不存在{sourceFilepath}");
                    return;
                }
                if (!destFilePath.FileDirectoryExists())
                {
                    destFilePath.FileDirectoryCreateDirectory();
                }

                changeNotice(DateTime.Now.ToString(), true);
                //　XCOPY source [destination] [/Y][/-Y] [/A | /M] [/D[:date]] [/P] [/S [/E]] [/W][/C] [/I] [/Q] [/F] [/L] [/H] [/R] [/T] [/U][/K] [/N]
                changeNotice($"Git Pull:{sourceFilepath}");
                changeNotice($"开始处理。。。。 \n");

                var gitlistCmd = new List<string>()
                {
                    "git pull"
                };
                var getResultAsync = await "CD".RunCmdListAsync(gitlistCmd, sourceFilepath);
                var getResult = getResultAsync.Split(Environment.NewLine.ToCharArray());
                changeNotice(string.Join(Environment.NewLine, getResult.Skip(7).Take(getResult.Length - 10)));
                //changeNotice(string.Join(Environment.NewLine, getResult));
                changeNotice("Git Pull 成功");

                //System.Windows.Forms.MessageBox.Show("Git Pull成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
                btnPull.IsEnabled = true;

            }
        }

        private async void btnPush_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Delay(10);
                btnPush.IsEnabled = false;
                this.Cursor = Cursors.Wait;
                string destFilePath = txtDestination.Text.Trim();
                string sourceFilepath = txtSource.Text.Trim();
                if (destFilePath == sourceFilepath || destFilePath.IsNullOrWhiteSpace())
                {
                    txtDestination.Focus();
                    return;
                }

                if (MessageBox.Show($"是否要Push!{destFilePath}", "提示", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }

                if (!sourceFilepath.FileDirectoryExists())
                {
                    MessageBox.Show($"文件目录不存在{sourceFilepath}");
                    return;
                }
                if (!destFilePath.FileDirectoryExists())
                {
                    destFilePath.FileDirectoryCreateDirectory();
                }

                changeNotice(DateTime.Now.ToString(), true);
                //　XCOPY source [destination] [/Y][/-Y] [/A | /M] [/D[:date]] [/P] [/S [/E]] [/W][/C] [/I] [/Q] [/F] [/L] [/H] [/R] [/T] [/U][/K] [/N]
                changeNotice($"Git Push:{destFilePath}");
                changeNotice($"开始处理。。。。 \n");
                var gitlistCmd = new List<string>()
                {
                    "git pull",
                    "git add -u",
                    $"git commit -m {txtUpdate.Text.Trim()}",
                    "git push"
                };
                var getResultAsync = await "CD".RunCmdListAsync(gitlistCmd, destFilePath);
                var getResult = getResultAsync.Split(Environment.NewLine.ToCharArray());
                lblmsg.Text += string.Join(Environment.NewLine, getResult.Skip(7).Take(getResult.Length - 10));
                //lblmsg.Text += string.Join(Environment.NewLine, getResult);
                changeNotice("Git push 成功");

                //System.Windows.Forms.MessageBox.Show("Git push 成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
                btnPush.IsEnabled = true;

            }
        }
    }
}
