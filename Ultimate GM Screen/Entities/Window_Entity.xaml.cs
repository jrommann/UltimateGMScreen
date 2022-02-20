using AdonisUI.Controls;
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
using System.Windows.Shapes;

namespace Ultimate_GM_Screen.Entities
{
    /// <summary>
    /// Interaction logic for Window_Entity.xaml
    /// </summary>
    public partial class Window_Entity : AdonisWindow
    {       

        public Window_Entity()
        {
            InitializeComponent();
        }

        public void Load(Entity ent)
        {
            Title = ent.Name;
            entityEditor.Load(ent, true);
        }

        private void AdonisWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            entityEditor.Save();
        }
    }
}
