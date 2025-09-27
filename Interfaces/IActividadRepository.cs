using ExGradoBack.Models;

namespace ExGradoBack.Repositories
{
    public interface IActividadRepository
    {
        Task RegistrarAsync(string usuario, string accion, int facturaId, object? antes, object? despues);
    }
}