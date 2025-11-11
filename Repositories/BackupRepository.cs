using ExGradoBack.Models;
using Microsoft.EntityFrameworkCore;
using ExGradoBack.Data;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Text;
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
        private string userRoot = Environment.GetEnvironmentVariable("root") ?? "undefined";
        private string rootPassword = Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD") ?? "";
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

                string command = $"mysqldump -h {dbHost} -u {userRoot} -p{rootPassword} --no-tablespaces {dbName}";

                var psi = new ProcessStartInfo
                {
                    FileName = "mysqldump",
                    Arguments = $"-h {dbHost} -u {dbUser} -p{dbPassword} --no-tablespaces {dbName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = psi };
                process.Start();

                using var ms = new MemoryStream();
                await process.StandardOutput.BaseStream.CopyToAsync(ms);

                string stdError = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                string stdOutputPreview = Encoding.UTF8.GetString(ms.ToArray(), 0, Math.Min(500, (int)ms.Length));

                if (!string.IsNullOrEmpty(stdError))
                {
                    _logger.LogWarning("StdErr: {StdError}", stdError);
                }

                if (process.ExitCode != 0)
                {
                    return (false, $"Error al crear el respaldo: {stdError}", null!, fileName);
                }

                return (true, "Respaldo creado exitosamente", ms.ToArray(), fileName);
            }
            catch (Exception ex)
            {
                return (false, $"Excepción al crear el respaldo: {ex.Message}", null!, $"backup.sql:{ex.Message}");
            }
        }
        public async Task<(bool Success, string Message)> RestoreBackupAsync(string backupFilePath)
        {
            try
            {
                if (!File.Exists(backupFilePath))
                    return (false, $"El archivo de respaldo no existe: {backupFilePath}");

                backupFilePath = backupFilePath.Replace("\\", "/");

                var cleanupPsi = new ProcessStartInfo
                {
                    FileName = "mysql",
                    Arguments = $"-h {dbHost} -u {dbUser} -p{dbPassword} -e \"DROP DATABASE IF EXISTS {dbName}; CREATE DATABASE {dbName};\"",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var cleanupProcess = Process.Start(cleanupPsi))
                {
                    if (cleanupProcess == null)
                        return (false, "No se pudo iniciar el proceso de limpieza MySQL.");

                    string cleanupOut = await cleanupProcess.StandardOutput.ReadToEndAsync();
                    string cleanupErr = await cleanupProcess.StandardError.ReadToEndAsync();
                    await cleanupProcess.WaitForExitAsync();

                    if (cleanupProcess.ExitCode != 0)
                        return (false, $"Error al limpiar la base de datos: {cleanupErr}");
                }

                // Dependiendo del sistema operativo, el método de restauración cambia
                bool isWindows = OperatingSystem.IsWindows();

                ProcessStartInfo psi;
                if (isWindows)
                {
                    psi = new ProcessStartInfo("mysql")
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

                    string stdOut = await process.StandardOutput.ReadToEndAsync();
                    string stdErr = await process.StandardError.ReadToEndAsync();
                    await process.WaitForExitAsync();

                    if (process.ExitCode == 0)
                        return (true, "Copia de seguridad restaurada exitosamente.");
                    else
                        return (false, $"Error al restaurar el backup (Windows): {stdErr}");
                }
                else
                {
                    var tempFile = Path.Combine(Path.GetTempPath(), Path.GetFileName(backupFilePath));
                    File.Copy(backupFilePath, tempFile, overwrite: true);

                    psi = new ProcessStartInfo
                    {
                        FileName = "sh",
                        Arguments = $"-c \"mysql -h {dbHost} -u {dbUser} -p{dbPassword} {dbName} < {tempFile}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using var process = Process.Start(psi);
                    if (process == null)
                        return (false, "No se pudo iniciar el proceso MySQL (Linux/Docker).");

                    string stdOut = await process.StandardOutput.ReadToEndAsync();
                    string stdErr = await process.StandardError.ReadToEndAsync();
                    await process.WaitForExitAsync();

                    File.Delete(tempFile);

                    if (process.ExitCode == 0)
                        return (true, "Copia de seguridad restaurada exitosamente.");
                    else
                        return (false, $"Error al restaurar el backup (Linux/Docker): {stdErr}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error general al restaurar el respaldo: {ex.Message}");
            }
        }
    }
}