using RudycommerceWeb.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RudycommerceWeb.Attributes
{
    public class IsClientFilledActionFilter : IsCartFilledActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (filterContext.Controller.ViewBag.CartEmpty == null)
            {
                if (filterContext.HttpContext.Request.Cookies[ConstVal.cookieClientIDName] == null)
                {
                    RedirectToOtherPage(filterContext);
                }
                else
                {
                    var cookie = filterContext.HttpContext.Request.Cookies[ConstVal.cookieClientIDName];
                    if (cookie.Value == null ||
                        cookie.Value == "undefined")
                    {
                        RedirectToOtherPage(filterContext);
                    }
                }
            }
        }

        private void RedirectToOtherPage(ActionExecutingContext filterContext)
        {
            var controller = (CustomBaseController)filterContext.Controller;
            controller.ViewBag.ClientEmpty = true;
            filterContext.Result = controller.RedirectToAction("PersonalInfoChoice", "Clients");
        }
    }
}