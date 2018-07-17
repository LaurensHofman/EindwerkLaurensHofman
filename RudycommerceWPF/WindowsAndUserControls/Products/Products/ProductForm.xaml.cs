using RudycommerceData.Entities;
using RudycommerceData.Entities.Products.Categories;
using RudycommerceData.Entities.Products.Products;
using RudycommerceData.Entities.Products.Specifications;
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

        private List<CategorySpecification> _catSpecList;
        private List<Specification> _specificationsList;
        private List<Language> _languageList { get; set; }

        private ILanguageRepository _langRepo;
        private ISpecificationRepository _specRepo;
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
                LocalizedProducts = new List<LocalizedProduct>(),
                Images = new List<ProductImage>()
            };

            _langRepo = new LanguageRepository();

            GenerateProductNameLabelsAndInputs();

            GetSpecificationList();

            FillCategoryDropdown();
            FillBrandDropdown();
        }

        private async void GetSpecificationList()
        {
            _specRepo = new SpecificationRepository();
            _specificationsList = await _specRepo.GetAllAsync();
        }

        private async void FillBrandDropdown()
        {
            _brandRepo = new BrandRepository();

            BrandsList = (await _brandRepo.GetAllAsync()).OrderBy(x => x.Name).ToList();
        }

        private async void FillCategoryDropdown()
        {
            _catRepo = new CategoryRepository();

            CategoryList = (await _catRepo.GetAllAsync());

            foreach (var cat in CategoryList)
            {
                cat.LocalizedName = cat.LocalizedCategories.SingleOrDefault(x => x.LanguageID == _preferredLanguage.ID).Name;
            }

            CategoryList = CategoryList.OrderBy(x => x.LocalizedName).ToList();
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

        private void cmbxCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Category cat = cmbxCategories.SelectedItem as Category;

            _catSpecList = cat.CategorySpecifications.ToList();

            foreach (var catSpec in _catSpecList)
            {
                Specification spec = _specificationsList.SingleOrDefault(x => x.ID == catSpec.SpecificationID);

                catSpec.SpecificationName = spec.LocalizedSpecifications.Single(x => x.LanguageID == _preferredLanguage.ID).LookupName;
            }

            _catSpecList = _catSpecList.OrderBy(x => x.DisplayOrder).ToList();
        }
        
        #region Generating, Drag&Drop and Removing images

        Grid gridImageToDrag;

        private void AddImage(object sender, RoutedEventArgs e)
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

                    CreateImageControls(filename);

                    ProductImage prdImg = new ProductImage
                    {
                        FileLocation = filename,
                        Order = imgPnl.Children.Count - 1
                    };
                    ProductModel.Images.Add(prdImg);
                }
            }

            catch (Exception)
            {
                // TODO
                throw;
            }

        }

        private void CreateImageControls(string filename)
        {
            Grid grd = new Grid
            {
                Margin = new Thickness(10, 0, 10, 10)
            };

            Border brd = new Border
            {
                BorderThickness = new Thickness(0),
                BorderBrush = Brushes.Transparent,
                Margin = new Thickness(10)
            };

            Image img = new Image
            {
                Width = 160,
                Height = 160,
                Stretch = Stretch.Fill,
                AllowDrop = true,
                VerticalAlignment = VerticalAlignment.Stretch,
                Source = new BitmapImage(new Uri(filename))
            };
            img.DragEnter += Image_DragEnter;
            img.MouseLeftButtonDown += Image_MouseLeftButtonDown;

            brd.Child = img;
            grd.Children.Add(brd);

            Image btnImage = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/64_GarbageCanWhite.png"))
            };

            Button btn = new Button
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Height = 60,
                Width = 60,
                Content = btnImage
            };
            btn.Click += DeleteImage;

            grd.Children.Add(btn);

            imgPnl.Children.Add(grd);

            if (imgPnl.Children.IndexOf(grd) == 0)
            {
                brd.BorderThickness = new Thickness(2);
                brd.BorderBrush = Brushes.Black;
            }
        }

        private void DeleteImage(object sender, RoutedEventArgs e)
        {
            Button sourceButton = (Button)e.Source;

            Grid grd = (Grid)sourceButton.Parent;

            int removedIndex = imgPnl.Children.IndexOf(grd);

            ProductModel.Images.Remove(ProductModel.Images.ToList()[removedIndex]);

            for (int i = removedIndex; i < ProductModel.Images.Count; i++)
            {
                ProductModel.Images.ToList()[i].Order -= 1;
            }

            imgPnl.Children.Remove(grd);
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            gridImageToDrag = (Grid)((Border)((Image)e.Source).Parent).Parent;
            DragDrop.DoDragDrop(gridImageToDrag, gridImageToDrag, DragDropEffects.Move);
        }

        private void Image_DragEnter(object sender, DragEventArgs e)
        {
            Grid grdImg = (Grid)((Border)((Image)e.Source).Parent).Parent;

            int where_to_drop = imgPnl.Children.IndexOf(grdImg);
            int initial_location = imgPnl.Children.IndexOf(gridImageToDrag);

            imgPnl.Children.Remove(gridImageToDrag);
            imgPnl.Children.Insert(where_to_drop, gridImageToDrag);


            int deltaLocation = Math.Abs(where_to_drop - initial_location);

            if (deltaLocation == 1 || deltaLocation == 0)
            {
                ProductImage temporary = ProductModel.Images.Single(x => x.Order == initial_location);
                ProductImage temporary2 = ProductModel.Images.Single(x => x.Order == where_to_drop);

                temporary.Order = where_to_drop;
                temporary2.Order = initial_location;
            }
            else
            {
                if (initial_location < where_to_drop)
                {
                    ProductImage draggedImage = ProductModel.Images.Single(x => x.Order == initial_location);

                    for (int i = initial_location + 1; i <= where_to_drop; i++)
                    {
                        ProductModel.Images.Single(x => x.Order == i).Order = i - 1;
                    }

                    draggedImage.Order = where_to_drop;
                }
                else
                {
                    ProductImage draggedImage = ProductModel.Images.Single(x => x.Order == initial_location);

                    for (int i = initial_location - 1; i >= where_to_drop; i--)
                    {
                        ProductModel.Images.Single(x => x.Order == i).Order = i + 1;
                    }

                    draggedImage.Order = where_to_drop;
                }
            }

            ProductModel.Images = ProductModel.Images.OrderBy(x => x.Order).ToList();

            foreach (Grid grd in imgPnl.Children)
            {
                Border brd = (Border)grd.Children[0];
                brd.BorderBrush = (imgPnl.Children.IndexOf(grd) == 0) ? Brushes.Black : Brushes.Transparent;
                brd.BorderThickness = new Thickness((imgPnl.Children.IndexOf(grd) == 0) ? 2 : 0);
            }
        }

        protected override void btnSave_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private void SubmitGeneral(object sender, RoutedEventArgs e)
        {
            if (ValidateGeneralTab())
            {
                tabItemMultilingualProperties.IsSelected = true;
                tabItemMultilingualProperties.IsEnabled = true;

                WrapPanel wp = (WrapPanel)tabItemMultilingualProperties.Header;

                foreach (var child in wp.Children)
                {
                    if (child is TextBlock)
                    {
                        ((TextBlock)child).Foreground = Brushes.Black;
                    }
                    else
                    {
                        if (child is Border)
                        {
                            ((Border)child).BorderBrush = Brushes.Black;
                            ((TextBlock)((Border)child).Child).Foreground = Brushes.Black;
                        }
                    }
                }

                tabItemMultilingualProperties.BorderBrush = Brushes.Black;
            }            
        }

        private bool ValidateGeneralTab()
        {
            foreach (var locProd in ProductModel.LocalizedProducts)
            {
                if (locProd.Name == null)
                {
                    return false;
                }
            }

            if (ProductModel.Category == null)
            {
                return false;
            }

            if (ProductModel.Brand == null)
            {
                return false;
            }

            if (ProductModel.UnitPrice == null || ProductModel.UnitPrice <= 0)
            {
                return false;
            }

            if (ProductModel.InitialStock == null || ProductModel.InitialStock < 0)
            {
                return false;
            }

            if (ProductModel.Images.Count <= 0)
            {
                return false;
            }

            return true;
        }

        private void AnimatedTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var tabItem in AnimatedTabControl.Items)
            {
                var tab = (TabItem)tabItem;

                if (tab.IsSelected)
                {
                    tab.BorderThickness = new Thickness(1);
                }
                else
                {
                    tab.BorderThickness = new Thickness(0);
                }
            }
        }
    }
}