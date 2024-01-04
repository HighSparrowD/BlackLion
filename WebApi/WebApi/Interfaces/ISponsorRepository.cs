using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Enums.Enums.Sponsor;
using WebApi.Models.Models.Sponsor;
using WebApi.Models.Models.User;

namespace WebApi.Interfaces
{
    public interface ISponsorRepository
    {
        Task<List<AdvertisementItem>> GetAdvertisementListAsync(int sponsorId);
        Task<Advertisement> GetAdvertisementAsync(int advertisementId);
        Task<List<AdvertisementStats>> GetAdvertisementStatsAsync(long advertisementId);
        Task<List<AdvertisementStats>> GetAllAdvertisementsStatsAsync(long userId);
        Task AddAdvertisementAsync(AdvertisementNew model);
        Task UpdateAdvertisementAsync(AdvertisementUpdate model);
        Task DeleteAdvertisementAsync(int advertisementId);
        List<GetLocalizedEnum> GetPrioritiesAsync();
        Task SetAdvertisementPriorityAsync(int advertisementId, AdvertisementPriority priority);
        Task SwitchShowStatusAsync(int advertisementId);
    }
}
