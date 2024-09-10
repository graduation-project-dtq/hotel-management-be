using AutoMapper;
using Hotel.Contract.Repositories.Entity;
using Hotel.ModelViews.RoomModelView;
using Hotel.ModelViews.RoomTypeDetailsMovelView;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Hotel_API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping  RoomCreateModelView to Room
            CreateMap<RoomModelView, Room>();
            CreateMap<Room, RoomModelView>();

            // Mapping  RoomTypeDetail to RoomTypeDetailModelView
            CreateMap<RoomTypeDetail, RoomTypeDetailMovelView>();
            CreateMap<RoomTypeDetailMovelView, RoomTypeDetail>();

        }
       
    }
}
