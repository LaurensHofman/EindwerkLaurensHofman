using MahApps.Metro.Controls;
using RudycommerceData.Entities;
using RudycommerceData.Entities.Products.Categories;
using RudycommerceData.Entities.Products.Products;
using RudycommerceData.Entities.Products.Specifications;
using RudycommerceData.Models;
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
using System.Windows.Controls.Primitives;
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
        //
        // TODO Messagebox on changing category in update/create
        // TODO Changing back and forth between tabs
        //
        
        private readonly int _defaultHeight = 30;
        private readonly int _defaultWidth = 300;
        private readonly int _descriptionHeight = 200;

        public Product ProductModel { get; set; }

        public List<Category> CategoryList { get; set; }
        public List<Brand> BrandsList { get; set; }

        private List<NecessarySpecification> _necessarySpecList;
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

            _prodRepo = new ProductRepository();

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

        private TextBox AddBindedTextBox(object bindingSource, string bindingLocation, Panel parentElement)
        {
            TextBox tb = new TextBox
            {
                Style = Application.Current.Resources["FormInputTextBox"] as Style,
                Width = _defaultWidth
            };
            Binding tbBinding = new Binding(bindingLocation)
            {
                Source = bindingSource
            };
            tb.SetBinding(TextBox.TextProperty, tbBinding);

            parentElement.Children.Add(tb);

            return tb;
        }

        private ComboBox AddBindedComboBox(object bindingSource, string bindingLocation, Panel parentElement, int specID)
        {
            // using the style defined in styles.xaml is not entirely working, so doing it manually

            ComboBox cb = new ComboBox
            {
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 20, 0, 0),
                Height = _defaultHeight,
                Width = _defaultWidth,
                Padding = new Thickness(0, 5, 0, 5),
                DisplayMemberPath = "LocalizedValue",
                SelectedValuePath = "ID"
            };

            Specification spec = _specificationsList.SingleOrDefault(x => x.ID == specID);
            List<SpecificationEnum> enums = spec.Enumerations.ToList();

            foreach (var e in enums)
            {
                e.LocalizedValue = e.LocalizedEnumValues.SingleOrDefault(x => x.LanguageID == _preferredLanguage.ID).Value;
            }

            cb.SetBinding(ItemsControl.ItemsSourceProperty,
                new Binding
                {
                    Source = enums
                });

            cb.SetBinding(
               Selector.SelectedValueProperty,
               new Binding(bindingLocation)
               {
                   Source = bindingSource
               });

            NonMLStackRightInput.Children.Add(cb);

            return cb;
        }

        private CheckBox AddBindedCheckBox(object bindingSource, string bindingLocation, Panel parentElement)
        {
            // styles is not working, so it was added manually here

            CheckBox cb = new CheckBox
            {
                Width = _defaultWidth,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0,20,0,0),
                Height = _defaultHeight,
                Padding = new Thickness(0,-5,0,-5)
            };
            Binding cbBinding = new Binding(bindingLocation)
            {
                Source = bindingSource
            };
            cb.SetBinding(CheckBox.IsCheckedProperty, cbBinding);

            parentElement.Children.Add(cb);

            return cb;
        }

        private Label AddFormLabel(string content, Panel parentElement)
        {
            Label lbl = new Label
            {
                Content = content,
                Style = Application.Current.Resources["FormLabel"] as Style
            };
            parentElement.Children.Add(lbl);

            return lbl;
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
                    i.BorderThickness = new Thickness(1, 0, 1, 0);
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

        private void EnableTabs(params TabItem[] tabItems)
        {
            foreach (TabItem tb in tabItems)
            {
                tb.IsEnabled = true;

                WrapPanel wp = (WrapPanel)tb.Header;

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

                tb.BorderBrush = Brushes.Black;
            }
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
                    tab.BorderThickness = new Thickness(1,0,1,0);
                }
            }
        }

        #region GeneralTab

        private async void GenerateProductNameLabelsAndInputs()
        {
            _languageList = await _langRepo.GetAllAsync();

            foreach (var lang in _languageList)
            {
                string labelContent = $"\"{lang.LocalName}\" {LangResource.Name} * : ";

                AddFormLabel(labelContent, GeneralNameLabels);

                LocalizedProduct locProd = ProductModel.LocalizedProducts.SingleOrDefault(x => x.LanguageID == lang.ID);

                if (locProd == null)
                {
                    locProd = new LocalizedProduct
                    {
                        LanguageID = lang.ID
                    };
                    ProductModel.LocalizedProducts.Add(locProd);
                }

                AddBindedTextBox(locProd, "Name", GeneralNameInputs);
            }
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

        private void cmbxCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Category cat = cmbxCategories.SelectedItem as Category;

            _necessarySpecList = new List<NecessarySpecification>();

            foreach (var catSpec in cat.CategorySpecifications)
            {
                Specification spec = _specificationsList.SingleOrDefault(x => x.ID == catSpec.SpecificationID);

                NecessarySpecification necSpec = new NecessarySpecification
                {
                    CategoryID = catSpec.CategoryID,
                    SpecificationID = catSpec.SpecificationID,
                    IsBool = spec.IsBool,
                    IsEnumeration = spec.IsEnumeration,
                    IsMultilingual = spec.IsMultilingual,
                    LookupName = spec.LocalizedSpecifications.Single(x => x.LanguageID == _preferredLanguage.ID).LookupName,
                    DisplayOrder = catSpec.DisplayOrder
                };

                _necessarySpecList.Add(necSpec);
            }

            _necessarySpecList = _necessarySpecList.OrderBy(x => x.DisplayOrder).ToList();

            FillMultilingualTab();
            FillNonMultilingualTab();
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

        #endregion

        private void SubmitGeneral(object sender, RoutedEventArgs e)
        {
            if (ValidateGeneralTab())
            {
                EnableTabs(tabItemGeneral, tabItemMultilingualProperties);

                tabItemMultilingualProperties.IsSelected = true;
            }
            else
            {
                MessageBox.Show("BeepBoop");
            }
        }

        private bool ValidateGeneralTab()
        {
            foreach (var locProd in ProductModel.LocalizedProducts)
            {
                if (string.IsNullOrWhiteSpace(locProd.Name))
                {
                    return false;
                }
            }

            if (ProductModel.CategoryID <= 0)
            {
                return false;
            }

            if (ProductModel.BrandID <= 0)
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

        #endregion

        #region MLTab

        private void FillMultilingualTab()
        {
            TabControlLanguages.Items.Clear();

            ProductModel.Values_ProductSpecifications = new List<Value_ProductSpecification>();

            foreach (var lang in _languageList)
            {
                CreateMLLocalizedTab(lang);
            }
        }

        private void CreateMLLocalizedTab(Language lang)
        {
            MetroTabItem tabItem = CreateMetroTabItem(lang);
            Grid tabGrid = new Grid { Style = Application.Current.Resources["GridBelowTabItem"] as Style };
            tabGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            tabGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            StackPanel stackPanelLeft = CreateLeftStackPanelForLabels(lang);
            tabGrid.Children.Add(stackPanelLeft);
            Grid.SetColumn(stackPanelLeft, 0);

            StackPanel stackPanelRight = CreateRightStackPanelForInput(lang);
            tabGrid.Children.Add(stackPanelRight);
            Grid.SetColumn(stackPanelRight, 1);

            tabItem.Content = tabGrid;

            TabControlLanguages.Items.Add(tabItem);
        }

        private StackPanel CreateRightStackPanelForInput(Language lang)
        {
            StackPanel stackInput = new StackPanel();

            LocalizedProduct lp = ProductModel.LocalizedProducts.SingleOrDefault(x => x.LanguageID == lang.ID);

            TextBox tbDescription = AddBindedTextBox(lp, "Description", stackInput);
            tbDescription.Height = _descriptionHeight;
            tbDescription.TextWrapping = TextWrapping.Wrap;
            tbDescription.AcceptsReturn = true;
            tbDescription.VerticalContentAlignment = VerticalAlignment.Top;
            tbDescription.Padding = new Thickness(0, 5, 0, 5);

            foreach (var spec in
                _necessarySpecList.Where(ns => ns.IsMultilingual == true && ns.IsEnumeration == false).OrderBy(ns => ns.DisplayOrder))
            {
                Value_ProductSpecification val = new Value_ProductSpecification
                {
                    SpecificationID = spec.SpecificationID,
                    LanguageID = lang.ID
                };
                ProductModel.Values_ProductSpecifications.Add(val);

                AddBindedTextBox(val, "Value", stackInput);
            }

            return stackInput;
        }

        private StackPanel CreateLeftStackPanelForLabels(Language lang)
        {
            StackPanel stackLabels = new StackPanel();

            Label lblDescription = AddFormLabel(LangResource.Description + " * : ", stackLabels);
            lblDescription.Margin = new Thickness(0, 20, 0, _descriptionHeight - _defaultHeight);


            foreach (var spec in
                _necessarySpecList.Where(ns => ns.IsMultilingual == true && ns.IsEnumeration == false).OrderBy(ns => ns.DisplayOrder))
            {
                AddFormLabel(spec.LookupName + " * : ", stackLabels);
            }

            return stackLabels;
        }

        private MetroTabItem CreateMetroTabItem(Language lang)
        {
            MetroTabItem metroTab = new MetroTabItem
            {
                Header = lang.LocalName,
                Name = $"tab{lang.ID}",
                Padding = new Thickness { Left = 25, Right = 25 },
                Background = Brushes.Beige
            };

            // Creates a new style for the new tabItem
            Style AutoGeneratedTabItem = new Style
            {
                TargetType = typeof(MetroTabItem)
            };

            // Adds a setter to give the tabItem border
            Setter SelectedStyle = new Setter
            {
                Property = TabItem.BorderThicknessProperty,
                Value = new Thickness { Top = 2, Bottom = 2, Left = 2, Right = 2 }
            };

            // Adds a new trigger, for when the tabItem is selected
            Trigger SelectedTrigger = new Trigger
            {
                Property = TabItem.IsSelectedProperty,
                Value = true
            };

            // When the tabItem gets selected, it will generate borders, to clearly see which one is selected
            SelectedTrigger.Setters.Add(SelectedStyle);


            // The same as above for when it is selected, but now to revert the change when unselected
            Setter NotSelectedStyle = new Setter
            {
                Property = TabItem.BorderThicknessProperty,
                Value = new Thickness { Top = 0, Bottom = 0, Left = 1, Right = 1 }
            };
            Trigger NotSelectedTrigger = new Trigger
            {
                Property = TabItem.IsSelectedProperty,
                Value = false
            };
            NotSelectedTrigger.Setters.Add(NotSelectedStyle);

            AutoGeneratedTabItem.Triggers.Add(SelectedTrigger);
            AutoGeneratedTabItem.Triggers.Add(NotSelectedTrigger);

            AutoGeneratedTabItem.Setters.Add(new Setter() { Property = ControlsHelper.HeaderFontSizeProperty, Value = 18.0 });
            metroTab.Style = AutoGeneratedTabItem;

            return metroTab;
        }

        private void SubmitML(object sender, RoutedEventArgs e)
        {
            if (ValidateMLTab())
            {
                EnableTabs(tabItemGeneral, tabItemMultilingualProperties, tabItemNonMultilingualProperties);

                tabItemNonMultilingualProperties.IsSelected = true;
            }
            else
            {
                MessageBox.Show("RIPBoop");
            }
        }

        private bool ValidateMLTab()
        {
            foreach (var spec in
                _necessarySpecList.Where(ns => ns.IsMultilingual == true && ns.IsEnumeration == false))
            {
                foreach (var valSpec in ProductModel.Values_ProductSpecifications.Where(x => x.SpecificationID == spec.SpecificationID))
                {
                    if (valSpec == null)
                    {
                        return false;
                    }

                    if (String.IsNullOrWhiteSpace(valSpec.Value))
                    {
                        return false;
                    }
                }
            }

            foreach (var lp in ProductModel.LocalizedProducts)
            {
                if (String.IsNullOrWhiteSpace(lp.Description))
                {
                    return false;
                }
            }

            return true;
        }

        private void BackToGeneral(object sender, RoutedEventArgs e)
        {
            tabItemGeneral.IsSelected = true;
        }

        #endregion

        #region NonMLTab
        
        private void FillNonMultilingualTab()
        {
            NonMLStackLeftLabels.Children.Clear();
            NonMLStackRightInput.Children.Clear();
            
            foreach (var spec in _necessarySpecList.Where(ns => ns.IsMultilingual == false || ns.IsEnumeration == true).OrderBy(x => x.DisplayOrder))
            {
                Value_ProductSpecification val = new Value_ProductSpecification
                {
                    SpecificationID = spec.SpecificationID,
                    LanguageID = null
                };

                if (spec.IsBool)
                {
                    AddFormLabel(spec.LookupName + " * : ", NonMLStackLeftLabels);
                    AddBindedCheckBox(val, "Value", NonMLStackRightInput);
                }
                else
                {
                    if (spec.IsEnumeration)
                    {
                        if (spec.IsMultilingual)
                        {
                            AddFormLabel(spec.LookupName + " * : ", NonMLStackLeftLabels);
                            AddBindedComboBox(val, "SpecificationEnumID", NonMLStackRightInput, val.SpecificationID);
                        }
                        else
                        {
                            AddFormLabel(spec.LookupName + " * : ", NonMLStackLeftLabels);
                            AddBindedComboBox(val, "SpecificationEnumID", NonMLStackRightInput, val.SpecificationID);
                        }
                    }
                    else
                    {
                        AddFormLabel(spec.LookupName + " * : ", NonMLStackLeftLabels);
                        AddBindedTextBox(val, "Value", NonMLStackRightInput);
                    }
                }

                ProductModel.Values_ProductSpecifications.Add(val);
            }
        }        

        private void BackToML(object sender, RoutedEventArgs e)
        {
            tabItemMultilingualProperties.IsSelected = true;
        }

        #endregion

        protected override async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_updatingPage)
                {

                }
                else
                {
                    PrepareModelForSaving();

                    await _prodRepo.AddWithImagesAsync(ProductModel);

                    TriggerSaveEvent();
                }
            }
            catch (Exception)
            {
                string content = String.Format(LangResource.MBContentObjSaveFailed, LangResource.TheProduct.ToLower());
                string title = StringExtensions.FirstCharToUpper(String.Format(LangResource.MBTitleObjSaveFailed, LangResource.Product.ToLower()));

                MessageBox.Show(content, title);
            }
        }

        private void PrepareModelForSaving()
        {
            ProductModel.CurrentStock = (int)ProductModel.InitialStock;

            List<Value_ProductSpecification> tempList = new List<Value_ProductSpecification>();

            // TODO Comment this mess holy moly

            foreach (var val in ProductModel.Values_ProductSpecifications.Where(x => x.LanguageID == null))
            {
                bool isFirstLanguage = true;

                foreach (var lang in _languageList)
                {
                    if (isFirstLanguage)
                    {
                        val.LanguageID = lang.ID;
                    }
                    else
                    {
                        tempList.Add(
                            new Value_ProductSpecification
                            {
                                LanguageID = lang.ID,
                                SpecificationID = val.SpecificationID,
                                Value = val.Value,
                                SpecificationEnum = val.SpecificationEnum
                            });
                    }

                    isFirstLanguage = false;
                }
            }

            foreach (var tempItem in tempList)
            {
                ProductModel.Values_ProductSpecifications.Add(tempItem);
            }

            ProductModel.Values_ProductSpecifications = ProductModel.Values_ProductSpecifications.OrderBy(x => x.SpecificationID).ToList();
        }
    }
}