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
        [HttpPost("crear")]
        public async Task<IActionResult> CreateBackup()
        {
            var result = await _backupService.CreateBackupAsync();
            if (result.Success)
                return Ok(new { message = result.Message, path = result.PathOrError });

            return StatusCode(500, new { message = result.Message, error = result.PathOrError });
        }
        [HttpPost("restaurar")]
        public async Task<IActionResult> RestoreBackup([FromBody] string backupFileName)
        {
            var result = await _backupService.RestoreBackupAsync(backupFileName);
            if (result.Success)
                return Ok(new { message = result.Message });

            return StatusCode(500, new { message = result.Message });
        }
        [HttpGet("abrir")]
        public IActionResult OpenBackup([FromQuery] string path)
        {
            if (_backupService.OpenBackup(path, out string error))
                return Ok(new { message = "Archivo de respaldo abierto correctamente." });

            return StatusCode(500, new { message = "Error al abrir el archivo de respaldo", error });
        }
    }
}