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
            //if (pathChanged)
                UpdateNotesList();
        }

        private void DatabaseManager_OnFoldersChanged()
        {            
            UpdateNotesList();
        }

        private void button_delete_Click(object sender, RoutedEventArgs e)
        {
            if (_currentEntity != null)
            {
                _currentEntity.Archived = true;
                DatabaseManager.Update(_currentEntity, true);
                noteEditor.Load();
            }
        }

        void UpdateNotesList(List<Entity> notes=null)
        {
            #region -> full list
            if (notes == null)
            {
                #region -> build tree
                var folders = DatabaseManager.Folders_GetAll(Folders.FolderType.Note);
                folders.Sort((x, y) => x.ParentID.CompareTo(y.ParentID));

                Dictionary<int, TreeViewItem> treeFolders = new Dictionary<int, TreeViewItem>();
                treeView.Items.Clear();

                foreach (var f in folders)
                {
                    TreeViewItem tvi = new TreeViewItem();
                    tvi.Header = f;
                    tvi.IsExpanded = f.IsExpanded;

                    #region -> save expanded / collasped
                    //tvi.Expanded += (sender, e) => 
                    //{
                    //    var fe = (sender as TreeViewItem).Header as Folders.FolderEntry;
                    //    fe.IsExpanded = true;
                    //    DatabaseManager.Update(fe, false);
                    //};
                    //tvi.Collapsed += (sender, e) =>
                    //{
                    //    var fe = (sender as TreeViewItem).Header as Folders.FolderEntry;
                    //    fe.IsExpanded = false;
                    //    DatabaseManager.Update(fe, false);
                    //};
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
                    notes = DatabaseManager.Entities_GetAll();
                                
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DatabaseManager.IsOpened)
            {
                DatabaseManager.OnEntitiesChanged += DatabaseManager_OnEntitiesChanged;
                DatabaseManager.OnFoldersChanged += DatabaseManager_OnFoldersChanged;

                noteEditor.OnPinClicked += NoteEditor_OnPinClicked;

                comboBox_history.ItemsSource = _noteHistory;

                Notes = this;
            
                noteEditor.Load();
                UpdateNotesList();             
            }

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

        private void treeView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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
                    }
                }
            }
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {            
            if (treeView.SelectedItem != null)
            {
                var note = (treeView.SelectedItem as TreeViewItem).Header as Entity;
                if (note != null)
                    ChangeNote(note, true);
            }
        }
    }
}
