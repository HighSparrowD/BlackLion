using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Main.Entities.Admin;
using WebApi.Main.Models.Admin;
using WebApi.Models.Models.Report;
using models = WebApi.Models.Models;

namespace WebApi.Interfaces.Services
{
    public interface IAdminService
    {
        Task StartInDebug(List<long> userIds);

        Task<List<GroupedFeedback>> GetGroupedFeedbackAsync();

        Task<List<Feedback>> GetRecentFeedbackAsync();

        Task<List<Report>> GetRecentReportsAsync();

        Task<List<models.Admin.VerificationRequest>> GetVerificationRequestsAsync();

		Task ResolveVerificationRequestAsync(ResolveVerificationRequest request);

		Task<models.Admin.RecentUpdates> GetRecentUpdatesAsync();

        Task<ICollection<models.Sponsor.Advertisement>> GetPendingtAdvertisementsAsync();

        Task<models.Sponsor.Advertisement> ResolveAdvertisementsAsync(ResolveAdvertisement request);

        Task<ICollection<models.Adventure.Adventure>> GetPendingAdventuresAsync();
        
        Task<models.Adventure.Adventure> ResolveAdventureAsync(ResolveAdventure request);
    }
}
