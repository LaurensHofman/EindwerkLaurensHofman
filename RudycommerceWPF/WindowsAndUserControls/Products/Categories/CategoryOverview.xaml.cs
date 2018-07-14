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

        private void InitializeWindow()
        {
            DataContext = this;

            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;
            SetLanguageDictionary();

            _catRepo = new CategoryRepository();

            CategoryList = new ObservableCollection<CategoryOverviewItem>(_catRepo.GetCategoryOverview(_preferredLanguage.ID));

            BindData();
        }

        private void BindData()
        {
            CategoryList.OrderBy(c => c.LocalizedName);

            dgCategoryOverview.ItemsSource = CategoryList;
            dgCategoryOverview.DataContext = CategoryList;
        }

        public override void LoadDataGridData()
        {
            _catRepo = new CategoryRepository();

            CategoryList = new ObservableCollection<CategoryOverviewItem>(_catRepo.GetCategoryOverview(_preferredLanguage.ID));

            BindData();
        }
        
        protected override void OpenForm(object sender, RoutedEventArgs e)
        {
            var myWindow = (NavigationWindow)GetParentWindow();

            ContentControl form = myWindow.ccCategoryForm;
            ContentControl ov = myWindow.ccCategoryOverview;

            form.Content = null;

            myWindow.ShowFormUserControl<CategoryForm, CategoryOverview>(form, ov);
        }

        protected override void Update(object sender, RoutedEventArgs e)
        {
            CategoryOverviewItem cat = ((FrameworkElement)sender).DataContext as CategoryOverviewItem;

            ShowUpdateForm<CategoryForm>(cat.ID);
        }

        protected override async void Delete(object sender, RoutedEventArgs e)
        {
            try
            {
                CategoryOverviewItem ToBeDeleted = ((FrameworkElement)sender).DataContext as CategoryOverviewItem;

                string messageboxTitle = String.Format(LangResource.MBTitleDeleteObj, ToBeDeleted.LocalizedName);
                string messageboxContent = String.Format(LangResource.MBContentDeleteObj, LangResource.TheCategory.ToLower(), ToBeDeleted.LocalizedName);

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

                    _catRepo.Delete(ToBeDeleted.ID);
                    CategoryList.Remove(ToBeDeleted);

                    await _catRepo.SaveChangesAsync();

                    BindData();
                }
                else
                { MessageBoxManager.Unregister(); }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
