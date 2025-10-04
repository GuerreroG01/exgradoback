using ExGradoBack.DTOs;
using ExGradoBack.Models;

namespace ExGradoBack.Repositories
{
    public interface IActividadRepository
    {
        Task RegistrarAsync(string usuario, string accion, int facturaId, object? antes, object? despues);
        Task<List<ActividadResumenDto>> ObtenerActividadesAsync(string usuario, string? accion = null, int? meses = null);
        Task<ActividadFactura?> GetActividadByIdAsync(int id);
        Task LimpiarActividadesAsync();
    }
}