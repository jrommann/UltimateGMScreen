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
        public UC_Web_Browser()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            browser.Source = new Uri(textBox.Text, UriKind.Absolute);
        }
    }
}
