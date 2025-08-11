using ExGradoBack.Data;
using ExGradoBack.Models;
using Microsoft.EntityFrameworkCore;

namespace ExGradoBack.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProfileRepository> _logger;
        private readonly string _imagePath;
        private readonly string _defaultImageName = "Default.png";
        private readonly IWebHostEnvironment _env;

        public ProfileRepository(AppDbContext context, ILogger<ProfileRepository> logger, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _env = env;
            _imagePath = Path.Combine(_env.WebRootPath, "profile_photos");
            if (!Directory.Exists(_imagePath))
            {
                Directory.CreateDirectory(_imagePath);
            }
        }

        public async Task<IEnumerable<InfoUser>> GetAllProfilesAsync()
        {
            return await _context.InfoUsers.ToListAsync();
        }

        public async Task<InfoUser?> GetProfileByIdAsync(int authId)
        {
            return await _context.InfoUsers.FirstOrDefaultAsync(u => u.AuthId == authId);
        }
        /*private async Task<string?> SaveProfilePhotoAsync(IFormFile? photoFile)
        {
            try
            {
                string nombreArchivo;

                if (photoFile != null)
                {
                    nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(photoFile.FileName);
                    var rutaArchivo = Path.Combine(_imagePath ?? string.Empty, nombreArchivo);

                    await using var stream = new FileStream(rutaArchivo, FileMode.Create);
                    await photoFile.CopyToAsync(stream);
                }
                else
                {
                    var rutaImagenDefault = Path.Combine(_imagePath ?? string.Empty, _defaultImageName);
                    if (!System.IO.File.Exists(rutaImagenDefault))
                    {
                        return null; // Maneja este null en los métodos que lo usan si es necesario
                    }

                    nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(rutaImagenDefault);
                    var nuevaRuta = Path.Combine(_imagePath ?? string.Empty, nombreArchivo);
                    System.IO.File.Copy(rutaImagenDefault, nuevaRuta);
                }

                return nombreArchivo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la imagen de perfil.");
                return null;
            }
        }*/

        public async Task<InfoUser> CreateProfileAsync(InfoUser profile, IFormFile? photoFile)
        {
            if (profile == null)
            {
                _logger.LogWarning("Se intentó crear un perfil con un objeto nulo.");
                throw new ArgumentNullException(nameof(profile));
            }

            _logger.LogInformation("Iniciando creación de perfil para AuthId: {AuthId}", profile.AuthId);

            var imagePath = _imagePath ?? throw new InvalidOperationException("La ruta de la imagen no está configurada.");
            _logger.LogInformation("Ruta de imágenes: {ImagePath}", imagePath);

            var authExists = await _context.Auth.AnyAsync(a => a.Id == profile.AuthId);
            if (!authExists)
            {
                _logger.LogWarning("No existe un usuario con AuthId: {AuthId}", profile.AuthId);
                throw new ArgumentException($"No existe un usuario con el AuthId {profile.AuthId}");
            }

            var profileExists = await _context.InfoUsers.AnyAsync(p => p.AuthId == profile.AuthId);
            if (profileExists)
            {
                _logger.LogWarning("Ya existe un perfil para AuthId: {AuthId}", profile.AuthId);
                throw new InvalidOperationException($"El usuario con AuthId {profile.AuthId} ya tiene un perfil.");
            }

            try
            {
                if (photoFile != null)
                {
                    var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(photoFile.FileName);
                    var filePath = Path.Combine(imagePath, newFileName);

                    _logger.LogInformation("Guardando foto del usuario en: {FilePath}", filePath);

                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await photoFile.CopyToAsync(stream);

                    profile.FotoPerfil = newFileName;
                }
                else
                {
                    var defaultImagePath = Path.Combine(imagePath, _defaultImageName);
                    if (!System.IO.File.Exists(defaultImagePath))
                    {
                        _logger.LogError("No se encontró la imagen predeterminada en: {DefaultImagePath}", defaultImagePath);
                        throw new FileNotFoundException("No se encontró la imagen predeterminada.", defaultImagePath);
                    }

                    var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(defaultImagePath);
                    var newFilePath = Path.Combine(imagePath, newFileName);

                    _logger.LogInformation("Usando imagen predeterminada para el perfil. Copiando a: {NewFilePath}", newFilePath);
                    System.IO.File.Copy(defaultImagePath, newFilePath);

                    profile.FotoPerfil = newFileName;
                }

                _context.InfoUsers.Add(profile);
                _logger.LogInformation("Agregando perfil a la base de datos para AuthId: {AuthId}", profile.AuthId);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Perfil creado exitosamente con ID: {Id}", profile.Id);
                return profile;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al guardar el perfil en la base de datos para AuthId: {AuthId}", profile.AuthId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear el perfil para AuthId: {AuthId}", profile.AuthId);
                throw;
            }
        }

        public async Task<InfoUser> UpdateProfileAsync(InfoUser profile, IFormFile? photoFile = null)
        {
            _logger.LogInformation("Iniciando actualización del perfil con AuthId: {AuthId}", profile.AuthId);

            var existingProfile = await _context.InfoUsers.FirstOrDefaultAsync(p => p.AuthId == profile.AuthId);
            if (existingProfile == null)
            {
                _logger.LogWarning("Perfil no encontrado para AuthId: {AuthId}", profile.AuthId);
                throw new KeyNotFoundException($"No se encontró el perfil con AuthId {profile.AuthId}.");
            }

            var imagePath = _imagePath ?? throw new InvalidOperationException("La ruta de la imagen no está configurada.");
            try
            {
                existingProfile.Nombres = profile.Nombres;
                existingProfile.Apellidos = profile.Apellidos;
                existingProfile.Email = profile.Email;
                existingProfile.Nacimiento = profile.Nacimiento;
                existingProfile.Genero = profile.Genero;
                existingProfile.Telefono = profile.Telefono;

                if (photoFile != null)
                {
                    _logger.LogInformation("Se recibió una nueva foto de perfil para AuthId: {AuthId}", profile.AuthId);

                    var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(photoFile.FileName);
                    var filePath = Path.Combine(imagePath, newFileName);

                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await photoFile.CopyToAsync(stream);

                    if (!string.IsNullOrEmpty(existingProfile.FotoPerfil) && existingProfile.FotoPerfil != _defaultImageName)
                    {
                        var oldFilePath = Path.Combine(imagePath, existingProfile.FotoPerfil);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    existingProfile.FotoPerfil = newFileName;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Perfil actualizado exitosamente para AuthId: {AuthId}", profile.AuthId);
                return existingProfile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el perfil con AuthId: {AuthId}", profile.AuthId);
                throw;
            }
        }

        public async Task<InfoUser?> GetProfileByAuthIdAsync(int authId)
        {
            return await _context.InfoUsers
                .FirstOrDefaultAsync(p => p.AuthId == authId);
        }
    }
}