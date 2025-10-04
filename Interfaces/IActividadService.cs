using ExGradoBack.Models;
using ExGradoBack.DTOs;

namespace ExGradoBack.Services
{
    public interface IActividadService
    {
        Task RegistrarAsync(string usuario, string accion, int facturaId, object? antes, object? despues);
        Task<List<ActividadResumenDto>> ObtenerActividadesAsync(string usuario, string? accion = null, int? meses = null);
        Task<ActividadFactura?> GetActividadByIdAsync(int id);
        Task LimpiarActividadesAsync();
    }
}