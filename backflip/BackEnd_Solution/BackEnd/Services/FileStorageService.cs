using Microsoft.AspNetCore.Hosting;

namespace BackEnd.Services
{
    public class FileStorageService
    {
        private readonly IWebHostEnvironment _env;

        public FileStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }
        private static readonly string[] permittedExtensions = { ".jpg", ".jpeg", ".png"};

        public bool IsExtensionPermitted(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return permittedExtensions.Contains(ext);
        }

        public async Task<string> SavePhotoAsync(IFormFile file, int userId) { 
            var uploadsRoot = Path.Combine(_env.ContentRootPath, "uploads", userId.ToString());
            if (!Directory.Exists(uploadsRoot))
            {
                Directory.CreateDirectory(uploadsRoot);
            }
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsRoot, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var relativePath = Path.Combine("uploads", userId.ToString(), fileName).Replace("\\", "/");
            return relativePath;

        }
        public async Task<bool> DeletePhotoAsync(string relativePath)
        {
            var fullPath = Path.Combine(_env.ContentRootPath, relativePath);
            if (File.Exists(fullPath))
            {
                try
                {
                    await Task.Run(() => File.Delete(fullPath));
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        public (Stream? Stream, string ContentType)? GetPhotoStream(string relativePath)
        {
            var fullPath = Path.Combine(_env.ContentRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (!File.Exists(fullPath))
                return null;

            var ext = Path.GetExtension(fullPath).ToLowerInvariant();
            var contentType = ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return (stream, contentType);
        }
    }
}
