using RudycommerceData.Entities;
using RudycommerceData.Entities.DesktopUsers;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceLib.Properties;
using RudycommerceLib.Security;
using RudycommerceWPF.WindowsAndUserControls.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace RudycommerceWPF.WindowsAndUserControls.Login
{
    /// <summary>
    /// Interaction logic for AdminUserForm.xaml
    /// </summary>
    public partial class AdminUserForm : MultilingualWindow
    {
        private List<Language> _languageList;

        private ILanguageRepository _languageRepo;
        private IDesktopUserRepository _userRepo;

        public DesktopUser NewDesktopUser { get; set; }

        public AdminUserForm()
        {
            InitializeComponent();

            NewDesktopUser = new DesktopUser()
            {
                IsAdmin = true,
                VerifiedByAdmin = true,
                Salt = Encryption.GetNewSalt(32)
            };

            _languageRepo = new LanguageRepository();
            _userRepo = new DesktopUserRepository();

            InitializeWindow();
        }

        private async void InitializeWindow()
        {
            // Gets the desktoplanguages
            _languageList = (await _languageRepo.GetAllAsync()).Where(l => l.IsDesktopLanguage == true).ToList();

            // Checks whether the languageList contains the desktopLanguages.
            bool listContainsDesktopLanguages = _languageList.Any(l => l.LocalName == "Nederlands") && _languageList.Any(l => l.LocalName == "English");

            // If it contains the desktoplanguages, choose dutch as per default
            if (listContainsDesktopLanguages)
            {
                rbPreferNL.IsChecked = true;
            }
            else
            {
                // If it doesnt contain the desktopLanguages, collapse the selector, so the preferredLanguageID will remain null
                languageSelector.Visibility = Visibility.Collapsed;
                lblPrefLang.Visibility = Visibility.Collapsed;
            }

            DataContext = this;            
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Encrypts the user password
                NewDesktopUser.EncryptedPassword = Encryption.EncryptPassword(NewDesktopUser.Salt, pwdPassword.Password);

                try
                {
                    // Create new user
                    NewDesktopUser = _userRepo.Add(NewDesktopUser);
                    await _userRepo.SaveChangesAsync();
                }
                catch (Exception)
                {
                    MessageBox.Show(LangResource.ErrSaveFailedContent, LangResource.ErrSaveFailedTitle);
                    NewDesktopUser.EncryptedPassword = null;
                    pwdPassword.Password = null;
                }
                
                // Opens up the mainwindow
                NavigationWindow naviWindow = new NavigationWindow(NewDesktopUser.ID);
                naviWindow.Show();

                this.Close();
            }
            catch (Exception)
            {
                // TODO ERROR MESSAGE

                pwdPassword.Password = null;
                throw;
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // Selects the correct PreferredLanguageID for the new user
            if (rbPreferNL.IsChecked == true)
            {
                NewDesktopUser.PreferredLanguageID = _languageList.Single(l => l.LocalName == "Nederlands").ID;
                _preferredLanguage = _languageList.Single(l => l.LocalName == "Nederlands");
                SetLanguageDictionary();
            }
            if (rbPreferEN.IsChecked == true)
            {
                NewDesktopUser.PreferredLanguageID = _languageList.Single(l => l.LocalName == "English").ID;
                _preferredLanguage = _languageList.Single(l => l.LocalName == "English");
                SetLanguageDictionary();
            }
        }

        private void pwdPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Makes the hidden textbox have the same content as the password box when changing the content in it

            txtPasswordVisible.Text = pwdPassword.Password;

            int start = pwdPassword.Password.Length;
            int length = 0;
            pwdPassword.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(pwdPassword, new object[] { start, length });
        }

        private void txtPasswordVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Makes the passwordbox have the same content

            pwdPassword.Password = txtPasswordVisible.Text;

            int start = txtPasswordVisible.Text.Length;
            int length = 0;
            txtPasswordVisible.Select(start, length);
        }

        private void btnShowHidePwd_Click(object sender, RoutedEventArgs e)
        {
            // Changes the icon of the toggle button

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
    }
}
