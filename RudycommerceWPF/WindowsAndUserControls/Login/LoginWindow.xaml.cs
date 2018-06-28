using RudycommerceData.Entities;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceLib.Properties;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RudycommerceWPF.WindowsAndUserControls.Login
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : LanguageWindow
    {
        private List<Language> _languageList;
        private bool newWindow = false;

        private IDesktopUserRepository _desktopUserRepo;
        private ILanguageRepository _languageRepo;

        public LoginWindow()
        {
            InitializeComponent();

            _desktopUserRepo = new DesktopUserRepository();
            _languageRepo = new LanguageRepository();

            try
            {
                InitializeWindow();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async void InitializeWindow()
        {
            AnyDesktopUser();
            _languageList = (await _languageRepo.GetAllAsync()).Where(l => l.IsDesktopLanguage == true).ToList() ;

            bool listContainsDesktopLanguages = _languageList.Any(l => l.LocalName == "Nederlands") && _languageList.Any(l => l.LocalName == "English");

            if (listContainsDesktopLanguages)
            {
                rbPreferNL.IsChecked = true;
            }
            else
            {
                languageSelector.Visibility = Visibility.Collapsed;
            }
        }

        private async void AnyDesktopUser()
        {
            try
            {
                bool anyDesktopUser; 

                anyDesktopUser = await _desktopUserRepo.AnyAsync();

                if (anyDesktopUser)
                {
                    txtUsername.Focus();
                }
                else
                {
                    newWindow = true;

                    // TODO

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected override void SetLanguageDictionary()
        {
            base.SetLanguageDictionary();

            CultureInfo cult;            

            switch (_preferredLanguage.LocalName)
            {
                case "Nederlands":
                    cult = CultureInfo.CreateSpecificCulture("nl");
                    break;

                case "English":
                    cult = CultureInfo.CreateSpecificCulture("en");
                    break;

                default:
                    cult = CultureInfo.CreateSpecificCulture("nl");
                    break;
            };

            Thread.CurrentThread.CurrentUICulture = cult;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (newWindow)
            {
                e.Cancel = false;
            }
            else
            {
                string messageboxContent = LangResource.MBExitContent;
                string messageboxTitle = LangResource.MBExitTitle;

                MessageBoxManager.Yes = LangResource.Yes;
                MessageBoxManager.No = LangResource.No;
                MessageBoxManager.Register();

                if (MessageBox.Show(messageboxContent, messageboxTitle, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    MessageBoxManager.Unregister();

                    e.Cancel = false;
                }
                else
                {
                    MessageBoxManager.Unregister();
                    e.Cancel = true;
                }
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLazy_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtPasswordVisible_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnNewUser_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnShowHidePwd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void pwdPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (rbPreferNL.IsChecked == true)
            {
                _preferredLanguage = _languageList.Single(l => l.LocalName == "Nederlands");
                SetLanguageDictionary();
            }
            if (rbPreferEN.IsChecked == true)
            {
                _preferredLanguage = _languageList.Single(l => l.LocalName == "English");
                SetLanguageDictionary();
            }
        }
    }
}
