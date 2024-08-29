using AutoMapper;
using Hotel.Contract.Repositories.Entity;
using Hotel.ModelViews.RoomModelView;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace XuongMayBE.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Cấu hình mapping từ RoomCreateModelView sang Room
            CreateMap<RoomModelView, Room>();
        }
    }
}
