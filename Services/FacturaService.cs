using ExGradoBack.Models;
using ExGradoBack.Repositories;
using ExGradoBack.DTOs;

namespace ExGradoBack.Services
{
    public class FacturaService : IFacturaService
    {
        private readonly IFacturaRepository _facturaRepository;

        public FacturaService(IFacturaRepository facturaRepository)
        {
            _facturaRepository = facturaRepository;
        }

        public async Task<IEnumerable<int>> GetAniosConFacturasAsync()
        {
            var anios = await _facturaRepository.GetAniosConFacturasAsync();
            if (!anios.Any())
                throw new Exception("No hay facturas registradas.");

            return anios;
        }

        public async Task<IEnumerable<int>> GetMesesConFacturasAsync(int anio)
        {
            if (anio < 2000 || anio > DateTime.UtcNow.Year)
                throw new ArgumentException("Año inválido.");

            var meses = await _facturaRepository.GetMesesConFacturasAsync(anio);
            if (!meses.Any())
                throw new Exception($"No hay facturas registradas para el año {anio}.");

            return meses;
        }

        public async Task<IEnumerable<int>> GetDiasConFacturasAsync(int anio, int mes)
        {
            if (anio < 2000 || anio > DateTime.UtcNow.Year)
                throw new ArgumentException("Año inválido.");
            if (mes < 1 || mes > 12)
                throw new ArgumentException("Mes inválido.");

            var dias = await _facturaRepository.GetDiasConFacturasAsync(anio, mes);
            if (!dias.Any())
                throw new Exception($"No hay facturas registradas para {anio}-{mes:D2}.");

            return dias;
        }

        public async Task<IEnumerable<Factura>> GetFacturasPorDiaAsync(int anio, int mes, int dia)
        {
            if (anio < 2000 || anio > DateTime.UtcNow.Year)
                throw new ArgumentException("Año inválido.");
            if (mes < 1 || mes > 12)
                throw new ArgumentException("Mes inválido.");

            if (dia < 1 || dia > DateTime.DaysInMonth(anio, mes))
                throw new ArgumentException("Día inválido para el mes y año especificados.");

            var fechaIngresada = new DateTime(anio, mes, dia);
            var fechaActual = DateTime.UtcNow.Date;

            if (fechaIngresada > fechaActual)
                throw new ArgumentException("La fecha ingresada aún no ha ocurrido");

            var facturas = await _facturaRepository.GetFacturasPorDiaAsync(anio, mes, dia);
            if (!facturas.Any())
                throw new Exception($"No hay facturas registradas para {anio}-{mes:D2}-{dia:D2}.");

            return facturas;
        }
        public async Task<Factura?> GetFacturaByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID inválido.");

            var factura = await _facturaRepository.GetFacturaByIdAsync(id);
            if (factura == null)
                throw new Exception($"No se encontró la factura con ID {id}.");

            return factura;
        }

        public async Task<Factura> CreateFacturaAsync(Factura factura)
        {
            if (factura.Total <= 0)
                throw new ArgumentException("El total debe ser mayor a 0.");

            if (string.IsNullOrWhiteSpace(factura.TipoCliente))
                throw new ArgumentException("El tipo de cliente es obligatorio.");

            return await _facturaRepository.CreateFacturaAsync(factura);
        }

        public async Task<Factura> UpdateFacturaAsync(Factura factura)
        {
            if (!await _facturaRepository.FacturaExistsAsync(factura.Id))
                throw new Exception($"La factura con ID {factura.Id} no existe.");

            if (factura.Total <= 0)
                throw new ArgumentException("El total debe ser mayor a 0.");

            if (string.IsNullOrWhiteSpace(factura.TipoCliente))
                throw new ArgumentException("El tipo de cliente es obligatorio.");

            return await _facturaRepository.UpdateFacturaAsync(factura);
        }

        public async Task<bool> DeleteFacturaAsync(int id)
        {
            if (!await _facturaRepository.FacturaExistsAsync(id))
                throw new Exception($"La factura con ID {id} no existe.");

            return await _facturaRepository.DeleteFacturaAsync(id);
        }

        public async Task<bool> FacturaExistsAsync(int id)
        {
            return await _facturaRepository.FacturaExistsAsync(id);
        }
        public async Task<List<(string Vendedor, int TotalVendidos)>> ObtenerTop3VendedoresAsync()
        {
            return await _facturaRepository.ObtenerTop3VendedoresAsync();
        }
        public async Task<List<(int Mes, decimal TotalVentas)>> ObtenerTotalVentasPorMesAsync(int anio)
        {
            if (anio < 1)
                throw new ArgumentException("El año ingresado no es válido.");

            var ventas = await _facturaRepository.ObtenerTotalVentasPorMesAsync(anio);

            if (ventas == null || !ventas.Any())
                throw new InvalidOperationException($"No se encontraron ventas registradas para el año {anio}.");

            return ventas;
        }
        public async Task<List<FacturasPorBloqueDto>> ObtenerFacturasPorBloqueAsync()
            => await _facturaRepository.ObtenerFacturasPorBloqueAsync();
        public async Task<Dictionary<DateTime, DatosPorDiaDto>> GetCantidadFacturasPorDiaUltimaSemanaAsync()
            => await _facturaRepository.GetCantidadFacturasPorDiaUltimaSemanaAsync();

        public async Task<Dictionary<DateTime, DatosPorDiaDto>> GetRepuestosVendidosPorDiaUltimaSemanaAsync()
            => await _facturaRepository.GetCantidadRepuestosVendidosPorDiaUltimaSemanaAsync();
    }
}