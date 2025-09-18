namespace ExGradoBack.DTOs
{
    public class MarcaRepuestoUsadaDto
    {
        public int MarcaRepuestoId { get; set; }
        public string NombreMarca { get; set; } = string.Empty;
        public int TotalVendidos { get; set; }
    }
}