using ExGradoBack.Models;
using ExGradoBack.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExGradoBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturaController : ControllerBase
    {
        private readonly IFacturaService _facturaService;

        public FacturaController(IFacturaService facturaService)
        {
            _facturaService = facturaService;
        }

        [HttpGet("year")]
        public async Task<ActionResult<IEnumerable<int>>> GetYears()
        {
            try
            {
                var anios = await _facturaService.GetAniosConFacturasAsync();
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
                var meses = await _facturaService.GetMesesConFacturasAsync(year);
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
                var dias = await _facturaService.GetDiasConFacturasAsync(year, month);
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
        [HttpGet("{year}/{month}/{day}")]
        public async Task<ActionResult<IEnumerable<Factura>>> GetFacturasPorDia(int year, int month, int day)
        {
            try
            {
                var facturas = await _facturaService.GetFacturasPorDiaAsync(year, month, day);
                return Ok(facturas);
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
        public async Task<ActionResult<Factura>> GetFactura(int id)
        {
            try
            {
                var factura = await _facturaService.GetFacturaByIdAsync(id);
                return Ok(factura);
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
        public async Task<ActionResult<Factura>> CreateFactura([FromBody] Factura factura)
        {
            try
            {
                var nuevaFactura = await _facturaService.CreateFacturaAsync(factura);
                return CreatedAtAction(nameof(GetFactura), new { id = nuevaFactura.Id }, nuevaFactura);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Factura>> UpdateFactura(int id, [FromBody] Factura factura)
        {
            if (id != factura.Id)
                return BadRequest(new { mensaje = "El ID de la URL no coincide con el de la factura." });

            try
            {
                var facturaActualizada = await _facturaService.UpdateFacturaAsync(factura);
                return Ok(facturaActualizada);
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
        public async Task<IActionResult> DeleteFactura(int id)
        {
            try
            {
                var eliminada = await _facturaService.DeleteFacturaAsync(id);
                if (!eliminada)
                    return NotFound(new { mensaje = "No se pudo eliminar la factura." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
        [HttpGet("mejores-vendedores")]
        public async Task<IActionResult> ObtenerTop3Vendedores()
        {
            var topVendedores = await _facturaService.ObtenerTop3VendedoresAsync();
            return Ok(topVendedores.Select(v => new { v.Vendedor, v.TotalVendidos }));
        }
        [HttpGet("ventas-por-mes")]
        public async Task<IActionResult> ObtenerTotalVentasPorMes([FromQuery] int year)
        {
            if (year < 1)
                return BadRequest("Año inválido.");

            var resultados = await _facturaService.ObtenerTotalVentasPorMesAsync(year);

            var response = resultados.Select(r => new
            {
                Mes = r.Mes,
                TotalVentas = r.TotalVentas
            });

            return Ok(response);
        }
        [HttpGet("Actividad_Compras")]
        public async Task<IActionResult> ObtenerFacturasPorBloqueAsync()
        {
            var resultado = await _facturaService.ObtenerFacturasPorBloqueAsync();
            return Ok(resultado);
        }
        [HttpGet("Facturas_ultimaSemana")]
        public async Task<IActionResult> GetFacturasPorDiaUltimaSemana()
        {
            var result = await _facturaService.GetCantidadFacturasPorDiaUltimaSemanaAsync();
            return Ok(result);
        }
        [HttpGet("Repuestos_ultimaSemana")]
        public async Task<IActionResult> GetRepuestosPorDiaUltimaSemana()
        {
            var result = await _facturaService.GetRepuestosVendidosPorDiaUltimaSemanaAsync();
            return Ok(result);
        }
    }
}