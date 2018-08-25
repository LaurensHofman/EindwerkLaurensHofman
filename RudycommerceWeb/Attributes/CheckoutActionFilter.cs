using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RudycommerceWeb.Attributes
{
    /// <summary>
    /// Hides the Shopping Cart in the checkout
    /// </summary>
    public class CheckoutActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.Controller.ViewBag.HideCart = true;
        }
    }
}