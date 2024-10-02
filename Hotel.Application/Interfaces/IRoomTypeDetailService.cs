
using Hotel.Application.DTOs.RoomTypeDetailDTO;
using Hotel.Application.DTOs.RoomTypeDTO;
using Hotel.Domain.Entities;


namespace Hotel.Application.Interfaces
{
    public interface IRoomTypeDetailService
    {
        Task<List<GetRoomTypeDetailDTO>> GetAllRoomTypeDetail();
        Task<RoomTypeDetail> CreateRoomTypeDetail(PostRoomTypeDetailDTO portRoomTypeDetail);
        Task<GetRoomTypeDetailDTO> GetRoomTypeDetailById(string id);
        Task<List<List<GetRoomTypeDetailDTO>>> FindRoom(int soNguoi, string roomTypeID);
        Task<List<GetRoomTypeDetailDTO>> GetByRoomTypeId(string id);
    }
}
