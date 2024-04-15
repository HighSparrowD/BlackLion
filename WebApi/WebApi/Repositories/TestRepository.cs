using WebApi.Data;
using WebApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using WebApi.Main.Entities.Language;
using WebApi.Main.Entities.Location;
using WebApi.Models.Models.User;
using WebApi.Enums.Enums.General;
using WebApi.Models.Utilities;
using WebApi.Enums.Enums.Report;
using WebApi.Enums.Enums.User;

namespace WebApi.Repositories
{
    public class TestRepository : ITestRepository
    {
        public UserContext _contx { get; set; }

        public TestRepository(UserContext contx)
        {
            _contx = contx;
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
            var cities = await _contx.Cities.Where(c => c.CountryId == countryId && c.CountryLang == localisationId).ToListAsync();
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

        public List<GetLocalizedEnum> GetFeedbackReasons()
        {
            var reasons = new List<GetLocalizedEnum>();

            foreach (var reason in Enum.GetValues(typeof(FeedbackReason)))
            {
                reasons.Add(new GetLocalizedEnum
                {
                    Id = (byte)reason,
                    Name = EnumLocalizer.GetLocalizedValue((FeedbackReason)reason)
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

        public async Task<List<Language>> GetLanguagesAsync(AppLanguage lang)
        {
            return await _contx.Languages
                .Where(l => l.Lang == lang)
                .OrderBy(l => l.Priority)
                .ToListAsync();
        }

        public async Task<Models.Models.Test.Test> GetSingleTestAsync(long testId, AppLanguage localisationId)
        {
            // TODO: Apply commented code when test are localized
            return await _contx.Tests
                .Where(t => t.Id == testId) // && t.Language == localisationId
                .Include(t => t.Questions)
                .ThenInclude(q => q.Answers)
                .Include(q => q.Scales)
                .Include(t => t.Results.OrderBy(r => r.Score))
                .FirstOrDefaultAsync();
        }

        //public async Task<List<string>> GetSimmilarTagsAsync(string tag)
        //{
        //    return await _contx.UserTags
        //        .Where(t => EF.Functions.FuzzyStringMatchDifference(EF.Functions.FuzzyStringMatchSoundex(t.Tag), tag) >= 1)
        //        .OrderByDescending(t => EF.Functions.FuzzyStringMatchDifference(EF.Functions.FuzzyStringMatchSoundex(t.Tag), tag))
        //        .Select(t => t.Tag)
        //        .Take(3)
        //        .ToListAsync();
        //}
    }
}
