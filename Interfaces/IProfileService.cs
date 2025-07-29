using ExGradoBack.Models;
namespace ExGradoBack.Services
{
    public interface IProfileService
    {
        Task<IEnumerable<InfoUser>> GetAllProfilesAsync();
        Task<InfoUser?> GetProfileByIdAsync(int id);
        Task<InfoUser> CreateProfileAsync(InfoUser profile, IFormFile? photoFile = null);
        Task<InfoUser> UpdateProfileAsync(InfoUser profile, IFormFile? photoFile = null);

        Task<bool> ProfileExistsAsync(int authId);
    }
}