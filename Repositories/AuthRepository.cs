using ExGradoBack.Models;
using ExGradoBack.Data;
using Microsoft.EntityFrameworkCore;
using ExGradoBack.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Auth>> GetAllAsync()
    {
        return await _context.Auth.Include(u => u.InfoUser).ToListAsync();
    }

    public async Task<Auth?> GetByIdAsync(int id)
    {
        return await _context.Auth.Include(u => u.InfoUser)
                                        .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Auth?> GetByUsernameAsync(string username)
    {
        return await _context.Auth
                            .Include(u => u.InfoUser)
                            .Include(a => a.Rol)
                            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<Auth> AddAsync(Auth user)
    {
        _context.Auth.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<Auth> UpdateAsync(Auth user)
    {
        _context.Auth.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await GetByIdAsync(id);
        if (user == null) return false;

        _context.Auth.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}