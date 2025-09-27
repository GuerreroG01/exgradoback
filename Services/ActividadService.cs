using ExGradoBack.Repositories;

namespace ExGradoBack.Services
{
    public class ActividadService : IActividadService
    {
        private readonly IActividadRepository _actividadRepository;

        public ActividadService(IActividadRepository actividadRepository)
        {
            _actividadRepository = actividadRepository;
        }

        public async Task RegistrarAsync(string usuario, string accion, int facturaId, object? antes, object? despues)
        {
            await _actividadRepository.RegistrarAsync(usuario, accion, facturaId, antes, despues);
        }
    }
}