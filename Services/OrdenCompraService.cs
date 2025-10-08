using ExGradoBack.Models;
using Microsoft.Extensions.Logging;
using ExGradoBack.Repositories;
using ExGradoBack.DTOs;

namespace ExGradoBack.Services
{
    public class OrdenCompraService : IOrdenCompraService
    {
        private readonly IOrdenCompraRepository _repository;
        private readonly ILogger<OrdenCompraService> _logger;

        public OrdenCompraService(IOrdenCompraRepository repository, ILogger<OrdenCompraService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<int>> GetAniosConOrdenesAsync()
        {
            return await _repository.GetAniosConOrdenesAsync();
        }

        public async Task<IEnumerable<int>> GetMesesConOrdenesAsync(int anio)
        {
            if (anio <= 0)
                throw new ArgumentException("El año debe ser un número válido.");

            return await _repository.GetMesesConOrdenesAsync(anio);
        }

        public async Task<IEnumerable<int>> GetDiasConOrdenesAsync(int anio, int mes)
        {
            if (anio <= 0 || mes <= 0 || mes > 12)
                throw new ArgumentException("Fecha no válida para la búsqueda de órdenes.");

            return await _repository.GetDiasConOrdenesAsync(anio, mes);
        }

        public async Task<IEnumerable<OrdenCompra>> GetOrdenesPorDiaAsync(int anio, int mes, int dia)
        {
            if (anio <= 0 || mes <= 0 || mes > 12 || dia <= 0 || dia > 31)
                throw new ArgumentException("Fecha no válida.");

            return await _repository.GetOrdenesPorDiaAsync(anio, mes, dia);
        }

        public async Task<OrdenCompra?> GetOrdenByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID no válido.");

            return await _repository.GetOrdenByIdAsync(id);
        }

        public async Task<OrdenCompra> CreateOrdenAsync(OrdenCompra orden)
        {
            _logger.LogInformation("Creando orden de compra. Datos: {@orden}", orden);

            try
            {
                ValidarOrdenCompra(orden, isUpdate: false);
                var resultado = await _repository.CreateOrdenAsync(orden);
                _logger.LogInformation("Orden creada con éxito. ID: {Id}", resultado.Id);
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la orden de compra.");
                throw;
            }
        }

        public async Task<OrdenCompra> UpdateOrdenAsync(OrdenCompra orden)
        {
            var existente = await _repository.GetOrdenByIdAsync(orden.Id);
            if (existente == null)
            {
                throw new KeyNotFoundException("La orden que intenta actualizar no existe.");
            }

            existente.Fecha = orden.Fecha;
            existente.ProveedorId = orden.ProveedorId;
            existente.Total = orden.Total;
            existente.Estado = orden.Estado;
            existente.Solicitante = orden.Solicitante;

            await _repository.SaveChangesAsync();

            return existente;
        }

        public async Task<bool> DeleteOrdenAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID no válido para eliminar.");

            var existente = await _repository.GetOrdenByIdAsync(id);
            if (existente == null)
                throw new KeyNotFoundException("La orden que intenta eliminar no existe.");

            return await _repository.DeleteOrdenAsync(id);
        }

        public async Task<bool> OrdenExistsAsync(int id)
        {
            if (id <= 0)
                return false;

            return await _repository.OrdenExistsAsync(id);
        }
        public async Task<List<OrdenCompra>> GetOrdenesPendientesAsync()
        {
            return await _repository.GetOrdenesPendientesAsync();
        }

        private void ValidarOrdenCompra(OrdenCompra orden, bool isUpdate = false)
        {
            if (orden == null)
                throw new ArgumentNullException(nameof(orden), "La orden no puede ser nula.");

            if (isUpdate && orden.Id <= 0)
                throw new ArgumentException("ID inválido para actualización.");

            if (orden.Fecha == default)
                throw new ArgumentException("La fecha de la orden es obligatoria.");

            if (orden.ProveedorId <= 0)
                throw new ArgumentException("Debe especificar un proveedor válido.");

            if (orden.Total < 0)
                throw new ArgumentException("El total no puede ser negativo.");
        }
        public async Task<OrdenCompraResumenDTO?> GetResumenOrdenesPorFechaAsync(DateTime fecha)
        {
            return await _repository.GetResumenOrdenesPorFechaAsync(fecha);
        }
        public async Task<List<OrdenCompraDto>> GetOrdenesEntregadasAsync()
        {
            var ordenes = await _repository.GetOrdenesEntregadasAsync();

            if (ordenes == null || !ordenes.Any())
            {
                throw new InvalidOperationException("No se encontraron órdenes de compra con estado 'Entregado'.");
            }

            return ordenes;
        }
    }
}