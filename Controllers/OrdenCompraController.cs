using ExGradoBack.Models;
using ExGradoBack.Services;
using ExGradoBack.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ExGradoBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdenCompraController : ControllerBase
    {
        private readonly IOrdenCompraService _ordenService;

        public OrdenCompraController(IOrdenCompraService ordenService)
        {
            _ordenService = ordenService;
        }

        [HttpGet("year")]
        public async Task<ActionResult<IEnumerable<int>>> GetYears()
        {
            try
            {
                var anios = await _ordenService.GetAniosConOrdenesAsync();
                return Ok(anios);
            }
            catch (Exception ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        [HttpGet("month/{year}")]
        public async Task<ActionResult<IEnumerable<int>>> GetMonths(int year)
        {
            try
            {
                var meses = await _ordenService.GetMesesConOrdenesAsync(year);
                return Ok(meses);
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

        [HttpGet("days/{year}/{month}")]
        public async Task<ActionResult<IEnumerable<int>>> GetDays(int year, int month)
        {
            try
            {
                var dias = await _ordenService.GetDiasConOrdenesAsync(year, month);
                return Ok(dias);
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

        [HttpGet("orders/{year}/{month}/{day}")]
        public async Task<ActionResult<IEnumerable<OrdenCompra>>> GetOrders(int year, int month, int day)
        {
            try
            {
                var ordenes = await _ordenService.GetOrdenesPorDiaAsync(year, month, day);
                return Ok(ordenes);
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

        [HttpGet("{id}")]
        public async Task<ActionResult<OrdenCompra>> GetOrderById(int id)
        {
            try
            {
                var orden = await _ordenService.GetOrdenByIdAsync(id);
                if (orden == null)
                    return NotFound(new { mensaje = "Orden de compra no encontrada." });
                return Ok(orden);
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
        public async Task<ActionResult<OrdenCompra>> CreateOrder(OrdenCompra orden)
        {
            try
            {
                var nuevaOrden = await _ordenService.CreateOrdenAsync(orden);
                return CreatedAtAction(nameof(GetOrderById), new { id = nuevaOrden.Id }, nuevaOrden);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<OrdenCompra>> UpdateOrder(int id, OrdenCompra orden)
        {
            try
            {
                if (id != orden.Id)
                    return BadRequest(new { mensaje = "El ID de la orden no coincide." });

                var ordenActualizada = await _ordenService.UpdateOrdenAsync(orden);
                return Ok(ordenActualizada);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            try
            {
                var resultado = await _ordenService.DeleteOrdenAsync(id);
                if (!resultado)
                    return NotFound(new { mensaje = "Orden de compra no encontrada." });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
        [HttpGet("cantidad_estados")]
        public async Task<ActionResult<OrdenCompraResumenDTO?>> GetResumenOrdenesPorFecha([FromQuery] DateTime fecha)
        {
            try
            {
                var resumen = await _ordenService.GetResumenOrdenesPorFechaAsync(fecha);
                if (resumen == null)
                    return NotFound(new { mensaje = "No se encontraron órdenes para la fecha especificada." });

                return Ok(resumen);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}