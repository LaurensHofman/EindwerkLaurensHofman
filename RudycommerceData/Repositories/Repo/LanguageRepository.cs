using RudycommerceData.Entities;
using RudycommerceData.Repositories.BaseRepo;
using RudycommerceData.Repositories.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Repositories.Repo
{
    public class LanguageRepository : BaseRepository<Language>, ILanguageRepository
    {
        public async Task<Language> MakeNewDefaultLanguage(Language newDefault)
        {
            Language oldDefault = GetAllQueryable().SingleOrDefault(x => x.IsDefault);

            oldDefault.IsDefault = false;
            await UpdateAsync(oldDefault);

            return Add(newDefault);
        }

        public async Task<Language> SwapDefaultLanguages(Language newDefault)
        {
            Language oldDefault = GetAllQueryable().SingleOrDefault(x => x.IsDefault);

            oldDefault.IsDefault = false;
            await UpdateAsync(oldDefault);

            newDefault.IsDefault = true;
            return await UpdateAsync(newDefault);
        }
    }
}
