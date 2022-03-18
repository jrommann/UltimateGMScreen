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

namespace Ultimate_GM_Screen.Search
{
    /// <summary>
    /// Interaction logic for UC_Search.xaml
    /// </summary>
    public partial class UC_Search : UserControl
    {
        List<Entities.Entity> _notes = new List<Entities.Entity>();

        public UC_Search()
        {
            InitializeComponent();
        }       

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            _notes = DatabaseManager.Entities_GetAll();
        }

        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }

        void Search()
        {
            if (_notes.Count == 0)
                _notes = DatabaseManager.Entities_GetAll();

            resultList.Children.Clear();

            var search = search_textBox.Text;
            var list = _notes.FindAll(x => x.Details.Contains(search, StringComparison.OrdinalIgnoreCase) || x.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            foreach (var l in list)
            {
                var r = new UC_SearchResult();
                r.Load(l);
                resultList.Children.Add(r);
            }
        }

        private void search_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
                Search();
        }
    }
}
