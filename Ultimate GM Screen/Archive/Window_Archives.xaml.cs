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
using Ultimate_GM_Screen.Entities;

namespace Ultimate_GM_Screen.Archive
{
    /// <summary>
    /// Interaction logic for Window_Archives.xaml
    /// </summary>
    public partial class Window_Archives : AdonisWindow
    {
        Entity _currentEntity;

        public Window_Archives()
        {
            InitializeComponent();

            DatabaseManager.OnEntitiesChanged += DatabaseManager_OnEntitiesChanged;            
        }

        private void DatabaseManager_OnEntitiesChanged(Entity specificItem, bool pathChanged)
        {
            if (pathChanged)
                UpdateNotesList();
        }

        private void button_restore_Click(object sender, RoutedEventArgs e)
        {
            if (_currentEntity != null)
            {
                _currentEntity.Archived = false;
                DatabaseManager.Update(_currentEntity, true);
            }
        }

        private void button_delete_Click(object sender, RoutedEventArgs e)
        {
            if (_currentEntity != null)
                DatabaseManager.Delete(_currentEntity);
        }

        private void textBox_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!DatabaseManager.IsOpened)
                return;

            if (string.IsNullOrEmpty(textBox_search.Text))
                UpdateNotesList();
            else
            {
                var notes = DatabaseManager.Entities_Search(textBox_search.Text);
                UpdateNotesList(notes);
            }
        }
        private void button_clearSearch_Click(object sender, RoutedEventArgs e)
        {
            textBox_search.Text = "";
        }
        private void listBoxNotes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxNotes.SelectedItem is Entity)
                Viewer.Markdown = (listBoxNotes.SelectedItem as Entity).Details;
        }

        void UpdateNotesList(List<Entity> notes = null)
        {
            if (notes != null)
                listBoxNotes.ItemsSource = notes;
            else
                listBoxNotes.ItemsSource = DatabaseManager.Entities_GetAll_Archieved();
        }

        
    }
}
