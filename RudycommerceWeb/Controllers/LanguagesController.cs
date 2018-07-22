using RudycommerceData.Entities;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RudycommerceWeb.Controllers
{
    public class LanguagesController : Controller
    {
        private ILanguageRepository _langRepo;

        public LanguagesController()
        {
            _langRepo = new LanguageRepository();
        }

        public ActionResult _LanguageSelection()
        {
            List<Language> languages = _langRepo.GetAllQueryable().ToList();

            return View("_LanguageSelection", languages);
        }
    }
}