using RudycommerceData.Models.ASPModels;
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
                else
                {
                    try
                    {
                        CartFromJSON cart = Newtonsoft.Json.JsonConvert.DeserializeObject<CartFromJSON>(cookie.Value);
                    }
                    catch (Exception)
                    {
                        filterContext.HttpContext.Response.Cookies[ConstVal.cookieCartName].Expires = DateTime.Now.AddDays(-1);
                        RedirectToOtherPage(filterContext);
                    }
                }
            }
        }

        private void RedirectToOtherPage(ActionExecutingContext filterContext)
        {
            var controller = (CustomBaseController)filterContext.Controller;
            controller.ViewBag.CartEmpty = true;
            filterContext.Result = controller.RedirectToAction("CartOverview", "Products");
        }
    }
}