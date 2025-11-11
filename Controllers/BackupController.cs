using ExGradoBack.Models;
using ExGradoBack.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ExGradoBack.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BackupController : ControllerBase
    {
        private readonly IBackupService _backupService;
        private readonly ILogger<BackupController> _logger;
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
            _logger.LogInformation("[Controller] Iniciando descarga del respaldo...");

            var result = await _backupService.CreateBackupAsync();

            if (!result.Success)
            {
                _logger.LogError("[Controller] Error al crear el respaldo: {Message}", result.Message);
                return StatusCode(500, new { message = result.Message });
            }

            _logger.LogInformation("[Controller] Respaldo generado correctamente: {FileName} ({Size} bytes)", result.FileName, result.BackupBytes.Length);

            Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{result.FileName}\"");

            return File(result.BackupBytes, "application/octet-stream");
        }

        [HttpPost("restaurar")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> RestaurarBackup([FromForm] IFormFile backupFile)
        {
            _logger.LogInformation("[Controller] Petición de restauración recibida...");

            if (backupFile == null || backupFile.Length == 0)
            {
                _logger.LogWarning("[Controller] No se recibió archivo o está vacío.");
                return BadRequest("No se recibió el archivo de respaldo.");
            }

            // Ruta temporal dentro del contenedor (asegúrate de crearla en el Dockerfile)
            var tempDir = "/app/backups/temp";
            Directory.CreateDirectory(tempDir);

            var tempFilePath = Path.Combine(tempDir, $"{Guid.NewGuid()}.sql");
            _logger.LogInformation("[Controller] Guardando archivo temporal en: {Path}", tempFilePath);

            try
            {
                using (var stream = System.IO.File.Create(tempFilePath))
                {
                    await backupFile.CopyToAsync(stream);
                }

                _logger.LogInformation("[Controller] Archivo guardado correctamente. Tamaño: {Length} bytes", backupFile.Length);

                var result = await _backupService.RestoreBackupAsync(tempFilePath);

                _logger.LogInformation("[Controller] Resultado de la restauración: {Message}", result.Message);

                if (result.Success)
                {
                    _logger.LogInformation("[Controller] Restauración completada con éxito.");
                    return Ok(result.Message);
                }

                _logger.LogWarning("[Controller] Falló la restauración: {Message}", result.Message);
                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Controller] Error al procesar la restauración.");
                return BadRequest($"Error al restaurar el respaldo: {ex.Message}");
            }
            finally
            {
                if (System.IO.File.Exists(tempFilePath))
                {
                    _logger.LogInformation("[Controller] Eliminando archivo temporal: {Path}", tempFilePath);
                    System.IO.File.Delete(tempFilePath);
                }
            }
        }
        public class RestoreBackupRequest
        {
            public required string FileName { get; set; }
        }
    }
}