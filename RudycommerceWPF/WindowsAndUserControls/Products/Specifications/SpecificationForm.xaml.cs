using MahApps.Metro.Controls;
using RudycommerceData.Entities;
using RudycommerceData.Entities.Products.Specifications;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceLib.Properties;
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

            InitializeWindow();

            InitializeCreateContent();

            DataContext = this;
        }

        public SpecificationForm(int ID)
        {
            InitializeComponent();

            InitializeWindow();
            
            InitializeUpdateContent(ID);

            DataContext = this;
        }

        private async void InitializeUpdateContent(int ID)
        {
            LanguageList = await _langRepo.GetAllAsync();

            SpecModel = _specRepo.Get(ID);

            _updatingPage = true;

            CheckCheckBoxes();

            foreach (Language l in LanguageList)
            {
                CreateLocalizedTab(l);
            }
        }

        private bool firstUpdateLoad = false;

        private void CheckCheckBoxes()
        {
            firstUpdateLoad = true;

            cbIsBool.IsChecked = SpecModel.IsBool;

            cbIsMultilingual.IsChecked = SpecModel.IsMultilingual;

            cbIsEnum.IsChecked = SpecModel.IsEnumeration;

            firstUpdateLoad = false;
        }

        private void InitializeWindow()
        {
            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;

            SetLanguageDictionary();

            _langRepo = new LanguageRepository();
            _specRepo = new SpecificationRepository();

            SetTitle();
        }

        private async void InitializeCreateContent()
        {
            SpecModel = new Specification
            {
                IsBool = true
            };

            LanguageList = await _langRepo.GetAllAsync();

            SpecModel.LocalizedSpecifications = new List<LocalizedSpecification>();

            foreach (Language l in LanguageList)
            {
                CreateLocalizedTab(l);
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

            StackPanel stackRight = new StackPanel();

            TextBox txtName = new TextBox
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

            TextBox txtAdviceDescription = new TextBox
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

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            SaveModel();
        }

        private void SaveModel()
        {
            if (_updatingPage)
            {
                PrepareModelForSave();

                _specRepo.UpdateAsync(SpecModel);
                _specRepo.SaveChangesAsync();

                TriggerSaveEvent();
            }
            else
            {
                PrepareModelForSave();         

                _specRepo.Add(SpecModel);
                _specRepo.SaveChangesAsync();

                TriggerSaveEvent();
            }
        }

        private void PrepareModelForSave()
        {
            if (SpecificationEnumList == null)
            {

            }
            else
            {
                if (SpecModel.IsMultilingual == false && SpecModel.IsEnumeration == true)
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
            if (SpecModel.IsBool)
            {
                SpecModel.IsMultilingual = false;
                SpecModel.IsEnumeration = false;
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
