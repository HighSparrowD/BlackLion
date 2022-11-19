using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWebApi.Interfaces;
using Microsoft.Extensions.Logging;
using MyWebApi.Entities.LocalisationEntities;
using MyWebApi.Entities.TestEntities;
using MyWebApi.Entities.SecondaryEntities;
using MyWebApi.Entities.LocationEntities;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private ITestRepository _repository;
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger, ITestRepository repos)
        {
            _repository = repos;
            _logger = logger;
        }


        [HttpGet("/GetLocalisation/{localisationId}")]
        public async Task<List<Localisation>> GetLocalisations(int localisationId)
        {
            return await _repository.GetLocalisationAsync(localisationId);
        }

        [HttpGet("/GetSecondaryLocalisations")]
        public async Task<List<SecondaryLocalisationModel>> GetSecondaryLocalisations()
        {
            return await _repository.GetSecondaryLocalisationAsync();
        }

        [HttpGet("/GetClassLocalisations/{localisationId}")]
        public async Task<List<ClassLocalisation>> GetClassLocalisations(int localisationId)
        {
            return await _repository.GetClassLocalisationAsync(localisationId);
        }

        [HttpGet("/GetAppLanguages")]
        public async Task<List<AppLanguage>> GetAppLanguages()
        {
            return await _repository.GetAppLanguages();
        }

        [HttpGet("/GetGenders/{localisationId}")]
        public async Task<List<Gender>> GetGenders(int localisationId)
        {
            return await _repository.GetGenders(localisationId);
        }

        [HttpGet("/GetReasons/{localisationId}")]
        public async Task<List<UserReason>> GetReasons(int localisationId)
        {
            return await _repository.GetReasons(localisationId);
        }

        [HttpGet("/GetAgePreferences/{localisationId}")]
        public async Task<List<AgePreference>> GetAgePreferences(int localisationId)
        {
            return await _repository.GetAgePreferences(localisationId);
        }

        [HttpGet("/GetCommunicationPreferences/{localisationId}")]
        public async Task<List<CommunicationPreference>> GetCommunicationPreferences(int localisationId)
        {
            return await _repository.GetCommunicationPreferences(localisationId);
        }

        [HttpGet("/GetCountries/{localisationId}")]
        public async Task<List<Country>> GetCountries(int localisationId)
        {
            return await _repository.GetCountries(localisationId);
        }

        [HttpGet("/GetCities/{countryId}/{localisationId}")]
        public async Task<List<City>> GetCities(int countryId, int localisationId)
        {
            return await _repository.GetCities(countryId, localisationId);
        }

        [HttpGet("/GetLanguages/{localisationId}")]
        public async Task<List<Language>> GetLanguages(int localisationId)
        {
            return await _repository.GetLanguagesAsync(localisationId);
        }

        [HttpGet("/GetPsychologicalTests")]
        public async Task<List<Test>> GetPsychologicalTests()
        {
            var tests = await _repository.GetPsychologicalTestsAsync();
            return tests;
        }

        [HttpGet("/GetSinglePsychologicalTest/{id}/{locId}")]
        public async Task<Test> GetSinglePsychologicalTest(long id, int locId)
        {
            var tests = await _repository.GetSinglePsychologicalTestAsync(id, locId);
            return tests;
        }

        //[HttpGet("/GetIntellectualTests")]
        //public async Task<List<IntellectualTest>> GetIntellectualTests()
        //{
        //    var tests = await _repository.GetIntellectualTestsAsync();
        //    return tests;
        //}

    }
}
