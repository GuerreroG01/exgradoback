using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ExGradoBack.Models;

namespace ExGradoBack.Models
{
    public class Auth
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Username { get; set; }

        [Required]
        [StringLength(100)]
        public required string Password { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        [Required]
        public int RolId { get; set; }
        [JsonIgnore]
        public virtual Rol? Rol { get; set; }

        [JsonIgnore]
        public InfoUser? InfoUser { get; set; }

    }
}