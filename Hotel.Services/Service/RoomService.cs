using Hotel.Contract.Repositories.Entity;
using Hotel.Contract.Repositories.IUOW;
using Hotel.Contract.Services;


namespace Hotel.Services.Service
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RoomService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task Add(Room room)
        {
            room.Id = Guid.NewGuid().ToString("N");
            IGenericRepository<Room> genericRepository = _unitOfWork.GetGenericRepository<Room>();
            await genericRepository.InsertAsync(room);
            await _unitOfWork.SaveAsync();
        }

        public async Task Delete(object id)
        {
            IGenericRepository<Room> genericRepository = _unitOfWork.GetGenericRepository<Room>();
            await genericRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }

        public Task<IList<Room>> GetAll()
        {
            return _unitOfWork.GetGenericRepository<Room>().GetAllAsync();
        }

        public Task<Room?> GetById(object id)
        {
            return _unitOfWork.GetGenericRepository<Room>().GetByIdAsync(id);
        }

        public async Task Update(Room room)
        {
            IGenericRepository<Room> genericRepository = _unitOfWork.GetGenericRepository<Room>();
            await genericRepository.UpdateAsync(room);
            await _unitOfWork.SaveAsync();
        }

    }
}
