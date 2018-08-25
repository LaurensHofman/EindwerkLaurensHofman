using MahApps.Metro.Controls;
using RudycommerceData.Entities;
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

namespace RudycommerceWPF.WindowsAndUserControls.Products.Specifications
{
    /// <summary>
    /// Interaction logic for SpecificationForm.xaml
    /// </summary>
    public partial class SpecificationForm : FormUserControl
    {
        public Specification SpecModel { get; set; }

        public List<Language> LanguageList { get; set; }

        private ISpecificationRepository _specRepo;
        private ILanguageRepository _langRepo;


        public SpecificationForm()
        {
            InitializeComponent();
            
            // Defines the progressbar and submit button to allow the progressbar methods to work in the FormUserControl(base)
            progressBar = prog;
            submitButton = btnSubmit;

            SpecModel = new Specification() { LocalizedSpecifications = new List<LocalizedSpecification>()};

            _specRepo = new SpecificationRepository();

            InitializeCreateForm();

            // Model binding is not always working for checkboxes, so I'm checking these in code.
            cbIsBool.IsChecked = true;

            DataContext = this;
        }

        public SpecificationForm(int ID)
        {
            InitializeComponent();

            // Defines the progressbar and submit button to allow the progressbar methods to work in the FormUserControl(base)
            progressBar = prog;
            submitButton = btnSubmit;

            _specRepo = new SpecificationRepository();

            SpecModel = _specRepo.Get(ID);

            _updatingPage = !SpecModel.IsNew();

            PrepareUpdate();

            DataContext = this;
        }

        private async void PrepareUpdate()
        {
            await InitializeCreateForm();

            CheckCheckBoxes();
        }

        private async Task InitializeCreateForm()
        {
            // Gets the preferred language and sets the language dictionary for in XAML and Content from Resources files
            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;
            SetLanguageDictionary();

            _langRepo = new LanguageRepository();

            // Gets all the languages

            LanguageList = await _langRepo.GetAllAsync();

            // Create a tab for each language
            foreach (Language l in LanguageList)
            {
                CreateLocalizedTab(l);
            }

            SetTitle();
        }

        /// <summary>
        /// Checks whether its the first load after an update
        /// </summary>
        private bool firstUpdateLoad = false;

        private void CheckCheckBoxes()
        {
            // for some reason, all the binding is working, except the checkbox binding...
            if (_updatingPage)
            {
                // Is set to true, so the CheckedEvents know they don't have to clear the values this time
                firstUpdateLoad = true;
            }
            
            cbIsBool.IsChecked = SpecModel.IsBool;
            
            cbIsMultilingual.IsChecked = SpecModel.IsMultilingual;

            cbIsEnum.IsChecked = SpecModel.IsEnumeration;

            //cbIsNumber.IsChecked = SpecModel.IsNumber;

            if (_updatingPage)
            {
                firstUpdateLoad = false;
            }
        }
        
        #region Generating Tabs

        private Thickness _defaultMargin = new Thickness { Top = 20 };
        private const int _defaultHeight = 30;
        private const int _defaultWidth = 400;
        private const int _defaultLabelFontSize = 18;
        private Thickness _defaultLabelPadding = new Thickness { Left = 0, Top = -5, Right = 0, Bottom = -5 };
        private Brush _defaultLabelForeground = Brushes.Black;

        private void CreateLocalizedTab(Language lang)
        {
            // Creates a tab item
            MetroTabItem tabItem = CreateMetroTabItem(lang);
            // Creates a grid for under the tab item
            Grid tabGrid = new Grid { Style = Application.Current.Resources["GridBelowTabItem"] as Style };

            // Defines 2 grid Columns so the content can be put into 2 equal columns
            tabGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)});
            tabGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Create a stackpanel for the labels and put it in the left column of the grid
            StackPanel stackPanelLeft = CreateLeftStackPanelForLabels(lang);
            tabGrid.Children.Add(stackPanelLeft);
            Grid.SetColumn(stackPanelLeft, 0);

            // Create a stackpanel for the input fields and put it in the right column of the grid
            StackPanel stackPanelRight = CreateRightStackPanelForInput(lang);
            tabGrid.Children.Add(stackPanelRight);
            Grid.SetColumn(stackPanelRight, 1);

            tabItem.Content = tabGrid;

            TabControlLanguages.Items.Add(tabItem);
        }

        private StackPanel CreateLeftStackPanelForLabels(Language lang)
        {
            // Creates a stackpanel
            StackPanel stackLeft = new StackPanel();

            // Creates a label for the Name and AdviceDescription property
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
            Label labelAdviceDescription = new Label
            {
                Content = LangResource.AdviceDescription + " : ",
                Height = _defaultHeight,
                Foreground = _defaultLabelForeground,
                FontSize = _defaultLabelFontSize,
                Margin = new Thickness { Top = 20, Bottom = 120 },
                Padding = _defaultLabelPadding,
                HorizontalContentAlignment = HorizontalAlignment.Right
            };

            // Adds the labels to the stackpanel
            stackLeft.Children.Add(labelName);
            stackLeft.Children.Add(labelAdviceDescription);

            return stackLeft;
        }

        private StackPanel CreateRightStackPanelForInput(Language lang)
        {
            // Checks if the specification already has a localized specification belonging to the language
            LocalizedSpecification locSpec = SpecModel.LocalizedSpecifications.FirstOrDefault(x => x.LanguageID == lang.ID);

            // If there is no localized specification yet...
            if (locSpec == null)
            {
                // ... create a new localized specification and add it to the Model
                locSpec = new LocalizedSpecification
                {
                    LanguageID = lang.ID,
                    SpecificationID = SpecModel.ID
                };

                SpecModel.LocalizedSpecifications.Add(locSpec);
            }

            // Creates a stackpanel
            StackPanel stackRight = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Left
            };

            // Creates a textbox for the name property
            ClickSelectTextBox txtName = new ClickSelectTextBox
            {
                Height = _defaultHeight,
                Width = _defaultWidth,
                Margin = _defaultMargin,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            // Creates a binding to the LookupName property of the localized specification, and adds the binding to the textbox
            Binding nameBinding = new Binding("LookupName")
            {
                Source = locSpec
            };
            txtName.SetBinding(TextBox.TextProperty, nameBinding);

            // Ditto for the AdviceDescription
            ClickSelectTextBox txtAdviceDescription = new ClickSelectTextBox
            {
                Height = 300,
                Width = _defaultWidth,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                Margin = _defaultMargin
            };
            Binding descriptionBinding = new Binding("AdviceDescription")
            {
                Source = locSpec
            };
            txtAdviceDescription.SetBinding(TextBox.TextProperty, descriptionBinding);

            // Adds the textboxes to the stackpanel
            stackRight.Children.Add(txtName);
            stackRight.Children.Add(txtAdviceDescription);

            return stackRight;
        }

        private MetroTabItem CreateMetroTabItem(Language lang)
        {
            // Creates a new tab item
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

        #endregion

        protected override void SetTitle()
        {
            // Make the title label its content refer to the localized value in the dictionary
            lblTitle.SetResourceReference(ContentProperty, _updatingPage ? "UpdateSpecificationTitle" : "NewSpecificationTitle");
        }

        protected override void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TurnOnProgressBar();

                SaveModel();

                TurnOffProgressBar();
            }
            catch (Exception)
            {
                TurnOffProgressBar();

                string content = String.Format(LangResource.MBContentObjSaveFailed, LangResource.TheSpec.ToLower());
                string title = StringExtensions.FirstCharToUpper(String.Format(LangResource.MBTitleObjSaveFailed, LangResource.Specification.ToLower()));

                MessageBox.Show(content, title);
            }
        }

        private async void SaveModel()
        {
            try
            {
                if (_updatingPage)
                {
                    PrepareModelForSave();

                    await _specRepo.UpdateAsync(SpecModel);
                    await _specRepo.SaveChangesAsync();

                    TriggerSaveEvent();
                }
                else
                {
                    PrepareModelForSave();

                    _specRepo.Add(SpecModel);
                    await _specRepo.SaveChangesAsync();
                                        
                    // Forces the product form and category form to be refreshed the next time they're opened, because the products and categories are dependent on the specifications
                    var window = (NavigationWindow)GetParentWindow();
                    window.ccProductForm.Content = null;
                    window.ccCategoryForm.Content = null;

                    TriggerSaveEvent();
                }
            }
            catch (Exception)
            {

                throw;
            }            
        }

        private void PrepareModelForSave()
        {
            // If the Specification doesn't have an enumeration for its values...
            if (!SpecModel.IsEnumeration)
            {
                // Currently do nothing
            }
            // If the Specification does have an enumeration for its values...
            else
            {
                // Check if Specification is not multilingual
                if (SpecModel.IsMultilingual == false)
                {
                    // If the spec is not multilingual but is an enumeration...
                    // Put the same value for every language

                    SpecModel.Enumerations = SpecificationEnumList.ToList();

                    foreach (var enums in SpecModel.Enumerations)
                    {
                        foreach (var lang in LanguageList)
                        {
                            LocalizedEnumValue locEnum = enums.LocalizedEnumValues.FirstOrDefault(x => x.LanguageID == lang.ID);

                            if (locEnum == null)
                            {
                                locEnum = new LocalizedEnumValue()
                                {
                                    LanguageID = lang.ID,
                                    EnumerationID = enums.ID
                                };
                                enums.LocalizedEnumValues.Add(locEnum);
                            }
                        }

                        foreach (var val in enums.LocalizedEnumValues)
                        {
                            val.Value = enums.TemporaryNonMLValue;
                        }
                    }
                }
                else
                {
                    // If the spec is multilingual, every value will already be filled in, so it's just added to the model
                    SpecModel.Enumerations = SpecificationEnumList.ToList();
                }
            }
        }

        private void EnumChecked(object sender, RoutedEventArgs e)
        {
            // for some reason, all the bindings are working, except the checkboxes in update mode...
            if (!firstUpdateLoad)
            {
                // Inverse the value of the IsEnumeration bool
                SpecModel.IsEnumeration = !SpecModel.IsEnumeration;
            }            

            // Hide the enumeration datagrid (and a button belonging to the datagrid) when the IsEnum bool is false
            dgEnumeration.Visibility = SpecModel.IsEnumeration ? Visibility.Visible : Visibility.Collapsed;
            btnAdd.Visibility = SpecModel.IsEnumeration ? Visibility.Visible : Visibility.Collapsed;

            // Generate the appropriate datagrid for the enum values
            if (SpecModel.IsMultilingual == true)
            {
                GenerateMultilingualDataGridColumns();
            }
            else
            {
                Generate1DataGridColumn();
            }
        }

        private void MLChecked(object sender, RoutedEventArgs e)
        {
            // for some reason, all the bindings are working, except the checkboxes in update mode...
            if (!firstUpdateLoad)
            {
                // Inverse the boolvalue
                SpecModel.IsMultilingual = !SpecModel.IsMultilingual;
            }

            // Generate the appropriate datagrid columns, based on whether it's multilingual (column per language) or whether it's not multilingual (1 column)
            if (SpecModel.IsMultilingual == true)
            {
                GenerateMultilingualDataGridColumns();
            }
            else
            {
                Generate1DataGridColumn();
            }
        }
        
        private void BoolChecked(object sender, RoutedEventArgs e)
        {
            // for some reason, all the bindings are working, except the checkboxes in update mode...
            if (!firstUpdateLoad)
            {
                SpecModel.IsBool = !SpecModel.IsBool;
            }

            if (SpecModel.IsBool)
            {
                // If the specification is bool, it cannot be an enumeration or cannot be multilingual.
                // Hides the checkboxes and labels of the enum and multilingual bool property
                cbIsEnum.IsChecked = false;
                cbIsEnum.Visibility = Visibility.Collapsed;
                lblIsEnum.Visibility = Visibility.Collapsed;

                cbIsMultilingual.IsChecked = false;
                cbIsMultilingual.Visibility = Visibility.Collapsed;
                lblIsMultilingual.Visibility = Visibility.Collapsed;
            }
            else
            {
                // If the specification is not a bool, it can be an enumeration or can be multilingual

                cbIsEnum.Visibility = Visibility.Visible;
                lblIsEnum.Visibility = Visibility.Visible;
                
                cbIsMultilingual.Visibility = Visibility.Visible;
                lblIsMultilingual.Visibility = Visibility.Visible;
            }
        }
        
        #region Generating DataGrid for Enumerations

        /// <summary>
        /// The values that the specification can have, when it's has enumeration values
        /// </summary>
        public ObservableCollection<SpecificationEnum> SpecificationEnumList { get; set; }

        private void Generate1DataGridColumn()
        {
            // If its the first load in an update, fill the datagrid with the current enumerations
            if (firstUpdateLoad)
            {
                foreach (var en in SpecModel.Enumerations)
                {
                    en.TemporaryNonMLValue = en.LocalizedEnumValues.First().Value;
                }

                SpecificationEnumList = new ObservableCollection<SpecificationEnum>(SpecModel.Enumerations);
            }
            // If its not the first load in an update, clear the values list
            else
            {
                SpecificationEnumList = new ObservableCollection<SpecificationEnum>();
            }

            // Binds the datagrid
            dgEnumeration.ItemsSource = SpecificationEnumList;
            dgEnumeration.DataContext = SpecificationEnumList;

            dgEnumeration.Columns.Clear();

            // Sets the single column
            SetLanguageDictionary();
            TextBlock header = new TextBlock()
            {
                Text = LangResource.PotentialValues
            };

            // Creates a binding on the temporary non multilingual value.
            // Later on this value will be put for each language
            string Bindinglocation = "TemporaryNonMLValue";
            Binding valuesBinding = new Binding(Bindinglocation);

            // Creates a new column and adds the binding
            DataGridTextColumn dgCol = new DataGridTextColumn
            {
                Header = header,
                Binding = valuesBinding,
                Width = new DataGridLength(1.0, DataGridLengthUnitType.Star)
            };

            dgEnumeration.Columns.Add(dgCol);

            if (!firstUpdateLoad)
            {
                // Adds 1 row per default
                AddEnumRow(null, null);
            }
        }

        private void GenerateMultilingualDataGridColumns()
        {
            // if it's the first load in an update, gets the specifications from the current model
            if (firstUpdateLoad)
            {
                SpecificationEnumList = new ObservableCollection<SpecificationEnum>(SpecModel.Enumerations);
            }
            // else, creates a new list
            else
            {
                SpecificationEnumList = new ObservableCollection<SpecificationEnum>();
            }
            
            // Binds the datagrid to the list
            dgEnumeration.ItemsSource = SpecificationEnumList;
            dgEnumeration.DataContext = SpecificationEnumList;

            dgEnumeration.Columns.Clear();

            // For each language it will check whether the items exist to bind on, and it will create a binded column
            foreach (var lang in LanguageList)
            {
                // Checks if each enumeration has a value for the current language
                foreach (var en in SpecModel.Enumerations)
                {
                    // Checks if there is already a localized enumeration value in the list.
                    var locEnum = en.LocalizedEnumValues.FirstOrDefault(x => x.LanguageID == lang.ID);

                    // If there is not one yet (for example in a create form, or in an update when a language was added)
                    if (locEnum == null)
                    {
                        // Create a new localized enumeration value and adds it to the model
                        locEnum = new LocalizedEnumValue()
                        {
                            LanguageID = lang.ID,
                            EnumerationID = en.ID
                        };
                        en.LocalizedEnumValues.Add(locEnum);
                    }
                }

                // Creates a new textblock which will be used as header for the column
                TextBlock header = new TextBlock
                {
                    Text = lang.LocalName
                };
                header.Typography.Capitals = FontCapitals.SmallCaps;
                

                // Creates a constant binding location
                int index = LanguageList.IndexOf(lang);
                string Bindinglocation = $"LocalizedEnumValues[{index}].Value";
                Binding valuesBinding = new Binding(Bindinglocation);

                // Adds a column and adds a binding based on the matching language
                DataGridTextColumn dgCol
                    = new DataGridTextColumn
                    {
                        Header = header,
                        Binding = valuesBinding,
                        Width = new DataGridLength(1.0, DataGridLengthUnitType.Star)
                    };
                dgEnumeration.Columns.Add(dgCol);
            }

            if (!firstUpdateLoad)
            {
                // Add an empty row per default
                AddEnumRow(null, null);
            }
        }

        private void AddEnumRow(object sender, RoutedEventArgs e)
        {
            // Creates a new specification enum to bind on
            SpecificationEnum newEnum = new SpecificationEnum();

            // Adds a localized enumeration value to the new SpecificationEnum for each language
            foreach (var lang in LanguageList)
            {
                newEnum.LocalizedEnumValues.Add(
                    new LocalizedEnumValue
                    {
                        LanguageID = lang.ID
                    });
            }

            // Adds the specificationEnum to the list and refreshes the grid
            SpecificationEnumList.Add(newEnum);
            dgEnumeration.ItemsSource = SpecificationEnumList;
            dgEnumeration.DataContext = SpecificationEnumList;
        }

        #endregion        
    }
}
