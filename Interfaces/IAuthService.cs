using System.Collections.Generic;
using System.Threading.Tasks;
using ExGradoBack.Models;

namespace ExGradoBack.Services
{
    public interface IAuthService
    {
        Task<IEnumerable<Auth>> GetAllUsersAsync();
        Task<Auth?> GetUserByIdAsync(int id);
        Task<Auth> CreateUserAsync(Auth newUser);
        Task<Auth> UpdateUserAsync(Auth updatedUser);
        Task<bool> DeleteUserAsync(int id);

        Task<string?> LoginAsync(string username, string password, bool isLogin);
        Task<Auth> RegisterAsync(RegisterDto dto);

        Task<bool> UserExistsAsync(string username);
        Task<Auth> GetUserByUsernameAsync(string username);
        String GenerateJwtToken(Auth user);
    }
}