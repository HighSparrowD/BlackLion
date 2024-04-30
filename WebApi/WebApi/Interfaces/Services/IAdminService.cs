using System.Collections.Generic;
using System.Threading.Tasks;
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

        Task<models.Admin.RecentUpdates> GetRecentUpdates();
    }
}
