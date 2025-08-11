using ExGradoBack.Models;

namespace ExGradoBack.Services
{
    public interface IVehiculoInfoService
    {
        Task<IEnumerable<VehiculoInfo>> GetVehiculoInfosByMarcaAndAnioAsync(string? marca, int? anio);
        Task<VehiculoInfo?> GetVehiculoInfoByIdAsync(int id);
        Task<VehiculoInfo> CreateVehiculoInfoAsync(VehiculoInfo vehiculoInfo, IFormFile? fotoReferencia);
        Task<VehiculoInfo> UpdateVehiculoInfoAsync(VehiculoInfo vehiculoInfo, IFormFile? fotoReferencia);
        Task<bool> DeleteVehiculoInfoAsync(int id);
        Task<bool> VehiculoInfoExistsAsync(string placa);
        Task<IEnumerable<int>> GetAniosVehiculosAsync(string marca);
        Task<IEnumerable<string>> GetMarcasVehiculosAsync();
    }
}