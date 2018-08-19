using System;
using System.Collections.Generic;
using System.Linq;
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
            ViewBag.CheckoutProgress = 1;

            _clientRepo = new ClientRepository();
        }
        
        [HttpGet]
        [IsCartFilledActionFilter]
        public ActionResult PersonalInfoChoice()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> PersonalInfoChoice(LoginModel loginModel)
        {
            int? clientID = await loginModel.Authenticate(_clientRepo);

            if (clientID != null)
            {
                AddClientCookie((int)clientID);
                return RedirectToAction("DeliveryOption", "Orders");
            }
            else
            {
                ModelState.AddModelError("", Resources.Global.LoginFailedCredentials);
            }

            ViewBag.LoginModel = loginModel;
            return View();
        }

        [HttpGet]
        [IsCartFilledActionFilter]
        public ActionResult PersonalInfoForm()
        {
            //ViewBag.HideCartOverview = true;

            Client clientModel = new Client();
            
            return View(clientModel);
        }

        [HttpPost]
        public async Task<ActionResult> PersonalInfoForm(Client client)
        {
            Client clientEntity = client;

            //ViewBag.HideCartOverview = true;

            // validate unique e-mail

            if (ModelState.IsValidField("Email") && _clientRepo.EmailTaken(clientEntity.Email))
            {
                ModelState.AddModelError("Email", Resources.Checkout.EmailNotUnique);
            }

            if (ModelState.IsValid)
            {
                _clientRepo.Add(clientEntity);
                await _clientRepo.SaveChangesAsync();
                AddClientCookie(client.ID);

                return RedirectToAction("DeliveryOption", "Orders");
            }
            else
            {
                ViewBag.ShowValidationSummary = true;
                return View("PersonalInfoForm", client);
            }           
        }

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