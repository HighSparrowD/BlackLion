using WebApi.Entities.TestEntities;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Entities.UserActionEntities;
using WebApi.Main.Models.Location;
using WebApi.Main.Enums.General;
using WebApi.Main.Models.Language;
using WebApi.Main.Models.Test;

namespace WebApi.Interfaces
{
    public interface ITestRepository
    {
        List<GetLocalizedEnum> GetAppLanguages();
        List<GetLocalizedEnum> GetReasons();
        List<GetLocalizedEnum> GetFeedbackReasons();
        List<GetLocalizedEnum> GetCommunicationPreferences();
        Task<List<City>> GetCities(int countryId, AppLanguage localisationId);
        Task<List<Country>> GetCountries(AppLanguage localisationId);
        Task<List<Language>> GetLanguagesAsync(AppLanguage localisationId);
        Task<Test> GetSingleTestAsync(long testId, AppLanguage localisationId);
        //Task<List<IntellectualTest>> GetIntellectualTestsAsync();
        //Task<List<string>> GetSimmilarTagsAsync(string tag);
    }
}
