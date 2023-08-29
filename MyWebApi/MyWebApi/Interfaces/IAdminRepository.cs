using System.Threading.Tasks;
using System.Collections.Generic;
using WebApi.Entities.ReportEntities;
using WebApi.Entities.AchievementEntities;
using WebApi.Entities.AdminEntities;
using WebApi.Entities.TestEntities;
using WebApi.Entities;
using WebApi.Enums;

namespace WebApi.Interfaces
{
    public interface IAdminRepository
    {
        Task<bool> CheckUserIsAdmin(long userId);
        Task<byte> SwitchAdminStatus(long userId);
        Task<bool?> GetAdminStatus(long userId);
        Task<long> UploadCountries(List<UpdateCountry> countries);
        Task<long> UploadCities(List<UpdateCity> cities);
        Task<long> UploadLanguages(List<UpdateLanguage> langs);
        Task<byte> UploadTestsAsync(List<UploadTest> model);
        //Task<long> UploadInTest(UploadInTest model);
        Task<List<Feedback>> GetFeedbacks ();
        Task<long> DeleteUser (long userId);
        Task<int> DeleteAllUsers ();
        Task<byte> UploadAchievements(List<Achievement> achievements);
        Task<byte> AddNewAchievements(List<Achievement> achievements);
        Task<List<TickRequest>> GetTickRequestsAsync();
        Task<TickRequest> GetTickRequestAsync(long? requestId = null);
        Task<string> GetNewNotificationsCountAsync(long adminId);
        Task<bool> ResolveTickRequestAsync(ResolveTickRequest request);
        Task<bool> AbortTickRequestAsync(long requestId);
        Task<bool> NotifyFailierTickRequestAsync(long requestId, long adminId);
        Task<List<long>> GetRecentlyBannedUsersAsync();
    }
}
