using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Interfaces;
using Microsoft.Extensions.Logging;
using WebApi.Entities.TestEntities;
using WebApi.Entities.SecondaryEntities;
using WebApi.Entities.LocationEntities;
using WebApi.Entities.UserActionEntities;
using WebApi.Enums;

namespace WebApi.Controllers
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

        [HttpGet("/app-languages")]
        public List<GetLocalizedEnum> GetAppLanguages()
        {
            return _repository.GetAppLanguages();
        }

        [HttpGet("/usage-reasons")]
        public List<GetLocalizedEnum> GetReasons()
        {
            return _repository.GetReasons();
        }

        [HttpGet("/communication-preferences")]
        public List<GetLocalizedEnum> GetCommunicationPreferences()
        {
            return _repository.GetCommunicationPreferences();
        }

        [HttpGet("/GetCountries/{localisationId}")]
        public async Task<List<Country>> GetCountries(AppLanguage localisationId)
        {
            return await _repository.GetCountries(localisationId);
        }

        [HttpGet("/GetCities/{countryId}/{localisationId}")]
        public async Task<List<City>> GetCities(int countryId, AppLanguage localisationId)
        {
            return await _repository.GetCities(countryId, localisationId);
        }

        [HttpGet("/GetLanguages/{lang}")]
        public async Task<List<Language>> GetLanguages(AppLanguage lang)
        {
            return await _repository.GetLanguagesAsync(lang);
        }

        [HttpGet("/GetSingleTest/{id}/{locId}")]
        public async Task<Test> GetSingleTest(long id, AppLanguage locId)
        {
            var tests = await _repository.GetSingleTestAsync(id, locId);
            return tests;
        }

        //[HttpGet("/GetIntellectualTests")]
        //public async Task<List<IntellectualTest>> GetIntellectualTests()
        //{
        //    var tests = await _repository.GetIntellectualTestsAsync();
        //    return tests;
        //}

        [HttpGet("/similar-tags")]
        public async Task<List<string>> GetSimmilarTagsAsync([FromQuery] string tag)
        {
            return await _repository.GetSimmilarTagsAsync(tag);
        }

    }
}
