using RudycommerceData.Models.ASPModels;
using RudycommerceWeb.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RudycommerceWeb.Attributes
{
    /// <summary>
    /// Validates whether the cart is filled. Redirects if not
    /// </summary>
    public class IsCartFilledActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Checks if there is a cookie for the cart
            if (filterContext.HttpContext.Request.Cookies[ConstVal.cookieCartName] == null)
            {
                RedirectToOtherPage(filterContext);
            }
            else
            {
                // If a cookie exists, check if it has a value
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
                        // If the cookie exists, and has a value, check if it can be Deserialized to a c# object
                        CartFromJSON cart = Newtonsoft.Json.JsonConvert.DeserializeObject<CartFromJSON>(cookie.Value);
                    }
                    catch (Exception)
                    {
                        // If the value is corrupted, delete the cookie
                        filterContext.HttpContext.Response.Cookies[ConstVal.cookieCartName].Expires = DateTime.Now.AddDays(-1);
                        RedirectToOtherPage(filterContext);
                    }
                }
            }
        }

        /// <summary>
        /// Redirects to a fitting page
        /// </summary>
        /// <param name="filterContext"></param>
        private void RedirectToOtherPage(ActionExecutingContext filterContext)
        {
            // Gets the controller
            var controller = (CustomBaseController)filterContext.Controller;
            // Defines the cart was empty
            controller.ViewBag.CartEmpty = true;
            // Redirects the page to the cart overview
            filterContext.Result = controller.RedirectToAction("CartOverview", "Products");
        }
    }
}