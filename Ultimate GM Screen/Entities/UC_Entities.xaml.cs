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
        Entity _currentEntity;

        public UC_Entities()
        {
            InitializeComponent();

            DatabaseManager.OnEntitiesChanged += DatabaseManager_OnEntitiesChanged;
        }

        private void DatabaseManager_OnEntitiesChanged(Entity specificItem = null)
        {            
            LoadTreeView(DatabaseManager.Entities_GetAll());
        }

        private void button_delete_Click(object sender, RoutedEventArgs e)
        {            
            if(_currentEntity != null)
                DatabaseManager.Delete(_currentEntity);
        }

        void LoadTreeView(List<Entity> entries)
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
                        if (i<pCount && parents[i].ContainsKey(split[i]))
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
            try { LoadTreeView(DatabaseManager.Entities_GetAll()); } catch { }
            Loaded -= UserControl_Loaded;
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                if (e.NewValue is Entity)
                    ChangeNote(e.NewValue as Entity, true);
                else if (e.NewValue is TreeViewItem)
                {
                    var t = e.NewValue as TreeViewItem;
                    if (t.Header is Entity)
                        ChangeNote(t.Header as Entity, true);
                }
            }
        }

        async void ChangeNote(Entity ent, bool edit)
        {            
            await noteEditor.Save();
            noteEditor.Load(ent, edit);
            _currentEntity = ent;
        }

        private void treeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = Common.VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                e.Handled = true;
                if (treeViewItem.Header is Entity)
                {
                    var w = new Window_Entity();
                    w.Load(treeViewItem.Header as Entity);
                    w.Show();
                }
            }
        }
        
    }
}
