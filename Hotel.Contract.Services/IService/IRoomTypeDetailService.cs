using Hotel.Contract.Repositories.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Contract.Services.IService
{
    public interface IRoomTypeDetailService
    {
        Task<IList<RoomTypeDetail>> GetAll();
        Task<IList<RoomTypeDetail>> GetAllActive();
        Task<RoomTypeDetail?> GetById(object id);
        Task Add(RoomTypeDetail roomTypeDetail);
    }
}
