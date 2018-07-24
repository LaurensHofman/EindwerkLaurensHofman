using RudycommerceData.Entities.Products.Products;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceLib.Properties;
using RudycommerceLib.Utilities;
using RudycommerceWPF.WindowsAndUserControls.Abstracts;
using System;
using System.Collections.Generic;
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

namespace RudycommerceWPF.WindowsAndUserControls.Products.Brands
{
    /// <summary>
    /// Interaction logic for BrandForm.xaml
    /// </summary>
    public partial class BrandForm : FormUserControl
    {
        public Brand BrandModel { get; set; }

        private IBrandRepository _brandRepo;

        public BrandForm()
        {
            InitializeComponent();

            BrandModel = new Brand();

            InitializeWindow();
        }

        public BrandForm(int ID)
        {
            InitializeComponent();

            _brandRepo = new BrandRepository();

            Brand brand = _brandRepo.Get(ID);

            BrandModel = brand;

            ShowImage(brand.LogoURL);

            InitializeWindow();
        }

        private void InitializeWindow()
        {
            progressBar = prog;
            submitButton = btnSubmit;

            _updatingPage = !BrandModel.IsNew();

            _brandRepo = new BrandRepository();

            DataContext = this;

            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;

            SetLanguageDictionary();

            SetTitle();
        }

        protected override void SetTitle()
        {
            lblTitle.SetResourceReference(ContentProperty, _updatingPage ? "UpdateBrandTitle" : "NewBrandTitle");
        }

        private void AddRemoveImage(object sender, RoutedEventArgs e)
        {
            if (BrandModel.LocalLogoPath == null && BrandModel.LogoURL == null)
            {
                try
                {
                    Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog
                    {
                        Filter = "Image File (*.jpg; *.png)| *.jpg; *.png"
                    };

                    bool? result = fileDialog.ShowDialog();

                    if (result == true)
                    {
                        string filename = fileDialog.FileName;

                        BrandModel.LocalLogoPath = filename;

                        ShowImage(filename);
                    }
                    else
                    {
                        // TODO
                    }
                }
                catch (Exception)
                {
                    // TODO
                    throw;
                }
            }
            else
            {
                HideImage();
            }
        }

        private void HideImage()
        {
            BrandModel.LocalLogoPath = null;
            BrandModel.LogoURL = null;
            LogoImage.Source = null;
            LogoImage.Visibility = Visibility.Collapsed;

            RemoveImageButton.Visibility = Visibility.Collapsed;

            AddImageButton.Visibility = Visibility.Visible;
        }

        private void ShowImage(string filePath)
        {
            LogoImage.Source = new BitmapImage(new Uri(filePath));
            LogoImage.Visibility = Visibility.Visible;

            RemoveImageButton.Visibility = Visibility.Visible;

            AddImageButton.Visibility = Visibility.Collapsed;
        }

        private async Task SaveModel()
        {
            if (_updatingPage)
            {
                await _brandRepo.UpdateAsyncWithImage(BrandModel);
                // TODO WITH IMAGE
            }
            else
            {
                await _brandRepo.AddAsyncWithImage(BrandModel);
                Visibility = Visibility.Collapsed;
            }           

            await _brandRepo.SaveChangesAsync();
            TriggerSaveEvent();
        }

        private bool Validate()
        {
            // TODO

            return true;
        }

        protected override async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TurnOnProgressBar();

                if (Validate())
                {
                    await SaveModel();
                }
                else
                {
                    // TODO Message
                }

                TurnOffProgressBar();
            }
            catch (Exception)
            {
                TurnOffProgressBar();

                string content = String.Format(LangResource.MBContentObjSaveFailed, LangResource.TheBrand.ToLower());
                string title = StringExtensions.FirstCharToUpper(String.Format(LangResource.MBTitleObjSaveFailed, LangResource.Brand.ToLower()));

                MessageBox.Show(content, title);
            }
        }
    }
}
