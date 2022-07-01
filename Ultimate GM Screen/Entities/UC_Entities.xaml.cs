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
        Entity _currentEntity;
        
        List<UC_EntityEditor> _pinnedEditors = new List<UC_EntityEditor>();
        List<Button> _pinnedButtons = new List<Button>();
        ObservableCollection<Entity> _noteHistory = new ObservableCollection<Entity>();       

        public UC_Entities()
        {
            InitializeComponent();

            DatabaseManager.OnEntitiesChanged += DatabaseManager_OnEntitiesChanged;
            noteEditor.OnPinClicked += NoteEditor_OnPinClicked;

            comboBox_history.ItemsSource = _noteHistory;
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
            AddToHistory(noteEditor.Current);   

            noteEditor.Load(ent, edit);
            _currentEntity = ent;

            Switch_Displayed_Note(noteEditor);
        }

        private void treeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = Common.VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                e.Handled = true;
                if (treeViewItem.Header is Entity)
                {
                    var win = new Window_Entity();                   
                    win.Load(treeViewItem.Header as Entity);                    
                    win.ShowInTaskbar = true;
                    win.Owner = this.Parent as Window;
                    win.ShowActivated = true;
                    win.Show();
                }
            }
        }

        private void textBox_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!DatabaseManager.IsOpened)
                return;

            if (string.IsNullOrEmpty(textBox_search.Text))
                LoadTreeView(DatabaseManager.Entities_GetAll());
            else
            {
                var notes = DatabaseManager.Entities_FindByName(textBox_search.Text);
                LoadTreeView(notes);
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
    }
}
