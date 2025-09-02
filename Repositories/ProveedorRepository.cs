using ExGradoBack.Data;
using ExGradoBack.Models;
using Microsoft.EntityFrameworkCore;
using ExGradoBack.DTOs;

namespace ExGradoBack.Repositories
{
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly AppDbContext _context;

        public ProveedorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<object>> GetProveedorsAsync(string? country, string? city, bool isMinInfo = false)
        {
            var query = _context.Proveedor.AsQueryable();

            if (!string.IsNullOrWhiteSpace(country))
            {
                query = query.Where(p => p.Pais == country);
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(p => p.Ciudad == city);
            }

            if (isMinInfo)
            {
                return await query
                    .Select(p => new ProveedorMinInfo
                    {
                        Id = p.Id,
                        Nombre = p.Nombre
                    })
                    .ToListAsync();
            }

            return await query.ToListAsync();
        }

        public async Task<Proveedor?> GetProveedorByIdAsync(int id)
        {
            return await _context.Proveedor.FindAsync(id);
        }

        public async Task<Proveedor> CreateProveedorAsync(Proveedor proveedor)
        {
            _context.Proveedor.Add(proveedor);
            await _context.SaveChangesAsync();
            return proveedor;
        }

        public async Task<Proveedor> UpdateProveedorAsync(Proveedor proveedor)
        {
            _context.Proveedor.Update(proveedor);
            await _context.SaveChangesAsync();
            return proveedor;
        }
        public async Task<bool> DeleteProveedorAsync(int id)
        {
            var proveedor = await _context.Proveedor.FindAsync(id);
            if (proveedor == null)
            {
                return false;
            }

            _context.Proveedor.Remove(proveedor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ProveedorExistsAsync(string nombre)
        {
            return await _context.Proveedor.AnyAsync(p => p.Nombre == nombre);
        }
        public async Task<IEnumerable<string>> GetCountryProvAsync()
        {
            return await _context.Proveedor
                .Where(p => !string.IsNullOrEmpty(p.Pais))
                .Select(p => p.Pais!)
                .Distinct()
                .ToListAsync();
        }
        public async Task<IEnumerable<string>> GetCityProvAsync(string country)
        {
            return await _context.Proveedor
                .Where(p => p.Pais == country && !string.IsNullOrEmpty(p.Ciudad))
                .Select(p => p.Ciudad!)
                .Distinct()
                .ToListAsync();
        }
    }
}