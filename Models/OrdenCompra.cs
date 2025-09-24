using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExGradoBack.Models
{
    public class OrdenCompra
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [Required]
        public int ProveedorId { get; set; }

        [ForeignKey("ProveedorId")]
        public virtual Proveedor? Proveedor { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }
        public required string Estado { get; set; }
        public required string Solicitante { get; set; }

        public virtual ICollection<DetalleOrdenCompra>? Detalles { get; set; } = new List<DetalleOrdenCompra>();
    }
}