using RudycommerceData.Entities.Products.Products;
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

namespace RudycommerceWPF.WindowsAndUserControls.Products.Brands
{
    /// <summary>
    /// Interaction logic for BrandOverview.xaml
    /// </summary>
    public partial class BrandOverview : OverviewUserControl
    {
        private IBrandRepository _brandRepo;
        public ObservableCollection<Brand> BrandsList { get; set; }

        public BrandOverview()
        {
            InitializeComponent();

            InitializeWindow();
        }

        private async void InitializeWindow()
        {
            // Sets the display language
            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;

            SetLanguageDictionary();

            DataContext = this;

            await LoadDataGridData();
        }


        public override async Task LoadDataGridData()
        {
            _brandRepo = new BrandRepository();

            BrandsList = new ObservableCollection<Brand>(await _brandRepo.GetAllAsync());

            BindData();
        }

        /// <summary>
        /// Refreshes the datagrid
        /// </summary>
        private void BindData()
        {

            BrandsList = new ObservableCollection<Brand>( BrandsList.OrderBy(x => x.Name)) ;

            dgBrandsOverview.ItemsSource = BrandsList;
            //.OrderBy(x => x.Name);
            dgBrandsOverview.DataContext = BrandsList;
            //.OrderBy(x => x.Name);
        }

        private async void dgBrandsOverview_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            // Gets the row and the corresponding brand
            DataGridRow _dgRow = e.Row;
            Brand _changedValue = _dgRow.DataContext as Brand;

            try
            {
                // Updates
                await _brandRepo.UpdateAsync(_changedValue);
                await _brandRepo.SaveChangesAsync();
            }
            catch (Exception)
            {
                MessageBox.Show(LangResource.ErrUpdateOverviewFailed);
            }            
        }
        
        protected override async void Delete(object sender, RoutedEventArgs e)
        {
            try
            {
                // Gets the brand to delete

                Brand ToBeDeleted = ((FrameworkElement)sender).DataContext as Brand;

                string messageboxTitle = String.Format(LangResource.MBTitleDeleteObj, ToBeDeleted.Name);
                string messageboxContent = String.Format(LangResource.MBContentDeleteObj, LangResource.TheBrand.ToLower(), ToBeDeleted.Name);

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

                    // Deletes the brand

                    _brandRepo.Delete(ToBeDeleted);
                    BrandsList.Remove(ToBeDeleted);

                    await _brandRepo.SaveChangesAsync();

                    // Refreshes the datagrid
                    BindData();
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

        protected override void Update(object sender, RoutedEventArgs e)
        {
            // Gets the brand
            Brand brand = ((FrameworkElement)sender).DataContext as Brand;

            // Shows a brand update form in popup
            ShowUpdateForm<BrandForm>(brand.ID);
        }

        /// <summary>
        /// Method called when the update in the popup has been submitted
        /// </summary>
        protected override void Updated()
        {
            base.Updated();

            // This makes the productForm refresh (see Navigation window)
            var win = (NavigationWindow)GetParentWindow();
            win.ccProductForm.Content = null;
        }

        /// <summary>
        /// Opens a create form for a brand in the same window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OpenForm(object sender, RoutedEventArgs e)
        {
            var myWindow = (NavigationWindow)GetParentWindow();

            ContentControl form = myWindow.ccBrandForm;
            ContentControl ov = myWindow.ccBrandOverview;

            form.Content = null;

            myWindow.ShowFormUserControl<BrandForm, BrandOverview>(form, ov);
        }
    }
}
