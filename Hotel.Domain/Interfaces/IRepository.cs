
namespace Hotel.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Entities { get; }
        Task<IList<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task InsertAsync(T entity);
        Task DeleteAsync(string id);
        Task UpdateAsync(T entity);

    }
}
