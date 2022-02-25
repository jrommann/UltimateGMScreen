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

namespace Ultimate_GM_Screen.WebBrowser
{
    /// <summary>
    /// Interaction logic for UC_Web_Browser.xaml
    /// </summary>
    public partial class UC_Web_Browser : UserControl
    {
        public string StartingAddress { get; set; }
        bool _webInit = false;

        public UC_Web_Browser()
        {
            InitializeComponent();
            InitializeAsync();                
        }

        async void InitializeAsync()
        {
            await browser.EnsureCoreWebView2Async(null);
            _webInit = true;

            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tinymce\\index.html");
            browser.Source = new Uri("file:///" + path);
            browser.DefaultBackgroundColor = System.Drawing.Color.FromArgb(61, 61, 76);
            if (!string.IsNullOrEmpty(StartingAddress))
            {
                browser.Source = new Uri(StartingAddress);
                addressbox.Text = StartingAddress;
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            if(browser.Source != null)
                browser.Reload();
        }

        private void addressbox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter || e.Key == Key.Return)
                browser.Source = new Uri(addressbox.Text, UriKind.Absolute);
        }
    }
}
