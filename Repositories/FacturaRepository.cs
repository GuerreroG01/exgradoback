using ExGradoBack.Data;
using ExGradoBack.Models;
using ExGradoBack.DTOs;
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
        public async Task<List<(string Vendedor, int TotalVendidos)>> ObtenerTop3VendedoresAsync()
        {
            var result = await _context.Factura
                .SelectMany(f => f.Detalles, (factura, detalle) => new { factura.Vendedor, detalle.Cantidad })
                .GroupBy(x => x.Vendedor)
                .Select(g => new
                {
                    Vendedor = g.Key,
                    TotalVendidos = g.Sum(x => x.Cantidad)
                })
                .OrderByDescending(x => x.TotalVendidos)
                .Take(3)
                .ToListAsync();

            return result.Select(r => (r.Vendedor, r.TotalVendidos)).ToList();
        }
        public async Task<List<(int Mes, decimal TotalVentas)>> ObtenerTotalVentasPorMesAsync(int anio)
        {
            var consulta = await _context.Factura
                .Where(f => f.Fecha.Year == anio)
                .GroupBy(f => f.Fecha.Month)
                .Select(g => new
                {
                    Mes = g.Key,
                    TotalVentas = g.Sum(f => f.Total)
                })
                .OrderBy(r => r.Mes)
                .ToListAsync();
            var resultado = consulta
                .Select(r => (r.Mes, TotalVentas: Math.Round(r.TotalVentas, 2)))
                .ToList();

            return resultado;
        }
        public async Task<List<FacturasPorBloqueDto>> ObtenerFacturasPorBloqueAsync()
        {
            var resultado = await _context.Factura
                .GroupBy(f => f.Fecha.Hour / 2)
                .Select(g => new FacturasPorBloqueDto
                {
                    Bloque = g.Key,
                    Cantidad = g.Count()
                })
                .OrderBy(f => f.Bloque)
                .ToListAsync();

            return resultado;
        }
        private (DateTime fechaInicioUtc, DateTime mananaUtc) ObtenerRangoUltimaSemanaUtc()
        {
            var zonaManagua = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
            var ahoraManagua = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zonaManagua);

            var hoyManagua = ahoraManagua.Date;
            var fechaInicioManagua = hoyManagua.AddDays(-6);

            var fechaInicioUtc = TimeZoneInfo.ConvertTimeToUtc(fechaInicioManagua, zonaManagua);
            var mananaUtc = TimeZoneInfo.ConvertTimeToUtc(hoyManagua.AddDays(1), zonaManagua);

            return (fechaInicioUtc, mananaUtc);
        }
        public async Task<Dictionary<DateTime, DatosPorDiaDto>> GetCantidadFacturasPorDiaUltimaSemanaAsync()
        {
            var (fechaInitUTC, tomorrowUTC) = ObtenerRangoUltimaSemanaUtc();

            var facturasPorDiaUtc = await _context.Factura
                .Where(f => f.Fecha >= fechaInitUTC && f.Fecha < tomorrowUTC)
                .GroupBy(f => f.Fecha.Date)
                .Select(g => new { Dia = g.Key, Cantidad = g.Count() })
                .ToDictionaryAsync(
                    g => g.Dia,
                    g => new DatosPorDiaDto
                    {
                        DiaMes = g.Dia.Day,
                        DiaSemana = g.Dia.DayOfWeek.ToString(),
                        Cantidad = g.Cantidad
                    }
                );

            return facturasPorDiaUtc;
        }
        public async Task<Dictionary<DateTime, DatosPorDiaDto>> GetCantidadRepuestosVendidosPorDiaUltimaSemanaAsync()
        {
            var (fechaInitUTC, tomorrowUTC) = ObtenerRangoUltimaSemanaUtc();

            var detallePorDia = await _context.DetalleFactura
                .Where(d => d.Factura != null && d.Factura.Fecha >= fechaInitUTC && d.Factura.Fecha < tomorrowUTC)
                .GroupBy(d => d.Factura!.Fecha.Date)
                .Select(g => new
                {
                    Dia = g.Key,
                    CantidadTotal = g.Sum(x => x.Cantidad)
                })
                .ToDictionaryAsync(
                    x => x.Dia,
                    x => new DatosPorDiaDto
                    {
                        DiaMes = x.Dia.Day,
                        DiaSemana = x.Dia.DayOfWeek.ToString(),
                        Cantidad = x.CantidadTotal
                    }
                );

            return detallePorDia;
        }
    }
}