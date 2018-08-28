using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using RudycommerceData.Entities;
using RudycommerceData.Models.ASPModels;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceLib.Security;
using RudycommerceWeb.Attributes;

namespace RudycommerceWeb.Controllers
{
    [CheckoutActionFilter]
    public class ClientsController : Base.CustomBaseController
    {
        private IClientRepository _clientRepo; 

        public ClientsController()
        {
            // Defines the checkout progress, which can be shown in 3 arrows
            ViewBag.CheckoutProgress = 1;

            _clientRepo = new ClientRepository();
        }
        
        /// <summary>
        /// Gets the page that allows the client to either log, register, forgot password... or either to continue to a client form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [IsCartFilledActionFilter]
        public ActionResult PersonalInfoChoice()
        {
            return View();
        }

        /// <summary>
        /// When attempting to log in
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> PersonalInfoChoice(LoginModel loginModel)
        {
            // Authenticates the client
            int? clientID = await loginModel.Authenticate(_clientRepo);

            // If the client ID is not null, that means the authentication was succesful
            if (clientID != null)
            {
                // Create a cookie and redirect to the next step
                AddClientCookie((int)clientID);
                return RedirectToAction("DeliveryOption", "Orders");
            }
            else
            {
                // If the client ID is null, that means that either the password was wrong, or that no user was found
                ModelState.AddModelError("", Resources.Global.LoginFailedCredentials);
            }

            ViewBag.LoginModel = loginModel;
            return View();
        }

        /// <summary>
        /// Gets a form for the client to fill in
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [IsCartFilledActionFilter]
        public ActionResult PersonalInfoForm()
        {
            Client clientModel = new Client();
            
            return View(clientModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PersonalInfoForm(Client client)
        {
            Client clientEntity = client;

            // Validates whether the email is unique
            if (ModelState.IsValidField("Email") && _clientRepo.EmailTaken(clientEntity.Email))
            {
                ModelState.AddModelError("Email", Resources.Checkout.EmailNotUnique);
            }

            // If there are no validation errors
            if (ModelState.IsValid)
            {
                // Save the client and create a cookie
                _clientRepo.Add(clientEntity);
                await _clientRepo.SaveChangesAsync();
                AddClientCookie(client.ID);

                // Redirect to the next step
                return RedirectToAction("DeliveryOption", "Orders");
            }
            else
            {
                // Enables to show the validation summary
                ViewBag.ShowValidationSummary = true;
                // Show the form again
                return View("PersonalInfoForm", client);
            }           
        }

        /// <summary>
        /// Creates a cookie containing the encrypted Client ID
        /// </summary>
        /// <param name="clientID"></param>
        private void AddClientCookie(int clientID)
        {
            HttpCookie clientIDCookie = new HttpCookie(ConstVal.cookieClientIDName)
            {
                Value = Encryption.EncryptString(clientID.ToString()),
                Expires = DateTime.Now.AddDays(1)
            };

            Response.Cookies.Add(clientIDCookie);
        }
    }
}