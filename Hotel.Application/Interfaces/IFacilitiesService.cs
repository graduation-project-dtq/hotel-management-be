
using Hotel.Application.DTOs.CustomerDTO;
using Hotel.Application.DTOs.FacilitiesDTO;
using Hotel.Application.PaggingItems;
using Microsoft.AspNetCore.Http;

namespace Hotel.Application.Interfaces
{
    public interface IFacilitiesService
    {
        Task<PaginatedList<GetFacilitiesDTO>> GetPageAsync(int index, int pageSize, string idSearch,
          string nameSearch);

        Task<PaginatedList<GetFacilitiesRoomDTO>> GetFacilitiesByRoomId(int index, int pageSize, string roomId,
         string nameSearch);

        Task<GetFacilitiesDTO> CreateFacilities(ICollection<IFormFile>? images,PostFacilitiesDTO model);
        Task<GetFacilitiesDTO> UpdateFacilities(string id, PutFacilitiesDTO model, ICollection<IFormFile>? images);
        Task DeleteFacilities(string id);
    }
}
