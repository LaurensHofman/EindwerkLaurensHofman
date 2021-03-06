﻿using System;
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

        T Get(int ID);

        Task<List<T>> GetAllAsync();

        List<T> GetAll();

        T Add(T entity);

        Task<T> UpdateAsync(T entity);

        T Update(T entity);

        T[] Update(params T[] entities);

        void Delete(T entity);

        void Delete(int entityID);

        Task<bool> AnyAsync();

        Task<int> SaveChangesAsync();
    }
}
