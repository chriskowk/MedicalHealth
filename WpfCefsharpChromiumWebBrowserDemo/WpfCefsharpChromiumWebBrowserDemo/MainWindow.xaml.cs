using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfCefsharpChromiumWebBrowserDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public ChromiumWebBrowser _browser;
        public MainWindow()
        {
            InitializeComponent();
            LoadHtmlData();
        }

        public void LoadHtmlData()
        {
            _browser = new ChromiumWebBrowser();
            _browser.Address = "http://www.baidu.com/";

            _browser.SizeChanged += _browser_SizeChanged;
            _browser.FrameLoadEnd += _browser_FrameLoadEnd;
            this.gd.Children.Add(_browser);

        }

        private void _browser_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _browser.UpdateLayout();
        }

        private void _browser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            try
            {
                if (_browser != null && _browser.GetBrowser() != null && _browser.GetBrowser().MainFrame != null)
                {
                    // _browser.GetBrowser().MainFrame.ExecuteJavaScriptAsync("Chart1()");
                }
            }
            catch (Exception ex)
            {
                //  MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txt.Text))
            {
                _browser.Address = txt.Text;
            }
        }
    }
}
