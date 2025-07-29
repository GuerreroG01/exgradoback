using Microsoft.AspNetCore.Mvc;
using ExGradoBack.Models;
using ExGradoBack.Services;

namespace ExGradoBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProfiles()
        {
            var profiles = await _profileService.GetAllProfilesAsync();
            return Ok(profiles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfileById(int id)
        {
            var profile = await _profileService.GetProfileByIdAsync(id);
            if (profile == null)
                return NotFound($"No se encontró un perfil con ID {id}.");

            return Ok(profile);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProfile([FromForm] InfoUser profile, IFormFile? photoFile)
        {
            try
            {
                var createdProfile = await _profileService.CreateProfileAsync(profile, photoFile);
                return CreatedAtAction(nameof(GetProfileById), new { id = createdProfile.Id }, createdProfile);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocurrió un error al crear el perfil.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromForm] InfoUser profile, IFormFile? photoFile)
        {
            if (id != profile.Id)
                return BadRequest("El ID del perfil no coincide con el parámetro de la URL.");

            try
            {
                var updatedProfile = await _profileService.UpdateProfileAsync(profile, photoFile);
                return Ok(updatedProfile);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocurrió un error al actualizar el perfil.");
            }
        }
    }
}