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
            object? datosAntes = null;
            object? datosDespues = null;

            if (antes != null && despues != null)
            {
                var cambiosAntes = DiferenciadorDeCambios.GetOriginal(antes, despues);
                var cambiosDespues = DiferenciadorDeCambios.GetDiferencia(antes, despues, _logger);

                datosAntes = cambiosAntes.Count > 0 ? cambiosAntes : new Dictionary<string, object?>();
                datosDespues = cambiosDespues.Count > 0 ? cambiosDespues : new Dictionary<string, object?>();

                _logger.LogInformation("Datos antes: {datosAntes}", JsonSerializer.Serialize(datosAntes));
                _logger.LogInformation("Datos despues: {datosDespues}", JsonSerializer.Serialize(datosDespues));
            }
            else
            {
                datosAntes = antes;
                datosDespues = despues;
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
        private static readonly HashSet<string> _propiedadesIgnoradas = new()
        {
            "Factura", "Repuesto", "MarcaRepuesto", "VehiculoInfoIds"
        };

        public static Dictionary<string, object?> GetDiferencia(object before, object after, ILogger? logger = null)
        {
            var differences = new Dictionary<string, object?>();

            if (before == null || after == null) return differences;
            var type = before.GetType();
            if (type != after.GetType()) return differences;

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead) continue;
                if (_propiedadesIgnoradas.Contains(prop.Name)) continue;

                var beforeValue = prop.GetValue(before);
                var afterValue = prop.GetValue(after);

                if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                {
                    var beforeList = (beforeValue as IEnumerable)?.Cast<object>().ToList() ?? new List<object>();
                    var afterList = (afterValue as IEnumerable)?.Cast<object>().ToList() ?? new List<object>();

                    var itemType = prop.PropertyType.GetGenericArguments().FirstOrDefault();
                    if (itemType == null) continue;

                    var idProp = itemType.GetProperty("Id");
                    if (idProp == null) continue;

                    var repuestoProp = itemType.GetProperty("Repuesto");
                    var cambiosLista = new List<Dictionary<string, object?>>();

                    foreach (var afterItem in afterList)
                    {
                        var afterId = idProp.GetValue(afterItem);
                        var beforeItem = beforeList.FirstOrDefault(b => idProp.GetValue(b)?.Equals(afterId) == true);
                        if (beforeItem == null) continue;

                        var subCambios = GetDiferencia(beforeItem, afterItem, logger);

                        string? nombreRepuesto = null;
                        if (repuestoProp != null)
                        {
                            var repuestoObj = repuestoProp.GetValue(afterItem);
                            var nombreProp = repuestoObj?.GetType().GetProperty("Nombre");
                            nombreRepuesto = nombreProp?.GetValue(repuestoObj)?.ToString();
                        }

                        if (subCambios.Count > 0)
                        {
                            var resumen = new Dictionary<string, object?>();
                            if (!string.IsNullOrEmpty(nombreRepuesto))
                                resumen["Repuesto"] = nombreRepuesto;

                            foreach (var kv in subCambios)
                                if (kv.Key != "Id")
                                    resumen[kv.Key] = kv.Value;

                            if (resumen.Count > 0)
                                cambiosLista.Add(resumen);
                        }
                    }

                    if (cambiosLista.Count > 0)
                        differences[prop.Name] = cambiosLista;

                    continue;
                }

                if (!Equals(beforeValue, afterValue))
                    differences[prop.Name] = afterValue;
            }

            return differences;
        }

        public static Dictionary<string, object?> GetOriginal(object before, object after, ILogger? logger = null)
        {
            var originals = new Dictionary<string, object?>();

            if (before == null || after == null) return originals;
            var type = before.GetType();
            if (type != after.GetType()) return originals;

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead) continue;
                if (_propiedadesIgnoradas.Contains(prop.Name)) continue;

                var beforeValue = prop.GetValue(before);
                var afterValue = prop.GetValue(after);

                if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                {
                    var beforeList = (beforeValue as IEnumerable)?.Cast<object>().ToList() ?? new List<object>();
                    var afterList = (afterValue as IEnumerable)?.Cast<object>().ToList() ?? new List<object>();

                    var itemType = prop.PropertyType.GetGenericArguments().FirstOrDefault();
                    if (itemType == null) continue;

                    var idProp = itemType.GetProperty("Id");
                    if (idProp == null) continue;

                    var repuestoProp = itemType.GetProperty("Repuesto");
                    var cambiosLista = new List<Dictionary<string, object?>>();

                    foreach (var beforeItem in beforeList)
                    {
                        var beforeId = idProp.GetValue(beforeItem);
                        var afterItem = afterList.FirstOrDefault(a => idProp.GetValue(a)?.Equals(beforeId) == true);
                        if (afterItem == null) continue;

                        var subCambios = GetOriginal(beforeItem, afterItem, logger);

                        var resumen = new Dictionary<string, object?>();

                        if (repuestoProp != null)
                        {
                            var repuestoObj = repuestoProp.GetValue(beforeItem);
                            var nombreProp = repuestoObj?.GetType().GetProperty("Nombre");
                            var nombreRepuesto = nombreProp?.GetValue(repuestoObj)?.ToString();
                            if (!string.IsNullOrEmpty(nombreRepuesto))
                                resumen["Repuesto"] = nombreRepuesto;
                        }

                        foreach (var kv in subCambios)
                            if (kv.Key != "Id")
                                resumen[kv.Key] = kv.Value;

                        if (resumen.Count > 0)
                            cambiosLista.Add(resumen);
                    }

                    if (cambiosLista.Count > 0)
                        originals[prop.Name] = cambiosLista;

                    continue;
                }

                if (!Equals(beforeValue, afterValue))
                    originals[prop.Name] = beforeValue;
            }

            return originals;
        }
    }
}