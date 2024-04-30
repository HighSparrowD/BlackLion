using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Entities.TestEntities;
using WebApi.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Main.Entities.Admin;
using WebApi.Models.Models.Admin;
using WebApi.Models.Models.User;
using System;
using Microsoft.AspNetCore.Authorization;
using WebApi.Interfaces.Services;
using WebApi.Models.Models.Identity.Attributes.Machine;
using WebApi.Models.Models.Identity.Attributes.Admin;
using models = WebApi.Models.Models;
using WebApi.Models.Models.Report;
using WebApi.Services.Admin;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private IAdminRepository _repository;
        private IAdminService _adminService;
        private readonly ILogger<UserController> _logger;

        public AdminController(ILogger<UserController> logger, IAdminRepository repos, 
            IAdminService adminService)
        {
            _repository = repos;
            _adminService = adminService;
            _logger = logger;
        }

        [HttpPost("/debug")]
        [Authorize]
        [RequiresMachine]
        public async Task<ActionResult> SetDebugProperties([FromBody] List<long> userIds) // TODO: remove in production
        {
            await _adminService.StartInDebug(userIds);
            return NoContent();
        }

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
                    await userRepo.RegisterUserAsync(new UserRegistrationModel
                    {
                        UserName = initialUser.Username,
                        Age = initialUser.Age,
                        AgePrefs = initialUser.AgePrefs,
                        AppLanguage = initialUser.Language,
                        UsesOcean = true,
                        CityCode = initialUser.City,
                        CountryCode = initialUser.Country,
                        CommunicationPrefs = initialUser.CommunicationPrefs,
                        Description = initialUser.Description,
                        Gender = initialUser.Gender,
                        GenderPrefs = initialUser.GenderPrefs,
                        MediaType = initialUser.MediaType,
                        Tags = initialUser.Tags,
                        Id = initialUser.Id + i + new Random().Next(1000),
                        LanguagePreferences = initialUser.LanguagePreferences,
                        Languages = initialUser.Languages,
                        Media = initialUser.Media,
                        RealName = initialUser.RealName,
                        Reason = initialUser.Reason,
                        UserLocationPreferences = initialUser.LocationPreferences,
                    });
                    System.Console.WriteLine($"Registered users count: {i}");
                }
                catch { }
            }
        }

        [HttpGet("feedbacks/all")]
        //[RequiresAdminOrCreator]
        public async Task<ActionResult<GroupedFeedback>> GetGroupedFeedbacks([FromServices] IAdminService adminService)
        {
            var feedbacks = await adminService.GetGroupedFeedbackAsync();

            return Ok(feedbacks);
        }

        [HttpGet("feedbacks/recent")]
        //[RequiresAdminOrCreator]
        public async Task<ActionResult<Feedback>> GetRecentFeedbacks([FromServices] IAdminService adminService)
        {
            var feedbacks = await adminService.GetRecentFeedbackAsync();

            return Ok(feedbacks);
        }

        [HttpGet("reports/recent")]
        //[RequiresAdminOrCreator]
        public async Task<ActionResult<Report>> GetRecentReports([FromServices] IAdminService adminService)
        {
            var reports = await adminService.GetRecentReportsAsync();

            return Ok(reports);
        }

        [HttpGet("updates")]
        //[RequiresAdminOrCreator]
        public async Task<ActionResult<RecentUpdates>> GetUpdates([FromServices] IAdminService adminService)
        {
            var updates = await adminService.GetRecentUpdates();

            return Ok(updates);
        }

        [HttpGet("verification-request")]
        //[RequiresAdminOrCreator]
        public async Task<ActionResult<List<models.Admin.VerificationRequest>>> GetVerificationRequest(IAdminService adminService)
        {
            var requests = await adminService.GetVerificationRequestsAsync();

            return Ok(requests);
        }

        [HttpPost("verification-request")]
        //[RequiresAdminOrCreator]
        public async Task<ActionResult> ResolveVerificationRequest([FromBody] ResolveVerificationRequest request)
        {
            await _repository.ResolveVerificationRequestAsync(request);

            return NoContent();
        }
    }
}
