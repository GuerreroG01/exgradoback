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
        [Range(0, 5, ErrorMessage = "La calificación debe estar entre 0 y 5.")]
        public double? Calificacion { get; set; }
    }
}