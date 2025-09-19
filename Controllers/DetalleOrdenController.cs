using ExGradoBack.Models;
using ExGradoBack.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExGradoBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DetalleOrdenController : ControllerBase
    {
        private readonly IDetalleOrdenService _detalleOrdenService;
        public DetalleOrdenController(IDetalleOrdenService detalleOrdenService)
        {
            _detalleOrdenService = detalleOrdenService;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<DetalleOrdenCompra>> GetDetalleById(int id)
        {
            try
            {
                var detalle = await _detalleOrdenService.GetDetalleOrdenByIdOrdenAsync(id);
                return Ok(detalle);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }
        [HttpPost]
        public async Task<ActionResult<DetalleOrdenCompra>> CreateDetalle([FromBody] DetalleOrdenCompra detalle)
        {
            try
            {
                var nuevoDetalle = await _detalleOrdenService.CreateDetalleOrdenAsync(detalle);
                return CreatedAtAction(nameof(GetDetalleById), new { id = nuevoDetalle.Id }, nuevoDetalle);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
        [HttpPut]
        public async Task<ActionResult<DetalleOrdenCompra>> UpdateDetalle(int id, [FromBody] DetalleOrdenCompra detalle)
        {
            if (id != detalle.Id)
                return BadRequest(new { mensaje = "El id de la URL no coincide con el de los detalles de la factura" });

            try
            {
                var detalleActualizado = await _detalleOrdenService.UpdateDetalleOrdenAsync(detalle);
                return Ok(detalleActualizado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetalle(int id)
        {
            try
            {
                var eliminada = await _detalleOrdenService.DeleteDetalleOrdenAsync(id);
                if (!eliminada)
                    return NotFound(new { mensaje = "No se puede eliminar los detalles de esta factura." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}