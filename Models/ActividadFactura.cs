using System.ComponentModel.DataAnnotations;

namespace ExGradoBack.Models
{
    public class ActividadFactura
    {
        [Key]
        public int Id { get; set; }

        public int FacturaId { get; set; }

        [Required]
        public string Usuario { get; set; } = null!;

        [Required]
        public string Accion { get; set; } = null!;

        public DateTime Fecha { get; set; } = DateTime.UtcNow.AddHours(-6);

        public string? DatosAntes { get; set; }

        public string? DatosDespues { get; set; }
    }
}