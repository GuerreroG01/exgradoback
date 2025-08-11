using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExGradoBack.Models
{
    public class DetalleOrdenCompra
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int OrdenCompraId { get; set; }

        [ForeignKey("OrdenCompraId")]
        public virtual OrdenCompra OrdenCompra { get; set; } = null!;

        [Required]
        public int RepuestoId { get; set; }

        [ForeignKey("RepuestoId")]
        public virtual Repuesto Repuesto { get; set; } = null!;

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal PrecioUnitario { get; set; }

        [Required]
        public decimal Subtotal { get; set; }
    }
}