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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tools.OpenExe.Exts;

namespace Tools.OpenExe
{
    /// <summary>
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : Page
    {
        private ObservableCollection<string> _cacheListBinding = new ObservableCollection<string>();
        public ConcurrentDictionary<string, string> _allFileDit = new ConcurrentDictionary<string, string>();
        public Func<string, Task<bool>> SetCurrPathAction { get; set; }
        public Action<List<string>, bool> AutoSaveAction { get; set; }
        string currPath;
        public SettingPage()
        {
            InitializeComponent();
        }
        public void Closing()
        {
            AutoSaveAction?.Invoke(_allFileDit.Keys.ToList(), true);
        }

        public void initFirst()
        {
            //this.Topmost = true;
            //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            allfileslstbox.ItemsSource = _cacheListBinding;

        }
        public void initDic(List<string> files)
        {
            foreach (var item in files)
            {
                currPath = item;
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

                currPath = dialog.SelectedPath;

                txtfilePathData.Text = currPath;
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
                    currPath = txtfilePathData.Text.Trim().Trim('\\');
                    if (_allFileDit.ContainsKey(currPath))
                    {
                        return;
                    }
                    _allFileDit[currPath] = currPath;
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
                    Closing();
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
                    currPath = dd;

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
    }
}
