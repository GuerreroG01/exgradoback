namespace ExGradoBack.DTOs
{
    public class OrdenCompraResumenDTO
    {
        public DateTime Fecha { get; set; }
        public int Pendientes { get; set; }
        public int Enviados { get; set; }
        public int Entregado { get; set; }
        public int Inventario { get; set; }
    }
}