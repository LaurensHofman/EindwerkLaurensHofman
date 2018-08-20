﻿using RudycommerceData.Models.ASPModels;
using RudycommerceWeb.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RudycommerceWeb.Attributes
{
    public class IsDeliveryOptionFilledActionFilter : IsClientFilledActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (filterContext.Controller.ViewBag.ClientEmpty == null)
            {
                if (filterContext.HttpContext.Request.Cookies[ConstVal.cookieDeliverOptionName] == null)
                {
                    RedirectToOtherPage(filterContext);
                }
                else
                {
                    var cookie = filterContext.HttpContext.Request.Cookies[ConstVal.cookieDeliverOptionName];
                    if (cookie.Value == null  || 
                        cookie.Value == "undefined")
                    {
                        RedirectToOtherPage(filterContext);
                    }
                    else
                    {
                        try
                        {
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

        private void RedirectToOtherPage(ActionExecutingContext filterContext)
        {
            var controller = (CustomBaseController)filterContext.Controller;
            filterContext.Result = controller.RedirectToAction("DeliveryOption", "Orders");
        }
    }
}