public class RegisterDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int RolId { get; set; }
    public InfoUserDto? InfoUser { get; set; } = null!;
}

public class InfoUserDto
{
    public string Nombres { get; set; } = null!;
    public string Apellidos { get; set; } = null!;
    public string? FotoPerfil { get; set; }
    public string Email { get; set; } = null!;
    public DateTime Nacimiento { get; set; }
    public string Genero { get; set; } = null!;
    public string Telefono { get; set; } = null!;
}