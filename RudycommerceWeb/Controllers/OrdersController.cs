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
using RudycommerceLib.Notify;
using RudycommerceLib.CustomExceptions;

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
            // Defines the progress, which is shown in the arrows of the _checkoutLayout
            ViewBag.CheckoutProgress = 2;

            var client = _client;

            // Gives the delivery the default value of the client address
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
            // Gets the sent delivery option by the user

            ViewBag.CheckoutProgress = 2;

            if (ModelState.IsValid || !delivery.OtherAddress)
            {
                // If the model has no errors
                // Parse the selected delivery option to JSON
                var jsonDelivery = Newtonsoft.Json.JsonConvert.SerializeObject(delivery);

                // Create a cookie holding the delivery option
                HttpCookie deliveryCookie = new HttpCookie(ConstVal.cookieDeliverOptionName)
                {
                    Value = jsonDelivery,
                    Expires = DateTime.Now.AddDays(1)
                };
                Response.Cookies.Add(deliveryCookie);

                // Send the user to the next step
                return RedirectToAction("Payment");
            }
            else
            {
                // Enables to show the validationSummary
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
            // Gets the language ISO (to give to Stripe, to enable a localized payment popup)
            ViewBag.LangISO = GetISO();

            // Defines the current progress in the checkout process, which will be shown by the arrows in the _CheckoutLayout
            ViewBag.CheckoutProgress = 3;

            // Gets the totalprice from the products in the cart saved in the cookie
            ViewBag.totalPrice = GetTotalPriceFromCookieCart();

            // Gets the stripePublishKey and puts it in the ViewBag for later use
            var stripePublishKey = ConfigurationManager.AppSettings["stripePublishableKey"];
            ViewBag.StripePublishKey = stripePublishKey;

            return View();
        }

        [CheckoutActionFilter]
        public ActionResult Charge(string stripeEmail, string stripeToken)
        {
            // TODO Make less dependent on Stripe

            // Gets the total price
            var totalPrice = GetTotalPriceFromCookieCart();
            int priceInCents = (int)(totalPrice * 100);

            // Gets the information about the customers from Stripe
            var customers = new StripeCustomerService();
            var customer = customers.Create(new StripeCustomerCreateOptions
            {
                Email = stripeEmail,
                SourceToken = stripeToken
            });

            // Gets the information about the charges made from Stripe
            var charges = new StripeChargeService();
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

                // Gets the deliveryOption from the cookie
                Delivery deliveryOption = Newtonsoft.Json.JsonConvert.DeserializeObject<Delivery>(Request.Cookies[ConstVal.cookieDeliverOptionName].Value);

                IncomingOrder order;
                
                if (deliveryOption.OtherAddress)
                {
                    // If the client wants to use another address
                    
                    // Get the client ID
                    int clientID = int.Parse(RudycommerceLib.Security.Encryption.DecryptString(Request.Cookies[ConstVal.cookieClientIDName].Value));

                    // Pass the inserted deliveryOption to the incoming order
                    order = new IncomingOrder(deliveryOption)
                    {
                        ClientID = clientID
                    };
                }
                else
                {
                    // If the client wants to use his home address for delivery

                    // Gets the client
                    var client = _client;

                    // Adds the client with his address to the incoming order
                    order = new IncomingOrder(client)
                    {
                        ClientID = client.ID
                    };
                }

                // Gives a status code 0 ( = Ordered, but not yet picked)
                order.StatusCode = 0;

                // Adds a paymentComplete, paymentOption and totalprice
                order.PaymentComplete = true;
                order.PaymentOption = charge.Source.Card.Brand;
                order.TotalPrice = totalPrice;

                // Gets the products from the cart cookie and adds them as order lines)
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

                // Creates the order and saves it
                _incOrderRepo.Add(order);
                _incOrderRepo.SaveChangesAsync();

                try
                {
                    string productsString = "";
                    foreach (var prod in cart.ProductList)
                    {
                        productsString += prod.Quantity.ToString() + " x " + prod.Name + "\r\n";
                    }

                    string title = Resources.Checkout.OrderEmailTitle;
                    string content = string.Format(Resources.Checkout.OrderEmailContent, _client.FullName, productsString, deliveryOption.StreetAndNumber,
                        deliveryOption.MailBox, deliveryOption.PostalCode, deliveryOption.City);

                    GmailNotifier gmail = new GmailNotifier();
                    gmail.Notify(new System.Net.Mail.MailAddress(_client.Email), title, content);
                }
                catch (EmailSentFailed)
                {
                    
                }
                catch (Exception)
                {
                    throw;
                }                

                // Redirects to the Thank you page
                return RedirectToAction("OrderFinished", "Orders", null);
            }
            else
            {
                // If the payment failed, send back to the payment page
                return Payment();
            }
        }

        [HttpGet]
        public ActionResult OrderFinished()
        {
            // Clear cart
            // Clear user id
            // Clear delivery

            DeleteCookie(ConstVal.cookieCartName);
            DeleteCookie(ConstVal.cookieClientIDName);
            DeleteCookie(ConstVal.cookieDeliverOptionName);

            return View();
        }

        private Decimal GetTotalPriceFromCookieCart()
        {
            // TODO Error

            // Gets a list of IDs from the products in the cart (can contain duplicates which means higher quantity)
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