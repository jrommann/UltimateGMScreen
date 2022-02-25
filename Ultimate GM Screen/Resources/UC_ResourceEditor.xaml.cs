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

namespace Ultimate_GM_Screen.Resources
{
    /// <summary>
    /// Interaction logic for UC_ResourceEditor.xaml
    /// </summary>
    public partial class UC_ResourceEditor : UserControl
    {
        Resource _current = new Resource();
        bool _edit = false;
        public UC_ResourceEditor()
        {
            InitializeComponent();
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

            textbox_path.Text = _current.Path;
            textbox_name.Text = _current.Name;
            textbox_address.Text = _current.Address;

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

            _current.Path = textbox_path.Text;
            _current.Name = textbox_name.Text;
            _current.Address = textbox_address.Text;

            if (_edit)
                DatabaseManager.Update(_current);
            else
                DatabaseManager.Add(_current);

            _edit = true;
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
        private void popoutBtn_Click(object sender, RoutedEventArgs e)
        {
            var w = new Window_Resource();
            w.Load(_current);
            w.Show();
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
    }
}
