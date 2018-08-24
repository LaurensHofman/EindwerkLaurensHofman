using RudycommerceData.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RudycommerceWPF.WindowsAndUserControls.Abstracts
{
    public class MultilingualUserControl : UserControl
    {
        public MultilingualUserControl()
        {

        }

        /// <summary>
        /// Defines the user's preferred display language
        /// </summary>
        protected Language _preferredLanguage;

        /// <summary>
        /// Sets the language Dictionary according to the user's preferred display language
        /// </summary>
        protected virtual void SetLanguageDictionary()
        {
            ResourceDictionary dict = new ResourceDictionary();

            string langDictionary;

            // chooses the correct dictionary for XAML

            switch (_preferredLanguage.LocalName)
            {
                case "Nederlands":
                    langDictionary = "..\\LanguageResources\\Dutch.xaml";
                    break;

                case "English":
                    langDictionary = "..\\LanguageResources\\English.xaml";
                    break;

                default:
                    langDictionary = "..\\LanguageResources\\Dutch.xaml";
                    break;
            }

            dict.Source = new Uri(langDictionary, UriKind.Relative);

            // Adds the dictionary so this one can be used

            this.Resources.MergedDictionaries.Add(dict);

            CultureInfo ci;

            // Chooses the correct culture according to the preferred display language

            switch (_preferredLanguage.LocalName)
            {
                case "Nederlands":
                    ci = CultureInfo.CreateSpecificCulture("nl");
                    break;

                case "English":
                    ci = CultureInfo.CreateSpecificCulture("en");
                    break;

                default:
                    ci = CultureInfo.CreateSpecificCulture("nl");
                    break;
            };

            // Sets the culture, so it can be used by resource files in C#

            Thread.CurrentThread.CurrentUICulture = ci;
            Thread.CurrentThread.CurrentCulture = ci;
            CultureInfo.DefaultThreadCurrentCulture = ci;
            CultureInfo.DefaultThreadCurrentUICulture = ci;
        }
    }
}
