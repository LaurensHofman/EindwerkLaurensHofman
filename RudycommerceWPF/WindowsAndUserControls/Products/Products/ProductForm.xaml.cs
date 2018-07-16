using RudycommerceData.Entities;
using RudycommerceData.Entities.Products.Categories;
using RudycommerceData.Entities.Products.Products;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceLib.Properties;
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

namespace RudycommerceWPF.WindowsAndUserControls.Products.Products
{
    /// <summary>
    /// Interaction logic for ProductForm.xaml
    /// </summary>
    public partial class ProductForm : FormUserControl
    {
        public Product ProductModel { get; set; }

        public List<Category> CategoryList { get; set; }
        public List<Brand> BrandsList { get; set; }

        private List<Language> _languageList { get; set; }

        private ILanguageRepository _langRepo;
        private IProductRepository _prodRepo;
        private IBrandRepository _brandRepo;
        private ICategoryRepository _catRepo;

        public ProductForm()
        {
            InitializeComponent();

            DataContext = this;

            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;

            SetLanguageDictionary();

            SetTitle();

            TabItemColours();

            ProductModel = new Product
            {
                LocalizedProducts = new List<LocalizedProduct>()
            };

            _langRepo = new LanguageRepository();

            GenerateProductNameLabelsAndInputs();

            FillCategoryDropdown();
            FillBrandDropdown();
        }

        private async void FillBrandDropdown()
        {
            _brandRepo = new BrandRepository();

            BrandsList = await _brandRepo.GetAllAsync();
        }

        private async void FillCategoryDropdown()
        {
            _catRepo = new CategoryRepository();

            CategoryList = await _catRepo.GetAllAsync();

            foreach (var cat in CategoryList)
            {
                cat.LocalizedName = cat.LocalizedCategories.SingleOrDefault(x => x.LanguageID == _preferredLanguage.ID).Name;
            }
        }

        private async void GenerateProductNameLabelsAndInputs()
        {
            _languageList = await _langRepo.GetAllAsync();

            foreach (var lang in _languageList)
            {
                string labelContent = $"\"{lang.LocalName}\" {LangResource.Name} * : ";

                AddFormLabel(lang, labelContent, GeneralNameLabels);


                LocalizedProduct locProd = ProductModel.LocalizedProducts.SingleOrDefault(x => x.LanguageID == lang.ID);

                if (locProd == null)
                {
                    locProd = new LocalizedProduct();
                    ProductModel.LocalizedProducts.Add(locProd);
                }

                AddBindedTextBox(locProd, "Name", GeneralNameInputs);
            }
        }

        private void AddBindedTextBox(object bindingSource, string bindingLocation, Panel parentElement)
        {
            TextBox tb = new TextBox
            {
                Style = Application.Current.Resources["FormInputTextBox"] as Style,
                Width = 300
            };
            Binding tbBinding = new Binding(bindingLocation)
            {
                Source = bindingSource
            };
            tb.SetBinding(TextBox.TextProperty, tbBinding);

            parentElement.Children.Add(tb);
        }

        private void AddFormLabel(Language lang, string content, Panel parentElement)
        {
            Label lbl = new Label
            {
                Content = content,
                Style = Application.Current.Resources["FormLabel"] as Style
            };
            parentElement.Children.Add(lbl);
        }

        private void TabItemColours()
        {
            foreach (var item in AnimatedTabControl.Items)
            {
                var i = item as TabItem;

                if (AnimatedTabControl.Items.IndexOf(item) == 0)
                {
                    i.BorderThickness = new Thickness(1, 1, 1, 1);
                }
                else
                {
                    var wp = (i.Header as WrapPanel);

                    var brd = wp.Children[0] as Border;
                    var tbnr = brd.Child as TextBlock;

                    var tbtxt = wp.Children[1] as TextBlock;

                    brd.BorderBrush = Brushes.Gray;
                    tbnr.Foreground = Brushes.Gray;
                    tbtxt.Foreground = Brushes.Gray;
                }
            }
        }

        protected override void SetTitle()
        {
            lblTitle.SetResourceReference(ContentProperty, _updatingPage ? "UpdateProductTitle" : "NewProductTitle");
        }

        protected override void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }
        private void cmbxCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show((cmbxBrands.SelectedItem as Category).LocalizedName);
        }

        private void AddImage(object sender, RoutedEventArgs e)
        {

        }

        private void cmbxBrands_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show((cmbxBrands.SelectedItem as Brand).Name);
        }
    }
}
