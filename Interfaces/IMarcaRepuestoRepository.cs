using ExGradoBack.Models;
namespace ExGradoBack.Repositories
{
    public interface IMarcaRepuestoRepository
    {
        Task<MarcaRepuesto> GetMarcaRepuestoAsync(string nombre);
        Task<MarcaRepuesto?> GetMarcaRepuestoByIdAsync(int id);
        Task<MarcaRepuesto> CreateMarcaRepuestoAsync(MarcaRepuesto marcaRepuesto);
        Task<MarcaRepuesto> UpdateMarcaRepuestoAsync(MarcaRepuesto marcaRepuesto);
        Task<bool> DeleteMarcaRepuestoAsync(int id);
        Task<bool> MarcaRepuestoExistsAsync(string nombre);
    }
}