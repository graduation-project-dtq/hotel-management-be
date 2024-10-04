using AutoMapper;
using Hotel.Application.DTOs.ImageDTO;
using Hotel.Application.Extensions;
using Hotel.Application.Interfaces;
using Hotel.Core.Common;
using Hotel.Core.Constants;
using Hotel.Core.Exceptions;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace Hotel.Application.Services
{
    public class ImageService : IImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        public ImageService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }
        public async Task<List<GetImageDTO>> GetAllImage()
        {
            var listImage = await _unitOfWork.GetRepository<Image>().Entities
                .Where(i=>i.DeletedTime==null).OrderBy(i=>i.LastUpdatedTime).ToListAsync();
            List<GetImageDTO> images= _mapper.Map<List<GetImageDTO>>(listImage);
            return images;
        }
        public async Task<GetImageDTO> CreateImage(PostImageDTO model)
        {
            var imageExited = _unitOfWork.GetRepository<Image>().Entities.FirstOrDefaultAsync(i => i.URL == model.URL)
                ?? throw new ErrorException(StatusCodes.Status409Conflict, ResponseCodeConstants.EXISTED, "Đã tồn tại ảnh");

            string userId = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            Image image=_mapper.Map<Image>(model);

            image.CreatedBy = userId;
            image.LastUpdatedBy= userId;
            image.CreatedTime = CoreHelper.SystemTimeNow;
            image.DeletedTime = CoreHelper.SystemTimeNow;

            await  _unitOfWork.GetRepository<Image>().InsertAsync(image);
            await _unitOfWork.SaveChangesAsync();

            GetImageDTO getImage = _mapper.Map<GetImageDTO>(image);
            return getImage;

        }
    }
}