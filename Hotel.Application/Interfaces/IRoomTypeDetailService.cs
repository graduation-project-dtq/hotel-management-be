
using Hotel.Application.DTOs.RoomTypeDetailDTO;
using Hotel.Application.DTOs.RoomTypeDTO;
using Hotel.Domain.Entities;


namespace Hotel.Application.Interfaces
{
    public interface IRoomTypeDetailService
    {
        Task<List<GetRoomTypeDetailDTO>> GetAllRoomTypeDetail();
        Task<PortRoomTypeDetailDTO> CreateRoomTypeDetail(PortRoomTypeDetailDTO portRoomTypeDetail);
    }
}
