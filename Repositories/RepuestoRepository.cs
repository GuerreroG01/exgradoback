using ExGradoBack.Models;
using ExGradoBack.DTOs;
using ExGradoBack.Data;
using Microsoft.EntityFrameworkCore;
namespace ExGradoBack.Repositories
{
    public class RepuestoRepository : IRepuestoRepository
    {
        private readonly AppDbContext _context;

        public RepuestoRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Repuesto>> GetRepuestosByNameAsync(string nombre)
        {
            return await _context.Repuesto
                .Include(r => r.VehiculoInfoIds)
                .Include(r => r.MarcaRepuesto)
                .Where(r => r.Nombre.Contains(nombre))
                .ToListAsync();
        }
        public async Task<Repuesto?> GetRepuestoByIdAsync(int id)
        {
            return await _context.Repuesto
                .Include(r => r.VehiculoInfoIds)
                .Include(r => r.MarcaRepuesto)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        public async Task<IEnumerable<Repuesto>> GetRepuestosByUbicacionAsync(string ubicacion)
        {
            return await _context.Repuesto
                .Include(r => r.VehiculoInfoIds)
                .Where(r => r.Ubicacion.Contains(ubicacion))
                .ToListAsync();
        }

        public async Task<Repuesto> CreateRepuestoAsync(Repuesto repuesto)
        {
            _context.Repuesto.Add(repuesto);
            await _context.SaveChangesAsync();
            return repuesto;
        }

        public async Task<Repuesto> UpdateRepuestoAsync(Repuesto repuesto)
        {
            _context.Repuesto.Update(repuesto);
            await _context.SaveChangesAsync();
            return repuesto;
        }

        public async Task<bool> DeleteRepuestoAsync(int id)
        {
            var repuesto = await GetRepuestoByIdAsync(id);
            if (repuesto == null) return false;

            _context.Repuesto.Remove(repuesto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RepuestoExistsAsync(int id)
        {
            return await _context.Repuesto.AnyAsync(r => r.Id == id);
        }
        public async Task<IEnumerable<string>> GetAllUbicacionesAsync()
        {
            return await _context.Repuesto
                                .Select(r => r.Ubicacion)
                                .Distinct()
                                .ToListAsync();
        }
        public async Task<IEnumerable<RepuestoStockDto>> GetTop10RepuestosMayorStockAsync()
        {
            return await _context.Repuesto
                .OrderByDescending(r => r.StockActual)
                .Take(10)
                .Select(r => new RepuestoStockDto
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    StockActual = r.StockActual
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<RepuestoStockDto>> GetTop10RepuestosMenorStockAsync()
        {
            return await _context.Repuesto
                .Where(r => r.StockActual > 0)
                .OrderBy(r => r.StockActual)
                .Take(10)
                .Select(r => new RepuestoStockDto
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    StockActual = r.StockActual
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<RepuestoReabastecimientoDto>> GetRepuestosSinStockAsync()
        {
            return await _context.Repuesto
                .Where(r => r.StockActual == 0)
                .Select(r => new RepuestoReabastecimientoDto
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    StockActual = r.StockActual,
                    FechaAbastecimiento = r.FechaAbastecimiento
                })
                .ToListAsync();
        }
    }
}