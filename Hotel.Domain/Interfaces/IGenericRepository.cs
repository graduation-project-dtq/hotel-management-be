using Hotel.Infrastructure.Base;
using System.Linq.Expressions;


namespace Hotel.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> Entities { get; }

        //void
        T? GetById(object id);
        void Insert(T obj);
        void InsertRange(List<T> obj);
        Task InsertRangeAsync(List<T> obj);

        void Update(T obj);
        void Delete(object id);
        void Save();

        //Task
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task InsertAsync(T obj);
        Task UpdateAsync(T obj);
        Task DeleteAsync(object id);
        Task SaveAsync();

        //another
        T? Find(Expression<Func<T, bool>> predicate);
        Task<PaginatedList<T>> GetPagging(IQueryable<T> query, int index, int pageSize);


        //new
        Task<T?> FindByConditionAsync(Expression<Func<T, bool>> expression);
        Task<T?> FindByConditionWithIncludesAsync(
            Expression<Func<T, bool>> expression,
            params Expression<Func<T, object>>[] includes);
        public IQueryable<T> GetEntitiesWithCondition(
            Expression<Func<T, bool>> expression,
            params Expression<Func<T, object>>[] includes);
        Task<TResult?> FindByConditionWithIncludesAndSelectAsync<TResult>(
            Expression<Func<T, bool>> expression,
            Expression<Func<T, TResult>> selector,
            params Expression<Func<T, object>>[] includes);
        public IQueryable<TResult> GetEntitiesWithConditionAndSelect<TResult>(
            Expression<Func<T, bool>> expression,
            Expression<Func<T, TResult>> selector,
            params Expression<Func<T, object>>[] includes);
    }
}
