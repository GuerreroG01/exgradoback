using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ExGradoBack.Models;

namespace ExGradoBack.Models
{
    public class VehiculoInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Se debe ingresar una marca.")]
        [StringLength(50)]
        public required string Marca { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un modelo.")]
        [StringLength(50)]
        public required string Modelo { get; set; }

        [Required(ErrorMessage = "Se debe ingresar un año.")]
        public int Anio { get; set; }
        public string? FotoReferencia { get; set; }
        [Required(ErrorMessage = "Se debe ingresar un tipo de motor.")]
        public string TipoMotor { get; set; } = string.Empty;
        [Required(ErrorMessage = "Se debe ingresar un tipo de combustible.")]
        public string TipoCombustible { get; set; } = string.Empty;
    }
}