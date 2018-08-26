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
    public class LanguagesController : Base.CustomBaseController
    {
        private ILanguageRepository _langRepo;

        public LanguagesController()
        {
            _langRepo = new LanguageRepository();
        }

        /// <summary>
        /// Gets all the languages, which can be used for the language selector in the header's navbar
        /// </summary>
        /// <returns></returns>
        public ActionResult _LanguageSelection()
        {
            List<Language> languages = _langRepo.GetAll().ToList();

            return View("_LanguageSelection", languages);
        }
    }
}