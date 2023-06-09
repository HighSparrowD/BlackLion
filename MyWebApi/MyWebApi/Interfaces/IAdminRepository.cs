using System.Threading.Tasks;
using System.Collections.Generic;
using WebApi.Entities.SecondaryEntities;
using WebApi.Entities.ReportEntities;
using WebApi.Entities.ReasonEntities;
using WebApi.Entities.AchievementEntities;
using WebApi.Entities.DailyTaskEntities;
using WebApi.Entities.AdminEntities;
using System;
using WebApi.Entities.TestEntities;
using WebApi.Entities.UserActionEntities;
using WebApi.Entities;

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
        Task<byte> UploadPsTestsAsync(List<UploadTest> model);
        //Task<long> UploadInTest(UploadInTest model);
        Task<long> UploadFeedbackReasons(List<FeedbackReason> reasons);
        Task<List<Feedback>> GetFeedbacks ();
        Task<long> DeleteUser (long userId);
        Task<int> DeleteAllUsers ();
        Task<byte> UploadAchievements(List<Achievement> achievements);
        Task<byte> AddNewAchievements(List<Achievement> achievements);
        Task<byte> AddDailyTaskAsync(DailyTask model);
        Task<List<TickRequest>> GetTickRequestsAsync();
        Task<TickRequest> GetTickRequestAsync(Guid? requestId = null);
        Task<string> GetNewNotificationsCountAsync(long adminId);
        Task<bool> ResolveTickRequestAsync(ResolveTickRequest request);
        Task<bool> AbortTickRequestAsync(Guid requestId);
        Task<bool> NotifyFailierTickRequestAsync(Guid requestId, long adminId);
        Task<List<long>> GetRecentlyBannedUsersAsync();
    }
}
