using BackEnd.Data;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services
{
    public class PhotoService
    {
        private readonly AppDbContext _db;
        private readonly FileStorageService _storage;

        public PhotoService(AppDbContext db, FileStorageService storage)
        {
            _db = db;
            _storage = storage;
        }

        public async Task<PhotoListResponse> ListUserPhotosAsync(string userEmail)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return new PhotoListResponse { Photos = Array.Empty<PhotoResponse>() };

            var photos = await _db.Photos
                .Where(p => p.OwnerId == user.Id)
                .OrderByDescending(p => p.UploadedAt)
                .ToListAsync();

            return new PhotoListResponse
            {
                Photos = photos.Select(p => new PhotoResponse
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    FilePath = p.FilePath,
                    UploadedAt = p.UploadedAt,
                    OwnerId = p.OwnerId
                }).ToArray()
            };
        }

        public async Task<PhotoResponse?> GetPhotoAsync(int id, string requestorEmail)
        {
            var photo = await _db.Photos.FindAsync(id);
            if (photo == null) return null;

            // prosty check: jeśli właściciel lub zdjęcie zostało udostępnione - można rozszerzyć
            var owner = await _db.Users.FindAsync(photo.OwnerId);
            if (owner == null) return null;
            if (owner.Email != requestorEmail)
            {
                var shared = await _db.PhotoShares.AnyAsync(ps => ps.PhotoId == id && ps.SharedWithUser.Email == requestorEmail);
                if (!shared) return null;
            }

            return new PhotoResponse
            {
                Id = photo.Id,
                Title = photo.Title,
                Description = photo.Description,
                FilePath = photo.FilePath,
                UploadedAt = photo.UploadedAt,
                OwnerId = photo.OwnerId
            };
        }

        public async Task<(Stream Stream, string ContentType)?> GetPhotoFileAsync(int id)
        {
            var photo = await _db.Photos.FindAsync(id);
            if (photo == null) return null;
            var s = _storage.GetPhotoStream(photo.FilePath);
            return s;
        }

        public async Task<PhotoResponse> CreatePhotoAsync(string ownerEmail, PhotoCreateRequest req)
        {
            var owner = await _db.Users.FirstOrDefaultAsync(u => u.Email == ownerEmail);
            if (owner == null) throw new InvalidOperationException("Owner not found");

            var relative = await _storage.SavePhotoAsync(req.File!, owner.Id);
            var photo = new Photo
            {
                Title = req.Title ?? string.Empty,
                Description = req.Description,
                FilePath = relative,
                OwnerId = owner.Id,
                UploadedAt = DateTime.UtcNow
            };
            _db.Photos.Add(photo);
            await _db.SaveChangesAsync();

            return new PhotoResponse
            {
                Id = photo.Id,
                Title = photo.Title,
                Description = photo.Description,
                FilePath = photo.FilePath,
                UploadedAt = photo.UploadedAt,
                OwnerId = photo.OwnerId
            };
        }

        public async Task<bool> UpdatePhotoAsync(int id, string requesterEmail, PhotoUpdateRequest req)
        {
            var photo = await _db.Photos.FindAsync(id);
            if (photo == null) return false;
            var owner = await _db.Users.FindAsync(photo.OwnerId);
            if (owner == null || owner.Email != requesterEmail) return false;

            if (!string.IsNullOrEmpty(req.Title)) photo.Title = req.Title;
            photo.Description = req.Description;
            _db.Photos.Update(photo);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePhotoAsync(int id, string requesterEmail)
        {
            var photo = await _db.Photos.FindAsync(id);
            if (photo == null) return false;
            var owner = await _db.Users.FindAsync(photo.OwnerId);
            if (owner == null || owner.Email != requesterEmail) return false;

            var deletedFile = await _storage.DeletePhotoAsync(photo.FilePath);
            _db.Photos.Remove(photo);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SharePhotoAsync(int id, string requesterEmail, PhotoShareRequest req)
        {
            if (string.IsNullOrWhiteSpace(req?.Email)) return false;
            var photo = await _db.Photos.FindAsync(id);
            if (photo == null) return false;
            var owner = await _db.Users.FindAsync(photo.OwnerId);
            if (owner == null || owner.Email != requesterEmail) return false;

            var sharedWith = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
            if (sharedWith == null) return false;

            var exists = await _db.PhotoShares.AnyAsync(ps => ps.PhotoId == id && ps.SharedWithUserId == sharedWith.Id);
            if (exists) return true;

            var share = new PhotoShare
            {
                PhotoId = id,
                SharedWithUserId = sharedWith.Id,
                SharedAt = DateTime.UtcNow
            };
            _db.PhotoShares.Add(share);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}