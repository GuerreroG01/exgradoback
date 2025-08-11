using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExGradoBack.Models
{
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Nombres { get; set; }

        [Required]
        public string Telefono { get; set; } = string.Empty;
        [Required]
        [StringLength(20)]
        public string TipoCliente { get; set; } = string.Empty;
    }
}