using ExGradoBack.Models;
using ExGradoBack.DTOs;

namespace ExGradoBack.Services
{
    public interface IProveedorService
    {
        Task<IEnumerable<Proveedor>> GetProveedorInfoFullAsync(string? country, string? city);
        Task<IEnumerable<ProveedorMinInfo>> GetProveedorInfoMinAsync(string? country, string? city);
        Task<Proveedor?> GetProveedorByIdAsync(int id);
        Task<Proveedor> CreateProveedorAsync(Proveedor proveedor);
        Task<Proveedor> UpdateProveedorAsync(Proveedor proveedor);
        Task<bool> DeleteProveedorAsync(int id);
        Task<bool> ProveedorExistsAsync(string nombre);
        Task<IEnumerable<string>> GetCountryProvAsync();
        Task<IEnumerable<string>> GetCityProvAsync(string country);
        Task<IEnumerable<ProveedorMinInfo>> AutocompletarProveedoresAsync(string nombre);
    }
}