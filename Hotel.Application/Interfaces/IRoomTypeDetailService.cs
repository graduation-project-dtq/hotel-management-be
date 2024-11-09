
using Hotel.Application.DTOs.RoomTypeDetailDTO;
using Hotel.Application.DTOs.RoomTypeDTO;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Http;


namespace Hotel.Application.Interfaces
{
    public interface IRoomTypeDetailService
    {
        Task<List<GetRoomTypeDetailDTO>> GetAllRoomTypeDetail();
        Task<GetRoomTypeDetailDTO> CreateRoomTypeDetail(ICollection<IFormFile> images, PostRoomTypeDetailDTO portRoomTypeDetail);
        Task<List<List<GetRoomTypeDetailDTO>>> FindRoom(int soNguoi, string roomTypeID);
        Task<List<GetRoomTypeDetailDTO>> GetByRoomTypeId(string id);
        Task<GetRoomTypeDetailDTO> GetById(string id);
        Task<decimal> GetDiscountPrice(string roomTypeDetailId);
    }
}
