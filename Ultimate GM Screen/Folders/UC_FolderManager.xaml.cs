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
        public UC_FolderManager()
        {
            InitializeComponent();

            if (DatabaseManager.IsOpened)
            {
                Note_UpdateParentList();
                Note_UpdateTreeView();
            }
        }

        void Note_UpdateParentList()
        {
            var list = DatabaseManager.Folders_GetAll(FolderType.Note);
            list.Insert(0, new FolderEntry() { ID = -1, Name = "None" });
            comboBox_notes.ItemsSource = list;
            comboBox_notes.SelectedIndex = 0;
            //comboBox_notes.SelectedItem = comboBox_notes.Items[0];
        }

        void Note_UpdateTreeView()
        {
            treeView_notes.Items.Clear();

            var list = DatabaseManager.Folders_GetAll(FolderType.Note);
            list.Sort((x,y) => x.ParentID.CompareTo(y.ParentID));
                        
            List<TreeViewItem> items = new List<TreeViewItem>();

            foreach (var f in list)
            {
                if (f.ParentID == -1)
                {
                    TreeViewItem i = new TreeViewItem();
                    i.Header = f;
                    i.IsExpanded = true;                    
                    items.Add(i);

                    treeView_notes.Items.Add(i);
                }
                else
                {
                    var treeviewItem = items.Find(x => (x.Header as FolderEntry).ID == f.ParentID);

                    if (treeviewItem != null)
                    {
                        TreeViewItem i = new TreeViewItem();
                        i.Header = f;
                        i.IsExpanded = true;
                        items.Add(i);

                        treeviewItem.Items.Add(i);
                    }
                }
            }
        }

        private void button_notes_Click(object sender, RoutedEventArgs e)
        {
            var fe = new FolderEntry();
            fe.Name = textBox_notes.Text;
            fe.ParentID = (comboBox_notes.SelectedItem as FolderEntry).ID;
            fe.Type = FolderType.Note;

            DatabaseManager.Add(fe);

            Note_UpdateParentList();
            Note_UpdateTreeView();
        }
    }
}
