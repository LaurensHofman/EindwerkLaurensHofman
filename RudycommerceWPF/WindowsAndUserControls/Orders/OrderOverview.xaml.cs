using RudycommerceData.Entities.Orders;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceWPF.WindowsAndUserControls.Abstracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RudycommerceWPF.WindowsAndUserControls.Orders
{
    /// <summary>
    /// Interaction logic for OrderOverview.xaml
    /// </summary>
    public partial class OrderOverview : MultilingualUserControl
    {
        private IIncOrderRepository _orderRepo;

        /// <summary>
        /// ViewSource for the PickingList
        /// </summary>
        public CollectionViewSource ViewSourcePickingList { get; set; }
        /// <summary>
        /// ViewSource for the Orders who are ready to be picked up by the courrier
        /// </summary>
        public CollectionViewSource ViewSourceToBePickedUp { get; set; }

        public ObservableCollection<RudycommerceData.Entities.Orders.IncomingOrder> IncOrderList { get; set; }

        public OrderOverview()
        {
            InitializeComponent();

            InitializeWindow();
        }

        private async void InitializeWindow()
        {
            DataContext = this;

            // Sets the preferred displaylanguage
            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;
            SetLanguageDictionary();

            await LoadDataGridData();
        }

        /// <summary>
        /// Loads the content of the datagrids
        /// </summary>
        /// <returns></returns>
        public async Task LoadDataGridData()
        {
            _orderRepo = new IncOrderRepository();

            // Gets all the incoming orders
            IncOrderList = new ObservableCollection<RudycommerceData.Entities.Orders.IncomingOrder>((await _orderRepo.GetAllAsync()));

            // Instanciate the ViewSource of the picking list datagrid
            ViewSourcePickingList = new CollectionViewSource()
            {
                Source = IncOrderList.Where(x => x.StatusCode == 0)
            };
            dgOrderOverview.ItemsSource = ViewSourcePickingList.View;
            dgOrderOverview.DataContext = IncOrderList;

            // Instanciate the ViewSource of the ready to be picked up orders datagrid
            ViewSourceToBePickedUp = new CollectionViewSource()
            {
                Source = IncOrderList.Where(x => x.StatusCode == 1)
            };
            dgOrderOverviewToBePickedUp.ItemsSource = ViewSourceToBePickedUp.View;
            dgOrderOverviewToBePickedUp.DataContext = IncOrderList;

            BindData();
        }

        /// <summary>
        /// Refreshes the ViewSources
        /// </summary>
        private void BindData()
        {
            ViewSourcePickingList.View.Refresh();
            ViewSourceToBePickedUp.View.Refresh();
        }

        /// <summary>
        /// Queries the database to refresh the incoming orders datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RefreshGrid(object sender, RoutedEventArgs e)
        {
            StartProgressBar();

            await LoadDataGridData();

            StopProgressBar();
        }

        private void StopProgressBar()
        {
            prog.IsIndeterminate = false;
            prog.Visibility = Visibility.Collapsed;
        }

        private void StartProgressBar()
        {
            prog.IsIndeterminate = true;
            prog.Visibility = Visibility.Visible;
        }

        private void AnimatedTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Triggers and setters were doing weird in XAML, so I gave the selected tab item a border manually here in code

            foreach (var tabItem in AnimatedTabControl.Items)
            {
                var tab = (TabItem)tabItem;

                if (tab.IsSelected)
                {
                    tab.BorderThickness = new Thickness(1);
                }
                else
                {
                    tab.BorderThickness = new Thickness(1, 0, 1, 0);
                }
            }
        }

        /// <summary>
        /// Shows a details popup of the incoming order
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowDetails(object sender, RoutedEventArgs e)
        {
            int detailsID = (((FrameworkElement)sender).DataContext as IncomingOrder).ID;

            var window = new OrderDetails(detailsID);
            window.OrderReadyEvent += OrderPutReady;
            window.Show();
        }

        /// <summary>
        /// Defines that the order is ready for pick up by the courrier
        /// </summary>
        /// <param name="id"></param>
        private async void OrderPutReady(int id)
        {
            var order = IncOrderList.FirstOrDefault(x => x.ID == id);

            if (order != null)
            {
                order = await _orderRepo.SetOrderAsReadyForPickup(id);

                await _orderRepo.SaveChangesAsync();

                BindData();
            }
        }
    }
}
