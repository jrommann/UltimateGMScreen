using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Ultimate_GM_Screen.Resources
{
    /// <summary>
    /// Interaction logic for UC_Resources.xaml
    /// </summary>
    public partial class UC_Resources : UserControl
    {
        List<UC_ResourceEditor> _pinnedEditors = new List<UC_ResourceEditor>();
        List<Button> _pinnedButtons = new List<Button>();
        Resource _currentResource;
        bool _loaded = false;

        public UC_Resources()
        {
            InitializeComponent();
            
            DatabaseManager.OnResourcesChanged += DatabaseManager_OnResourcesChanged;

            resourceEditor.OnPinClicked += Editor_OnPinClicked;
        }

        private void DatabaseManager_OnResourcesChanged(Resource specificItem = null, bool pathChanged = false)
        {
            if (pathChanged)
                UpdateResourceList();
        }

        void UpdateResourceList(List<Resource> notes = null)
        {
            if (notes != null)
                listBox_resources.ItemsSource = notes;
            else
                listBox_resources.ItemsSource = DatabaseManager.Resources_GetAll();
        }

        void ChangeResource(Resource res, bool edit)
        {
            resourceEditor.Save();
            resourceEditor.Load(res, edit);
            _currentResource = res;
        }

        private void button_delete_Click(object sender, RoutedEventArgs e)
        {
            if (_currentResource != null)
            {
                DatabaseManager.Delete(_currentResource);
                _currentResource = null;
            }
        }

        private void btn_Current_Click(object sender, RoutedEventArgs e)
        {
            Switch_Displayed_Note(resourceEditor);
        }

        void Switch_Displayed_Note(UC_ResourceEditor show)
        {
            resourceEditor.Visibility = Visibility.Hidden;
            _pinnedEditors.ForEach(x => x.Visibility = Visibility.Hidden);

            show.Visibility = Visibility.Visible;
        }

        private void Editor_OnPinClicked(Resource note = null)
        {
            if (note == null)
                return;

            var editor = new UC_ResourceEditor();
            editor.SetValue(Grid.RowProperty, 1);
            editor.Load(note, true);
            editor.CanPopout = false;
            editor.Visibility = Visibility.Hidden;
            editor.Pinned = true;
            editor.OnUnpinClicked += Editor_OnUnpinClicked;
            notesGrid.Children.Add(editor);
            _pinnedEditors.Add(editor);

            var btn = new Button();
            btn.Content = note.Name;
            btn.Click += (object sender, RoutedEventArgs e) => { Switch_Displayed_Note(editor); };
            btn.Tag = note;
            _pinnedButtons.Add(btn);

            stackpanel_pinnedNotes.Children.Add(btn);
        }

        private void Editor_OnUnpinClicked(Resource item = null)
        {
            var btn = _pinnedButtons.Find(x => x.Tag == item);
            var editor = _pinnedEditors.Find(x => x.Current == item);
            _pinnedButtons.Remove(btn);
            _pinnedEditors.Remove(editor);

            stackpanel_pinnedNotes.Children.Remove(btn);
            notesGrid.Children.Remove(editor);

            Switch_Displayed_Note(resourceEditor);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded)
                return;

            UpdateResourceList();
            _loaded = true;
        }      
        private void listBox_resources_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox_resources.SelectedItem is Resource)
                ChangeResource(listBox_resources.SelectedItem as Resource, true);
        }

        private void listBox_resources_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is TextBlock)
            {
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    if ((e.OriginalSource as TextBlock).DataContext is Resource)
                    {
                        e.Handled = true;

                        var r = (e.OriginalSource as TextBlock).DataContext as Resource;
                        var w = new Window_Resource();
                        w.Load(r);
                        w.Show();
                    }
                }
                else if (e.MiddleButton == MouseButtonState.Pressed)
                {
                    if ((e.OriginalSource as TextBlock).DataContext is Resource)
                    {
                        e.Handled = true;
                        var r = (e.OriginalSource as TextBlock).DataContext as Resource;
                        Editor_OnPinClicked(r);
                    }
                }
            }
        }
    }
}
