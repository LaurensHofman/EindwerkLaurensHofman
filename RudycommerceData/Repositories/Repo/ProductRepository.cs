using RudycommerceData.Entities.Products.Products;
using RudycommerceData.EqualityComparer;
using RudycommerceData.Models;
using RudycommerceData.Models.ASPModels;
using RudycommerceData.Models.ASPModels.ProductDetailsPageSubItems;
using RudycommerceData.Repositories.BaseRepo;
using RudycommerceData.Repositories.IRepo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Repositories.Repo
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public async Task<Product> AddWithImagesAsync(Product entity)
        {
            using (var ctxTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Adds the product and saves, so the product has an ID now, which is required to save the images in appropriate folders.
                    _context.Products.Add(entity);
                    await _context.SaveChangesAsync();

                    IImageRepository _imgRepo = new ImageRepository();

                    // Uploads all the images and gets the image URL in return
                    foreach (var img in entity.Images)
                    {
                        img.ImageURL = await _imgRepo.SaveImage(img, entity.ID);
                    }

                    await _context.SaveChangesAsync();

                    ctxTransaction.Commit();

                    return entity;
                }
                catch (Exception)
                {
                    ctxTransaction.Rollback();
                    // TODO
                    throw;
                }

            }
        }

        public async Task<Product> UpdateWithImagesAsync(Product entity)
        {
            using (var ctxTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    IImageRepository _imgRepo = new ImageRepository();

                    foreach (var img in entity.Images)
                    {
                        img.ImageURL = await _imgRepo.SaveImage(img, entity.ID);
                        img.ProductID = entity.ID;
                    }

                    await UpdateAsync(entity);

                    await _context.SaveChangesAsync();

                    ctxTransaction.Commit();

                    return entity;
                }
                catch (Exception)
                {
                    ctxTransaction.Rollback();
                    // TODO
                    throw;
                }

            }
        }

        public List<ProductOverviewItem> GetProductOverview(int languageID)
        {
            SqlParameter langID = new SqlParameter("@langID", languageID.ToString());

            return _context.Database.SqlQuery<ProductOverviewItem>("exec dbo.sprocGetProductOverview @langID", langID).ToList();
        }

        public void ToggleProductActive(int ProductID)
        {
            Product prod = Get(ProductID);

            prod.IsActive = !prod.IsActive;

            Update(prod);
        }

        /// <summary>
        /// Gets the filter options, belonging to a category
        /// </summary>
        /// <param name="languageISO">The language the page is currently in</param>
        /// <param name="categoryID">The Category ID</param>
        /// <returns></returns>
        public Filters GetFilters(string languageISO, int categoryID)
        {
            SqlParameter langISO = new SqlParameter("@langISO", languageISO);
            SqlParameter catID = new SqlParameter("@catID", categoryID);

            // Gets all the filter options
            List<FilterOptionSQL> filterOptionsSQL = _context.Database.SqlQuery<FilterOptionSQL>
                ("exec dbo.sprocGetFilterOptions @langISO, @catID", langISO, catID).ToList();

            // Creates a new Filters object
            Filters filters = new Filters() { CategoryID = categoryID };

            // Transform the FilterOptions from SQL to the Filters object that is going to be used in ASP
            foreach (var item in filterOptionsSQL)
            {
                // Checks if the Filters object already has a specification matching the ID
                var spec = filters.FilterOptions.FirstOrDefault(x => x.SpecID == item.SpecID);

                // If it doesn't have the specification yet, make a new one.
                if (spec == null)
                {
                    // Adds a new filterOption, and make a subCollection of FilterValues and adds a first value.
                    filters.FilterOptions.Add(new FilterOption()
                    {
                        DisplayOrder = item.DisplayOrder,
                        SpecID = item.SpecID,
                        IsBool = item.IsBool,
                        IsEnum = item.IsEnum,
                        SpecName = item.SpecName,
                        FilterValues = new List<FilterValue>()
                        {
                            new FilterValue
                            {
                                BoolValue = item.BoolValue,
                                EnumID = item.EnumID,
                                Value = item.Value
                            }
                        }
                    });
                }
                // If the specification exists already, just add one value to the existing FilterValues for that specification
                else
                {
                    spec.FilterValues.Add(
                        new FilterValue
                        {
                            BoolValue = item.BoolValue,
                            EnumID = item.EnumID,
                            Value = item.Value
                        }
                    );
                }
            }

            // As a starter, I'm not gonna work with numerical values
            foreach (var item in filters.FilterOptions)
            {
                item.FilterValues.RemoveAll(x => x.StringValueIsNumber);
            }

            // Removes all distinct values, so the user doesn't have to see 15 checkboxes for the colour Black
            foreach (var item in filters.FilterOptions)
            {
                item.FilterValues = item.FilterValues.Distinct(new FilterValueComparer()).ToList();
            }

            // Removes all FilterOptions where there was only 1 filter value anyways
            filters.FilterOptions.RemoveAll(x => x.FilterValues.Count <= 1);

            return filters;
        }

        /// <summary>
        /// Gets the appropriate products, according to the given filters
        /// </summary>
        /// <param name="languageISO"></param>
        /// <param name="filt"></param>
        /// <param name="catID"></param>
        /// <returns></returns>
        public List<ProductListItem> GetFilteredCategoryItems(string languageISO, Filters filt, int catID)
        {
            if (filt == null)
            {
                filt = new Filters();
            }

            SqlParameter langISO = new SqlParameter("@langISO", languageISO);
            SqlParameter categoryID = new SqlParameter("@categoryID", catID);

            // Because SQL doesn't accept a list of objects, but does accept XML, I transformed my filterOptions to XML
            string xmlFilters = "";

            // I will only send the unwanted values to SQL, 
            // and SQL will filter the products matching those values out of the total list of products for the selected category

            // Looks through each filterOption
            foreach (var filterOption in filt.FilterOptions)
            {
                // If all the filterValues within one filterOption are not selected, I assume the user has no opinion about that filterOption, 
                // Meaning I will accept all those values as Wanted, even though they are not selected.
                if (!filterOption.FilterValues.All(x => x.IsSelected == false))
                {
                    // If at least one value is selected, add all the not selected (unwanted) values to the XML
                    foreach (var filterValue in filterOption.FilterValues.Where(x => x.IsSelected == false))
                    {
                        // If the filterOption is a bool, give 1 or 0 as value (= true or false in SQL)
                        if (filterOption.IsBool == true)
                        {
                            string filtbool = (bool)filterValue.BoolValue ? "1" : "0";
                            xmlFilters += $"<filter id = \"{filterOption.SpecID}\"><val>{filtbool}</val></filter>";
                        }
                        // Else, just put the value
                        else
                        {
                            xmlFilters += $"<filter id = \"{filterOption.SpecID}\"><val>{filterValue.Value}</val></filter>";
                        }                        
                    }
                }                            
            }

            SqlParameter filters = new SqlParameter("@filters", xmlFilters);

            // Sends the filters, language and CategoryID to SQL and it will return a list of products
            return _context.Database.SqlQuery<ProductListItem>("exec dbo.sprocGetFilteredCategoryItems @langISO, @categoryID, @filters", langISO, categoryID, filters).ToList();
        }

        /// <summary>
        /// Gets the cart overview based on the ProductIDs (IDs can appear more than once, will result in a quantity > 1)
        /// </summary>
        /// <param name="languageISO"></param>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public List<CartOverviewItem> GetCartOverview(string languageISO, List<int> IDs)
        {
            // Puts the IDs in a string so it can be sent to and interpretted by SQL
            string IDString = string.Join(",", IDs);

            return GetCartOverview(languageISO, IDString);
        }

        /// <summary>
        /// Gets the cart overview based on the ProductIDs (IDs can appear more than once, will result in a quantity > 1)
        /// </summary>
        /// <param name="languageISO"></param>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public List<CartOverviewItem> GetCartOverview(string languageISO, string IDString)
        {
            // The IDs are put in a string so they can be sent to and interpretted by SQL

            SqlParameter langISO = new SqlParameter("@langISO", languageISO);
            SqlParameter intList = new SqlParameter("@intList", IDString);

            return _context.Database.SqlQuery<CartOverviewItem>("exec dbo.sprocGetCartOverview @intList, @langISO", intList, langISO).ToList();
        }

        /// <summary>
        /// Gets the Products that will be shown in a Product List
        /// </summary>
        /// <param name="languageISO">The ISO of the language it has to be shown in</param>
        /// <param name="choiceOption">Choices are: "new", "bestsell", "search", "category" or none</param>
        /// <param name="queryString">Query string for Search Query, or the Category ID for the category choice</param>
        /// <returns></returns>
        public List<ProductListItem> GetProductListItems(string languageISO, string choiceOption, string queryString)
        {
            SqlParameter langISO = new SqlParameter("@langISO", languageISO);
            SqlParameter choice = new SqlParameter("@choice", choiceOption);
            SqlParameter query = new SqlParameter("@query", queryString);

            return _context.Database.SqlQuery<ProductListItem>("exec dbo.sprocProductListItems @langISO, @choice, @query", langISO, choice, query).ToList();
        }

        public List<ProductListItem> GetProductListItems(string languageISO, string choiceOption)
        {
            return GetProductListItems(languageISO, choiceOption, "");
        }

        /// <summary>
        /// Gets the Details of a product for a Details page
        /// </summary>
        /// <param name="ISO">The ISO of the language</param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ProductDetailsPageItem GetProductDetails(string ISO, int ID)
        {
            ProductDetailsPageItem details = new ProductDetailsPageItem
            {
                // Gets all the generic product info (description, name, price, ...)
                ProductInfo = GetProductInfo(ISO, ID),
                // Gets the images
                Images = GetProductImages(ID).OrderBy(x => x.DisplayOrder).ToList(),
                // Gets the specifications and the product's values for those specifications
                SpecificationInfoAndValues = GetSpecificationInfo(ISO, ID).OrderBy(x => x.DisplayOrder).ToList()
            };

            return details;
        }

        /// <summary>
        /// Gets the totalPrice based on the product IDs
        /// </summary>
        /// <param name="IDs">List of product IDs (can have duplicates for a quantity > 1)</param>
        /// <returns></returns>
        public Decimal GetTotalPrice(List<int> IDs)
        {
            string IDString = string.Join(",", IDs);

            return GetTotalPrice(IDString);
        }

        public Decimal GetTotalPrice(string IDs)
        {
            SqlParameter idstring = new SqlParameter("@idstring", IDs);

            return _context.Database.SqlQuery<Decimal>("exec dbo.sprocGetTotalPrice @idstring", idstring).FirstOrDefault();
        }

        #region Private methods for GetProductDetails(...)
        private List<ProdDetSpecInfoAndValue> GetSpecificationInfo(string ISO, int ID)
        {
            SqlParameter langISO = new SqlParameter("@langISO", ISO);
            SqlParameter productID = new SqlParameter("@productID", ID);

            return _context.Database.SqlQuery<ProdDetSpecInfoAndValue>("exec dbo.sprocProdDetSpecs @langISO, @productID", langISO, productID).ToList();
        }

        private List<ProdDetImage> GetProductImages(int ID)
        {
            SqlParameter productID = new SqlParameter("@productID", ID);

            return _context.Database.SqlQuery<ProdDetImage>("exec dbo.sprocProdDetImages @productID", productID).ToList();
        }

        private ProdDetProductInfo GetProductInfo(string ISO, int ID)
        {
            SqlParameter langISO = new SqlParameter("@langISO", ISO);
            SqlParameter productID = new SqlParameter("@productID", ID);

            return _context.Database.SqlQuery<ProdDetProductInfo>("exec dbo.sprocProdDetProductInfo @langISO, @productID", langISO, productID).FirstOrDefault();
        }
        #endregion

    }
}
