using ExGradoBack.Data;
using ExGradoBack.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace ExGradoBack.Repositories
{
    public class VehiculoInfoRepository : IVehiculoInfoRepository
    {
        private readonly AppDbContext _context;

        public VehiculoInfoRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<VehiculoInfo>> GetVehiculoInfosByMarcaAndAnioAsync(string? marca, int? anio)
        {
            var query = _context.VehiculoInfo.AsQueryable();

            if (!string.IsNullOrWhiteSpace(marca))
            {
                query = query.Where(v => v.Marca == marca);
            }

            if (anio.HasValue)
            {
                query = query.Where(v => v.Anio == anio.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<VehiculoInfo?> GetVehiculoInfoByIdAsync(int id)
        {
            return await _context.VehiculoInfo.FindAsync(id);
        }

        public async Task<VehiculoInfo> CreateVehiculoInfoAsync(VehiculoInfo vehiculoInfo)
        {
            _context.VehiculoInfo.Add(vehiculoInfo);
            await _context.SaveChangesAsync();
            return vehiculoInfo;
        }
        public async Task<VehiculoInfo> UpdateVehiculoInfoAsync(VehiculoInfo vehiculoInfo)
        {
            _context.VehiculoInfo.Update(vehiculoInfo);
            await _context.SaveChangesAsync();
            return vehiculoInfo;
        }

        public async Task<bool> DeleteVehiculoInfoAsync(int id)
        {
            var vehiculoInfo = await GetVehiculoInfoByIdAsync(id);
            if (vehiculoInfo == null) return false;

            _context.VehiculoInfo.Remove(vehiculoInfo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VehiculoInfoExistsAsync(string modelo)
        {
            return await _context.VehiculoInfo.AnyAsync(v => v.Modelo == modelo);
        }
        public async Task<IEnumerable<int>> GetAniosVehiculosAsync(string marca)
        {
            var anios = await _context.VehiculoInfo
                .Where(v => v.Marca == marca)
                .Select(v => v.Anio)
                .Distinct()
                .OrderByDescending(a => a)
                .ToListAsync();
            return anios;
        }
        public async Task<IEnumerable<string>> GetMarcasVehiculosAsync()
        {
            var marcas = await _context.VehiculoInfo
                .Select(v => v.Marca)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
            return marcas;
        }
    }
}