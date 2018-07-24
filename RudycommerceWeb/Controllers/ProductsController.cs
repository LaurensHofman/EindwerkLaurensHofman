using RudycommerceData.Models.ASPModels;
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
    public class ProductsController : MultilingualBaseController
    {
        private IProductRepository _prodRepo;

        public ProductsController()
        {
            _prodRepo = new ProductRepository();
        }

        // GET: Products
        public ActionResult Index()
        {
            ViewBag.IsHome = true;

            return View();
        }

        public ActionResult NewestProducts()
        {
            List<ProductListItem> products = _prodRepo.GetHomepageItems(GetISO(), "new") ;

            return View("ProductList", products);
        }
    }
}