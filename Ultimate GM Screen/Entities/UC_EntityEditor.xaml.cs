using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Ultimate_GM_Screen.Entities
{
    /// <summary>
    /// Interaction logic for UC_EntityEditor.xaml
    /// </summary>
    public partial class UC_EntityEditor : UserControl
    {
        public static RoutedCommand SaveCommand = new RoutedCommand();
        Entity _current = new Entity();
        public Entity Current { get { return _current; } }

        bool _edit = false;
        bool _webInit = false;
        bool _saving = false;
        
        public UC_EntityEditor()
        {
            SaveCommand.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));

            InitializeComponent();
            InitializeAsync();

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

        async private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            await Save();
        }

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            string path = textBox_path.Text;
            Load();

            textBox_name.Text = "Note " + DatabaseManager.Entity_Count();

            if (checkBox_keepPath.IsChecked.Value)
                textBox_path.Text = path;           
        }

        public async Task Save()
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

            string text = await GetBrowserText();
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

        async private void copyBtn_Click(object sender, RoutedEventArgs e)
        {            
            _current = new Entity();
            _edit = false;
            textBox_name.Text += "(Copy)";

            await Save();
            Load(_current, true);            
        }

        private void WebView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (_webInit)
                SetBrowserText(_current.Details);
        }

        async void InitializeAsync()
        {
            await webView.EnsureCoreWebView2Async(null);
            _webInit = true;

            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tinymce\\index.html");
            webView.Source = new Uri("file:///" + path);
            webView.DefaultBackgroundColor = System.Drawing.Color.FromArgb(61, 61, 76);
            webView.NavigationCompleted += WebView_NavigationCompleted;
        }

        void SetBrowserText(string text)
        {
            if (_webInit)
                webView.ExecuteScriptAsync(string.Format("set(\"{0}\")", text));    
        }

        async Task<string> GetBrowserText()
        {
            string result = await webView.ExecuteScriptAsync(@"save()");
            result = result.Remove(0, 1).Remove(result.Length - 2, 1);
            return result;
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

        private void webView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_webInit)
            {               
                try { webView.Reload(); } catch { }
            }
        }        

        async private void SaveCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            await Save();
        }

        private void revisionsBtn_Click(object sender, RoutedEventArgs e)
        {            
            var w = new Window_EntityRevisions();
            w.Current = _current;
            w.ShowDialog();

            Load(DatabaseManager.Entity_FromID(_current.ID), true);
        }
    }
}
