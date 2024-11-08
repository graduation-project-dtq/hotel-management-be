using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.ImageDTO
{
    public class PostImageViewModel
    {
        [Required(ErrorMessage = "File is required")]
        public IFormFile File { get; set; } 
    }
}
