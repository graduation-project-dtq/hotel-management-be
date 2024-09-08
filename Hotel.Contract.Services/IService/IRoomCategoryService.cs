using Hotel.Contract.Repositories.Entity;
using Task = System.Threading.Tasks.Task;

namespace Hotel.Contract.Services.IService
{
    public interface IRoomCategoryService
    {
        Task<IList<RoomCategory>> GetAll();
        Task<RoomCategory?> GetById(object id);
        Task Add(RoomCategory roomCategory);
        Task Update(RoomCategory roomCategory);
        Task Delete(object id);
    }
}
