using ExGradoBack.Services;
using ExGradoBack.Repositories;
using ExGradoBack.Models;
using ExGradoBack.DTOs;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BCrypt.Net;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace ExGradoBack.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IRolRepository _rolRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository authRepository, IRolRepository rolRepository, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _authRepository = authRepository;
            _rolRepository = rolRepository;
            _configuration = configuration;
            _logger = logger;
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
            var existingUser = await _authRepository.GetByIdAsync(updatedUser.Id);
            if (existingUser == null)
                throw new KeyNotFoundException("Usuario no encontrado.");

            if (!string.IsNullOrWhiteSpace(updatedUser.Username) && updatedUser.Username != existingUser.Username)
            {
                existingUser.Username = updatedUser.Username;
            }

            if (!string.IsNullOrWhiteSpace(updatedUser.Password))
            {
                if (!updatedUser.Password.StartsWith("$2a$"))
                {
                    _logger.LogInformation("Contraseña en texto plano recibida: {Password}", updatedUser.Password);
                    existingUser.Password = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);
                }
                else
                {
                    _logger.LogWarning("Se recibió una contraseña ya hasheada, omitiendo actualización.");
                }
            }
            if (updatedUser.RolId != 0 && updatedUser.RolId != existingUser.RolId)
            {
                existingUser.RolId = updatedUser.RolId;
            }
            if (updatedUser.FechaRegistro != default && updatedUser.FechaRegistro != existingUser.FechaRegistro)
            {
                existingUser.FechaRegistro = updatedUser.FechaRegistro;
            }
            await _authRepository.UpdateAsync(existingUser);
            var rol = await _rolRepository.GetRoleByIdAsync(existingUser.RolId);
            existingUser.Rol = rol;
            return existingUser;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _authRepository.DeleteAsync(id);
        }
        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            var user = await _authRepository.GetByUsernameAsync(username);
            return user != null && BCrypt.Net.BCrypt.Verify(password, user.Password);
        }
        public async Task<TokenResponse?> LoginAsync(string username, string password)
        {
            var user = await _authRepository.GetByUsernameAsync(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                return null;

            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            await _authRepository.SaveRefreshTokenAsync(user.Id, refreshToken);

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<Auth> RegisterAsync(RegisterDto dto)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(dto, null, null);
            if (!Validator.TryValidateObject(dto, context, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage);
                var errorMsg = string.Join("; ", errors);
                _logger.LogWarning("Validación fallida para el DTO de registro: {Errors}", errorMsg);
                throw new ArgumentException($"Datos inválidos: {errorMsg}");
            }

            var rolExistente = await _rolRepository.GetRoleByIdAsync(dto.RolId);
            if (rolExistente == null)
            {
                throw new Exception($"El rol '{dto.RolId}' no existe.");
            }

            var newUser = new Auth
            {
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RolId = rolExistente.Id,
                FechaRegistro = DateTime.Now,
            };

            if (dto.InfoUser != null)
            {
                newUser.InfoUser = new InfoUser
                {
                    Nombres = dto.InfoUser.Nombres,
                    Apellidos = dto.InfoUser.Apellidos,
                    FotoPerfil = dto.InfoUser.FotoPerfil,
                    Email = dto.InfoUser.Email,
                    Nacimiento = dto.InfoUser.Nacimiento,
                    Genero = dto.InfoUser.Genero,
                    Telefono = dto.InfoUser.Telefono
                };
            }
            else
            {
                _logger.LogInformation("No se recibió InfoUser para el usuario: {Username}", dto.Username);
            }
            var result = await _authRepository.AddAsync(newUser);

            return result;
        }
        public async Task<bool> UserExistsAsync(string username)
        {
            var user = await _authRepository.GetByUsernameAsync(username);
            return user != null;
        }

        public string GenerateJwtToken(Auth admin)
        {
            DateTime expiresAt = DateTime.UtcNow.AddDays(5);
            long expiresAtUnix = new DateTimeOffset(expiresAt).ToUnixTimeSeconds();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, admin.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                        new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                        ClaimValueTypes.Integer64),
                new Claim("id", admin.Id.ToString()),
                new Claim("rol", admin.Rol?.Nombre ?? "SinRol"),
                new Claim(ClaimTypes.Role, admin.Rol?.Nombre ?? "SinRol"),
                new Claim(JwtRegisteredClaimNames.Exp, expiresAtUnix.ToString(), ClaimValueTypes.Integer64)
            };

            var jsonWebTokenSecret = Environment.GetEnvironmentVariable("JsonWebTokenSecret") ?? "";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jsonWebTokenSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "ExGradoSystem.com",
                audience: "ExGradoSystem.com",
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public async Task<Auth> GetUserByUsernameAsync(string username)
        {
            var user = await _authRepository.GetByUsernameAsync(username);

            if (user == null)
            {
                throw new KeyNotFoundException($"Usuario con username '{username}' no encontrado.");
            }

            return user;
        }
        public async Task<RefreshToken?> GetRefreshTokenAsync(int userId, string token)
        {
            return await _authRepository.GetRefreshTokenAsync(userId, token);
        }
        public async Task<int> GetTotalUsersAsync()
        {
            return await _authRepository.CountUsersAsync();
        }
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var jsonWebTokenSecret = Environment.GetEnvironmentVariable("JsonWebTokenSecret") ?? "";
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jsonWebTokenSecret)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }
        public async Task<List<UserDto>> GetByUsernameAsync(string username)
        {
            var users = await _authRepository.GetUserByUsernameAsync(username);

            if (users == null || users.Count == 0)
            {
                throw new KeyNotFoundException($"No se encontraron usuarios con username que contenga '{username}'.");
            }

            return users;
        }
    }
}