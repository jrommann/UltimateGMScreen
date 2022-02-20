using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Ultimate_GM_Screen.Entities
{
    /// <summary>
    /// Interaction logic for UC_Entities.xaml
    /// </summary>
    public partial class UC_Entities : UserControl
    {
        public UC_Entities()
        {
            InitializeComponent();

            DatabaseManager.OnEntitiesChanged += DatabaseManager_OnEntitiesChanged;
        }

        private void DatabaseManager_OnEntitiesChanged(Entity specificItem = null)
        {            
            LoadTreeView(DatabaseManager.Entiies_GetAll());
        }

        private void button_delete_Click(object sender, RoutedEventArgs e)
        {

        }

        void LoadTreeView(List<Entity> entries)
        {
            var parents = new List<Dictionary<string, TreeViewItem>>();
            treeView.Items.Clear();
            foreach (var e in entries)
            {
                if (!string.IsNullOrEmpty(e.Path))
                {
                    #region -> build tree
                    var split = e.Path.Split('/');
                    TreeViewItem parent = null;
                    for (int i = 0; i < split.Length; i++)
                    {
                        if (parents.Count <= i)
                            parents.Add(new Dictionary<string, TreeViewItem>());

                        if (parents[i].ContainsKey(split[i]))
                            parent = parents[i][split[i]];
                        else
                        {

                            var p = new TreeViewItem();
                            p.Header = split[i];
                            p.IsExpanded = true;

                            if (parent != null)
                                parent.Items.Add(p);
                            else
                                treeView.Items.Add(p);

                            parents[i].Add(split[i], p);
                            parent = p;
                        }
                    }
                    #endregion

                    parent?.Items.Add(e);
                }
                else
                    treeView.Items.Add(e);
            }

            try
            {
                treeView.Items.SortDescriptions.Clear();
                treeView.Items.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending));
                foreach (var i in parents)
                {
                    foreach (var kv in i)
                    {
                        kv.Value.Items.SortDescriptions.Clear();

                        if (kv.Value.Items[0] is Entity)
                            kv.Value.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                        else
                            kv.Value.Items.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending));
                    }
                }
            }
            catch { }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            noteEditor.Load();
            try { LoadTreeView(DatabaseManager.Entiies_GetAll()); } catch { }
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is Entity)
                noteEditor.Load(e.NewValue as Entity, true);
        }
    }
}
