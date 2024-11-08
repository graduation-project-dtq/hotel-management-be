using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Hotel.Core.Until
{
    public class FileUploadHelper
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public readonly static string _customDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        public FileUploadHelper(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            if (!Directory.Exists(_customDirectory))
            {
                Directory.CreateDirectory(_customDirectory);
            }
        }

        public static async Task<string> UploadFile(IFormFile file, string id)
        {
            if (file.Length > 0)
            {
                try
                {
                    string fileName = $"{id}.png"; // Luôn sử dụng phần mở rộng .png
                    string filePath = Path.Combine(_customDirectory, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return fileName;
                }
                catch (Exception ex)
                {
                    throw new Exception("File upload failed: " + ex.Message);
                }
            }
            else
            {
                throw new Exception("File upload failed, file is empty.");
            }
        }

        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
