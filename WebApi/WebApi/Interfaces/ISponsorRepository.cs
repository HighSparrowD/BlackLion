using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Enums.Enums.Sponsor;
using WebApi.Main.Models.Admin;
using WebApi.Models.Models.Sponsor;
using WebApi.Models.Models.User;
using entities = WebApi.Main.Entities.Sponsor;

namespace WebApi.Interfaces
{
    public interface ISponsorRepository
    {
        Task<List<AdvertisementItem>> GetAdvertisementListAsync(long sponsorId);

        Task<ICollection<entities.Advertisement>> GetPendingAdvertisementsAsync();

        Task<entities.Advertisement> GetAdvertisementAsync(long advertisementId);

        Task<entities.Advertisement> ResolveAdvertisement(ResolveAdvertisement model);

		Task<List<AdvertisementEconomyStats>> GetAdvertisementEconomyStatsAsync(long userId, AdvertisementStatsRequest searchModel, long? addId = null);
        
        Task<List<AdvertisementEngagementStats>> GetAdvertisementEngagementStatsAsync(long userId, AdvertisementStatsRequest searchModel, long? addId = null);

        Task<Advertisement> AddAdvertisementAsync(AdvertisementNew model);

        Task UpdateAdvertisementAsync(AdvertisementUpdate model);

        Task DeleteAdvertisementAsync(long advertisementId);

        List<GetLocalizedEnum> GetPriorities();

        Task SetAdvertisementPriorityAsync(long advertisementId, AdvertisementPriority priority);

        Task SwitchShowStatusAsync(long advertisementId);
    }
}
