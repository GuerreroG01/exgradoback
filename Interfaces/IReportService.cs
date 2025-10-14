using ExGradoBack.DTOs;
namespace ExGradoBack.Services
{
    public interface IReportService
    {
        Task<byte[]> GenerarReporteMensualExcelAsync(int anio, int mes);
        Task<byte[]> GenerarReporteHistorialProveedor(int proveedorId);
        Task<byte[]> GenerarReporteRepuestosaReabastecer();
        Task<byte[]> GenerarReporteRepuestosMasVendidos(int top);
        Task<byte[]> GenerarReporteTiposCliente();
        Task<byte[]> GenerarReporteActividadEmpleados();
    }
}