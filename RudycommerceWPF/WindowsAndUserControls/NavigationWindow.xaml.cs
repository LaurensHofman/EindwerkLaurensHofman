using RudycommerceWPF.WindowsAndUserControls.Abstracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for NavigationWindow.xaml
    /// </summary>
    public partial class NavigationWindow : LanguageWindow
    {
        public NavigationWindow()
        {
            InitializeComponent();
        }

        public NavigationWindow(int userID)
        {
            InitializeComponent();

            MessageBox.Show(userID.ToString() + "navi");
        }

        protected override void SetLanguageDictionary()
        {
            base.SetLanguageDictionary();

            CultureInfo ci;

            switch (_preferredLanguage.LocalName)
            {
                case "Nederlands":
                    ci = CultureInfo.CreateSpecificCulture("nl");
                    break;

                case "English":
                    ci = CultureInfo.CreateSpecificCulture("en");
                    break;

                default:
                    ci = CultureInfo.CreateSpecificCulture("nl");
                    break;
            };

            Thread.CurrentThread.CurrentUICulture = ci;
        }
    }
}
