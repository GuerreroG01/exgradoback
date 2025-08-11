using ExGradoBack.Models;
namespace ExGradoBack.Repositories
{
    public interface IRepuestoRepository
    {
        Task<IEnumerable<Repuesto>> GetRepuestosByNameAsync(string nombre);
        Task<Repuesto?> GetRepuestoByIdAsync(int id);
        Task<Repuesto> CreateRepuestoAsync(Repuesto repuesto);
        Task<Repuesto> UpdateRepuestoAsync(Repuesto repuesto);
        Task<bool> DeleteRepuestoAsync(int id);
        Task<bool> RepuestoExistsAsync(string nombre);
    }
}