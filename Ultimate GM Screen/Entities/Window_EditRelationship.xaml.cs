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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ultimate_GM_Screen.Entities
{
    /// <summary>
    /// Interaction logic for UC_EditRelationship.xaml
    /// </summary>
    public partial class Window_EditRelationship : AdonisWindow
    {
        Entity _owner;
        public EntityRelationship Relationship { get; private set; }
        bool _isEdit = false;

        public Window_EditRelationship()
        {
            InitializeComponent();
            LoadNotes();            
        }

        void LoadNotes(List<NoteListing> list = null)
        {
            if (list == null)
                list = DatabaseManager.Entities_GetAll_Listing();

            list.Sort((x, y) => x.FolderPath.CompareTo(y.FolderPath));
            listBox_choices.ItemsSource = list;
        }

        public void Load(Entity parent, EntityRelationship rel, bool isEdit)
        {
            _owner = parent;
            _isEdit = isEdit;
            if (rel == null)
            {
                Relationship = new EntityRelationship();
                Relationship.ParentID = _owner.ID;
            }
            else
                Relationship = rel;

            textBox_description.Text = Relationship.Description;
            listBox_choices.SelectedItem = Relationship.Child;
        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            Relationship.Description = textBox_description.Text;
            if(listBox_choices.SelectedItem != null)
                Relationship.ChildID = (listBox_choices.SelectedItem as NoteListing).ID;
            
            if (_isEdit)
                DatabaseManager.Update(Relationship, false);
            else
                DatabaseManager.Add(Relationship);

            DialogResult = true;
        }

        private void button_cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void textBox_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!DatabaseManager.IsOpened)
                return;

            if (string.IsNullOrEmpty(textBox_search.Text))
                LoadNotes();
            else
            {
                var notes = DatabaseManager.Entities_Search_Listing(textBox_search.Text);
                LoadNotes(notes);
            }
        }

        private void btn_ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            textBox_search.Text = "";
        }
    }
}
