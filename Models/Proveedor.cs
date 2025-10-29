using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExGradoBack.Models
{
    public class Proveedor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        [StringLength(100)]
        public required string Nombre { get; set; }

        [StringLength(20, ErrorMessage = "El Documento no puede tener más de 20 caracteres.")]
        //Número ruc o número de cédula jurídica fiscal. 
        public string? Documento { get; set; }

        [Required(ErrorMessage = "El campo del nombre de contacto es obligatorio.")]
        [StringLength(100)]
        public required string NombreContacto { get; set; }

        [Required(ErrorMessage = "El campo Teléfono es obligatorio.")]
        [StringLength(100)]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Email es obligatorio.")]
        [StringLength(100)]
        public required string Email { get; set; }

        [Required(ErrorMessage = "El campo del Pais es obligatorio.")]
        [StringLength(30)]
        public required string Pais { get; set; }

        [Required(ErrorMessage = "El campo del ciudad es obligatorio.")]
        [StringLength(30)]
        public required string Ciudad { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        [StringLength(200)]
        public string? Notas { get; set; }

    }
}