namespace BackEnd.Models
{
    public class PhotoShare
    {
        public int Id { get; set; }
        public int PhotoId { get; set; }
        public Photo Photo { get; set; }
        public int SharedWithUserId { get; set; }
        public User SharedWithUser { get; set; }

        public DateTime SharedAt { get; set; } = DateTime.UtcNow;
    }
}
