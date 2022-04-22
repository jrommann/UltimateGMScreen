using AdonisUI.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;


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
                const string EXIST_BTN = "exist";

                var btns = new List<IMessageBoxButtonModel>();

                string existing = Properties.Settings.Default.LastDatabase;
                if (!string.IsNullOrEmpty(existing) && System.IO.File.Exists(existing))
                {
                    string name = System.IO.Path.GetFileName(existing);
                    btns.Add(MessageBoxButtons.Custom("Open " + name, EXIST_BTN));
                }

                btns.Add(MessageBoxButtons.Custom("Open Existing", OPEN_BTN));
                btns.Add(MessageBoxButtons.Custom("Create New", CREATE_BTN));

                var messageBox = new MessageBoxModel
                {
                    Text = "Open existing or create a new GM database",
                    Caption = "Existing or New GM Database",
                    Icon = AdonisUI.Controls.MessageBoxImage.Question,
                    Buttons = btns.ToArray(),
                    IsSoundEnabled = false,                    
                };
                #endregion

                AdonisUI.Controls.MessageBox.Show(messageBox);
                switch (messageBox.Result)
                {
                    case AdonisUI.Controls.MessageBoxResult.Custom:
                        if ((string)messageBox.ButtonPressed.Id == EXIST_BTN)
                        {
                            Title = Title + " - " + Properties.Settings.Default.LastDatabase;
                            _db = DatabaseManager.Open(Properties.Settings.Default.LastDatabase);                            
                        }
                        else if ((string)messageBox.ButtonPressed.Id == OPEN_BTN)
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
                            {
                                Properties.Settings.Default.LastDatabase = dlg.FileName;
                                Properties.Settings.Default.Save();
                                Title = Title + " - " + dlg.FileName;

                                _db = DatabaseManager.Open(dlg.FileName);
                            }
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
                            {
                                Properties.Settings.Default.LastDatabase = dlg.FileName;
                                Properties.Settings.Default.Save();
                                Title = Title + " - " + dlg.FileName;

                                _db = DatabaseManager.Open(dlg.FileName);
                            }
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

        private void buttonTable_Click(object sender, RoutedEventArgs e)
        {
            browser.Visibility = Visibility.Visible;
            notes.Visibility = Visibility.Hidden;
            resources.Visibility = Visibility.Hidden;
            search.Visibility = Visibility.Hidden;
            magicItems.Visibility = Visibility.Hidden;
        }

        private void buttonNotes_Click(object sender, RoutedEventArgs e)
        {            
            notes.Visibility = Visibility.Visible;
            browser.Visibility = Visibility.Hidden;
            resources.Visibility = Visibility.Hidden;
            search.Visibility = Visibility.Hidden;
            magicItems.Visibility = Visibility.Hidden;
        }

        private void buttonResources_Click(object sender, RoutedEventArgs e)
        {
            resources.Visibility = Visibility.Visible;
            notes.Visibility = Visibility.Hidden;
            browser.Visibility = Visibility.Hidden;
            search.Visibility = Visibility.Hidden;
            magicItems.Visibility = Visibility.Hidden;
        }

        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            search.Visibility = Visibility.Visible;
            resources.Visibility = Visibility.Hidden;
            notes.Visibility = Visibility.Hidden;
            browser.Visibility = Visibility.Hidden;
            magicItems.Visibility = Visibility.Hidden;
        }

        private void buttonMagicItems_Click(object sender, RoutedEventArgs e)
        {
            magicItems.Visibility = Visibility.Visible;
            search.Visibility = Visibility.Hidden;
            resources.Visibility = Visibility.Hidden;
            notes.Visibility = Visibility.Hidden;
            browser.Visibility = Visibility.Hidden;
        }
    }
}
