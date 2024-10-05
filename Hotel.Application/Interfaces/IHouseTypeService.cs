using Hotel.Application.DTOs.HouseTypeDTO;
using Hotel.Application.PaggingItems;

namespace Hotel.Application.Interfaces
{
    public interface IHouseTypeService
    {
        Task<PaginatedList<GetHoustTypeDTO>> GetPageAsync(int index, int pageSize, string idSearch, string nameSearch);
        Task CreateHouseType(PostHouseTypeDTO model);
    }
}
