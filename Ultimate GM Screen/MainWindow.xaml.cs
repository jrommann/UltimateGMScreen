using AdonisUI.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Ultimate_GM_Screen.Archive;
using Ultimate_GM_Screen.Folders;

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
            string appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (Properties.Settings.Default.ApplicationVersion != appVersion)
            { 
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.ApplicationVersion = appVersion;
                Properties.Settings.Default.Save(); 
            }

            InitializeComponent();            
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Title += string.Format(" - v{0}.{1}", version.Major, version.Minor);

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

                browser.StartingAddress = DatabaseManager.TableURL;
            }
        }

        private void AdonisWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DatabaseManager.TableURL = browser.browser.Source.ToString();
        }  

        private void buttonTable_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            browser.Visibility = Visibility.Visible;
        }

        private void buttonNotes_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            notes.Visibility = Visibility.Visible;
        }

        private void buttonResources_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            resources.Visibility = Visibility.Visible;
        }

        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            search.Visibility = Visibility.Visible;
        }

        private void buttonMagicItems_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            magicItems.Visibility = Visibility.Visible;            
        }

        private void buttonDice_Click(object sender, RoutedEventArgs e)
        {
            HideAll();
            dice.Visibility = Visibility.Visible;
        }

        void HideAll()
        {            
            magicItems.Visibility = Visibility.Hidden;
            search.Visibility = Visibility.Hidden;
            resources.Visibility = Visibility.Hidden;
            notes.Visibility = Visibility.Hidden;
            browser.Visibility = Visibility.Hidden;
            dice.Visibility = Visibility.Hidden;
        }

        private void buttonArchives_Click(object sender, RoutedEventArgs e)
        {
            var win = new Window_Archives();            
            win.ShowInTaskbar = true;
            win.Owner = this.Parent as Window;
            win.ShowActivated = true;
            win.Show();
        }

        private void buttonFolders_Click(object sender, RoutedEventArgs e)
        {
            var win = new Window_Folders();
            win.ShowInTaskbar = true;
            win.Owner = this.Parent as Window;
            win.ShowActivated = true;
            win.Show();
        }
        
        private void buttonRevisions_Click(object sender, RoutedEventArgs e)
        {
            var win = new Window_RevisionManager();
            win.ShowInTaskbar = true;
            win.Owner = this.Parent as Window;
            win.ShowActivated = true;
            win.Show();
        }


    }
}
