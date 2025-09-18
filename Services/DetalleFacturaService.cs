using ExGradoBack.Models;
using ExGradoBack.Repositories;

namespace ExGradoBack.Services
{
    public class DetalleFacturaService : IDetalleFacturaService
    {
        private readonly IDetalleFacturaRepository _detalleFacturaRepository;
        private readonly IFacturaRepository _facturaRepository;
        private readonly IRepuestoRepository _repuestoRepository;
        public DetalleFacturaService(IDetalleFacturaRepository detalleFacturaRepository, IFacturaRepository facturaRepository, IRepuestoRepository repuestoRepository)
        {
            _detalleFacturaRepository = detalleFacturaRepository;
            _facturaRepository = facturaRepository;
            _repuestoRepository = repuestoRepository;
        }
        public async Task<List<DetalleFactura>?> GetDetalleFacturaByIdFacturaAsync(int facturaId)
        {
            if (facturaId <= 0)
                throw new ArgumentException("FacturaId inválido");

            var detalle = await _detalleFacturaRepository.GetDetalleFacturaByIdFacturaAsync(facturaId);

            if (detalle == null || detalle.Count == 0)
                return null;

            return detalle;
        }
        public async Task<DetalleFactura> CreateDetalleFacturaAsync(DetalleFactura detalle)
        {
            if (!await _facturaRepository.FacturaExistsAsync(detalle.FacturaId))
                throw new Exception($"La factura con Id {detalle.FacturaId} no existe");

            if (!await _repuestoRepository.RepuestoExistsAsync(detalle.RepuestoId))
                throw new Exception($"El repuesto con Id {detalle.RepuestoId} no existe");

            detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;
            return await _detalleFacturaRepository.CreateDetalleFacturaAsync(detalle);
        }
        public async Task<DetalleFactura> UpdateDetalleFacturaAsync(DetalleFactura detalle)
        {
            if (!await _facturaRepository.FacturaExistsAsync(detalle.FacturaId))
                throw new Exception($"La factura con Id {detalle.FacturaId} no existe");

            if (!await _repuestoRepository.RepuestoExistsAsync(detalle.RepuestoId))
                throw new Exception($"El repuesto con Id {detalle.RepuestoId} no existe");
            if (detalle.Subtotal <= 0)
                throw new ArgumentException("El subtotal debe ser mayor a 0.");

            return await _detalleFacturaRepository.UpdateDetalleFacturaAsync(detalle);
        }
        public async Task<bool> DeleteDetalleFacturaAsync(int id)
        {
            if (!await _detalleFacturaRepository.DetalleFacturaExistsAsync(id))
                throw new Exception($"El ID {id} no existe.");

            return await _detalleFacturaRepository.DeleteDetalleFacturaAsync(id);
        }
        public async Task<bool> DetalleFacturaExistsAsync(int id)
        {
            return await _detalleFacturaRepository.DetalleFacturaExistsAsync(id);
        }
        public async Task<List<(int RepuestoId, string Nombre, int TotalVendidos)>> ObtenerTop10RepuestosAsync()
        {
            return await _detalleFacturaRepository.ObtenerTop10RepuestosAsync();
        }
    }
}