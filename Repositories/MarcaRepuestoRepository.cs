using ExGradoBack.Data;
using ExGradoBack.Models;
using ExGradoBack.DTOs;
using Microsoft.EntityFrameworkCore;
namespace ExGradoBack.Repositories
{
    public class MarcaRepuestoRepository : IMarcaRepuestoRepository
    {
        private readonly AppDbContext _context;

        public MarcaRepuestoRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<MarcaRepuesto>> GetMarcaRepuestoPorCalificacionAsync(double calificacion)
        {
            var marcas = await _context.MarcaRepuesto
                .Where(m => m.Calificacion.HasValue &&
                            m.Calificacion.Value >= calificacion &&
                            m.Calificacion.Value < calificacion + 1)
                .ToListAsync();

            if (marcas == null || marcas.Count == 0)
                throw new InvalidOperationException($"No se encontraron marcas de repuesto con calificación alrededor de '{calificacion}'.");

            return marcas;
        }
        public async Task<MarcaRepuesto?> GetByNameAsync(string nombre)
        {
            return await _context.MarcaRepuesto
                .FirstOrDefaultAsync(m => m.Nombre.ToLower() == nombre.ToLower());
        }

        public async Task<MarcaRepuesto?> GetMarcaRepuestoByIdAsync(int id)
        {
            return await _context.MarcaRepuesto.FindAsync(id);
        }

        public async Task<MarcaRepuesto> CreateMarcaRepuestoAsync(MarcaRepuesto marcaRepuesto)
        {
            _context.MarcaRepuesto.Add(marcaRepuesto);
            await _context.SaveChangesAsync();
            return marcaRepuesto;
        }

        public async Task<MarcaRepuesto> UpdateMarcaRepuestoAsync(MarcaRepuesto marcaRepuesto)
        {
            _context.MarcaRepuesto.Update(marcaRepuesto);
            await _context.SaveChangesAsync();
            return marcaRepuesto;
        }

        public async Task<bool> DeleteMarcaRepuestoAsync(int id)
        {
            var marcaRepuesto = await GetMarcaRepuestoByIdAsync(id);
            if (marcaRepuesto == null) return false;

            _context.MarcaRepuesto.Remove(marcaRepuesto);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarcaRepuestoExistsAsync(string nombre)
        {
            return await _context.MarcaRepuesto.AnyAsync(m => m.Nombre == nombre);
        }
        
        public async Task<List<MarcaRepuestoUsadaDto>> ObtenerMarcasRepuestoMasUsadasAsync()
        {
            var resultado = await _context.DetalleFactura
                .Include(df => df.Repuesto)
                    .ThenInclude(r => r != null ? r.MarcaRepuesto : null)
                .GroupBy(df => new { df.Repuesto!.MarcaRepuestoId, df.Repuesto.MarcaRepuesto!.Nombre })
                .Select(g => new MarcaRepuestoUsadaDto
                {
                    MarcaRepuestoId = g.Key.MarcaRepuestoId,
                    NombreMarca = g.Key.Nombre,
                    TotalVendidos = g.Sum(df => df.Cantidad)
                })
                .OrderByDescending(m => m.TotalVendidos)
                .Take(5)
                .ToListAsync();

            return resultado;
        }
    }
}