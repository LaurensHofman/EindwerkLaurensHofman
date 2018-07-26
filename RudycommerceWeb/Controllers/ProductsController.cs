using RudycommerceData;
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
        private ICategoryRepository _catRepo;

        public ProductsController()
        {
            _prodRepo = new ProductRepository();
            _catRepo = new CategoryRepository();
        }

        // GET: Products
        public ActionResult Index()
        {
            ViewBag.IsHome = true;

            return View();
        }

        public ActionResult NewestProducts()
        {
            List<ProductListItem> products = _prodRepo.GetProductListItems(GetISO(), "new") ;

            return View("_ProductList", products);
        }

        public ActionResult BestSellingProducts()
        {
            List<ProductListItem> products = _prodRepo.GetProductListItems(GetISO(), "bestsell");

            return View("_ProductList", products);
        }        

        [HttpPost]
        public ActionResult Search(string searchQuery)
        {
            List<ProductListItem> products = _prodRepo.GetProductListItems(GetISO(), "search", searchQuery);

            ViewBag.SearchQuery = searchQuery;
            ViewBag.Title = "\"" + searchQuery + "\"";
            ViewBag.Products = products;

            return View("SearchResultPage");
        }

        public ActionResult CategoryPage(int id)
        {
            List<ProductListItem> products = _prodRepo.GetProductListItems(GetISO(), "category", id.ToString());

            ViewBag.Products = products;
            ViewBag.Title = _catRepo.GetLocalizedCatListItems(GetISO()).First(x => x.CategoryID == id).LocalizedPluralName;

            return View("CategoryPage"); 
        }        
    }
}