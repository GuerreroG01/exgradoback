using ExGradoBack.Models;
using ExGradoBack.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace ExGradoBack.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MarcaRepuestoController : ControllerBase
    {
        private readonly IMarcaRepuestoService _marcaRepuestoService;

        public MarcaRepuestoController(IMarcaRepuestoService marcaRepuestoService)
        {
            _marcaRepuestoService = marcaRepuestoService;
        }
        [HttpGet]
        public async Task<IActionResult> GetByCalificacion(double calificacion)
        {
            try
            {
                var marcas = await _marcaRepuestoService.GetMarcaRepuestoPorCalificacionAsync(calificacion);
                return Ok(marcas);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [HttpGet("byname")]
        public async Task<IActionResult> GetByName([FromQuery] string nombre)
        {
            var marca = await _marcaRepuestoService.GetByNameAsync(nombre);
            if (marca == null)
            {
                return NotFound($"No se encontró una marca con el nombre '{nombre}'.");
            }

            return Ok(marca);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var marcaRepuesto = await _marcaRepuestoService.GetMarcaRepuestoByIdAsync(id);
            if (marcaRepuesto == null) return NotFound();
            return Ok(marcaRepuesto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MarcaRepuesto marcaRepuesto)
        {
            var nuevaMarca = await _marcaRepuestoService.CreateMarcaRepuestoAsync(marcaRepuesto);
            return CreatedAtAction(nameof(GetById), new { id = nuevaMarca.Id }, nuevaMarca);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MarcaRepuesto marcaRepuesto)
        {
            if (id != marcaRepuesto.Id) return BadRequest("El ID de la marca no coincide.");
            var actualizado = await _marcaRepuestoService.UpdateMarcaRepuestoAsync(marcaRepuesto);
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var eliminado = await _marcaRepuestoService.DeleteMarcaRepuestoAsync(id);
                if (!eliminado)
                    return NotFound();

                return NoContent(); 
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }
        [HttpGet("mejores_marcas")]
        public async Task<IActionResult> ObtenerMarcasRepuestoMasUsadas()
        {
            var masVendidas = await _marcaRepuestoService.ObtenerMarcasRepuestoMasUsadasAsync();
            return Ok(masVendidas);
        }
    }
}