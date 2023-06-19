using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Packaging;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace TicketMachine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string printerName;

        public MainWindow()
        {
            InitializeComponent();
            code_input_box.Focus();

            var printerName = ConfigurationManager.AppSettings["PrinterName"];
            if (printerName != null)
            {
                this.printerName = printerName;
            }
            else
            {
                throw new NullReferenceException("PrinterName can not be null.");
            }

        }

        private void code_input_box_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                do_print_stuff(code_input_box.Text.Trim());
                code_input_box.Text = "";
            }
        }

        /// <summary>For printing stuff</summary>
        /// <exception cref="System.Printing.PrintQueueException">Exception is thrown if no printer with the specified name is found</exception>
        private void do_print_stuff(string text)
        {
            var printQueue = new LocalPrintServer().GetPrintQueue(printerName);

            PrintCapabilities printCapabilities = printQueue.GetPrintCapabilities();

            PageContent pageContent = new PageContent();
            pageContent.Width = printCapabilities.OrientedPageMediaWidth ?? 0;
            pageContent.Height = printCapabilities.OrientedPageMediaHeight ?? 0;

            Canvas canvas = new Canvas();
            canvas.Width = pageContent.Width;
            canvas.Height = pageContent.Height;

            var textBlock = new TextBlock();
            textBlock.Text = text;
            canvas.Children.Add(textBlock);

            var rect = new Rectangle();
            rect.Stroke = new SolidColorBrush(Colors.Black);
            rect.Width = canvas.Width;
            rect.Height = 20;
            canvas.Children.Add(rect);

            FixedPage fixedPage = new FixedPage();
            fixedPage.Children.Add(canvas);

            FixedDocument fixedDocument = new FixedDocument();

            ((IAddChild)pageContent).AddChild(fixedPage);

            fixedDocument.Pages.Add(pageContent);

            var documentWriter = PrintQueue.CreateXpsDocumentWriter(printQueue);
            documentWriter.Write(fixedDocument);
        }
    }
}
