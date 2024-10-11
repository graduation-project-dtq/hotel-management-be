using Hotel.Application.DTOs.PriceAdjustmentPlanDTO;
using Hotel.Application.PaggingItems;

namespace Hotel.Application.Interfaces
{
    public interface IPriceAdjustmentPlanService
    {
        Task<PaginatedList<GetPriceAdjustmentPlanDTO>> GetPageAsync(int index, int pageSize, string idSearch, string nameSearch);
        Task CreateRoomPriceAdjustmentPlan(PostPriceAdjustmentPlanDTO model);
        Task UpdateRoomPriceAdjustmentPlan(string id,PutPriceAdjustmentPlanDTO model);
        Task DeleteRoomPriceAdjustmentPlan(string id);
    }
}
