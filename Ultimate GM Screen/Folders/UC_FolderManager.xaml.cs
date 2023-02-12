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

namespace Ultimate_GM_Screen.Folders
{
    /// <summary>
    /// Interaction logic for UC_FolderManager.xaml
    /// </summary>
    public partial class UC_FolderManager : UserControl
    {
        FolderEntry _selectedEntry = null;

        public UC_FolderManager()
        {
            InitializeComponent();

            if (DatabaseManager.IsOpened)
            {
                Note_UpdateParentList();
                Note_UpdateTreeView();

                Resources_UpdateParentList();
                Resources_UpdateTreeView();
            }
        }

        #region -> notes
        void Note_UpdateParentList()
        {
            var list = DatabaseManager.Folders_GetAll(FolderType.Note);
            list.Insert(0, new FolderEntry() { ID = -1, Name = "None" });
            comboBox_notes.ItemsSource = list;
            comboBox_notes.SelectedIndex = 0;
        }

        void Note_UpdateTreeView()
        {
            treeView_notes.Items.Clear();

            var list = DatabaseManager.Folders_GetAll(FolderType.Note);
            list.Sort((x,y) => x.ParentID.CompareTo(y.ParentID));
                        
            List<TreeViewItem> items = new List<TreeViewItem>();
            foreach (var f in list)
            {
                TreeViewItem i = new TreeViewItem();
                i.Header = f;
                i.IsExpanded = true;
                items.Add(i);
            }

            foreach (var i in items)
            {
                var f = i.Header as Folders.FolderEntry;
                if (f.ParentID == -1)
                {
                    treeView_notes.Items.Add(i);
                }
                else
                {
                    var treeviewItem = items.Find(x => (x.Header as FolderEntry).ID == f.ParentID);
                    if (treeviewItem != null)
                        treeviewItem.Items.Add(i);
                }
            }
        }

        private void button_notesAdd_Click(object sender, RoutedEventArgs e)
        {
            var fe = new FolderEntry();
            fe.Name = textBox_notes.Text;
            fe.ParentID = (comboBox_notes.SelectedItem as FolderEntry).ID;
            fe.Type = FolderType.Note;

            DatabaseManager.Add(fe);

            Note_UpdateParentList();
            Note_UpdateTreeView();
        }

        private void button_notesUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEntry != null)
            {
                _selectedEntry.Name = textBox_notes.Text;
                _selectedEntry.ParentID = (comboBox_notes.SelectedItem as FolderEntry).ID;

                DatabaseManager.Update(_selectedEntry, true);

                Note_UpdateParentList();
                Note_UpdateTreeView();
            }
        }

        private void button_notesDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEntry != null)
            {
                DatabaseManager.Delete(_selectedEntry);
                _selectedEntry = null;

                textBox_notes.Text = "";
                comboBox_notes.SelectedIndex = 0;

                Note_UpdateParentList();
                Note_UpdateTreeView();
            }
        }

        private void treeView_notes_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeView_notes.SelectedItem != null)
            {
                _selectedEntry = (treeView_notes.SelectedItem as TreeViewItem).Header as FolderEntry;
                if (_selectedEntry != null)
                {
                    textBox_notes.Text = _selectedEntry.Name;
                    comboBox_notes.SelectedItem = comboBox_notes.Items.Cast<FolderEntry>().ToList().Find(x => x.ID == _selectedEntry.ParentID);
                }
            }
        }
        #endregion
        #region -> resources
        void Resources_UpdateParentList()
        {
            var list = DatabaseManager.Folders_GetAll(FolderType.Resource);
            list.Insert(0, new FolderEntry() { ID = -1, Name = "None" });
            comboBox_resources.ItemsSource = list;
            comboBox_resources.SelectedIndex = 0;
        }

        void Resources_UpdateTreeView()
        {
            treeView_resources.Items.Clear();

            var list = DatabaseManager.Folders_GetAll(FolderType.Resource);
            list.Sort((x, y) => x.ParentID.CompareTo(y.ParentID));

            List<TreeViewItem> items = new List<TreeViewItem>();
            foreach (var f in list)
            {
                TreeViewItem i = new TreeViewItem();
                i.Header = f;
                i.IsExpanded = true;
                items.Add(i);
            }

            foreach (var i in items)
            {
                var f = i.Header as Folders.FolderEntry;
                if (f.ParentID == -1)
                {
                    treeView_resources.Items.Add(i);
                }
                else
                {
                    var treeviewItem = items.Find(x => (x.Header as FolderEntry).ID == f.ParentID);
                    if (treeviewItem != null)
                        treeviewItem.Items.Add(i);
                }
            }
        }

        private void treeView_resources_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeView_resources.SelectedItem != null)
            {
                _selectedEntry = (treeView_resources.SelectedItem as TreeViewItem).Header as FolderEntry;
                if (_selectedEntry != null)
                {
                    textBox_resources.Text = _selectedEntry.Name;
                    comboBox_resources.SelectedItem = comboBox_resources.Items.Cast<FolderEntry>().ToList().Find(x => x.ID == _selectedEntry.ParentID);
                }
            }
        }

        private void button_resourcesAdd_Click(object sender, RoutedEventArgs e)
        {
            var fe = new FolderEntry();
            fe.Name = textBox_resources.Text;
            fe.ParentID = (comboBox_resources.SelectedItem as FolderEntry).ID;
            fe.Type = FolderType.Resource;

            DatabaseManager.Add(fe);

            Resources_UpdateParentList();
            Resources_UpdateTreeView();
        }

        private void button_resourcesUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEntry != null)
            {
                _selectedEntry.Name = textBox_resources.Text;
                _selectedEntry.ParentID = (comboBox_resources.SelectedItem as FolderEntry).ID;

                DatabaseManager.Update(_selectedEntry, true);

                Resources_UpdateParentList();
                Resources_UpdateTreeView();
            }
        }

        private void button_resourcesDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEntry != null)
            {
                DatabaseManager.Delete(_selectedEntry);
                _selectedEntry = null;

                textBox_resources.Text = "";
                comboBox_resources.SelectedIndex = 0;

                Resources_UpdateParentList();
                Resources_UpdateTreeView();
            }
        }
        #endregion
    }
}
