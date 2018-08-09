using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RudycommerceData.Entities;
using RudycommerceWeb.Attributes;

namespace RudycommerceWeb.Controllers
{
    [CheckoutActionFilter]
    public class ClientsController : Controller
    {
        public ClientsController()
        {
            ViewBag.CheckoutProgress = 1;
        }
        
        public ActionResult PersonalInfoChoice()
        {
            return View();
        }

        [HttpGet]
        public ActionResult PersonalInfoForm()
        {
            ViewBag.HideCartOverview = true;

            Client clientModel = new Client();

            return View(clientModel);
        }

        [HttpPost]
        public ActionResult PersonalInfoForm(Client client)
        {
            return View("PersonalInfoForm", client);
        }
    }
}