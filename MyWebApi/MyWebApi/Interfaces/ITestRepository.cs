using MyWebApi.Entities.LocalisationEntities;
using MyWebApi.Entities.LocationEntities;
using MyWebApi.Entities.SecondaryEntities;
using MyWebApi.Entities.TestEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyWebApi.Interfaces
{
    public interface ITestRepository
    {
        Task<List<Localisation>> GetLocalisationAsync(int localisationId);
        Task<List<ClassLocalisation>> GetClassLocalisationAsync(int localisationId);
        Task<List<SecondaryLocalisationModel>> GetSecondaryLocalisationAsync();
        Task<List<AppLanguage>> GetAppLanguages();
        Task<List<Gender>> GetGenders(int localisationId);
        Task<List<UserReason>> GetReasons(int localisationId);
        Task<List<AgePreference>> GetAgePreferences(int localisationId);
        Task<List<CommunicationPreference>> GetCommunicationPreferences(int localisationId);
        Task<List<City>> GetCities(int countryId, int localisationId);
        Task<List<Country>> GetCountries(int localisationId);
        Task<List<Language>> GetLanguagesAsync(int localisationId);
        Task<List<PsychologicalTest>> GetPsychologicalTestsAsync();
        Task<PsychologicalTest> GetSinglePsychologicalTestAsync(long testId, int localisationId);
        //Task<List<IntellectualTest>> GetIntellectualTestsAsync();
    }
}
