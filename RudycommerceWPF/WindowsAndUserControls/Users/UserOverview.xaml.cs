 using RudycommerceData.Entities.DesktopUsers;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceLib.Properties;
using RudycommerceWPF.WindowsAndUserControls.Abstracts;
using RudycommerceWPF.WindowsAndUserControls.Login;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RudycommerceWPF.WindowsAndUserControls.Users
{
    /// <summary>
    /// Interaction logic for UserOverview.xaml
    /// </summary>
    public partial class UserOverview : MultilingualUserControl
    {
        private IDesktopUserRepository _userRepo;

        public CollectionViewSource ViewSource { get; set; }
        public ObservableCollection<DesktopUser> UserList { get; set; }

        public UserOverview()
        {
            InitializeComponent();

            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;
            SetLanguageDictionary();
            
            InitializeContent();
        }

        private async void InitializeContent()
        {
            DataContext = this;

            await LoadData();
        }

        private async Task LoadData()
        {
            _userRepo = new DesktopUserRepository();

            UserList = new ObservableCollection<DesktopUser>(
                (await _userRepo.GetAllAsync()).OrderByDescending(x => x.IsAdmin)
                                                .ThenBy(x => x.VerifiedByAdmin)
                                                .ThenBy(x => x.FullName));

            ViewSource = new CollectionViewSource();
            ViewSource.Source = UserList;

            dgDesktopUserOverview.ItemsSource = ViewSource.View;
            dgDesktopUserOverview.DataContext = UserList;

            BindData();
        }

        private void BindData()
        {
            ViewSource.View.Refresh();
        }

        private void RefreshGrid(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void MakeAdmin(object sender, RoutedEventArgs e)
        {
            try
            {
                SetLanguageDictionary();

                DesktopUser newAdmin = ((FrameworkElement)sender).DataContext as DesktopUser;

                string messageboxContent = String.Format(LangResource.MBContentMakeUserAdmin, newAdmin.FullName);
                string messageboxTitle = String.Format(LangResource.MBTitleMakeUserAdmin, newAdmin.FullName);

                MessageBoxManager.Yes = LangResource.Yes;
                MessageBoxManager.No = LangResource.No;
                MessageBoxManager.Register();

                if (MessageBox.Show(messageboxContent,
                                    messageboxTitle,
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Warning)
                    == MessageBoxResult.Yes)
                {
                    _userRepo.AssignNewAdmin(newAdmin);
                    _userRepo.SaveChangesAsync();

                    LoginWindow login = new LoginWindow();
                    login.Show();

                    NavigationWindow win = (NavigationWindow)Window.GetWindow(this);

                    win.Close();

                    MessageBoxManager.Unregister();
                }
                else
                { MessageBoxManager.Unregister(); }
            }
            catch (Exception)
            {
                MessageBox.Show(LangResource.ErrUpdateOverviewFailed);
                MessageBoxManager.Unregister();
            }            
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            try
            {
                SetLanguageDictionary();

                var user = ((FrameworkElement)sender).DataContext as DesktopUser;

                string messageboxContent = String.Format(LangResource.MBContentDeleteObj, LangResource.TheEmployee.ToLower(), user.FullName);
                string messageboxTitle = String.Format(LangResource.MBTitleDeleteObj, user.FullName);

                MessageBoxManager.Yes = LangResource.Yes;
                MessageBoxManager.No = LangResource.No;
                MessageBoxManager.Register();

                if (MessageBox.Show(messageboxContent,
                                    messageboxTitle,
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Warning)
                    == MessageBoxResult.Yes)
                {
                    MessageBoxManager.Unregister();

                    _userRepo.Delete(user);
                    _userRepo.SaveChangesAsync();
                    UserList.Remove(user);
                }
                else
                { MessageBoxManager.Unregister(); }
            }
            catch (Exception)
            {
                MessageBox.Show(LangResource.ErrUpdateOverviewFailed);
                MessageBoxManager.Unregister();
            }
        }

        private void Verify(object sender, RoutedEventArgs e)
        {
            try
            {
                SetLanguageDictionary();

                var user = ((FrameworkElement)sender).DataContext as DesktopUser;

                string messageboxContent = String.Format(LangResource.MBContentVerifyUser, user.FullName);
                string messageboxTitle = String.Format(LangResource.MBTitleVerifyUser, user.FullName);

                MessageBoxManager.Yes = LangResource.Yes;
                MessageBoxManager.No = LangResource.No;
                MessageBoxManager.Register();

                if (MessageBox.Show(messageboxContent,
                                    messageboxTitle,
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Warning)
                    == MessageBoxResult.Yes)
                {
                    MessageBoxManager.Unregister();

                    user.VerifiedByAdmin = true;
                    _userRepo.Update(user);
                    _userRepo.SaveChangesAsync();

                    BindData() ;
                }
                else
                { MessageBoxManager.Unregister(); }
            }
            catch (Exception)
            {
                MessageBox.Show(LangResource.ErrUpdateOverviewFailed);
                MessageBoxManager.Unregister();
            }
        }
    }
}
