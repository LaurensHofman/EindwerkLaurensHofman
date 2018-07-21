using RudycommerceData.Entities.DesktopUsers;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceWPF.WindowsAndUserControls.Abstracts;
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

        public ObservableCollection<DesktopUser> UserList { get; set; }

        public UserOverview()
        {
            InitializeComponent();

            DataContext = this;

            GetData();
        }

        private async void GetData()
        {
            _userRepo = new DesktopUserRepository();

            UserList = new ObservableCollection<DesktopUser>(
                (await _userRepo.GetAllAsync()).OrderByDescending(x => x.IsAdmin)
                                                .ThenBy(x => x.VerifiedByAdmin)
                                                .ThenBy(x => x.FullName));

            dgDesktopUserOverview.ItemsSource = UserList;
            dgDesktopUserOverview.DataContext = UserList;
        }

        private void RefreshGrid(object sender, RoutedEventArgs e)
        {
            GetData();
        }

        private void MakeAdmin(object sender, RoutedEventArgs e)
        {

        }

        private void Delete(object sender, RoutedEventArgs e)
        {

        }

        private void Verify(object sender, RoutedEventArgs e)
        {

        }
    }
}
