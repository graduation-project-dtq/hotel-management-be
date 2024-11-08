using Hotel.Application.DTOs.OverviewDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OverviewController : ControllerBase
    {
        private IOverviewService _overviewService;
        public OverviewController(IOverviewService overviewService)
        { 
            _overviewService = overviewService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPageAsync(int index = 1, int pageSize = 10, string idSearch = "", string customerID = "")
        {
             PaginatedList<GetOverviewDTO> result= await _overviewService.GetPageAsync(index, pageSize, idSearch, customerID);
            return Ok(new BaseResponse<string?>
            (
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: null,
                message: "Lấy thông tin đánh giá thành công"
            ));
        }
        [HttpGet("average")]
        public async Task<IActionResult> GetAvgOverview()
        {
           GetOverviewDTO result= await _overviewService.GetAvgOverview();
            return Ok(new BaseResponse<GetOverviewDTO>
            (
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                message: "Lấy thông tin đánh giá thành công"
            ));
        }

        [HttpPost]
        public async Task<IActionResult> CreateOverview([FromBody]PostOverviewDTO model)
        {
            await _overviewService.CreateOverview(model);
            return Ok(new BaseResponse<string?>
            (
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: null,
                message: "Đánh giá thành công"
            ));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOverview(string id ,[FromBody] PutOverviewDTO model)
        {
            await _overviewService.UpdateOverview(id,model);
            return Ok(new BaseResponse<string?>
            (
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: null,
                message: "Cập nhật đánh giá thành công"
            ));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOverview([FromBody] string id)
        {
            await _overviewService.DeleteOverview(id);
            return Ok(new BaseResponse<string?>
            (
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: null,
                message: "Xoá đánh giá thành công"
            ));
        }
    }
}
