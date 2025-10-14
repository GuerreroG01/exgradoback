using ExGradoBack.DTOs;
using ExGradoBack.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ExGradoBack.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("reporteVentas-mensual/excel")]
        public async Task<IActionResult> DescargarReporteMensualExcel([FromQuery] int anio, [FromQuery] int mes)
        {
            var excelBytes = await _reportService.GenerarReporteMensualExcelAsync(anio, mes);

            if (excelBytes == null || excelBytes.Length == 0)
                return NotFound("No hay facturas registradas en ese período.");

            var nombreArchivo = $"ReporteFacturas_{anio}_{mes:00}.xlsx";
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
        }
        [HttpGet("Reporte_HistorialProveedor/excel")]
        public async Task<IActionResult> DescargarReporteHistorialExcel([FromQuery] int proveedorId)
        {
            var excelBytes = await _reportService.GenerarReporteHistorialProveedor(proveedorId);

            if (excelBytes == null || excelBytes.Length == 0)
                return NotFound("No hay ordenes registradas aún para este proveedor.");

            var nombreArchivo = $"ReporteOrdenes_Proveedor_{proveedorId}.xlsx";
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
        }
        [HttpGet("Reporte_RepuestosReabastecer/excel")]
        public async Task<IActionResult> DescargarReporteReabastecerExcel()
        {
            var excelBytes = await _reportService.GenerarReporteRepuestosaReabastecer();

            if (excelBytes == null || excelBytes.Length == 0)
                return NotFound("Aún no hay repuestos que necesiten ser reabastecidos.");

            var fecha = DateTime.UtcNow.AddHours(-6).ToString("dd-MM-yyyy");
            var nombreArchivo = $"ReporteRepuestos_Reabastecer_{fecha}.xlsx";
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
        }
        [HttpGet("Reporte_RepuestosMasVendidos/excel")]
        public async Task<IActionResult> DescargarReporteRepuestoMasVendidos([FromQuery] int top)
        {
            var excelBytes = await _reportService.GenerarReporteRepuestosMasVendidos(top);

            if (excelBytes == null || excelBytes.Length == 0)
                return NotFound("No se encontrarón resultados");

            var fecha = DateTime.UtcNow.AddHours(-6).ToString("dd-MM-yyyy");
            var nombreArchivo = $"ReporteRepuestos_MejorVendidosTop{top}_{fecha}.xlsx";
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
        }
        [HttpGet("Reporte_TiposCliente/excel")]
        public async Task<IActionResult> DescargarReporteParticipacionTiposCliente()
        {
            var excelBytes = await _reportService.GenerarReporteTiposCliente();

            if (excelBytes == null || excelBytes.Length == 0)
                return NotFound("No se encontrarón resultados");

            var fecha = DateTime.UtcNow.AddHours(-6).ToString("dd-MM-yyyy");
            var nombreArchivo = $"Reporte_TiposClientes_{fecha}.xlsx";
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
        }
        [HttpGet("Reporte_ActividadEmpleados/excel")]
        public async Task<IActionResult> DescargarReporteActividadEmpleados()
        {
            var excelBytes = await _reportService.GenerarReporteActividadEmpleados();

            if (excelBytes == null || excelBytes.Length == 0)
                return NotFound("No se encontrarón resultados");

            var fecha = DateTime.UtcNow.AddHours(-6).ToString("dd-MM-yyyy");
            var nombreArchivo = $"Reporte de Actividad_{fecha}.xlsx";
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
        }
    }
}