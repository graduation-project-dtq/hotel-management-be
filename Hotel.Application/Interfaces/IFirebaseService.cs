using Hotel.Application.DTOs.ImageDTO;

namespace Hotel.Application.Interfaces
{
    public interface IFirebaseService
    {
        Task<string> UploadFileAsync(PostImageViewModel model);
        Task DeleteFileAsync(string fileName);
        string GetFileUrl(string fileName);
    }
}
