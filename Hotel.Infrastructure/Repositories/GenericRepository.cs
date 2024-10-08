﻿using Hotel.Domain.Interfaces;
using Hotel.Domain.PaggingItems;
using Hotel.Infrastructure.Base;
using Hotel.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Hotel.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly HotelDBContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(HotelDBContext dbContext)
        {
            _context = dbContext;
            _dbSet = _context.Set<T>();
        }

        public IQueryable<T> Entities => _context.Set<T>();

        public void Delete(object id)
        {
            T entity = _dbSet.Find(id) ?? throw new Exception();
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task DeleteAsync(object id)
        {
            T entity = await _dbSet.FindAsync(id) ?? throw new Exception();
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public T? Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public T? GetById(object id)
        {

            return _dbSet.Find(id);
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<PaginatedList<T>> GetPagging(IQueryable<T> query, int index, int pageSize)
        {
            return await query.GetPaginatedList(index, pageSize);
        }

        public void Insert(T obj)
        {
            _dbSet.Add(obj);
        }

        public async Task InsertAsync(T obj)
        {
            await _dbSet.AddAsync(obj);
        }
        public void InsertRange(List<T> obj)
        {
            _dbSet.AddRange(obj);
        }



        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(T obj)
        {
            _context.Entry(obj).State = EntityState.Modified;
        }

        //public async Task UpdateAsync(T obj)
        //{
        //    _dbSet.Attach(obj);
        //    _context.Entry(obj).State = EntityState.Modified;
        //}

        public Task UpdateAsync(T obj)
        {
            return Task.FromResult(_dbSet.Update(obj));
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task InsertRangeAsync(List<T> obj)
        {
            await _dbSet.AddRangeAsync(obj);
        }



        // New method: FindByConditionAsync
        public async Task<T?> FindByConditionAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.FirstOrDefaultAsync(expression);
        }

        public async Task<T?> FindByConditionWithIncludesAsync(
            Expression<Func<T, bool>> expression,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // Apply the specified condition first
            query = query.Where(expression);

            // Apply eager loading for all specified navigation properties
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Return the first matching result
            return await query.FirstOrDefaultAsync();
        }


        // New method: GetEntitiesWithCondition
        public IQueryable<T> GetEntitiesWithCondition(
            Expression<Func<T, bool>> expression,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // Apply the specified condition first
            query = query.Where(expression);

            // Apply eager loading for all specified navigation properties
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Return the query with the specified condition and eager loading
            return query;
        }


        public async Task<TResult?> FindByConditionWithIncludesAndSelectAsync<TResult>(
            Expression<Func<T, bool>> expression,
            Expression<Func<T, TResult>> selector,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // Apply the specified condition first
            query = query.Where(expression);

            // Apply eager loading for all specified navigation properties
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Project using the selector and return the first matching result
            return await query.Select(selector).FirstOrDefaultAsync();
        }


        public IQueryable<TResult> GetEntitiesWithConditionAndSelect<TResult>(
            Expression<Func<T, bool>> expression,
            Expression<Func<T, TResult>> selector,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // Apply the specified condition first
            query = query.Where(expression);

            // Apply eager loading for all specified navigation properties
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Finally, project using the selector
            return query.Select(selector);
        }
    }
}
