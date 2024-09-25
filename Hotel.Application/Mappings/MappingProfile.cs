using AutoMapper;
using Hotel.Application.DTOs.CustomerDTO;
using Hotel.Application.DTOs.EmployeeDTO;
using Hotel.Application.DTOs.FloorDTO;
using Hotel.Application.DTOs.RoomTypeDetailDTO;
using Hotel.Application.DTOs.RoomTypeDTO;
using Hotel.Application.DTOs.UserDTO;
using Hotel.Domain.Entities;

namespace Hotel.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            //Accout
            CreateMap<RegisterRequestDto, Account>();

            //Customer
            CreateMap<CreateCustomerDTO, Customer>();

            //Employee
            CreateMap<CreateEmployeeDTO, Employee>();

            //RoomType
            CreateMap<CreateRoomTypeDTO,RoomType>();

            CreateMap<GetRoomTypeDetailDTO,RoomTypeDetail>();

            CreateMap<GetFloorDTO,Floor>();
        }
    
    }
}
