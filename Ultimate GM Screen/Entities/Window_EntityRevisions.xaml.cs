using AdonisUI.Controls;
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
using System.Windows.Shapes;

namespace Ultimate_GM_Screen.Entities
{
    /// <summary>
    /// Interaction logic for Window_EntityRevisions.xaml
    /// </summary>
    public partial class Window_EntityRevisions : AdonisWindow
    {
        public Entity Current { get; set; }

        int _currentPage = 1;
        int _totalPages;
        int _entriesPerPage = 10;

        public Window_EntityRevisions()
        {
            InitializeComponent();
            DatabaseManager.OnRevisionsChanged += DatabaseManager_OnRevisionsChanged;            
        }

        private void DatabaseManager_OnRevisionsChanged()
        {            
            Load();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = "Revisions for " + Current.Name;            
            Load();
        }

        void Load()
        {
            _totalPages = DatabaseManager.Entity_RevisionCount(Current.ID) / _entriesPerPage;
            if (_totalPages == 0)
                menuPanel.Visibility = Visibility.Collapsed;

            revisionsLbl.Content = string.Format("{0} of {1}", _currentPage, _totalPages);

            stackPanel.Children.Clear();            

            var list = DatabaseManager.Entity_GetRevisions(Current.ID, _currentPage, _entriesPerPage);
            foreach (var r in list)
            {
                var n = new UC_EntityRevisionEntry();
                n.Load(r);
                n.HorizontalAlignment = HorizontalAlignment.Stretch;
                stackPanel.Children.Add(n);
            }            
        }

        private void prevBtn_Click(object sender, RoutedEventArgs e)
        {
            int newPage = Math.Max(1, _currentPage - 1);
            if (_currentPage != newPage)
            {
                _currentPage = newPage;
                Load();
            }
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            int newPage = Math.Min(_totalPages, _currentPage + 1);
            if (_currentPage != newPage)
            {
                _currentPage = newPage;
                Load();
            }
        }

        private void deleteAllBtn_Click(object sender, RoutedEventArgs e)
        {
            DatabaseManager.OnRevisionsChanged -= DatabaseManager_OnRevisionsChanged;
            
            foreach (var c in stackPanel.Children)
                DatabaseManager.Delete((c as UC_EntityRevisionEntry).Current);

            DatabaseManager.OnRevisionsChanged += DatabaseManager_OnRevisionsChanged;

            prevBtn_Click(sender, e);
            Load();
        }
    }

}
