using ExGradoBack.Models;
namespace ExGradoBack.DTOs
{
    public class FacturaReporteDto
    {
        public int FacturaId { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Vendedor { get; set; } = string.Empty;
        public int Descuento { get; set; }
        public decimal Total { get; set; }

        public List<DetalleFacturaReporteDto> Detalles { get; set; } = new();
    }

    public class DetalleFacturaReporteDto
    {
        public string Repuesto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
    public class OrdenCompraReportDto
    {
        public int OrdenId { get; set; }
        public DateTime Fecha { get; set; }
        public string ProveedorNombre { get; set; } = string.Empty;
        public Proveedor Proveedor { get; set; } = null!;
        public required string Solicitante { get; set; }
        public List<DetalleOrdenDto> DetallesOrden { get; set; } = new();
    }
    public class DetalleOrdenDto
    {
        public string Repuesto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
    public class RepuestosAReabastecerDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
        public DateTime FechaAbastecimiento { get; set; }
    }
    public class RepuestosMasVendidosDto
    {
        public int RepuestoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int CantidadVendida { get; set; }
        public decimal Precio { get; set; }
        public decimal PrecioProveedor { get; set; }
    }
    public class TipoClienteReporteDto
    {
        public string TipoCliente { get; set; } = string.Empty;
        public int CantidadFacturas { get; set; }
        public int CantidadRepuestosComprados { get; set; }
        public decimal TotalIngresos { get; set; }
        public double PorcentajeParticipacion { get; set; }
    }
    public class ActividadEmpleadosDto
    {
        public string NombreEmpleado { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public int Movimientos { get; set; }
    }
}   