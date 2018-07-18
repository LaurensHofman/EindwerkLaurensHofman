using RudycommerceData.Entities;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceLib.CustomExceptions;
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

namespace RudycommerceWPF.WindowsAndUserControls.Languages
{
    /// <summary>
    /// Interaction logic for LanguageForm.xaml
    /// </summary>
    public partial class LanguageForm : FormUserControl
    {
        // TODO Validation (incl. 2 letter ISO)

        public Language LanguageModel { get; set; }
        private ILanguageRepository _langRepo;

        public LanguageForm() : this(new RudycommerceData.Entities.Language()) { }

        public LanguageForm(Language languageModel)
        {
            InitializeComponent();
            InitializeWindow(languageModel);
        }

        public LanguageForm(int ID)
        {
            InitializeComponent();

            _langRepo = new LanguageRepository();

            Language lang = _langRepo.Get(ID);

            btnCancel.Visibility = Visibility.Collapsed;

            InitializeWindow(lang);
        }

        private void InitializeWindow(Language languageModel)
        {
            _updatingPage = !languageModel.IsNew();

            _langRepo = new LanguageRepository();

            LanguageModel = languageModel;

            DataContext = this;

            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;

            SetLanguageDictionary();

            SetTitle();
        }

        protected override void SetTitle()
        {
            lblTitle.SetResourceReference(ContentProperty, _updatingPage ? "UpdateLanguageTitle" : "NewLanguageTitle");
        }

        protected async override void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LanguageModel.LocalName == "Nederlands" || LanguageModel.LocalName == "English")
                {
                    LanguageModel.IsDesktopLanguage = true;
                }

                if (ValidateModel())
                {
                    if (LanguageModel.IsDefault)
                    {
                        Language lang = _langRepo.GetAllQueryable().SingleOrDefault(x => x.IsDefault);

                        if (lang != null && lang.ISO != LanguageModel.ISO)
                        {
                            if (MessageBox.Show(LangResource.MBContMakeLangDefault, LangResource.MBTitleMakeLangDefault,
                            MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                await _langRepo.MakeNewDefaultLanguage(LanguageModel);
                                await _langRepo.SaveChangesAsync();

                                TriggerSaveEvent();
                            }
                            else
                            {
                                LanguageModel.IsDefault = false;
                            }
                        }
                        else
                        {
                            await SaveModel();
                        }
                    }
                    else
                    {
                        await SaveModel();
                    }
                }
            }
            catch (Exception)
            {
                string content = String.Format(LangResource.MBContentObjSaveFailed, LangResource.TheLanguage.ToLower());
                string title = StringExtensions.FirstCharToUpper(String.Format(LangResource.MBTitleObjSaveFailed, LangResource.TheLanguage.ToLower()));

                MessageBox.Show(content, title);
            }
        }

        private async Task SaveModel()
        {
            if (_updatingPage)
            {
                await _langRepo.UpdateAsync(LanguageModel);
                await _langRepo.SaveChangesAsync();

                TriggerSaveEvent();
            }
            else
            {
                _langRepo.Add(LanguageModel);
                await _langRepo.SaveChangesAsync();

                Visibility = Visibility.Collapsed;

                TriggerSaveEvent();
            }
        }

        private bool ValidateModel()
        {
            if (LanguageModel.ISO.Length != 2)
            {
                return false;
            }

            return true;
        }
    }
}
