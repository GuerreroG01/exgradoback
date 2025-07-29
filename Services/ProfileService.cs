using ExGradoBack.Repositories;
using ExGradoBack.Models;

namespace ExGradoBack.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;

        public ProfileService(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public Task<IEnumerable<InfoUser>> GetAllProfilesAsync() => _profileRepository.GetAllProfilesAsync();

        public Task<InfoUser?> GetProfileByIdAsync(int authId) => _profileRepository.GetProfileByAuthIdAsync(authId);

        public Task<InfoUser> CreateProfileAsync(InfoUser profile, IFormFile? photoFile = null) 
            => _profileRepository.CreateProfileAsync(profile, photoFile);

        public Task<InfoUser> UpdateProfileAsync(InfoUser profile, IFormFile? photoFile = null) 
            => _profileRepository.UpdateProfileAsync(profile, photoFile);

        public Task<bool> ProfileExistsAsync(int authId)
        {
            return _profileRepository.GetProfileByAuthIdAsync(authId).ContinueWith(task => task.Result != null);
        }
    }
}