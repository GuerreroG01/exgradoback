using ExGradoBack.Models;
using ExGradoBack.Repositories;
using Microsoft.Extensions.Logging;

namespace ExGradoBack.Services
{
    public class DetalleOrdenService : IDetalleOrdenService
    {
        private readonly IDetalleOrdenRepository _detalleOrdenRepository;
        private readonly IOrdenCompraRepository _ordenCompraRepository;
        private readonly IRepuestoRepository _repuestoRepository;
        private readonly ILogger<DetalleOrdenService> _logger;
        public DetalleOrdenService(IDetalleOrdenRepository detalleOrdenRepository, IOrdenCompraRepository ordenCompraRepository, IRepuestoRepository repuestoRepository, ILogger<DetalleOrdenService> logger)
        {
            _detalleOrdenRepository = detalleOrdenRepository;
            _ordenCompraRepository = ordenCompraRepository;
            _repuestoRepository = repuestoRepository;
            _logger = logger;
        }
        public async Task<List<DetalleOrdenCompra>> GetDetalleOrdenByIdOrdenAsync(int OrdenCompraId)
        {
            if (OrdenCompraId <= 0)
                throw new ArgumentException("OrdenCompraId inválido");

            var detalle = await _detalleOrdenRepository.GetDetalleOrdenByIdOrdenAsync(OrdenCompraId);

            return detalle ?? new List<DetalleOrdenCompra>();
        }
        public async Task<DetalleOrdenCompra> CreateDetalleOrdenAsync(DetalleOrdenCompra detalle)
        {
            _logger.LogInformation("Agregando detalles a la orden de compra. Datos: {@detalle}", detalle);

            if (!await _ordenCompraRepository.OrdenExistsAsync(detalle.OrdenCompraId))
                throw new Exception($"La orden de compra con Id {detalle.OrdenCompraId} no existe");

            if (!await _repuestoRepository.RepuestoExistsAsync(detalle.RepuestoId))
                throw new Exception($"El repuesto con Id {detalle.RepuestoId} no existe");

            detalle.Subtotal = detalle.Cantidad * detalle.PrecioProveedor;
            return await _detalleOrdenRepository.CreateDetalleOrdenAsync(detalle);
        }
        public async Task<DetalleOrdenCompra> UpdateDetalleOrdenAsync(DetalleOrdenCompra detalle)
        {
            if (!await _ordenCompraRepository.OrdenExistsAsync(detalle.OrdenCompraId))
                throw new Exception($"La orden de compra con Id {detalle.OrdenCompraId} no existe");

            if (!await _repuestoRepository.RepuestoExistsAsync(detalle.RepuestoId))
                throw new Exception($"El repuesto con Id {detalle.RepuestoId} no existe");
            
            return await _detalleOrdenRepository.UpdateDetalleOrdenAsync(detalle);
        }
        public async Task<bool> DeleteDetalleOrdenAsync(int id)
        {
            if (!await _detalleOrdenRepository.DetalleOrdenExistsAsync(id))
                throw new Exception($"El dato con el ID {id} no existe.");
            return await _detalleOrdenRepository.DeleteDetalleOrdenAsync(id);
        }
        public async Task<bool> DetalleOrdenExistsAsync(int id)
        {
            return await _detalleOrdenRepository.DetalleOrdenExistsAsync(id);
        }
    }
}