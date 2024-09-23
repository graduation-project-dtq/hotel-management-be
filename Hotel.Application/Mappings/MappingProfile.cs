using AutoMapper;
using Hotel.Application.DTOs.CustomerDTO;
using Hotel.Application.DTOs.EmployeeDTO;
using Hotel.Application.DTOs.RoomType;
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

            CreateMap<CreateRoomType,RoomType>();

        }
    
    }
}
