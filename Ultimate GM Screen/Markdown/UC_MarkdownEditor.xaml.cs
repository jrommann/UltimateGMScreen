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

        private static MarkdownPipeline BuildPipeline()
        {
            return new MarkdownPipelineBuilder()
                .UseSupportedExtensions()
                .Build();
        }

        public string GetMarkdown()
        {
            return markdownText.Text;
        }

        public void SetMarkdown(string markdown)
        {
            markdownText.Text = markdown;
            RenderMarkdown(markdown);            
        }

        void RenderMarkdown(string markdown)
        {
            if (string.IsNullOrEmpty(markdown))
                return;

            var xaml = Markdig.Wpf.Markdown.ToXaml(markdown, BuildPipeline());
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xaml)))
            {
                using (var reader = new XamlXmlReader(stream, new MyXamlSchemaContext()))
                {
                    if (XamlReader.Load(reader) is FlowDocument document)
                    {
                        Viewer.Document = document;
                    }
                }
            }
        }

        private void markdownText_TextChanged(object sender, TextChangedEventArgs e)
        {
            RenderMarkdown(markdownText.Text);
        }

        private void OpenHyperlink(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            
        }

        #region -> class MyXamlSchemaContext
        class MyXamlSchemaContext : XamlSchemaContext
        {
            public override bool TryGetCompatibleXamlNamespace(string xamlNamespace, out string compatibleNamespace)
            {
                if (xamlNamespace.Equals("clr-namespace:Markdig.Wpf", StringComparison.Ordinal))
                {
                    compatibleNamespace = $"clr-namespace:Markdig.Wpf;assembly={Assembly.GetAssembly(typeof(Markdig.Wpf.Styles)).FullName}";
                    return true;
                }
                return base.TryGetCompatibleXamlNamespace(xamlNamespace, out compatibleNamespace);
            }
        }
        #endregion
    }
}
