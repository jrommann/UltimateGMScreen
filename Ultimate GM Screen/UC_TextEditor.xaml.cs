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

namespace Ultimate_GM_Screen
{
    /// <summary>
    /// Interaction logic for UC_TextEditor.xaml
    /// </summary>
    public partial class UC_TextEditor : UserControl
    {
        bool _webInit = false;
        string _currentContent;

        public UC_TextEditor()
        {
            InitializeComponent();

            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tinymce\\index.html");
            webView.Source = new Uri("file:///" + path);
            webView.DefaultBackgroundColor = System.Drawing.Color.FromArgb(61, 61, 76);
            webView.NavigationCompleted += WebView_NavigationCompleted;
            InitializeAsync();
        }

        private void WebView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (_webInit)
                Set(_currentContent);
        }

        async void InitializeAsync()
        {
            await webView.EnsureCoreWebView2Async(null);            
            _webInit = true;
        }

        public void Set(string text)
        {
            _currentContent = text;
            webView.ExecuteScriptAsync(string.Format("set('{0}')", text));                
        }

        async public Task<string> Get()
        {
            string result = await webView.ExecuteScriptAsync(@"save()");
            result = result.Remove(0, 1).Remove(result.Length - 2, 1);
            return result;
        }
       
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_webInit)
                webView.Reload();
        }
    }
}
