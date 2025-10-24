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

            var backup = await _context.Backup
                .OrderBy(b => b.Id)
                .FirstOrDefaultAsync();
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
        public async Task<(bool Success, string Message, byte[] BackupBytes, string FileName)> CreateBackupAsync()
        {
            try
            {
                string fileName = $"ExGradoBackup_{DateTime.Now:yyyyMMddHHmmss}.sql";

                var psi = new ProcessStartInfo
                {
                    FileName = "mysqldump",
                    Arguments = $"-h {dbHost} -u {dbUser} -p{dbPassword} {dbName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = psi };
                process.Start();

                using var ms = new MemoryStream();
                await process.StandardOutput.BaseStream.CopyToAsync(ms);

                await process.WaitForExitAsync();

                string error = await process.StandardError.ReadToEndAsync();

                if (process.ExitCode != 0 || (!string.IsNullOrEmpty(error) && !error.Contains("Using a password")))
                    return (false, "Error al crear el respaldo", null!, fileName);

                return (true, "Respaldo creado exitosamente", ms.ToArray(), fileName);
            }
            catch (Exception ex)
            {
                return (false, "Excepción al crear el respaldo", null!, $"backup.sql:{ex.Message}");
            }
        }

        /*Metodo funcional sin docker
        public async Task<(bool Success, string Message)> RestoreBackupAsync(string backupFilePath)
        {
            try
            {
                if (!File.Exists(backupFilePath))
                    return (false, "El archivo de respaldo no existe.");

                var psi = new ProcessStartInfo("mysql")
                {
                    Arguments = $"-h {dbHost} -u {dbUser} -p{dbPassword} {dbName}",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = psi };
                process.Start();

                using var fileStream = new StreamReader(backupFilePath);
                string? line;
                while ((line = await fileStream.ReadLineAsync()) != null)
                {
                    await process.StandardInput.WriteLineAsync(line);
                }

                process.StandardInput.Close();
                await process.WaitForExitAsync();

                string stdOutput = await process.StandardOutput.ReadToEndAsync();
                string stdError = await process.StandardError.ReadToEndAsync();

                if (process.ExitCode == 0)
                {
                    return (true, "Copia de seguridad restaurada exitosamente.");
                }
                else
                {
                    return (false, $"Error al restaurar el backup: {stdError}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error al restaurar el respaldo: {ex.Message}");
            }
        }*/

        //Metodo funcional con docker
        public async Task<(bool Success, string Message)> RestoreBackupAsync(string backupFilePath)
        {
            if (!File.Exists(backupFilePath))
                return (false, "El archivo de respaldo no existe.");

            try
            {
                // Creamos un archivo temporal con instrucciones para deshabilitar constraints, ejecutar el backup y reactivarlas
                string tempSqlFile = Path.Combine(Path.GetTempPath(), $"restore_{Guid.NewGuid()}.sql");

                // Construimos el contenido del archivo temporal
                string sqlContent = $@"
                    SET FOREIGN_KEY_CHECKS=0;
                    SET UNIQUE_CHECKS=0;
                    SOURCE {backupFilePath};
                    SET FOREIGN_KEY_CHECKS=1;
                    SET UNIQUE_CHECKS=1;
                    ";

                await File.WriteAllTextAsync(tempSqlFile, sqlContent);

                var psi = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"mysql -h {dbHost} -u{dbUser} -p{dbPassword} {dbName} < {tempSqlFile}\"",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = psi };
                process.Start();

                string stdOutput = await process.StandardOutput.ReadToEndAsync();
                string stdError = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                // Eliminamos el archivo temporal
                File.Delete(tempSqlFile);

                if (process.ExitCode == 0)
                    return (true, "Copia de seguridad restaurada exitosamente.");
                else
                    return (false, $"Error al restaurar el backup: {stdError}");
            }
            catch (Exception ex)
            {
                return (false, $"Error al restaurar el respaldo: {ex.Message}");
            }
        }
    }
}