using System.Threading.Tasks;
using System.Collections.Generic;
using MyWebApi.Entities.SecondaryEntities;
using MyWebApi.Entities.ReportEntities;
using MyWebApi.Entities.ReasonEntities;
using MyWebApi.Entities.LocationEntities;

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
        Task<long> UploadFeedbackReasons(List<FeedbackReason> reasons);
        Task<List<Feedback>> GetFeedbacks ();
        Task<long> DeleteUser (long userId);
        Task<int> DeleteAllUsers ();
    }
}
