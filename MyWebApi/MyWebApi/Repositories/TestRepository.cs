using MyWebApi.Data;
using MyWebApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWebApi.Entities.LocalisationEntities;
using MyWebApi.Entities.TestEntities;
using MyWebApi.Entities.SecondaryEntities;
using System.Linq;
using MyWebApi.Entities.LocationEntities;

namespace MyWebApi.Repositories
{
    public class TestRepository : ITestRepository
    {
        public UserContext _contx { get; set; }

        public TestRepository(UserContext contx)
        {
            _contx = contx;
        }

        public async Task<List<SecondaryLocalisationModel>> GetSecondaryLocalisationAsync()
        {
            var localisations = await _contx.SECONDARY_LOCALISATIONS.ToListAsync();
            return localisations;
        }

        public async Task<List<Localisation>> GetLocalisationAsync(int localisationId)
        {
            var localisations = await _contx.LOCALISATIONS.Where(l => l.Id == localisationId)
                .Include(l => l.Loc).ToListAsync();
            return localisations;
        }

        public async Task<List<ClassLocalisation>> GetClassLocalisationAsync(int localisationId)
        {
            var localisations = await _contx.CLASS_LOCALISATIONS
                .Where(l => l.Id == localisationId)
                .Include(l => l.Languages)
                //.Include(l => l.Cities) // Retrieve using GetCities after choosing a country!
                .Include(l => l.Countries)
                .Include(l => l.Genders)
                .ToListAsync();
            return localisations;
        }

        public async Task<List<AppLanguage>> GetAppLanguages()
        {
            return await _contx.APP_LANGUAGES.ToListAsync();
        }

        public async Task<List<PsychologicalTest>> GetPsychologicalTestsAsync()
        {
            return await _contx.PSYCHOLOGICAL_TESTS.Include(p => p.Questions).ThenInclude(q => q.Answers).ToListAsync();
        }

        public async Task<List<IntellectualTest>> GetIntellectualTestsAsync()
        {
            return await _contx.INTELLECTUAL_TESTS.Include(i => i.Questions).ThenInclude(q => q.Answers).ToListAsync();
        }

        public async Task<List<Gender>> GetGenders(int localisationId)
        {
            return await _contx.SYSTEM_GENDERS.Where(g => g.ClassLocalisationId == localisationId).ToListAsync();
        }

        public async Task<List<City>> GetCities(int countryId, int localisationId)
        {
            var cities = await _contx.CITIES.Where(c => c.CountryId == countryId && c.CountryClassLocalisationId == localisationId).ToListAsync();
            return cities;
        }

        public async Task<List<Country>> GetCountries(int localisationId)
        {
            return await _contx.COUNTRIES.Where(c => c.ClassLocalisationId == localisationId).ToListAsync();
        }

        public async Task<List<UserReason>> GetReasons(int localisationId)
        {
            return await _contx.USER_REASONS.Where(r => r.ClassLocalisationId == localisationId).ToListAsync();
        }

        public async Task<List<AgePreference>> GetAgePreferences(int localisationId)
        {
            return await _contx.AGE_PREFERENCES.Where(p => p.ClassLocalisationId == localisationId).ToListAsync();
        }

        public async Task<List<CommunicationPreference>> GetCommunicationPreferences(int localisationId)
        {
            return await _contx.COMMUNICATION_PREFERENCES.Where(p => p.ClassLocalisationId == localisationId).ToListAsync();
        }

        public async Task<List<Language>> GetLanguagesAsync(int localisationId)
        {
            return await _contx.LANGUAGES
                .Where(l => l.ClassLocalisationId == localisationId)
                .OrderByDescending(l => l.Priority)
                .ToListAsync();
        }
    }
}
