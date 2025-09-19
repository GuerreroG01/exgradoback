using ExGradoBack.Models;
namespace ExGradoBack.Repositories
{
    public interface IDetalleOrdenRepository
    {
        Task<List<DetalleOrdenCompra>> GetDetalleOrdenByIdOrdenAsync(int idOrden);
        Task<DetalleOrdenCompra> CreateDetalleOrdenAsync(DetalleOrdenCompra detalle);
        Task<DetalleOrdenCompra> UpdateDetalleOrdenAsync(DetalleOrdenCompra detalle);
        Task<bool> DeleteDetalleOrdenAsync(int id);
        Task<bool> DetalleOrdenExistsAsync(int id);
    }
}