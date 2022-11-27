using System.Threading.Tasks;
using System.Collections.Generic;
using MyWebApi.Entities.SecondaryEntities;
using MyWebApi.Entities.ReportEntities;
using MyWebApi.Entities.ReasonEntities;
using MyWebApi.Entities.LocationEntities;
using MyWebApi.Entities.AchievementEntities;
using MyWebApi.Entities.DailyTaskEntities;
using MyWebApi.Entities.AdminEntities;
using System;
using MyWebApi.Entities.TestEntities;

namespace MyWebApi.Interfaces
{
    public interface IAdminRepository
    {
        Task<bool> CheckUserIsAdmin(long userId);
        Task<byte> SwitchAdminStatus(long userId);
        Task<bool?> GetAdminStatus(long userId);
        Task<long> UploadCountries(List<Country> countries);
        Task<long> UploadCities(List<City> cities);
        Task<long> UploadLanguages(List<Language> langs);
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
        Task<string> GetUserPhotoAsync(long userId);
        Task<bool> ResolveTickRequestAsync(Guid requestId, long adminId, bool isAccepted);
        Task<bool> AbortTickRequestAsync(Guid requestId);
        Task<bool> NotifyFailierTickRequestAsync(Guid requestId, long adminId);
    }
}
