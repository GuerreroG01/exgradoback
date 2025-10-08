using ExGradoBack.Models;
using ExGradoBack.Repositories;
using System.ComponentModel.DataAnnotations;
using ExGradoBack.DTOs;
namespace ExGradoBack.Services
{
    public class RepuestoService : IRepuestoService
    {
        private readonly IRepuestoRepository _repuestoRepository;
        private readonly IMarcaRepuestoRepository _marcaRepuestoRepository;
        private readonly IVehiculoInfoRepository _vehiculoInfoRepository;

        public RepuestoService(IRepuestoRepository repuestoRepository, IMarcaRepuestoRepository marcaRepuestoRepository, IVehiculoInfoRepository vehiculoInfoRepository)
        {
            _repuestoRepository = repuestoRepository;
            _marcaRepuestoRepository = marcaRepuestoRepository;
            _vehiculoInfoRepository = vehiculoInfoRepository;
        }

        public async Task<IEnumerable<Repuesto>> GetRepuestosByNameAsync(string nombre)
        {
            return await _repuestoRepository.GetRepuestosByNameAsync(nombre);
        }

        public async Task<Repuesto?> GetRepuestoByIdAsync(int id)
        {
            return await _repuestoRepository.GetRepuestoByIdAsync(id);
        }
        public async Task<IEnumerable<Repuesto>> GetRepuestosByUbicacionAsync(string ubicacion)
        {
            return await _repuestoRepository.GetRepuestosByUbicacionAsync(ubicacion);
        }

        public async Task<Repuesto> CreateRepuestoAsync(RepuestoDto repuestoDto)
        {
            if (repuestoDto.MarcaRepuestoId <= 0)
                throw new ValidationException("ID de marca de repuesto inválido.");

            if (repuestoDto.VehiculoInfoIds == null || !repuestoDto.VehiculoInfoIds.Any())
                throw new ValidationException("Debe especificar al menos un vehículo compatible.");

            var marcaRepuesto = await _marcaRepuestoRepository.GetMarcaRepuestoByIdAsync(repuestoDto.MarcaRepuestoId);
            if (marcaRepuesto == null)
                throw new KeyNotFoundException($"La marca de repuesto con ID {repuestoDto.MarcaRepuestoId} no ha sido encontrada.");

            var vehiculos = new List<VehiculoInfo>();
            foreach (var vehiculoId in repuestoDto.VehiculoInfoIds)
            {
                var vehiculo = await _vehiculoInfoRepository.GetVehiculoInfoByIdAsync(vehiculoId);
                if (vehiculo == null)
                    throw new KeyNotFoundException($"El vehículo con ID {vehiculoId} no ha sido encontrado.");
                vehiculos.Add(vehiculo);
            }

            var repuesto = new Repuesto
            {
                Nombre = repuestoDto.Nombre,
                Descripcion = repuestoDto.Descripcion,
                PrecioUnitario = repuestoDto.PrecioUnitario,
                PrecioProveedor = repuestoDto.PrecioProveedor,
                StockActual = repuestoDto.StockActual,
                StockMinimo = repuestoDto.StockMinimo,
                FechaAbastecimiento = repuestoDto.FechaAbastecimiento,
                Ubicacion = repuestoDto.Ubicacion,
                MarcaRepuestoId = repuestoDto.MarcaRepuestoId,
                VehiculoInfoIds = vehiculos
            };

            return await _repuestoRepository.CreateRepuestoAsync(repuesto);
        }

        public async Task<Repuesto> UpdateRepuestoAsync(int id, RepuestoDto repuestodto)
        {
            var repuestoExistente = await _repuestoRepository.GetRepuestoByIdAsync(id);
            if (repuestoExistente == null)
                throw new KeyNotFoundException($"Repuesto con ID {id} no encontrado.");

            var marcaRepuesto = await _marcaRepuestoRepository.GetMarcaRepuestoByIdAsync(repuestodto.MarcaRepuestoId);
            if (marcaRepuesto == null)
                throw new KeyNotFoundException($"Marca de repuesto con ID {repuestodto.MarcaRepuestoId} no encontrada.");

            if (repuestodto.VehiculoInfoIds == null || !repuestodto.VehiculoInfoIds.Any())
                throw new ValidationException("Debe especificar al menos un vehículo compatible.");

            var vehiculos = new List<VehiculoInfo>();
            foreach (var vehiculoId in repuestodto.VehiculoInfoIds.Distinct())
            {
                var vehiculo = await _vehiculoInfoRepository.GetVehiculoInfoByIdAsync(vehiculoId);
                if (vehiculo == null)
                    throw new KeyNotFoundException($"Vehículo con ID {vehiculoId} no encontrado.");
                vehiculos.Add(vehiculo);
            }
            repuestoExistente.Nombre = repuestodto.Nombre;
            repuestoExistente.Descripcion = repuestodto.Descripcion;
            repuestoExistente.PrecioUnitario = repuestodto.PrecioUnitario;
            repuestoExistente.PrecioProveedor = repuestodto.PrecioProveedor;
            repuestoExistente.StockActual = repuestodto.StockActual;
            repuestoExistente.StockMinimo = repuestodto.StockMinimo;
            repuestoExistente.FechaAbastecimiento = repuestodto.FechaAbastecimiento;
            repuestoExistente.Ubicacion = repuestodto.Ubicacion;
            repuestoExistente.MarcaRepuestoId = repuestodto.MarcaRepuestoId;
            repuestoExistente.VehiculoInfoIds = vehiculos;

            return await _repuestoRepository.UpdateRepuestoAsync(repuestoExistente);
        }

        public async Task<bool> DeleteRepuestoAsync(int id)
        {
            return await _repuestoRepository.DeleteRepuestoAsync(id);
        }

        public async Task<bool> RepuestoExistsAsync(int id)
        {
            return await _repuestoRepository.RepuestoExistsAsync(id);
        }
        public async Task<IEnumerable<string>> GetAllUbicacionesAsync()
        {
            return await _repuestoRepository.GetAllUbicacionesAsync();
        }
        public async Task<IEnumerable<RepuestoStockDto>> GetTop10RepuestosMayorStockAsync()
        {
            var repuestos = await _repuestoRepository.GetTop10RepuestosMayorStockAsync();

            if (!repuestos.Any())
                throw new InvalidOperationException("No se encontraron repuestos con stock.");

            return repuestos;
        }
        public async Task<IEnumerable<RepuestoStockDto>> GetTop10RepuestosMenorStockAsync()
        {
            var repuestos = await _repuestoRepository.GetTop10RepuestosMenorStockAsync();

            if (!repuestos.Any())
                throw new InvalidOperationException("No se encontraron repuestos con stock bajo.");

            return repuestos;
        }
        public async Task<IEnumerable<RepuestoReabastecimientoDto>> GetRepuestosSinStockAsync()
        {
            var repuestos = await _repuestoRepository.GetRepuestosSinStockAsync();

            if (!repuestos.Any())
                throw new InvalidOperationException("No hay repuestos que requieran reabastecimiento.");

            return repuestos;
        }
    }
}