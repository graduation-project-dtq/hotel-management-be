
using Hotel.Application.DTOs.VoucherDTO;
using Hotel.Application.PaggingItems;

namespace Hotel.Application.Interfaces
{
    public interface IVoucherService
    {
        Task<PaginatedList<GetVoucherDTO>> GetPageAsync(int index, int pageSize, string idSearch, string customerId);
        Task<List<GetVoucherDTO>> GetVoucherByCustomerId(string customerID);
        Task CreateVoucher(PostVoucherDTO model);
        Task UpdateVoucher(string id, PutVoucherDTO model);
        Task DeleteVoucher(string id);
    }
}
