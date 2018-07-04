﻿using RudycommerceData.Entities;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceLib.CustomExceptions;
using RudycommerceLib.Properties;
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
    public partial class LanguageForm : LanguageUserControl
    {
        // TODO Validation (incl. 2 letter ISO)

        public Language LanguageModel { get; set; }
        private ILanguageRepository _langRepo;

        private bool _updatingPage = false;

        public LanguageForm() : this(new RudycommerceData.Entities.Language()) { }

        public LanguageForm(Language languageModel)
        {
            InitializeComponent();

            _updatingPage = ! languageModel.IsNew();

            _langRepo = new LanguageRepository();

            LanguageModel = languageModel;

            DataContext = this;

            _preferredLanguage = Properties.Settings.Default.CurrentUser.PreferredLanguage;

            SetLanguageDictionary();

            SetTitle();
        }

        private void SetTitle()
        {
            lblTitle.SetResourceReference(ContentProperty, _updatingPage ? "UpdateLanguageTitle" : "NewLanguageTitle");
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateModel())
                {
                    if (LanguageModel.IsDefault)
                    {
                        Language lang = _langRepo.GetAllQueryable().SingleOrDefault(x => x.IsDefault);

                        if (lang != null)
                        {
                            if (MessageBox.Show(LangResource.MBContMakeLangDefault, LangResource.MBTitleMakeLangDefault,
                            MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                lang.IsDefault = false;
                                await _langRepo.UpdateAsync(lang);
                                _langRepo.AddAsync(LanguageModel);

                                await _langRepo.SaveChangesAsync();
                            }
                            else
                            {
                                LanguageModel.IsDefault = false;
                            }
                        }
                        else
                        {
                            _langRepo.AddAsync(LanguageModel);
                            await _langRepo.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        _langRepo.AddAsync(LanguageModel);
                        await _langRepo.SaveChangesAsync();
                    }
                }                
            }
            catch (Exception)
            {

                throw;
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
