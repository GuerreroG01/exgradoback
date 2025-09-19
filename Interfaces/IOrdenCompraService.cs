using ExGradoBack.Models;
namespace ExGradoBack.Services
{
    public interface IOrdenCompraService
    {
        Task<IEnumerable<int>> GetAniosConOrdenesAsync();
        Task<IEnumerable<int>> GetMesesConOrdenesAsync(int anio);
        Task<IEnumerable<int>> GetDiasConOrdenesAsync(int anio, int mes);
        Task<IEnumerable<OrdenCompra>> GetOrdenesPorDiaAsync(int anio, int mes, int dia);
        Task<OrdenCompra?> GetOrdenByIdAsync(int id);
        Task<OrdenCompra> CreateOrdenAsync(OrdenCompra orden);
        Task<OrdenCompra> UpdateOrdenAsync(OrdenCompra orden);
        Task<bool> DeleteOrdenAsync(int id);
        Task<bool> OrdenExistsAsync(int id);
    }
}