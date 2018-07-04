using RudycommerceData.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RudycommerceWPF.WindowsAndUserControls.Abstracts
{
    public class LanguageUserControl : UserControl
    {
        protected Language _preferredLanguage;

        protected virtual void SetLanguageDictionary()
        {
            ResourceDictionary dict = new ResourceDictionary();

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

            this.Resources.MergedDictionaries.Add(dict);
        }
    }
}
