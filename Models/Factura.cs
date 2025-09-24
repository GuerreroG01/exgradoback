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
        [StringLength(100)]
        public string? NombresCliente { get; set; }
        public required string TipoCliente { get; set; }
        public required int Descuento { get; set; }

        public required string Vendedor { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public virtual ICollection<DetalleFactura> Detalles { get; set; } = new List<DetalleFactura>();
    }
}