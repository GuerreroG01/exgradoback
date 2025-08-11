using ExGradoBack.Models;
namespace ExGradoBack.Services
{
    public interface IMarcaRepuestoService
    {
        Task<MarcaRepuesto> GetMarcaRepuestoAsync(string nombre);
        Task<MarcaRepuesto?> GetMarcaRepuestoByIdAsync(int id);
        Task<MarcaRepuesto> CreateMarcaRepuestoAsync(MarcaRepuesto marcaRepuesto);
        Task<MarcaRepuesto> UpdateMarcaRepuestoAsync(MarcaRepuesto marcaRepuesto);
        Task<bool> DeleteMarcaRepuestoAsync(int id);
        Task<bool> MarcaRepuestoExistsAsync(string nombre);
    }
}