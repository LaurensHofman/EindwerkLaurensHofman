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
        /// <summary>
        /// A viewsource can actually refresh on changes, whereas an observable collection will only change on deletes and creates.
        /// </summary>
        public CollectionViewSource ViewSource { get; set; }

        /// <summary>
        /// Collection containing all the products
        /// </summary>
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

            await LoadDataGridData();
        }

        public override async Task LoadDataGridData()
        {
            _prodRepo = new ProductRepository();

            // gets all the products
            ProductList = new ObservableCollection<ProductOverviewItem>(_prodRepo.GetProductOverview(_preferredLanguage.ID)
                .OrderBy(x => x.CategoryName)
                .ThenByDescending(x => x.IsActive)
                .ThenBy(x => x.ProductName));

            // Creates a viewsource for the product list
            ViewSource = new CollectionViewSource
            {
                Source = ProductList
            };

            // Binds the datagrid on the viewsource
            dgProductOverview.ItemsSource = ViewSource.View;
            dgProductOverview.DataContext = ProductList;

            // Refreshes the datagrid
            BindData();
        }

        /// <summary>
        /// Refreshes the DataGrid
        /// </summary>
        private void BindData()
        {
            ViewSource.View.Refresh();
        }

        protected override async void Delete(object sender, RoutedEventArgs e)
        {
            try
            {
                // Gets the product to be deleted
                ProductOverviewItem ToBeDeleted = ((FrameworkElement)sender).DataContext as ProductOverviewItem;

                string messageboxTitle = String.Format(LangResource.MBTitleDeleteObj, ToBeDeleted.ProductName);
                string messageboxContent = String.Format(LangResource.MBContentDeleteObj, LangResource.TheProduct.ToLower(), ToBeDeleted.ProductName);

                MessageBoxManager.Yes = LangResource.Yes;
                MessageBoxManager.No = LangResource.No;
                MessageBoxManager.Register();

                // Shows a messagebox with localized content to propmpt if the user is sure he wants to delete the product
                if (MessageBox.Show(messageboxContent,
                                    messageboxTitle,
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Warning)
                    == MessageBoxResult.Yes)
                {
                    MessageBoxManager.Unregister();

                    // Delete the product from the database and the ProductList
                    _prodRepo.Delete(ToBeDeleted.ID);
                    ProductList.Remove(ToBeDeleted);
                    await _prodRepo.SaveChangesAsync();

                    // Refreshes the grid
                    BindData();
                }
                else
                { MessageBoxManager.Unregister(); }
            }
            catch (Exception)
            {
                MessageBoxManager.Unregister();
                MessageBox.Show(LangResource.ErrUpdateOverviewFailed);
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
            // Toggles the visibility of the image column and toggles the text in the toggle button
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

        /// <summary>
        /// Opens a small window to prompt for the added stock value
        /// </summary>
        private void AddStock(object sender, RoutedEventArgs e)
        {
            // Creates a new dialig, with custom cancel text, submit text and add stock title text
            var dialog = new MyDialog(LangResource.Cancel, LangResource.Submit, LangResource.AddStockTitle);

            // If the submit buttonn was clicked in the dialog
            if (dialog.ShowDialog() == true)
            {
                // Try to parse the value to an integer
                if (int.TryParse(dialog.ResponseText, out int response))
                {
                    // If the result is bigger than 0, add the response to the stock of the product, and update the CurrentStock.
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
