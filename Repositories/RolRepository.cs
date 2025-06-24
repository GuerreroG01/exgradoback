using ExGradoBack.Data;
using ExGradoBack.Repositories;
using ExGradoBack.Models;
using Microsoft.EntityFrameworkCore;

namespace ExGradoBack.Repositories
{
    public class RolRepository : IRolRepository
    {
        private readonly AppDbContext _context;

        public RolRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rol>> GetAllRolesAsync()
        {
            return await _context.Rol.ToListAsync();
        }

        public async Task<Rol?> GetRoleByIdAsync(int id)
        {
            return await _context.Rol.FindAsync(id);
        }

        public async Task<Rol> CreateRoleAsync(Rol role)
        {
            _context.Rol.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            var role = await GetRoleByIdAsync(id);
            if (role == null) return false;

            _context.Rol.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _context.Rol.AnyAsync(r => r.Nombre == roleName);
        }
    }
}