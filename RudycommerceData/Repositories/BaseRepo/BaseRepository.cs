﻿using RudycommerceData.Data;
using RudycommerceData.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Repositories.BaseRepo
{
    public class BaseRepository<T> : IBaseRepository<T> where T: BaseEntity<int>
    {
        protected readonly RudyDbContext _context;

        public BaseRepository()
        {
            _context = new RudyDbContext();
        }

        public IQueryable<T> GetAllQueryable()
        {
            return _context.Set<T>().AsQueryable();
        }

        public T Add(T entity)
        {
            _context.Set<T>().Add(entity);

            return entity;
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void Delete(int entityID)
        {
            Delete(Get(entityID));
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetAsync(int ID)
        {
            return await _context.Set<T>().FindAsync(ID);
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            entity.ModifiedAt = DateTime.Now;

            var original = await GetAsync(entity.ID);

            if (original != null)
            {
                _context.Entry(original).CurrentValues.SetValues(entity);

                return entity;
            }

            return null;
        }

        public virtual T Update(T entity)
        {
            entity.ModifiedAt = DateTime.Now;

            var original = Get(entity.ID);

            if (original != null)
            {
                _context.Entry(original).CurrentValues.SetValues(entity);

                return entity;
            }

            return null;
        }

        public async Task<bool> AnyAsync()
        {
            return await _context.Set<T>().AnyAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public T Get(int ID)
        {
            return _context.Set<T>().Find(ID);
        }
    }
}
