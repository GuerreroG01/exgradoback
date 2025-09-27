using System.Text.Json;
using ExGradoBack.Models;
using ExGradoBack.Data;

namespace ExGradoBack.Repositories
{
    public class ActividadRepository : IActividadRepository
    {
        private readonly AppDbContext _context;

        public ActividadRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task RegistrarAsync(string usuario, string accion, int facturaId, object? antes, object? despues)
        {
            var actividad = new ActividadFactura
            {
                FacturaId = facturaId,
                Usuario = usuario,
                Accion = accion,
                DatosAntes = antes != null ? JsonSerializer.Serialize(antes) : null,
                DatosDespues = despues != null ? JsonSerializer.Serialize(despues) : null
            };

            _context.ActividadFactura.Add(actividad);
            await _context.SaveChangesAsync();
        }
    }
}