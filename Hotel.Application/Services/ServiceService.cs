using AutoMapper;
using Hotel.Application.DTOs.ImageDTO;
using Hotel.Application.DTOs.ServiceDTO;
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

namespace Hotel.Application.Services
{
    public class ServiceService : IServiceService
    {
       
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        public ServiceService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        //Lấy thông tin service
        public async Task<PaginatedList<GetServiceDTO>> GetPageAsync(int index, int pageSize, string idSearch, string nameSearch)
        {
            if (index <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng nhập số trang hợp lệ!");
            }

            IQueryable<Service> query = _unitOfWork.GetRepository<Service>().Entities
                 .Include(r => r.ImageServices)
                 .Where(c => !c.DeletedTime.HasValue)
                 .OrderByDescending(c => c.CreatedTime);

            // Áp dụng điều kiện tìm kiếm theo ID
            if (!string.IsNullOrWhiteSpace(idSearch))
            {
                query = query.Where(r => r.Id.ToString() == idSearch);
                bool exists = await query.AnyAsync();
                if (!exists)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy dịch vu với ID đã cho!");
                }
            }

            // Áp dụng điều kiện tìm kiếm theo tên
            if (!string.IsNullOrWhiteSpace(nameSearch))
            {
                query = query.Where(r => r.Name.Contains(nameSearch));
                bool exists = await query.AnyAsync();
                if (!exists)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy phòng với tên đã cho!");
                }
            }

            var totalCount = await query.CountAsync();  // Tổng số bản ghi
            if (totalCount == 0)
            {
                return new PaginatedList<GetServiceDTO>(new List<GetServiceDTO>(), totalCount, index, pageSize);
            }

            var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();

            
            List<GetServiceDTO> responseItems = new List<GetServiceDTO>();
        
            foreach (Service item in resultQuery)
            {
                Account account = await _unitOfWork.GetRepository<Account>().GetByIdAsync(item.CreatedBy != null ? item.CreatedBy : "");;
                GetServiceDTO response = new GetServiceDTO()
                {
                    Id = item.Id,
                    CreateBy= account !=null ? account.Name : item.CreatedBy,
                    Name = item.Name,
                     Price=item.Price,
                };
                response.GetImageServiceDTOs = new List<GetImageServiceDTO>();

                if (item.ImageServices.Count > 0) 
                { 
                    foreach(var image in item.ImageServices)
                    {
                        //Thêm url ảnh vào 
                        Image url=await _unitOfWork.GetRepository<Image>().GetByIdAsync(image.ImageID);
                        if(url != null)
                        {
                            response.GetImageServiceDTOs.Add(new GetImageServiceDTO() { URL = url.URL });
                        }    
                    }
                }
                responseItems.Add(response);
            }
            // Tạo danh sách phân trang
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var responsePaginatedList = new PaginatedList<GetServiceDTO>(
                responseItems,
                totalCount,
                index,
                pageSize
            );

            return responsePaginatedList;
        }

        //Tạo dịch vụ mới

        public async Task CreateService(PostServiceDTO model)
        {
            if(model.Price<=0 || model.Price % 1!=0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Giá tiền phải lớn hơn không và là số nguyên dương");
            }   
            string userID=Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            Service service = new Service()
            {
                Name = model.Name,
                Price = model.Price > 0 ? model.Price : 0,
                Description=model.Description,
            };
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.GetRepository<Service>().InsertAsync(service);
            if (model.PostImageServiceDTOs !=null  )
            {
                //Thêm ảnh
                foreach(var item in model.PostImageServiceDTOs)
                {
                    Image image = new Image()
                    {
                        URL=item.URL,
                        CreatedBy=userID,
                        LastUpdatedBy=userID,
                        CreatedTime=CoreHelper.SystemTimeNow,
                        LastUpdatedTime=CoreHelper.SystemTimeNow,
                    };
                    await _unitOfWork.GetRepository<Image>().InsertAsync(image);
                    await _unitOfWork.SaveChangesAsync();
                    Hotel.Domain.Entities.ImageService imageService = new Hotel.Domain.Entities.ImageService()
                    {
                        ImageID = image.Id,
                        ServiceID=service.Id,
                    };
             
                    await _unitOfWork.GetRepository<Hotel.Domain.Entities.ImageService>().InsertAsync(imageService);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateService(string id, PutServiceDTO model)
        {
            if (model.Price <= 0 || model.Price % 1 != 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Giá tiền phải lớn hơn không và là số nguyên dương");
            }
            string userID = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            Service service = await _unitOfWork.GetRepository<Service>().GetByIdAsync(id)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy dịch vụ!");
          
            service.Name= model.Name;
            service.Price = model.Price;
            service.Description= model.Description;
            service.LastUpdatedBy = userID;
            service.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Service>().UpdateAsync(service);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteService(string id)
        {
            if(String.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Không được để trống ID!");
            }
            Service service = await _unitOfWork.GetRepository<Service>().GetByIdAsync(id)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy dịch vụ!");

            string userID = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            service.DeletedBy=userID;
            service.DeletedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Service>().UpdateAsync(service);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
