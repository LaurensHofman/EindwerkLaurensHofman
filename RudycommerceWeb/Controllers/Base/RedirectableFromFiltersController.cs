using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RudycommerceWeb.Controllers.Base
{
    //https://stackoverflow.com/questions/5453338/redirect-from-action-filter-attribute
    public abstract class RedirectableFromFiltersController : Controller
    {
        public new RedirectToRouteResult RedirectToAction(string action, string controller)
        {
            return base.RedirectToAction(action, controller);
        }
    }
}