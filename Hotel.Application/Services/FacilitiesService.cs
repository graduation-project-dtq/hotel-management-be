
using AutoMapper;
using Hotel.Application.DTOs.FacilitiesDTO;
using Hotel.Application.Extensions;
using Hotel.Application.Interfaces;
using Hotel.Core.Constants;
using Hotel.Core.Exceptions;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Application.Services
{
    public class FacilitiesService  : IFacilitiesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFirebaseService _firebaseService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        private string currentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
        public FacilitiesService(IUnitOfWork unitOfWork,IFirebaseService firebaseService, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _firebaseService = firebaseService;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }
        public async Task<GetFacilitiesDTO> CreateFacilities(ICollection<IFormFile> ? images,PostFacilitiesDTO model)
        {
            Facilities ? exitFacilities = await _unitOfWork.GetRepository<Facilities>().Entities.Where(f => f.Name.Equals(model) && !f.DeletedTime.HasValue).FirstOrDefaultAsync();
            if(exitFacilities != null)
            {
                throw new ErrorException(StatusCodes.Status409Conflict, ResponseCodeConstants.DUPLICATE, "Đã tồn tại tên này");
            }

            if(model.Price<0 && model.Price %1 != 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Giá tiền không hợp lệ");

            }

            Facilities facilities = _mapper.Map<Facilities>(model);
            facilities.CreatedBy = currentUserId;
            facilities.LastUpdatedBy = currentUserId;

            await _unitOfWork.GetRepository<Facilities>().InsertAsync(facilities);
            await _unitOfWork.SaveChangesAsync();

            //Trả dữ liệu ra

          
            return null;
        }
        //public async Task<GetFacilitiesDTO> UpdateFacilities(string id, PutFacilitiesDTO model)
        //{

        //}
        //public async Task DeleteFacilities(string id)
        //{

        //}
    }
}
