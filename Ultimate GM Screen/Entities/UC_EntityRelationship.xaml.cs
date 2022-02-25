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
    /// Interaction logic for UC_EntityRelationship.xaml
    /// </summary>
    public partial class UC_EntityRelationship : UserControl
    {
        EntityRelationship _relationship;

        public UC_EntityRelationship()
        {
            InitializeComponent();
        }

        public void Load(EntityRelationship rel)
        {
            _relationship = rel;
            label_title.Content = rel.Description;
            label_child_link.Inlines.Clear();
            label_child_link.Inlines.Add(rel.Child.Name);

        }

        private void label_child_link_Click(object sender, RoutedEventArgs e)
        {
            var w = new Window_Entity();
            w.Load(_relationship.Child);
            w.Show();
        }

        private void button_edit_Click(object sender, RoutedEventArgs e)
        {
            var w = new Window_EditRelationship();
            w.Load(_relationship.Parent, _relationship, true);
            w.ShowDialog();
        }

        private void button_delete_Click(object sender, RoutedEventArgs e)
        {
            DatabaseManager.Delete(_relationship);
        }
    }
}
