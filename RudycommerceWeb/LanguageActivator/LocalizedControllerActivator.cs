using RudycommerceData.Repositories.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RudycommerceWeb.LanguageActivator
{
    /// <summary>
    /// Gets the language the user wants to see while creating the controller.
    /// </summary>
    public class LocalizedControllerActivator : IControllerActivator
    {
        // https://www.ryadel.com/en/setup-a-multi-language-website-using-asp-net-mvc/

        private LanguageRepository _langRepo = new LanguageRepository();

        private string _DefaultLanguage = "en";

        public IController Create(RequestContext requestContext, Type controllerType)
        {
            //Get the {language} parameter in the RouteData
            string lang = (string)requestContext.RouteData.Values["lang"] ?? _DefaultLanguage;

            // If the language is not english
            if (lang != _DefaultLanguage)
            {
                try
                {
                    // Verify whether the language is supported
                    if (_langRepo.GetAll().Select(x => x.ISO).Contains(lang))
                    {
                        Thread.CurrentThread.CurrentCulture =
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
                    }
                    // Else, use english as backup language
                    else
                    {
                        Thread.CurrentThread.CurrentCulture =
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(_DefaultLanguage);
                    }
                }
                catch (Exception e)
                {
                    throw new NotSupportedException(String.Format("ERROR: Invalid language code '{0}'.", lang));
                }
            }

            return DependencyResolver.Current.GetService(controllerType) as IController;
        }
    }
}