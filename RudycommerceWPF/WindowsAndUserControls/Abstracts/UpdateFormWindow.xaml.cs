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

namespace RudycommerceWPF.WindowsAndUserControls.Abstracts
{
    /// <summary>
    /// A window specifically made to show update forms as popup
    /// </summary>
    public partial class UpdateFormWindow: MultilingualWindow 
    {
        //public UpdateFormWindow()
        //{
        //    InitializeComponent();
        //}

        // Gets a form as parameter to show in the window
        public UpdateFormWindow(FormUserControl form)
        {
            InitializeComponent();

            contentControl.Content = form;
        }
    }
}
