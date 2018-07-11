using RudycommerceData.Entities.Products.Products;
using RudycommerceData.Repositories.BaseRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Repositories.IRepo
{
    public interface IBrandRepository : IBaseRepository<Brand>
    {
        Task<Brand> AddAsyncWithImage(Brand entity);

        Task<Brand> UpdateAsyncWithImage(Brand entity);
    }
}
