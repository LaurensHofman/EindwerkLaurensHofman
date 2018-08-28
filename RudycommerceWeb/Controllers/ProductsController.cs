using RudycommerceData;
using RudycommerceData.Models.ASPModels;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceLib.CustomAttributes;
using RudycommerceLib.Utilities;
using RudycommerceWeb.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace RudycommerceWeb.Controllers
{
    public class ProductsController : Base.MultilingualBaseController
    {
        private IProductRepository _prodRepo;
        private ICategoryRepository _catRepo;

        public ProductsController()
        {
            _prodRepo = new ProductRepository();
            _catRepo = new CategoryRepository();
        }

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

        /// <summary>
        /// Post with the search box
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(string search)
        {
            // Sends the search query to the search page
            return RedirectToAction("Search", "Products", new { searchQuery = search });
        }

        [HttpGet]
        public ActionResult Search(string searchQuery)
        {
            // If somehow an empty search query was received, at least show a white space so it can be shown to the user.
            if (searchQuery == null)
            {
                searchQuery = " ";
            }

            // Gets the products matching the search query
            List<ProductListItem> products = _prodRepo.GetProductListItems(GetISO(), "search", searchQuery);

            // Saves the search query for later use
            ViewBag.SearchQuery = searchQuery;
            // As title use the search query surrounded by quotes
            ViewBag.Title = "\"" + searchQuery + "\"";

            // If products is not null, order them by name (default)
            if (products != null)
            {
                ViewBag.Products = products.OrderBy(x => x.Name).ToList();
            }
            else
            {
                ViewBag.Products = null;
            }

            return View("SearchResultPage");
        }

        [HttpGet]
        public ActionResult CategoryPage(int id)
        {
            // Gets the filtered category items
            List<ProductListItem> products = _prodRepo.GetFilteredCategoryItems(GetISO(), null, id);

            try
            {
                // Orders the products by name (default)
                ViewBag.Products = products.OrderBy(x => x.Name).ToList();
                // Gets the name localized plural name of the category, to be shown as title
                ViewBag.Title = _catRepo.GetLocalizedCatListItems(GetISO()).First(x => x.CategoryID == id).LocalizedPluralName;

                // Gets the fitting filters for a category
                var filters = _prodRepo.GetFilters(GetISO(), id);

                // Saves the category ID for later use
                ViewBag.CategoryID = filters.CategoryID;

                // Order the filter options by display priority
                filters.FilterOptions = filters.FilterOptions.OrderBy(x => x.DisplayOrder).ToList();

                // For boolean results, show the True before the False
                foreach (var item in filters.FilterOptions)
                {
                    item.FilterValues = item.FilterValues.OrderBy(x => x.Value).ThenByDescending(x => x.BoolValue).ToList();
                }

                return View("CategoryPage", filters);
            }
            catch (Exception)
            {
                // If something went wrong, send an empty list of products to the view. The view will know what to show if the ProductsList == null
                ViewBag.Products = null;

                return View("CategoryPage");
            }
        }

        [HttpPost]
        public ActionResult CategoryPage(Filters filters)
        {
            // Gets the products matching the chosen filters
            List<ProductListItem> products = _prodRepo.GetFilteredCategoryItems(GetISO(), filters, filters.CategoryID);

            // Filters currently also contains the sorting option
            if (filters != null)
            {
                // Checks which sorting option was selected
                switch (filters.Sort.ToLower())
                {
                    // Orders by name ascending
                    case "name-asc":
                        ViewBag.Products = products.OrderBy(p => p.Name).ToList();
                        break;

                    // Orders by name descending
                    case "name-desc":
                        ViewBag.Products = products.OrderByDescending(p => p.Name).ToList();
                        break;

                    // Orders by price ascending
                    case "price-asc":
                        ViewBag.Products = products.OrderBy(p => p.UnitPrice).ToList();
                        break;

                    // Orders by price descending
                    case "price-desc":
                        ViewBag.Products = products.OrderByDescending(p => p.UnitPrice).ToList();
                        break;

                    // Default, order by name ascending
                    default:
                        ViewBag.Products = products.OrderBy(p => p.Name).ToList();
                        break;
                }
            }
            else
            {
                // Default, order by name ascending
                ViewBag.Products = products.OrderBy(p => p.Name).ToList();
            }

            // Get the category plural name for the title
            ViewBag.Title = _catRepo.GetLocalizedCatListItems(GetISO()).First(x => x.CategoryID == filters.CategoryID).LocalizedPluralName;
            // Saves the category ID for later use
            ViewBag.CategoryID = filters.CategoryID;

            // Returns the unmodified filters, so the user can still see what filters they had chosen
            return View(filters);
        }

        public ActionResult Details(int ID)
        {
            // Gets the details of a product
            RudycommerceData.Models.ASPModels.ProductDetailsPageItem details = _prodRepo.GetProductDetails(GetISO(), ID);

            // If product info was succesfully found in the database, make the title the product's name
            if (details.ProductInfo != null)
            {
                ViewBag.Title = details.ProductInfo.Name;
            }

            return View("Details", details);
        }

        [HttpPost]
        public ActionResult Cart(List<CartItem> cartItems)
        {
            // Submitting the cart sends the user to the cart overview
            return RedirectToAction("CartOverview", "Products");
        }

        [HttpGet]
        [DontSavePageInCache]
        [CheckoutActionFilter]
        public ActionResult CartOverview()
        {
            if (Request.Cookies[ConstVal.cookieCartName] != null)
            {
                return View("CartOverview", GetCartItemsFromCookie());
            }
            else
            {
                return View("CartOverview", null);
            }
        }

        [HttpPost]
        public ActionResult CartOverview(CartOverviewItem cartOverviewItems)
        {
            return RedirectToAction("PersonalInfoChoice", "Clients");
        }

        public PartialViewResult _CheckoutCartList()
        {
            return PartialView("_CheckoutCartList", GetCartItemsFromCookie());
        }

        private List<CartOverviewItem> GetCartItemsFromCookie()
        {
            try
            {
                // Tries to parse the cart from the cookie to a C# object, if it fails, the cookie will be removed and a 404 will be thrown.

                CartFromJSON cart = Newtonsoft.Json.JsonConvert.DeserializeObject<CartFromJSON>(Request.Cookies[ConstVal.cookieCartName].Value);

                // Makes a list of the product IDs (duplicate mean quantity > 1)
                List<int> IDs = new List<int>();
                foreach (var prod in cart.ProductList)
                {
                    for (int i = 0; i < prod.Quantity; i++)
                    {
                        IDs.Add(prod.ID);
                    }
                }

                var xy = _prodRepo.GetCartOverview(GetISO(), IDs).OrderBy(x => x.ProductName).ToList();
                return xy;
            }
            catch (Exception)
            {
                HttpContext.Response.Cookies[ConstVal.cookieCartName].Expires = DateTime.Now.AddDays(-1);
                return null;
            }
        }
    }
}