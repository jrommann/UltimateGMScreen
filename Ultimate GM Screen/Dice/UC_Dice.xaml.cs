using Dice;
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

namespace Ultimate_GM_Screen.Dice
{
    /// <summary>
    /// Interaction logic for UC_Dice.xaml
    /// </summary>
    public partial class UC_Dice : UserControl
    {
        public UC_Dice()
        {
            InitializeComponent();
            DatabaseManager.OnOpened += () =>
            {

                var list = DatabaseManager.Dice_GetAll();
                foreach (var e in list)
                    diceSets.Children.Add(new UC_DiceExpression() { DiceEntry = e, ListboxResults = listbox_results });
            };
        }

        private void btn_roll_Click(object sender, RoutedEventArgs e)
        {
            Roll();
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            DiceDB entry = new DiceDB() { Name = "new", Expression = textbox_expression.Text };
            DatabaseManager.Add(entry);
            diceSets.Children.Add(new UC_DiceExpression() { DiceEntry = entry, ListboxResults = listbox_results });
        }

        private void textbox_expression_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Roll();
        }

        void Roll()
        {
            try
            {
                var result = Roller.Roll(textbox_expression.Text);
                listbox_results.Items.Insert(0, string.Format("{0} : {1}", result.Expression, result.Value));                
            }
            catch (Exception x) { MessageBox.Show("Invalid dice text. Check your expression syntax.", "ERROR - Dice Experssion"); }
        }

        #region -> doc label / link
        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://skizzerz.net/DiceRoller/Dice_Reference");
        }
        #endregion
    }
}
