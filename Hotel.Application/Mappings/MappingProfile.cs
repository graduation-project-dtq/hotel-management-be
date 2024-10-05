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
using Hotel.Application.DTOs.ViewHotelDTO;
using Hotel.Application.DTOs.BookingDTO;
using Hotel.Application.DTOs.ServiceDTO;
using Hotel.Application.DTOs.HouseTypeDTO;

namespace Hotel.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Accout
            CreateMap<RegisterRequestDto, Account>();

            // Customer
            CreateMap<Customer, CreateCustomerDTO>().ReverseMap();
            CreateMap<Customer, PutCustomerDTO>().ReverseMap();
            CreateMap<Customer, GetCustomerDTO>().ReverseMap();

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
            CreateMap<PostRoomTypeDetailDTO, RoomTypeDetail>().ReverseMap();

            //Room
            CreateMap<GetRoomDTO, Room>().ReverseMap();
            CreateMap<PostRoomDTO, Room>().ReverseMap();
            CreateMap<PutRoomDTO, Room>().ReverseMap();

            //FLoor
            CreateMap<GetFloorDTO, Floor>().ReverseMap();
            CreateMap<PutFloorDTO, Floor>().ReverseMap();
            CreateMap<PostFloorDTO, Floor>().ReverseMap();

            //RoomTypeDetail
            CreateMap<GetRoomTypeDetailDTO, RoomTypeDetail>().ReverseMap();
            CreateMap<PostRoomTypeDetailDTO, RoomTypeDetail>().ReverseMap();

            //Image
            CreateMap<GetImageDTO, Image>().ReverseMap();
            CreateMap<PostImageDTO, Image>().ReverseMap();

            //ViewHotel
            CreateMap<ViewHotel,GetViewHotelDTO>().ReverseMap();
            CreateMap<ViewHotel, PostViewHotelDTO>().ReverseMap();
            CreateMap<ViewHotel, PutViewHotelDTO>().ReverseMap();

            //Booking
            CreateMap<Booking, GetBookingDTO>().ReverseMap();
            CreateMap<Booking, PostBookingDTO>().ReverseMap();
            CreateMap<Booking, PutBookingDTO>().ReverseMap();

            //Service
            CreateMap<Service, GetServiceDTO>().ReverseMap();
            CreateMap<Service, PostServiceDTO>().ReverseMap();
            CreateMap<Service, PutServiceDTO>().ReverseMap();

            //HouseType

            CreateMap<HouseType, GetHoustTypeDTO>().ReverseMap();
            CreateMap<HouseType, PostHouseTypeDTO>().ReverseMap();
            CreateMap<HouseType, PutHouseTypeDTO>().ReverseMap();


        }
    }
}
