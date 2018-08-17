using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RudycommerceWeb.Controllers
{
    public class UtilitiesController : Base.CustomBaseController
    {
        public ActionResult TermsAndConditions(bool popup = false)
        {
            ViewBag.PopupWindow = popup;

            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult FAQ()
        {
            return View();
        }

        public ActionResult DeliverPrices()
        {
            return View();
        }

        public ActionResult DeliveryCountries()
        {
            return View();
        }
    }
}