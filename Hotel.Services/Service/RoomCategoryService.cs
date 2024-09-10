using Hotel.Contract.Repositories.Entity;
using Hotel.Contract.Repositories.IUOW;
using Hotel.Contract.Services.IService;
using Hotel.Core.Base;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Hotel.Core.Base.BaseException;

namespace Hotel.Services.Service
{
    public class RoomCategoryService : IRoomCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RoomCategoryService> _logger;
        public RoomCategoryService(IUnitOfWork unitOfWork, ILogger<RoomCategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task Add(RoomCategory roomCategory)
        {
            roomCategory.Id = Guid.NewGuid().ToString("N");

            // Kiểm tra xem InternalCode đã tồn tại chưa
            IGenericRepository<RoomCategory> genericRepository = _unitOfWork.GetGenericRepository<RoomCategory>();
            bool exists = await genericRepository.ExistsAsync(r => r.InternalCode == roomCategory.InternalCode);

            if (exists)
            {
                _logger.LogWarning("Room category with the same InternalCode already exists: {InternalCode}", roomCategory.InternalCode);
                throw new DuplicateInternalCodeException("Room category with the same InternalCode already exists");
            }

            try
            {
                await genericRepository.InsertAsync(roomCategory);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,"Failed to insert room category");
                throw new InvalidOperationException("Failed to insert room category");
            }
        }

        public async Task Delete(object id)
        {
            try
            {
                IGenericRepository<RoomCategory> genericRepository = _unitOfWork.GetGenericRepository<RoomCategory>();
                await genericRepository.DeleteAsync(id);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete room category with id: {Id}", id);
                throw;
            }
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
