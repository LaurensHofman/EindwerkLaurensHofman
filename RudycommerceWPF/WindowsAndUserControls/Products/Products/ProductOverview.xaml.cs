using RudycommerceData.Models;
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

namespace RudycommerceWPF.WindowsAndUserControls.Products.Products
{
    /// <summary>
    /// Interaction logic for ProductOverview.xaml
    /// </summary>
    public partial class ProductOverview : OverviewUserControl
    {
        public ObservableCollection<ProductOverviewItem> ProductList { get; set; }

        private IProductRepository _prodRepo;

        public ProductOverview()
        {
            InitializeComponent();

            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;

            SetLanguageDictionary();

            DataContext = this;

            LoadDataGridData();
        }

        public override void LoadDataGridData()
        { 
            _prodRepo = new ProductRepository();

            ProductList = new ObservableCollection<ProductOverviewItem>(_prodRepo.GetProductOverview(_preferredLanguage.ID));

            BindData();
        }

        private void BindData()
        {
            ProductList.OrderBy(x => x.ProductName);

            dgProductOverview.ItemsSource = ProductList;
            dgProductOverview.DataContext = ProductList;
        }

        protected override void Delete(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OpenForm(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void Update(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ToggleVisibilityImages(object sender, RoutedEventArgs e)
        {
            if (ImageColumn.Visibility == Visibility.Collapsed)
            {
                ImageColumn.Visibility = Visibility.Visible;
                ((Button)e.Source).SetResourceReference(ContentProperty, "HideProductImages");
            }
            else
            {
                ImageColumn.Visibility = Visibility.Collapsed;
                ((Button)e.Source).SetResourceReference(ContentProperty, "ShowProductImages");
            }
        }

        private async void ToggleProductActive(object sender, RoutedEventArgs e)
        {
            ProductOverviewItem item = (ProductOverviewItem)((FrameworkElement)sender).DataContext;

            _prodRepo.ToggleProductActive(item.ID);
            await _prodRepo.SaveChangesAsync();

            item.IsActive = !item.IsActive;

            // for some reason, datagrid wasn't showing the updated isActive toggle;

            LoadDataGridData();

            //BindData();
        }
    }
}
