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
using System.Collections.ObjectModel;

namespace Ultimate_GM_Screen.Entities
{
    /// <summary>
    /// Interaction logic for UC_Entities.xaml
    /// </summary>
    public partial class UC_Entities : UserControl
    {
        public static UC_Entities Notes { get; private set; }

        Entity _currentEntity;
        
        List<UC_EntityEditor> _pinnedEditors = new List<UC_EntityEditor>();
        List<Button> _pinnedButtons = new List<Button>();
        ObservableCollection<Entity> _noteHistory = new ObservableCollection<Entity>();
        UC_EntityEditor _visibleEditor;

        public UC_Entities()
        {
            InitializeComponent();

            DatabaseManager.OnEntitiesChanged += DatabaseManager_OnEntitiesChanged;
            noteEditor.OnPinClicked += NoteEditor_OnPinClicked;

            comboBox_history.ItemsSource = _noteHistory;

            Notes = this;
        }

        private void NoteEditor_OnPinClicked(Entity note = null)
        {
            if (note == null)
                return;

            var editor = new UC_EntityEditor();            
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
            noteEditor.Load();
            Switch_Displayed_Note(editor);
        }

        private void Editor_OnUnpinClicked(Entity item = null)
        {
            var btn = _pinnedButtons.Find(x => x.Tag == item);
            var editor = _pinnedEditors.Find(x => x.Current == item);
            _pinnedButtons.Remove(btn);
            _pinnedEditors.Remove(editor);

            stackpanel_pinnedNotes.Children.Remove(btn);
            notesGrid.Children.Remove(editor);

            Switch_Displayed_Note(noteEditor);
        }

        private void DatabaseManager_OnEntitiesChanged(Entity specificItem = null, bool pathChanged = false)
        {
            if (pathChanged)
                UpdateNotesList();
        }

        private void button_delete_Click(object sender, RoutedEventArgs e)
        {            
            if(_currentEntity != null)
                DatabaseManager.Delete(_currentEntity);
        }

        void UpdateNotesList(List<Entity> notes=null)
        {
            if (notes != null)
                listBoxNotes.ItemsSource = notes;
            else
                listBoxNotes.ItemsSource = DatabaseManager.Entities_GetAll();
        }        

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            noteEditor.Load();
            UpdateNotesList();
            Loaded -= UserControl_Loaded;
        }

        public void ChangeNote(Entity ent, bool edit)
        {            
            noteEditor.Save();
            AddToHistory(noteEditor.Current);   

            noteEditor.Load(ent, edit);
            _currentEntity = ent;

            Switch_Displayed_Note(noteEditor);
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

        private void btn_Current_Click(object sender, RoutedEventArgs e)
        {
            Switch_Displayed_Note(noteEditor);
        }

        void Switch_Displayed_Note(UC_EntityEditor show)
        {
            noteEditor.Visibility = Visibility.Hidden;
            _pinnedEditors.ForEach(x => x.Visibility = Visibility.Hidden);

            show.Visibility = Visibility.Visible;
            _visibleEditor = show;
        }

        private void comboBox_history_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_history.SelectedItem is Entity)
            {
                Switch_Displayed_Note(noteEditor);                     
                
                ChangeNote(comboBox_history.SelectedItem as Entity, true);
                comboBox_history.SelectedItem = null;
            }
        }

        void AddToHistory(Entity note)
        {
            _noteHistory.Remove(note);            
            _noteHistory.Insert(0, note);            
        }

        private void button_clearSearch_Click(object sender, RoutedEventArgs e)
        {
            textBox_search.Text = "";
        }

        private void listBoxNotes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(listBoxNotes.SelectedItem is Entity)
                ChangeNote(listBoxNotes.SelectedItem as Entity, true);
        }

        private void listBoxNotes_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is TextBlock)
            {
                if (e.MiddleButton == MouseButtonState.Pressed)
                {

                    if ((e.OriginalSource as TextBlock).DataContext is Entity)
                    {
                        e.Handled = true;

                        var note = (e.OriginalSource as TextBlock).DataContext as Entity;
                        if (_visibleEditor != null)
                            _visibleEditor.InsertLink(note);
                        else
                            noteEditor.InsertLink(note);
                    }

                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    if ((e.OriginalSource as TextBlock).DataContext is Entity)
                    {
                        e.Handled = true;

                        var note = (e.OriginalSource as TextBlock).DataContext as Entity;
                        NoteEditor_OnPinClicked(note);
                        //var win = new Window_Entity();
                        //win.Load(note);
                        //win.ShowInTaskbar = true;
                        //win.Owner = this.Parent as Window;
                        //win.ShowActivated = true;
                        //win.Show();
                    }
                }
            }
        }
    }
}
