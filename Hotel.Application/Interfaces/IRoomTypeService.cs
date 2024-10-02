

using Hotel.Application.DTOs.RoomTypeDTO;
using Hotel.Domain.Entities;

namespace Hotel.Application.Interfaces
{
    public interface IRoomTypeService
    {
        Task<List<GetRoomTypeDTO>> GetAllRoomType();
        Task<RoomType> CreateRoomType(PortRoomTypeDTO model);
        Task DeleteRoomType(string id);
        Task<GetRoomTypeDTO> GetRoomTypeById(string id);

    }
}
