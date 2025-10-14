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
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
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
        public async Task<List<FacturaReporteDto>> ObtenerReporteMensualAsync(int anio, int mes)
        {
            return await _context.Factura
                .Where(f => f.Fecha.Year == anio && f.Fecha.Month == mes)
                .Include(f => f.Detalles)
                    .ThenInclude(d => d.Repuesto)
                .Select(f => new FacturaReporteDto
                {
                    FacturaId = f.Id,
                    Fecha = f.Fecha,
                    Cliente = f.NombresCliente ?? "Cliente no registrado",
                    Vendedor = f.Vendedor,
                    Descuento = f.Descuento,
                    Total = f.Total,
                    Detalles = f.Detalles.Select(d => new DetalleFacturaReporteDto
                    {
                        Repuesto = d.Repuesto != null ? d.Repuesto.Nombre : "N/A",
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal
                    }).ToList()
                })
                .ToListAsync();
        }
        public async Task<List<TipoClienteReporteDto>> ObtenerReportePorTipoClienteAsync()
        {
            var facturas = await _context.Factura
                .Include(f => f.Detalles)
                .ToListAsync();

            var totalGeneral = facturas.Sum(f => f.Total);

            var reporte = facturas
                .GroupBy(f => f.TipoCliente)
                .Select(g => new TipoClienteReporteDto
                {
                    TipoCliente = g.Key,
                    CantidadFacturas = g.Count(),
                    CantidadRepuestosComprados = g.Sum(f => f.Detalles.Sum(d => d.Cantidad)),
                    TotalIngresos = g.Sum(f => f.Total),
                    PorcentajeParticipacion = totalGeneral == 0 
                        ? 0 
                        : Math.Round((double)(g.Sum(f => f.Total) / totalGeneral) * 100, 2)
                })
                .OrderByDescending(r => r.TotalIngresos)
                .ToList();

            return reporte;
        }
    }
}