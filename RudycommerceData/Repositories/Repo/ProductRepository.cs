using RudycommerceData.Entities.Products.Products;
using RudycommerceData.Models;
using RudycommerceData.Repositories.BaseRepo;
using RudycommerceData.Repositories.IRepo;
using System;
using System.Collections.Generic;
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
    }
}
