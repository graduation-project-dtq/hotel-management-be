

namespace Hotel.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class;
        Task SaveChangesAsync();
    }
}
