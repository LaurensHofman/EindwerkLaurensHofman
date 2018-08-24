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

            // Shows the image belonging to the brand
            ShowImage(brand.LogoURL);

            InitializeWindow();
        }

        private void InitializeWindow()
        {
            // Defines the progress bar and submit button, to allow the ProgressBar methods to work (FormUserControl)
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
            // Sets the content by referencing to the localized string in the dictionary
            lblTitle.SetResourceReference(ContentProperty, _updatingPage ? "UpdateBrandTitle" : "NewBrandTitle");
        }

        private void AddRemoveImage(object sender, RoutedEventArgs e)
        {
            // If the brand has no image yet, open a filedialog to select one
            if (BrandModel.LocalLogoPath == null && BrandModel.LogoURL == null)
            {
                try
                {
                    // Opens a filedialog filtered on image files
                    Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog
                    {
                        Filter = "Image File (*.jpg; *.png)| *.jpg; *.png"
                    };

                    // Checks if a file was chosen
                    bool? result = fileDialog.ShowDialog();

                    // If a file was chosen
                    if (result == true)
                    {
                        // Put the file path into the BrandModel
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
                    MessageBox.Show(LangResource.AddImageFailed);
                    BrandModel.LogoURL = null;
                    BrandModel.LocalLogoPath = null;
                }
            }
            else
            {
                HideImage();
            }
        }

        /// <summary>
        /// Hides the image control, and shows an add image button
        /// </summary>
        private void HideImage()
        {
            BrandModel.LocalLogoPath = null;
            BrandModel.LogoURL = null;

            // Makes the image control empty and hides it
            LogoImage.Source = null;
            LogoImage.Visibility = Visibility.Collapsed;

            // Hides the remove image button
            RemoveImageButton.Visibility = Visibility.Collapsed;

            // Shows the add image button
            AddImageButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Shows the image control, and shows a remove image button
        /// </summary>
        /// <param name="filePath"></param>
        private void ShowImage(string filePath)
        {
            // Shows the image
            LogoImage.Source = new BitmapImage(new Uri(filePath));
            LogoImage.Visibility = Visibility.Visible;

            // Shows the remove image button
            RemoveImageButton.Visibility = Visibility.Visible;

            // Hides the add image button
            AddImageButton.Visibility = Visibility.Collapsed;
        }

        private async Task SaveModel()
        {
            try
            {
                if (_updatingPage)
                {
                    // Updates the brand, including the image
                    await _brandRepo.UpdateAsyncWithImage(BrandModel);
                }
                else
                {
                    // Creates the brand, including the image
                    await _brandRepo.AddAsyncWithImage(BrandModel);
                    Visibility = Visibility.Collapsed;
                }

                await _brandRepo.SaveChangesAsync();
                TriggerSaveEvent();
            }
            catch (Exception)
            {
                MessageBox.Show(LangResource.ErrSaveFailedContent, LangResource.ErrSaveFailedTitle);
            }            
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
