using ExGradoBack.Models;

namespace ExGradoBack.Services
{
    public interface IFacturaService
    {
        Task<IEnumerable<int>> GetAniosConFacturasAsync();
        Task<IEnumerable<int>> GetMesesConFacturasAsync(int anio);
        Task<IEnumerable<int>> GetDiasConFacturasAsync(int anio, int mes);
        Task<IEnumerable<Factura>> GetFacturasPorDiaAsync(int anio, int mes, int dia);
        Task<Factura?> GetFacturaByIdAsync(int id);
        Task<Factura> CreateFacturaAsync(Factura factura);
        Task<Factura> UpdateFacturaAsync(Factura factura);
        Task<bool> DeleteFacturaAsync(int id);
        Task<bool> FacturaExistsAsync(int id);
    }
}