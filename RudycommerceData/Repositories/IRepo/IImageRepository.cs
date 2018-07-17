using RudycommerceData.Entities.Products.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Repositories.IRepo
{
    public interface IImageRepository
    {
        Task<string> SaveImage(Brand brand);

        Task<string> SaveImage(Product product);
    }
}
