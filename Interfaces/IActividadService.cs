using ExGradoBack.Models;

namespace ExGradoBack.Services
{
    public interface IActividadService
    {
        Task RegistrarAsync(string usuario, string accion, int facturaId, object? antes, object? despues);
    }
}