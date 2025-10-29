using System.ComponentModel.DataAnnotations;
namespace ExGradoBack.DTOs
{
    public class RepuestoDto
    {
        [Required(ErrorMessage = "Debe ingresar un nombre para el repuesto.")]
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        [Required(ErrorMessage = "Debe ingresar el precio unitario.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor que 0.")]
        public decimal PrecioUnitario { get; set; }
        [Required(ErrorMessage = "Debe ingresar el precio unitario.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio del proveedor debe ser mayor que 0.")]
        public decimal PrecioProveedor { get; set; }
        [Required(ErrorMessage = "Debe ingresar el stock actual.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock actual no puede ser negativo.")]
        public int StockActual { get; set; }
        [Required(ErrorMessage = "Debe ingresar el stock mínimo.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
        public int StockMinimo { get; set; }
        [Required(ErrorMessage = "Debe seleccionar una fecha de abastecimiento.")]
        public DateTime FechaAbastecimiento { get; set; }
        [Required(ErrorMessage = "Debe ingresar una ubicación.")]
        public required string Ubicacion { get; set; }
        [Required(ErrorMessage = "Debe seleccionar una marca.")]
        public int MarcaRepuestoId { get; set; }
        [Required(ErrorMessage = "Debe seleccionar al menos un vehículo compatible.")]
        public List<int> VehiculoInfoIds { get; set; } = new();
    }
    public class RepuestoStockDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int StockActual { get; set; }
    }
    public class RepuestoReabastecimientoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int StockActual { get; set; }
        public DateTime? FechaAbastecimiento { get; set; }
    }
}