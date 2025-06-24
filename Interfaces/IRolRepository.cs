using ExGradoBack.Models;

namespace ExGradoBack.Repositories
{
    public interface IRolRepository
    {
        Task<IEnumerable<Rol>> GetAllRolesAsync();
        Task<Rol?> GetRoleByIdAsync(int id);
        Task<Rol> CreateRoleAsync(Rol role);
        Task<bool> DeleteRoleAsync(int id);
        Task<bool> RoleExistsAsync(string roleName);
    }
}