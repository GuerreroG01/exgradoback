using ExGradoBack.Models;
using ExGradoBack.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExGradoBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolController : ControllerBase
    {
        private readonly IRolService _rolService;

        public RolController(IRolService rolService)
        {
            _rolService = rolService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _rolService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rol = await _rolService.GetRoleByIdAsync(id);
            if (rol == null) return NotFound();
            return Ok(rol);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Rol rol)
        {
            if (await _rolService.RoleExistsAsync(rol.Nombre))
                return Conflict("Ya existe un rol con ese nombre.");

            var nuevoRol = await _rolService.CreateRoleAsync(rol);
            return CreatedAtAction(nameof(GetById), new { id = nuevoRol.Id }, nuevoRol);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _rolService.DeleteRoleAsync(id);
            if (!eliminado) return NotFound();
            return NoContent();
        }
    }
}