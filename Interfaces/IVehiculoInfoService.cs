using ExGradoBack.Models;
using ExGradoBack.DTOs;

namespace ExGradoBack.Services
{
    public interface IVehiculoInfoService
    {
        Task<IEnumerable<VehiculoInfo>> GetVehiculoInfosFullAsync(string? marca, int? anio);
        Task<IEnumerable<VehiculoInfoMinDto>> GetVehiculoInfosMinAsync(string? marca, int? anio);
        Task<IEnumerable<VehiculoInfo>> GetVehiculoFullAsync(string? modelo);
        Task<IEnumerable<VehiculoInfoMinDto>> GetVehiculoMinAsync(string? modelo);
        Task<VehiculoInfo?> GetVehiculoInfoByIdAsync(int id);
        Task<VehiculoInfo> CreateVehiculoInfoAsync(VehiculoInfo vehiculoInfo, IFormFile? fotoReferencia);
        Task<VehiculoInfo> UpdateVehiculoInfoAsync(VehiculoInfo vehiculoInfo, IFormFile? fotoReferencia);
        Task<bool> DeleteVehiculoInfoAsync(int id);
        Task<bool> VehiculoInfoExistsAsync(string placa);
        Task<IEnumerable<int>> GetAniosVehiculosAsync(string marca);
        Task<IEnumerable<string>> GetMarcasVehiculosAsync();
    }
}