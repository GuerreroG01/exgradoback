using ExGradoBack.Models;
using ExGradoBack.Repositories;

namespace ExGradoBack.Services
{
    public class MarcaRepuestoService : IMarcaRepuestoService
    {
        private readonly IMarcaRepuestoRepository _marcaRepuestoRepository;

        public MarcaRepuestoService(IMarcaRepuestoRepository marcaRepuestoRepository)
        {
            _marcaRepuestoRepository = marcaRepuestoRepository;
        }
        public Task<MarcaRepuesto> GetMarcaRepuestoAsync(string nombre)
            => _marcaRepuestoRepository.GetMarcaRepuestoAsync(nombre);

        public Task<MarcaRepuesto?> GetMarcaRepuestoByIdAsync(int id)
            => _marcaRepuestoRepository.GetMarcaRepuestoByIdAsync(id);

        public async Task<MarcaRepuesto> CreateMarcaRepuestoAsync(MarcaRepuesto marcaRepuesto)
        {
            if (string.IsNullOrWhiteSpace(marcaRepuesto.Nombre))
            {
                throw new ArgumentException("El nombre de la marca no puede ser nulo o vacío.", nameof(marcaRepuesto.Nombre));
            }
            if (await MarcaRepuestoExistsAsync(marcaRepuesto.Nombre))
            {
                throw new ArgumentException("Ya existe una marca con ese nombre.", nameof(marcaRepuesto.Nombre));
            }
            return await _marcaRepuestoRepository.CreateMarcaRepuestoAsync(marcaRepuesto);
        }
        public async Task<MarcaRepuesto> UpdateMarcaRepuestoAsync(MarcaRepuesto marcaRepuesto)
        {
            if (string.IsNullOrWhiteSpace(marcaRepuesto.Nombre))
            {
                throw new ArgumentException("El nombre de la marca no puede ser nulo o vacío.", nameof(marcaRepuesto.Nombre));
            }
            if (await MarcaRepuestoExistsAsync(marcaRepuesto.Nombre))
            {
                throw new ArgumentException("Ya existe una marca con ese nombre.", nameof(marcaRepuesto.Nombre));
            }
            return await _marcaRepuestoRepository.UpdateMarcaRepuestoAsync(marcaRepuesto);
        }

        public Task<bool> DeleteMarcaRepuestoAsync(int id)
            => _marcaRepuestoRepository.DeleteMarcaRepuestoAsync(id);

        public Task<bool> MarcaRepuestoExistsAsync(string nombre) 
            => _marcaRepuestoRepository.MarcaRepuestoExistsAsync(nombre);
    }
}