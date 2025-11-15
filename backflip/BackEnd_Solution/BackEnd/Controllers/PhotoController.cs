using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotoController : ControllerBase
    {
        private readonly PhotoService _photoService;

        public PhotoController(PhotoService photoService)
        {
            _photoService = photoService;
        }

        // Pobierz listę zdjęć zalogowanego użytkownika
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<PhotoListResponse>> GetMyPhotos()
        {
            var email = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var list = await _photoService.ListUserPhotosAsync(email);
            return Ok(list);
        }

        // Pobierz metadane zdjęcia
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<PhotoResponse>> GetPhoto(int id)
        {
            var email = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var photo = await _photoService.GetPhotoAsync(id, email);
            if (photo == null) return NotFound();
            return Ok(photo);
        }

        // Pobierz plik zdjęcia (stream)
        [HttpGet("{id:int}/file")]
        public async Task<IActionResult> GetPhotoFile(int id)
        {
            var streamInfo = await _photoService.GetPhotoFileAsync(id);
            if (streamInfo == null) return NotFound();
            return File(streamInfo.Value.Stream, streamInfo.Value.ContentType);
        }

        // Upload zdjęcia (multipart/form-data)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Upload([FromForm] PhotoCreateRequest req)
        {
            var email = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            if (req.File == null) return BadRequest("Brak pliku.");
            var result = await _photoService.CreatePhotoAsync(email, req);
            return CreatedAtAction(nameof(GetPhoto), new { id = result.Id }, result);
        }

        // Aktualizuj metadane
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, PhotoUpdateRequest req)
        {
            var email = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var ok = await _photoService.UpdatePhotoAsync(id, email, req);
            if (!ok) return NotFound();
            return NoContent();
        }

        // Usuń zdjęcie
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var email = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var ok = await _photoService.DeletePhotoAsync(id, email);
            if (!ok) return NotFound();
            return NoContent();
        }

        // Udostępnij zdjęcie innemu użytkownikowi (po email)
        [HttpPost("{id:int}/share")]
        [Authorize]
        public async Task<IActionResult> Share(int id, PhotoShareRequest req)
        {
            var email = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var ok = await _photoService.SharePhotoAsync(id, email, req);
            if (!ok) return BadRequest("Nie można udostępnić.");
            return NoContent();
        }
    }
}