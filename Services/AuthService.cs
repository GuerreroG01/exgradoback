using ExGradoBack.Services;
using ExGradoBack.Repositories;
using ExGradoBack.Models;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BCrypt.Net;

namespace ExGradoBack.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Auth>> GetAllUsersAsync()
        {
            return await _authRepository.GetAllAsync();
        }

        public async Task<Auth?> GetUserByIdAsync(int id)
        {
            return await _authRepository.GetByIdAsync(id);
        }

        public async Task<Auth> CreateUserAsync(Auth newUser)
        {
            newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
            newUser.FechaRegistro = DateTime.UtcNow;
            return await _authRepository.AddAsync(newUser);
        }

        public async Task<Auth> UpdateUserAsync(Auth updatedUser)
        {
            return await _authRepository.UpdateAsync(updatedUser);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _authRepository.DeleteAsync(id);
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            var user = await _authRepository.GetByUsernameAsync(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                return null;

            return GenerateJwtToken(user);
        }

        public async Task<Auth> RegisterAsync(RegisterDto dto)
        {
            var newUser = new Auth
            {
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = dto.Rol,
                FechaRegistro = DateTime.Now,
                InfoUser = dto.InfoUser != null ? new InfoUser
                {
                    Nombres = dto.InfoUser.Nombres,
                    Apellidos = dto.InfoUser.Apellidos,
                    FotoPerfil = dto.InfoUser.FotoPerfil,
                    Email = dto.InfoUser.Email,
                    Nacimiento = dto.InfoUser.Nacimiento,
                    Genero = dto.InfoUser.Genero,
                    Telefono = dto.InfoUser.Telefono
                } : null
            };

            // Solo si InfoUser fue creado, asignamos la relación inversa
            if (newUser.InfoUser != null)
            {
                newUser.InfoUser.Auth = newUser;
            }

            return await _authRepository.AddAsync(newUser);
        }
        public async Task<bool> UserExistsAsync(string username)
        {
            var user = await _authRepository.GetByUsernameAsync(username);
            return user != null;
        }

        private string GenerateJwtToken(Auth admin)
        {
            DateTime expiresAt = DateTime.UtcNow.AddMinutes(60);
            long expiresAtUnix = new DateTimeOffset(expiresAt).ToUnixTimeSeconds();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, admin.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                          new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                          ClaimValueTypes.Integer64),
                new Claim("id", admin.Id.ToString()),
                new Claim("rol", admin.Rol),
                new Claim(ClaimTypes.Role, admin.Rol),
                new Claim(JwtRegisteredClaimNames.Exp, expiresAtUnix.ToString(), ClaimValueTypes.Integer64)
            };
            
            var jsonWebTokenSecret = Environment.GetEnvironmentVariable("JsonWebTokenSecret") ?? "";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jsonWebTokenSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "ExGradoSystem.com",
                audience: "ExGradoSystem.com",
                claims: claims,
                expires: expiresAt, // Esto ya maneja la expiración
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}