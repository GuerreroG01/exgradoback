using ExGradoBack.Models;
using ExGradoBack.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ExGradoBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiculoInfoController : ControllerBase
    {
        private readonly IVehiculoInfoService _vehiculoInfoService;
        private readonly ILogger<VehiculoInfoController> _logger;

        public VehiculoInfoController(IVehiculoInfoService vehiculoInfoService, ILogger<VehiculoInfoController> logger)
        {
            _vehiculoInfoService = vehiculoInfoService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetByMarcaAndAnio([FromQuery] string? marca, [FromQuery] int? anio)
        {
            try
            {
                var vehiculos = await _vehiculoInfoService.GetVehiculoInfosByMarcaAndAnioAsync(marca, anio);
                return Ok(vehiculos);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var vehiculo = await _vehiculoInfoService.GetVehiculoInfoByIdAsync(id);
            if (vehiculo == null) return NotFound();
            return Ok(vehiculo);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] VehiculoInfo vehiculoInfo, IFormFile? fotoReferencia)
        {
            try
            {
                var nuevoVehiculo = await _vehiculoInfoService.CreateVehiculoInfoAsync(vehiculoInfo, fotoReferencia);
                return CreatedAtAction(nameof(GetById), new { id = nuevoVehiculo.Id }, nuevoVehiculo);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
        #if DEBUG
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
        #else
                return StatusCode(500, new { message = "Error interno del servidor." });
        #endif
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] VehiculoInfo vehiculoInfo, IFormFile? fotoReferencia)
        {
            _logger.LogInformation("Id obtenido: {Id}", vehiculoInfo.Id);
            _logger.LogInformation("Id a comparar: {Id}", id);
            if (id != vehiculoInfo.Id)
            {
                return BadRequest("El ID del vehículo no coincide.");
            }

            var actualizado = await _vehiculoInfoService.UpdateVehiculoInfoAsync(vehiculoInfo, fotoReferencia);
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _vehiculoInfoService.DeleteVehiculoInfoAsync(id);
            if (!eliminado) return NotFound();
            return NoContent();
        }
        [HttpGet("anios_by_marca")]
        public async Task<IActionResult> GetAniosByMarca([FromQuery] string marca)
        {
            if (string.IsNullOrWhiteSpace(marca))
                return BadRequest(new { message = "La marca es obligatoria." });

            try
            {
                var anios = await _vehiculoInfoService.GetAniosVehiculosAsync(marca);
                return Ok(anios);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error inesperado.", detalle = ex.Message });
            }
        }
        [HttpGet("marcas")]
        public async Task<IActionResult> GetMarcas()
        {
            try
            {
                var marcas = await _vehiculoInfoService.GetMarcasVehiculosAsync();
                return Ok(marcas);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error inesperado.", detalle = ex.Message });
            }
        }
    }
}