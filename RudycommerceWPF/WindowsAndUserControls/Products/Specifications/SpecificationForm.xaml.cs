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
        //
        //
        // ERROR MESSAGE ON SAVE FAILED
        // INTERFACES
        //
        //
        

        public Specification SpecModel { get; set; }

        public List<Language> LanguageList { get; set; }

        private ISpecificationRepository _specRepo;
        private ILanguageRepository _langRepo;


        public SpecificationForm()
        {
            InitializeComponent();
            
            progressBar = prog;
            submitButton = btnSubmit;

            SpecModel = new Specification();

            InitializeForm();

            cbIsBool.IsChecked = true;

            DataContext = this;
        }

        public SpecificationForm(int ID)
        {
            InitializeComponent();
            
            progressBar = prog;
            submitButton = btnSubmit;

            _specRepo = new SpecificationRepository();

            SpecModel = _specRepo.Get(ID);

            _updatingPage = !SpecModel.IsNew();

            InitializeForm();
            
            CheckCheckBoxes();

            DataContext = this;
        }

        private async void InitializeForm()
        {
            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;

            SetLanguageDictionary();

            _langRepo = new LanguageRepository();
            _specRepo = new SpecificationRepository();

            LanguageList = await _langRepo.GetAllAsync();

            SpecModel.LocalizedSpecifications = new List<LocalizedSpecification>();

            foreach (Language l in LanguageList)
            {
                CreateLocalizedTab(l);
            }

            SetTitle();
        }

        private bool firstUpdateLoad = false;

        private void CheckCheckBoxes()
        {
            // for some reason, all the binding is working, except the checkbox binding...
            if (_updatingPage)
            {
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
            MetroTabItem tabItem = CreateMetroTabItem(lang);
            Grid tabGrid = new Grid { Style = Application.Current.Resources["GridBelowTabItem"] as Style };
            tabGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)});
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
                Content = LangResource.AdviceDescription + " : ",
                Height = _defaultHeight,
                Foreground = _defaultLabelForeground,
                FontSize = _defaultLabelFontSize,
                Margin = new Thickness { Top = 20, Bottom = 120 },
                Padding = _defaultLabelPadding,
                HorizontalContentAlignment = HorizontalAlignment.Right
            };

            stackLeft.Children.Add(labelName);
            stackLeft.Children.Add(labelDescription);

            return stackLeft;
        }

        private StackPanel CreateRightStackPanelForInput(Language lang)
        {
            LocalizedSpecification locSpec;

            if (_updatingPage)
            {
                locSpec = SpecModel.LocalizedSpecifications.SingleOrDefault(x => x.LanguageID == lang.ID);
            }
            else
            {
                locSpec = new LocalizedSpecification
                {
                    LanguageID = lang.ID
                };
            }

            StackPanel stackRight = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Left
            };

            ClickSelectTextBox txtName = new ClickSelectTextBox
            {
                Height = _defaultHeight,
                Width = _defaultWidth,
                Margin = _defaultMargin,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            Binding nameBinding = new Binding("LookupName")
            {
                Source = locSpec
            };
            txtName.SetBinding(TextBox.TextProperty, nameBinding);

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

            stackRight.Children.Add(txtName);
            stackRight.Children.Add(txtAdviceDescription);

            SpecModel.LocalizedSpecifications.Add(locSpec);

            return stackRight;
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

        #endregion

        protected override void SetTitle()
        {
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
            if (!SpecModel.IsEnumeration)
            {

            }
            else
            {
                if (SpecModel.IsMultilingual == false)
                {
                    SpecModel.Enumerations = SpecificationEnumList.ToList();

                    foreach (var enums in SpecModel.Enumerations)
                    {
                        foreach (var val in enums.LocalizedEnumValues)
                        {
                            val.Value = enums.TemporaryNonMLValue;
                        }
                    }
                }
                else
                {
                    SpecModel.Enumerations = SpecificationEnumList.ToList();
                }
            }
        }

        private void EnumChecked(object sender, RoutedEventArgs e)
        {
            // for some reason, all the bindings are working, except the checkboxes in update mode...
            if (!firstUpdateLoad)
            {
                SpecModel.IsEnumeration = !SpecModel.IsEnumeration;
            }            

            dgEnumeration.Visibility = SpecModel.IsEnumeration ? Visibility.Visible : Visibility.Collapsed;
            btnAdd.Visibility = SpecModel.IsEnumeration ? Visibility.Visible : Visibility.Collapsed;

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
                SpecModel.IsMultilingual = !SpecModel.IsMultilingual;
            }

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
                cbIsEnum.IsChecked = false;
                cbIsEnum.Visibility = Visibility.Collapsed;
                lblIsEnum.Visibility = Visibility.Collapsed;

                cbIsMultilingual.IsChecked = false;
                cbIsMultilingual.Visibility = Visibility.Collapsed;
                lblIsMultilingual.Visibility = Visibility.Collapsed;
            }
            else
            {
                cbIsEnum.Visibility = Visibility.Visible;
                lblIsEnum.Visibility = Visibility.Visible;
                
                cbIsMultilingual.Visibility = Visibility.Visible;
                lblIsMultilingual.Visibility = Visibility.Visible;
            }
        }
        
        #region Generating DataGrid for Enumerations

        public ObservableCollection<SpecificationEnum> SpecificationEnumList { get; set; }

        private void Generate1DataGridColumn()
        {
            if (firstUpdateLoad)
            {
                foreach (var en in SpecModel.Enumerations)
                {
                    en.TemporaryNonMLValue = en.LocalizedEnumValues.First().Value;
                }

                SpecificationEnumList = new ObservableCollection<SpecificationEnum>(SpecModel.Enumerations);
            }
            else
            {
                SpecificationEnumList = new ObservableCollection<SpecificationEnum>();
            }

            dgEnumeration.ItemsSource = SpecificationEnumList;
            dgEnumeration.DataContext = SpecificationEnumList;

            dgEnumeration.Columns.Clear();

            SetLanguageDictionary();
            TextBlock header = new TextBlock()
            {
                Text = LangResource.PotentialValues
            };

            string Bindinglocation = "TemporaryNonMLValue";
            Binding valuesBinding = new Binding(Bindinglocation);

            DataGridTextColumn dgCol = new DataGridTextColumn
            {
                Header = header,
                Binding = valuesBinding,
                Width = new DataGridLength(1.0, DataGridLengthUnitType.Star)
            };

            dgEnumeration.Columns.Add(dgCol);

            if (!firstUpdateLoad)
            {
                AddEnumRow(null, null);
            }
        }

        private void GenerateMultilingualDataGridColumns()
        {
            if (firstUpdateLoad)
            {
                SpecificationEnumList = new ObservableCollection<SpecificationEnum>(SpecModel.Enumerations);
            }
            else
            {
                SpecificationEnumList = new ObservableCollection<SpecificationEnum>();
            }
            
            dgEnumeration.ItemsSource = SpecificationEnumList;
            dgEnumeration.DataContext = SpecificationEnumList;

            dgEnumeration.Columns.Clear();

            foreach (var lang in LanguageList)
            {
                TextBlock header = new TextBlock
                {
                    Text = lang.LocalName
                };
                header.Typography.Capitals = FontCapitals.SmallCaps;

                int index = LanguageList.IndexOf(lang);
                string Bindinglocation = $"LocalizedEnumValues[{index}].Value";
                Binding valuesBinding = new Binding(Bindinglocation);


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
                AddEnumRow(null, null);
            }
        }

        private void AddEnumRow(object sender, RoutedEventArgs e)
        {
            SpecificationEnum newEnum = new SpecificationEnum();

            foreach (var lang in LanguageList)
            {
                newEnum.LocalizedEnumValues.Add(
                    new LocalizedEnumValue
                    {
                        LanguageID = lang.ID
                    });
            }

            SpecificationEnumList.Add(newEnum);

            dgEnumeration.ItemsSource = SpecificationEnumList;
            dgEnumeration.DataContext = SpecificationEnumList;
        }

        #endregion

        
    }
}
