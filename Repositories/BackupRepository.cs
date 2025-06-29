using ExGradoBack.Models;
using Microsoft.EntityFrameworkCore;
using ExGradoBack.Data;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using MySqlConnector;
using dotenv.net;

namespace ExGradoBack.Repositories
{
    public class BackupRepository : IBackupRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BackupRepository> _logger;
        private string dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "undefined";
        private string dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "undefined";
        private string dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "undefined";
        private string dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";
        private readonly string _backupDirectory = @"C:\Backups";
        public BackupRepository(AppDbContext context, ILogger<BackupRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<Backup?> GetBackupConfigAsync()
        {
            bool hasAny = await _context.Backup.AnyAsync();
            if (!hasAny)
            {
                return null;
            }

            var backup = await _context.Backup.FirstOrDefaultAsync();
            return backup;
        }
        public async Task<Backup?> GetBackupByIdAsync(int id)
        {
            return await _context.Backup.FindAsync(id);
        }

        public async Task AddBackupConfigAsync(Backup backup)
        {
            if (backup == null)
            {
                throw new ArgumentNullException(nameof(backup), "La configuración de respaldo no puede ser nula.");
            }

            try
            {
                _context.Backup.Add(backup);
                var result = await _context.SaveChangesAsync();

                if (result <= 0)
                {
                    throw new Exception("No se pudo guardar la configuración de respaldo.");
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar la configuración de respaldo.", ex);
            }
        }

        public async Task UpdateBackupConfigAsync( int Id ,Backup backup)
        {
            if (Id == 0)
            {
                throw new ArgumentException("El ID de la configuración de respaldo debe ser mayor que cero.", nameof(Id));
            }

            try
            {
                var existingBackup = await _context.Backup.FirstOrDefaultAsync();

                if (existingBackup == null)
                    throw new InvalidOperationException("No existe configuración de respaldo para actualizar.");

                existingBackup.Fecha_Repaldo = backup.Fecha_Repaldo;
                existingBackup.Frecuencia_Respaldo = backup.Frecuencia_Respaldo;
                existingBackup.Fecha_RespaldoAnterior = backup.Fecha_RespaldoAnterior;
                existingBackup.Activo = backup.Activo;

                _context.Entry(existingBackup).State = EntityState.Modified;
                var result = await _context.SaveChangesAsync();

                if (result <= 0)
                    throw new Exception("No se pudo actualizar la configuración de respaldo.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al actualizar la configuración de backup: {ex.Message}");
                throw new Exception("Error al actualizar la configuración de respaldo.", ex);
            }
        }
        public async Task<(bool Success, string Message, string PathOrError)> CreateBackupAsync()
        {
            try
            {
                string backupFilePath = Path.Combine(_backupDirectory, $"BancoNetDb_{DateTime.Now:yyyyMMddHHmmss}.sql");

                if (!Directory.Exists(_backupDirectory))
                    Directory.CreateDirectory(_backupDirectory);

                string arguments = $"-h {dbHost} -u {dbUser} -p{dbPassword} {dbName} > \"{backupFilePath}\"";

                var psi = new ProcessStartInfo("cmd.exe")
                {
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = psi };
                process.Start();

                await process.StandardInput.WriteLineAsync($"mysqldump {arguments}");
                process.StandardInput.Close();
                await process.WaitForExitAsync();

                var errorOutput = await process.StandardError.ReadToEndAsync();

                if (errorOutput.Contains("[Warning] Using a password on the command line interface can be insecure"))
                    return (true, "Respaldo creado exitosamente", backupFilePath);

                return (false, "Error al crear el respaldo", errorOutput);
            }
            catch (Exception ex)
            {
                return (false, "Excepción al crear el respaldo", ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> RestoreBackupAsync(string backupFileName)
        {
            string fullPath = Path.Combine(_backupDirectory, backupFileName);

            try
            {
                if (!File.Exists(fullPath))
                    return (false, "El archivo de respaldo no existe.");

                string arguments = $"-h {dbHost} -u {dbUser} -p{dbPassword} {dbName} < \"{fullPath}\"";

                var psi = new ProcessStartInfo("cmd.exe")
                {
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = psi };
                process.Start();

                await process.StandardInput.WriteLineAsync($"mysql {arguments}");
                process.StandardInput.Close();
                await process.WaitForExitAsync();

                string errorOutput = await process.StandardError.ReadToEndAsync();

                if (errorOutput.Contains("[Warning] Using a password on the command line interface can be insecure"))
                    return (true, "Copia de seguridad restaurada exitosamente");

                return (false, $"Error al restaurar el backup: {errorOutput}");
            }
            catch (Exception ex)
            {
                return (false, $"Error al restaurar el respaldo: {ex.Message}");
            }
        }

        public bool OpenBackup(string backupPath, out string error)
        {
            try
            {
                if (!File.Exists(backupPath))
                {
                    error = "El archivo de respaldo no existe.";
                    return false;
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"/select,\"{backupPath}\"",
                    UseShellExecute = true
                });

                error = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}