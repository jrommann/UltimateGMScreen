using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Markdig.Wpf;
using Ultimate_GM_Screen.Folders;

namespace Ultimate_GM_Screen.Entities
{
    /// <summary>
    /// Interaction logic for UC_EntityEditor.xaml
    /// </summary>
    public partial class UC_EntityEditor : UserControl
    {
        public delegate void Event_PinClicked(Entity item = null);
        public event Event_PinClicked OnPinClicked;
        public event Event_PinClicked OnUnpinClicked;

        bool _canPopout = true;
        public bool CanPopout { get { return _canPopout; } set { _canPopout = value; popoutBtn.Visibility = (value ? Visibility.Visible : Visibility.Collapsed); } }
        bool _canPin = true;
        public bool CanPin { get { return _canPin; } set { _canPin = value; pinBtn.Visibility = (value ? Visibility.Visible : Visibility.Collapsed); } }

        bool _pinned = false;
        public bool Pinned { get { return _pinned; } set { _pinned = value; pinBtn.Content = (value ? "Unpin" : "Pin"); } }

        public static RoutedCommand SaveCommand = new RoutedCommand();
        Entity _current = new Entity();
        public Entity Current { get { return _current; } }

        bool _edit = false;        

        public UC_EntityEditor()
        {
            SaveCommand.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));

            InitializeComponent();

            DatabaseManager.OnRelationshipsChanged += DatabaseManager_OnRelationshipsChanged;
            DatabaseManager.OnFoldersChanged += DatabaseManager_OnFoldersChanged;            
        }

        private void DatabaseManager_OnFoldersChanged()
        {
            var list = DatabaseManager.Folders_GetAll(FolderType.Note);            
            list.Sort((x, y) => x.Fullpath.CompareTo(y.Fullpath));
            list.Insert(0, new FolderEntry() { ID = FolderEntry.NO_PARENT_FOLDER, Name = "None" });
            comboBox_parent.ItemsSource = list;           

            if (_current != null)
            {
                if (_current.FolderID != FolderEntry.NO_PARENT_FOLDER)
                    comboBox_parent.SelectedIndex = comboBox_parent.Items.Cast<FolderEntry>().ToList().FindIndex(x => x.ID == _current.FolderID);
                else
                    comboBox_parent.SelectedIndex = 0;
            }
            else
                comboBox_parent.SelectedIndex = 0;
        }

        private void DatabaseManager_OnRelationshipsChanged(EntityRelationship specificItem = null)
        {
            dockpanel_relationships.Children.Clear();
            var rList = DatabaseManager.EntityRelationship_GetAll(_current.ID);
            foreach (var r in rList)
                Relationship_Add(r);
        }

        public void Load(Entity current=null, bool edit=false)
          {            
                           _edit = edit;
            if (current == null)
                _current = new Entity();
            else
                _current = current;

            textBox_name.Text = _current.Name;
            textBox_tags.Text = _current.Tags;
            SetBrowserText(_current.Details);            

            if (_current.FolderID != -1)
                comboBox_parent.SelectedIndex = comboBox_parent.Items.Cast<FolderEntry>().ToList().FindIndex(x => x.ID == _current.FolderID);
            else
                comboBox_parent.SelectedIndex = 0;

            dockpanel_relationships.Children.Clear();
            try
            {
                var rList = DatabaseManager.EntityRelationship_GetAll(_current.ID);
                foreach (var r in rList)
                    Relationship_Add(r);
            }
            catch { }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {            
            Load();

            textBox_name.Text = "New Note";            
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(textBox_name.Text))
                return;

            bool save = false;
            bool pathNameChanged = false;
            var revision = new EntityRevision(_current);            

            if (_current.Name != textBox_name.Text)
            {
                save = true;
                _current.Name = textBox_name.Text;
            }

            if (_current.Tags != textBox_tags.Text)
            {
                save = true;
                _current.Tags = textBox_tags.Text;
            }

            string text = markdownEditor.GetMarkdown();
            if (_current.Details != text)
            {
                save = true;
                _current.Details = text;
            }
                        
            int parentID = (comboBox_parent.SelectedItem as FolderEntry).ID;
            if (_current.FolderID != parentID)
            {
                save = true;
                _current.FolderID = parentID;
                pathNameChanged = true;
            }
            

            if (save)
            {
                if (_edit)
                {
                    DatabaseManager.Add(revision);
                    DatabaseManager.Update(_current, pathNameChanged);
                }
                else
                    DatabaseManager.Add(_current);

                _edit = true;
            }
        }

        private void copyBtn_Click(object sender, RoutedEventArgs e)
        {            
            _current = new Entity();
            _edit = false;
            textBox_name.Text += "(Copy)";

            Save();
            Load(_current, true);            
        }

        public void InsertLink(Entity note)
        {
            markdownEditor.InsertLink(note);
        }

        void SetBrowserText(string text)
        {
            //if (!String.IsNullOrEmpty(text))
            //    try { text = System.Text.RegularExpressions.Regex.Unescape(text); } catch{ }

            markdownEditor.SetMarkdown(text);
        }

        private void button_addRelationship_Click(object sender, RoutedEventArgs e)
        {
            var w = new Window_EditRelationship();
            w.Load(_current, null, false);
            w.ShowDialog();
        }

        void Relationship_Add(EntityRelationship rel)
        {
            var r = new UC_EntityRelationship();
            r.Load(rel);
            DockPanel.SetDock(r, Dock.Top);
            dockpanel_relationships.Children.Add(r);
        }

        private void SaveCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Save();
        }

        private void revisionsBtn_Click(object sender, RoutedEventArgs e)
        {            
            var w = new Window_EntityRevisions();
            w.Current = _current;
            w.ShowDialog();

            Load(DatabaseManager.Entity_FromID(_current.ID), true);
        }

        private void pinBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CanPin)
            {
                if (_pinned)
                    OnUnpinClicked?.Invoke(_current);
                else
                    OnPinClicked?.Invoke(_current);       
            }
        }

        private void popoutBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CanPopout && _current != null)
            {
                var win = new Window_Entity();
                win.Load(_current);
                win.ShowInTaskbar = true;
                win.Owner = this.Parent as Window;
                win.ShowActivated = true;
                win.Show();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DatabaseManager.IsOpened)
                DatabaseManager_OnFoldersChanged();
        }
    }
}
