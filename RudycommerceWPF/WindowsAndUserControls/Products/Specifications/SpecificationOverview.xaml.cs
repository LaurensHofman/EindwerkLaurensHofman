using RudycommerceData.Entities.Products.Specifications;
using RudycommerceData.Mapping;
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

namespace RudycommerceWPF.WindowsAndUserControls.Products.Specifications
{
    /// <summary>
    /// Interaction logic for SpecificationOverview.xaml
    /// </summary>
    public partial class SpecificationOverview : OverviewUserControl
    {
        private ISpecificationRepository _specRepo;

        public ObservableCollection<SpecificationOverviewItem> SpecList { get; set; }

        public SpecificationOverview()
        {
            InitializeComponent();

            InitializeWindow();
        }

        private void InitializeWindow()
        {
            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;
            InitializeContent();
        }

        private async void InitializeContent()
        {
            SetLanguageDictionary();

            await LoadDataGridData();

            DataContext = this;
        }

        /// <summary>
        /// Refreshes the datagrid
        /// </summary>
        private void BindData()
        {
            dgSpecificationOverview.ItemsSource = SpecList;
            dgSpecificationOverview.DataContext = SpecList;
        }

        /// <summary>
        /// Loads the data
        /// </summary>
        /// <returns></returns>
        public override async Task LoadDataGridData()
        {
            _specRepo = new SpecificationRepository();

            // Gets all the specifications, and maps it to objects fit to display in the datagrid
            SpecList = new ObservableCollection<SpecificationOverviewItem>
                (EntitiesMapping.MapToSpecificationOverviewItem(await _specRepo.GetAllAsync(), _preferredLanguage.ID)
                .OrderBy(x => x.SpecName).ToList()) ;

            BindData();
        }
        
        protected override void Update(object sender, RoutedEventArgs e)
        {
            SpecificationOverviewItem spec = ((FrameworkElement)sender).DataContext as SpecificationOverviewItem;

            ShowUpdateForm<SpecificationForm>(spec.ID);
        }

        protected override void Updated()
        {
            base.Updated();

            // If an update is done, forces the product form and category form to refresh on the next load, because products and categories are dependent on specifications
            var win = (NavigationWindow)GetParentWindow();
            win.ccProductForm.Content = null;
            win.ccCategoryForm.Content = null;
        }

        protected override async void Delete(object sender, RoutedEventArgs e)
        {
            try
            {
                SpecificationOverviewItem ToBeDeleted = ((FrameworkElement)sender).DataContext as SpecificationOverviewItem;

                string messageboxTitle = String.Format(LangResource.MBTitleDeleteObj, ToBeDeleted.SpecName);
                string messageboxContent = String.Format(LangResource.MBContentDeleteObj, LangResource.TheSpec.ToLower(), ToBeDeleted.SpecName);

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

                    _specRepo.Delete(ToBeDeleted.ID);
                    SpecList.Remove(ToBeDeleted);

                    await _specRepo.SaveChangesAsync();

                    BindData();
                }
                else
                { MessageBoxManager.Unregister(); }
            }
            catch (Exception)
            { MessageBoxManager.Unregister();
                MessageBox.Show(LangResource.ErrUpdateOverviewFailed);
            }
        }

        /// <summary>
        /// Opens a create form in the same window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OpenForm(object sender, RoutedEventArgs e)
        {
            var myWindow = (NavigationWindow)GetParentWindow();

            ContentControl form = myWindow.ccSpecificationForm;
            ContentControl ov = myWindow.ccSpecificationOverview;

            form.Content = null;

            myWindow.ShowFormUserControl<SpecificationForm, SpecificationOverview>(form, ov);
        }
    }
}
