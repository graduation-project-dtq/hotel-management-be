using Hotel.Application.DTOs.OverviewDTO;
using Hotel.Application.PaggingItems;

namespace Hotel.Application.Interfaces
{
    public interface IOverviewService
    {
        Task<PaginatedList<GetOverviewDTO>> GetPageAsync(int index, int pageSize, string idSearch, string customerID);
        Task<GetOverviewDTO> GetAvgOverview();
        Task CreateOverview(PostOverviewDTO model);
        Task UpdateOverview(string id, PutOverviewDTO model);
        Task DeleteOverview(string id);
    }
}
