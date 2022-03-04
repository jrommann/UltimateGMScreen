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
    /// Interaction logic for UC_EntityRevisionEntry.xaml
    /// </summary>
    public partial class UC_EntityRevisionEntry : UserControl
    {
        public EntityRevision Current { get; set; }
        
        public UC_EntityRevisionEntry()
        {
            InitializeComponent();
        }        

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {            
            dateLbl.Content = Current.Date;            
            detailsHtml.Html = System.Text.RegularExpressions.Regex.Unescape(Current.Details);
        }

        private void restoreBtn_Click(object sender, RoutedEventArgs e)
        {
            var entity = DatabaseManager.Entity_FromID(Current.EntityID);
            if (entity != null)
            {
                entity.Details = Current.Details;
                DatabaseManager.Update(entity);
            }            
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            DatabaseManager.Delete(Current);
        }
    }
}
