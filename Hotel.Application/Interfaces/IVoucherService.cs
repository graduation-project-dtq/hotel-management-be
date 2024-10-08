
using Hotel.Application.DTOs.VoucherDTO;
using Hotel.Application.PaggingItems;

namespace Hotel.Application.Interfaces
{
    public interface IVoucherService
    {
        Task<PaginatedList<GetVoucherDTO>> GetPageAsync(int index, int pageSize, string idSearch);

    }
}
