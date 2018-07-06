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
            _currentUser = await _userRepo.GetAsync(Properties.Settings.Default.CurrentUser.ID);

            Properties.Settings.Default.CurrentUser = _currentUser;

            _languageList = (await _languageRepo.GetAllAsync()).Where(l => l.IsDesktopLanguage == true).ToList();

            bool listContainsDesktopLanguages = _languageList.Any(l => l.LocalName == "Nederlands") && _languageList.Any(l => l.LocalName == "English");

            if (listContainsDesktopLanguages)
            {
                if (_currentUser.PreferredLanguageID == null)
                {
                    rbPreferNL.IsChecked = true;
                }
                else
                {
                    SelectRadioButtonByLanguage();
                }
            }
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
            if (_preferredLanguage == null)
            {
                rbPreferNL.IsChecked = true;
            }

            _currentUser.PreferredLanguageID = _preferredLanguage.ID;
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
