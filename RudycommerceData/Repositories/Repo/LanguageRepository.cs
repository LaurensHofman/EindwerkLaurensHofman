using RudycommerceData.Entities;
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
    public class LanguageRepository : BaseRepository<Language>, ILanguageRepository
    {
        public override void Delete(int entityID)
        {
            Delete(Get(entityID));
        }

        public override void Delete(Language entity)
        {
            entity.DeletedAt = DateTime.Now;

            Update(entity);
        }

        public override List<Language> GetAll()
        {
            return base.GetAll().Where(x => x.DeletedAt == null).ToList();
        }

        public async override Task<List<Language>> GetAllAsync()
        {
            return (await base.GetAllAsync()).Where(x => x.DeletedAt == null).ToList();
        }

        public override IQueryable<Language> GetAllQueryable()
        {
            return base.GetAllQueryable().Where(x => x.DeletedAt == null).AsQueryable();
        }

        public async Task<Language> MakeNewDefaultLanguage(Language newDefault)
        {
            try
            {
                Language oldDefault = GetAllQueryable().SingleOrDefault(x => x.IsDefault);

                oldDefault.IsDefault = false;
                await UpdateAsync(oldDefault);

                return Add(newDefault);
            }
            catch (Exception)
            {

                throw;
            }            
        }

        public async Task<Language> SwapDefaultLanguages(Language newDefault)
        {
            try
            {
                Language oldDefault = GetAllQueryable().SingleOrDefault(x => x.IsDefault);

                oldDefault.IsDefault = false;
                await UpdateAsync(oldDefault);

                newDefault.IsDefault = true;
                return await UpdateAsync(newDefault);

            }
            catch (Exception)
            {

                throw;
            }            
        }

        public int GetLanguageIDByISO(string iso)
        {
            SqlParameter langISO = new SqlParameter("@langISO", iso);

            // First or default has to be used, because C# doesn't know yet that it will not receive a table.
            return _context.Database.SqlQuery<int>("exec dbo.sprocGetLangIDByISO @langISO", langISO).FirstOrDefault();
        }    
    }
}
