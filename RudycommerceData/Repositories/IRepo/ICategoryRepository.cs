using RudycommerceData.Entities.Products.Categories;
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
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        List<CategoryOverviewItem> GetCategoryOverview(int languageID);

        List<LocalizedCatListItem> GetLocalizedCatListItems(string iso);
    }
}
