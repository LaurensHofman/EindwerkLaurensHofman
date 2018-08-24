using MahApps.Metro.Controls;
using RudycommerceData.Entities;
using RudycommerceData.Entities.Products.Categories;
using RudycommerceData.Entities.Products.Specifications;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceLib.Properties;
using RudycommerceLib.Utilities;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RudycommerceWPF.WindowsAndUserControls.Products.Categories
{
    /// <summary>
    /// Interaction logic for CategoryForm.xaml
    /// </summary>
    public partial class CategoryForm : FormUserControl
    {
        public Category CategoryModel { get; set; }

        private ICategoryRepository _categoryRepo;
        private ILanguageRepository _langRepo;
        private ISpecificationRepository _specRepo;

        /// <summary>
        /// List of all the existing specifications, that the user can choose of to add to their category.
        /// </summary>
        public ObservableCollection<LocalizedSpecification> SelectionSpecList { get; set; }

        /// <summary>
        /// List of all the languages
        /// </summary>
        private List<Language> LanguageList;

        public CategoryForm()
        {
            InitializeComponent();

            // Creates new category
            CategoryModel = new Category
            {
                CategorySpecifications = new ObservableCollection<CategorySpecification>(),
                LocalizedCategories = new List<LocalizedCategory>()
            };

            _categoryRepo = new CategoryRepository();

            InitializeWindow();

            GenerateTabs();

            FillSelectionDataGrid();
        }

        public CategoryForm(int id)
        {
            InitializeComponent();

            _categoryRepo = new CategoryRepository();

            // Gets the category from the database
            CategoryModel = _categoryRepo.Get(id);

            InitializeWindow();

            LoadUpdateWindow();
        }

        private async void LoadUpdateWindow()
        {
            // Generate the tabs for each language (to fill in the category name and plural name)
            GenerateTabs();

            // Fills the specification datagrid so you can use which specification you want to add to a category
            await FillSelectionDataGrid();

            // Fill the datagrids based on which specifications were already added
            PrepareDataGrids();

            BindCategorySpecificationsGrid();
        }

        private void PrepareDataGrids()
        {
            // Gets the specifications that were already added to the category
            List<int> specIDs = new List<int>();
            foreach (var catSpec in CategoryModel.CategorySpecifications.OrderBy(cs => cs.DisplayOrder))
            {
                specIDs.Add(catSpec.SpecificationID);
            }

            // Clear the added specifications
            CategoryModel.CategorySpecifications = new ObservableCollection<CategorySpecification>();

            // Add them again to the category. Now they will be correctly removed from the selection datagrid.
            foreach (int id in specIDs)
            {
                AddSpecificationByID(id);
            }
        }

        private void AddSpecificationByID(int id)
        {
            // Gets the specification
            LocalizedSpecification spec = SelectionSpecList.SingleOrDefault(x => x.SpecificationID == id);

            // Adds it to the datagrid with the selected specs
            AddSelectedSpecification(spec);

            // Remove it from the datagrid with the possible specs
            RemoveSelectedSpecificationFromList(spec);
        }

        /// <summary>
        /// Functionality that has to happen in both the create and update form
        /// </summary>
        private void InitializeWindow()
        {
            // Defines the progressBar and SubmitButton to allow the ProgressBar methods to work (see FormUserControl)
            progressBar = prog;
            submitButton = btnSubmit;

            _langRepo = new LanguageRepository();
            _specRepo = new SpecificationRepository();

            // Sets display language
            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;
            SetLanguageDictionary();

            DataContext = this;

            _updatingPage = !CategoryModel.IsNew();

            SetTitle();
        }

        private async Task FillSelectionDataGrid()
        {
            // Gets all the specs from the database
            List<Specification> specs = await _specRepo.GetAllAsync();

            // Gets the localized specifications, according to the preferred display language of the user
            List<LocalizedSpecification> locspecs = new List<LocalizedSpecification>();
            foreach (var spec in specs)
            {
                locspecs.Add(spec.LocalizedSpecifications.SingleOrDefault(x => x.LanguageID == _preferredLanguage.ID));
            }

            // Make an observable collection of the localized specifications, so it can be used to display in the datagrid
            SelectionSpecList = new ObservableCollection<LocalizedSpecification>(locspecs);
            BindSpecificationSelectionData();
        }

        protected override async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TurnOnProgressBar();

                if (_updatingPage)
                {
                    _categoryRepo.Update(CategoryModel);
                }
                else
                {
                    _categoryRepo.Add(CategoryModel);

                    // Force the product form to refresh when the user opens it the next time
                    var win = (NavigationWindow)GetParentWindow();
                    win.ccProductForm.Content = null;
                }

                await _categoryRepo.SaveChangesAsync();

                // Triggers the save event, so this can close and the category overview can be shown.
                TriggerSaveEvent();

                TurnOffProgressBar();
            }
            catch (Exception)            
            {
                TurnOffProgressBar();

                string content = String.Format(LangResource.MBContentObjSaveFailed, LangResource.TheCategory.ToLower());
                string title = StringExtensions.FirstCharToUpper(String.Format(LangResource.MBTitleObjSaveFailed, LangResource.Category.ToLower()));

                MessageBox.Show(content, title);
            }
        }

        /// <summary>
        /// Sets the content of the title label
        /// </summary>
        protected override void SetTitle()
        {
            // Make the content of the title label refer to a localized dictionary value
            lblTitle.SetResourceReference(ContentProperty, _updatingPage ? "UpdateCategoryTitle" : "NewCategoryTitle");
        }

        /// <summary>
        /// Generate a tab for each language
        /// </summary>
        private async void GenerateTabs()
        {
            LanguageList = await _langRepo.GetAllAsync();

            foreach (var lang in LanguageList)
            {
                CreateLocalizedTab(lang);
            }
        }

        #region GenerateLocalizedTabs

        private Thickness _defaultMargin = new Thickness { Top = 20 };
        private const int _defaultHeight = 30;
        private const int _defaultWidth = 400;
        private const int _defaultLabelFontSize = 18;
        private Thickness _defaultLabelPadding = new Thickness { Left = 0, Top = -5, Right = 0, Bottom = -5 };
        private Brush _defaultLabelForeground = Brushes.Black;

        /// <summary>
        /// Creates a tab for the given language
        /// </summary>
        /// <param name="lang"></param>
        private void CreateLocalizedTab(Language lang)
        {
            // Create a tab item
            MetroTabItem tabItem = CreateMetroTabItem(lang);
            // Creates a grid for under the tab item, with a style defined in the styles.xaml file
            Grid tabGrid = new Grid { Style = Application.Current.Resources["GridBelowTabItem"] as Style };

            //Creates a wrappanel, so the stackpanel with the labels and the stackpanel with the input are nicely next to eachother
            WrapPanel wrapForStacks = new WrapPanel { HorizontalAlignment = HorizontalAlignment.Center };

            // Stackpanel for the labels
            StackPanel stackPanelLeft = CreateLeftStackPanelForLabels(lang);
            // Stackpanel for the input fields (Name, Plural name)
            StackPanel stackPanelRight = CreateRightStackPanelForInput(lang);

            // Adds the stackpanels to the wrappanel
            wrapForStacks.Children.Add(stackPanelLeft);
            wrapForStacks.Children.Add(stackPanelRight);

            // Adds the wrappanel to the grid
            tabGrid.Children.Add(wrapForStacks);

            // Adds the grid to the tabItem
            tabItem.Content = tabGrid;

            // Adds the tabItem to the tabControl
            TabControlLanguages.Items.Add(tabItem);
        }

        private MetroTabItem CreateMetroTabItem(Language lang)
        {
            // Creates a new metroTabItem, with a header text equal to the language its local name
            MetroTabItem metroTab = new MetroTabItem
            {
                Header = lang.LocalName,
                Name = $"tab{lang.ID}",
                Padding = new Thickness { Left = 25, Right = 25 },
                Background = Brushes.Beige
            };

            //https://stackoverflow.com/questions/23377194/overwrite-mahapps-metro-style-for-me-header-tabitem
            //https://social.msdn.microsoft.com/Forums/vstudio/en-US/ffcd8d49-267c-4ccb-8ceb-b80305447cb4/c-wpf-implementing-style-using-code?forum=wpf

            // Creates a new style for the new tabItem
            Style InCodeGeneratedTabItem = new Style
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

            InCodeGeneratedTabItem.Triggers.Add(SelectedTrigger);
            InCodeGeneratedTabItem.Triggers.Add(NotSelectedTrigger);

            InCodeGeneratedTabItem.Setters.Add(new Setter() { Property = ControlsHelper.HeaderFontSizeProperty, Value = 18.0 });
            metroTab.Style = InCodeGeneratedTabItem;

            return metroTab;
        }

        private StackPanel CreateLeftStackPanelForLabels(Language lang)
        {
            // Create a new stackpanel
            StackPanel stackLeft = new StackPanel();

            // Creates a label for the Name property
            Label labelName = new Label
            {
                Content = LangResource.Name + " * : ",
                Height = _defaultHeight,
                Foreground = _defaultLabelForeground,
                FontSize = _defaultLabelFontSize,
                Margin = _defaultMargin,
                Padding = _defaultLabelPadding,
                HorizontalContentAlignment = HorizontalAlignment.Right
            };

            // Creates a label for the Plural name property
            Label labelPluralName = new Label
            {
                Content = LangResource.PluralName + " * : ",
                Height = _defaultHeight,
                Foreground = _defaultLabelForeground,
                FontSize = _defaultLabelFontSize,
                Margin = _defaultMargin,
                Padding = _defaultLabelPadding,
                HorizontalContentAlignment = HorizontalAlignment.Right
            };

            // Adds the labels to the stackpanel
            stackLeft.Children.Add(labelName);
            stackLeft.Children.Add(labelPluralName);

            return stackLeft;
        }

        private StackPanel CreateRightStackPanelForInput(Language lang)
        {
            // Checks if the category already contains a localized category for said language.
            LocalizedCategory locCat = CategoryModel.LocalizedCategories.FirstOrDefault(x => x.LanguageID == lang.ID);

            // If the category does not contain one yet, 
            if (locCat == null)
            {
                // Create a new one and add it to the model
                locCat = new LocalizedCategory
                {
                    LanguageID = lang.ID
                };
                CategoryModel.LocalizedCategories.Add(locCat);
            }

            // Creates a stackpanel
            StackPanel stackRight = new StackPanel();

            // Creates a new textbox for the Name property
            ClickSelectTextBox txtName = new ClickSelectTextBox
            {
                Height = _defaultHeight,
                Width = _defaultWidth,
                Margin = _defaultMargin,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            // Creates a binding with source the localized category and binds it to the Name property
            Binding nameBinding = new Binding("Name")
            {
                Source = locCat
            };
            // Adds the binding to the textbox
            txtName.SetBinding(TextBox.TextProperty, nameBinding);

            // Idem for the plural name textbox
            ClickSelectTextBox txtPluralName = new ClickSelectTextBox
            {
                Height = _defaultHeight,
                Width = _defaultWidth,
                Margin = _defaultMargin,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            Binding pluralNameBinding = new Binding("PluralName")
            {
                Source = locCat
            };
            txtPluralName.SetBinding(TextBox.TextProperty, pluralNameBinding);

            // Adds both textboxes to the stackpanels
            stackRight.Children.Add(txtName);
            stackRight.Children.Add(txtPluralName);            

            return stackRight;
        }

        #endregion

        #region AddProperty

        /// <summary>
        /// Adds the specification to the category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddSpecification(object sender, RoutedEventArgs e)
        {
            // Gets the selected specification
            LocalizedSpecification spec = ((FrameworkElement)sender).DataContext as LocalizedSpecification;
            // Adds the selected specification to the category
            AddSelectedSpecification(spec);
            // Remove the specification from the specification selection datagrid
            RemoveSelectedSpecificationFromList(spec);
        }

        private void AddSelectedSpecification(LocalizedSpecification spec)
        {
            // Adds a specification to the category
            CategoryModel.CategorySpecifications.Add(
                new CategorySpecification
                {
                    SpecificationName = spec.LookupName,
                    SpecificationID = spec.SpecificationID,
                    DisplayOrder = CategoryModel.CategorySpecifications.Count()
                });

            // Refreshes the category specification datagrid
            BindCategorySpecificationsGrid();
        }

        private void RemoveSelectedSpecificationFromList(LocalizedSpecification spec)
        {
            // Removes the specification from the specification selection list
            SelectionSpecList.Remove(spec);
            // Refreshes the specification selection datagrid
            BindSpecificationSelectionData();
        }

        #endregion

        #region RemoveProperty

        /// <summary>
        /// Removes the specification from the category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveSpecification(object sender, RoutedEventArgs e)
        {
            // Gets the specification
            CategorySpecification catspec = ((FrameworkElement)sender).DataContext as CategorySpecification;
            // Remove the specification from the category
            RemoveSelectedSpecFromCategory(catspec);
            // Add the specification back to the selection list
            AddSpecBackToSelectionList(catspec);
        }

        private void RemoveSelectedSpecFromCategory(CategorySpecification catspec)
        {
            // Gets the index of the removed spec
            int removedSpecIndex = CategoryModel.CategorySpecifications.ToList().IndexOf(catspec);

            // Removes the specification from the category
            CategoryModel.CategorySpecifications.Remove(catspec);

            // Every specification coming after the removed one, has its display order reduced by one
            for (int i = removedSpecIndex; i < CategoryModel.CategorySpecifications.Count(); i++)
            {
                CategoryModel.CategorySpecifications.ToList()[i].DisplayOrder = i;
            }

            // Refresh the category specification grid
            BindCategorySpecificationsGrid();
        }

        private void AddSpecBackToSelectionList(CategorySpecification catspec)
        {
            // Create a specification and add it to the selection list
            SelectionSpecList.Add(
                new LocalizedSpecification
                {
                    SpecificationID = catspec.SpecificationID,
                    LookupName = catspec.SpecificationName
                });

            // Refresh the selection grid
            BindSpecificationSelectionData();
        }

        #endregion

        /// <summary>
        /// Refreshes the specification selection datagrid
        /// </summary>
        private void BindSpecificationSelectionData()
        {
            dgSelectSpec.ItemsSource = SelectionSpecList.OrderBy(x => x.LookupName);
            dgSelectSpec.DataContext = SelectionSpecList;
        }

        /// <summary>
        /// Refreshes the category specification datagrid
        /// </summary>
        private void BindCategorySpecificationsGrid()
        {
            ObservableCollection<CategorySpecification> obsColl = new ObservableCollection<CategorySpecification>(CategoryModel.CategorySpecifications);
            dgCategorySpecifications.ItemsSource = obsColl;
            dgCategorySpecifications.DataContext = obsColl;
        }

        private void MovePropertyUp(object sender, RoutedEventArgs e)
        {
            // Moving property up in the datagrid means the index (and display order) decreases

            // Gets the specification
            CategorySpecification spec = ((FrameworkElement)sender).DataContext as CategorySpecification;

            // Gets the index of the specification
            int index = CategoryModel.CategorySpecifications.ToList().IndexOf(spec);

            // If its index is 0, then it can't move up
            if (index > 0)
            {
                // Swaps the display order of the swapped specs
                CategoryModel.CategorySpecifications.ToList()[index].DisplayOrder = index - 1;
                CategoryModel.CategorySpecifications.ToList()[index - 1].DisplayOrder = index;

                // Swaps the specs in the list
                CategoryModel.CategorySpecifications
                    = ListUtilities<CategorySpecification>.Swap
                    (CategoryModel.CategorySpecifications.ToList(), index, index - 1);

                // Refreshes the category specification grid
                BindCategorySpecificationsGrid();
            }
        }

        private void MovePropertyDown(object sender, RoutedEventArgs e)
        {
            // Moving property down in the datagrid means the index increases

            // Gets the specification
            CategorySpecification catspec = ((FrameworkElement)sender).DataContext as CategorySpecification;

            // Gets the index of the specification
            int index = CategoryModel.CategorySpecifications.ToList().IndexOf(catspec);

            // If the spec is the last one in the list, it can't move down
            if (index < CategoryModel.CategorySpecifications.Count() - 1)
            {
                // Swaps the display order of the swapped specs
                CategoryModel.CategorySpecifications.ToList()[index].DisplayOrder = index + 1;
                CategoryModel.CategorySpecifications.ToList()[index + 1].DisplayOrder = index;

                // Swaps the 2 specs in the list
                CategoryModel.CategorySpecifications
                    = ListUtilities<CategorySpecification>.Swap
                    (CategoryModel.CategorySpecifications.ToList(), index, index + 1);

                // Refreshes the grid
                BindCategorySpecificationsGrid();
            }
        }
    }
}
