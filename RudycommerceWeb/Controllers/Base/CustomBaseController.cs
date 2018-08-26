using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RudycommerceWeb.Controllers.Base
{
    //https://stackoverflow.com/questions/5453338/redirect-from-action-filter-attribute
    public abstract class CustomBaseController : Controller
    {
        /// <summary>
        /// Makes a new RedirectToAction method because the one from the Controller class is protected.
        /// This allows me to redirect from for example ActionFilters
        /// </summary>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public new RedirectToRouteResult RedirectToAction(string action, string controller)
        {
            return base.RedirectToAction(action, controller);
        }
    }
}