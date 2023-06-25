using Microsoft.Web.WebView2.Core;
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
using Ultimate_GM_Screen.Folders;

namespace Ultimate_GM_Screen.Resources
{
    /// <summary>
    /// Interaction logic for UC_ResourceEditor.xaml
    /// </summary>
    public partial class UC_ResourceEditor : UserControl
    {
        public delegate void Event_PinClicked(Resource item = null);
        public event Event_PinClicked OnPinClicked;
        public event Event_PinClicked OnUnpinClicked;

        Resource _current = new Resource();
        public Resource Current { get { return _current; } }

        bool _edit = false;
     
        bool _canPopout = true;
        public bool CanPopout { get { return _canPopout; } set { _canPopout = value; popoutBtn.Visibility = (value ? Visibility.Visible : Visibility.Collapsed); } }
        bool _canPin = true;
        public bool CanPin { get { return _canPin; } set { _canPin = value; pinBtn.Visibility = (value ? Visibility.Visible : Visibility.Collapsed); } }

        bool _pinned = false;
        public bool Pinned { get { return _pinned; } set { _pinned = value; pinBtn.Content = (value ? "Unpin" : "Pin"); } }
        public UC_ResourceEditor()
        {
            InitializeComponent();
            InitializeAsync();

            DatabaseManager.OnFoldersChanged += DatabaseManager_OnFoldersChanged;            
        }

        async void InitializeAsync()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;

            var browserEnviorment = await CoreWebView2Environment.CreateAsync(null, Common.UserDataFolder);
            await browser.EnsureCoreWebView2Async(browserEnviorment);

            browser.DefaultBackgroundColor = System.Drawing.Color.FromArgb(61, 61, 76);            
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            DatabaseManager_OnFoldersChanged();
        }

        private void DatabaseManager_OnFoldersChanged()
        {
            if (DatabaseManager.IsOpened)
            {
                var list = DatabaseManager.Folders_GetAll(FolderType.Resource);
                list.Insert(0, new FolderEntry() { ID = FolderEntry.NO_PARENT_FOLDER, Name = "None" });
                list.Sort((x, y) => x.Fullpath.CompareTo(y.Fullpath));

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
        }

        public void Load(Resource current = null, bool edit = false)
        {
            _edit = edit;
            if (current == null)
            {
                _current = new Resource();
                _current.Name = "Resource " + DatabaseManager.Entity_Count();
            }
            else
                _current = current;

            if (string.IsNullOrEmpty(_current.Path))
            {
                textbox_path.Visibility = Visibility.Hidden;
                label_path.Visibility = Visibility.Hidden;
            }
            else
            {
                textbox_path.Visibility = Visibility.Visible;
                label_path.Visibility = Visibility.Visible;
                textbox_path.Text = _current.Path;
            }

            textbox_name.Text = _current.Name;
            textbox_address.Text = _current.Address;

            if (_current.FolderID != -1)
                comboBox_parent.SelectedItem = comboBox_parent.Items.Cast<FolderEntry>().ToList().Find(x => x.ID == _current.FolderID);
            else
                comboBox_parent.SelectedIndex = 0;

            button_go_Click(this, null);
        }

        public void New()
        {
            string path = textbox_path.Text;
            Uri uri = browser.Source;
            Load();

            textbox_name.Text = "Resource " + DatabaseManager.Resource_Count();
            if(uri != null)
                textbox_address.Text = uri.ToString();

            if (checkBox_keepPath.IsChecked.Value)
                textbox_path.Text = path;
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(textbox_name.Text))
                return;

            bool save = false;
            bool folderChanged = false;

            if (_current.Path != textbox_path.Text)
            {
                _current.Path = textbox_path.Text;
                save = true;
            }
            if (_current.Name != textbox_name.Text)
            {
                _current.Name = textbox_name.Text;
                save = true;
            }
            if (_current.Address != textbox_address.Text)
            {
                _current.Address = textbox_address.Text;
                save = true;
            }

            int parentID = FolderEntry.NO_PARENT_FOLDER;
            var fd = comboBox_parent.SelectedItem;
            if (fd != null)
                parentID = (fd as FolderEntry).ID;
            
            if (_current.FolderID != parentID)
            {
                save = true;
                _current.FolderID = parentID;
                folderChanged = true;
            }            

            if (save)
            {
                if (_edit)
                    DatabaseManager.Update(_current, folderChanged);
                else
                    DatabaseManager.Add(_current);

                _edit = true;
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void copyBtn_Click(object sender, RoutedEventArgs e)
        {
            _current = new Resource();
            _edit = false;
            textbox_name.Text += "(Copy)";
            Save();
            Load(_current, true);
        }

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            New();
        }
        private void button_go_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(textbox_address.Text))
                try { browser.Source = new Uri(textbox_address.Text); } catch { }
        }

        private void button_browse_Click(object sender, RoutedEventArgs e)
        {
            #region -> open
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            //dlg.DefaultExt = "*.*";
            //dlg.Filter = "Ultimate GM Screen (*.ugs)|*.ugs";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                textbox_address.Text = dlg.FileName;
            }
            #endregion
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
                var win = new Window_Resource();
                win.Load(_current);
                win.ShowInTaskbar = true;
                win.Owner = this.Parent as Window;
                win.ShowActivated = true;
                win.Show();
            }
        }        
    }
}
