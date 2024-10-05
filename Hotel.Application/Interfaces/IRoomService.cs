
using Hotel.Application.DTOs.RoomDTO;
using Hotel.Application.PaggingItems;

namespace Hotel.Application.Interfaces
{
    public interface IRoomService
    {
        Task<List<GetRoomDTO>> GetAllRoom();
        Task<GetRoomDTO> CreateRoom(PostRoomDTO portRoom);
        Task<GetRoomDTO> GetRoomById(string id);
        Task<List<GetRoomDTO>> FindRoomBooking(DateOnly checkInDate, DateOnly checkOutDate, string roomTypeDetailID);
        Task<PaginatedList<GetRoomDTO>> GetPageAsync(int index, int pageSize, string idSearch,string nameSreach);

    }
}
