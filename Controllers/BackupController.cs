using ExGradoBack.Models;
using ExGradoBack.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExGradoBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BackupController : ControllerBase
    {
        private readonly IBackupService _backupService;
        public BackupController(IBackupService backupService)
        {
            _backupService = backupService;
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
            if (backupFile == null || backupFile.Length == 0)
                return BadRequest("No se recibió el archivo de respaldo.");

            var tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try
            {
                using (var stream = System.IO.File.Create(tempFilePath))
                {
                    await backupFile.CopyToAsync(stream);
                }

                var result = await _backupService.RestoreBackupAsync(tempFilePath);

                if (result.Success)
                    return Ok(result.Message);

                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al restaurar el respaldo: {ex.Message}");
            }
            finally
            {
                if (System.IO.File.Exists(tempFilePath))
                {
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