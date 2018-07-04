using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Repositories.BaseRepo
{
    public interface IBaseRepository<T>
    {
        IQueryable<T> GetAllQueryable();

        Task<T> GetAsync(int ID);

        Task<List<T>> GetAllAsync();

        T AddAsync(T entity);

        Task<T> UpdateAsync(T entity);

        void DeleteAsync(T entity);

        Task<bool> AnyAsync();

        Task<int> SaveChangesAsync();
    }
}
