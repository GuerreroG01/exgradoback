using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ExGradoBack.Models
{
    public class Backup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {get; set;}
        [JsonPropertyName("fecha_Respaldo")]
        public DateTime Fecha_Repaldo { get; set; }
        [JsonPropertyName("frecuencia_Respaldo")]
        public required string Frecuencia_Respaldo { get; set; }
        [JsonPropertyName("fecha_RespaldoAnterior")]
        public DateTime? Fecha_RespaldoAnterior { get; set; }
        [JsonPropertyName("activo")]
        public bool Activo { get; set; } = false;
    }
}