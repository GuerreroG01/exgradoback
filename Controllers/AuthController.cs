using Microsoft.AspNetCore.Mvc;
using ExGradoBack.Services;
using ExGradoBack.Repositories;
using ExGradoBack.Models;
using ExGradoBack.DTOs;

namespace ExGradoBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthService authService, IAuthRepository authRepository)
        {
            _authService = authService;
            _authRepository = authRepository;
        }

        // GET: api/auth
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Auth>>> GetAll()
        {
            var users = await _authService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/auth/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Auth>> GetById(int id)
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<Auth>> Register([FromBody] RegisterDto dto)
        {
            var exists = await _authService.UserExistsAsync(dto.Username);
            if (exists) return BadRequest("El nombre de usuario ya existe.");

            var createdUser = await _authService.RegisterAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var token = await _authService.LoginAsync(loginDto.Username, loginDto.Password);
            if (token == null)
                return Unauthorized("Credenciales inválidas.");

            var refreshToken = _authService.GenerateRefreshToken();

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(7),
                Path = "/"
            });

            return Ok(new { token = token.AccessToken });
        }

        // PUT: api/auth/5
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> UpdateUser(int id, [FromBody] Auth updatedUser)
        {
            if (id != updatedUser.Id)
                return BadRequest("ID no coincide.");

            try
            {
                var user = await _authService.UpdateUserAsync(updatedUser);

                var newToken = _authService.GenerateJwtToken(user);

                return Ok(new { token = newToken });
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar usuario: {ex.Message}");
            }
        }

        // DELETE: api/auth/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _authService.DeleteUserAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }
        [HttpGet("find-username")]
        public async Task<ActionResult<List<Auth>>> GetUserByUsername([FromQuery] string username)
        {
            try
            {
                var users = await _authService.GetUserByUsernameAsync(username);
                return Ok(users);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [HttpGet("totalUsers")]
        public async Task<IActionResult> GetTotalUsers()
        {
            int totalUsers = await _authService.GetTotalUsersAsync();
            return Ok(totalUsers);
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest request)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("Refresh token no encontrado");

            var principal = _authService.GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
                return Unauthorized("Token inválido");

            var userId = int.Parse(principal.FindFirst("id")?.Value ?? "0");
            var refreshTokenDb = await _authRepository.GetRefreshTokenAsync(userId, refreshToken);

            if (refreshTokenDb == null || refreshTokenDb.Expiration < DateTime.UtcNow)
                return Unauthorized("Refresh token inválido o expirado");

            var user = await _authRepository.GetByIdAsync(userId);
            if (user == null) return Unauthorized();

            var newAccessToken = _authService.GenerateJwtToken(user);
            var newRefreshToken = _authService.GenerateRefreshToken();

            await _authRepository.SaveRefreshTokenAsync(userId, newRefreshToken);

            Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new { token = newAccessToken });
        }
        [HttpGet("FindUsers")]
        public async Task<ActionResult<List<UserDto>>> GetUsersByUsername([FromQuery] string username)
        {
            try
            {
                var users = await _authRepository.GetUserByUsernameAsync(username);

                if (users == null || users.Count == 0)
                {
                    return NotFound($"No se encontraron usuarios con username que contenga '{username}'.");
                }

                return Ok(users);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}