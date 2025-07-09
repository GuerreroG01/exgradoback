using ExGradoBack.Repositories;

namespace ExGradoBack.Jobs
{
    public class BackupJob
    {
        private readonly IBackupRepository _repository;
        private readonly string _backupFolder;
        private readonly ILogger<BackupJob> _logger;

        public BackupJob(IBackupRepository backuprepository, IWebHostEnvironment env, ILogger<BackupJob> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (backuprepository == null) throw new ArgumentNullException(nameof(backuprepository));
            if (env == null) throw new ArgumentNullException(nameof(env));
            {
                _repository = backuprepository;
                _backupFolder = Path.Combine(env.ContentRootPath, "Backups");
            }
            _logger = logger;
        }

        public async Task RunAsync()
        {
            var backupConfig = await _repository.GetBackupConfigAsync();
            if (backupConfig == null)
            {
                _logger.LogError("No se encontró la configuración de respaldo.");
                throw new Exception("No se encontró la configuración de respaldo.");
            }
            if (backupConfig.Activo)
            {
                try
                {
                    await EjecutarBackup();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al ejecutar el backup.");
                    throw new Exception($"Error al ejecutar el backup: {ex.Message}", ex);
                }
            }
            else
            {
                _logger.LogInformation("El backup está deshabilitado en la configuración.");
                throw new Exception("El backup está deshabilitado en la configuración.");
            }
        }
        private DateTime? CalcularProximoRespaldo(string frecuencia, DateTime fechaAnterior)
        {
            var fechaBase = fechaAnterior;

            switch (frecuencia?.ToLower())
            {
                case "dia":
                    return fechaBase.AddDays(1);
                case "semana":
                    return fechaBase.AddDays(7);
                case "quincena":
                    return fechaBase.AddDays(15);
                case "mes":
                    return fechaBase.AddMonths(1);
                case "año":
                    return fechaBase.AddYears(1);
                default:
                    return null;
            }
        }
        private async Task EjecutarBackup()
        {
            var result = await _repository.CreateBackupAsync();

            if (result.Success)
            {
                Directory.CreateDirectory(_backupFolder);

                var filePath = Path.Combine(_backupFolder, result.FileName);
                await File.WriteAllBytesAsync(filePath, result.BackupBytes);
                _logger.LogInformation($"Backup creado exitosamente: {filePath}");

                var backupConfig = await _repository.GetBackupConfigAsync();
                if (backupConfig != null)
                {
                    var fechaAnterior = DateTime.Now;
                    var nuevaFecha = CalcularProximoRespaldo(backupConfig.Frecuencia_Respaldo, fechaAnterior);

                    if (nuevaFecha != null)
                    {
                        backupConfig.Fecha_RespaldoAnterior = fechaAnterior;
                        backupConfig.Fecha_Repaldo = nuevaFecha.Value;

                        await _repository.UpdateBackupConfigAsync(backupConfig.Id, backupConfig);
                        _logger.LogInformation($"Configuración de respaldo actualizada: próximo respaldo {nuevaFecha.Value:yyyy-MM-dd}");
                    }
                }
            }
            else
            {
                _logger.LogError($"Error creando backup: {result.Message}");
                throw new Exception($"Error creando backup: {result.Message}");
            }
        }
    }
}

