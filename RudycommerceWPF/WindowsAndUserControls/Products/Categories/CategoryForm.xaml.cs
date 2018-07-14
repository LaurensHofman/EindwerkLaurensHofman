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

        public ObservableCollection<LocalizedSpecification> SelectionSpecList { get; set; }

        private List<Language> LanguageList;

        public CategoryForm()
        {
            InitializeComponent();

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

            CategoryModel = _categoryRepo.Get(id);

            InitializeWindow();


            LoadUpdateWindow();
        }

        private async void LoadUpdateWindow()
        {
            GenerateTabs();

            await FillSelectionDataGrid();

            PrepareDataGrids();

            BindCategorySpecificationsGrid();
        }

        private void PrepareDataGrids()
        {
            List<int> specIDs = new List<int>() ;

            foreach (var catSpec in CategoryModel.CategorySpecifications)
            {
                specIDs.Add(catSpec.SpecificationID);
            }

            CategoryModel.CategorySpecifications = new ObservableCollection<CategorySpecification>();

            foreach (int id in specIDs)
            {
                AddPropertyByID(id);
            }
        }

        private void AddPropertyByID(int id)
        {
            LocalizedSpecification spec = SelectionSpecList.SingleOrDefault(x => x.SpecificationID == id);
            AddSelectedSpecification(spec);
            RemoveSelectedSpecificationFromList(spec);
        }

        private void InitializeWindow()
        {
            _langRepo = new LanguageRepository();
            _specRepo = new SpecificationRepository();

            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;

            SetLanguageDictionary();

            DataContext = this;

            _updatingPage = !CategoryModel.IsNew();

            SetTitle();
        }

        private async Task FillSelectionDataGrid()
        {
            List<Specification> specs = await _specRepo.GetAllAsync();

            List<LocalizedSpecification> locspecs = new List<LocalizedSpecification>() ;

            foreach (var spec in specs)
            {
                locspecs.Add(spec.LocalizedSpecifications.SingleOrDefault(x => x.LanguageID == _preferredLanguage.ID));
            }

            SelectionSpecList = new ObservableCollection<LocalizedSpecification>(locspecs);

            BindSpecificationSelectionData();
        }        

        protected override async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_updatingPage)
                {
                    _categoryRepo.Update(CategoryModel);
                }
                else
                {
                    _categoryRepo.Add(CategoryModel);
                }

                await _categoryRepo.SaveChangesAsync();

                TriggerSaveEvent();
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected override void SetTitle()
        {
            lblTitle.SetResourceReference(ContentProperty, _updatingPage ? "UpdateCategoryTitle" : "NewCategoryTitle");
        }

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

        private void CreateLocalizedTab(Language lang)
        {
            MetroTabItem tabItem = CreateMetroTabItem(lang);
            Grid tabGrid = new Grid { Style = Application.Current.Resources["GridBelowTabItem"] as Style };

            //Creates a wrappanel, so the stackpanel with the labels and the stackpanel with the input are nicely next to eachother
            WrapPanel wrapForStacks = new WrapPanel { HorizontalAlignment = HorizontalAlignment.Center };

            StackPanel stackPanelLeft = CreateLeftStackPanelForLabels(lang);
            StackPanel stackPanelRight = CreateRightStackPanelForInput(lang);

            wrapForStacks.Children.Add(stackPanelLeft);
            wrapForStacks.Children.Add(stackPanelRight);

            tabGrid.Children.Add(wrapForStacks);

            tabItem.Content = tabGrid;

            TabControlLanguages.Items.Add(tabItem);
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

            //https://stackoverflow.com/questions/23377194/overwrite-mahapps-metro-style-for-me-header-tabitem
            //https://social.msdn.microsoft.com/Forums/vstudio/en-US/ffcd8d49-267c-4ccb-8ceb-b80305447cb4/c-wpf-implementing-style-using-code?forum=wpf

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

        private StackPanel CreateLeftStackPanelForLabels(Language lang)
        {
            StackPanel stackLeft = new StackPanel();
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
            Label labelDescription = new Label
            {
                Content = LangResource.PluralName + " * : ",
                Height = _defaultHeight,
                Foreground = _defaultLabelForeground,
                FontSize = _defaultLabelFontSize,
                Margin = _defaultMargin,
                Padding = _defaultLabelPadding,
                HorizontalContentAlignment = HorizontalAlignment.Right
            };

            stackLeft.Children.Add(labelName);
            stackLeft.Children.Add(labelDescription);

            return stackLeft;
        }

        private StackPanel CreateRightStackPanelForInput(Language lang)
        {
            LocalizedCategory locCat = CategoryModel.LocalizedCategories.SingleOrDefault(x => x.LanguageID == lang.ID);

            if (locCat == null)
            {
                locCat = new LocalizedCategory
                {
                    LanguageID = lang.ID
                };
            }

            StackPanel stackRight = new StackPanel();

            TextBox txtName = new TextBox
            {
                Height = _defaultHeight,
                Width = _defaultWidth,
                Margin = _defaultMargin,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            Binding nameBinding = new Binding("Name")
            {
                Source = locCat
            };
            txtName.SetBinding(TextBox.TextProperty, nameBinding);

            TextBox txtPluralName = new TextBox
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

            stackRight.Children.Add(txtName);
            stackRight.Children.Add(txtPluralName);

            CategoryModel.LocalizedCategories.Add(locCat);

            return stackRight;
        }

        #endregion

        #region AddProperty

        private void AddProperty(object sender, RoutedEventArgs e)
        {
            LocalizedSpecification spec = ((FrameworkElement)sender).DataContext as LocalizedSpecification;
            AddSelectedSpecification(spec);
            RemoveSelectedSpecificationFromList(spec);
        }

        private void AddSelectedSpecification(LocalizedSpecification spec)
        {
            CategoryModel.CategorySpecifications.Add(
                new CategorySpecification
                {
                    SpecificationName = spec.LookupName,
                    SpecificationID = spec.SpecificationID,
                    DisplayOrder = CategoryModel.CategorySpecifications.Count()
                });

            BindCategorySpecificationsGrid();
        }

        private void RemoveSelectedSpecificationFromList(LocalizedSpecification spec)
        {
            SelectionSpecList.Remove(spec);
            BindSpecificationSelectionData();
        }

        #endregion

        #region RemoveProperty

        private void RemoveProperty(object sender, RoutedEventArgs e)
        {
            CategorySpecification catspec = ((FrameworkElement)sender).DataContext as CategorySpecification;
            RemoveSelectedSpecFromCategory(catspec);
            AddSpecBackToSelectionList(catspec);
        }

        private void RemoveSelectedSpecFromCategory(CategorySpecification catspec)
        {
            int removedSpecIndex = CategoryModel.CategorySpecifications.ToList().IndexOf(catspec);

            CategoryModel.CategorySpecifications.Remove(catspec);

            for (int i = removedSpecIndex; i < CategoryModel.CategorySpecifications.Count(); i++)
            {
                CategoryModel.CategorySpecifications.ToList()[i].DisplayOrder = i;
            }

            BindCategorySpecificationsGrid();
        }

        private void AddSpecBackToSelectionList(CategorySpecification catspec)
        {
            SelectionSpecList.Add(
                new LocalizedSpecification
                {
                    SpecificationID = catspec.SpecificationID,
                    LookupName = catspec.SpecificationName
                });

            BindSpecificationSelectionData();
        }

        #endregion
        
        private void BindSpecificationSelectionData()
        {
            dgSelectSpec.ItemsSource = SelectionSpecList.OrderBy(x => x.LookupName);
            dgSelectSpec.DataContext = SelectionSpecList;
        }

        private void BindCategorySpecificationsGrid()
        {
            ObservableCollection<CategorySpecification> obsColl = new ObservableCollection<CategorySpecification>(CategoryModel.CategorySpecifications);
            dgCategorySpecifications.ItemsSource = obsColl;
            dgCategorySpecifications.DataContext = obsColl;
        }

        private void MovePropertyUp(object sender, RoutedEventArgs e)
        {
            // moving property up in the datagrid means the index decreases

            CategorySpecification prop = ((FrameworkElement)sender).DataContext as CategorySpecification;

            int index = CategoryModel.CategorySpecifications.ToList().IndexOf(prop);

            if (index > 0)
            {
                CategoryModel.CategorySpecifications.ToList()[index].DisplayOrder = index - 1;
                CategoryModel.CategorySpecifications.ToList()[index - 1].DisplayOrder = index;

                CategoryModel.CategorySpecifications
                    = ListUtilities<CategorySpecification>.Swap
                    (CategoryModel.CategorySpecifications.ToList(), index, index - 1);

                BindCategorySpecificationsGrid();
            }
        }

        private void MovePropertyDown(object sender, RoutedEventArgs e)
        {
            // moving property down in the datagrid means the index increases

            CategorySpecification catspec = ((FrameworkElement)sender).DataContext as CategorySpecification;

            int index = CategoryModel.CategorySpecifications.ToList().IndexOf(catspec);

            if (index < CategoryModel.CategorySpecifications.Count() - 1)
            {
                CategoryModel.CategorySpecifications.ToList()[index].DisplayOrder = index + 1;
                CategoryModel.CategorySpecifications.ToList()[index + 1].DisplayOrder = index;

                CategoryModel.CategorySpecifications
                    = ListUtilities<CategorySpecification>.Swap
                    (CategoryModel.CategorySpecifications.ToList(), index, index + 1);

                BindCategorySpecificationsGrid();
            }
        }
    }
}
