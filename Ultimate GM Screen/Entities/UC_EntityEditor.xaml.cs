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
    /// Interaction logic for UC_EntityEditor.xaml
    /// </summary>
    public partial class UC_EntityEditor : UserControl
    {
        Entity _current = new Entity();
        bool _edit = false;

        public UC_EntityEditor()
        {
            InitializeComponent();
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
            textEditor_details.Text = _current.Details;
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            _current.Path = textBox_path.Text;
            _current.Name = textBox_name.Text;
            _current.Tags = textBox_tags.Text;
            _current.Details = textEditor_details.Text;

            if (_edit)
                DatabaseManager.Update(_current);
            else
                DatabaseManager.Add(_current);
        }

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            string path = textBox_path.Text;
            Load();

            if (checkBox_keepPath.IsChecked.Value)
                textBox_path.Text = path;
            
        }
    }
}
