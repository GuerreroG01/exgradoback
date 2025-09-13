using ExGradoBack.Data;
using ExGradoBack.Models;
using Microsoft.EntityFrameworkCore;

namespace ExGradoBack.Repositories
{
    public class DetalleFacturaRepository : IDetalleFacturaRepository
    {
        private readonly AppDbContext _context;
        public DetalleFacturaRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<DetalleFactura>> GetDetalleFacturaByIdFacturaAsync(int idFactura)
        {
            return await _context.DetalleFactura
                .Where(d => d.FacturaId == idFactura)
                .ToListAsync();
        }
        public async Task<DetalleFactura> CreateDetalleFacturaAsync(DetalleFactura detalle)
        {
            _context.DetalleFactura.Add(detalle);
            await _context.SaveChangesAsync();
            return detalle;
        }
        public async Task<DetalleFactura> UpdateDetalleFacturaAsync(DetalleFactura detalle)
        {
            _context.DetalleFactura.Update(detalle);
            await _context.SaveChangesAsync();
            return detalle;
        }
        public async Task<bool> DeleteDetalleFacturaAsync(int id)
        {
            var detalle = await _context.DetalleFactura.FindAsync(id);
            if (detalle == null)
            {
                return false;
            }
            _context.DetalleFactura.Remove(detalle);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DetalleFacturaExistsAsync(int id)
        {
            return await _context.DetalleFactura.AnyAsync(d => d.Id == id);
        }
    }
}