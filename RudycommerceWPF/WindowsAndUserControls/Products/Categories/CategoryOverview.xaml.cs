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

namespace RudycommerceWPF.WindowsAndUserControls.Products.Categories
{
    /// <summary>
    /// Interaction logic for CategoryOverview.xaml
    /// </summary>
    public partial class CategoryOverview : OverviewUserControl
    {
        public ObservableCollection<CategoryOverviewItem> CategoryList { get; set; }

        private ICategoryRepository _catRepo;

        public CategoryOverview()
        {
            InitializeComponent();

            InitializeWindow();
        }

        private async void InitializeWindow()
        {
            DataContext = this;

            // Sets the display language
            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;
            SetLanguageDictionary();

            await LoadDataGridData();

            BindData();
        }

        private void BindData()
        {
            // Refreshes the datagrid

            CategoryList.OrderBy(c => c.LocalizedName);

            dgCategoryOverview.ItemsSource = CategoryList;
            dgCategoryOverview.DataContext = CategoryList;
        }

        public override async Task LoadDataGridData()
        {
            // Gets all the categories

            _catRepo = new CategoryRepository();

            CategoryList = new ObservableCollection<CategoryOverviewItem>(_catRepo.GetCategoryOverview(_preferredLanguage.ID));

            BindData();
        }
        
        protected override void OpenForm(object sender, RoutedEventArgs e)
        {
            // Opens opens a Create form within the same window
            var myWindow = (NavigationWindow)GetParentWindow();

            ContentControl form = myWindow.ccCategoryForm;
            ContentControl ov = myWindow.ccCategoryOverview;

            form.Content = null;

            myWindow.ShowFormUserControl<CategoryForm, CategoryOverview>(form, ov);
        }

        protected override void Update(object sender, RoutedEventArgs e)
        {
            // Opens an update form in a popup window

            CategoryOverviewItem cat = ((FrameworkElement)sender).DataContext as CategoryOverviewItem;

            ShowUpdateForm<CategoryForm>(cat.ID);
        }

        protected override void Updated()
        {
            // After the update event, make the product form refresh (because the product is dependent on categories)
            base.Updated();

            var win = (NavigationWindow)GetParentWindow();
            win.ccProductForm.Content = null;
        }

        protected override async void Delete(object sender, RoutedEventArgs e)
        {
            try
            {
                // Gets the item that has to be deleted
                CategoryOverviewItem ToBeDeleted = ((FrameworkElement)sender).DataContext as CategoryOverviewItem;

                // Create custom message box

                string messageboxTitle = String.Format(LangResource.MBTitleDeleteObj, ToBeDeleted.LocalizedName);
                string messageboxContent = String.Format(LangResource.MBContentDeleteObj, LangResource.TheCategory.ToLower(), ToBeDeleted.LocalizedName);

                MessageBoxManager.Yes = LangResource.Yes;
                MessageBoxManager.No = LangResource.No;
                MessageBoxManager.Register();

                // If people are sure to delete...
                if (MessageBox.Show(messageboxContent,
                                    messageboxTitle,
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Warning)
                    == MessageBoxResult.Yes)
                {
                    MessageBoxManager.Unregister();

                    // ... then delete the object
                    _catRepo.Delete(ToBeDeleted.ID);
                    CategoryList.Remove(ToBeDeleted);

                    await _catRepo.SaveChangesAsync();

                    // And refresh the datagrid
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
    }
}
