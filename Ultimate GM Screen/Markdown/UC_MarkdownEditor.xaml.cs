using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xaml;
using Markdig;
using Markdig.Wpf;
using XamlReader = System.Windows.Markup.XamlReader;
using Ultimate_GM_Screen.Entities;

namespace Ultimate_GM_Screen.Markdown
{
    /// <summary>
    /// Interaction logic for UC_MarkdownEditor.xaml
    /// </summary>
    public partial class UC_MarkdownEditor : UserControl
    {
        Timer _updateTimer = new Timer(500);

        public UC_MarkdownEditor()
        {
            InitializeComponent();

            _updateTimer.Elapsed += _updateTimer_Elapsed;
            _updateTimer.AutoReset = false;
        }

        private void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Viewer.Dispatcher.Invoke(() => Viewer.Markdown = markdownText.Text);
        }

        public string GetMarkdown()
        {
            return markdownText.Text;
        }

        public void SetMarkdown(string markdown)
        {
            markdownText.Text = markdown;
            Viewer.Markdown = markdownText.Text;
        }

        public void InsertLink(Entity note)
        {
            markdownText.Text += string.Format("\n\n[{0}]({1})", note.Name, note.ID);
        }

        private void markdownText_TextChanged(object sender, TextChangedEventArgs e)
        {
            _updateTimer.Stop();
            _updateTimer.Start();
        }

        private void Command_HyperlinkClicked(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            int id = -1;
            if (int.TryParse(e.Parameter as string, out id))
            {
                var note = DatabaseManager.Entity_FromID(id);
                if(note != null)
                    UC_Entities.Notes.ChangeNote(note, true);
            }
        }
    }
}
