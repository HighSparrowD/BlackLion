using System.Threading.Tasks;
using System.Collections.Generic;
using WebApi.Entities.TestEntities;
using WebApi.Main.Entities.Admin;
using WebApi.Main.Entities.Report;
using WebApi.Models.Models.Admin;
using entities = WebApi.Main.Entities;
using WebApi.Enums.Enums.User;

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
        Task<List<entities.Admin.VerificationRequest>> GetVerificationRequestAsync();
        Task<entities.Admin.VerificationRequest> GetVerificationRequestByIdAsync(long requestId, VerificationRequestStatus status = VerificationRequestStatus.ToView);
        Task<entities.Admin.VerificationRequest> ResolveVerificationRequest(ResolveVerificationRequest model);

		Task<bool> AbortTickRequestAsync(long requestId);
        Task<bool> NotifyFailierTickRequestAsync(long requestId, long adminId);
        Task<List<long>> GetRecentlyBannedUsersAsync();
    }
}
