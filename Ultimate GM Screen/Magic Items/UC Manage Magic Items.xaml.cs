using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;


namespace Ultimate_GM_Screen.Magic_Items
{
    /// <summary>
    /// Interaction logic for UC_Manage_Magic_Items.xaml
    /// </summary>
    public partial class UC_Manage_Magic_Items : UserControl
    {
        public UC_Manage_Magic_Items()
        {
            InitializeComponent();            
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DatabaseManager.OnMagicItemsChanged += DatabaseManager_OnItemsChanged;
            try { itemListBox.ItemsSource = DatabaseManager.MagicItem_GetAll(); } catch { }
        }

        private void DatabaseManager_OnItemsChanged(MagicItem specificItem = null, bool pathChanged=false)
        {
            if(pathChanged)
                try { itemListBox.ItemsSource = DatabaseManager.MagicItem_GetAll(); } catch { }
        }        

        private void perchanceBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            var list = DatabaseManager.MagicItem_GetAll();
            foreach (var i in list)
            {
                sb.Append("<p><b>")
                    .Append(i.Name)
                    .Append("</b></p><p>")
                    .Append(i.Description)
                    .Append("</p><p>")
                    .Append("<b>").Append("Source ").Append("</b><br>").Append(i.Source).Append(" pg").Append(i.PageNumber)
                    .Append("</p>")
                    .Append("@");
            }

            var s = sb.ToString();

            RegexOptions options = RegexOptions.Multiline;
            Regex regex = new Regex(System.Environment.NewLine, options);
            s = regex.Replace(s, "<br>");
            s = s.Replace("@", System.Environment.NewLine);

            var filename = Path.GetTempFileName();
            File.WriteAllText(filename, s);
            System.Diagnostics.Process.Start("notepad", filename);
        }

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            itemEditor.Load(null);
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var item = itemListBox.SelectedItem as MagicItem;
            DatabaseManager.Delete(item);
        }

        private void itemListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var item = itemListBox.SelectedItem as MagicItem;
            itemEditor.Load(item);
        }

        private void pickRandomBtn_Click(object sender, RoutedEventArgs e)
        {
            var r = new Random();
            var list = DatabaseManager.MagicItem_GetAll();
            if (list.Count > 0)
                AdonisUI.Controls.MessageBox.Show(list[r.Next(0, list.Count)].ToStringFull(), caption: "Random Item", buttons: AdonisUI.Controls.MessageBoxButton.OK);
        }

        
    }
}
