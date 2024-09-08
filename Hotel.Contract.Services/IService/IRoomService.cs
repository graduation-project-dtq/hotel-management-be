using Hotel.Contract.Repositories.Entity;
using Task = System.Threading.Tasks.Task;

namespace Hotel.Contract.Services.IService
{
    public interface IRoomService
    {
        Task<IList<Room>> GetAll();
        Task<Room?> GetById(object id);
        Task Add(Room category);
        Task Update(Room room);
        Task Delete(object id);
    }
}
