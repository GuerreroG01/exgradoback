namespace ExGradoBack.DTOs
{
    public class ActividadResumenDto
    {
        public int Id { get; set; }
        public int FacturaId { get; set; }
        public string Accion { get; set; } = null!;
        public DateTime Fecha { get; set; }
    }
}