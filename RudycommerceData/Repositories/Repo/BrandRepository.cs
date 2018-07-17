using RudycommerceData.Data;
using RudycommerceData.Entities.Products.Products;
using RudycommerceData.Repositories.BaseRepo;
using RudycommerceData.Repositories.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Repositories.Repo
{
    public class BrandRepository: BaseRepository<Brand>, IBrandRepository
    {
        public async Task<Brand> AddAsyncWithImage(Brand entity)
        {
            using (var ctxTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Brands.Add(entity);

                    IImageRepository _imgRepo = new ImageRepository();

                    entity.LogoURL = await _imgRepo.SaveImage(entity);

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

        public async Task<Brand> UpdateAsyncWithImage(Brand entity)
        {
            using (var ctxTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    IImageRepository _imgRepo = new ImageRepository();

                    entity.LogoURL = await _imgRepo.SaveImage(entity);

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
    }
}
