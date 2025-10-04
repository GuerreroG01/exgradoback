using ExGradoBack.Repositories;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Collections;
using System.Reflection;
using ExGradoBack.Models;
using ExGradoBack.DTOs;

namespace ExGradoBack.Services
{
    public class ActividadService : IActividadService
    {
        private readonly IActividadRepository _actividadRepository;
        private readonly ILogger<ActividadService> _logger;

        public ActividadService(IActividadRepository actividadRepository, ILogger<ActividadService> logger)
        {
            _actividadRepository = actividadRepository;
            _logger = logger;
        }

        public async Task RegistrarAsync(string usuario, string accion, int facturaId, object? antes, object? despues)
        {
            _logger.LogInformation(" Servicio : Registrando actividad: Usuario={usuario}, Accion={accion}, FacturaId={facturaId}, Antes={antes}, Despues={despues}",
                usuario, accion, facturaId,
                antes != null ? JsonSerializer.Serialize(antes) : "Derecha",
                despues != null ? JsonSerializer.Serialize(despues) : "Derecha");
            object? datosAntes = null;
            object? datosDespues = null;

            if (antes != null && despues != null)
            {
                var cambiosAntes = DiferenciadorDeCambios.GetOriginal(antes, despues);
                var cambiosDespues = DiferenciadorDeCambios.GetDiferencia(antes, despues, _logger);

                if (cambiosAntes.Count > 0)
                    datosAntes = cambiosAntes;
                else
                    datosAntes = new Dictionary<string, object?>();

                if (cambiosDespues.Count > 0)
                    datosDespues = cambiosDespues;
                else
                    datosDespues = new Dictionary<string, object?>();

                _logger.LogInformation("Datos antes para registrar: {datosAntes}", JsonSerializer.Serialize(datosAntes));
                _logger.LogInformation("Datos despues para registrar: {datosDespues}", JsonSerializer.Serialize(datosDespues));
            }
            else
            {
                datosAntes = antes;
                datosDespues = despues;

                _logger.LogInformation("No se compararon objetos, datosAntes: {datosAntes}, datosDespues: {datosDespues}",
                    JsonSerializer.Serialize(datosAntes), JsonSerializer.Serialize(datosDespues));
            }

            await _actividadRepository.RegistrarAsync(
                usuario,
                accion,
                facturaId,
                datosAntes != null ? JsonSerializer.Serialize(datosAntes) : null,
                datosDespues != null ? JsonSerializer.Serialize(datosDespues) : null
            );
        }
        public async Task<List<ActividadResumenDto>> ObtenerActividadesAsync(string usuario, string? accion = null, int? meses = null)
        {
            return await _actividadRepository.ObtenerActividadesAsync(usuario, accion, meses);
        }
        public async Task<ActividadFactura?> GetActividadByIdAsync(int id)
        {
            return await _actividadRepository.GetActividadByIdAsync(id);
        }
        public async Task LimpiarActividadesAsync()
        {
            await _actividadRepository.LimpiarActividadesAsync();
        }
    }

    public static class DiferenciadorDeCambios
    {
        public static Dictionary<string, object?> GetDiferencia(object before, object after, ILogger logger)
        {
            var differences = new Dictionary<string, object?>();

            if (before == null || after == null)
                return differences;

            var type = before.GetType();
            if (type != after.GetType())
                return differences;

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead) continue;

                var beforeValue = prop.GetValue(before);
                var afterValue = prop.GetValue(after);

                if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                {
                    var beforeEnum = (beforeValue as IEnumerable)?.Cast<object>().ToList();
                    var afterEnum = (afterValue as IEnumerable)?.Cast<object>().ToList();

                    if ((beforeEnum == null || !beforeEnum.Any()) && (afterEnum == null || !afterEnum.Any()))
                        continue;

                    var beforeJson = JsonSerializer.Serialize(beforeValue);
                    var afterJson = JsonSerializer.Serialize(afterValue);

                    if (beforeJson != afterJson)
                    {
                        logger.LogInformation(
                            "Cambio en propiedad colección '{Propiedad}': Antes = {Antes}, Después = {Despues}",
                            prop.Name, beforeJson, afterJson
                        );

                        differences[prop.Name] = afterValue;
                    }

                    continue;
                }

                if (!object.Equals(beforeValue, afterValue))
                {
                    logger.LogInformation(
                        "Cambio en propiedad '{Propiedad}': Antes = {Antes}, Después = {Despues}",
                        prop.Name, beforeValue, afterValue
                    );

                    differences[prop.Name] = afterValue;
                }
            }

            return differences;
        }

        public static Dictionary<string, object?> GetOriginal(object before, object after)
        {
            var originals = new Dictionary<string, object?>();

            if (before == null || after == null)
                return originals;

            var type = before.GetType();
            if (type != after.GetType())
                return originals;

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead) continue;

                var beforeValue = prop.GetValue(before);
                var afterValue = prop.GetValue(after);

                if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                {
                    var beforeEnum = (beforeValue as IEnumerable)?.Cast<object>().ToList();
                    var afterEnum = (afterValue as IEnumerable)?.Cast<object>().ToList();

                    if ((beforeEnum == null || !beforeEnum.Any()) && (afterEnum == null || !afterEnum.Any()))
                        continue;

                    var beforeJson = JsonSerializer.Serialize(beforeValue);
                    var afterJson = JsonSerializer.Serialize(afterValue);

                    if (beforeJson != afterJson)
                        originals[prop.Name] = beforeValue;

                    continue;
                }

                if (!object.Equals(beforeValue, afterValue))
                {
                    originals[prop.Name] = beforeValue;
                }
            }

            return originals;
        }
    }
}