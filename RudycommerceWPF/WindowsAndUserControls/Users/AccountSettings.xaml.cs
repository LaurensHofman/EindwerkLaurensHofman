using RudycommerceData.Entities;
using RudycommerceData.Entities.DesktopUsers;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RudycommerceWPF.WindowsAndUserControls.Users
{
    /// <summary>
    /// Interaction logic for AccountSettings.xaml
    /// </summary>
    public partial class AccountSettings : MultilingualUserControl
    {
        public delegate void AccountSaved(DesktopUser desktopUser);
        public event AccountSaved OnAccountSave;

        private List<Language> _languageList;

        private bool listContainsDesktopLanguages;

        private ILanguageRepository _languageRepo;
        private IDesktopUserRepository _userRepo;
        private DesktopUser _currentUser;

        public AccountSettings()
        {
            InitializeComponent();

            _languageRepo = new LanguageRepository();
            _userRepo = new DesktopUserRepository();

            DataContext = this;

            InitializeUserControl();
        }

        private async void InitializeUserControl()
        {
            // Gets the current user

            _currentUser = await _userRepo.GetAsync(Properties.Settings.Default.CurrentUser.ID);

            // Puts the current user in a global accessible variable
            Properties.Settings.Default.CurrentUser = _currentUser;

            // Gets all the languages
            _languageList = (await _languageRepo.GetAllAsync()).Where(l => l.IsDesktopLanguage == true).ToList();

            // Checks whether the database already contains the 2 default display languages
            listContainsDesktopLanguages = _languageList.Any(l => l.LocalName == "Nederlands") && _languageList.Any(l => l.LocalName == "English");

            // If the desktop languages exist yet...
            if (listContainsDesktopLanguages)
            {
                // ... But the user has no preferred Language yet, select Dutch 
                if (_currentUser.PreferredLanguageID == null)
                {
                    rbPreferNL.IsChecked = true;
                }
                // ... And the user has a preferred language, select its preferred language
                else
                {
                    SelectRadioButtonByLanguage();
                }
            }
            // Else, hide the selector
            else
            {
                languageSelector.Visibility = Visibility.Collapsed;
            }
        }

        private void SelectRadioButtonByLanguage()
        {
            switch (_currentUser.PreferredLanguage.LocalName)
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
        
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // If somehow no language was selected, choose dutch
            if (_preferredLanguage == null && listContainsDesktopLanguages)
            {
                rbPreferNL.IsChecked = true;
            }

            // Sets the preferred language for the user
            _currentUser.PreferredLanguageID = _preferredLanguage.ID;

            // Update the user and save
            await _userRepo.UpdateAsync(_currentUser);
            await _userRepo.SaveChangesAsync();
            OnAccountSave(_currentUser);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
