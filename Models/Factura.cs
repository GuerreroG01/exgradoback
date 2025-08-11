using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExGradoBack.Models
{
    public class Factura
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [Required]
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente Cliente { get; set; } = null!;

        [Required]
        public decimal Total { get; set; }

        public virtual ICollection<DetalleFactura> Detalles { get; set; } = new List<DetalleFactura>();
    }
}