using RudycommerceData.Entities.Products.Categories;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace RudycommerceWeb.Controllers
{
    public class CategoriesController : Controller
    {
        private ICategoryRepository _catRepo;

        public CategoriesController()
        {
            _catRepo = new CategoryRepository();
        }

        public ActionResult _CategoryDropdown()
        {
            string iso = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

            return View("_CategoryDropdown", _catRepo.GetLocalizedCatListItems(iso));
        }
    }
}