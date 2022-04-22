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
using Ultimate_GM_Screen.Entities;

namespace Ultimate_GM_Screen.Search
{
    /// <summary>
    /// Interaction logic for UC_SearchResult.xaml
    /// </summary>
    public partial class UC_SearchResult : UserControl
    {
        Entity _entry;

        public UC_SearchResult()
        {
            InitializeComponent();
        }

        public void Load(Entity e)
        {
            _entry = e;
            groupbox.Header = e.Name;            
        }

        private void buttonName_Click(object sender, RoutedEventArgs e)
        {
            var entry = new Window_Entity();
            entry.Load(_entry);
            entry.Show();
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            detailsHtml.Html = System.Text.RegularExpressions.Regex.Unescape(_entry.Details);
        }
    }
}
