using ExGradoBack.Models;
using Microsoft.AspNetCore.Mvc;
namespace ExGradoBack.Repositories
{
    public interface IVehiculoInfoRepository
    {
        Task<IEnumerable<object>> GetVehiculoInfosByMarcaAndAnioAsync(string? marca, int? anio, bool isMinInfo = false);
        Task<IEnumerable<object>> GetVehiculoInfosByModeloAsync(string? modelo, bool isMinInfo = false);
        Task<VehiculoInfo?> GetVehiculoInfoByIdAsync(int id);
        Task<VehiculoInfo> CreateVehiculoInfoAsync(VehiculoInfo vehiculoInfo);
        Task<VehiculoInfo> UpdateVehiculoInfoAsync(VehiculoInfo vehiculoInfo);
        Task<bool> DeleteVehiculoInfoAsync(int id);
        Task<bool> VehiculoInfoExistsAsync(string modelo);
        Task<IEnumerable<int>> GetAniosVehiculosAsync(string marca);
        Task<IEnumerable<string>> GetMarcasVehiculosAsync();
    }
}