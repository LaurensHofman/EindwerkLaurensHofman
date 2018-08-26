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
    public class CategoriesController : Base.MultilingualBaseController
    {
        private ICategoryRepository _catRepo;

        public CategoriesController()
        {
            _catRepo = new CategoryRepository();
        }

        /// <summary>
        /// Gets the list of categories for combobox items, to be used in the Category Dropdown in the Header's navbar
        /// </summary>
        /// <returns></returns>
        public ActionResult _CategoryDropdown()
        {
            return View("_CategoryDropdown", _catRepo.GetLocalizedCatListItems(GetISO()));
        }

        /// <summary>
        /// Gets a list of categories in links
        /// </summary>
        /// <returns></returns>
        public ActionResult _CategoryList()
        {
            return View("_CategoryList", _catRepo.GetLocalizedCatListItems(GetISO()));
        }
    }
}