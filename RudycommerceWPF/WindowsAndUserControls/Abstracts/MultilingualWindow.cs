using MahApps.Metro.Controls;
using RudycommerceData.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RudycommerceWPF.WindowsAndUserControls.Abstracts
{
    public abstract class MultilingualWindow : MetroWindow
    {
        /// <summary>
        /// Defines the user's preferred language
        /// </summary>
        protected Language _preferredLanguage;

        /// <summary>
        /// Sets the dictionary and Culture so it can be used by resource files
        /// </summary>
        protected virtual void SetLanguageDictionary()
        {
            ResourceDictionary dict = new ResourceDictionary();

            // Chooses the correct 

            string langDictionary;

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

            // Adds the dictionary so it can be used as a Resource in XAML

            this.Resources.MergedDictionaries.Add(dict);

            CultureInfo ci;

            // Gets the culture according to the user's preferred language

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

            // Sets the culture, so it can be used to get the correct ResourceFiles in C#

            Thread.CurrentThread.CurrentUICulture = ci;
            Thread.CurrentThread.CurrentCulture = ci;
            CultureInfo.DefaultThreadCurrentCulture = ci;
            CultureInfo.DefaultThreadCurrentUICulture = ci;
        }
    }
}
