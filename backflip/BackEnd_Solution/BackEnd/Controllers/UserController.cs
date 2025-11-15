using BackEnd.Data;
using BackEnd.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
        private readonly IConfiguration _config;
        public UserController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _config = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterRequest r_user)
        {
            if(r_user == null)
            {
                return BadRequest("Brak danych.");
            }

            if (string.IsNullOrWhiteSpace(r_user.Email) || string.IsNullOrWhiteSpace(r_user.Name) || string.IsNullOrWhiteSpace(r_user.Surname) || string.IsNullOrWhiteSpace(r_user.Password) || string.IsNullOrWhiteSpace(r_user.Login))
            {
                return BadRequest("Email, Imie, Nazwisko, Login i Hasło są wymagane do rejestracji.");
            }

            bool EmailExist = await _context.Users.AnyAsync((u) => u.Email == r_user.Email);
            if (EmailExist)
            {
                return BadRequest("Podany adres e-mail jest już zajęty.");
            }

            User user = new User( r_user.Name,  r_user.Surname, r_user.Email, r_user.Login);

            user.HashPassword = _passwordHasher.HashPassword(user, r_user.Password);

            _context.Users.Add(user);
            
            await _context.SaveChangesAsync();

            return Ok("Rejestracja ukończona pomyślnie");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequest l_user)
        {   
            if(l_user == null)
            {
                return BadRequest("Brak danych.");
            }

            if(string.IsNullOrWhiteSpace(l_user.Email) || string.IsNullOrWhiteSpace(l_user.Password))
            {
                return BadRequest("Email i hasło jest wymagane do logowania.");
            }

            User? user = await _context.Users.FirstOrDefaultAsync((u) => u.Email == l_user.Email);

            if(user == null)
            {
                return Unauthorized("Nieprawidłowy email lub hasło.");
            }
            
            PasswordVerificationResult verifyPassword = _passwordHasher.VerifyHashedPassword(user, user.HashPassword, l_user.Password);

            if(verifyPassword == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Nieprawidłowy email lub hasło.");
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("name", user.Name),
                new Claim("surname", user.Surname),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            IConfigurationSection? jwtSection = _config.GetSection("Jwt");
            string? key = jwtSection.GetValue<string>("Key");
            string? issuer = jwtSection.GetValue<string>("Issuer");
            string? audience = jwtSection.GetValue<string>("Audience");
            double expiresMinutes = jwtSection.GetValue<int>("ExpiresMinutes");

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtToken = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: credentials
            );

            string? tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            string? jti = jwtToken.Claims.First((c) => c.Type == JwtRegisteredClaimNames.Jti).Value;

            Token? existingToken = await _context.Tokens.FirstOrDefaultAsync((t) => t.Email == user.Email);

            if(existingToken == null)
            {
                existingToken = new Token
                {
                    Email = user.Email,
                    Value = jti,
                    ExpiresAt = jwtToken.ValidTo
                };
                _context.Tokens.Add(existingToken);
            }
            else
            {
                existingToken.Value = jti;
                existingToken.ExpiresAt = jwtToken.ValidTo;
                _context.Tokens.Update(existingToken);
            }

            await _context.SaveChangesAsync();

            CookieOptions? cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = jwtToken.ValidTo
            };

            Response.Cookies.Append("authToken",tokenString,cookieOptions);

            return Ok( tokenString );
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            string? userEmail = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("Nieautoryzowany dostęp.");
            }
            Token? existingToken = await _context.Tokens.FirstOrDefaultAsync((t) => t.Email == userEmail && !t.IsDeleted);
            if (existingToken != null)
            {
                _context.Tokens.Remove(existingToken);
                await _context.SaveChangesAsync();
            }
            Response.Cookies.Delete("authToken");
            return Ok("Wylogowano pomyślnie.");
        }

        [HttpDelete("deleteAccount")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        {
            string? userEmail = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("Nieautoryzowany dostęp.");
            }
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                return NotFound("Użytkownik nie istnieje.");
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            Response.Cookies.Delete("authToken");

            return Ok("Konto zostało usunięte pomyślnie.");
        }

        [HttpPut("edit_user")]
        [Authorize]
        public async Task<IActionResult> Edituser(UserUpdateRequest u_user)
        {
            string? userEmail = User.Claims.FirstOrDefault((e) => e.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("Nieautoryzowany dostęp.");
            }

            User? user = await _context.Users.FirstOrDefaultAsync((u) => u.Email == userEmail);
            if(user == null)
            {
                return NotFound("Użytkownik nie istnieje.");
            }

            if(string.IsNullOrEmpty(u_user.Name) || string.IsNullOrEmpty(u_user.Surname) || string.IsNullOrEmpty(u_user.Login))
            {
                return BadRequest("Imie, Nazwisko i Login są wymagane.");
            }

            user.Name = u_user.Name;
            user.Surname = u_user.Surname;
            user.Login = u_user.Login;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Ok("Pomyślnie zaktualizowano dane użytkownika.");
        }

        [HttpGet("get_user_logged_data")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            string? userEmail = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized("Nieautoryzowany dostęp.");
            }

            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                return NotFound("Użytkownik nie istnieje.");
            }

            UserResponse? response = new UserResponse
            {
                Email = user.Email,
                Login = user.Login,
                Name = user.Name,
                Surname = user.Surname
            };

            return Ok(response);
        }
    }
}
