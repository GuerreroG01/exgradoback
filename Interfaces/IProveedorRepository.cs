using ExGradoBack.Models;
namespace ExGradoBack.Repositories
{
    public interface IProveedorRepository
    {
        Task<IEnumerable<object>> GetProveedorsAsync(string? country, string? city, bool isMinInfo = false);
        Task<Proveedor?> GetProveedorByIdAsync(int id);
        Task<Proveedor> CreateProveedorAsync(Proveedor proveedor);
        Task<bool> DeleteProveedorAsync(int id);
        Task<bool> ProveedorExistsAsync(string nombre);
        Task SaveChangesAsync();
        Task<IEnumerable<string>> GetCountryProvAsync();
        Task<IEnumerable<string>> GetCityProvAsync(string country);
    }
}