using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExGradoBack.Models
{
    public class Proveedor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Nombre { get; set; }  // Nombre de empresa o persona

        [StringLength(20)]
        public string? Documento { get; set; } // RUC o DNI (opcional)

        [Required]
        [StringLength(100)]
        public string Telefono { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }
    }
}