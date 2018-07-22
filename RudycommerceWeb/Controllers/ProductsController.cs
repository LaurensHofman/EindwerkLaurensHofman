using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace RudycommerceWeb.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products
        public ActionResult Index()
        {
            ViewBag.IsHome = true;

            return View();
        }

        private static string GetISO()
        {
            return Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
        }
    }
}