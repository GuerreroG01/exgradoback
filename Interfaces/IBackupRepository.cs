using ExGradoBack.Models;
namespace ExGradoBack.Repositories
{
    public interface IBackupRepository
    {
        Task<Backup?> GetBackupConfigAsync();
        Task<Backup?> GetBackupByIdAsync(int id);
        Task AddBackupConfigAsync(Backup backup);
        Task UpdateBackupConfigAsync( int Id, Backup backup);
        Task<(bool Success, string Message, byte[] BackupBytes, string FileName)> CreateBackupAsync();
        Task<(bool Success, string Message)> RestoreBackupAsync(string backupFileName);
    }
}