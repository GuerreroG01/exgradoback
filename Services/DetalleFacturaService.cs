using ExGradoBack.Models;
using ExGradoBack.Repositories;

namespace ExGradoBack.Services
{
    public class DetalleFacturaService : IDetalleFacturaService
    {
        private readonly IDetalleFacturaRepository _detalleFacturaRepository;
        private readonly IFacturaRepository _facturaRepository;
        private readonly IRepuestoRepository _repuestoRepository;
        private readonly ILogger<DetalleFacturaService> _logger;
        public DetalleFacturaService(IDetalleFacturaRepository detalleFacturaRepository, IFacturaRepository facturaRepository, IRepuestoRepository repuestoRepository, ILogger<DetalleFacturaService> logger)
        {
            _detalleFacturaRepository = detalleFacturaRepository;
            _facturaRepository = facturaRepository;
            _repuestoRepository = repuestoRepository;
            _logger = logger;
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

            var repuesto = await _repuestoRepository.GetRepuestoByIdAsync(detalle.RepuestoId);
            if (repuesto == null)
                throw new Exception($"El repuesto con Id {detalle.RepuestoId} no existe");

            if (repuesto.StockActual < detalle.Cantidad)
                throw new Exception($"Stock insuficiente para el repuesto {repuesto.Nombre}. Stock disponible: {repuesto.StockActual}");

            detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;

            repuesto.StockActual -= detalle.Cantidad;

            await _repuestoRepository.UpdateRepuestoAsync(repuesto);

            return await _detalleFacturaRepository.CreateDetalleFacturaAsync(detalle);
        }
        public async Task<DetalleFactura> UpdateDetalleFacturaAsync(DetalleFactura detalle)
        {
            try
            {
                if (!await _facturaRepository.FacturaExistsAsync(detalle.FacturaId))
                    throw new Exception($"La factura con Id {detalle.FacturaId} no existe");

                if (!await _repuestoRepository.RepuestoExistsAsync(detalle.RepuestoId))
                    throw new Exception($"El repuesto con Id {detalle.RepuestoId} no existe");

                if (detalle.Subtotal <= 0)
                    throw new ArgumentException("El subtotal debe ser mayor a 0.");

                var detallesExistentes = await _detalleFacturaRepository.GetDetalleFacturaByIdFacturaAsync(detalle.FacturaId)
                                        ?? new List<DetalleFactura>();

                var detallesAEliminar = detallesExistentes
                    .Where(d => d.Id != detalle.Id)
                    .ToList();

                foreach (var d in detallesAEliminar)
                {
                    await DeleteDetalleFacturaAsync(d.Id);
                }

                var detalleOriginal = await _detalleFacturaRepository.GetDetalleFacturaByIdAsync(detalle.Id);
                if (detalleOriginal == null)
                    throw new Exception("El detalle de factura a actualizar no existe");


                var repuesto = await _repuestoRepository.GetRepuestoByIdAsync(detalle.RepuestoId);
                if (repuesto == null)
                    throw new Exception($"El repuesto con Id {detalle.RepuestoId} no existe");

                int cantidadOriginal = detalleOriginal.Cantidad;
                int cantidadNueva = detalle.Cantidad;
                int diferencia = cantidadNueva - cantidadOriginal;

                repuesto.StockActual -= diferencia;
                
                if (repuesto.StockActual < 0)
                    throw new Exception("No hay suficiente stock para realizar esta modificación");

                await _repuestoRepository.UpdateRepuestoAsync(repuesto);

                detalleOriginal.Cantidad = detalle.Cantidad;
                detalleOriginal.PrecioUnitario = detalle.PrecioUnitario;
                detalleOriginal.Subtotal = detalle.Subtotal;
                detalleOriginal.FacturaId = detalle.FacturaId;
                detalleOriginal.RepuestoId = detalle.RepuestoId;


                var resultado = await _detalleFacturaRepository.UpdateDetalleFacturaAsync(detalleOriginal);

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando el detalle de factura con ID {DetalleId}", detalle.Id);
                throw;
            }
        }
        public async Task<bool> DeleteDetalleFacturaAsync(int id)
        {
            var detalle = await _detalleFacturaRepository.GetDetalleFacturaByIdAsync(id);
            if (detalle == null)
                throw new Exception($"El detalle con ID {id} no existe.");

            var repuesto = await _repuestoRepository.GetRepuestoByIdAsync(detalle.RepuestoId);
            if (repuesto == null)
                throw new Exception($"El repuesto con ID {detalle.RepuestoId} no existe.");

            repuesto.StockActual += detalle.Cantidad;
            await _repuestoRepository.UpdateRepuestoAsync(repuesto);

            var resultado = await _detalleFacturaRepository.DeleteDetalleFacturaAsync(id);

            return resultado;
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