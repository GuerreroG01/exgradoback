namespace ExGradoBack.DTOs
{
    public class OrdenCompraDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int ProveedorId { get; set; }
        public required string Estado { get; set; }
    }
    public class OrdenCompraResumenDTO
    {
        public DateTime Fecha { get; set; }
        public int Pendientes { get; set; }
        public int Enviados { get; set; }
        public int Entregado { get; set; }
        public int Inventario { get; set; }
    }
}