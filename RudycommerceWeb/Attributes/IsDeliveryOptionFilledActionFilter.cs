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
    /// Checks if the delivery option cookie is correctly filled
    /// </summary>
    public class IsDeliveryOptionFilledActionFilter : IsClientFilledActionFilter
    {
        // Every place that requires a delivery option cookie to be filled, also needs a client filled.
        // So it executes the client validation first. If the ViewBag.ClientEmpty == null, means that the client is correctly filled
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // If ViewBag.ClientEmpty == null => Client is correctly filled (see IsClientFilledActionFilter)
            if (filterContext.Controller.ViewBag.ClientEmpty == null)
            {
                // Checks if a cookie for the delivery option exists
                if (filterContext.HttpContext.Request.Cookies[ConstVal.cookieDeliverOptionName] == null)
                {
                    RedirectToOtherPage(filterContext);
                }
                else
                {
                    // Checks if the cookie for the delivery option has a value
                    var cookie = filterContext.HttpContext.Request.Cookies[ConstVal.cookieDeliverOptionName];
                    if (cookie.Value == null ||
                        cookie.Value == "undefined")
                    {
                        RedirectToOtherPage(filterContext);
                    }
                    else
                    {
                        try
                        {
                            // Tries to parse the cookie value to a C# object
                            Delivery delivery = Newtonsoft.Json.JsonConvert.DeserializeObject<Delivery>(cookie.Value);
                        }
                        catch (Exception)
                        {
                            filterContext.HttpContext.Response.Cookies[ConstVal.cookieDeliverOptionName].Expires = DateTime.Now.AddDays(-1);
                            RedirectToOtherPage(filterContext);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Redirects to a fatting page, if the cookie wasn't correctly filled
        /// </summary>
        /// <param name="filterContext"></param>
        private void RedirectToOtherPage(ActionExecutingContext filterContext)
        {
            // Gets the controller
            var controller = (CustomBaseController)filterContext.Controller;
            // Redirects to a fitting page
            filterContext.Result = controller.RedirectToAction("DeliveryOption", "Orders");
        }
    }
}