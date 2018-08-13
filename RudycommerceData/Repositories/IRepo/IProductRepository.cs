using RudycommerceData.Entities.Products.Products;
using RudycommerceData.Models;
using RudycommerceData.Models.ASPModels;
using RudycommerceData.Repositories.BaseRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Repositories.IRepo
{
    public interface IProductRepository: IBaseRepository<Product>
    {
        Task<Product> AddWithImagesAsync(Product entity);

        Task<Product> UpdateWithImagesAsync(Product entity);

        List<ProductOverviewItem> GetProductOverview(int languageID);

        void ToggleProductActive(int ProductID);

        List<ProductListItem> GetProductListItems(string langISO, string choice, string queryString);

        List<ProductListItem> GetProductListItems(string languageISO, string choiceOption);

        ProductDetailsPageItem GetProductDetails(string ISO, int ID);

        List<CartOverviewItem> GetCartOverview(string languageISO, List<int> IDs);

        List<CartOverviewItem> GetCartOverview(string languageISO, string IDString);

        Decimal GetTotalPrice(List<int> IDs);
    }
}
