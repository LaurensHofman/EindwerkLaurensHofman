﻿using RudycommerceData.Entities.Products.Products;
using RudycommerceData.Models;
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

        List<ProductOverviewItem> GetProductOverview(int languageID);

        void ToggleProductActive(int ProductID);
    }
}
