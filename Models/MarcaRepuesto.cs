using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ExGradoBack.Models;

namespace ExGradoBack.Models
{
    public class MarcaRepuesto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un nombre.")]
        public string Nombre { get; set; } = string.Empty;
        public string? Detalle { get; set; }
    }
}