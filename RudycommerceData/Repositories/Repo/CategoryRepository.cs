using RudycommerceData.Entities.Products.Categories;
using RudycommerceData.Models;
using RudycommerceData.Models.ASPModels;
using RudycommerceData.Repositories.BaseRepo;
using RudycommerceData.Repositories.IRepo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Repositories.Repo
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        /// <summary>
        /// Gets the categories with its most important information, to be shown in a category overview
        /// </summary>
        /// <param name="languageID"></param>
        /// <returns></returns>
        public List<CategoryOverviewItem> GetCategoryOverview(int languageID)
        {
            SqlParameter langID = new SqlParameter("@langID", languageID.ToString());

            return _context.Database.SqlQuery<CategoryOverviewItem>("exec dbo.sprocGetCategoryOverview @langID", langID).ToList();
        }

        /// <summary>
        /// Gets a list of all the categories, in the selected language.
        /// </summary>
        /// <param name="iso"></param>
        /// <returns></returns>
        public List<LocalizedCatListItem> GetLocalizedCatListItems(string iso)
        {
            SqlParameter langISO = new SqlParameter("langISO", iso);

            List<LocalizedCatListItem> list = _context.Database.SqlQuery<LocalizedCatListItem>("exec dbo.sprocGetLocalizedCategoryList @langISO", langISO).ToList();
            return list;
        }

        public override Category Update(Category entity)
        {
            _context.Categories.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return entity;
        }
    }
}
