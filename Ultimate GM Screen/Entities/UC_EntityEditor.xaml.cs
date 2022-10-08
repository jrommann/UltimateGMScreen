using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Markdig.Wpf;

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
            //InitializeAsync();

            DatabaseManager.OnRelationshipsChanged += DatabaseManager_OnRelationshipsChanged;            
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

            textBox_path.Text = _current.Path;
            textBox_name.Text = _current.Name;
            textBox_tags.Text = _current.Tags;
            SetBrowserText(_current.Details);

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
            string path = textBox_path.Text;
            Load();

            textBox_name.Text = "Note " + DatabaseManager.Entity_Count();

            if (checkBox_keepPath.IsChecked.Value)
                textBox_path.Text = path;           
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(textBox_name.Text))
                return;

            bool save = false;
            var revision = new EntityRevision(_current);

            if (_current.Path != textBox_path.Text)
            {
                save = true;
                _current.Path = textBox_path.Text;
            }

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

            if (save)
            {
                if (_edit)
                {
                    DatabaseManager.Add(revision);
                    DatabaseManager.Update(_current);
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

        void SetBrowserText(string text)
        {

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
    }
}
