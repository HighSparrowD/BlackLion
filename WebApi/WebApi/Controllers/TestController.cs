using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Interfaces;
using Microsoft.Extensions.Logging;
using WebApi.Main.Models.Location;
using WebApi.Main.Models.Language;
using WebApi.Main.Models.Test;
using WebApi.Models.Models.User;
using WebApi.Enums.Enums.General;
using models = WebApi.Models.Models.Test;

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

        [HttpGet("/feedback-reasons")]
        public List<GetLocalizedEnum> FeedbackReasons()
        {
            return _repository.GetFeedbackReasons();
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

        [HttpGet("/single-test/{id}/{language}")]
        public async Task<models.Test> GetSingleTest(long id, AppLanguage language)
        {
             var tests = await _repository.GetSingleTestAsync(id, language);
            return tests;
        }

        [HttpGet("/similar-tags")]
        public IActionResult GetSimmilarTagsAsync()
        {
            //return await _repository.GetSimmilarTagsAsync(tag);
            return NoContent();
        }

    }
}
