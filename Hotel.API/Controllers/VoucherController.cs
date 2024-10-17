using Hotel.Application.DTOs.RoomDTO;
using Hotel.Application.DTOs.VoucherDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
using Hotel.Application.Services;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Hotel.Domain.Base;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }
        [HttpGet]
        public async Task<IActionResult> GetPageAsync(int index = 1, int pageSize = 10, string idSearch = "", string customerId = "")
        {
            PaginatedList<GetVoucherDTO> result = await _voucherService.GetPageAsync(index, pageSize, idSearch, customerId);
            return Ok(new BaseResponseModel<PaginatedList<GetVoucherDTO>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Lấy danh sách voucher thành công!",
                data: result
            ));
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetVoucherByCustomerId(string customerId)
        {
            List<GetVoucherDTO> result =await _voucherService.GetVoucherByCustomerId(customerId);
            return Ok(new BaseResponse<List<GetVoucherDTO>>(
              statusCode: StatusCodes.Status200OK,
              code: ResponseCodeConstants.SUCCESS,
              data: result,
              message: "Lấy danh sách voucher của khách hàng thành công!"
           ));
        }

        [HttpPost]
        public async Task<IActionResult> CreateVoucher(PostVoucherDTO model)
        {
            await _voucherService.CreateVoucher(model);
            return Ok(new BaseResponse<string?>(
              statusCode: StatusCodes.Status200OK,
              code: ResponseCodeConstants.SUCCESS,
              data: null,
              message: "Tạo voucher thành công!"
           ));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVoucher(string id,PutVoucherDTO model)
        {
            await _voucherService.UpdateVoucher(id,model);
            return Ok(new BaseResponse<string?>(
              statusCode: StatusCodes.Status200OK,
              code: ResponseCodeConstants.SUCCESS,
              data: null,
              message: "Update voucher thành công!"
           ));
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteVoucher(string id)
        {
            await _voucherService.DeleteVoucher(id);
            return Ok(new BaseResponse<string?>(
              statusCode: StatusCodes.Status200OK,
              code: ResponseCodeConstants.SUCCESS,
              data: null,
              message: "Xoá voucher thành công!"
           ));
        }
    }
}
