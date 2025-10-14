using ExGradoBack.DTOs;
using ExGradoBack.Repositories;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace ExGradoBack.Services
{
    public class ReportService : IReportService
    {
        private readonly IFacturaRepository _facturaRepository;
        private readonly IExportExcellService _excellService;
        private readonly IOrdenCompraRepository _ordenCompraRepository;
        private readonly IRepuestoRepository _repuestoRepository;
        private readonly IActividadRepository _actividadRepository;

        public ReportService
        (
            IFacturaRepository facturaRepository, IExportExcellService excellService,
            IOrdenCompraRepository ordenCompraRepository, IRepuestoRepository repuestoRepository,
            IActividadRepository actividadRepository
        )
        {
            _facturaRepository = facturaRepository;
            _excellService = excellService;
            _ordenCompraRepository = ordenCompraRepository;
            _repuestoRepository = repuestoRepository;
            _actividadRepository = actividadRepository;

        }
        public async Task<byte[]> GenerarReporteMensualExcelAsync(int anio, int mes)
        {
            var facturas = await _facturaRepository.ObtenerReporteMensualAsync(anio, mes);

            if (facturas == null || !facturas.Any())
                return Array.Empty<byte>();

            return _excellService.GenerarExcelReport(facturas);
        }
        /*public async Task<string> GuardarReporteMensualEnDiscoAsync(int anio, int mes, string carpetaDestino)
        {
            var bytes = await GenerarReporteMensualExcelAsync(anio, mes);

            if (bytes.Length == 0)
                return string.Empty;

            if (!Directory.Exists(carpetaDestino))
                Directory.CreateDirectory(carpetaDestino);

            string nombreArchivo = $"ReporteFacturas_{anio}_{mes:00}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            string rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

            File.WriteAllBytes(rutaCompleta, bytes);
            return rutaCompleta;
        }*/
        public async Task<byte[]> GenerarReporteHistorialProveedor(int proveedorId)
        {
            var ordenes = await _ordenCompraRepository.ObtenerHistorialProveedorAsync(proveedorId);

            if (ordenes == null || !ordenes.Any())
                return Array.Empty<byte>();

            return _excellService.GenerarReporteOrdenesCompra(ordenes);
        }
        public async Task<byte[]> GenerarReporteRepuestosaReabastecer()
        {
            var repuestos = await _repuestoRepository.GetRepuestosPorDebajoDelStockMinimoAsync();

            if (repuestos == null || !repuestos.Any())
                return Array.Empty<byte>();

            return _excellService.GenerarReporteRepuestosParaReabastecer(repuestos.ToList());
        }
        public async Task<byte[]> GenerarReporteRepuestosMasVendidos(int top)
        {
            var repuestosMasVendidos = await _repuestoRepository.GetRepuestosMasVendidos(top);

            if (repuestosMasVendidos == null || !repuestosMasVendidos.Any())
                return Array.Empty<byte>();

            return _excellService.GenerarReporteRepuestosMasVendidos(repuestosMasVendidos);
        }
        public async Task<byte[]> GenerarReporteTiposCliente()
        {
            var participaciontipos = await _facturaRepository.ObtenerReportePorTipoClienteAsync();

            if (participaciontipos == null || !participaciontipos.Any())
                return Array.Empty<byte>();

            return _excellService.GenerarReporteTiposDeClientes(participaciontipos);
        }
        public async Task<byte[]> GenerarReporteActividadEmpleados()
        {
            var actividades = await _actividadRepository.ObtenerActividadEmpleadosAsync();

            if (actividades == null || !actividades.Any())
                return Array.Empty<byte>();

            return _excellService.GenerarReporteActividadEmpleados(actividades);
        }
    }
}