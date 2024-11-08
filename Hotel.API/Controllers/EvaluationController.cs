using Hotel.Application.DTOs.BookingDTO;
using Hotel.Application.DTOs.EvaluationDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sprache;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluationController : ControllerBase
    {
        private readonly IEvaluationService _evaluationService;
        public EvaluationController(IEvaluationService evaluationService)
        {
            _evaluationService = evaluationService;
        }
        [HttpGet("getPage")]
        public async Task<IActionResult> GetPageAsync(int index = 1, int pageSize = 10, string idSearch = "", string customerID = "", string roomTypeDetailId = "")
        {
            PaginatedList<GetEvaluationDTO> result = await _evaluationService.GetPageAsync( index,  pageSize, idSearch, customerID, roomTypeDetailId);
            return Ok(new BaseResponse<PaginatedList<GetEvaluationDTO>>(
               statusCode: StatusCodes.Status200OK,
               code: ResponseCodeConstants.SUCCESS,
               data: result,
               message: "Lấy thông tin giá thành công!"
           ));
        }
        [HttpGet("all")]
        public async Task<IActionResult>  GetEvaluationAsync(string roomTypeDetailId)
        {
            List<GetEvaluationDTO> result = await _evaluationService.GetEvaluationAsync(roomTypeDetailId);
            return Ok(new BaseResponse<List<GetEvaluationDTO>>(
               statusCode: StatusCodes.Status200OK,
               code: ResponseCodeConstants.SUCCESS,
               data: result,
               message: "Lấy thông tin giá thành công!"
           ));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateEvaluationAsync([FromForm] List<IFormFile>? images, [FromForm] PostEvaluationDTO model)
        {
            await _evaluationService.CreateEvaluationAsync(images, model);
            return Ok(new BaseResponse<string?>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: null,
                message: "Đánh giá thành công!"
            ));
        }

    }
}
