using Hotel.Application.DTOs.PriceAdjustmentPlanDTO;
using Hotel.Application.DTOs.RoomDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
using Hotel.Application.Services;
using Hotel.Core.Common;
using Hotel.Core.Constants;
using Hotel.Domain.Base;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sprache;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceAdjustmentPlanController : ControllerBase
    {
        private readonly IPriceAdjustmentPlanService _priceAdjustmentPlanService;
        private readonly IUnitOfWork _unitOfWork;
        public PriceAdjustmentPlanController(IPriceAdjustmentPlanService priceAdjustmentPlanService,IUnitOfWork unitOfWork)
        {
            _priceAdjustmentPlanService = priceAdjustmentPlanService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetPageAsync(int index = 1, int pageSize = 10, string idSearch = "", string nameSreach = "")
        {
            PaginatedList<GetPriceAdjustmentPlanDTO> result = await _priceAdjustmentPlanService.GetPageAsync(index, pageSize, idSearch, nameSreach);
            return Ok(new BaseResponseModel<PaginatedList<GetPriceAdjustmentPlanDTO>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Lấy danh sách điều chỉnh giá phòng thành công!",
                data: result
            ));
        }
        [HttpPost]
        public async Task<IActionResult> CreateRoomPriceAdjustmentPlan(PostPriceAdjustmentPlanDTO model)
        {
            await _priceAdjustmentPlanService.CreateRoomPriceAdjustmentPlan(model);
            return Ok(new BaseResponseModel<string ?>(
                 statusCode: StatusCodes.Status200OK,
                 code: ResponseCodeConstants.SUCCESS,
                 message: "Tạo điều chỉnh giá phòng thành công!",
                 data: null
             ));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateRoomPriceAdjustmentPlan(string id, PutPriceAdjustmentPlanDTO model)
        {
            await _priceAdjustmentPlanService.UpdateRoomPriceAdjustmentPlan(id,model);
            return Ok(new BaseResponseModel<string?>(
                 statusCode: StatusCodes.Status200OK,
                 code: ResponseCodeConstants.SUCCESS,
                 message: "Chỉnh sửa điều chỉnh giá phòng thành công!",
                 data: null
             ));
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteRoomPriceAdjustmentPlan(string id)
        {
            await _priceAdjustmentPlanService.DeleteRoomPriceAdjustmentPlan(id);
            return Ok(new BaseResponseModel<string?>(
                 statusCode: StatusCodes.Status200OK,
                 code: ResponseCodeConstants.SUCCESS,
                 message: "Xoá điều chỉnh giá phòng thành công!",
                 data: null
             ));
        }
        [HttpGet("Test")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(_unitOfWork.GetRepository<PriceAdjustmentPlan>().Entities.Where(p => p.StartDate <= CoreHelper.SystemTimeNow && p.EndDate >= CoreHelper.SystemTimeNow)
                    .OrderByDescending(p => p.StartDate)
                    .FirstOrDefault());
        }
    }
}
