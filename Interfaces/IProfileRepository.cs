using ExGradoBack.Models;
namespace ExGradoBack.Repositories
{
    public interface IProfileRepository
    {
        Task<IEnumerable<InfoUser>> GetAllProfilesAsync();
        Task<InfoUser?> GetProfileByIdAsync(int authId);
        Task<InfoUser> CreateProfileAsync(InfoUser profile, IFormFile? photoFile = null);
        Task<InfoUser> UpdateProfileAsync(InfoUser profile, IFormFile? photoFile = null);
        Task<InfoUser?> GetProfileByAuthIdAsync(int authId);
    }
}