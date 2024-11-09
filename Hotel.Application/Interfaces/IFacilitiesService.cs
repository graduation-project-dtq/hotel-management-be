
using Hotel.Application.DTOs.FacilitiesDTO;

namespace Hotel.Application.Interfaces
{
    public interface IFacilitiesService
    {
        Task<GetFacilitiesDTO> CreateFacilities(PostFacilitiesDTO model);
        Task<GetFacilitiesDTO> UpdateFacilities(string id,PutFacilitiesDTO model);
        Task DeleteFacilities(string id);
    }
}
