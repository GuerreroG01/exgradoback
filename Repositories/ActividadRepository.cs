using System.Text.Json;
using ExGradoBack.Models;
using ExGradoBack.Data;
using Microsoft.EntityFrameworkCore;
using ExGradoBack.DTOs;

namespace ExGradoBack.Repositories
{
    public class ActividadRepository : IActividadRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ActividadRepository> _logger;

        public ActividadRepository(AppDbContext context, ILogger<ActividadRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task RegistrarAsync(string usuario, string accion, int facturaId, object? antes, object? despues)
        {
            _logger.LogInformation(" Repo : Registrando actividad: Usuario={usuario}, Accion={accion}, FacturaId={facturaId}, Antes={antes}, Despues={despues}",
                usuario, accion, facturaId,
                antes != null ? JsonSerializer.Serialize(antes) : "Algo malo",
                despues != null ? JsonSerializer.Serialize(despues) : "Algo malo");
            var actividad = new ActividadFactura
            {
                FacturaId = facturaId,
                Usuario = usuario,
                Accion = accion,
                DatosAntes = antes != null ? JsonSerializer.Serialize(antes) : null,
                DatosDespues = despues != null ? JsonSerializer.Serialize(despues) : null
            };
            _logger.LogInformation("En el repositorio: Registrando actividad: {@actividad}", actividad);

            _context.ActividadFactura.Add(actividad);
            await _context.SaveChangesAsync();
        }
        public async Task<List<ActividadResumenDto>> ObtenerActividadesAsync(string usuario, string? accion = null, int? meses = null)
        {
            DateTime? fechaInicio = null;
            if (meses.HasValue)
            {
                fechaInicio = DateTime.UtcNow.AddMonths(-meses.Value);
            }

            var query = _context.ActividadFactura.AsQueryable();

            query = query.Where(a => a.Usuario == usuario);

            if (!string.IsNullOrEmpty(accion))
            {
                query = query.Where(a => a.Accion == accion);
            }

            if (fechaInicio.HasValue)
            {
                query = query.Where(a => a.Fecha >= fechaInicio.Value);
            }

            return await query
                .OrderByDescending(a => a.Fecha)
                .Select(a => new ActividadResumenDto
                {
                    Id = a.Id,
                    FacturaId = a.FacturaId,
                    Accion = a.Accion,
                    Fecha = a.Fecha
                })
                .ToListAsync();
        }
        public async Task<ActividadFactura?> GetActividadByIdAsync(int id)
        {
            return await _context.ActividadFactura.FindAsync(id);
        }
        public async Task LimpiarActividadesAsync()
        {
            var usuarios = await _context.ActividadFactura
                .Select(a => a.Usuario)
                .Distinct()
                .ToListAsync();

            foreach (var usuario in usuarios)
            {
                var fechaMaxima = await _context.ActividadFactura
                    .Where(a => a.Usuario == usuario)
                    .MaxAsync(a => (DateTime?)a.Fecha);

                if (fechaMaxima == null)
                    continue;

                var fechaCorteUsuario = fechaMaxima.Value.AddYears(-1);

                await _context.ActividadFactura
                    .Where(a => a.Usuario == usuario && a.Fecha < fechaCorteUsuario)
                    .ExecuteDeleteAsync();
            }
        }
        public async Task<List<ActividadEmpleadosDto>> ObtenerActividadEmpleadosAsync()
        {
            var actividades = await _context.ActividadFactura
                .GroupBy(a => new { a.Usuario, a.Accion })
                .Select(g => new ActividadEmpleadosDto
                {
                    NombreEmpleado = g.Key.Usuario,
                    Accion = g.Key.Accion,
                    Movimientos = g.Count()
                })
                .ToListAsync();

            return actividades;
        }
    }
}