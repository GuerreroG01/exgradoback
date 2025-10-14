using ExGradoBack.Models;
using ExGradoBack.DTOs;

namespace ExGradoBack.Repositories
{
    public interface IFacturaRepository
    {
        Task<IEnumerable<int>> GetAniosConFacturasAsync();
        Task<IEnumerable<int>> GetMesesConFacturasAsync(int anio);
        Task<IEnumerable<int>> GetDiasConFacturasAsync(int anio, int mes);
        Task<IEnumerable<Factura>> GetFacturasPorDiaAsync(int anio, int mes, int dia);
        Task<Factura?> GetFacturaByIdAsync(int id);
        Task<Factura> CreateFacturaAsync(Factura factura);
        Task SaveChangesAsync();
        Task<bool> DeleteFacturaAsync(int id);
        Task<bool> FacturaExistsAsync(int id);
        Task<List<(string Vendedor, int TotalVendidos)>> ObtenerTop3VendedoresAsync();
        Task<List<(int Mes, decimal TotalVentas)>> ObtenerTotalVentasPorMesAsync(int anio);
        Task<List<FacturasPorBloqueDto>> ObtenerFacturasPorBloqueAsync();
        Task<Dictionary<DateTime, DatosPorDiaDto>> GetCantidadFacturasPorDiaUltimaSemanaAsync();
        Task<Dictionary<DateTime, DatosPorDiaDto>> GetCantidadRepuestosVendidosPorDiaUltimaSemanaAsync();
        Task<List<FacturaReporteDto>> ObtenerReporteMensualAsync(int anio, int mes);
        Task<List<TipoClienteReporteDto>> ObtenerReportePorTipoClienteAsync();
    }
}