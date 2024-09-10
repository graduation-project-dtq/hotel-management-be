using Castle.Core.Logging;
using Hotel.Contract.Repositories.Entity;
using Hotel.Contract.Repositories.IUOW;
using Hotel.Contract.Services.IService;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Hotel.Core.Base.BaseException;

namespace Hotel.Services.Service
{
    public class RoomTypeDetailService : IRoomTypeDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RoomTypeDetailService> _logger;
        public RoomTypeDetailService(IUnitOfWork unitOfWork, ILogger<RoomTypeDetailService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

        }
        //Get All RoomTypeDetails deleted
        public Task<IList<RoomTypeDetail>> GetAll()
        {
            return _unitOfWork.GetGenericRepository<RoomTypeDetail>()
                              .GetAllAsync();
        }
        //Get all RoomTypeDetails 
        public Task<IList<RoomTypeDetail>> GetAllActive()
        {
            return _unitOfWork.GetGenericRepository<RoomTypeDetail>()
                              .GetWhereAsync(r => r.DeletedTime == null);
        }
        //Get by ID
        public Task<RoomTypeDetail?> GetById(object id)
        {
            return _unitOfWork.GetGenericRepository<RoomTypeDetail>().GetByIdAsync(id);
        }
        public async Task Add(RoomTypeDetail roomTypeDetail)
        {
            roomTypeDetail.Id = Guid.NewGuid().ToString("N");

            // Kiểm tra xem InternalCode đã tồn tại chưa
            IGenericRepository<RoomTypeDetail> genericRepository = _unitOfWork.GetGenericRepository<RoomTypeDetail>();
            bool exists = await genericRepository.ExistsAsync(r => r.InternalCode == roomTypeDetail.InternalCode);

            if (exists)
            {
                _logger.LogWarning("RoomTypeDetail with the same InternalCode already exists: {InternalCode}", roomTypeDetail.InternalCode);
                throw new DuplicateInternalCodeException("RoomTypeDetail with the same InternalCode already exists");
            }
            //Kiểm tra khoá ngoại
            IGenericRepository<RoomCategory> listRoomCategory = _unitOfWork.GetGenericRepository<RoomCategory>();
            bool catygory = await genericRepository.ExistsAsync(r => r.Id == roomTypeDetail.RoomCategoryId);
            if (catygory)
            {
                _logger.LogWarning("RoomTypeDetail with RoomCategoryId not exists {RoomCategoryId}", roomTypeDetail.RoomCategoryId);
                throw new ForeignKeyViolationException("RoomTypeDetail with RoomCategoryId not exists");
            }
            try
            {
                await genericRepository.InsertAsync(roomTypeDetail);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,"Failed to insert RoomTypeDetail");
                throw new InvalidOperationException("Failed to insert RoomTypeDetail");
            }
        }
    }
}
