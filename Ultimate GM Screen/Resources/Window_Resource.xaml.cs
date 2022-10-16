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

namespace Ultimate_GM_Screen.Resources
{
    /// <summary>
    /// Interaction logic for Window_Resource.xaml
    /// </summary>
    public partial class Window_Resource : AdonisWindow
    {
        public Window_Resource()
        {
            InitializeComponent();
        }

        private void AdonisWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            editor.Save();
        }

        public void Load(Resource res)
        {
            Title = res.Name;
            editor.Load(res, true);
            editor.CanPin = false;
            editor.CanPopout = false;

        }
    }
}
