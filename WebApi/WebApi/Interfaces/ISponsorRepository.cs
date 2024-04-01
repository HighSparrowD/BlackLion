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

        Task<List<AdvertisementEconomyStats>> GetAdvertisementEconomyStatsAsync(long userId, AdvertisementStatsRequest searchModel, int? addId = null);
        
        Task<List<AdvertisementEngagementStats>> GetAdvertisementEngagementStatsAsync(long userId, AdvertisementStatsRequest searchModel, int? addId = null);

        Task<Advertisement> AddAdvertisementAsync(AdvertisementNew model);

        Task UpdateAdvertisementAsync(AdvertisementUpdate model);

        Task DeleteAdvertisementAsync(int advertisementId);

        List<GetLocalizedEnum> GetPriorities();

        Task SetAdvertisementPriorityAsync(int advertisementId, AdvertisementPriority priority);

        Task SwitchShowStatusAsync(int advertisementId);
    }
}
