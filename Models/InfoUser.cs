using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ExGradoBack.Models
{
    public class InfoUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int AuthId { get; set; }

        [ForeignKey("AuthId")]
        [JsonIgnore]
        public Auth? Auth { get; set; }

        [Required(ErrorMessage = "Los nombres son obligatorios.")]
        [StringLength(100)]
        public required string Nombres { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(100)]
        public required string Apellidos { get; set; }

        public string? FotoPerfil { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress]
        public required string Email { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatorio.")]
        public DateTime Nacimiento { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un género.")]
        public required string Genero { get; set; }

        [Required(ErrorMessage = "Ingrese un número de teléfono.")]
        public required string Telefono { get; set; }
    }
}