namespace ExGradoBack.DTOs
{
    public class RepuestoDto
    {
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioProveedor { get; set; }
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
        public DateTime FechaAbastecimiento { get; set; }
        public required string Ubicacion { get; set; }
        public int MarcaRepuestoId { get; set; }
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