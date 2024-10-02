
using Hotel.Application.DTOs.RoomDTO;

namespace Hotel.Application.Interfaces
{
    public interface IRoomService
    {
        Task<List<GetRoomDTO>> GetAllRoom();
        Task<GetRoomDTO> CreateRoom(PostRoomDTO portRoom);
        Task<GetRoomDTO> GetRoomById(string id);
        Task<List<GetRoomDTO>> FindRoomBooking(DateTime checkInDate, DateTime checkOutDate, string roomTypeDetailID);
    }
}
