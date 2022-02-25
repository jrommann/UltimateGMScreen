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

namespace Ultimate_GM_Screen.Resources
{
    /// <summary>
    /// Interaction logic for UC_Resources.xaml
    /// </summary>
    public partial class UC_Resources : UserControl
    {
        Resource _currentResource;
        bool _loaded = false;

        public UC_Resources()
        {
            InitializeComponent();
            
            DatabaseManager.OnResourcesChanged += DatabaseManager_OnResourcesChanged;
        }

        private void DatabaseManager_OnResourcesChanged(Resource specificItem = null)
        {
            LoadTreeView(DatabaseManager.Resources_GetAll());
        }

        void LoadTreeView(List<Resource> entries)
        {
            var parents = new List<Dictionary<string, TreeViewItem>>();
            treeView.Items.Clear();

            #region -> build tree
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
                }
            }
            #endregion

            #region -> populate tree
            foreach (var e in entries)
            {
                if (!string.IsNullOrEmpty(e.Path))
                {
                    string path = string.Format("{0}/{1}", e.Path, e.Name);
                    var split = path.Split('/');
                    TreeViewItem parent = null;
                    int pCount = parents.Count;
                    for (int i = 0; i < split.Length; i++)
                    {
                        if (i < pCount && parents[i].ContainsKey(split[i]))
                            parent = parents[i][split[i]];
                    }

                    if (parent.Header as string == e.Name)
                        parent.Header = e;
                    else
                        parent.Items.Add(e);
                }
                else
                    treeView.Items.Add(e);
            }
            #endregion

            try
            {
                treeView.Items.SortDescriptions.Clear();
                treeView.Items.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending));
                foreach (var i in parents)
                {
                    foreach (var kv in i)
                    {
                        kv.Value.Items.SortDescriptions.Clear();

                        if (kv.Value.Items[0] is Resource)
                            kv.Value.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                        else
                            kv.Value.Items.SortDescriptions.Add(new SortDescription("Header", ListSortDirection.Ascending));
                    }
                }
            }
            catch { }
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is Resource)
                ChangeResource(e.NewValue as Resource, true);
            else if (e.NewValue is TreeViewItem)
            {
                var t = e.NewValue as TreeViewItem;
                if (t.Header is Resource)
                    ChangeResource(t.Header as Resource, true);
            }
        }

        void ChangeResource(Resource res, bool edit)
        {
            resourceEditor.Save();
            resourceEditor.Load(res, edit);
            _currentResource = res;
        }

        private void button_delete_Click(object sender, RoutedEventArgs e)
        {
            if (_currentResource != null)
            {
                DatabaseManager.Delete(_currentResource);
                _currentResource = null;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded)
                return;

            try { LoadTreeView(DatabaseManager.Resources_GetAll()); } catch { }
            _loaded = true;
        }
    }
}
