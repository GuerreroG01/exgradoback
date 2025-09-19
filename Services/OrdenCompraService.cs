using ExGradoBack.Models;
using ExGradoBack.Repositories;

namespace ExGradoBack.Services
{
    public class OrdenCompraService : IOrdenCompraService
    {
        private readonly IOrdenCompraRepository _repository;

        public OrdenCompraService(IOrdenCompraRepository repository)
        {
            _repository = repository;
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
            ValidarOrdenCompra(orden, isUpdate: false);

            return await _repository.CreateOrdenAsync(orden);
        }

        public async Task<OrdenCompra> UpdateOrdenAsync(OrdenCompra orden)
        {
            ValidarOrdenCompra(orden, isUpdate: true);

            var existente = await _repository.GetOrdenByIdAsync(orden.Id);
            if (existente == null)
                throw new KeyNotFoundException("La orden que intenta actualizar no existe.");

            return await _repository.UpdateOrdenAsync(orden);
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

            if (orden.Detalles == null || !orden.Detalles.Any())
                throw new ArgumentException("La orden debe tener al menos un detalle.");

            foreach (var detalle in orden.Detalles)
            {
                if (detalle.RepuestoId <= 0)
                    throw new ArgumentException("Cada detalle debe tener un ID de repuesto válido.");

                if (detalle.Cantidad <= 0)
                    throw new ArgumentException("La cantidad de cada repuesto debe ser mayor a cero.");

                if (detalle.PrecioUnitario < 0)
                    throw new ArgumentException("El precio unitario no puede ser negativo.");
            }
        }
    }
}