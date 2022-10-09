using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xaml;
using Markdig;
using Markdig.Wpf;
using XamlReader = System.Windows.Markup.XamlReader;

namespace Ultimate_GM_Screen.Markdown
{
    /// <summary>
    /// Interaction logic for UC_MarkdownEditor.xaml
    /// </summary>
    public partial class UC_MarkdownEditor : UserControl
    {
        public UC_MarkdownEditor()
        {
            InitializeComponent();            
        }
       
        public string GetMarkdown()
        {
            return markdownText.Text;
        }

        public void SetMarkdown(string markdown)
        {
            markdownText.Text = markdown;         
        }       

        private void markdownText_TextChanged(object sender, TextChangedEventArgs e)
        {
            Viewer.Markdown = markdownText.Text;
        }
    }
}
