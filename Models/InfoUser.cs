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

        [Required]
        [StringLength(100)]
        public required string Nombres { get; set; }

        [Required]
        [StringLength(100)]
        public required string Apellidos { get; set; }

        public string? FotoPerfil { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public DateTime Nacimiento { get; set; }

        [Required]
        public required string Genero { get; set; }

        [Required]
        public required string Telefono { get; set; }
    }
}