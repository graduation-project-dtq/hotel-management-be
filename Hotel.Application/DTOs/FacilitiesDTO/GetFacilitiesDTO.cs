using Hotel.Application.DTOs.ImageDTO;

namespace Hotel.Application.DTOs.FacilitiesDTO
{
    public class GetFacilitiesDTO
    {
        public string Id {  get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ? Description { get; set; }
        public virtual ICollection<GetImage>? Images { get; set; }
    }
}
