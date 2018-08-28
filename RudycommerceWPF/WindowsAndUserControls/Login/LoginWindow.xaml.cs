using RudycommerceData.Entities;
using RudycommerceData.Models;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceLib.Properties;
using RudycommerceWPF.WindowsAndUserControls.Abstracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
    public partial class LoginWindow : MultilingualWindow
    {
        private List<Language> _languageList;

        /// <summary>
        /// Determines whether a new window is going to be shown, so no message has to be asked because the window is closing
        /// </summary>
        private bool newWindow = false;
        public DesktopLogin DesktopLogin { get; set; }

        private IDesktopUserRepository _desktopUserRepo;
        private ILanguageRepository _languageRepo;

        public LoginWindow()
        {
            InitializeComponent();

            DataContext = this;

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
            try
            {
                // Opens another window to create an admin account if there are no desktop users yet.
                AnyDesktopUser();

                // Gets all the languages
                _languageList = (await _languageRepo.GetAllAsync()).Where(l => l.IsDesktopLanguage == true).ToList();

                // Checks whether the necessary desktop languages are already in the database
                bool listContainsDesktopLanguages = _languageList.Any(l => l.LocalName == "Nederlands") && _languageList.Any(l => l.LocalName == "English");

                // If they the necessary desktop languages exist, select dutch as default.
                if (listContainsDesktopLanguages)
                {
                    rbPreferNL.IsChecked = true;
                }
                // Otherwise hide the language selector
                else
                {
                    languageSelector.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(LangResource.ConnectionToDatabaseFailed);
                newWindow = true;
                this.Close();
            }
        }

        /// <summary>
        /// Gets whether there exists any desktop user already. If not, opens another window to create an admin account.
        /// </summary>
        private async void AnyDesktopUser()
        {
            try
            {
                bool anyDesktopUser;

                anyDesktopUser = await _desktopUserRepo.AnyAsync();

                if (anyDesktopUser)
                {
                    txtUsername.Focus();

                    DesktopLogin = new DesktopLogin();
                }
                else
                {
                    newWindow = true;

                    AdminUserForm newAdminForm = new AdminUserForm();

                    newAdminForm.Show();

                    this.Close();
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
                // Calcels closing the window
                e.Cancel = false;
            }
            else
            {
                // Prompt if the user wants to close the window

                string messageboxContent = LangResource.MBExitContent;
                string messageboxTitle = LangResource.MBExitTitle;

                MessageBoxManager.Yes = LangResource.Yes;
                MessageBoxManager.No = LangResource.No;
                MessageBoxManager.Register();

                if (MessageBox.Show(messageboxContent, messageboxTitle, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // Close the window

                    MessageBoxManager.Unregister();

                    e.Cancel = false;
                }
                else
                {
                    // Don't close the window

                    MessageBoxManager.Unregister();
                    e.Cancel = true;
                }
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            DesktopLogin.Username = txtUsername.Text;
            DesktopLogin.Password = pwdPassword.Password;

            int? userID = await DesktopLogin.Authenticate();

            // User ID >= 0 means the user exists
            if (userID != null)
            {
                // User ID == 0 means the user exists but is not verified yet by the admin
                if (userID == 0)
                {
                    MessageBox.Show(LangResource.NotVerifiedYet);
                }
                // Succesful login
                else
                {
                    NavigationWindow naviWindow = new NavigationWindow((int)userID);
                    naviWindow.Show();
                    newWindow = true;

                    this.Close();
                }
            }
            // Login failed
            else
            {
                MessageBox.Show(LangResource.LoginFailedWPF);
            }

            DesktopLogin.Password = null;
            pwdPassword.Password = null;
        }

        ///// <summary>
        ///// Please delete this
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void btnLazy_Click(object sender, RoutedEventArgs e)
        //{
        //    DesktopLogin.Username = "laurens";
        //    DesktopLogin.Password = "laurens";
        //    btnLogin_Click(null, null);
        //}

        /// <summary>
        /// Makes the passwordbox have the same content as the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPasswordVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            pwdPassword.Password = txtPasswordVisible.Text;

            int start = txtPasswordVisible.Text.Length;
            int length = 0;
            txtPasswordVisible.Select(start, length);
        }

        /// <summary>
        /// Opens a new window with a desktop user form, passing the (possibly) selected preferred language
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewUser_Click(object sender, RoutedEventArgs e)
        {
            NewUserForm NewDesktopUser = new NewUserForm(_preferredLanguage);
            NewDesktopUser.Show();
            newWindow = true;
            this.Close();
        }

        /// <summary>
        /// Changes the icon of the toggle button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowHidePwd_Click(object sender, RoutedEventArgs e)
        {
            btnShowHidePwd.Content =
                (txtPasswordVisible.Visibility == Visibility.Collapsed) ?
                FindResource("Hide") : FindResource("Show");

            ToggleShowPassword();
        }

        /// <summary>
        /// Toggles the visibility of the password and text box
        /// </summary>
        private void ToggleShowPassword()
        {
            if (txtPasswordVisible.Visibility == Visibility.Collapsed)
            {
                txtPasswordVisible.Visibility = Visibility.Visible;
                pwdPassword.Visibility = Visibility.Collapsed;
                txtPasswordVisible.Focus();
            }
            else
            {
                pwdPassword.Visibility = Visibility.Visible;
                txtPasswordVisible.Visibility = Visibility.Collapsed;
                pwdPassword.Focus();
            }
        }

        /// <summary>
        /// Makes the hidden textbox have the same content as the password box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pwdPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtPasswordVisible.Text = pwdPassword.Password;

            int start = pwdPassword.Password.Length;
            int length = 0;
            pwdPassword.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(pwdPassword, new object[] { start, length });

        }

        /// <summary>
        /// Selects the language matching the selected radiobutton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
