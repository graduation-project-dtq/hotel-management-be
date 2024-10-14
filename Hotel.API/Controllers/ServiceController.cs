using Hotel.Application.DTOs.ServiceDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Hotel.Domain.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        /// <summary>
        /// Lấy danh sách dịch vụ
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="idSearch"></param>
        /// <param name="nameSearch"></param>

        [HttpGet]
        public async Task<IActionResult> GetPageAsync(int index=1, int pageSize=10, string idSearch="", string nameSearch="")
        {
            PaginatedList<GetServiceDTO> result=await _serviceService.GetPageAsync(index, pageSize, idSearch, nameSearch);
            return Ok(new BaseResponseModel<PaginatedList<GetServiceDTO>>(
               statusCode: StatusCodes.Status200OK,
               code: ResponseCodeConstants.SUCCESS,
               message: "Lấy danh sách dịch vụ thành công!",
               data: result
           ));
        }
        [HttpPost]
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        public async Task<IActionResult> CreateService([FromBody] PostServiceDTO model)
        {
           
            await _serviceService.CreateService(model);
            return Ok(new BaseResponseModel<string ?>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Tạo dịch vụ thành công!",
                data: null
            ));

        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        public async Task<IActionResult> CreateService(string id,[FromBody] PutServiceDTO model)
        {

            await _serviceService.UpdateService(id,model);
            return Ok(new BaseResponseModel<string?>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Sửa dịch vụ thành công!",
                data: null
            ));

        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        public async Task<IActionResult> DeleteService(string id)
        {
            await _serviceService.DeleteService(id);
            return Ok(new BaseResponseModel<string?>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Xoá dịch vụ thành công!",
                data: null
            ));

        }
    }
}
