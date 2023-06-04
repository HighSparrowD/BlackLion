using WebApi.Data;
using WebApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Entities.LocalisationEntities;
using WebApi.Entities.TestEntities;
using WebApi.Entities.SecondaryEntities;
using System.Linq;
using WebApi.Entities.LocationEntities;
using System;
using WebApi.Entities.UserActionEntities;
using WebApi.Enums;
using WebApi.Utilities;

namespace WebApi.Repositories
{
    public class TestRepository : ITestRepository
    {
        public UserContext _contx { get; set; }

        public TestRepository(UserContext contx)
        {
            _contx = contx;
        }

        public async Task<List<SecondaryLocalizationModel>> GetSecondaryLocalisationAsync()
        {
            var localisations = await _contx.SecondaryLocalizations.ToListAsync();
            return localisations;
        }

        public async Task<List<Localization>> GetLocalisationAsync(int localisationId)
        {
            var localisations = await _contx.Localizations.Where(l => l.Id == localisationId)
                .Include(l => l.Loc).ToListAsync();
            return localisations;
        }

        public async Task<List<ClassLocalization>> GetClassLocalisationAsync(int localisationId)
        {
            var localisations = await _contx.ClassLocalizations
                .Where(l => l.Id == localisationId)
                .Include(l => l.Languages)
                //.Include(l => l.Cities) // Retrieve using GetCities after choosing a country!
                //.Include(l => l.Countries)
                .ToListAsync();
            return localisations;
        }

        public List<GetLocalizedEnum> GetAppLanguages()
        {
            var languages = new List<GetLocalizedEnum>();

            foreach (var language in Enum.GetValues(typeof(AppLanguage)))
            {
                languages.Add(new GetLocalizedEnum
                {
                    Id = (byte)language,
                    Name = EnumLocalizer.GetLocalizedValue((AppLanguage)language)
                });
            }

            return languages;
        }

        //public async Task<List<IntellectualTest>> GetIntellectualTestsAsync()
        //{
        //    return await _contx.INTELLECTUAL_TESTS.Include(i => i.Questions).ThenInclude(q => q.Answers).ToListAsync();
        //}

        public async Task<List<City>> GetCities(int countryId, AppLanguage localisationId)
        {
            var cities = await _contx.Cities.Where(c => c.CountryId == countryId && c.Lang == localisationId).ToListAsync();
            return cities;
        }

        public async Task<List<Country>> GetCountries(AppLanguage localisationId)
        {
            return await _contx.Countries.Where(c => c.Lang == localisationId)
                .OrderBy(c => c.Priority)
                .ToListAsync();
        }

        public List<GetLocalizedEnum> GetReasons()
        {
            var reasons = new List<GetLocalizedEnum>();

            foreach (var reason in Enum.GetValues(typeof(UsageReason)))
            {
                reasons.Add(new GetLocalizedEnum
                {
                    Id = (byte)reason,
                    Name = EnumLocalizer.GetLocalizedValue((UsageReason)reason)
                });
            }

            return reasons;
        }

        public List<GetLocalizedEnum> GetCommunicationPreferences()
        {
            var preferences = new List<GetLocalizedEnum>();

            foreach (var preference in Enum.GetValues(typeof(CommunicationPreference)))
            {
                preferences.Add(new GetLocalizedEnum
                {
                    Id = (byte)preference,
                    Name = EnumLocalizer.GetLocalizedValue((CommunicationPreference)preference)
                });
            }

            return preferences;
        }

        public async Task<List<Language>> GetLanguagesAsync(int localisationId)
        {
            return await _contx.Languages
                .Where(l => l.ClassLocalisationId == localisationId)
                .OrderByDescending(l => l.Priority)
                .ToListAsync();
        }

        public async Task<Test> GetSingleTestAsync(long testId, AppLanguage localisationId)
        {
            return await _contx.Tests
                .Where(t => t.Id == testId && t.Language == localisationId)
                .Include(t => t.Questions)
                .ThenInclude(q => q.Answers)
                .Include(t => t.Results.OrderBy(r => r.Score))
                .SingleOrDefaultAsync();
        }
    }
}
