using RudycommerceWeb.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RudycommerceWeb.Attributes
{
    /// <summary>
    /// Checks if the client cookie is filled
    /// </summary>
    public class IsClientFilledActionFilter : IsCartFilledActionFilter
    {
        // Every place that requires a client cookie to be filled, also needs a cart filled.
        // So it executes the cart validation first. If the ViewBag.CartEmpty == null, means that the cart is correctly filled
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // If ViewBag.CartEmpty == null => Cart is correctly filled (see IsCartFilledActionFilter)
            if (filterContext.Controller.ViewBag.CartEmpty == null)
            {
                // Checks if a cookie for the client exists
                if (filterContext.HttpContext.Request.Cookies[ConstVal.cookieClientIDName] == null)
                {
                    RedirectToOtherPage(filterContext);
                }
                else
                {
                    // Checks if the cookie for the client has a value
                    var cookie = filterContext.HttpContext.Request.Cookies[ConstVal.cookieClientIDName];
                    if (cookie.Value == null ||
                        cookie.Value == "undefined")
                    {
                        RedirectToOtherPage(filterContext);
                    }
                    else
                    {
                        try
                        {
                            // Tries to parse the cookie value to an integer (after decrypting it)
                            int clientID = int.Parse(RudycommerceLib.Security.Encryption.DecryptString(cookie.Value));
                        }
                        catch (Exception e)
                        {
                            filterContext.HttpContext.Response.Cookies[ConstVal.cookieClientIDName].Expires = DateTime.Now.AddDays(-1);
                            RedirectToOtherPage(filterContext);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Redirects to a fitting page, if the cookie wasn't correctly filled
        /// </summary>
        /// <param name="filterContext"></param>
        private void RedirectToOtherPage(ActionExecutingContext filterContext)
        {
            // Gets the controller
            var controller = (CustomBaseController)filterContext.Controller;
            // Defines the client cookie wasn't filled correctly
            controller.ViewBag.ClientEmpty = true;
            // Redirects to a fitting page
            filterContext.Result = controller.RedirectToAction("PersonalInfoChoice", "Clients");
        }
    }
}