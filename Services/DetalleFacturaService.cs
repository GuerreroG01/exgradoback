using ExGradoBack.Models;
using ExGradoBack.Repositories;
using ExGradoBack.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ExGradoBack.Services
{
    public class DetalleFacturaService : IDetalleFacturaService
    {
        private readonly IDetalleFacturaRepository _detalleFacturaRepository;
        private readonly IFacturaRepository _facturaRepository;
        private readonly IRepuestoRepository _repuestoRepository;
        private readonly ILogger<DetalleFacturaService> _logger;
        private readonly IHubContext<StockHub> _hubContext;
        public DetalleFacturaService(IDetalleFacturaRepository detalleFacturaRepository, IFacturaRepository facturaRepository, IRepuestoRepository repuestoRepository, ILogger<DetalleFacturaService> logger, IHubContext<StockHub> hubContext)
        {
            _detalleFacturaRepository = detalleFacturaRepository;
            _facturaRepository = facturaRepository;
            _repuestoRepository = repuestoRepository;
            _logger = logger;
            _hubContext = hubContext;
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

            await _hubContext.Clients.All.SendAsync("ActualizarStock", new
            {
                id = repuesto.Id,
                nuevoStock = repuesto.StockActual
            });

            return await _detalleFacturaRepository.CreateDetalleFacturaAsync(detalle);
        }
        public async Task<DetalleFactura> UpdateSingleAsync(DetalleFactura detalle)
        {
            var detalleOriginal = await _detalleFacturaRepository.GetDetalleFacturaByIdAsync(detalle.Id);
            if (detalleOriginal == null)
                throw new Exception("El detalle de factura a actualizar no existe");

            var repuesto = await _repuestoRepository.GetRepuestoByIdAsync(detalle.RepuestoId);
            if (repuesto == null)
                throw new Exception($"El repuesto con Id {detalle.RepuestoId} no existe");

            int diferencia = detalle.Cantidad - detalleOriginal.Cantidad;

            if (repuesto.StockActual < diferencia)
                throw new Exception("No hay suficiente stock para realizar esta modificación");

            repuesto.StockActual -= diferencia;
            await _repuestoRepository.UpdateRepuestoAsync(repuesto);

            await _hubContext.Clients.All.SendAsync("ActualizarStock", new
            {
                id = repuesto.Id,
                nuevoStock = repuesto.StockActual
            });

            detalleOriginal.Cantidad = detalle.Cantidad;
            detalleOriginal.PrecioUnitario = detalle.PrecioUnitario;
            detalleOriginal.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;
            detalleOriginal.RepuestoId = detalle.RepuestoId;

            return await _detalleFacturaRepository.UpdateDetalleFacturaAsync(detalleOriginal);
        }
        public async Task UpdateDetalleFacturaAsync(int facturaId, ICollection<DetalleFactura> detallesNuevos)
        {
            var detallesExistentes = await _detalleFacturaRepository.GetDetalleFacturaByIdFacturaAsync(facturaId)
                                    ?? new List<DetalleFactura>();

            var detallesAEliminar = detallesExistentes
                .Where(d => !detallesNuevos.Any(n => n.Id == d.Id))
                .ToList();

            foreach (var d in detallesAEliminar)
            {
                await DeleteDetalleFacturaAsync(d.Id);
            }

            foreach (var detalle in detallesNuevos)
            {
                detalle.FacturaId = facturaId;
                if (detalle.Id > 0)
                {
                    var existeEnDB = detallesExistentes.Any(d => d.Id == detalle.Id);
                    if (existeEnDB)
                    {
                        await UpdateSingleAsync(detalle);
                    }
                    else
                    {
                        detalle.Id = 0;
                        await CreateDetalleFacturaAsync(detalle);
                    }
                }
                else
                {
                    await CreateDetalleFacturaAsync(detalle);
                }
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