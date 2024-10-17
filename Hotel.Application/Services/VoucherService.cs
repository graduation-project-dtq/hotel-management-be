using AutoMapper;
using Hotel.Application.DTOs.VoucherDTO;
using Hotel.Application.Extensions;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
using Hotel.Core.Common;
using Hotel.Core.Constants;
using Hotel.Core.Exceptions;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Hotel.Application.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<VoucherService> _logger;
        private Regex regex = new Regex(@"^[1-9]\d*$");

        public VoucherService(IUnitOfWork unitOfWork,IMapper mapper, IHttpContextAccessor contextAccessor,ILogger<VoucherService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }
        public async Task<PaginatedList<GetVoucherDTO>> GetPageAsync(int index, int pageSize, string idSearch,string customerId)
        {
            if (index <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng nhập số trang hợp lệ!");
            }

            IQueryable<Voucher> query = _unitOfWork.GetRepository<Voucher>().Entities
                 .Where(c => !c.DeletedTime.HasValue)
                 .OrderByDescending(c => c.CreatedTime);

            // Áp dụng điều kiện tìm kiếm theo ID
            if (!string.IsNullOrWhiteSpace(idSearch))
            {
                query = query.Where(r => r.Id.ToString() == idSearch);
                bool exists = await query.AnyAsync();
                if (!exists)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy phòng với ID đã cho!");
                }
            }
            if(!String.IsNullOrWhiteSpace(customerId))
            {
                query = query.Where(r => r.CustomerId == customerId);
            }
            var totalCount = await query.CountAsync();  // Tổng số bản ghi
            if (totalCount == 0)
            {
                return new PaginatedList<GetVoucherDTO>(new List<GetVoucherDTO>(), totalCount, index, pageSize);
            }

            var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();

            // Ánh xạ từ Room sang GetRoomDTO
            var responseItems = resultQuery.Select(room => _mapper.Map<GetVoucherDTO>(room)).ToList();

            // Tạo danh sách phân trang
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var responsePaginatedList = new PaginatedList<GetVoucherDTO>(
                responseItems,
                totalCount,
                index,
                pageSize
            );
            return responsePaginatedList;
        }
        public async Task<List<GetVoucherDTO>> GetVoucherByCustomerId(string customerID)
        {
            if(String.IsNullOrEmpty(customerID))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Vui lòng nhập khách hàng để xem voucher!");
            }
            Customer customer = await _unitOfWork.GetRepository<Customer>().GetByIdAsync(customerID)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Khách hàng không tồn tại!");

            List<Voucher> vouchers = await _unitOfWork.GetRepository<Voucher>().Entities
                  .Where(v => (v.CustomerId == null || v.CustomerId == customerID) && v.IsActive == true  && v.DeletedTime == null)
                  .ToListAsync();
            Console.WriteLine($"CustomerId: {customerID}, Found Vouchers: {vouchers.Count}");
            //
            //if(vouchers.Count == 0)
            //{
            //    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Khách hàng không có voucher!");
            //}
            List<GetVoucherDTO> voucherModel = _mapper.Map<List<GetVoucherDTO>>(vouchers);
            return voucherModel;
        }
        public async  Task CreateVoucher(PostVoucherDTO model)
        {
            if(!regex.IsMatch(model.Quantity.ToString()))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Vui lòng nhập đúng số lượng voucher!");
            }

            Voucher voucher = await _unitOfWork.GetRepository<Voucher>().Entities.Where(v=>v.Code.Equals(model.Code) && v.IsActive == true).FirstOrDefaultAsync();
            if (voucher != null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Mã voucher đã tồn tại!");

            }
            if (model.StartDate > model.EndDate)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc!");

            }
            Voucher voucherInsert =_mapper.Map<Voucher>(model);
            string userId = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

            voucherInsert.IsActive = true;
            voucherInsert.CreatedBy=userId;
            voucherInsert.LastUpdatedBy=userId;
            await _unitOfWork.GetRepository<Voucher>().InsertAsync(voucherInsert);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task UpdateVoucher(string id, PutVoucherDTO model)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Vui lòng nhập Id của voucher!");
            }
            Voucher voucher = await _unitOfWork.GetRepository<Voucher>().Entities.FirstOrDefaultAsync(v => v.Id == id)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy voucher!");

            string userId = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

            voucher = _mapper.Map<Voucher>(model);
            voucher.LastUpdatedBy = userId;
            voucher.LastUpdatedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<Voucher>().UpdateAsync(voucher);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteVoucher(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Vui lòng nhập Id của voucher!");
            }
            Voucher voucher = await _unitOfWork.GetRepository<Voucher>().Entities.FirstOrDefaultAsync(v => v.Id == id)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy voucher!");
            string userId = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            voucher.DeletedBy = userId;
            voucher.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<Voucher>().UpdateAsync(voucher);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
