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
        // TODO Put similarities constructors in a method
        //

        private readonly int _defaultHeight = 30;
        private readonly int _defaultWidth = 300;
        private readonly int _descriptionHeight = 200;

        public Product ProductModel { get; set; }

        public List<Category> CategoryList { get; set; }
        public List<Brand> BrandsList { get; set; }

        /// <summary>
        /// Specification necessary to fill in according to the category
        /// </summary>
        private List<NecessarySpecification> _necessarySpecList;
        /// <summary>
        /// List with all the specifications
        /// </summary>
        private List<Specification> _specificationsList;
        /// <summary>
        /// List with all the languages
        /// </summary>
        private List<Language> _languageList { get; set; }

        private ILanguageRepository _langRepo;
        private ISpecificationRepository _specRepo;
        private IProductRepository _prodRepo;
        private IBrandRepository _brandRepo;
        private ICategoryRepository _catRepo;

        public ProductForm()
        {
            InitializeComponent();

            // Defines the progress bar and submit button to let the ProgressBar methods work (see FormUserControl)
            progressBar = prog;
            submitButton = btnSubmit;

            DataContext = this;

            // Sets the display language
            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;
            SetLanguageDictionary();

            SetTitle();

            // Gives the tabs their appropriate colour (XAML didn't update their colours appropriatly)
            TabItemColours();

            _prodRepo = new ProductRepository();

            // Create new product
            ProductModel = new Product
            {
                LocalizedProducts = new List<LocalizedProduct>(),
                Images = new List<ProductImage>()
            };

            _langRepo = new LanguageRepository();
            _languageList = _langRepo.GetAll().OrderByDescending(x => x.IsDefault).ThenByDescending(x => x.IsDesktopLanguage).ToList();


            // Generate labels and input fields for the names for each language
            GenerateProductNameLabelsAndInputs();

            // Gets all the specifications
            GetSpecificationList();

            // Fills the category dropdown box
            FillCategoryDropdown();

            // Fills the brand dropdown
            FillBrandDropdown();
        }

        public ProductForm(int ID)
        {
            InitializeComponent();

            // Defines the progress bar and submit button for the ProgressBar methods to work (see FormUserControl)
            progressBar = prog;
            submitButton = btnSubmit;

            _updatingPage = true;

            // Disables the categories dropdown
            cmbxCategories.IsEnabled = false;

            // Initial stock shouldn't be editable in an update form
            lblInitStock.Visibility = tbInitStock.Visibility = Visibility.Collapsed;

            DataContext = this;

            // Sets the display language
            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;
            SetLanguageDictionary();

            // Gives the tabs their appropriate colour (XAML didn't update their colours appropriatly)
            TabItemColours();
            // Enable all tabs
            EnableTabs(tabItemGeneral, tabItemMultilingualProperties, tabItemNonMultilingualProperties);

            _prodRepo = new ProductRepository();

            // gets the product
            ProductModel = _prodRepo.Get(ID);

            _langRepo = new LanguageRepository();

            // Get all the languages in the database
            _languageList = _langRepo.GetAll().OrderByDescending(x => x.IsDefault).ThenByDescending(x => x.IsDesktopLanguage).ToList();

            SetTitle();

            // Generates the Name input fields and their labels
            GenerateProductNameLabelsAndInputs();

            // Gets all the specifications
            GetSpecificationList();

            // Fills the category dropdown
            FillCategoryDropdown();
            // Fills the brand dropdown
            FillBrandDropdown();

            // Adds the images based on the ones already belonging to the product
            AddImages();
        }

        private void AddImages()
        {
            // Foreach image in the product model, create the image controls, ordered by their display order
            foreach (var img in ProductModel.Images.OrderBy(x => x.Order))
            {
                AddImage(img, null);
            }
        }

        private async void GetSpecificationList()
        {
            _specRepo = new SpecificationRepository();
            _specificationsList = await _specRepo.GetAllAsync();
        }

        /// <summary>
        /// Adds a binded TextBox
        /// </summary>
        /// <param name="bindingSource">Object that contains a property that you want to bind on.</param>
        /// <param name="bindingLocation">The name of the property you want to bind on</param>
        /// <param name="parentElement">The XAML ParentElement where the textbox has to be put in</param>
        /// <returns></returns>
        private TextBox AddBindedTextBox(object bindingSource, string bindingLocation, Panel parentElement)
        {
            // Creates new textbox
            ClickSelectTextBox tb = new ClickSelectTextBox
            {
                Style = Application.Current.Resources["FormInputTextBox"] as Style,
                Width = _defaultWidth
            };
            // Adds binding and adds it to the textbox
            Binding tbBinding = new Binding(bindingLocation)
            {
                Source = bindingSource
            };
            tb.SetBinding(TextBox.TextProperty, tbBinding);

            // Adds the textbox to the parent element
            parentElement.Children.Add(tb);

            return tb;
        }

        /// <summary>
        /// Adds a binded ComboBox
        /// </summary>
        /// <param name="bindingSource">The object that contains the property that you want to bind the ComboBox on</param>
        /// <param name="bindingLocation">The name of the property that you want to bind the ComboBox on</param>
        /// <param name="parentElement">The XAML ParentElement in which the ComboBox has to be put in</param>
        /// <param name="specID">The specification ID, to find the appropriate Enumeration values to put in the ComboBox</param>
        /// <returns></returns>
        private ComboBox AddBindedComboBox(object bindingSource, string bindingLocation, Panel parentElement, int specID)
        {
            // using the style defined in styles.xaml is not entirely working, so doing it manually
            // Creates a comboBox, that displays the value, and selects the ID of the enumeration
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

            // Gets the specification based on the spec ID
            Specification spec = _specificationsList.SingleOrDefault(x => x.ID == specID);
            // Gets the enumerations of the specification
            List<SpecificationEnum> enums = spec.Enumerations.ToList();

            // Puts the localized value (based on the user's preferred display language) in a bindable property to show the text in the combobox
            foreach (var e in enums)
            {
                e.LocalizedValue = e.LocalizedEnumValues.SingleOrDefault(x => x.LanguageID == _preferredLanguage.ID).Value;
            }

            // Sets the Item source of the combobox to the enumeration values
            cb.SetBinding(ItemsControl.ItemsSourceProperty,
                new Binding
                {
                    Source = enums
                });

            // Binds the selected item (its ID) of the combobox to the Property of the BindingSource object
            cb.SetBinding(
               Selector.SelectedValueProperty,
               new Binding(bindingLocation)
               {
                   Source = bindingSource
               });

            // Adds the combobox to the parent element
            NonMLStackRightInput.Children.Add(cb);

            return cb;
        }

        /// <summary>
        /// Adds a binded CheckBox
        /// </summary>
        /// <param name="bindingSource">The object that contains the boolean property you want to bind the checkbox's value on</param>
        /// <param name="bindingLocation">The name of the boolean property you want to bind the checkbox's value on</param>
        /// <param name="parentElement">The XAML parent element that should contain the newly made checkbox</param>
        /// <returns></returns>
        private CheckBox AddBindedCheckBox(object bindingSource, string bindingLocation, Panel parentElement)
        {
            // styles is not working, so it was added manually here
            // Creates a new checkbox
            CheckBox cb = new CheckBox
            {
                Width = _defaultWidth,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 20, 0, 0),
                Height = _defaultHeight,
                Padding = new Thickness(0, -5, 0, -5)
            };
            // Creates a binding and adds it to the checkbox
            Binding cbBinding = new Binding(bindingLocation)
            {
                Source = bindingSource
            };
            cb.SetBinding(CheckBox.IsCheckedProperty, cbBinding);

            //Adds the checkbox to the parent element
            parentElement.Children.Add(cb);

            return cb;
        }

        /// <summary>
        /// Adds a form Label
        /// </summary>
        /// <param name="content">Content that the label has to show</param>
        /// <param name="parentElement">The XAML Parent element that will house the label</param>
        /// <returns></returns>
        private Label AddFormLabel(string content, Panel parentElement)
        {
            // Creates a new label, and adds it to the parent element
            Label lbl = new Label
            {
                Content = content,
                Style = Application.Current.Resources["FormLabel"] as Style
            };
            parentElement.Children.Add(lbl);

            return lbl;
        }

        /// <summary>
        /// Gives the tab items their appropriate colours (worked correctly through XAML in create window, but not in update window)
        /// </summary>
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

                i.Background = Brushes.Transparent;
            }
        }

        /// <summary>
        /// Sets the content of the title label
        /// </summary>
        protected override void SetTitle()
        {
            // Makes the content of the title label refer to a localized value of the dictionary
            lblTitle.SetResourceReference(ContentProperty, _updatingPage ? "UpdateProductTitle" : "NewProductTitle");
        }

        /// <summary>
        /// Enables/Disables the appropriate tabItems (to give it a feel of a lineair proces in which u can go back)
        /// </summary>
        /// <param name="tabItems"></param>
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

        /// <summary>
        /// Sets the border of the selected tab item (setter and trigger wasn't fully working in XAML)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    tab.BorderThickness = new Thickness(1, 0, 1, 0);
                }
            }
        }

        #region GeneralTab

        /// <summary>
        /// Generates the Labels and Input fields for the Name property for each language
        /// </summary>
        private void GenerateProductNameLabelsAndInputs()
        {
            // foreach language, make a textbox and label for the name input
            foreach (var lang in _languageList)
            {
                string labelContent = String.Format(LangResource.NameInLanguageX, lang.LocalName) + " * : ";

                AddFormLabel(labelContent, GeneralNameLabels);

                // Looks if the model already has a localized product for this language (can be the case when updating the product)
                LocalizedProduct locProd = ProductModel.LocalizedProducts.SingleOrDefault(x => x.LanguageID == lang.ID);

                if (locProd == null)
                {
                    // If the localized product doesn't exist yet, make a new one

                    locProd = new LocalizedProduct
                    {
                        LanguageID = lang.ID
                    };
                    ProductModel.LocalizedProducts.Add(locProd);
                }

                // Adds a textbox binded to the name
                AddBindedTextBox(locProd, "Name", GeneralNameInputs);
            }
        }

        /// <summary>
        /// Fills the dropdown for the Brands
        /// </summary>
        private async void FillBrandDropdown()
        {
            _brandRepo = new BrandRepository();

            BrandsList = (await _brandRepo.GetAllAsync()).OrderBy(x => x.Name).ToList();

            cmbxBrands.ItemsSource = BrandsList;
        }

        /// <summary>
        /// Fills the dropdown for the categories
        /// </summary>
        private async void FillCategoryDropdown()
        {
            _catRepo = new CategoryRepository();

            // Gets all the categories
            CategoryList = (await _catRepo.GetAllAsync());

            // Puts the localized value (based on the preferred display language of the user) in a seperate property to show in the combobox
            foreach (var cat in CategoryList)
            {
                cat.LocalizedName = cat.LocalizedCategories.SingleOrDefault(x => x.LanguageID == _preferredLanguage.ID).Name;
            }

            CategoryList = CategoryList.OrderBy(x => x.LocalizedName).ToList();

            cmbxCategories.ItemsSource = CategoryList;
        }

        /// <summary>
        /// Whenever the category changes, fill the input tabs with new inputs, according to the necessary specifications belonging to a category
        /// </summary>
        private void cmbxCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Gets the selected category
            Category cat = cmbxCategories.SelectedItem as Category;

            _necessarySpecList = new List<NecessarySpecification>();

            // Gets the specifications which are necessary according to the category and adds it to the necessary spec list
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

            // Fill the Multilingual tab and the NonMultilingual tab based on the necessary specifications
            FillMultilingualTab();
            FillNonMultilingualTab();
        }

        #region Generating images, Drag&Drop and Removing images

        /// <summary>
        /// The grid that the user is currently drag and dropping. (The grid contains an image, see CreateImageControls)
        /// </summary>
        Grid gridImageToDrag;

        private void AddImage(object sender, RoutedEventArgs e)
        {
            try
            {
                // If used from code, the type of sender will be of ProductImage

                if (sender.GetType() == typeof(ProductImage))
                {
                    // Creates an image control
                    CreateImageControls(((ProductImage)sender).ImageURL);
                }
                else
                {
                    // Opens a file dialog, to select an image                    

                    Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog
                    {
                        Filter = "Image File (*.jpg; *.png)| *.jpg; *.png"
                    };

                    // Checks if an image was selected in the dialog
                    bool? result = fileDialog.ShowDialog();
                    if (result == true)
                    {
                        // If an image was selected, get the fileLocation                        
                        string fileLocation = fileDialog.FileName;

                        // Creates the image controls
                        CreateImageControls(fileLocation);

                        // Adds a new object to the ProductImage list in the model
                        ProductImage prdImg = new ProductImage
                        {
                            FileLocation = fileLocation,
                            Order = imgPnl.Children.Count - 1
                        };
                        ProductModel.Images.Add(prdImg);
                    }
                }
            }

            catch (Exception)
            {
                MessageBox.Show(LangResource.AddImageFailed);
            }

        }

        /// <summary>
        /// Creates the controls to display the image and more
        /// </summary>
        /// <param name="filename">Path (local path or URL) to the image</param>
        private void CreateImageControls(string filename)
        {
            // Create a grid to house everything. This will be the object that can be dragged and dropped (selected by mouse events on the image)
            Grid grd = new Grid
            {
                Margin = new Thickness(10, 0, 10, 10)
            };

            // Creates a border and puts an image control in it. The border is needed if i wanted to show a border around the first (default) image
            Border brd = new Border
            {
                BorderThickness = new Thickness(0),
                BorderBrush = Brushes.Transparent,
                Margin = new Thickness(10)
            };
            // Creates an image
            Image img = new Image
            {
                Width = 160,
                Height = 160,
                Stretch = Stretch.Fill,
                AllowDrop = true,
                VerticalAlignment = VerticalAlignment.Stretch,
                Source = new BitmapImage(new Uri(filename))
            };
            // Defines mouse events for the image, to initiate a drag and drop.
            img.DragEnter += Image_DragEnter;
            img.MouseLeftButtonDown += Image_MouseLeftButtonDown;

            // Drag and MouseLeftButtonDown allows for the image to be moved. (to determine the display order of the images)

            brd.Child = img;
            grd.Children.Add(brd);

            // Adds a button with a trashcan image to delete the image
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

            // The Border containing the image, and the trashcan button are added to the grid. Now the grid can be added to the wrappanel
            imgPnl.Children.Add(grd);

            // Adds a border if the grid is the first one (to show whether it's the default image)
            if (imgPnl.Children.IndexOf(grd) == 0)
            {
                brd.BorderThickness = new Thickness(2);
                brd.BorderBrush = Brushes.Black;
            }
        }

        /// <summary>
        /// Delete the image (and the other controls who accompany it)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteImage(object sender, RoutedEventArgs e)
        {
            // Gets the Delete button which was clicked
            Button sourceButton = (Button)e.Source;

            // Finds the parent grid of the button
            Grid grd = (Grid)sourceButton.Parent;

            // Since the index of the grid within the wrappanel is equal to the index within the Model.Images list,
            // The image with the same index as the index of the grid within the wrappanel can be removed.
            int removedIndex = imgPnl.Children.IndexOf(grd);

            ProductModel.Images.Remove(ProductModel.Images.ToList()[removedIndex]);

            // Every image coming after the removed image, needs its DisplayOrder reduced by one
            for (int i = removedIndex; i < ProductModel.Images.Count; i++)
            {
                ProductModel.Images.ToList()[i].Order -= 1;
            }

            // Remove the image
            imgPnl.Children.Remove(grd);

            // Gives the first image a border, and all the other ones no border
            foreach (Grid grid in imgPnl.Children)
            {
                Border brd = (Border)grd.Children[0];
                brd.BorderBrush = (imgPnl.Children.IndexOf(grd) == 0) ? Brushes.Black : Brushes.Transparent;
                brd.BorderThickness = new Thickness((imgPnl.Children.IndexOf(grd) == 0) ? 2 : 0);
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // MouseLeftButtonDown on the image will look for its parent Grid (because the whole grid needs to be moved)
            // Image is inside a Border, which is inside a Grid.
            gridImageToDrag = (Grid)((Border)((Image)e.Source).Parent).Parent;

            // Default drag and drop function from WPF
            DragDrop.DoDragDrop(gridImageToDrag, gridImageToDrag, DragDropEffects.Move);
        }

        private void Image_DragEnter(object sender, DragEventArgs e)
        {
            // MouseLeftButtonDown on the image will look for its parent Grid (because the whole grid needs to be moved)
            // Image is inside a Border, which is inside a Grid.

            Grid grdImg = (Grid)((Border)((Image)e.Source).Parent).Parent;

            // Gets the 2 locations that got swapped.
            int where_to_drop = imgPnl.Children.IndexOf(grdImg);
            int initial_location = imgPnl.Children.IndexOf(gridImageToDrag);

            // Removes the image from the first position, and inserts it at the new location
            imgPnl.Children.Remove(gridImageToDrag);
            imgPnl.Children.Insert(where_to_drop, gridImageToDrag);

            // Gets the difference between the 2 location indices
            int deltaLocation = Math.Abs(where_to_drop - initial_location);

            // If the difference is 1, the 2 images get swapped (since they are next to each other)
            if (deltaLocation == 1 || deltaLocation == 0)
            {
                ProductImage temporary = ProductModel.Images.Single(x => x.Order == initial_location);
                ProductImage temporary2 = ProductModel.Images.Single(x => x.Order == where_to_drop);

                temporary.Order = where_to_drop;
                temporary2.Order = initial_location;
            }
            else
            {
                // If the difference is larger than 1, all the images in between need their DisplayOrder changed

                if (initial_location < where_to_drop)
                {
                    // If the image gets moved from left to right (with a difference larger than 1)
                    // Then: all the images in between those positions, are moved one position to the left (meaning their DisplayOrder gets reduced by one)

                    ProductImage draggedImage = ProductModel.Images.Single(x => x.Order == initial_location);

                    for (int i = initial_location + 1; i <= where_to_drop; i++)
                    {
                        ProductModel.Images.Single(x => x.Order == i).Order = i - 1;
                    }

                    draggedImage.Order = where_to_drop;
                }
                else
                {
                    // If the image gets moved from right to left (with a difference larger than 1)
                    // Then: all the images in between those positions, are moved one position to the right (meaning their DisplayOrder gets increased by one)

                    ProductImage draggedImage = ProductModel.Images.Single(x => x.Order == initial_location);

                    for (int i = initial_location - 1; i >= where_to_drop; i--)
                    {
                        ProductModel.Images.Single(x => x.Order == i).Order = i + 1;
                    }

                    draggedImage.Order = where_to_drop;
                }
            }

            // Images list gets ordered by DisplayOrder, so they align with the order of the displayed images

            ProductModel.Images = ProductModel.Images.OrderBy(x => x.Order).ToList();

            // Gives the first image (= default image) a border, and the other ones no border
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
            // If everything is correctly filled in, in the first tab, enable the second tab and open it
            if (ValidateGeneralTab())
            {
                EnableTabs(tabItemGeneral, tabItemMultilingualProperties);

                tabItemMultilingualProperties.IsSelected = true;
            }
            else
            {
                MessageBox.Show(LangResource.ProdFormGeneralTabInvalid);
            }
        }

        private bool ValidateGeneralTab()
        {
            // Checks whether the name for each language is filled.
            foreach (var locProd in ProductModel.LocalizedProducts)
            {
                if (string.IsNullOrWhiteSpace(locProd.Name))
                {
                    return false;
                }
            }

            // Checks whether a category was chosen
            if (ProductModel.CategoryID <= 0)
            {
                return false;
            }

            // Checks whether a brand was chosen
            if (ProductModel.BrandID <= 0)
            {
                return false;
            }

            // Checks whether a unit price is correctly filled in
            if (ProductModel.UnitPrice == null || ProductModel.UnitPrice <= 0)
            {
                return false;
            }

            // Checks whether an initial stock is correctly filled in
            if (ProductModel.InitialStock == null || ProductModel.InitialStock < 0)
            {
                return false;
            }

            // Checks whether at least one image was added
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
            // Clears the tab
            TabControlLanguages.Items.Clear();

            // Creates a new list of values for the product
            ProductModel.Values_ProductSpecifications = new List<Value_ProductSpecification>();

            // Creates a tab for each language
            foreach (var lang in _languageList)
            {
                // Creates a tab for language sensitive specifications
                CreateMLLocalizedTab(lang);
            }
        }

        private void CreateMLLocalizedTab(Language lang)
        {
            // Creates a tab item
            MetroTabItem tabItem = CreateMetroTabItem(lang);

            // Creates a grid for under the tab item, with 2 columns with equal width;
            Grid tabGrid = new Grid { Style = Application.Current.Resources["GridBelowTabItem"] as Style };
            tabGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            tabGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Creates a stackpanel for the labels, and is put in the left column of the grid
            StackPanel stackPanelLeft = CreateLeftStackPanelForLabels(lang);
            tabGrid.Children.Add(stackPanelLeft);
            Grid.SetColumn(stackPanelLeft, 0);

            // Creates a stackpanel for the input fields, and is put in the right column of the grid
            StackPanel stackPanelRight = CreateRightStackPanelForInput(lang);
            tabGrid.Children.Add(stackPanelRight);
            Grid.SetColumn(stackPanelRight, 1);

            tabItem.Content = tabGrid;

            TabControlLanguages.Items.Add(tabItem);
        }

        private StackPanel CreateRightStackPanelForInput(Language lang)
        {
            StackPanel stackInput = new StackPanel();

            // Gets the localized product to bind the description to.
            // Because the localized product already exists (because they were made/checked to generate the Name labels and input)
            // , we don't have to check whether they exist

            LocalizedProduct lp = ProductModel.LocalizedProducts.First(x => x.LanguageID == lang.ID);

            // Creates a textbox to bind the description to.
            // Because the Description textbox is a multiline textbox, some extra changes are made to the textbox.

            TextBox tbDescription = AddBindedTextBox(lp, "Description", stackInput);
            tbDescription.Height = _descriptionHeight;
            tbDescription.TextWrapping = TextWrapping.Wrap;
            tbDescription.AcceptsReturn = true;
            tbDescription.VerticalContentAlignment = VerticalAlignment.Top;
            tbDescription.Padding = new Thickness(0, 5, 0, 5);


            // Foreach specification (belonging to the selected category) that is multilingual (and not a enumeration, which would give a combobox)
            // Generate a label and a textbox
            foreach (var spec in
                _necessarySpecList.Where(ns => ns.IsMultilingual == true && ns.IsEnumeration == false).OrderBy(ns => ns.DisplayOrder))
            {
                Value_ProductSpecification val = ProductModel.Values_ProductSpecifications
                    .SingleOrDefault(x => x.SpecificationID == spec.SpecificationID && x.LanguageID == lang.ID);

                // Checks whether the specification already exists within the product (in case of updating a product)

                if (val == null)
                {
                    // If there was no value yet, create one and adds it to the list
                    val = new Value_ProductSpecification
                    {
                        SpecificationID = spec.SpecificationID,
                        LanguageID = lang.ID
                    };
                    ProductModel.Values_ProductSpecifications.Add(val);
                }

                // Adds a textbox because the multilingual inputs will always need a textbox
                AddBindedTextBox(val, "Value", stackInput);
            }

            return stackInput;
        }

        private StackPanel CreateLeftStackPanelForLabels(Language lang)
        {
            StackPanel stackLabels = new StackPanel();

            // Creates a label for the description property
            Label lblDescription = AddFormLabel(LangResource.Description + " * : ", stackLabels);
            lblDescription.Margin = new Thickness(0, 20, 0, _descriptionHeight - _defaultHeight);

            // Creates a label for each specification made in the 'right stackpanel for input'
            foreach (var spec in
                _necessarySpecList.Where(ns => ns.IsMultilingual == true && ns.IsEnumeration == false).OrderBy(ns => ns.DisplayOrder))
            {
                AddFormLabel(spec.LookupName + " * : ", stackLabels);
            }

            return stackLabels;
        }

        private MetroTabItem CreateMetroTabItem(Language lang)
        {
            // Creates a tab item
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

        /// <summary>
        /// Submits the multilingual tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitML(object sender, RoutedEventArgs e)
        {
            // Checks whether the multilingual tab is filled in correctly
            if (ValidateMLTab())
            {
                // Enables all tabs (reached last step of the lineair process) and navigates to the last tab
                EnableTabs(tabItemGeneral, tabItemMultilingualProperties, tabItemNonMultilingualProperties);
                tabItemNonMultilingualProperties.IsSelected = true;
            }
            else
            {
                MessageBox.Show(LangResource.ProdFormMLTabInvalid);
            }
        }

        private bool ValidateMLTab()
        {
            // Checks whether the product has its values filled in for each specification that is multilingual (those in the Multilingual tab)

            // Loops through every spec that is multilingual
            foreach (var spec in
                _necessarySpecList.Where(ns => ns.IsMultilingual == true && ns.IsEnumeration == false))
            {
                // Loops through every value that belongs to the multilingual spec
                foreach (var valSpec in ProductModel.Values_ProductSpecifications.Where(x => x.SpecificationID == spec.SpecificationID))
                {
                    // If its object doesn't exist or the value is null or whitespace, return a negative validation result
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

            // Checks if all descriptions are filled in
            foreach (var lp in ProductModel.LocalizedProducts)
            {
                if (String.IsNullOrWhiteSpace(lp.Description))
                {
                    return false;
                }
            }

            // Reaching here means there is no negative validation result
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
            // Clears the stackpanels containing the labels and input fields
            NonMLStackLeftLabels.Children.Clear();
            NonMLStackRightInput.Children.Clear();

            // Foreach specification that does not require a textbox for different languages, make an appropriate input element and bind it to it.

            foreach (var spec in _necessarySpecList.Where(ns => ns.IsMultilingual == false || ns.IsEnumeration == true).OrderBy(x => x.DisplayOrder))
            {                
                // Creates a value for each language
                foreach (var lang in _languageList)
                {
                    Value_ProductSpecification val = ProductModel.Values_ProductSpecifications
                    .SingleOrDefault(x => x.SpecificationID == spec.SpecificationID && x.LanguageID == lang.ID);

                    if (val == null)
                    {
                        val = new Value_ProductSpecification()
                        {
                            LanguageID = lang.ID,
                            SpecificationID = spec.SpecificationID,
                            ProductID = ProductModel.ID
                        };
                        ProductModel.Values_ProductSpecifications.Add(val);
                    }
                }

                // Because the properties are not multilingual, the value is going to be binded for the first language.
                int firstLangID = _languageList.FirstOrDefault().ID;

                Value_ProductSpecification value = ProductModel.Values_ProductSpecifications
                    .SingleOrDefault(x => x.SpecificationID == spec.SpecificationID && x.LanguageID == firstLangID);
                value.HoldsValueForOtherNonML = true;

                // If spec is bool...
                if (spec.IsBool)
                {
                    // If it's a newly created value (because there is no product id yet), give it a default value of false
                    if (value.ProductID == 0)
                    {
                        value.BoolValue = false;
                    }

                    // Create a label and a binded checkbox
                    AddFormLabel(spec.LookupName + " * : ", NonMLStackLeftLabels);
                    AddBindedCheckBox(value, "BoolValue", NonMLStackRightInput);
                }
                else
                {
                    // If the spec has enumeration values...
                    if (spec.IsEnumeration)
                    {
                        // If the spec is also multilingual...
                        if (spec.IsMultilingual)
                        {
                            // Create a binded CombBox and a label
                            AddFormLabel(spec.LookupName + " * : ", NonMLStackLeftLabels);
                            AddBindedComboBox(value, "SpecificationEnumID", NonMLStackRightInput, value.SpecificationID);
                        }
                        // At the moment it still works the same way
                        else
                        {
                            // Create a binded ComboBox and a label
                            AddFormLabel(spec.LookupName + " * : ", NonMLStackLeftLabels);
                            AddBindedComboBox(value, "SpecificationEnumID", NonMLStackRightInput, value.SpecificationID);
                        }
                    }
                    else
                    {
                        // If it's not an enumeration and not a bool, make a textbox
                        AddFormLabel(spec.LookupName + " * : ", NonMLStackLeftLabels);
                        AddBindedTextBox(value, "Value", NonMLStackRightInput);
                    }
                }
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
                TurnOnProgressBar();

                if (_updatingPage)
                {
                    // Prepare model for save
                    PrepareModelForUpdate();

                    // Update model
                    await _prodRepo.UpdateWithImagesAsync(ProductModel);

                    // Triggers save so the popup window will be closed and the overview will be shown
                    TriggerSaveEvent();

                    TurnOffProgressBar();
                }
                else
                {
                    // Prepare model for save
                    PrepareModelForCreate();

                    // Adds model
                    await _prodRepo.AddWithImagesAsync(ProductModel);

                    // Triggers save so the product overview will be shown
                    TriggerSaveEvent();

                    TurnOffProgressBar();
                }
            }
            catch (Exception)
            {
                TurnOffProgressBar();

                string content = String.Format(LangResource.MBContentObjSaveFailed, LangResource.TheProduct.ToLower());
                string title = StringExtensions.FirstCharToUpper(String.Format(LangResource.MBTitleObjSaveFailed, LangResource.Product.ToLower()));

                MessageBox.Show(content, title);
            }
        }

        private void PrepareModelForUpdate()
        {
            // For every non multilingual spec, give the value to all the specs that were not binded on
            foreach (var spec in _necessarySpecList.Where(ns => ns.IsMultilingual == false || ns.IsEnumeration == true).OrderBy(x => x.DisplayOrder))
            {
                Value_ProductSpecification val = ProductModel.Values_ProductSpecifications.FirstOrDefault(x => x.HoldsValueForOtherNonML == true
                    && x.SpecificationID == spec.SpecificationID);

                foreach (var value in ProductModel.Values_ProductSpecifications.Where(x => x.SpecificationID == spec.SpecificationID &&
                        x.HoldsValueForOtherNonML == false))
                {
                    value.Value = val.Value;
                    value.BoolValue = val.BoolValue;
                    value.SpecificationEnumID = val.SpecificationEnumID;
                }
            }

            ProductModel.Values_ProductSpecifications = ProductModel.Values_ProductSpecifications.OrderBy(x => x.SpecificationID).ToList();
        }

        private void PrepareModelForCreate()
        {
            // Sets the current stock
            ProductModel.CurrentStock = (int)ProductModel.InitialStock;

            // For every non multilingual spec, give the value to all the specs that were not binded on
            foreach (var spec in _necessarySpecList.Where(ns => ns.IsMultilingual == false || ns.IsEnumeration == true).OrderBy(x => x.DisplayOrder))
            {
                Value_ProductSpecification val = ProductModel.Values_ProductSpecifications.FirstOrDefault(x => x.HoldsValueForOtherNonML == true
                    && x.SpecificationID == spec.SpecificationID);

                foreach (var value in ProductModel.Values_ProductSpecifications.Where(x => x.SpecificationID == spec.SpecificationID &&
                        x.HoldsValueForOtherNonML == false))
                {
                    value.Value = val.Value;
                    value.BoolValue = val.BoolValue;
                    value.SpecificationEnumID = val.SpecificationEnumID;
                }
            }

            //List<Value_ProductSpecification> tempList = new List<Value_ProductSpecification>();

            //// For non multilingual values, makes copies for each language
            //foreach (var val in ProductModel.Values_ProductSpecifications.Where(x => x.LanguageID == null))
            //{
            //    bool isFirstLanguage = true;

            //    foreach (var lang in _languageList)
            //    {
            //        if (isFirstLanguage)
            //        {
            //            val.LanguageID = lang.ID;
            //        }
            //        else
            //        {
            //            tempList.Add(
            //                new Value_ProductSpecification
            //                {
            //                    LanguageID = lang.ID,
            //                    SpecificationID = val.SpecificationID,
            //                    Value = val.Value,
            //                    SpecificationEnumID = val.SpecificationEnumID,
            //                    BoolValue = val.BoolValue
            //                });
            //        }

            //        isFirstLanguage = false;
            //    }
            //}

            //// Adds them to the model
            //foreach (var tempItem in tempList)
            //{
            //    ProductModel.Values_ProductSpecifications.Add(tempItem);
            //}

            // Orders them for easier visibility
            ProductModel.Values_ProductSpecifications = ProductModel.Values_ProductSpecifications.OrderBy(x => x.SpecificationID).ToList();
        }
    }
}