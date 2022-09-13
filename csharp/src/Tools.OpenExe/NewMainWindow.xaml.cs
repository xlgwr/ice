using AvalonDock.Layout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Tools.OpenExe.UserControlComm;

namespace Tools.OpenExe
{
    /// <summary>
    /// NewMainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewMainWindow : Window
    {
        public NewMainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            initFirst();
        }

        private void initFirst()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
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
        private void OnShowWinformsWindow(object sender, RoutedEventArgs e)
        {

        }

        private void DockManager_DocumentClosing(object sender, AvalonDock.DocumentClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close the document?", "AvalonDock Sample", MessageBoxButton.YesNo) == MessageBoxResult.No)
                e.Cancel = true;
        }

        private void OnLayoutRootPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var activeContent = ((LayoutRoot)sender).ActiveContent;
            if (e.PropertyName == "ActiveContent")
            {
                Debug.WriteLine(string.Format("ActiveContent-> {0}", activeContent));
            }
        }

        private void AddTwoDocuments_click(object sender, RoutedEventArgs e)
        {
            var firstDocumentPane = dockManager.Layout.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            if (firstDocumentPane != null)
            {
                LayoutDocument doc = new LayoutDocument
                {
                    Title = "Test1"
                };
                doc.Content = new UserControl1();
                firstDocumentPane.Children.Add(doc);

                LayoutDocument doc2 = new LayoutDocument
                {
                    Title = "Test2"
                };
                firstDocumentPane.Children.Add(doc2);
            }

            var leftAnchorGroup = dockManager.Layout.LeftSide.Children.FirstOrDefault();
            if (leftAnchorGroup == null)
            {
                leftAnchorGroup = new LayoutAnchorGroup();
                dockManager.Layout.LeftSide.Children.Add(leftAnchorGroup);
            }

            leftAnchorGroup.Children.Add(new LayoutAnchorable() { Title = "New Anchorable" });
        }

        private void OnToolWindow1Hiding(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to hide this tool?", "AvalonDock", MessageBoxButton.YesNo) == MessageBoxResult.No)
                e.Cancel = true;
        }
    }
}
