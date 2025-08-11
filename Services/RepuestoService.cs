using ExGradoBack.Models;
using ExGradoBack.Repositories;
using System.ComponentModel.DataAnnotations;
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

        public async Task<Repuesto> CreateRepuestoAsync(Repuesto repuesto)
        {
            var context = new ValidationContext(repuesto);
            Validator.ValidateObject(repuesto, context, validateAllProperties: true);
            if (repuesto.VehiculoInfoId <= 0)
                    throw new ValidationException("ID de vehículo inválido.");
            if (repuesto.MarcaRepuestoId <= 0)
                throw new ValidationException("ID de marca de repuesto inválido.");
            var idVehiculo = repuesto.VehiculoInfoId;
            var vehiculoInfo = await _vehiculoInfoRepository.GetVehiculoInfoByIdAsync(idVehiculo);
            if (vehiculoInfo == null)
            {
                throw new KeyNotFoundException($"El vehículo con ID {idVehiculo} no ha sido encontrado.");
            }
            var idMarcaRepuesto = repuesto.MarcaRepuestoId;
            var marcaRepuesto = await _marcaRepuestoRepository.GetMarcaRepuestoByIdAsync(idMarcaRepuesto);
            if (marcaRepuesto == null)
            {
                throw new KeyNotFoundException($"La marca de repuesto con ID {idMarcaRepuesto} no ha sido encontrada.");
            }
            return await _repuestoRepository.CreateRepuestoAsync(repuesto);
        }

        public async Task<Repuesto> UpdateRepuestoAsync(Repuesto repuesto)
        {
            var context = new ValidationContext(repuesto);
            Validator.ValidateObject(repuesto, context, validateAllProperties: true);
            if (repuesto.VehiculoInfoId <= 0)
                    throw new ValidationException("ID de vehículo inválido.");
            if (repuesto.MarcaRepuestoId <= 0)
                throw new ValidationException("ID de marca de repuesto inválido.");
            var idVehiculo = repuesto.VehiculoInfoId;
            var vehiculoInfo = await _vehiculoInfoRepository.GetVehiculoInfoByIdAsync(idVehiculo);
            if (vehiculoInfo == null)
            {
                throw new KeyNotFoundException($"El vehículo con ID {idVehiculo} no ha sido encontrado.");
            }
            var idMarcaRepuesto = repuesto.MarcaRepuestoId;
            var marcaRepuesto = await _marcaRepuestoRepository.GetMarcaRepuestoByIdAsync(idMarcaRepuesto);
            if (marcaRepuesto == null)
            {
                throw new KeyNotFoundException($"La marca de repuesto con ID {idMarcaRepuesto} no ha sido encontrada.");
            }
            return await _repuestoRepository.UpdateRepuestoAsync(repuesto);
        }

        public async Task<bool> DeleteRepuestoAsync(int id)
        {
            return await _repuestoRepository.DeleteRepuestoAsync(id);
        }

        public async Task<bool> RepuestoExistsAsync(string nombre)
        {
            return await _repuestoRepository.RepuestoExistsAsync(nombre);
        }
    }
}