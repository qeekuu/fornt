namespace BackEnd.Models
{
    public class PhotoResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FilePath { get; set; }
        public DateTime UploadedAt { get; set; }
        public int OwnerId { get; set; }
    }
}