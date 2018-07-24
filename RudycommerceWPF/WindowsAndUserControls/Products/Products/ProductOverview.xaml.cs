using RudycommerceData.Entities.Products.Products;
using RudycommerceData.Models;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceLib.Properties;
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
using System.Windows.Forms;
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
        public CollectionViewSource ViewSource { get; set; }
        public ObservableCollection<ProductOverviewItem> ProductList { get; set; }

        private IProductRepository _prodRepo;

        public ProductOverview()
        {
            InitializeComponent();
            InitializeContent();
        }

        private async void InitializeContent()
        {
            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;

            SetLanguageDictionary();

            DataContext = this;

            LoadDataGridData();
        }

        public override async Task LoadDataGridData()
        {
            _prodRepo = new ProductRepository();

            ProductList = new ObservableCollection<ProductOverviewItem>(_prodRepo.GetProductOverview(_preferredLanguage.ID)
                .OrderBy(x => x.CategoryName)
                .ThenByDescending(x => x.IsActive)
                .ThenBy(x => x.ProductName));

            ViewSource = new CollectionViewSource();
            ViewSource.Source = ProductList;
            dgProductOverview.ItemsSource = ViewSource.View;
            dgProductOverview.DataContext = ProductList;

            BindData();
        }

        private void BindData()
        {
            ViewSource.View.Refresh();
        }

        protected override async void Delete(object sender, RoutedEventArgs e)
        {
            try
            {
                ProductOverviewItem ToBeDeleted = ((FrameworkElement)sender).DataContext as ProductOverviewItem;

                string messageboxTitle = String.Format(LangResource.MBTitleDeleteObj, ToBeDeleted.ProductName);
                string messageboxContent = String.Format(LangResource.MBContentDeleteObj, LangResource.TheProduct.ToLower(), ToBeDeleted.ProductName);

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

                    _prodRepo.Delete(ToBeDeleted.ID);
                    ProductList.Remove(ToBeDeleted);

                    await _prodRepo.SaveChangesAsync();

                    BindData();
                }
                else
                { MessageBoxManager.Unregister(); }
            }
            catch (Exception)
            {
                MessageBoxManager.Unregister();
                throw;
            }
        }

        /// <summary>
        /// Opens a new Create Form, but not as a popup
        /// </summary>
        protected override void OpenForm(object sender, RoutedEventArgs e)
        {
            var myWindow = (NavigationWindow)GetParentWindow();

            ContentControl form = myWindow.ccProductForm;
            ContentControl ov = myWindow.ccProductOverview;

            form.Content = null;

            myWindow.ShowFormUserControl<ProductForm, ProductOverview>(form, ov);
        }

        protected override void Update(object sender, RoutedEventArgs e)
        {
            ProductOverviewItem ToBeUpdated = ((FrameworkElement)sender).DataContext as ProductOverviewItem;

            ShowUpdateForm<ProductForm>(ToBeUpdated.ID);
        }

        /// <summary>
        /// Shows/Hides the images column
        /// </summary>
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
            // gets the item, changes the IsActive and saves it

            ProductOverviewItem item = (ProductOverviewItem)((FrameworkElement)sender).DataContext;

            _prodRepo.ToggleProductActive(item.ID);
            await _prodRepo.SaveChangesAsync();

            item.IsActive = !item.IsActive;

            BindData();
        }

        /// <summary>
        /// Opens a small window to prompt for the added stock value
        /// </summary>
        private void AddStock(object sender, RoutedEventArgs e)
        {
            var dialog = new MyDialog(LangResource.Cancel, LangResource.Submit, LangResource.AddStockTitle);
            if (dialog.ShowDialog() == true)
            {
                if (int.TryParse(dialog.ResponseText, out int response))
                {
                    if (response > 0)
                    {
                        var product = ((FrameworkElement)sender).DataContext as ProductOverviewItem;

                        Product toBeUpdated = _prodRepo.Get(product.ID);

                        toBeUpdated.CurrentStock += response;
                        product.CurrentStock += response;

                        _prodRepo.Update(toBeUpdated);
                        _prodRepo.SaveChangesAsync();

                        BindData();
                    }
                }
            }
        }
    }
}
