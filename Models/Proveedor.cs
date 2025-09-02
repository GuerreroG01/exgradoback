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
        public required string Nombre { get; set; }

        [StringLength(20)]
        //Número ruc o número de cédula jurídica fiscal. 
        public string? Documento { get; set; }

        [Required]
        [StringLength(100)]
        public required string NombreContacto { get; set; }

        [Required]
        [StringLength(100)]
        public string Telefono { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(30)]
        public string? Pais { get; set; }

        [StringLength(30)]
        public string? Ciudad { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        [StringLength(200)]
        public string? Notas { get; set; }

    }
}