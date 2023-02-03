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
        List<UC_ResourceEditor> _pinnedEditors = new List<UC_ResourceEditor>();
        List<Button> _pinnedButtons = new List<Button>();
        Resource _currentResource;
        bool _loaded = false;

        public UC_Resources()
        {
            InitializeComponent();
            
            DatabaseManager.OnResourcesChanged += DatabaseManager_OnResourcesChanged;
            DatabaseManager.OnFoldersChanged += DatabaseManager_OnFoldersChanged;

            resourceEditor.OnPinClicked += Editor_OnPinClicked;
        }

        private void DatabaseManager_OnFoldersChanged()
        {
            UpdateResourceList();
        }

        private void DatabaseManager_OnResourcesChanged(Resource specificItem = null, bool pathChanged = false)
        {
            if (pathChanged)
                UpdateResourceList();
        }

        void UpdateResourceList(List<Resource> notes = null)
        {
            if (DatabaseManager.IsOpened)
            {
                #region -> full list
                if (notes == null)
                {
                    #region -> build tree
                    var folders = DatabaseManager.Folders_GetAll(Folders.FolderType.Resource);
                    folders.Sort((x, y) => x.ParentID.CompareTo(y.ParentID));

                    Dictionary<int, TreeViewItem> treeFolders = new Dictionary<int, TreeViewItem>();
                    treeView.Items.Clear();

                    foreach (var f in folders)
                    {
                        TreeViewItem tvi = new TreeViewItem();
                        tvi.Header = f;
                        tvi.IsExpanded = f.IsExpanded;

                        #region -> save expanded / collasped
                        tvi.Expanded += (sender, e) =>
                        {
                            var fe = (sender as TreeViewItem).Header as Folders.FolderEntry;
                            fe.IsExpanded = true;
                            DatabaseManager.Update(fe, false);
                        };
                        tvi.Collapsed += (sender, e) =>
                        {
                            var fe = (sender as TreeViewItem).Header as Folders.FolderEntry;
                            fe.IsExpanded = false;
                            DatabaseManager.Update(fe, false);
                        };
                        #endregion

                        treeFolders.Add(f.ID, tvi);

                        if (f.ParentID == -1)
                            treeView.Items.Add(tvi);
                        else
                        {
                            var treeviewItem = treeFolders[f.ParentID];
                            if (treeviewItem != null)
                                treeviewItem.Items.Add(tvi);
                        }
                    }
                    #endregion

                    if (notes == null)
                        notes = DatabaseManager.Resources_GetAll();

                    foreach (var n in notes)
                    {
                        TreeViewItem tvi = new TreeViewItem();
                        tvi.Header = n;

                        if (n.FolderID == -1)
                            treeView.Items.Add(tvi);
                        else
                        {
                            TreeViewItem parent = null;
                            if (treeFolders.TryGetValue(n.FolderID, out parent))
                                parent.Items.Add(tvi);
                        }
                    }
                }
                #endregion
                #region -> search list
                else
                {
                    treeView.Items.Clear();

                    foreach (var n in notes)
                    {
                        TreeViewItem tvi = new TreeViewItem();
                        tvi.Header = n;
                        treeView.Items.Add(tvi);
                    }
                }
                #endregion
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

        private void btn_Current_Click(object sender, RoutedEventArgs e)
        {
            Switch_Displayed_Note(resourceEditor);
        }

        void Switch_Displayed_Note(UC_ResourceEditor show)
        {
            resourceEditor.Visibility = Visibility.Hidden;
            _pinnedEditors.ForEach(x => x.Visibility = Visibility.Hidden);

            show.Visibility = Visibility.Visible;
        }

        private void Editor_OnPinClicked(Resource note = null)
        {
            if (note == null)
                return;

            var editor = new UC_ResourceEditor();
            editor.SetValue(Grid.RowProperty, 1);
            editor.Load(note, true);
            editor.CanPopout = false;
            editor.Visibility = Visibility.Hidden;
            editor.Pinned = true;
            editor.OnUnpinClicked += Editor_OnUnpinClicked;
            notesGrid.Children.Add(editor);
            _pinnedEditors.Add(editor);

            var btn = new Button();
            btn.Content = note.Name;
            btn.Click += (object sender, RoutedEventArgs e) => { Switch_Displayed_Note(editor); };
            btn.Tag = note;
            _pinnedButtons.Add(btn);

            stackpanel_pinnedNotes.Children.Add(btn);
        }

        private void Editor_OnUnpinClicked(Resource item = null)
        {
            var btn = _pinnedButtons.Find(x => x.Tag == item);
            var editor = _pinnedEditors.Find(x => x.Current == item);
            _pinnedButtons.Remove(btn);
            _pinnedEditors.Remove(editor);

            stackpanel_pinnedNotes.Children.Remove(btn);
            notesGrid.Children.Remove(editor);

            Switch_Displayed_Note(resourceEditor);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded)
                return;

            UpdateResourceList();
            _loaded = true;
        }      
       
        private void button_clearSearch_Click(object sender, RoutedEventArgs e)
        {
            textBox_search.Text = "";
        }

        private void textBox_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!DatabaseManager.IsOpened)
                return;

            if (string.IsNullOrEmpty(textBox_search.Text))
                UpdateResourceList();
            else
            {
                var results = DatabaseManager.Resource_Search(textBox_search.Text);
                UpdateResourceList(results);
            }
        }

        private void treeView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is TextBlock)
            {
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    if ((e.OriginalSource as TextBlock).DataContext is Resource)
                    {
                        e.Handled = true;
                        var r = (e.OriginalSource as TextBlock).DataContext as Resource;
                        Editor_OnPinClicked(r);
                    }
                }
            }
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeView.SelectedItem != null)
            {
                var r = (treeView.SelectedItem as TreeViewItem).Header as Resource;
                ChangeResource(r, true);
            }
        }
    }
}
