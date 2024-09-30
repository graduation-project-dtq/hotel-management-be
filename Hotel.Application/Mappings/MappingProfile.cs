using AutoMapper;
using Hotel.Application.DTOs.CustomerDTO;
using Hotel.Application.DTOs.EmployeeDTO;
using Hotel.Application.DTOs.FloorDTO;
using Hotel.Application.DTOs.RoomTypeDetailDTO;
using Hotel.Application.DTOs.RoomTypeDTO;
using Hotel.Application.DTOs.ImageDTO;
using Hotel.Domain.Entities;
using Hotel.Application.DTOs.UserDTO;
using Hotel.Application.DTOs.RoomDTO;

namespace Hotel.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Accout
            CreateMap<RegisterRequestDto, Account>();

            // Customer
            CreateMap<CreateCustomerDTO, Customer>();

            // Employee
            CreateMap<CreateEmployeeDTO, Employee>();

            //RoomType
            CreateMap<GetRoomTypeDTO, RoomType>().ReverseMap();

            CreateMap<PortRoomTypeDTO, RoomType>().ReverseMap();
            // Ánh xạ ImageRoomType sang GetImageRoomTypeDTO
            CreateMap<ImageRoomType, GetImageRoomTypeDTO>()
                .ForMember(dest => dest.URL, opt => opt.MapFrom(src => src.Image.URL)); // Cập nhật ở đây

            // Ánh xạ RoomTypeDetail sang GetRoomTypeDetailDTO
            CreateMap<RoomTypeDetail, GetRoomTypeDetailDTO>().ReverseMap();
            CreateMap<PortRoomTypeDetailDTO, RoomTypeDetail>().ReverseMap();

            //Room
            CreateMap<GetRoomDTO, Room>().ReverseMap();
            CreateMap<PostRoomDTO, Room>().ReverseMap();
            CreateMap<PutRoomDTO, Room>().ReverseMap();

            //FLoor
            CreateMap<GetFloorDTO, Floor>().ReverseMap();  
            CreateMap<PutFloorDTO, Floor>().ReverseMap();  
            CreateMap<PostFloorDTO, Floor>().ReverseMap();
        }
    }
}
