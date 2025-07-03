using ExGradoBack.Repositories;
using ExGradoBack.Models;
namespace ExGradoBack.Services
{
    public class BackupService : IBackupService
    {
        private readonly IBackupRepository _backupRepository;
        public BackupService(IBackupRepository backupRepository)
        {
            _backupRepository = backupRepository;
        }
        public Task<Backup?> GetBackupConfigAsync()
        {
            try
            {
                return _backupRepository.GetBackupConfigAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al obtener la configuración del respaldo.", ex);
            }
        }
        public async Task<Backup?> GetBackupByIdAsync(int id)
        {
            return await _backupRepository.GetBackupByIdAsync(id);
        }
        public async Task AddBackupConfigAsync(Backup backup)
        {
            try
            {
                await _backupRepository.AddBackupConfigAsync(backup);
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al guardar la configuración del respaldo.", ex);
            }
        }
        public async Task UpdateBackupConfigAsync(int id, Backup backup)
        {
            var existing = await _backupRepository.GetBackupByIdAsync(id);
            if (existing == null)
                throw new Exception("Backup no encontrado");

            await _backupRepository.UpdateBackupConfigAsync(id, backup);
        }
        public Task<(bool Success, string Message, byte[] BackupBytes, string FileName)> CreateBackupAsync()
            => _backupRepository.CreateBackupAsync();

        public Task<(bool Success, string Message)> RestoreBackupAsync(string backupFileName)
            => _backupRepository.RestoreBackupAsync(backupFileName);
    }
}