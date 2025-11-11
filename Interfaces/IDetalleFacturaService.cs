using ExGradoBack.Models;

namespace ExGradoBack.Services
{
    public interface IDetalleFacturaService
    {
        Task<List<DetalleFactura>?> GetDetalleFacturaByIdFacturaAsync(int facturaId);
        Task<DetalleFactura> CreateDetalleFacturaAsync(DetalleFactura detalle);
        Task<DetalleFactura> UpdateSingleAsync(DetalleFactura detalle);
        Task UpdateDetalleFacturaAsync(int facturaId, ICollection<DetalleFactura> detallesNuevos);
        Task<bool> DeleteDetalleFacturaAsync(int id);
        Task<bool> DetalleFacturaExistsAsync(int id);
        Task<List<(int RepuestoId, string Nombre, int TotalVendidos)>> ObtenerTop10RepuestosAsync();
    }
}