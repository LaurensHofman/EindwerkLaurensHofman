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
using RudycommerceData.Entities.Orders;
using RudycommerceWeb.Attributes;
using RudycommerceLib.CustomAttributes;

namespace RudycommerceWeb.Controllers
{
    public class OrdersController : Base.MultilingualBaseController
    {
        private IClientRepository _clientRepo;
        private IProductRepository _prodRepo;
        private IIncOrderRepository _incOrderRepo;
        private Client _client
        {
            get
            {
                try
                {
                    int id = int.Parse(RudycommerceLib.Security.Encryption.DecryptString(Request.Cookies[ConstVal.cookieClientIDName].Value));
                    return _clientRepo.Get(id);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public OrdersController()
        {
            _clientRepo = new ClientRepository();
            _prodRepo = new ProductRepository();
            _incOrderRepo = new IncOrderRepository();
        }

        [HttpGet]
        [CheckoutActionFilter]
        [IsClientFilledActionFilter]
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
        [CheckoutActionFilter]
        public ActionResult DeliveryOption(Delivery delivery)
        {
            ViewBag.CheckoutProgress = 2;

            if (ModelState.IsValid)
            {
                var jsonDelivery = Newtonsoft.Json.JsonConvert.SerializeObject(delivery);

                HttpCookie deliveryCookie = new HttpCookie(ConstVal.cookieDeliverOptionName);
                deliveryCookie.Value = jsonDelivery;
                deliveryCookie.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(deliveryCookie);

                return RedirectToAction("Payment");
            }
            else
            {
                ViewBag.ShowValidationSummary = true;
                return View(delivery);
            }            
        }

        [HttpGet]
        [DontSavePageInCache]
        [CheckoutActionFilter]
        [IsDeliveryOptionFilledActionFilter]
        public ActionResult Payment()
        {
            ViewBag.LangISO = GetISO();

            ViewBag.CheckoutProgress = 3;

            ViewBag.totalPrice = GetTotalPriceFromCookieCart();

            var stripePublishKey = ConfigurationManager.AppSettings["stripePublishableKey"];
            ViewBag.StripePublishKey = stripePublishKey;

            return View();
        }

        [CheckoutActionFilter]
        public ActionResult Charge(string stripeEmail, string stripeToken)
        {
            // TODO Make less dependent on Stripe

            var totalPrice = GetTotalPriceFromCookieCart();
            int priceInCents = (int)(totalPrice * 100);

            var customers = new StripeCustomerService();
            var charges = new StripeChargeService();

            var customer = customers.Create(new StripeCustomerCreateOptions
            {
                Email = stripeEmail,
                SourceToken = stripeToken
            });

            var charge = charges.Create(new StripeChargeCreateOptions
            {
                Amount = priceInCents,//charge in cents
                Description = "Sample Charge",
                Currency = "eur",
                CustomerId = customer.Id
            });

            if (charge.Paid)
            {
                // TODO ERROR

                Delivery deliveryOption = Newtonsoft.Json.JsonConvert.DeserializeObject<Delivery>(Request.Cookies[ConstVal.cookieDeliverOptionName].Value);

                IncomingOrder order;

                if (deliveryOption.OtherAddress)
                {
                    int clientID = int.Parse(Request.Cookies[ConstVal.cookieClientIDName].Value);

                    order = new IncomingOrder(deliveryOption)
                    {
                        ClientID = clientID
                    };
                }
                else
                {
                    var client = _client;

                    order = new IncomingOrder(client)
                    {
                        ClientID = client.ID
                    };
                }

                order.PaymentComplete = true;
                order.PaymentOption = charge.Source.Card.Brand;
                order.TotalPrice = totalPrice;

                var cart = GetCartFromCookie();
                foreach (var item in cart.ProductList)
                {
                    order.IncomingOrderLines.Add(new IncomingOrderLines
                    {
                        ProductID = item.ID,
                        ProductQuantity = item.Quantity,
                        ProductUnitPrice = item.Price / 100
                    });
                }

                _incOrderRepo.Add(order);
                _incOrderRepo.SaveChangesAsync();

                return RedirectToAction("OrderFinished", "Orders", null);
            }
            else
            {
                return Payment();
            }
        }

        [HttpGet]
        public ActionResult OrderFinished()
        {
            // Clear cart
            // Clear user id (except when logged in)
            // Clear delivery

            DeleteCookie(ConstVal.cookieCartName);
            DeleteCookie(ConstVal.cookieClientIDName);
            DeleteCookie(ConstVal.cookieDeliverOptionName);

            return View();
        }

        private Decimal GetTotalPriceFromCookieCart()
        {
            // TODO Error

            CartFromJSON cart = Newtonsoft.Json.JsonConvert.DeserializeObject<CartFromJSON>(Request.Cookies[ConstVal.cookieCartName].Value);

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

        private CartFromJSON GetCartFromCookie()
        {
            // TODO Error

            return Newtonsoft.Json.JsonConvert.DeserializeObject<CartFromJSON>(Request.Cookies[ConstVal.cookieCartName].Value);
        }

        private void DeleteCookie(string cookieName)
        {
            if (Request.Cookies[cookieName] != null)
            {
                Response.Cookies[cookieName].Expires = DateTime.Now.AddDays(-1);
            }
        }
    }
}