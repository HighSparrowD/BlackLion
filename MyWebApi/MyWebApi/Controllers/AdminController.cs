using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Entities.AchievementEntities;
using WebApi.Entities.AdminEntities;
using WebApi.Entities.ReportEntities;
using WebApi.Entities.TestEntities;
using WebApi.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Entities;
using WebApi.Entities.LocationEntities;

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
        public async Task<long> UpdateCountries(List<UpdateCountry> countries)
        {
            return await _repository.UploadCountries(countries);
        }

        [HttpPost("/UpdateCities")]
        public async Task<long> UpdateCities(List<UpdateCity> cities)
        {
            return await _repository.UploadCities(cities);
        }

        [HttpPost("/UpdateLanguages")]
        public async Task<long> UpdateLanguages(List<UpdateLanguage> langs)
        {
            return await _repository.UploadLanguages(langs);
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

        [HttpGet("/DeleteAllUsersForever")]
        public async Task<int> DeleteAllUsers()
        {
            return await _repository.DeleteAllUsers();
        }

        [HttpPost("/upload-achievements")]
        public async Task AddNewAchievements([FromBody] List<UploadAchievement> achievements)
        {
            await _repository.AddAchievementsAsync(achievements);
        }

        [HttpGet("/GetTickRequests")]
        public async Task<List<TickRequest>> GetTickRequests()
        {
            return await _repository.GetTickRequestsAsync();
        }

        [HttpGet("/GetTickRequest/{id?}")]
        public async Task<TickRequest> GetTickRequest(long? id = null)
        {
            return await _repository.GetTickRequestAsync(id);
        }

        [HttpPost("/ResolveTickRequest")]
        public async Task<bool> ResolveTickRequest([FromBody] ResolveTickRequest request)
        {
            return await _repository.ResolveTickRequestAsync(request);
        }

        [HttpGet("/AbortTickRequest/{id}")]
        public async Task<bool> AbortTickRequest(long id)
        {
            return await _repository.AbortTickRequestAsync(id);
        }

        [HttpGet("/NotifyFailierTickRequest/{id}/{adminId}")]
        public async Task<bool> NotifyFailierTickRequest(long id, long adminId)
        {
            return await _repository.NotifyFailierTickRequestAsync(id, adminId);
        }

        [HttpPost("/UploadTests")]
        public async Task<byte> UploadPsTests([FromBody] List<UploadTest> model, [FromServices] IAdminRepository adminRepo)
        {
            return await adminRepo.UploadTestsAsync(model);
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

        [HttpGet("/add-decoys")]
        public async Task AddDecoys([FromQuery] long userId, [FromServices] IUserRepository userRepo)
        {
            for (int i = 1; i < 1_000_000; i++)
            {
                try
                {
                    var initialUser = await userRepo.GetUserInfoAsync(userId);
                    await userRepo.RegisterUserAsync(new Entities.UserActionEntities.UserRegistrationModel
                    {
                        UserName = initialUser.UserName,
                        Age = initialUser.UserAge,
                        AgePrefs = initialUser.AgePrefs,
                        AppLanguage = initialUser.Language,
                        UsesOcean = false,
                        CityCode = initialUser.CityId,
                        CountryCode = initialUser.CountryId,
                        CommunicationPrefs = initialUser.CommunicationPrefs,
                        Description = initialUser.UserDescription,
                        Gender = initialUser.UserGender,
                        GenderPrefs = initialUser.UserGenderPrefs,
                        MediaType = initialUser.MediaType,
                        Id = initialUser.Id + i,
                        LanguagePreferences = initialUser.LanguagePreferences,
                        Languages = initialUser.UserLanguages,
                        Media = initialUser.UserMedia,
                        RealName = initialUser.UserRealName,
                        Reason = initialUser.Reason,
                        UserLocationPreferences = initialUser.LocationPreferences
                    });
                    System.Console.WriteLine($"Registered users count: {i}");
                }
                catch { }
            }
        }
    }
}
