using ExGradoBack.Repositories;
using ExGradoBack.Models;

namespace ExGradoBack.Services
{
    public class RolService : IRolService
    {
        private readonly IRolRepository _rolRepository;

        public RolService(IRolRepository rolRepository)
        {
            _rolRepository = rolRepository;
        }

        public Task<IEnumerable<Rol>> GetAllRolesAsync() => _rolRepository.GetAllRolesAsync();

        public Task<Rol?> GetRoleByIdAsync(int id) => _rolRepository.GetRoleByIdAsync(id);

        public Task<Rol> CreateRoleAsync(Rol role) => _rolRepository.CreateRoleAsync(role);

        public Task<bool> DeleteRoleAsync(int id) => _rolRepository.DeleteRoleAsync(id);

        public Task<bool> RoleExistsAsync(string roleName) => _rolRepository.RoleExistsAsync(roleName);
    }
}