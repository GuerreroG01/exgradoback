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
        public Task<List<MarcaRepuesto>> GetMarcaRepuestoPorCalificacionAsync(double calificacion)
            => _marcaRepuestoRepository.GetMarcaRepuestoPorCalificacionAsync(calificacion);
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

            if (!marcaRepuesto.Calificacion.HasValue)
            {
                marcaRepuesto.Calificacion = 0;
            }

            return await _marcaRepuestoRepository.CreateMarcaRepuestoAsync(marcaRepuesto);
        }
        public async Task<MarcaRepuesto> UpdateMarcaRepuestoAsync(MarcaRepuesto marcaRepuesto)
        {
            if (string.IsNullOrWhiteSpace(marcaRepuesto.Nombre))
            {
                throw new ArgumentException("El nombre de la marca no puede ser nulo o vacío.", nameof(marcaRepuesto.Nombre));
            }

            var marcaOriginal = await _marcaRepuestoRepository.GetMarcaRepuestoByIdAsync(marcaRepuesto.Id);

            if (marcaOriginal == null)
                throw new ArgumentException("Marca no encontrada.", nameof(marcaRepuesto.Id));

            if (marcaOriginal.Nombre != marcaRepuesto.Nombre)
            {
                if (await MarcaRepuestoExistsAsync(marcaRepuesto.Nombre))
                {
                    throw new ArgumentException("Ya existe una marca con ese nombre.", nameof(marcaRepuesto.Nombre));
                }
            }
            marcaOriginal.Nombre = marcaRepuesto.Nombre;
            marcaOriginal.Calificacion = marcaRepuesto.Calificacion;

            return await _marcaRepuestoRepository.UpdateMarcaRepuestoAsync(marcaOriginal);
        }

        public Task<bool> DeleteMarcaRepuestoAsync(int id)
            => _marcaRepuestoRepository.DeleteMarcaRepuestoAsync(id);

        public Task<bool> MarcaRepuestoExistsAsync(string nombre) 
            => _marcaRepuestoRepository.MarcaRepuestoExistsAsync(nombre);
    }
}