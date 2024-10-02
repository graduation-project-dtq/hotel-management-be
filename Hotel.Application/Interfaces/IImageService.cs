
using Hotel.Application.DTOs.ImageDTO;
using Hotel.Application.PaggingItems;

namespace Hotel.Application.Interfaces
{
    public interface IImageService
    {
        Task<List<GetImageDTO>> GetAllImage();
        Task<GetImageDTO> CreateImage(PostImageDTO model);
    }


}
