﻿using RudycommerceData;
using RudycommerceData.Models.ASPModels;
using RudycommerceData.Repositories.IRepo;
using RudycommerceData.Repositories.Repo;
using RudycommerceLib.CustomAttributes;
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
        public ActionResult Index(string search)
        {
            return RedirectToAction("Search", "Products", new { searchQuery = search });
        }

        [HttpGet]
        public ActionResult Search(string searchQuery)
        {
            List<ProductListItem> products = _prodRepo.GetProductListItems(GetISO(), "search", searchQuery);

            ViewBag.SearchQuery = searchQuery;
            ViewBag.Title = "\"" + searchQuery + "\"";
            ViewBag.Products = products.OrderBy(x => x.Name).ToList();

            return View("SearchResultPage");
        }
        
        [HttpGet]
        public ActionResult CategoryPage(int id)
        {
            List<ProductListItem> products = _prodRepo.GetFilteredCategoryItems(GetISO(), null, id);

            ViewBag.Products = products.OrderBy(x => x.Name).ToList();
            ViewBag.Title = _catRepo.GetLocalizedCatListItems(GetISO()).First(x => x.CategoryID == id).LocalizedPluralName;
            ViewBag.CategoryID = id;

            var filters = _prodRepo.GetFilters(GetISO(), id);

            ViewBag.CategoryID = filters.CategoryID;
            filters.FilterOptions = filters.FilterOptions.OrderBy(x => x.DisplayOrder).ToList();

            foreach (var item in filters.FilterOptions)
            {
                item.FilterValues = item.FilterValues.OrderBy(x => x.Value).ThenByDescending(x => x.BoolValue).ToList();
            }

            return View("CategoryPage", filters);
        }

        [HttpPost]
        public ActionResult CategoryPage(Filters filters)
        {
            List<ProductListItem> products = _prodRepo.GetFilteredCategoryItems(GetISO(), filters, filters.CategoryID);

            if (filters != null)
            {
                switch (filters.Sort.ToLower())
                {
                    case "name-asc":
                        ViewBag.Products = products.OrderBy(p => p.Name).ToList();
                        break;

                    case "name-desc":
                        ViewBag.Products = products.OrderByDescending(p => p.Name).ToList();
                        break;

                    case "price-asc":
                        ViewBag.Products = products.OrderBy(p => p.UnitPrice).ToList();
                        break;

                    case "price-desc":
                        ViewBag.Products = products.OrderByDescending(p => p.UnitPrice).ToList();
                        break;

                    default:
                        ViewBag.Products = products.OrderBy(p => p.Name).ToList();
                        break;
                }
            }
            else
            {
                ViewBag.Products = products.OrderBy(p => p.Name).ToList();
            }

            ViewBag.Title = _catRepo.GetLocalizedCatListItems(GetISO()).First(x => x.CategoryID == filters.CategoryID).LocalizedPluralName;
            ViewBag.CategoryID = filters.CategoryID;

            return View(filters);
        }

        //[HttpGet]
        //public ActionResult _FilterOptions(int id)
        //{
        //    var filters = _prodRepo.GetFilters(GetISO(), id);

        //    ViewBag.CategoryID = filters.CategoryID;
        //    filters.FilterOptions = filters.FilterOptions.OrderBy(x => x.DisplayOrder).ToList();

        //    return View(filters);
        //}

        //[HttpPost]
        //public ActionResult _FilterOptions(Filters filters)
        //{
        //    return RedirectToAction("CategoryPage", "Products", new { id = filters.CategoryID, filter = filters });
        //}

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
        [DontSavePageInCache]
        [CheckoutActionFilter]
        //[IsCartFilledActionFilter]
        public ActionResult CartOverview()
        {
            try
            {
                if (Request.Cookies[ConstVal.cookieCartName] != null)
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
            return RedirectToAction("PersonalInfoChoice", "Clients");
        }

        public PartialViewResult _CheckoutCartList()
        {
            return PartialView("_CheckoutCartList", GetCartItemsFromCookie());
        }

        private List<CartOverviewItem> GetCartItemsFromCookie()
        {
            // TODO Error

            CartFromJSON cart = Newtonsoft.Json.JsonConvert.DeserializeObject<CartFromJSON>(Request.Cookies[ConstVal.cookieCartName].Value);

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