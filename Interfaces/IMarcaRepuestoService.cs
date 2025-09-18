using ExGradoBack.Models;
using ExGradoBack.DTOs;
namespace ExGradoBack.Services
{
    public interface IMarcaRepuestoService
    {
        Task<List<MarcaRepuesto>> GetMarcaRepuestoPorCalificacionAsync(double calificacion);
        Task<MarcaRepuesto?> GetByNameAsync(string nombre);
        Task<MarcaRepuesto?> GetMarcaRepuestoByIdAsync(int id);
        Task<MarcaRepuesto> CreateMarcaRepuestoAsync(MarcaRepuesto marcaRepuesto);
        Task<MarcaRepuesto> UpdateMarcaRepuestoAsync(MarcaRepuesto marcaRepuesto);
        Task<bool> DeleteMarcaRepuestoAsync(int id);
        Task<bool> MarcaRepuestoExistsAsync(string nombre);
        Task<List<MarcaRepuestoUsadaDto>> ObtenerMarcasRepuestoMasUsadasAsync();
    }
}