using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : Controller
    {
        public IRegistrationRepository _repository { get; set; }

        public RegistrationController(IRegistrationRepository registrationRepo)
        {
            _repository = registrationRepo;
        }

        //[HttpGet("/get-registration-localization")]
        //public async Task<Dictionary<string, string>> GetRegistrationLocalization()
        //{
        //    var s = Request.Headers;

        //    var localizationDict = new Dictionary<string, string>();
        //    await Task.Run(() =>
        //    {
        //        var rawLocalization = _localizer.GetAllStrings();

        //        foreach (var item in rawLocalization)
        //        {
        //            localizationDict.Add(item.Name, item.Value);
        //        }
        //    });

        //    return localizationDict;
        //}

        [HttpGet("/suggest-languages")]
        public async Task<List<string>> SuggestLanguages([FromQuery] string language)
        {
            return await _repository.SuggestLanguagesAsync(language);
        }

        [HttpGet("/suggest-countries")]
        public async Task<List<string>> SuggestCountries([FromQuery] string country)
        {
            return await _repository.SuggestCountriesAsync(country);
        }

        [HttpGet("/suggest-cities")]
        public async Task<List<string>> SuggestCities([FromQuery] string city)
        {
            return await _repository.SuggestCitiesAsync(city);
        }
    }
}
