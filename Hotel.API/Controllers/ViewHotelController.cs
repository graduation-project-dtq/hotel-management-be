using Hotel.Application.DTOs.RoomDTO;
using Hotel.Application.DTOs.ViewHotelDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
using Hotel.Application.Services;
using Hotel.Core.Constants;
using Hotel.Domain.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewHotelController : ControllerBase
    {
        private readonly IViewHotelService _viewHotelService;
        public ViewHotelController(IViewHotelService viewHotelService)
        {
            _viewHotelService= viewHotelService;
        }
        [HttpGet]
        public async Task<IActionResult> GetPageAsync(int index=1, int pageSize =10 , string idSearch = "")
        {
            PaginatedList<GetViewHotelDTO> result = await _viewHotelService.GetPageAsync(index, pageSize, idSearch);
            return Ok(new BaseResponseModel<PaginatedList<GetViewHotelDTO>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Lấy danh sách view thành công!",
                data: result
            ));
        }

        [HttpPost]
        public async Task<IActionResult> CreateViewHotel(PostViewHotelDTO model)
        {
            GetViewHotelDTO result = await _viewHotelService.CreateViewHotel(model);
            return Ok(new BaseResponseModel<GetViewHotelDTO>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Thêm view thành công!",
                data: result
            ));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateViewHotel(string id,PutViewHotelDTO model)
        {
            GetViewHotelDTO result = await _viewHotelService.UpdateViewHotel(id, model);
            return Ok(new BaseResponseModel<GetViewHotelDTO>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Thêm view thành công!",
                data: result
            ));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteViewHotel(string id)
        {
            await _viewHotelService.DeleteViewHotel(id);
            return Ok(new BaseResponseModel<string?>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Xoá view thành công!",
                data: null
            ));
        }
    }
}
