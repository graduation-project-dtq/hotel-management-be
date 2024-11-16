using Hotel.Application.DTOs.FloorDTO;

namespace Hotel.Application.Interfaces
{
    public interface IFloorService
    {
        Task<List<GetFloorDTO>> GetAllFloor();
        
        Task<GetFloorDTO> CreateFloor(PostFloorDTO postFloorDTO);
        Task<GetFloorDTO> UpdateFloor(string id,PutFloorDTO model);
        Task DeleteFloor(string id);
    }
}
