using ExGradoBack.Models;

namespace ExGradoBack.Services
{
    public interface IRolService
    {
        Task<IEnumerable<Rol>> GetAllRolesAsync();
        Task<Rol?> GetRoleByIdAsync(int id);
        Task<Rol> CreateRoleAsync(Rol role);
        Task<bool> DeleteRoleAsync(int id);
        Task<bool> RoleExistsAsync(string roleName);
    }
}