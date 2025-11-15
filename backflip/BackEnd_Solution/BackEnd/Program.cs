using BackEnd.Data;
using BackEnd.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
namespace BackEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();


            builder.Services.AddControllers();

            // CORS: pozwól po³¹czenia z frontendu na http://localhost:2137
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocal2137", policy =>
                {
                    policy.WithOrigins("http://localhost:2137")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            IConfigurationSection jwtSection = builder.Configuration.GetSection("Jwt");
            string? jwtKey = jwtSection.GetValue<string>("Key");
            string? jwtIssuer = jwtSection.GetValue<string>("Issuer");
            string? jwtAudience = jwtSection.GetValue<string>("Audience");

            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new Exception("Brak klucza JWT w konfiguracji (Jwt:Key).");
            }

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies["authToken"];
                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization();


            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            }

            app.UseHttpsRedirection();

            // W³¹cz CORS (u¿yj nazwy polityki)
            app.UseCors("AllowLocal2137");
            
            app.UseAuthentication();
            
            app.UseAuthorization();
            
            app.MapControllers();

            app.Run();
        }
    }
}
