using ExGradoBack.Models;
using ExGradoBack.DTOs;
namespace ExGradoBack.Services
{
    public interface IRepuestoService
    {
        Task<IEnumerable<Repuesto>> GetRepuestosByNameAsync(string nombre);
        Task<Repuesto?> GetRepuestoByIdAsync(int id);
        Task<IEnumerable<Repuesto>> GetRepuestosByUbicacionAsync(string ubicacion);
        Task<Repuesto> CreateRepuestoAsync(RepuestoDto repuestoDto);
        Task<Repuesto> UpdateRepuestoAsync(int id, RepuestoDto repuestodto);
        Task<bool> DeleteRepuestoAsync(int id);
        Task<bool> RepuestoExistsAsync(int id);
        Task<IEnumerable<string>> GetAllUbicacionesAsync();
    }
}