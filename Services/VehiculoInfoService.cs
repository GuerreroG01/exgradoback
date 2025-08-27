using ExGradoBack.Models;
using ExGradoBack.Repositories;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using ExGradoBack.DTOs;

namespace ExGradoBack.Services
{
    public class VehiculoInfoService : IVehiculoInfoService
    {
        private readonly IVehiculoInfoRepository _vehiculoInfoRepository;
        private readonly string _imageReferencePath;
        private readonly IWebHostEnvironment _env;

        public VehiculoInfoService(IVehiculoInfoRepository vehiculoInfoRepository, IWebHostEnvironment env)
        {
            _vehiculoInfoRepository = vehiculoInfoRepository;
            _env = env;
            _imageReferencePath = Path.Combine(_env.WebRootPath, "vehiculoReference_photos");
            if (!Directory.Exists(_imageReferencePath))
            {
                Directory.CreateDirectory(_imageReferencePath);
            }
        }

        public async Task<IEnumerable<VehiculoInfo>> GetVehiculoInfosFullAsync(string? marca, int? anio)
        {
            var resultado = await _vehiculoInfoRepository.GetVehiculoInfosByMarcaAndAnioAsync(marca, anio, false);
            if (!resultado.Any())
            {
                throw new InvalidOperationException("No se encontraron vehículos con los filtros proporcionados.");
            }

            // Cast segura porque sabes que isMinInfo = false devuelve VehiculoInfo
            return resultado.Cast<VehiculoInfo>();
        }
        public async Task<IEnumerable<VehiculoInfoMinDto>> GetVehiculoInfosMinAsync(string? marca, int? anio)
        {
            var resultado = await _vehiculoInfoRepository.GetVehiculoInfosByMarcaAndAnioAsync(marca, anio, true);
            if (!resultado.Any())
            {
                throw new InvalidOperationException("No se encontraron vehículos con los filtros proporcionados.");
            }

            // Cast segura porque sabes que isMinInfo = true devuelve VehiculoInfoMinDto
            return resultado.Cast<VehiculoInfoMinDto>();
        }

        public Task<VehiculoInfo?> GetVehiculoInfoByIdAsync(int id)
            => _vehiculoInfoRepository.GetVehiculoInfoByIdAsync(id);

        public async Task<VehiculoInfo> CreateVehiculoInfoAsync(VehiculoInfo vehiculoInfo, IFormFile? fotoReferencia)
        {
            var context = new ValidationContext(vehiculoInfo);
            Validator.ValidateObject(vehiculoInfo, context, validateAllProperties: true);

            if (vehiculoInfo.Anio <= 1900 || vehiculoInfo.Anio > DateTime.Now.Year)
            {
                throw new ArgumentOutOfRangeException(nameof(vehiculoInfo.Anio),
                    "El año debe ser mayor a 1900 y no puede ser mayor al año actual.");
            }

            var imagePath = _imageReferencePath ?? throw new InvalidOperationException("Ruta de la imagen no configurada correctamente.");

            
            if (fotoReferencia != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(fotoReferencia.FileName);
                var fullPath = Path.Combine(imagePath, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await fotoReferencia.CopyToAsync(stream);

                vehiculoInfo.FotoReferencia = fileName;
            }
            await _vehiculoInfoRepository.CreateVehiculoInfoAsync(vehiculoInfo);

            return vehiculoInfo;
        
        }
        public async Task<VehiculoInfo> UpdateVehiculoInfoAsync(VehiculoInfo vehiculoInfo, IFormFile? fotoReferencia)
        {
            var context = new ValidationContext(vehiculoInfo);
            Validator.ValidateObject(vehiculoInfo, context, validateAllProperties: true);

            if (vehiculoInfo.Anio <= 1900 || vehiculoInfo.Anio > DateTime.Now.Year)
            {
                throw new ArgumentOutOfRangeException(nameof(vehiculoInfo.Anio),
                    "El año debe ser mayor a 1900 y no puede ser mayor al año actual.");
            }

            var imagePath = _imageReferencePath ?? throw new InvalidOperationException("Ruta de la imagen no configurada correctamente.");
            try
            {
                if (fotoReferencia != null)
                {
                    var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(fotoReferencia.FileName);
                    var fullPath = Path.Combine(imagePath, newFileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await fotoReferencia.CopyToAsync(stream);

                    if (!string.IsNullOrWhiteSpace(vehiculoInfo.FotoReferencia))
                    {
                        var oldImagePath = Path.Combine(imagePath, vehiculoInfo.FotoReferencia);
                        if (File.Exists(oldImagePath))
                        {
                            File.Delete(oldImagePath);
                        }
                    }

                    vehiculoInfo.FotoReferencia = newFileName;
                }
                await _vehiculoInfoRepository.UpdateVehiculoInfoAsync(vehiculoInfo);

                return vehiculoInfo;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error al guardar la información del vehículo.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar la imagen de referencia.", ex);
            }
        }

        public Task<bool> DeleteVehiculoInfoAsync(int id)
            => _vehiculoInfoRepository.DeleteVehiculoInfoAsync(id);

        public Task<bool> VehiculoInfoExistsAsync(string modelo)
            => _vehiculoInfoRepository.VehiculoInfoExistsAsync(modelo);
        public async Task<IEnumerable<string>> GetMarcasVehiculosAsync()
        {
            var resultado = await _vehiculoInfoRepository.GetMarcasVehiculosAsync();
            if (resultado == null || !resultado.Any())
            {
                throw new InvalidOperationException("No se encontraron marcas de vehículos.");
            }
            return resultado;
        }

        public async Task<IEnumerable<int>> GetAniosVehiculosAsync(string marca)
        {
            var resultado = await _vehiculoInfoRepository.GetAniosVehiculosAsync(marca);
            if (resultado == null || !resultado.Any())
            {
                throw new InvalidOperationException("No se encontraron años de vehículos para la marca especificada.");
            }
            return resultado;
        }
    }
}