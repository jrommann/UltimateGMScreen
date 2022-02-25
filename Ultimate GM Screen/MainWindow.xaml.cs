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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ultimate_GM_Screen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : AdonisWindow
    {
        DatabaseManager _db = null;

        public MainWindow()
        {
            InitializeComponent();
            browser.StartingAddress = Properties.Settings.Default.TableTopAddress;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_db == null)
            {
                #region -> ask
                const string CREATE_BTN = "create";
                const string OPEN_BTN = "open";

                var messageBox = new MessageBoxModel
                {
                    Text = "Open an existing or create a new GM database",
                    Caption = "Existing or New GM Database",
                    Icon = AdonisUI.Controls.MessageBoxImage.Question,
                    Buttons = new[]
                    {
                        MessageBoxButtons.Custom("Open Existing", OPEN_BTN),
                        MessageBoxButtons.Custom("Create New", CREATE_BTN),
                    },
                    IsSoundEnabled = false,                    
                };
                #endregion

                AdonisUI.Controls.MessageBox.Show(messageBox);
                switch (messageBox.Result)
                {
                    case AdonisUI.Controls.MessageBoxResult.Custom:
                        if ((string)messageBox.ButtonPressed.Id == OPEN_BTN)
                        {
                            #region -> open
                            // Create OpenFileDialog
                            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                            // Set filter for file extension and default file extension 
                            dlg.DefaultExt = ".db";
                            dlg.Filter = "Ultimate GM Screen (*.ugs)|*.ugs";

                            // Display OpenFileDialog by calling ShowDialog method 
                            Nullable<bool> result = dlg.ShowDialog();

                            // Get the selected file name and display in a TextBox 
                            if (result == true)
                                _db = DatabaseManager.Open(dlg.FileName);
                            #endregion
                        }
                        else if ((string)messageBox.ButtonPressed.Id == CREATE_BTN)
                        {
                            #region -> new
                            // Configure save file dialog box
                            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                            dlg.FileName = "New GM Screen"; // Default file name
                            dlg.DefaultExt = ".ugs";
                            dlg.Filter = "Ultimate GM Screen (*.ugs)|*.ugs";

                            // Show save file dialog box
                            Nullable<bool> result = dlg.ShowDialog();

                            // Process save file dialog box results
                            if (result == true)
                                _db = DatabaseManager.Open(dlg.FileName);
                            #endregion
                        }
                        break;
                }
            }            
        }

        private void AdonisWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.TableTopAddress = browser.browser.Source.ToString();
            Properties.Settings.Default.Save();
        }
    }
}
