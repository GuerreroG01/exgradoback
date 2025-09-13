using ExGradoBack.Models;
namespace ExGradoBack.Services
{
    public interface IRepuestoService
    {
        Task<IEnumerable<Repuesto>> GetRepuestosByNameAsync(string nombre);
        Task<Repuesto?> GetRepuestoByIdAsync(int id);
        Task<IEnumerable<Repuesto>> GetRepuestosByUbicacionAsync(string ubicacion);
        Task<Repuesto> CreateRepuestoAsync(Repuesto repuesto);
        Task<Repuesto> UpdateRepuestoAsync(Repuesto repuesto);
        Task<bool> DeleteRepuestoAsync(int id);
        Task<bool> RepuestoExistsAsync(int id);
        Task<IEnumerable<string>> GetAllUbicacionesAsync();
    }
}