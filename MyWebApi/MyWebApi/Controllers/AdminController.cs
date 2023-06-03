using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using WebApi.Entities.AchievementEntities;
using WebApi.Entities.AdminEntities;
using WebApi.Entities.LocationEntities;
using WebApi.Entities.ReasonEntities;
using WebApi.Entities.ReportEntities;
using WebApi.Entities.SecondaryEntities;
using WebApi.Entities.TestEntities;
using WebApi.Entities.UserActionEntities;
using WebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private IAdminRepository _repository;
        private readonly ILogger<UserActionController> _logger;

        public AdminController(ILogger<UserActionController> logger, IAdminRepository repos)
        {
            _repository = repos;
            _logger = logger;
        }

        //[HttpGet("/get-admin-localisation")]
        //public async Task<Dictionary<string, string>> GetAdminLocalisation()
        //{
        //    var localisationDict = new Dictionary<string, string>();

        //    await Task.Run(() => {
        //        var rawLocalization = _localizer.GetAllStrings()
        //            .Select(w => new {w.Name, w.Value})
        //            .ToList();


        //        foreach (var item in rawLocalization)
        //        {
        //            localisationDict.Add(item.Name, item.Value);
        //        }
        //    });

        //    return localisationDict;
        //}

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

        [HttpGet("/GetTickRequests")]
        public async Task<List<TickRequest>> GetTickRequests()
        {
            return await _repository.GetTickRequestsAsync();
        }

        [HttpGet("/GetTickRequest/{id?}")]
        public async Task<TickRequest> GetTickRequest(Guid? id = null)
        {
            return await _repository.GetTickRequestAsync(id);
        }

        [HttpPost("/ResolveTickRequest")]
        public async Task<bool> ResolveTickRequest([FromBody] ResolveTickRequest request)
        {
            return await _repository.ResolveTickRequestAsync(request);
        }

        [HttpGet("/AbortTickRequest/{id}")]
        public async Task<bool> AbortTickRequest(Guid id)
        {
            return await _repository.AbortTickRequestAsync(id);
        }

        [HttpGet("/NotifyFailierTickRequest/{id}/{adminId}")]
        public async Task<bool> NotifyFailierTickRequest(Guid id, long adminId)
        {
            return await _repository.NotifyFailierTickRequestAsync(id, adminId);
        }

        [HttpPost("/UploadPsTests")]
        public async Task<byte> UploadPsTests([FromBody] List<UploadTest> model)
        {
            return await _repository.UploadPsTestsAsync(model);
        }

        [HttpGet("/GetNewNotificationsCount/{adminId}")]
        public async Task<string> GetNewNotificationsCount(long adminId)
        {
            return await _repository.GetNewNotificationsCountAsync(adminId);
        }

        [HttpGet("/banned-users")]
        public async Task<List<long>> GetBannedUsers()
        {
            return await _repository.GetRecentlyBannedUsersAsync();
        }
    }
}
