using RudycommerceData.Entities;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceLib.Properties;
using RudycommerceWPF.WindowsAndUserControls.Abstracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace RudycommerceWPF.WindowsAndUserControls.Languages
{
    /// <summary>
    /// Interaction logic for LanguageOverview.xaml
    /// </summary>
    public partial class LanguageOverview : OverviewUserControl
    {
        private ILanguageRepository _langRepo;

        public ObservableCollection<Language> LanguageList { get; set; }

        public LanguageOverview()
        {
            InitializeComponent();

            InitializeWindow();
        }

        private void InitializeWindow()
        {
            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;

            SetLanguageDictionary();
            
            DataContext = this;

            LoadDataGridData();
        }

        public override async Task LoadDataGridData()
        {
            _langRepo = new LanguageRepository();

            LanguageList = new ObservableCollection<Language>(await _langRepo.GetAllAsync());            

            BindData();
        }

        private void BindData()
        {
            LanguageList = new ObservableCollection<Language>(LanguageList.OrderByDescending(x => x.IsDefault).ThenByDescending(x => x.IsDesktopLanguage).ThenBy(x => x.ISO));

            dgLanguageOverview.ItemsSource = LanguageList;
            //.OrderByDescending(x => x.IsDefault).ThenByDescending(x => x.IsDesktopLanguage).ThenBy(x => x.ISO);
            dgLanguageOverview.DataContext = LanguageList;
            //.OrderByDescending(x => x.IsDefault).ThenByDescending(x => x.IsDesktopLanguage).ThenBy(x => x.ISO);
        }
        
        private async void dgLanguageOverview_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            DataGridRow _dgRow = e.Row;
            
            Language _changedValue = _dgRow.DataContext as Language;

            if (_changedValue.ISO.Length != 2)
            {                
                LoadDataGridData();
            }
            else
            {
                await _langRepo.UpdateAsync(_changedValue);
                await _langRepo.SaveChangesAsync();
            }
        }

        protected async override void Delete(object sender, RoutedEventArgs e)
        {
            try
            {
                Language ToBeDeleted = ((FrameworkElement)sender).DataContext as Language;

                string messageboxTitle = String.Format(LangResource.MBTitleDeleteObj, ToBeDeleted.LocalName);
                string messageboxContent = String.Format(LangResource.MBContentDeleteObj, LangResource.TheLanguage.ToLower(), ToBeDeleted.LocalName);

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

                    _langRepo.Delete(ToBeDeleted);
                    LanguageList.Remove(ToBeDeleted);

                    await _langRepo.SaveChangesAsync();

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

        protected override void Update(object sender, RoutedEventArgs e)
        {
            Language lang = ((FrameworkElement)sender).DataContext as Language;

            ShowUpdateForm<LanguageForm>(lang.ID);
        }

        private async void MakeLangDefault(object sender, RoutedEventArgs e)
        {
            try
            {
                Language newDefault = ((FrameworkElement)sender).DataContext as Language;

                string messageboxTitle = String.Format(LangResource.MBTitleMakeLangDefault, newDefault.LocalName);
                string messageboxContent = String.Format(LangResource.MBContentNewDefaultLang, newDefault.LocalName);

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

                    await _langRepo.SwapDefaultLanguages(newDefault);

                    await _langRepo.SaveChangesAsync();

                    LoadDataGridData();
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

            ContentControl form = myWindow.ccLanguageForm;
            ContentControl ov = myWindow.ccLanguageOverview;

            form.Content = null;

            myWindow.ShowFormUserControl<LanguageForm, LanguageOverview>(form, ov);
        }
    }
}
