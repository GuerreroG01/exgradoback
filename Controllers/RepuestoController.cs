using ExGradoBack.Models;
using ExGradoBack.Services;
using Microsoft.AspNetCore.Mvc;
namespace ExGradoBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepuestoController : ControllerBase
    {
        private readonly IRepuestoService _repuestoService;

        public RepuestoController(IRepuestoService repuestoService)
        {
            _repuestoService = repuestoService;
        }
        [HttpGet]
        public async Task<IActionResult> GetByName(string nombre)
        {
            var repuestos = await _repuestoService.GetRepuestosByNameAsync(nombre);
            return Ok(repuestos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var repuesto = await _repuestoService.GetRepuestoByIdAsync(id);
            if (repuesto == null) return NotFound();
            return Ok(repuesto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Repuesto repuesto)
        {
            var nuevoRepuesto = await _repuestoService.CreateRepuestoAsync(repuesto);
            return CreatedAtAction(nameof(GetById), new { id = nuevoRepuesto.Id }, nuevoRepuesto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Repuesto repuesto)
        {
            if (id != repuesto.Id) return BadRequest("El ID del repuesto no coincide.");
            var actualizado = await _repuestoService.UpdateRepuestoAsync(repuesto);
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _repuestoService.DeleteRepuestoAsync(id);
            if (!eliminado) return NotFound();
            return NoContent();
        }
    }
}