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
                    _context.Products.Add(entity);

                    IImageRepository _imgRepo = new ImageRepository();

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

        public Filters GetFilters(string languageISO, int categoryID)
        {
            SqlParameter langISO = new SqlParameter("@langISO", languageISO);
            SqlParameter catID = new SqlParameter("@catID", categoryID);

            List<FilterOptionSQL> filterOptionsSQL = _context.Database.SqlQuery<FilterOptionSQL>
                ("exec dbo.sprocGetFilterOptions @langISO, @catID", langISO, catID).ToList();

            Filters filters = new Filters() { CategoryID = categoryID };

            foreach (var item in filterOptionsSQL)
            {
                var spec = filters.FilterOptions.FirstOrDefault(x => x.SpecID == item.SpecID);

                if (spec == null)
                {
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

            foreach (var item in filters.FilterOptions)
            {
                item.FilterValues = item.FilterValues.Distinct(new FilterValueComparer()).ToList();
            }

            filters.FilterOptions.RemoveAll(x => x.FilterValues.Count <= 1);

            return filters;
        }

        public List<ProductListItem> GetFilteredCategoryItems(string languageISO, Filters filt, int catID)
        {
            SqlParameter langISO = new SqlParameter("@langISO", languageISO);
            SqlParameter categoryID = new SqlParameter("@categoryID", catID);

            string xmlFilters = "";

            foreach (var filterOption in filt.FilterOptions)
            {
                if (!filterOption.FilterValues.All(x => x.IsSelected == false))
                {
                    foreach (var filterValue in filterOption.FilterValues.Where(x => x.IsSelected == false))
                    {
                        if (filterOption.IsBool == true)
                        {
                            string filtbool = (bool)filterValue.BoolValue ? "1" : "0";
                            xmlFilters += $"<filter id = \"{filterOption.SpecID}\"><val>{filtbool}</val></filter>";
                        }
                        else
                        {
                            xmlFilters += $"<filter id = \"{filterOption.SpecID}\"><val>{filterValue.Value}</val></filter>";
                        }                        
                    }
                }                            
            }

            SqlParameter filters = new SqlParameter("@filters", xmlFilters);

            return _context.Database.SqlQuery<ProductListItem>("exec dbo.sprocGetFilteredCategoryItems @langISO, @categoryID, @filters", langISO, categoryID, filters).ToList();
        }

        public List<CartOverviewItem> GetCartOverview(string languageISO, List<int> IDs)
        {
            string IDString = string.Join(",", IDs);

            return GetCartOverview(languageISO, IDString);
        }

        public List<CartOverviewItem> GetCartOverview(string languageISO, string IDString)
        {
            SqlParameter langISO = new SqlParameter("@langISO", languageISO);
            SqlParameter intList = new SqlParameter("@intList", IDString);

            return _context.Database.SqlQuery<CartOverviewItem>("exec dbo.sprocGetCartOverview @intList, @langISO", intList, langISO).ToList();
        }

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

        public ProductDetailsPageItem GetProductDetails(string ISO, int ID)
        {
            ProductDetailsPageItem details = new ProductDetailsPageItem
            {
                ProductInfo = GetProductInfo(ISO, ID),
                Images = GetProductImages(ID).OrderBy(x => x.DisplayOrder).ToList(),
                SpecificationInfoAndValues = GetSpecificationInfo(ISO, ID).OrderBy(x => x.DisplayOrder).ToList()
            };

            return details;
        }

        public Decimal GetTotalPrice(List<int> IDs)
        {
            string IDString = string.Join(",", IDs);

            return GetTotalPrice(IDString);
        }

        public Decimal GetTotalPrice(string IDs)
        {
            SqlParameter idstring = new SqlParameter("@idstring", IDs);

            return _context.Database.SqlQuery<Decimal>("exec dbo.sprocGetTotalPrice @idstring", idstring).First();
        }

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

            return _context.Database.SqlQuery<ProdDetProductInfo>("exec dbo.sprocProdDetProductInfo @langISO, @productID", langISO, productID).First();
        }
    }
}
