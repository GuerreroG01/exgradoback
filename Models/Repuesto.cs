using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ExGradoBack.Models;

namespace ExGradoBack.Models
{
    public class Repuesto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un nombre para el repuesto.")]
        [StringLength(100)]
        public required string Nombre { get; set; }

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un precio unitario.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        [Required(ErrorMessage = "Se debe ingresar el precio del proveedor utilizado.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioProveedor { get; set; }

        [Required(ErrorMessage = "Falta el stock actual.")]
        public int StockActual { get; set; }

        [Required(ErrorMessage = "Es necesario fijar un stock mínimo.")]
        public int StockMinimo { get; set; }

        [Required(ErrorMessage = "Se debe ingresar la fecha de abastecimiento.")]
        public DateTime FechaAbastecimiento { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "No hay ubicación")]
        public string Ubicacion { get; set; } = string.Empty;

        public virtual ICollection<VehiculoInfo> VehiculoInfoIds { get; set; } = new List<VehiculoInfo>();

        public int MarcaRepuestoId { get; set; }
        public virtual MarcaRepuesto? MarcaRepuesto { get; set; }
    }
}