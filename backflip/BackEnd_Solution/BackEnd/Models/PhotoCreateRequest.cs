using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models
{
    public class PhotoCreateRequest
    {
        [Required]
        public IFormFile? File { get; set; }

        [Required, MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }
    }
}