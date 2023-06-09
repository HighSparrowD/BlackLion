using WebApi.Entities.LocalisationEntities;
using WebApi.Entities.LocationEntities;
using WebApi.Entities.SecondaryEntities;
using WebApi.Entities.TestEntities;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Entities.UserActionEntities;
using WebApi.Enums;

namespace WebApi.Interfaces
{
    public interface ITestRepository
    {
        Task<List<Localization>> GetLocalisationAsync(int localisationId);
        Task<List<ClassLocalization>> GetClassLocalisationAsync(int localisationId);
        Task<List<SecondaryLocalizationModel>> GetSecondaryLocalisationAsync();
        List<GetLocalizedEnum> GetAppLanguages();
        List<GetLocalizedEnum> GetReasons();
        List<GetLocalizedEnum> GetCommunicationPreferences();
        Task<List<City>> GetCities(int countryId, AppLanguage localisationId);
        Task<List<Country>> GetCountries(AppLanguage localisationId);
        Task<List<Language>> GetLanguagesAsync(AppLanguage localisationId);
        Task<Test> GetSingleTestAsync(long testId, AppLanguage localisationId);
        //Task<List<IntellectualTest>> GetIntellectualTestsAsync();
    }
}
