using ExGradoBack.Services;
using ExGradoBack.Repositories;
using Microsoft.Extensions.Logging;

namespace ExGradoBack.Jobs
{
    public class OrdenCompraEmailJob
    {
        private readonly IOrdenCompraService _ordenService;
        private readonly IDetalleOrdenRepository _detalleRepo;
        private readonly IEmailService _emailService;
        private readonly ILogger<OrdenCompraEmailJob> _logger;

        public OrdenCompraEmailJob( IOrdenCompraService ordenService, IDetalleOrdenRepository detalleRepo, IEmailService emailService, ILogger<OrdenCompraEmailJob> logger)
        {
            _ordenService = ordenService;
            _detalleRepo = detalleRepo;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            try
            {
                var pendientes = await _ordenService.GetOrdenesPendientesAsync();

                if (!pendientes.Any())
                {
                    _logger.LogInformation("No hay órdenes pendientes por procesar.");
                    return;
                }

                foreach (var orden in pendientes)
                {
                    var detalles = await _detalleRepo.GetDetalleOrdenByIdOrdenAsync(orden.Id);

                    await _emailService.SendOrdenCompraEmailAsync(orden, detalles);

                    orden.Estado = "Enviado";

                    await _ordenService.UpdateOrdenAsync(orden);  // Actualiza usando el servicio

                    _logger.LogInformation($"Orden {orden.Id} enviada correctamente.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar órdenes de compra.");
            }
        }
   }
}