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
    public class RoomTypeDetailService : IRoomTypeDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RoomTypeDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
    }
}
