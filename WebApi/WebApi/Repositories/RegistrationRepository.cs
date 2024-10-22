﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Interfaces;

namespace WebApi.Repositories
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private UserContext _contx;

        public RegistrationRepository(UserContext contx)
        {
            _contx = contx;
        }

        public async Task<List<string>> SuggestLanguagesAsync(string incorrectLanguage)
        {
            return await _contx.Languages
                .Select(l => new
                {
                    Distance = EF.Functions.FuzzyStringMatchLevenshteinLessEqual(l.LanguageName, incorrectLanguage, 2),
                    LanguageName = l.LanguageName
                })
                .Where(t => t.Distance <= 2)
                .OrderBy(t => t.Distance)
                .Select(t => t.LanguageName)
                .Take(3)
                .ToListAsync();
        }

        public async Task<List<string>> SuggestCountriesAsync(string incorrectCountry)
        {
            return await _contx.Countries
                .Where(t => EF.Functions.FuzzyStringMatchLevenshteinLessEqual(t.CountryName, incorrectCountry, 2) <= 2)
                .OrderBy(t => EF.Functions.FuzzyStringMatchLevenshteinLessEqual(t.CountryName, incorrectCountry, 2))
                .Select(t => t.CountryName)
                .Take(3)
                .ToListAsync();
        }

        public async Task<List<string>> SuggestCitiesAsync(string incorrectCity)
        {
            return await _contx.Cities
                .Where(t => EF.Functions.FuzzyStringMatchLevenshteinLessEqual(t.CityName, incorrectCity, 2) <= 2)
                .OrderBy(t => EF.Functions.FuzzyStringMatchLevenshteinLessEqual(t.CityName, incorrectCity, 2))
                .Select(t => t.CityName)
                .Take(3)
                .ToListAsync();
        }
    }
}
