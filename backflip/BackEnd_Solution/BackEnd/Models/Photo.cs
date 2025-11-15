
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models
{
    public class Photo
    {
        public int Id { get; set; }
        [Required,MaxLength(200)]
        public string Title { get; set; }
        [MaxLength(1000)]
        public string? Description { get; set; }
        [Required,MaxLength(400)]
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public int OwnerId { get; set; }
        public User Owner { get; set; }
        public ICollection<PhotoShare>Shares { get; set; } = new List<PhotoShare>();
    }
}
