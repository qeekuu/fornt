
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models
{
    public class Token
    {
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Email { get; set; }
        public string Value {  get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        public User User { get; set; }

    }
}
