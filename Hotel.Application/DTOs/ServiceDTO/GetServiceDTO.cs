
using Hotel.Application.DTOs.ImageDTO;

namespace Hotel.Application.DTOs.ServiceDTO
{
    public class GetServiceDTO
    {
        public string Id {  get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ? CreateBy {  get; set; }
        public virtual ICollection<GetImageServiceDTO> ? GetImageServiceDTOs {  get; set; }
    }
}
