using ExGradoBack.Models;
using ExGradoBack.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExGradoBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DetalleFacturaController : ControllerBase
    {
        private readonly IDetalleFacturaService _detalleFacturaService;
        public DetalleFacturaController(IDetalleFacturaService detalleFacturaService)
        {
            _detalleFacturaService = detalleFacturaService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DetalleFactura>> GetDetalleById(int id)
        {
            try
            {
                var detalle = await _detalleFacturaService.GetDetalleFacturaByIdFacturaAsync(id);
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
        public async Task<ActionResult<DetalleFactura>> CreateDetalle([FromBody] DetalleFactura detalle)
        {
            try
            {
                var nuevoDetalle = await _detalleFacturaService.CreateDetalleFacturaAsync(detalle);
                return CreatedAtAction(nameof(GetDetalleById), new { id = nuevoDetalle.Id }, nuevoDetalle);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut]
        public async Task<ActionResult<DetalleFactura>> UpdateDetalle(int id, [FromBody] DetalleFactura detalle)
        {
            if (id != detalle.Id)
                return BadRequest(new { mensaje = "El id de la URL no coincide con el de los detalles de la factura" });

            try
            {
                var detalleActualizado = await _detalleFacturaService.UpdateDetalleFacturaAsync(detalle);
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
                var eliminada = await _detalleFacturaService.DeleteDetalleFacturaAsync(id);
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