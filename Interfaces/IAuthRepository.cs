using System.Collections.Generic;
using System.Threading.Tasks;
using ExGradoBack.Models;

namespace ExGradoBack.Repositories
{
    public interface IAuthRepository
    {
        Task<IEnumerable<Auth>> GetAllAsync();
        Task<Auth?> GetByIdAsync(int id);
        Task<Auth?> GetByUsernameAsync(string username);
        Task<Auth> AddAsync(Auth newUser);
        Task<Auth> UpdateAsync(Auth updatedUser);
        Task<bool> DeleteAsync(int id);
        Task<int> CountUsersAsync();
        Task SaveRefreshTokenAsync(int userId, string token);
        Task<RefreshToken?> GetRefreshTokenAsync(int userId, string token);
    }
}