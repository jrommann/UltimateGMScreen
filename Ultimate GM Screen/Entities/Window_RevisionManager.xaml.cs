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

namespace Ultimate_GM_Screen
{
    /// <summary>
    /// Interaction logic for Window_RevisionManager.xaml
    /// </summary>
    public partial class Window_RevisionManager : AdonisWindow
    {
        public Window_RevisionManager()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadNotesandRevisions();
        }

        private void LoadNotesandRevisions()
        {
            treeView.Items.Clear();

            var notes = DatabaseManager.Entities_GetAll();
            notes.AddRange(DatabaseManager.Entities_GetAll_Archieved());

            foreach (var n in notes)
            {
                var t = new TreeViewItem();
                t.Header = n;
                treeView.Items.Add(t);

                var revisions = DatabaseManager.Entity_GetRevisions(n.ID, 1, 0);
                foreach (var r in revisions)
                    t.Items.Add(r);
            }
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeView.SelectedItem is Entities.EntityRevision)
            {
                notePanel.Visibility = Visibility.Hidden;
                revision.Visibility = Visibility.Visible;

                var r = treeView.SelectedItem as Entities.EntityRevision;
                revision.Load(r, true);
            }
            else if (treeView.SelectedItem is TreeViewItem)
            {
                notePanel.Visibility = Visibility.Visible;
                revision.Visibility = Visibility.Hidden;

                var note = (treeView.SelectedItem as TreeViewItem).Header as Entities.Entity;
                if (note != null)
                    noteLbl.Content = note.ToString();
            }
        }

        private void keepLast_Click(object sender, RoutedEventArgs e)
        {            
            if(treeView.SelectedItem is TreeViewItem)
            {
                var ti = treeView.SelectedItem as TreeViewItem;
                int count = ti.Items.Count;
                if (count > 1)
                {
                    for (int i = count - 1; i > 0; i--)
                    {
                        var r = ti.Items[i] as Entities.EntityRevision;
                        DatabaseManager.Delete(r);
                    }
                    DatabaseManager.Vacuum();
                    LoadNotesandRevisions();
                }
            }
        }

        private void deletAllNotesRevisions_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void deletAllRevisions_Click(object sender, RoutedEventArgs e)
        {
            var notes = treeView.Items;
            foreach (TreeViewItem ti in notes)
            {
                int count = ti.Items.Count;
                int toKeep = revisionsToKeep.Value.Value;
                if (count > toKeep)
                {
                    for (int i = count - 1; i > toKeep; i--)
                    {
                        var r = ti.Items[i] as Entities.EntityRevision;
                        DatabaseManager.Delete(r);
                    }
                }
            }

            DatabaseManager.Vacuum();
            LoadNotesandRevisions();
        }

        private void deletNoteRevisions_Click(object sender, RoutedEventArgs e)
        {
            if (treeView.SelectedItem is TreeViewItem)
            {
                var ti = treeView.SelectedItem as TreeViewItem;
                int count = ti.Items.Count;
                int toKeep = noteRevisionsToKeep.Value.Value;
                if (count > toKeep)
                {
                    for (int i = count - 1; i > toKeep; i--)
                    {
                        var r = ti.Items[i] as Entities.EntityRevision;
                        DatabaseManager.Delete(r);
                    }
                    DatabaseManager.Vacuum();
                    LoadNotesandRevisions();
                }
            }

            DatabaseManager.Vacuum();
            LoadNotesandRevisions();
        }
    }
}
