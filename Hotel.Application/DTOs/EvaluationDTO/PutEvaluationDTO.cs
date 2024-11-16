
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.EvaluationDTO
{
    public class PutEvaluationDTO
    {
        [FromForm]
        public string CustomerId { get; set; } = string.Empty;
      
        [FromForm]
        public string RoomTypeId { get; set; } = string.Empty;
        [FromForm]
        public string ? Comment { get; set; } = string.Empty;
        [FromForm]
        public float ? Starts { get; set; }
    }
}
