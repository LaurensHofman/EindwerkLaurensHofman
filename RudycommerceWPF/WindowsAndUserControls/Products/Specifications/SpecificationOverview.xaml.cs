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

            SetLanguageDictionary();

            LoadDataGridData();

            DataContext = this;
        }

        private void BindData()
        {
            dgSpecificationOverview.ItemsSource = SpecList;
            dgSpecificationOverview.DataContext = SpecList;
        }

        public override async void LoadDataGridData()
        {
            _specRepo = new SpecificationRepository();

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
            {

                throw;
            }
        }

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
