

using Hotel.Application.DTOs.RoomTypeDTO;
using Hotel.Domain.Entities;

namespace Hotel.Application.Interfaces
{
    public interface IRoomTypeService
    {
        Task<List<RoomType>> GetAllRoomType();
        Task<RoomType> CreateRoomType(CreateRoomTypeDTO model);
        Task DeleteRoomType(string id);
        Task<List<GetRoomTypeDTO>> Get();
    }
}
