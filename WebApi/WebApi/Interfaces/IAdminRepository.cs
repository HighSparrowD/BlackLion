using System.Threading.Tasks;
using System.Collections.Generic;
using WebApi.Entities.TestEntities;
using WebApi.Main.Models.Admin;
using WebApi.Main.Models.Report;
using WebApi.Models.Models.Admin;

namespace WebApi.Interfaces
{
    public interface IAdminRepository
    {
        Task<long> UploadCountries(List<UpdateCountry> countries);
        Task<long> UploadCities(List<UpdateCity> cities);
        Task<long> UploadLanguages(List<UpdateLanguage> langs);
        Task<byte> UploadTestsAsync(List<UploadTest> model);
        //Task<long> UploadInTest(UploadInTest model);
        Task<List<Feedback>> GetFeedbacks ();
        Task<int> DeleteAllUsers ();
        Task AddAchievementsAsync(List<UploadAchievement> achievements);
        Task<List<TickRequest>> GetTickRequestsAsync();
        Task<TickRequest> GetTickRequestAsync(long? requestId = null);
        Task<string> GetNewNotificationsCountAsync(long adminId);
        Task<bool> ResolveTickRequestAsync(ResolveTickRequest request);
        Task<bool> AbortTickRequestAsync(long requestId);
        Task<bool> NotifyFailierTickRequestAsync(long requestId, long adminId);
        Task<List<long>> GetRecentlyBannedUsersAsync();
    }
}
