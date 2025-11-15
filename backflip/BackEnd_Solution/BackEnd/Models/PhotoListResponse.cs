namespace BackEnd.Models
{
    public class PhotoListResponse
    {
        public PhotoResponse[] Photos { get; set; } = Array.Empty<PhotoResponse>();
    }
}