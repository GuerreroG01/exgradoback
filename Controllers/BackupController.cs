using ExGradoBack.Models;
using ExGradoBack.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace ExGradoBack.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BackupController : ControllerBase
    {
        private readonly IBackupService _backupService;
        private readonly ILogger _logger;
        public BackupController(IBackupService backupService, ILogger<BackupController> logger)
        {
            _backupService = backupService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<ActionResult<Backup?>> GetAllConfig()
        {
            var backup = await _backupService.GetBackupConfigAsync();
            return Ok(backup);
        }
        [HttpPost]
        public async Task<IActionResult> CreateConfig([FromBody] Backup backup)
        {
            var existingBackup = await _backupService.GetBackupConfigAsync();
            if (existingBackup != null)
                return Conflict("Ya existe una configuración de respaldo.");
            await _backupService.AddBackupConfigAsync(backup);
            return Ok();
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Backup>> UpdateConfig(int id, [FromBody] Backup backup)
        {
            await _backupService.UpdateBackupConfigAsync(id, backup);
            return Ok();
        }
        [HttpGet("descargar")]
        public async Task<IActionResult> DownloadBackup()
        {
            var result = await _backupService.CreateBackupAsync();

            if (!result.Success)
                return StatusCode(500, new { message = result.Message });

            Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{result.FileName}\"");

            return File(result.BackupBytes, "application/octet-stream");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("restaurar")]
        public async Task<IActionResult> RestaurarBackup([FromForm] IFormFile backupFile)
        {
            string? tempFilePath = null;

            try
            {
                if (backupFile == null)
                {
                    _logger.LogError("No se recibió el archivo de respaldo: backupFile es null.");
                    return BadRequest("No se recibió el archivo de respaldo.");
                }
                if (backupFile.Length == 0)
                {
                    _logger.LogError("Archivo recibido está vacío (length=0).");
                    return BadRequest("El archivo de respaldo está vacío.");
                }

                var tempDir = "/tmp";
                if (!Directory.Exists(tempDir))
                {
                    _logger.LogWarning($"/tmp no existe, usando directorio temporal predeterminado.");
                    tempDir = Path.GetTempPath();
                }
                tempFilePath = Path.Combine(tempDir, Path.GetRandomFileName());

                _logger.LogInformation($"Guardando archivo temporal en {tempFilePath}, tamaño: {backupFile.Length} bytes, nombre original: {backupFile.FileName}");

                using (var stream = System.IO.File.Create(tempFilePath))
                {
                    await backupFile.CopyToAsync(stream);
                }

                _logger.LogInformation("Archivo guardado temporalmente, iniciando restauración...");

                var result = await _backupService.RestoreBackupAsync(tempFilePath);

                if (result.Success)
                {
                    _logger.LogInformation("Restauración exitosa: " + result.Message);
                    return Ok(result.Message);
                }
                else
                {
                    _logger.LogError("Error en restauración: " + result.Message);
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al restaurar respaldo");
                return BadRequest($"Error al restaurar el respaldo: {ex.Message}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(tempFilePath))
                {
                    try
                    {
                        if (System.IO.File.Exists(tempFilePath))
                        {
                            System.IO.File.Delete(tempFilePath);
                            _logger.LogInformation($"Archivo temporal eliminado: {tempFilePath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"No se pudo eliminar archivo temporal: {tempFilePath}");
                    }
                }
            }
        }
        public class RestoreBackupRequest
        {
            public required string FileName { get; set; }
        }
    }
}