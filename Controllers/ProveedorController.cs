using ExGradoBack.Models;
using ExGradoBack.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace ExGradoBack.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProveedorController : ControllerBase
    {
        private readonly IProveedorService _proveedorService;
        private readonly ILogger<ProveedorController> _logger;
        public ProveedorController(IProveedorService proveedorService, ILogger<ProveedorController> logger)
        {
            _proveedorService = proveedorService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetInfoProv([FromQuery] string? country, [FromQuery] string? city, [FromQuery] bool isMinInfo = false)
        {
            _logger.LogInformation("Datos obtenidos para proveedores - Country: {Country}, City: {City}, IsMinInfo: {IsMinInfo}", country, city, isMinInfo);
            try
            {
                if (isMinInfo)
                {
                    var proveedoresMin = await _proveedorService.GetProveedorInfoMinAsync(country, city);
                    return Ok(proveedoresMin);
                }
                else
                {
                    var proveedoresFull = await _proveedorService.GetProveedorInfoFullAsync(country, city);
                    return Ok(proveedoresFull);
                }
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
            var proveedor = await _proveedorService.GetProveedorByIdAsync(id);
            if (proveedor == null) return NotFound();
            return Ok(proveedor);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Proveedor proveedor)
        {
            try
            {
                var nuevoProveedor = await _proveedorService.CreateProveedorAsync(proveedor);
                return CreatedAtAction(nameof(GetById), new { id = nuevoProveedor.Id }, nuevoProveedor);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detalle = ex.Message });
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Proveedor proveedor)
        {
            if (id != proveedor.Id)
            {
                return BadRequest(new { message = "El ID del proveedor no coincide con el ID de la ruta." });
            }

            try
            {
                var updatedProveedor = await _proveedorService.UpdateProveedorAsync(proveedor);
                return Ok(updatedProveedor);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detalle = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var eliminado = await _proveedorService.DeleteProveedorAsync(id);
                if (eliminado)
                    return NoContent();

                return NotFound();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar el proveedor con ID {id}", id);
                return StatusCode(500, "Error interno del servidor.");
            }
        }
        [HttpGet("Countries")]
        public async Task<IActionResult> GetCountryProv()
        {
            try
            {
                var countries = await _proveedorService.GetCountryProvAsync();
                return Ok(countries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detalle = ex.Message });
            }
        }
        [HttpGet("City")]
        public async Task<IActionResult> GetCityProv([FromQuery] string country)
        {
            if (string.IsNullOrWhiteSpace(country))
            {
                return BadRequest(new { message = "El país es obligatorio." });
            }

            try
            {
                var cities = await _proveedorService.GetCityProvAsync(country);
                return Ok(cities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detalle = ex.Message });
            }
        }
        [HttpGet("byName/{nombre}")]
        public async Task<IActionResult> GetByNameProveedores([FromRoute] string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return BadRequest(new { message = "El nombre es obligatorio." });
            }

            try
            {
                var proveedores = await _proveedorService.AutocompletarProveedoresAsync(nombre);
                return Ok(proveedores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", detalle = ex.Message });
            }
        }
    }
}