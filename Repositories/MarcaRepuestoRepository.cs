using ExGradoBack.Data;
using ExGradoBack.Models;
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
    }
}