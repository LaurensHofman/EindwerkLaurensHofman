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
using System.Windows.Shapes;

namespace RudycommerceWPF.WindowsAndUserControls
{
    /// <summary>
    /// Interaction logic for MyDialog.xaml
    /// </summary>
    public partial class MyDialog : Window
    {
        public MyDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Opens a dialog window, that has an input field, shows a Label, a Cancel button and a Submit button
        /// </summary>
        /// <param name="cancel">Text for the cancel button</param>
        /// <param name="submit">Text for the submit button</param>
        /// <param name="title">Text for the title label</param>
        public MyDialog(string cancel, string submit, string title)
        {
            InitializeComponent();

            tbTitle.Content = title;
            btnCancel.Content = cancel;
            btnSubmit.Content = submit;

            ResponseTextBox.Focus();
        }

        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }

        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // When DialogResult is set to True, the caller can use the ResponseText to get the value that was inserted

            DialogResult = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
