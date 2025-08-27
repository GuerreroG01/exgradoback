namespace ExGradoBack.DTOs
{
    public class VehiculoInfoMinDto
    {
        public int Id { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string? FotoReferencia { get; set; }
    }
}