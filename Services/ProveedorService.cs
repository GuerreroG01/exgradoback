using ExGradoBack.Models;
using ExGradoBack.DTOs;
using ExGradoBack.Repositories;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ExGradoBack.Services
{
    public class ProveedorService : IProveedorService
    {
        private readonly IProveedorRepository _proveedorRepository;

        public ProveedorService(IProveedorRepository proveedorRepository)
        {
            _proveedorRepository = proveedorRepository;
        }

        public async Task<IEnumerable<Proveedor>> GetProveedorInfoFullAsync(string? country, string? city)
        {
            var result = await _proveedorRepository.GetProveedorsAsync(country, city, isMinInfo: false);
            return result.Cast<Proveedor>();
        }
        public async Task<IEnumerable<ProveedorMinInfo>> GetProveedorInfoMinAsync(string? country, string? city)
        {
            var result = await _proveedorRepository.GetProveedorsAsync(country, city, isMinInfo: true);
            return result.Cast<ProveedorMinInfo>();
        }

        public async Task<Proveedor?> GetProveedorByIdAsync(int id)
        {
            return await _proveedorRepository.GetProveedorByIdAsync(id);
        }

        public async Task<Proveedor> CreateProveedorAsync(Proveedor proveedor)
        {
            ValidateProveedor(proveedor);
            return await _proveedorRepository.CreateProveedorAsync(proveedor);
        }

        public async Task<Proveedor> UpdateProveedorAsync(Proveedor proveedor)
        {
            ValidateProveedor(proveedor);

            var existingProveedor = await _proveedorRepository.GetProveedorByIdAsync(proveedor.Id);
            if (existingProveedor == null)
            {
                throw new KeyNotFoundException($"Proveedor con ID {proveedor.Id} no encontrado.");
            }

            existingProveedor.Nombre = proveedor.Nombre;
            existingProveedor.Documento = proveedor.Documento;
            existingProveedor.NombreContacto = proveedor.NombreContacto;
            existingProveedor.Telefono = proveedor.Telefono;
            existingProveedor.Email = proveedor.Email;
            existingProveedor.Pais = proveedor.Pais;
            existingProveedor.Ciudad = proveedor.Ciudad;
            existingProveedor.Direccion = proveedor.Direccion;
            existingProveedor.Notas = proveedor.Notas;

            await _proveedorRepository.SaveChangesAsync();

            return existingProveedor;
        }
        public async Task<bool> DeleteProveedorAsync(int id)
        {
            var existingProveedor = await _proveedorRepository.GetProveedorByIdAsync(id);
            if (existingProveedor == null)
            {
                throw new KeyNotFoundException($"Proveedor con ID {id} no encontrado.");
            }

            try
            {
                return await _proveedorRepository.DeleteProveedorAsync(id);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("No se puede eliminar el proveedor porque tiene órdenes de compra asociadas.", ex);
            }
        }
        public async Task<bool> ProveedorExistsAsync(string nombre)
        {
            return await _proveedorRepository.ProveedorExistsAsync(nombre);
        }
        public async Task<IEnumerable<string>> GetCountryProvAsync()
        {
            return await _proveedorRepository.GetCountryProvAsync();
        }
        public async Task<IEnumerable<string>> GetCityProvAsync(string country)
        {
            return await _proveedorRepository.GetCityProvAsync(country);
        }
        public async Task<IEnumerable<ProveedorMinInfo>> AutocompletarProveedoresAsync(string nombre)
        {
            return await _proveedorRepository.BuscarProveedoresPorNombreAsync(nombre);
        }

        private void ValidateProveedor(Proveedor proveedor)
        {
            var context = new ValidationContext(proveedor, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(proveedor, context, results, true))
            {
                var errors = string.Join("; ", results.Select(r => r.ErrorMessage));
                throw new ValidationException($"La validación falló: {errors}");
            }
        }
    }
}