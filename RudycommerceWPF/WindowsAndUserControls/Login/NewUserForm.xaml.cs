using RudycommerceData.Entities;
using RudycommerceData.Entities.DesktopUsers;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceLib.Notify;
using RudycommerceLib.Security;
using RudycommerceWPF.WindowsAndUserControls.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
    /// Interaction logic for NewUserForm.xaml
    /// </summary>
    public partial class NewUserForm : LanguageWindow
    {
        private List<Language> _languageList;

        private ILanguageRepository _languageRepo;
        private IDesktopUserRepository _userRepo;
        private GmailNotifier _notifier;

        public DesktopUser NewDesktopUser { get; set; }

        public NewUserForm() : this(null)
        {
        }

        public NewUserForm(Language _prefLanguage)
        {
            InitializeComponent();
            _languageRepo = new LanguageRepository();
            _userRepo = new DesktopUserRepository();
            _notifier = new GmailNotifier();

            InitializeWindow();

            _preferredLanguage = _prefLanguage;

            SelectRadioButtonByLanguage();
        }

        private async void InitializeWindow()
        {
            _languageList = (await _languageRepo.GetAllAsync()).Where(l => l.IsDesktopLanguage == true).ToList();

            bool listContainsDesktopLanguages = _languageList.Any(l => l.LocalName == "Nederlands") && _languageList.Any(l => l.LocalName == "English");

            if (listContainsDesktopLanguages)
            {
                rbPreferNL.IsChecked = true;
            }
            else
            {
                languageSelector.Visibility = Visibility.Collapsed;
                lblPrefLang.Visibility = Visibility.Collapsed;
            }

            DataContext = this;

            NewDesktopUser = new DesktopUser()
            {
                IsAdmin = true,
                VerifiedByAdmin = true,
                Salt = Encryption.GetNewSalt(32)
            };
        }

        private void SelectRadioButtonByLanguage()
        {
            if (_preferredLanguage != null)
            {
                switch (_preferredLanguage.LocalName)
                {
                    case "Nederlands":
                        rbPreferNL.IsChecked = true;
                        break;
                    case "English":
                        rbPreferEN.IsChecked = true;
                        break;
                    default:
                        rbPreferNL.IsChecked = true;
                        break;
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (rbPreferNL.IsChecked == true)
            {
                NewDesktopUser.PreferredLanguage = _languageList.Single(l => l.LocalName == "Nederlands");
                SetLanguageDictionary();

            }
            if (rbPreferEN.IsChecked == true)
            {
                NewDesktopUser.PreferredLanguage = _languageList.Single(l => l.LocalName == "English");
                SetLanguageDictionary();
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NewDesktopUser.EncryptedPassword = Encryption.EncryptPassword(NewDesktopUser.Salt, pwdPassword.Password);

                NewDesktopUser = _userRepo.Add(NewDesktopUser);
                await _userRepo.SaveChangesAsync();

                SendMailToAdmin();
                SendMailToNewUser();

                NavigationWindow naviWindow = new NavigationWindow(NewDesktopUser.ID);
                naviWindow.Show();

                this.Close();
            }
            catch (Exception)
            {
                pwdPassword.Password = null;
                throw;
            }
        }

        private void SendMailToNewUser()
        {
            DesktopUser user = NewDesktopUser;
            string adminEmail = _userRepo.GetAllQueryable().SingleOrDefault(du => du.IsAdmin == true).Email;

            string title = "TODO Title";
            string content = "TODO Content";

            if (_preferredLanguage != null)
            {
                switch (_preferredLanguage.LocalName)
                {
                    case "Nederlands":
                        title = $"Account aangemaakt bij Rudycommerce";
                        content =
                            $"Beste {user.ToString()}, \r\n" +
                            "\r\n" +
                            $"Uw account (met gebruikersnaam '{user.Username}' is aangemaakt, maar nu moet u afwachten tot de beheerder van de applicatie u de toegansgrechten zal toekennen.\r\n" +
                            $"Gelieve de applicatiebeheerder ({adminEmail}) te contacteren indien u te lang moet wachten.\r\n" +
                            "\r\n" +
                            "Met vriendelijke groeten, \r\n" +
                            "Rudycommerce";

                        break;

                    case "English":
                        title = $"Account created for Rudycommerce";
                        content =
                            $"Dear {user.ToString()}, \r\n" +
                            "\r\n" +
                            $"Your account (with username '{user.Username}' has been created, but now you will have to wait till the administrator gives you access rights to the application.\r\n" +
                            $"Please contact the administrator ({adminEmail}) in case you have to wait too long.\r\n" +
                            "\r\n" +
                            "With kind regards, \r\n" +
                            "Rudycommerce";

                        break;

                    default:
                        title = $"Account aangemaakt bij Rudycommerce";
                        content =
                            $"Beste {user.ToString()}, \r\n" +
                            "\r\n" +
                            $"Uw account (met gebruikersnaam '{user.Username}' is aangemaakt, maar nu moet u afwachten tot de beheerder van de applicatie u de toegansgrechten zal toekennen.\r\n" +
                            $"Gelieve de applicatiebeheerder ({adminEmail}) te contacteren indien u te lang moet wachten.\r\n" +
                            "\r\n" +
                            "Met vriendelijke groeten, \r\n" +
                            "Rudycommerce";

                        break;
                }
            }
            else
            {
                title = $"Account aangemaakt bij Rudycommerce";
                content =
                    $"Beste {user.ToString()}, \r\n" +
                    "\r\n" +
                    $"Uw account (met gebruikersnaam '{user.Username}' is aangemaakt, maar nu moet u afwachten tot de beheerder van de applicatie u de toegansgrechten zal toekennen.\r\n" +
                    $"Gelieve de applicatiebeheerder ({adminEmail}) te contacteren indien u te lang moet wachten.\r\n" +
                    "\r\n" +
                    "Met vriendelijke groeten, \r\n" +
                    "Rudycommerce";
            }

            _notifier.Notify(new MailAddress(user.Email, user.ToString()), title, content);
        }

        private void SendMailToAdmin()
        {
            DesktopUser admin = _userRepo.GetAllQueryable().SingleOrDefault(du => du.IsAdmin == true);

            string title = "TODO Title";
            string content = "TODO Content";

            if (_preferredLanguage != null)
            {
                switch (_preferredLanguage.LocalName)
                {
                    case "Nederlands":
                        title = $"{NewDesktopUser.ToString()} heeft een account aangemaakt";
                        content =
                            $"Beste {admin.ToString()}, \r\n" +
                            "\r\n" +
                            $"{NewDesktopUser.ToString()} (gebruikersnaam: {NewDesktopUser.Username} ; e-mail: {NewDesktopUser.Email} ) heeft een account aangemaakt om toegang te krijgen tot de applicatie. \r\n" +
                            $"Als u {NewDesktopUser.ToString()} toegang wil geven tot de applicatie, gelieve dan in te loggen en onder 'Beheer Gebruikers' de nieuwe gebruiker toegang te verlenen \r\n" +
                            "\r\n" +
                            "Met vriendelijke groeten, \r\n" +
                            "Rudycommerce";

                        break;

                    case "English":
                        title = $"{NewDesktopUser.ToString()} has made an account";
                        content =
                            $"Dear {admin.ToString()}, \r\n" +
                            "\r\n" +
                            $"{NewDesktopUser.ToString()} (username: {NewDesktopUser.Username} ; email: {NewDesktopUser.Email} ) has made an account to gain access to the application. \r\n" +
                            $"If you want to give {NewDesktopUser.ToString()} access to the application, please log in and give the new user access within the 'Manage Users' page. \r\n" +
                            "\r\n" +
                            "With kind regards, \r\n" +
                            "Rudycommerce";

                        break;

                    default:
                        title = $"{NewDesktopUser.ToString()} heeft een account aangemaakt";
                        content =
                            $"Beste {admin.ToString()}, \r\n" +
                            "\r\n" +
                            $"{NewDesktopUser.ToString()} (gebruikersnaam: {NewDesktopUser.Username} ; e-mail: {NewDesktopUser.Email} ) heeft een account aangemaakt om toegang te krijgen tot de applicatie. \r\n" +
                            $"Als u {NewDesktopUser.ToString()} toegang wil geven tot de applicatie, gelieve dan in te loggen en onder 'Beheer Gebruikers' de nieuwe gebruiker toegang te verlenen \r\n" +
                            "\r\n" +
                            "Met vriendelijke groeten, \r\n" +
                            "Rudycommerce";

                        break;
                }
            }
            else
            {
                title = $"{NewDesktopUser.ToString()} heeft een account aangemaakt";
                content =
                    $"Beste {admin.ToString()}, \r\n" +
                    "\r\n" +
                    $"{NewDesktopUser.ToString()} (gebruikersnaam: {NewDesktopUser.Username} ; e-mail: {NewDesktopUser.Email} ) heeft een account aangemaakt om toegang te krijgen tot de applicatie. \r\n" +
                    $"Als u {NewDesktopUser.ToString()} toegang wil geven tot de applicatie, gelieve dan in te loggen en onder 'Beheer Gebruikers' de nieuwe gebruiker toegang te verlenen \r\n" +
                    "\r\n" +
                    "Met vriendelijke groeten, \r\n" +
                    "Rudycommerce";
            }

            _notifier.Notify(new MailAddress(admin.Email, admin.ToString()), title, content);
        }

        private void pwdPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtPasswordVisible.Text = pwdPassword.Password;

            int start = pwdPassword.Password.Length;
            int length = 0;
            pwdPassword.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(pwdPassword, new object[] { start, length });
        }

        private void txtPasswordVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            pwdPassword.Password = txtPasswordVisible.Text;

            int start = txtPasswordVisible.Text.Length;
            int length = 0;
            txtPasswordVisible.Select(start, length);
        }

        private void btnShowHidePwd_Click(object sender, RoutedEventArgs e)
        {
            btnShowHidePwd.Content =
                (txtPasswordVisible.Visibility == Visibility.Collapsed) ?
                FindResource("Hide") : FindResource("Show");

            ToggleShowPassword();
        }

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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
