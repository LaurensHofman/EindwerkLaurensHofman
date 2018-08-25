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
                    // Adds the brand
                    _context.Brands.Add(entity);

                    // Saves changes, so the brand gets an ID (required to save the give the image name an appropriate name when saved externally)
                    await _context.SaveChangesAsync();

                    // Saves the image
                    IImageRepository _imgRepo = new ImageRepository();
                    entity.LogoURL = await _imgRepo.SaveImage(entity);

                    // Saves again, but now the brand has an URL for its logo
                    await _context.SaveChangesAsync();

                    // Commits the transaction
                    ctxTransaction.Commit();

                    return entity;
                }
                catch (Exception)
                {
                    ctxTransaction.Rollback();
                    //Resets the logoURL
                    entity.LogoURL = null;
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
                    // Saves the image
                    IImageRepository _imgRepo = new ImageRepository();
                    entity.LogoURL = await _imgRepo.SaveImage(entity);

                    // Updates the brand with the new URL for its logo
                    await UpdateAsync(entity);

                    await _context.SaveChangesAsync();

                    ctxTransaction.Commit();

                    return entity;
                }
                catch (Exception)
                {
                    ctxTransaction.Rollback();
                    throw;
                }
            }
        }
    }
}
