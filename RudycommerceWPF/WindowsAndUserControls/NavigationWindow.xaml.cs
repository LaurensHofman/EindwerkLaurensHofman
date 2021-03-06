﻿using RudycommerceData.Entities;
using RudycommerceData.Entities.DesktopUsers;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceLib.Properties;
using RudycommerceWPF.WindowsAndUserControls.Abstracts;
using RudycommerceWPF.WindowsAndUserControls.Languages;
using RudycommerceWPF.WindowsAndUserControls.Orders;
using RudycommerceWPF.WindowsAndUserControls.Products.Brands;
using RudycommerceWPF.WindowsAndUserControls.Products.Categories;
using RudycommerceWPF.WindowsAndUserControls.Products.Products;
using RudycommerceWPF.WindowsAndUserControls.Products.Specifications;
using RudycommerceWPF.WindowsAndUserControls.Users;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RudycommerceWPF.WindowsAndUserControls
{
    /// <summary>
    /// Interaction logic for NavigationWindow.xaml
    /// </summary>
    public partial class NavigationWindow : MultilingualWindow
    {
        private IDesktopUserRepository _userRepo;
        private ILanguageRepository _langRepo;

        private DesktopUser _currentUser;

        public NavigationWindow()
        {
            InitializeComponent();
        }

        public NavigationWindow(int userID)
        {
            InitializeComponent();

            _userRepo = new DesktopUserRepository();
            _langRepo = new LanguageRepository();

            InitializeWindow(userID);

            // Opens the order overview on load
            menuOrderOverview(null, null);
        }

        private void InitializeWindow(int userID)
        {
            // Gets the user based on the user id sent by the login screen
            _currentUser = _userRepo.Get(userID);
            // Puts the user in global accessible settings
            Properties.Settings.Default.CurrentUser = _currentUser;
            // Gets the language based on the preferrence of the user
            _preferredLanguage = _currentUser.PreferredLanguage;

            // Enable the user tab if the current user is the admin
            EnableUserTab();

            // Sets the display language
            SetLanguageDictionary();
        }

        /// <summary>
        /// Enable the User tab for the admin
        /// </summary>
        private void EnableUserTab()
        {
            if (_currentUser.IsAdmin)
            {
                stackDesktopUser.IsEnabled = true;
                stackDesktopUser.Visibility = Visibility.Visible;
                sepDesktopUser.Visibility = Visibility.Visible;
            }
        }

        #region Methods to show the user controls

        /// <summary>
        /// Shows the user control for a Form
        /// </summary>
        /// <typeparam name="formUC">The FormUserControl that you want to show</typeparam>
        /// <typeparam name="overviewUC">The OverviewUserControl that you want to show on a submit of the form</typeparam>
        /// <param name="formContentControl">The ContentControl in which the FormUserControl has to be shown</param>
        /// <param name="overviewContentControl">The ContentControl in which the OverviewUserControl has to be shown</param>
        public void ShowFormUserControl<formUC, overviewUC>(ContentControl formContentControl, ContentControl overviewContentControl)
                                    where formUC : FormUserControl, new()
                                    where overviewUC : OverviewUserControl, new()
        {
            // Gets the User control (<Type>) that has to be shown, and defines it as a UserControl (inheritence from :LanguageUserControl)
            // and makes sure its instantiatable ( new() ).
            // Gets the ContentControl in which to show the new UserControl.

            // First hides all the ContentControls.
            HideAllUserControls();

            // Checks if the ContentControl has content (or if the content is visible (which is done by the cancel click inside the userControl)
            // If the contentControl's content hasn't yet been defined, make a new UserControl.
            // If the contentControl's content has been hidden (by a cancel click), then make a new UserControl as well.
            // Else: act normal (show the userControl). This allows it to work a little bit as tabs, the content is not lost when switching back and forth.

            if (formContentControl.Content == null || (formContentControl.Content as formUC).Visibility == Visibility.Collapsed)
            {
                formUC _content = new formUC
                {
                    OverviewContentControl = overviewContentControl
                };

                _content.CreateEvent += GoToOverview<overviewUC>;

                formContentControl.Content = _content;
            }

            // Make the ContentControl and the UserControl visible
            formContentControl.Visibility = Visibility.Visible;
            (formContentControl.Content as formUC).Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Shows an overview
        /// </summary>
        /// <typeparam name="ToBeShownUC">The OverviewUserControl that you want to show</typeparam>
        /// <param name="contentControl">The contentControl in which the OverviewUserControl has to be shown</param>
        private void ShowOverviewUserControl<ToBeShownUC>(ContentControl contentControl) where ToBeShownUC : OverviewUserControl, new()
        {
            // Gets the User control (<Type>) that has to be shown, and defines it as a UserControl (inheritence from :LanguageUserControl)
            // and makes sure its instantiatable ( new() ).
            // Gets the ContentControl in which to show the new UserControl.

            // First hides all the ContentControls.
            HideAllUserControls();

            // Checks if the ContentControl has content (or if the content is visible (which is done by the cancel click inside the userControl)
            // If the contentControl's content hasn't yet been defined, make a new UserControl.
            // If the contentControl's content has been hidden (by a cancel click), then make a new UserControl as well.
            // Else: act normal (show the userControl). This allows it to work a little bit as tabs, the content is not lost when switching back and forth.
            if (contentControl.Content == null || (contentControl.Content as ToBeShownUC).Visibility == Visibility.Collapsed)
            {
                ToBeShownUC _content = new ToBeShownUC();

                contentControl.Content = _content;
            }
            else
            {
                (contentControl.Content as ToBeShownUC).LoadDataGridData();
            }

            // Make the ContentControl and the UserControl visible
            contentControl.Visibility = Visibility.Visible;
            (contentControl.Content as ToBeShownUC).Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Show any UserControl, without any events
        /// </summary>
        /// <typeparam name="ToBeShownUC"></typeparam>
        /// <param name="contentControl"></param>
        private void ShowGeneralUserControl<ToBeShownUC>(ContentControl contentControl) where ToBeShownUC : MultilingualUserControl, new()
        {
            HideAllUserControls();

            if (contentControl.Content == null || (contentControl.Content as ToBeShownUC).Visibility == Visibility.Collapsed)
            {
                ToBeShownUC _content = new ToBeShownUC();

                contentControl.Content = _content;
            }

            // Make the ContentControl and the UserControl visible
            contentControl.Visibility = Visibility.Visible;
            (contentControl.Content as ToBeShownUC).Visibility = Visibility.Visible;
        }

        /// <summary>
        /// After a form submit, go to the overview user control
        /// </summary>
        /// <typeparam name="overviewUC"></typeparam>
        /// <param name="contentControl"></param>
        private void GoToOverview<overviewUC>(ContentControl contentControl) where overviewUC : OverviewUserControl, new()
        {
            ShowOverviewUserControl<overviewUC>(contentControl);
        }

        /// <summary>
        /// Hide all the user controls, except the Order overview, because that is the default one to always show when others are closed
        /// </summary>
        public void HideAllUserControls()
        {
            foreach (ContentControl contentControl in UserControls.Children)
            {
                contentControl.Visibility = Visibility.Collapsed;
            }

            ccOrders.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 'Refreshes' all the user controls
        /// </summary>
        public void ResetAllUserControls(string untouchedContentControlName = "")
        {
            // By setting the content control's content to null, the next time you are going to open that contentControl, 
            // it's going to make a new instance of the UserControl 

            foreach (ContentControl contentControl in UserControls.Children)
            {
                if (contentControl.Name != "ccOrders" && contentControl.Name != untouchedContentControlName)
                {
                    contentControl.Content = null;
                }
            }
        }

        #endregion

        #region Orders

        private void menuOrderOverview(object sender, RoutedEventArgs e)
        {
            ShowGeneralUserControl<OrderOverview>(ccOrders);
        }

        #endregion

        #region User settings

        private void menuSettings(object sender, RoutedEventArgs e)
        {
            HideAllUserControls();

            if (ccSettings.Content == null || (ccSettings.Content as AccountSettings).Visibility == Visibility.Collapsed)
            {
                AccountSettings _content = new AccountSettings();

                _content.OnAccountSave += ApplySettings;

                ccSettings.Content = _content;
            }

            ccSettings.Visibility = Visibility.Visible;
            (ccSettings.Content as AccountSettings).Visibility = Visibility.Visible;
        }

        private async void ApplySettings(DesktopUser desktopUser)
        {
            // Sets the preferredLanguage based on the one chosen in the settings page

            _preferredLanguage = await _langRepo.GetAsync((int)desktopUser.PreferredLanguageID);

            SetLanguageDictionary();

            foreach (ContentControl contentControl in UserControls.Children)
            {
                contentControl.Content = null;
            }

            menuOrderOverview(null, null);
        }

        #endregion

        #region Desktop users

        private void menuDesktopUserOverview(object sender, RoutedEventArgs e)
        {
            ShowGeneralUserControl<UserOverview>(ccUsers);
        }

        #endregion

        #region Languages

        private void menuAddLanguage(object sender, RoutedEventArgs e)
        {
            ShowFormUserControl<LanguageForm, LanguageOverview>(ccLanguageForm, ccLanguageOverview);
        }

        private void menuLanguageOverview(object sender, RoutedEventArgs e)
        {
            ShowOverviewUserControl<LanguageOverview>(ccLanguageOverview);
        }

        #endregion

        #region Products and others

        #region Brands

        private void menuAddBrand(object sender, RoutedEventArgs e)
        {
            ShowFormUserControl<BrandForm, BrandOverview>(ccBrandForm, ccBrandOverview);
        }

        private void menuBrandOverview(object sender, RoutedEventArgs e)
        {
            ShowOverviewUserControl<BrandOverview>(ccBrandOverview);
        }

        #endregion

        #region Categories

        private void menuAddCategory(object sender, RoutedEventArgs e)
        {
            ShowFormUserControl<CategoryForm, CategoryOverview>(ccCategoryForm, ccCategoryOverview);
        }

        private void menuCategoryOverview(object sender, RoutedEventArgs e)
        {
            ShowOverviewUserControl<CategoryOverview>(ccCategoryOverview);
        }

        #endregion

        #region Specifications

        private void menuAddSpecification(object sender, RoutedEventArgs e)
        {
            ShowFormUserControl<SpecificationForm, SpecificationOverview>(ccSpecificationForm, ccSpecificationOverview);
        }

        private void menuSpecificationOverview(object sender, RoutedEventArgs e)
        {
            ShowOverviewUserControl<SpecificationOverview>(ccSpecificationOverview);
        }

        #endregion

        #region Products

        private void menuAddProduct(object sender, RoutedEventArgs e)
        {
            ShowFormUserControl<ProductForm, ProductOverview>(ccProductForm, ccProductOverview);
        }

        private void menuProductOverview(object sender, RoutedEventArgs e)
        {
            ShowOverviewUserControl<ProductOverview>(ccProductOverview);
        }

        #endregion

        #endregion
    }
}
