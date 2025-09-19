using ExGradoBack.Data;
using ExGradoBack.Models;
using Microsoft.EntityFrameworkCore;
namespace ExGradoBack.Repositories
{
    public class OrdenCompraRepository : IOrdenCompraRepository
    {
        private readonly AppDbContext _context;
        public OrdenCompraRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<int>> GetAniosConOrdenesAsync()
        {
            return await _context.OrdenCompra
                .Select(o => o.Fecha.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToListAsync();
        }
        public async Task<IEnumerable<int>> GetMesesConOrdenesAsync(int anio)
        {
            return await _context.OrdenCompra
                .Where(o => o.Fecha.Year == anio)
                .Select(o => o.Fecha.Month)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
        }
        public async Task<IEnumerable<int>> GetDiasConOrdenesAsync(int anio, int mes)
        {
            return await _context.OrdenCompra
                .Where(o => o.Fecha.Year == anio && o.Fecha.Month == mes)
                .Select(o => o.Fecha.Day)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();
        }
        public async Task<IEnumerable<OrdenCompra>> GetOrdenesPorDiaAsync(int anio, int mes, int dia)
        {
            return await _context.OrdenCompra
                .Where(o => o.Fecha.Year == anio && o.Fecha.Month == mes && o.Fecha.Day == dia)
                .OrderBy(o => o.Fecha)
                .ToListAsync();
        }
        public async Task<OrdenCompra?> GetOrdenByIdAsync(int id)
        {
            return await _context.OrdenCompra
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<OrdenCompra> CreateOrdenAsync(OrdenCompra orden)
        {
            _context.OrdenCompra.Add(orden);
            await _context.SaveChangesAsync();
            return orden;
        }
        public async Task<OrdenCompra> UpdateOrdenAsync(OrdenCompra orden)
        {
            _context.OrdenCompra.Update(orden);
            await _context.SaveChangesAsync();
            return orden;
        }
        public async Task<bool> DeleteOrdenAsync(int id)
        {
            var orden = await _context.OrdenCompra.FindAsync(id);
            if (orden == null)
                return false;

            _context.OrdenCompra.Remove(orden);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> OrdenExistsAsync(int id)
        {
            return await _context.OrdenCompra.AnyAsync(o => o.Id == id);
        }
    }
}