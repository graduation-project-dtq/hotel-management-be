
namespace Hotel.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Entities { get; }
        Task<IList<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task InsertAsync(T entity);
        Task DeleteAsync(int id);
        Task UpdateAsync(T entity);

    }
}
