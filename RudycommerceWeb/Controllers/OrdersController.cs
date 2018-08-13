using RudycommerceData.Entities;
using RudycommerceData.Models.ASPModels;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Common;
using System.Data;
using System.Web.Security;

namespace RudycommerceWeb.Controllers
{
    public class OrdersController : Controller
    {
        private IClientRepository _clientRepo;
        private IProductRepository _prodRepo;
        private Client _client
        {
            get
            {
                try
                {
                    int id = int.Parse((Request.Cookies["clientID"].Value));
                    return _clientRepo.Get(id);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
        private readonly string cookieCartName = "shoppingCartRudyCommerce";

        public OrdersController()
        {
            _clientRepo = new ClientRepository();
            _prodRepo = new ProductRepository();
        }

        [HttpGet]
        public ActionResult DeliveryOption()
        {
            ViewBag.CheckoutProgress = 2;

            var client = _client;

            Delivery delivery = new Delivery
            {
                StreetAndNumber = client.StreetAndNumber,
                City = client.City,
                CountryCode = client.CountryCode,
                MailBox = client.MailBox,
                PostalCode = client.PostalCode
            };

            return View(delivery);
        }

        [HttpPost]
        public ActionResult DeliveryOption(Delivery delivery)
        {
            ViewBag.CheckoutProgress = 2;

            if (ModelState.IsValid)
            {
                return RedirectToAction("Payment");
            }
            else
            {
                ViewBag.ShowValidationSummary = true;
                return View(delivery);
            }            
        }

        [HttpGet]
        public ActionResult Payment()
        {
            Decimal woop = GetTotalPriceFromCookieCart();

            return View();
        }

        private Decimal GetTotalPriceFromCookieCart()
        {
            // TODO Error

            CartFromJSON cart = Newtonsoft.Json.JsonConvert.DeserializeObject<CartFromJSON>(Request.Cookies[cookieCartName].Value);

            List<int> IDs = new List<int>();
            foreach (var prod in cart.ProductList)
            {
                for (int i = 0; i < prod.Quantity; i++)
                {
                    IDs.Add(prod.ID);
                }
            }

            return _prodRepo.GetTotalPrice(IDs);
        }

        //public ActionResult DeliveryOption()
        //{
        //    var stripePublishKey = ConfigurationManager.AppSettings["stripePublishableKey"];
        //    ViewBag.StripePublishKey = stripePublishKey;
        //    return View();
        //}

        //public ActionResult Charge(string stripeEmail, string stripeToken)
        //{
        //    var customers = new StripeCustomerService();
        //    var charges = new StripeChargeService();

        //    var customer = customers.Create(new StripeCustomerCreateOptions
        //    {
        //        Email = stripeEmail,
        //        SourceToken = stripeToken
        //    });

        //    var charge = charges.Create(new StripeChargeCreateOptions
        //    {
        //        Amount = 500,//charge in cents
        //        Description = "Sample Charge",
        //        Currency = "usd",
        //        CustomerId = customer.Id
        //    });

        //    // further application specific code goes here

        //    return View();
        //}
        //        @using(Html.BeginForm("Charge", "Orders", FormMethod.Post))
        //{
        //    <article>
        //        <label>Amount: $5.00</label>
        //    </article>
        //    <script src = "//checkout.stripe.com/v2/checkout.js"
        //            class="stripe-button"
        //            data-key="@ViewBag.StripePublishKey"
        //            data-currency="eur"
        //            data-locale="auto"
        //            data-description="Sample Charge"
        //            data-amount="500">
        //    </script>
        //}
        //@using Stripe;
    }
}