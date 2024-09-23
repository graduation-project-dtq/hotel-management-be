

using Hotel.Application.DTOs.RoomType;
using Hotel.Domain.Entities;

namespace Hotel.Application.Interfaces
{
    public interface IRoomTypeService
    {
        Task<List<RoomType>> GetAllRoomType();
        Task<RoomType> CreateRoomType(CreateRoomType model);
        Task DeleteRoomType(string id);
    }
}
