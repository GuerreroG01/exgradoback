using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using ExGradoBack.Models;
using ExGradoBack.DTOs;

namespace ExGradoBack.Services
{
    public interface IAuthService
    {
        Task<IEnumerable<Auth>> GetAllUsersAsync();
        Task<Auth?> GetUserByIdAsync(int id);
        Task<Auth> CreateUserAsync(Auth newUser);
        Task<Auth> UpdateUserAsync(Auth updatedUser);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ValidateCredentialsAsync(string username, string password);
        Task<TokenResponse?> LoginAsync(string username, string password);
        Task<Auth> RegisterAsync(RegisterDto dto);

        Task<bool> UserExistsAsync(string username);
        string GenerateJwtToken(Auth admin);
        string GenerateRefreshToken();
        Task<Auth> GetUserByUsernameAsync(string username);
        Task<RefreshToken?> GetRefreshTokenAsync(int userId, string token);
        Task<int> GetTotalUsersAsync();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}