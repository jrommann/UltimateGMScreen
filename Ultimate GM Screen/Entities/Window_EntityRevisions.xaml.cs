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

        public Window_EntityRevisions()
        {
            InitializeComponent();
            DatabaseManager.OnRevisionsChanged += DatabaseManager_OnRevisionsChanged;
        }

        private void DatabaseManager_OnRevisionsChanged()
        {
            stackPanel.Children.Clear();
            Load();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = "Revisions for " + Current.Name;

            Load();
        }

        void Load()
        {
            var list = DatabaseManager.Entity_GetRevisions(Current.ID);
            foreach (var r in list)
            {
                var n = new UC_EntityRevisionEntry();
                n.Current = r;
                n.HorizontalAlignment = HorizontalAlignment.Stretch;
                stackPanel.Children.Add(n);
            }
        }
    }

}
