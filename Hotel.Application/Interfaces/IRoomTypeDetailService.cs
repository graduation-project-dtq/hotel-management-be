
using Hotel.Application.DTOs.RoomTypeDTO;
using Hotel.Domain.Entities;


namespace Hotel.Application.Interfaces
{
    public interface IRoomTypeDetailService
    {
        Task<List<GetRoomTypeDTO>> GetAllRoomType();
    }
}
