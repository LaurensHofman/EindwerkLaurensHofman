using RudycommerceData.Entities;
using RudycommerceData.Entities.DesktopUsers;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceWPF.WindowsAndUserControls.Abstracts;
using RudycommerceWPF.WindowsAndUserControls.Users;
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
        private IDesktopUserRepository _userRepo;
        private ILanguageRepository _langRepo;

        private DesktopUser _currentUser;

        public NavigationWindow()
        {
            InitializeComponent();
        }

        public NavigationWindow(int userID)
        {
            InitializeComponent();

            _userRepo = new DesktopUserRepository();
            _langRepo = new LanguageRepository();

            InitializeWindow(userID);
        }

        private async Task InitializeWindow(int userID)
        {
            _currentUser = await _userRepo.GetAsync(userID);
            Properties.Settings.Default.CurrentUser = _currentUser;

            EnableUserTab();
        }

        private void EnableUserTab()
        {
            if (_currentUser.IsAdmin)
            {
                stackDesktopUser.IsEnabled = true;
                stackDesktopUser.Visibility = Visibility.Visible;
            }
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

        private void menuSettings(object sender, RoutedEventArgs e)
        {
            HideAllUserControls();

            if (ccSettings.Content == null || (ccSettings.Content as AccountSettings).Visibility == Visibility.Collapsed)
            {
                AccountSettings _content = new AccountSettings();

                _content.OnAccountSave += ApplySettings;

                ccSettings.Content = _content;
            }
            
            ccSettings.Visibility = Visibility.Visible;
            (ccSettings.Content as AccountSettings).Visibility = Visibility.Visible;
        }

        private async void ApplySettings(DesktopUser desktopUser)
        {
            _preferredLanguage = await _langRepo.GetAsync((int)desktopUser.PreferredLanguageID) ;

            SetLanguageDictionary();

            foreach (ContentControl contentControl in UserControls.Children)
            {
                contentControl.Content = null;
            }
        }

        private void ShowUserControl<langUC>(ContentControl contentControl) where langUC : LanguageUserControl, new()
        {
            // Gets the User control (<Type>) that has to be shown, and defines it as a UserControl (inheritence from :LanguageUserControl)
            // and makes sure its instantiatable ( new() ).
            // Gets the ContentControl in which to show the new UserControl.
            
            // First hides all the ContentControls.
            HideAllUserControls();

            // Checks if the ContentControl has content (or if the content is visible (which is done by the cancel click inside the userControl)
            // If the contentControl's content hasn't yet been defined, make a new UserControl.
            // If the contentControl's content has been hidden (by a cancel click), then make a new UserControl as well.
            // Else: act normal (show the userControl). This allows it to work a little bit as tabs, the content is not lost when switching back and forth.
            if (contentControl.Content == null || (contentControl.Content as langUC).Visibility == Visibility.Collapsed)
            {
                langUC _content = new langUC();

                contentControl.Content = _content;
            }

            // Make the ContentControl and the UserControl visible
            contentControl.Visibility = Visibility.Visible;
            (contentControl.Content as langUC).Visibility = Visibility.Visible;
        }

        private void HideAllUserControls()
        {
            foreach (ContentControl contentControl in UserControls.Children)
            {
                contentControl.Visibility = Visibility.Collapsed;
            }
        }
    }
}
