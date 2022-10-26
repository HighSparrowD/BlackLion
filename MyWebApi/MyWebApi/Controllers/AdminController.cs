using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MyWebApi.Entities.AchievementEntities;
using MyWebApi.Entities.LocationEntities;
using MyWebApi.Entities.ReasonEntities;
using MyWebApi.Entities.ReportEntities;
using MyWebApi.Entities.SecondaryEntities;
using MyWebApi.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private IAdminRepository _repository;
        private readonly ILogger<UserActionController> _logger;
        private IStringLocalizer<AdminController> _localizer;

        public AdminController(ILogger<UserActionController> logger, IAdminRepository repos, IStringLocalizer<AdminController> localizer)
        {
            _repository = repos;
            _logger = logger;
            _localizer = localizer;
        }

        [HttpGet("/get-admin-localisation")]
        public async Task<Dictionary<string, string>> GetAdminLocalisation()
        {
            var r = _localizer["YButton"];
            var d = Request.Headers;
            var rawLocalisation = _localizer.GetAllStrings()
                .Select(w => new {w.Name, w.Value})
                .ToList();

            var localisationDict = new Dictionary<string, string>();

            foreach (var item in rawLocalisation)
            {
                localisationDict.Add(item.Name, item.Value);
            }

            return localisationDict;
        }

        [HttpPost("/UpdateCountries")]
        public async Task<long> UpdateCountries(List<Country> countries)
        {
            return await _repository.UploadCountries(countries);
        }

        [HttpPost("/UpdateCities")]
        public async Task<long> UpdateCities(List<City> cities)
        {
            return await _repository.UploadCities(cities);
        }

        [HttpPost("/UpdateLanguages")]
        public async Task<long> UpdateLanguages(List<Language> langs)
        {
            return await _repository.UploadLanguages(langs);
        }

        [HttpPost("/UploadFeedbackReasons")]
        public async Task<long> UploadFeedbackReasons(List<FeedbackReason> reasons)
        {
            return await _repository.UploadFeedbackReasons(reasons);
        }

        [HttpGet("/GetFeedbacks")]
        public async Task<List<Feedback>> GetFeedbacks()
        {
            return await _repository.GetFeedbacks();
        }

        [HttpGet("/CheckUserIsAdmin/{userId}")]
        public async Task<bool> CheckUserIsAdmin(long userId)
        {
            return await _repository.CheckUserIsAdmin(userId);
        }

        [HttpGet("/SwitchAdminStatus/{userId}")]
        public async Task<byte> SwitchAdminStatus(long userId)
        {
            return await _repository.SwitchAdminStatus(userId);
        }

        [HttpGet("/GethAdminStatus/{userId}")]
        public async Task<bool?> GetAdminStatus(long userId)
        {
            return await _repository.GetAdminStatus(userId);
        }

        [HttpGet("/DeleteUserForever/{userId}")]
        public async Task<long> DeleteUser(long userId)
        {
            return await _repository.DeleteUser(userId);
        }

        [HttpGet("/DeleteAllUsersForever")]
        public async Task<int> DeleteAllUsers()
        {
            return await _repository.DeleteAllUsers();
        }

        [HttpPost("/UploadAchievements")]
        public async Task<int> UploadAchievements(List<Achievement> achievements)
        {
            return await _repository.UploadAchievements(achievements);
        }

        [HttpPost("/AddNewAchievements")]
        public async Task<int> AddNewAchievements(List<Achievement> achievements)
        {
            return await _repository.AddNewAchievements(achievements);
        }
    }
}
