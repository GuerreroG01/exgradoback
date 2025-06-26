using Microsoft.AspNetCore.Mvc;
using ExGradoBack.Services;
using ExGradoBack.Models;
using ExGradoBack.DTOs;

namespace ExGradoBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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
        public async Task<ActionResult<object>> Login([FromBody] LoginDto loginDto)
        {
            var token = await _authService.LoginAsync(loginDto.Username, loginDto.Password);
            if (token == null)
                return Unauthorized("Credenciales inválidas.");

            return Ok(new { token });
        }

        // PUT: api/auth/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Auth>> Update(int id, [FromBody] Auth updatedUser)
        {
            if (id != updatedUser.Id)
                return BadRequest("El ID no coincide.");

            var result = await _authService.UpdateUserAsync(updatedUser);
            return Ok(result);
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
        public async Task<ActionResult<Auth>> GetUserByUsername([FromQuery] string username)
        {
            try
            {
                var user = await _authService.GetUserByUsernameAsync(username);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}