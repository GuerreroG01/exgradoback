using ExGradoBack.Data;
using ExGradoBack.Models;
using Microsoft.EntityFrameworkCore;

namespace ExGradoBack.Repositories
{
    public class DetalleOrdenRepository : IDetalleOrdenRepository
    {
        private readonly AppDbContext _context;

        public DetalleOrdenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DetalleOrdenCompra> CreateDetalleOrdenAsync(DetalleOrdenCompra detalle)
        {
            _context.DetalleOrdenCompra.Add(detalle);
            await _context.SaveChangesAsync();
            return detalle;
        }
        public async Task<DetalleOrdenCompra> UpdateDetalleOrdenAsync(DetalleOrdenCompra detalle)
        {
            _context.Entry(detalle).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return detalle;
        }
        public async Task<bool> DeleteDetalleOrdenAsync(int id)
        {
            var detalle = await _context.DetalleOrdenCompra.FindAsync(id);
            if (detalle == null)
            {
                return false;
            }

            _context.DetalleOrdenCompra.Remove(detalle);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DetalleOrdenExistsAsync(int id)
        {
            return await _context.DetalleOrdenCompra.AnyAsync(e => e.Id == id);
        }

        public async Task<List<DetalleOrdenCompra>> GetDetalleOrdenByIdOrdenAsync(int idOrden)
        {
            return await _context.DetalleOrdenCompra
                                 .Where(d => d.OrdenCompraId == idOrden)
                                 .ToListAsync();
        }
    }
}