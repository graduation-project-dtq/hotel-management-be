
using Hotel.Application.DTOs.FacilitiesDTO;
using Microsoft.AspNetCore.Http;

namespace Hotel.Application.Interfaces
{
    public interface IFacilitiesService
    {
        Task<GetFacilitiesDTO> CreateFacilities(ICollection<IFormFile>? images,PostFacilitiesDTO model);
        //Task<GetFacilitiesDTO> UpdateFacilities(string id,PutFacilitiesDTO model);
        //Task DeleteFacilities(string id);
    }
}
