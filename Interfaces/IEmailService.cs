using ExGradoBack.Models;
namespace ExGradoBack.Services
{
    public interface IEmailService
    {
        Task SendOrdenCompraEmailAsync(OrdenCompra orden, List<DetalleOrdenCompra> detalles);
    }
}