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
        public Language LanguageModel { get; set; }
        private ILanguageRepository _langRepo;

        public LanguageForm() : this(new RudycommerceData.Entities.Language()) { }

        public LanguageForm(Language languageModel)
        {
            InitializeComponent();

            _langRepo = new LanguageRepository();

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

        /// <summary>
        /// Initializes everything the window needs, in both the update and create situation
        /// </summary>
        /// <param name="languageModel">The entity that will be used as Model for this page</param>
        private void InitializeWindow(Language languageModel)
        {
            // Define the progressbar and submit button to allow the TurnOn/OffProgressBarMethod to work
            progressBar = prog;
            submitButton = btnSubmit;

            _updatingPage = !languageModel.IsNew();            

            LanguageModel = languageModel;

            DataContext = this;

            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;
            SetLanguageDictionary();

            SetTitle();
        }
        
        protected override void SetTitle()
        {
            // Make the title label refer to the correct Resource of the XAML dictionary
            lblTitle.SetResourceReference(ContentProperty, _updatingPage ? "UpdateLanguageTitle" : "NewLanguageTitle");
        }

        protected async override void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TurnOnProgressBar();

                // Makes the language a desktopLanguage in case for some reason it didn't exist yet.
                if (LanguageModel.LocalName == "Nederlands" || LanguageModel.LocalName == "English")
                {
                    LanguageModel.IsDesktopLanguage = true;
                }

                if (ValidateModel())
                {
                    if (LanguageModel.IsDefault)
                    {
                        // If the language is default, check if there is already a default language

                        Language defaultLang = _langRepo.GetAllQueryable().SingleOrDefault(x => x.IsDefault);

                        if (defaultLang != null && defaultLang.ISO != LanguageModel.ISO)
                        {
                            // If there is already a default language, make sure the user wants to make this language the new default one

                            if (MessageBox.Show(LangResource.MBContMakeLangDefault, LangResource.MBTitleMakeLangDefault,
                            MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                // Makes the new language default
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

                TurnOffProgressBar();
            }
            catch (Exception)
            {
                TurnOffProgressBar();

                string content = String.Format(LangResource.MBContentObjSaveFailed, LangResource.TheLanguage.ToLower());
                string title = StringExtensions.FirstCharToUpper(String.Format(LangResource.MBTitleObjSaveFailed, LangResource.TheLanguage.ToLower()));

                MessageBox.Show(content, title);
            }
        }

        /// <summary>
        /// Saves the model
        /// </summary>
        /// <returns></returns>
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

                var window = (NavigationWindow)GetParentWindow();
                window.ResetAllUserControls();

                TriggerSaveEvent();
            }
        }

        private bool ValidateModel()
        {
            // TODO Validate language model

            if (LanguageModel.ISO.Length != 2)
            {
                return false;
            }

            return true;
        }
    }
}
