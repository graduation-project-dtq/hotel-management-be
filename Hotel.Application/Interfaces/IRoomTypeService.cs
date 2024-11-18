

using Hotel.Application.DTOs.RoomTypeDTO;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Hotel.Application.Interfaces
{
    public interface IRoomTypeService
    {
        Task<List<GetRoomTypeDTO>> GetAllRoomType();
        Task<GetRoomTypeDTO> CreateRoomType(ICollection<IFormFile>? images,PortRoomTypeDTO model);
        Task UpdateRoomType(string id,ICollection<IFormFile>? images, PutRoomTypeDTO model);
        Task DeleteRoomType(string id);
        Task<GetRoomTypeDTO> GetRoomTypeById(string id);

    }
}
