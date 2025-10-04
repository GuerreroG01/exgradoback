using ExGradoBack.Repositories;
using Microsoft.Extensions.Logging;

namespace ExGradoBack.Jobs
{
    public class LimpiezaActividadesJob
    {
        private readonly IActividadRepository _actividadRepository;
        private readonly ILogger<LimpiezaActividadesJob> _logger;

        public LimpiezaActividadesJob(IActividadRepository actividadRepository, ILogger<LimpiezaActividadesJob> logger)
        {
            _actividadRepository = actividadRepository ?? throw new ArgumentNullException(nameof(actividadRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("Iniciando job de limpieza de actividades antiguas...");

            try
            {
                await _actividadRepository.LimpiarActividadesAsync();

                _logger.LogInformation("Job de limpieza finalizado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando el job de limpieza de actividades.");
                throw;
            }
        }
    }
}