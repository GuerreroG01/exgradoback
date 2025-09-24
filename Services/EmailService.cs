using ExGradoBack.Models;
using ExGradoBack.Services;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Logging;

namespace ExGradoBack.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpFrom;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly ILogger<EmailService> _logger;
        private readonly IRepuestoService _repuestoService;

        public EmailService(
            string smtpHost,
            int smtpPort,
            string smtpFrom,
            string smtpUser,
            string smtpPass,
            ILogger<EmailService> logger,
            IRepuestoService repuestoService
        )
        {
            _smtpHost = smtpHost;
            _smtpPort = smtpPort;
            _smtpFrom = smtpFrom;
            _smtpUser = smtpUser;
            _smtpPass = smtpPass;
            _logger = logger;
            _repuestoService = repuestoService;
        }

        public async Task SendOrdenCompraEmailAsync(OrdenCompra orden, List<DetalleOrdenCompra> detalles)
        {
            if (orden.Proveedor == null || string.IsNullOrWhiteSpace(orden.Proveedor.Email))
            {
                _logger.LogWarning("Proveedor sin email o nulo para la orden {OrdenId}", orden.Id);
                return;
            }

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Email", "OrdenCompra.html");
            string templateHtml = await File.ReadAllTextAsync(templatePath);

            var detallesHtml = new StringBuilder();
            foreach (var item in detalles)
            {
                string nombreRepuesto = "Desconocido";
                try
                {
                    var repuesto = await _repuestoService.GetRepuestoByIdAsync(item.RepuestoId);
                    nombreRepuesto = repuesto?.Nombre ?? "Desconocido";
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "No se pudo obtener el nombre del repuesto con ID {RepuestoId}", item.RepuestoId);
                }

                detallesHtml.AppendLine("<tr>");
                detallesHtml.AppendLine($"<td>{nombreRepuesto}</td>");
                detallesHtml.AppendLine($"<td>{item.Cantidad}</td>");
                detallesHtml.AppendLine($"<td>{item.PrecioProveedor:C}</td>");
                detallesHtml.AppendLine($"<td>{item.Subtotal:C}</td>");
                detallesHtml.AppendLine("</tr>");
            }

            var body = templateHtml
                .Replace("{{OrdenId}}", orden.Id.ToString())
                .Replace("{{Fecha}}", orden.Fecha.ToString("dd/MM/yyyy"))
                .Replace("{{Solicitante}}", orden.Solicitante)
                .Replace("{{ProveedorNombre}}", orden.Proveedor.Nombre)
                .Replace("{{Total}}", orden.Total.ToString("C"))
                .Replace("{{Detalles}}", detallesHtml.ToString());

            var message = new MailMessage(_smtpFrom, orden.Proveedor.Email, "Orden de Compra", body);
            message.IsBodyHtml = true;

            using var smtp = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true
            };

            try
            {
                _logger.LogInformation("Enviando correo a {EmailDestino} usando SMTP {Host}:{Port}", orden.Proveedor.Email, _smtpHost, _smtpPort);
                await smtp.SendMailAsync(message);
                _logger.LogInformation("Correo enviado exitosamente para orden {OrdenId}", orden.Id);
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, "Error SMTP al enviar correo de orden {OrdenId}: {Message}", orden.Id, smtpEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al enviar correo de orden {OrdenId}: {Message}", orden.Id, ex.Message);
                throw;
            }
        }
    }
}