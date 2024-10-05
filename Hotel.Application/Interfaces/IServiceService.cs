using Hotel.Application.DTOs.ServiceDTO;
using Hotel.Application.PaggingItems;

namespace Hotel.Application.Interfaces
{
    public interface IServiceService
    {
        Task<PaginatedList<GetServiceDTO>> GetPageAsync(int index, int pageSize, string idSearch, string nameSearch);
        Task CreateService(PostServiceDTO model);
        Task UpdateService(string id, PutServiceDTO model);
        Task DeleteService(string id);
    }
}
