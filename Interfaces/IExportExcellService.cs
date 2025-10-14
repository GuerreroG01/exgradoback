using ExGradoBack.DTOs;
namespace ExGradoBack.Services
{
    public interface IExportExcellService
    {
        byte[] GenerarExcelReport(List<FacturaReporteDto> facturas);
        byte[] GenerarReporteOrdenesCompra(List<OrdenCompraReportDto> ordenes);
        byte[] GenerarReporteRepuestosParaReabastecer(List<RepuestosAReabastecerDto> repuestos);
        byte[] GenerarReporteRepuestosMasVendidos(List<RepuestosMasVendidosDto> repuestos);
        byte[] GenerarReporteTiposDeClientes(List<TipoClienteReporteDto> tiposClientes);
        byte[] GenerarReporteActividadEmpleados(List<ActividadEmpleadosDto> actividades);
    }
}