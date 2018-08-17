using RudycommerceWeb.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RudycommerceWeb.Attributes
{
    public class IsCartFilledActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.Cookies[ConstVal.cookieCartName] == null)
            {
                RedirectToOtherPage(filterContext);
            }
            else
            {
                var cookie = filterContext.HttpContext.Request.Cookies[ConstVal.cookieCartName];
                if (cookie.Value == null ||
                    cookie.Value == "undefined")
                {
                    RedirectToOtherPage(filterContext);
                }

                // TODO Try to parse cookie value to relevant object,
                // If failed, clear cookie and redirect to other page
            }
        }

        private void RedirectToOtherPage(ActionExecutingContext filterContext)
        {
            var controller = (CustomBaseController)filterContext.Controller;
            controller.ViewBag.CartEmpty = true;
            // TODO Redirect to something else than index page (same for other action filters)
            filterContext.Result = controller.RedirectToAction("CartOverview", "Products");
        }
    }
}