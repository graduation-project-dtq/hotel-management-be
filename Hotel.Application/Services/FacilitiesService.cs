
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
        public async Task<GetFacilitiesDTO> CreateFacilities(PostFacilitiesDTO model)
        {
            Facilities ? exitFacilities = await _unitOfWork.GetRepository<Facilities>().Entities.Where(f => f.Name.Equals(model) && !f.DeletedTime.HasValue).FirstOrDefaultAsync();
            if(exitFacilities != null)
            {
                throw new ErrorException(StatusCodes.Status409Conflict, ResponseCodeConstants.DUPLICATE, "Đã tồn tại tên này");
            }
        }
        //public async Task<GetFacilitiesDTO> UpdateFacilities(string id, PutFacilitiesDTO model)
        //{

        //}
        //public async Task DeleteFacilities(string id)
        //{

        //}
    }
}
