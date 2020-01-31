using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using System.Xml.Linq;

namespace L8T.SanitizeXML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new TextProcessor();

        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "XML Files (*.xml)|*.xml" };
            if (dialog.ShowDialog() == true)
            {
                var text = File.ReadAllText(dialog.FileName);
             
                ((TextProcessor)DataContext).InputText = text;
                raw.Text = text;
            }
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog { AddExtension = true, DefaultExt = ".xml", Filter = "XML Files (*.xml)|*.xml" };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllText(dialog.FileName, clean.Text);
                }
                catch (Exception ex)
                {
                    status.Text = ex.Message;
                }

            }

        }

        private void Lint_Click(object sender, RoutedEventArgs e)
        {
            status.Text = null;

            try
            {
                var doc = XDocument.Parse(raw.Text);
            }
            catch (Exception ex)
            {
                status.Text = ex.Message;
            }
        }

        private void Sanitize_Click(object sender, RoutedEventArgs e)
        {
            status.Text = null;

            try
            {

                var doc = XDocument.Parse(raw.Text);

                foreach (var el in ((TextProcessor)DataContext).Elements)
                {
                    string elS = null;
                    if (el.Value.Contains("*"))
                    {
                        elS = el.Value.Replace("*", "");

                        if (el.Value.EndsWith("*"))
                        {
                            doc.Descendants().Where(x => x.Name.LocalName.StartsWith(elS)).Remove();
                        }
                        if (el.Value.StartsWith("*"))
                        {
                            doc.Descendants().Where(x => x.Name.LocalName.EndsWith(elS)).Remove();
                        }
                    }
                    else
                    {
                        doc.Descendants(el.Value).Remove();
                    }
                }
                foreach (var el in doc.Descendants().Where(x => x.Attributes().Any(a => ((TextProcessor)DataContext).Attributes.Select(x =>x.Value).Contains(a.Name.ToString()))))
                {
                    el.Attributes().Where(x => ((TextProcessor)DataContext).Attributes.Select(x => x.Value).Contains(x.Name.ToString())).Remove();
                }

                foreach (var value in ((TextProcessor)DataContext).Values.Select(x => x.Value))
                {
                    doc.Descendants().Where(x => x.Value == value).Remove();
                }


                clean.Text = doc.ToString();
            }
            catch (Exception ex)
            {
                status.Text = ex.Message;
            }


        }
    }


    public class TextProcessor
    {
        public string InputText { get; set; }
        public string OutputText { get; set; }

        public List<Text> Elements { get; set; } = new List<Text> { new Text { Value = "*ID" }, new Text { Value = "*Status" } };
        public List<Text> Attributes { get; set; } = new List<Text> { new Text { Value = "product-version" }, new Text { Value = "platform-version" } };
        public List<Text> Values { get; set; } = new List<Text>();
    }


    public class Text
    {
        public string Value { get; set; }
    }
}
