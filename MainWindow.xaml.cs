using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        private List<string> BadElements = new List<string>
        {
             "LastModifiedTime", "LastModifiedBy", "*ID", "*Status"
        };

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var text = File.ReadAllText(@"C:\Users\dan\Downloads\test.xml");
            ((TextProcessor)DataContext).InputText = text;
            raw.Text = text;
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {

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

                foreach (var el in BadElements)
                {
                    string elS = null;
                    if (el.Contains("*"))
                    {
                        elS = el.Replace("*", "");

                        if (el.EndsWith("*"))
                        {
                            doc.Descendants().Where(x => x.Name.LocalName.StartsWith(elS)).Remove();
                        }
                        if (el.StartsWith("*"))
                        {
                            doc.Descendants().Where(x => x.Name.LocalName.EndsWith(elS)).Remove();
                        }
                    }
                    else
                    {
                        doc.Descendants(el).Remove();
                    }
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
    }

}
