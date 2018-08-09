using RudycommerceData;
using RudycommerceData.Models.ASPModels;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceWeb.Attributes;
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
        private readonly string cookieCartName = "shoppingCartRudyCommerce";

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
            List<ProductListItem> products = _prodRepo.GetProductListItems(GetISO(), "new");

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

        public ActionResult Details(int ID)
        {
            RudycommerceData.Models.ASPModels.ProductDetailsPageItem details = _prodRepo.GetProductDetails(GetISO(), ID);

            ViewBag.Title = details.ProductInfo.Name;

            return View("Details", details);
        }

        [HttpPost]
        public ActionResult Cart(List<CartItem> cartItems)
        {
            return RedirectToAction("CartOverview", "Products");
        }

        [HttpGet]
        [CheckoutActionFilter]
        public ActionResult CartOverview()
        {
            try
            {
                if (Request.Cookies[cookieCartName] != null)
                {
                    return View("CartOverview", GetCartItemsFromCookie());
                }
                else
                {
                    return View("CartOverview");
                }
            } 
            catch (Exception)
            {
                // TODO ErrorPage or smth else (as well in the else{} above)
                return View("CartOverview");
            }
        }

        [HttpPost]
        public ActionResult CartOverview(CartOverviewItem cartOverviewItems)
        {
            // TODO Validate
            CartFromJSON cartItems = Newtonsoft.Json.JsonConvert.DeserializeObject<CartFromJSON>(Request.Cookies[cookieCartName].Value);
            
            return RedirectToAction("PersonalInfoChoice", "Clients");
        }
        
        public PartialViewResult _CheckoutCartList()
        {
            return PartialView("_CheckoutCartList", GetCartItemsFromCookie());
        }

        private List<CartOverviewItem> GetCartItemsFromCookie()
        {
            // TODO Error

            CartFromJSON cart = Newtonsoft.Json.JsonConvert.DeserializeObject<CartFromJSON>(Request.Cookies[cookieCartName].Value);

            List<int> IDs = new List<int>();
            foreach (var prod in cart.ProductList)
            {
                for (int i = 0; i < prod.Quantity; i++)
                {
                    IDs.Add(prod.ID);
                }
            }

            return _prodRepo.GetCartOverview(GetISO(), IDs).OrderBy(x => x.ProductName).ToList();
        }
    }
}