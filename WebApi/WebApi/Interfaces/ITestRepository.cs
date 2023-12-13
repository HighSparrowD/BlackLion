using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Main.Models.Location;
using WebApi.Main.Models.Language;
using WebApi.Models.Models.User;
using WebApi.Enums.Enums.General;

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
        Task<Models.Models.Test.Test> GetSingleTestAsync(long testId, AppLanguage localisationId);
        //Task<List<IntellectualTest>> GetIntellectualTestsAsync();
        //Task<List<string>> GetSimmilarTagsAsync(string tag);
    }
}
