
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models
{
    public class User
    {
        public User(string name, string surname, string email, string login)
        {
            Name = name;
            Surname = surname;
            Email = email;
            Login = login;
        }

        public int Id { get; set; }
        
        [Required,MaxLength(50)]
        public string Login { get; set; }
        
        [Required, MaxLength(50)]
        public string Email { get; set; }
        
        [Required]
        public string HashPassword { get; set; }
        
        [Required, MaxLength(50)]
        public string Name { get; set; }
        
        [Required, MaxLength(50)]
        public string Surname { get; set; }

        public Token? Token { get; set; }
        public ICollection<Photo> Photos { get; set; } = new List<Photo>();
        public ICollection<PhotoShare> PhotosSharedWithMe { get; set; } = new List<PhotoShare>();
    }
}
