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
    /// Interaction logic for UC_DiceExpression.xaml
    /// </summary>
    public partial class UC_DiceExpression : UserControl
    {
        DiceDB _entry;
        public DiceDB DiceEntry 
        { 
            get { return _entry; }
            set
            {
                _entry = value;
                if (_entry != null)
                {
                    textbox_name.Text = _entry.Name;
                    textbox_expression.Text = _entry.Expression;
                }
            }
        }
        public ListBox ListboxResults { get; set; }

        public UC_DiceExpression()
        {
            InitializeComponent();

            if (_entry != null)
            {
                textbox_name.Text = DiceEntry.Name;
                textbox_expression.Text = DiceEntry.Expression;
            }
        }

        void Roll()
        {
            var result = Roller.Roll(textbox_expression.Text);
            ListboxResults.Items.Insert(0, string.Format("{0} : {1}", DiceEntry.Name, result.Value));
        }

        private void btn_roll_Click(object sender, RoutedEventArgs e)
        {
            Roll();
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            DiceEntry.Name = textbox_name.Text;
            DiceEntry.Expression = textbox_expression.Text;

            DatabaseManager.Update(DiceEntry, false);
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            DatabaseManager.Delete(DiceEntry);
            DiceEntry = null;
            (Parent as StackPanel).Children.Remove(this);
        }

        private void textbox_expression_KeyDown(object sender, KeyEventArgs e)
        {
            Roll();
        }

    }
}
