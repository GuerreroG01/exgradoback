using ExGradoBack.Models;
using ExGradoBack.DTOs;
namespace ExGradoBack.Repositories
{
    public interface IOrdenCompraRepository
    {
        Task<IEnumerable<int>> GetAniosConOrdenesAsync();
        Task<IEnumerable<int>> GetMesesConOrdenesAsync(int anio);
        Task<IEnumerable<int>> GetDiasConOrdenesAsync(int anio, int mes);
        Task<IEnumerable<OrdenCompra>> GetOrdenesPorDiaAsync(int anio, int mes, int dia);
        Task<OrdenCompra?> GetOrdenByIdAsync(int id);
        Task<OrdenCompra> CreateOrdenAsync(OrdenCompra orden);
        Task SaveChangesAsync();
        Task<bool> DeleteOrdenAsync(int id);
        Task<bool> OrdenExistsAsync(int id);
        Task<List<OrdenCompra>> GetOrdenesPendientesAsync();
        Task<OrdenCompraResumenDTO?> GetResumenOrdenesPorFechaAsync(DateTime fecha);
    }
}