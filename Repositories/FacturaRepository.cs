using ExGradoBack.Data;
using ExGradoBack.Models;
using Microsoft.EntityFrameworkCore;

namespace ExGradoBack.Repositories
{
    public class FacturaRepository : IFacturaRepository
    {
        private readonly AppDbContext _context;
        public FacturaRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<int>> GetAniosConFacturasAsync()
        {
            return await _context.Factura
                .Select(f => f.Fecha.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToListAsync();
        }
        public async Task<IEnumerable<int>> GetMesesConFacturasAsync(int anio)
        {
            return await _context.Factura
                .Where(f => f.Fecha.Year == anio)
                .Select(f => f.Fecha.Month)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
        }
        public async Task<IEnumerable<int>> GetDiasConFacturasAsync(int anio, int mes)
        {
            return await _context.Factura
                .Where(f => f.Fecha.Year == anio && f.Fecha.Month == mes)
                .Select(f => f.Fecha.Day)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();
        }
        public async Task<IEnumerable<Factura>> GetFacturasPorDiaAsync(int anio, int mes, int dia)
        {
            return await _context.Factura
                .Where(f => f.Fecha.Year == anio && f.Fecha.Month == mes && f.Fecha.Day == dia)
                .OrderBy(f => f.Fecha)
                .ToListAsync();
        }
        public async Task<Factura?> GetFacturaByIdAsync(int id)
        {
            return await _context.Factura.FindAsync(id);
        }
        public async Task<Factura> CreateFacturaAsync(Factura factura)
        {
            _context.Factura.Add(factura);
            await _context.SaveChangesAsync();
            return factura;
        }
        public async Task<Factura> UpdateFacturaAsync(Factura factura)
        {
            _context.Factura.Update(factura);
            await _context.SaveChangesAsync();
            return factura;
        }
        public async Task<bool> DeleteFacturaAsync(int id)
        {
            var factura = await _context.Factura.FindAsync(id);
            if (factura == null)
            {
                return false;
            }
            _context.Factura.Remove(factura);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> FacturaExistsAsync(int id)
        {
            return await _context.Factura.AnyAsync(c => c.Id == id);
        }
    }
}