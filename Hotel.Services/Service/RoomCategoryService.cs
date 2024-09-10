using Hotel.Contract.Repositories.Entity;
using Hotel.Contract.Repositories.IUOW;
using Hotel.Contract.Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Services.Service
{
    public class RoomCategoryService : IRoomCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RoomCategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task Add(RoomCategory roomCategory)
        {
            roomCategory.Id = Guid.NewGuid().ToString("N");
            IGenericRepository<RoomCategory> genericRepository = _unitOfWork.GetGenericRepository<RoomCategory>();
            await genericRepository.InsertAsync(roomCategory);
            await _unitOfWork.SaveAsync();
        }

        public async Task Delete(object id)
        {
            IGenericRepository<RoomCategory> genericRepository = _unitOfWork.GetGenericRepository<RoomCategory>();
            await genericRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
        }

        public Task<IList<RoomCategory>> GetAll()
        {
            return _unitOfWork.GetGenericRepository<RoomCategory>().GetAllAsync();
        }
        //Get all RoomTypeDetails 
        public Task<IList<RoomCategory>> GetAllActive()
        {
            return _unitOfWork.GetGenericRepository<RoomCategory>()
                              .GetWhereAsync(r => r.DeletedTime == null);
        }
        public Task<RoomCategory?> GetById(object id)
        {
            return _unitOfWork.GetGenericRepository<RoomCategory>().GetByIdAsync(id);
        }

        public async Task Update(RoomCategory roomCategory)
        {
            IGenericRepository<RoomCategory> genericRepository = _unitOfWork.GetGenericRepository<RoomCategory>();
            await genericRepository.UpdateAsync(roomCategory);
            await _unitOfWork.SaveAsync();
        }

    }
}
