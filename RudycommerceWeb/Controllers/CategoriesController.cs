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
    public class CategoriesController : MultilingualBaseController
    {
        private ICategoryRepository _catRepo;

        public CategoriesController()
        {
            _catRepo = new CategoryRepository();
        }

        public ActionResult _CategoryDropdown()
        {
            return View("_CategoryDropdown", _catRepo.GetLocalizedCatListItems(GetISO()));
        }

        public ActionResult _CategoryList()
        {
            return View("_CategoryList", _catRepo.GetLocalizedCatListItems(GetISO()));
        }
    }
}