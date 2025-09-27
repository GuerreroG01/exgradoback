namespace ExGradoBack.DTOs
{
    public class LoginDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required bool isLogin { get; set; } = true;
    }
}