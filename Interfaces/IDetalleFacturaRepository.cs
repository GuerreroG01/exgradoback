using ExGradoBack.Models;

namespace ExGradoBack.Repositories
{
    public interface IDetalleFacturaRepository
    {
        Task<List<DetalleFactura>> GetDetalleFacturaByIdFacturaAsync(int idFactura);
        Task<DetalleFactura> CreateDetalleFacturaAsync(DetalleFactura detalle);
        Task<DetalleFactura> UpdateDetalleFacturaAsync(DetalleFactura Detalle);
        Task<bool> DeleteDetalleFacturaAsync(int id);
        Task<bool> DetalleFacturaExistsAsync(int id);
        Task<List<(int RepuestoId, string Nombre, int TotalVendidos)>> ObtenerTop10RepuestosAsync();
    }
}