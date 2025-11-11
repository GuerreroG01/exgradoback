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
        private readonly ILogger<ReportController> _logger;
        public ReportController(IReportService reportService, ILogger<ReportController> logger)
        {
            _reportService = reportService;
            _logger = logger;
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
            _logger.LogInformation("Inicio de {Metodo} a las {Hora}", nameof(DescargarReporteActividadEmpleados), DateTime.UtcNow);

            try
            {
                _logger.LogInformation("Llamando a _reportService.GenerarReporteActividadEmpleados()");
                var excelBytes = await _reportService.GenerarReporteActividadEmpleados();

                if (excelBytes == null)
                {
                    _logger.LogWarning("El servicio devolvió null para el reporte de actividad de empleados.");
                    return NotFound("No se encontraron resultados (bytes nulos).");
                }

                if (excelBytes.Length == 0)
                {
                    _logger.LogWarning("El servicio devolvió un archivo vacío para el reporte de actividad de empleados.");
                    return NotFound("No se encontraron resultados (archivo vacío).");
                }

                var fecha = DateTime.UtcNow.AddHours(-6).ToString("dd-MM-yyyy");
                var nombreArchivo = $"Reporte de Actividad_{fecha}.xlsx";

                _logger.LogInformation("Reporte generado correctamente. Tamaño del archivo: {TamañoBytes} bytes. Nombre del archivo: {NombreArchivo}",
                    excelBytes.Length, nombreArchivo);

                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                _logger.LogInformation("Preparando archivo para descarga con Content-Type: {ContentType}", contentType);

                _logger.LogInformation("Finalización exitosa de {Metodo} a las {Hora}", nameof(DescargarReporteActividadEmpleados), DateTime.UtcNow);
                return File(excelBytes, contentType, nombreArchivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error inesperado en {Metodo} a las {Hora}", nameof(DescargarReporteActividadEmpleados), DateTime.UtcNow);
                return StatusCode(500, "Error interno al generar el reporte de actividad de empleados.");
            }
        }
    }
}