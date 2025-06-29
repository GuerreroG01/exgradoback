using ExGradoBack.Models;
namespace ExGradoBack.Services
{
    public interface IBackupService
    {
        Task<Backup?> GetBackupConfigAsync();
        Task<Backup?> GetBackupByIdAsync(int id);
        Task AddBackupConfigAsync(Backup backup);
        Task UpdateBackupConfigAsync( int Id, Backup backup);
        Task<(bool Success, string Message, string PathOrError)> CreateBackupAsync();
        Task<(bool Success, string Message)> RestoreBackupAsync(string backupFileName);
        bool OpenBackup(string backupPath, out string error);
    }
}