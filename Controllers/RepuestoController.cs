using ExGradoBack.Models;
using ExGradoBack.Services;
using ExGradoBack.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
namespace ExGradoBack.Controllers
{
    //[Authorize]
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
        [HttpGet("ubicacion/{ubicacion}")]
        public async Task<IActionResult> GetByUbicacion(string ubicacion)
        {
            var repuestos = await _repuestoService.GetRepuestosByUbicacionAsync(ubicacion);
            return Ok(repuestos);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RepuestoDto dto)
        {
            try
            {
                var nuevoRepuesto = await _repuestoService.CreateRepuestoAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = nuevoRepuesto.Id }, nuevoRepuesto);
            }
            catch (ValidationException valEx)
            {
                return BadRequest(new
                {
                    type = "ValidationError",
                    message = valEx.Message
                });
            }
            catch (KeyNotFoundException knfEx)
            {
                return NotFound(new
                {
                    type = "NotFound",
                    message = knfEx.Message
                });
            }
            catch (Exception ex)
            {
            #if DEBUG
                return StatusCode(500, new { type = "ServerError", message = ex.Message, stackTrace = ex.StackTrace });
            #else
                return StatusCode(500, new { type = "ServerError", message = "Error interno del servidor" });
            #endif
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRepuesto(int id, [FromBody] RepuestoDto dto)
        {
            try
            {
                var repuestoActualizado = await _repuestoService.UpdateRepuestoAsync(id, dto);
                return Ok(repuestoActualizado);
            }
            catch (ValidationException valEx)
            {
                return BadRequest(new
                {
                    type = "ValidationError",
                    message = valEx.Message
                });
            }
            catch (KeyNotFoundException knfEx)
            {
                return NotFound(new
                {
                    type = "NotFound",
                    message = knfEx.Message
                });
            }
            catch (Exception ex)
            {
            #if DEBUG
                return StatusCode(500, new { type = "ServerError", message = ex.Message, stackTrace = ex.StackTrace });
            #else
                return StatusCode(500, new { type = "ServerError", message = "Error interno del servidor" });
            #endif
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _repuestoService.DeleteRepuestoAsync(id);
            if (!eliminado) return NotFound();
            return NoContent();
        }
        [HttpGet("ubicaciones")]
        public async Task<ActionResult<IEnumerable<string>>> GetUbicaciones()
        {
            var ubicaciones = await _repuestoService.GetAllUbicacionesAsync();
            return Ok(ubicaciones);
        }

        [HttpGet("top-stock")]
        public async Task<IActionResult> GetTop10RepuestosMayorStock()
        {
            try
            {
                var repuestos = await _repuestoService.GetTop10RepuestosMayorStockAsync();
                return Ok(repuestos);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetTop10RepuestosMenorStock()
        {
            try
            {
                var repuestos = await _repuestoService.GetTop10RepuestosMenorStockAsync();
                return Ok(repuestos);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("sin-stock")]
        public async Task<IActionResult> GetRepuestosSinStock()
        {
            try
            {
                var repuestos = await _repuestoService.GetRepuestosSinStockAsync();
                return Ok(repuestos);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}