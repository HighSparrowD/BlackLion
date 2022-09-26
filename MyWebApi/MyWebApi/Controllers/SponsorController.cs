using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyWebApi.Entities.SponsorEntities;
using MyWebApi.Entities.UserInfoEntities;
using MyWebApi.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SponsorController : Controller
    {
        private ISponsorRepository _repository;
        private readonly ILogger<SponsorController> _logger;

        public SponsorController(ISponsorRepository repository, ILogger<SponsorController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("/CheckUserIsAwaiting/{userId}")]
        public async Task<bool> CheckUserIsAwaiting(long userId)
        {
            return await _repository.CheckUserIsAwaitingAsync(userId);
        }

        [HttpGet("/CheckUserIsAwaitingByUsername/{username}")]
        public async Task<bool> CheckUserIsAwaiting(string username)
        {
            return await _repository.CheckUserIsAwaitingAsync(username);
        }

        [HttpGet("/CheckUserIsSponsor/{userId}")]
        public async Task<bool> CheckUserIsSponsor(long userId)
        {
            return await _repository.CheckUserIsSponsorAsync(userId);
        }

        [HttpGet("/CheckSponsorIsMaxed/{userId}")]
        public async Task<bool> CheckSponsorIsMaxed(long userId)
        {
            return await _repository.CheckSponsorIsMaxedAsync(userId);
        }

        [HttpGet("/CheckSponsorHasViewsLeft/{userId}")]
        public async Task<bool> CheckSponsorHasViewsLeft(long userId)
        {
            return await _repository.CheckSponsorHasViewsLeftAsync(userId);
        }

        [HttpGet("/CheckSponsorIsPostponed/{userId}")]
        public async Task<bool> CheckSponsorIsPostponed(long userId)
        {
            return await _repository.CheckUserIsPostponed(userId);
        }

        [HttpGet("/CheckUserKeyWordIsCorrect/{userId}/{keyword}")]
        public async Task<bool> CheckUserKeyWordIsCorrect(long userId, string keyword)
        {
            return await _repository.CheckUserKeyWordIsCorrect(userId, keyword);
        }

        [HttpGet("/GetSponsorAds/{userId}")]
        public async Task<List<Ad>> GetSponsorAds(long userId)
        {
            return await _repository.GetSponsorAdsAsync(userId);
        }

        [HttpGet("/GetSponsors")]
        public async Task<List<Sponsor>> GetSponsors()
        {
            return await _repository.GetSponsorsAsync();
        }

        [HttpGet("/GetAwaitingUser/{username}")]
        public async Task<Sponsor> GetAwaitingUserAsync(string username)
        {
            return await _repository.GetAwaitingUserAsync(username);
        }

        [HttpGet("/GetSponsorAsync/{userId}")]
        public async Task<Sponsor> GetSponsorAsync(long userId)
        {
            return await _repository.GetSingleSponsorAsync(userId);
        }

        [HttpGet("/GetSponsorAd/{sponsorId}/{adId}")]
        public async Task<Ad> GetSponsorAd(long sponsorId, long adId)
        {
            return await _repository.GetSingleAdAsync(sponsorId, adId);
        }

        [HttpPost("/RegisterAwaitingUser")]
        public async Task<long> RegisterAwaitingUser(AwaitingUserRegistration user)
        {
            return await _repository.RegisterAwaitingUserAsync(user);
        }

        [HttpPost("/RegisterSponsor")]
        public async Task<long> RegisterSponsorAsync(RegisterSponsor model)
        {
            return await _repository.RegisterSponsorAsync(model);
        }

        [HttpPost("/AdAdd")]
        public async Task<long> AddAd(Ad ad)
        {
            return await _repository.AddAdAsync(ad);
        }

        [HttpPost("/SponsorUpdate")]
        public async Task<long> SponsorUpdate(Sponsor model)
        {
            return await _repository.UpdateSponsorAsync(model);
        }

        [HttpPost("/AdUpdate")]
        public async Task<long> AdUpdate(Ad ad)
        {
            return await _repository.UpdateAdAsync(ad);
        }

        [HttpDelete("/RemoveSponsor/{userId}")]
        public async Task<byte> RemoveSponsor(long userId)
        {
            return await _repository.RemoveSponsorAsync(userId);
        }

        [HttpDelete("/RemoveAd/{adId}/{sponsorId}")]
        public async Task<byte> RemoveAd(long adId, long sponsorId)
        {
            return await _repository.RemoveAdAsync(adId, sponsorId);
        }

        [HttpGet("/SubscribeForEvent/{userId}/{eventId}")]
        public async Task<byte> SubscribeForEvent(long userId, long eventId)
        {
            return await _repository.SubscribeForEvent(userId, eventId);
        }

        [HttpGet("/UnsubscribeFromEvent/{userId}/{eventId}")]
        public async Task<byte> UnsubscribeFromEvent(long userId, long eventId)
        {
            return await _repository.UnsubscribeFromEvent(userId, eventId);
        }

        [HttpGet("/GetEventAttendees/{eventId}")]
        public async Task<List<User>> GetEventAttendees(long eventId)
        {
            return await _repository.GetEventAttendees(eventId);
        }

        [HttpGet("/GetEventInfo/{eventId}")]
        public async Task<Event> GetEventInfo(long eventId)
        {
            return await _repository.GetEventInfo(eventId);
        }

        [HttpGet("/GetSponsorInfo/{sponsorId}")]
        public async Task<Sponsor> GetSponsorInfo(long sponsorId)
        {
            return await _repository.GetSponsorInfo(sponsorId);
        }

        [HttpGet("/GetEventOwnerInfo/{eventId}")]
        public async Task<Sponsor> GetEventOwnerInfo(long eventId)
        {
            return await _repository.GetEventOwnerInfo(eventId);
        }

        [HttpPost("/UpdateContactInfoAsync")]
        public async Task<long> UpdateContactInfoAsync(SponsorContactInfo model)
        {
            return await _repository.UpdateContactInfoAsync(model);
        }

        [HttpPost("/CancelEventAsync")]
        public async Task<long> CancelEventAsync(CancelEvent model)
        {
            return await _repository.CancelEventAsync(model);
        }

        [HttpPost("/AddEventAsync")]
        public async Task<long> AddEventAsync(Event model)
        {
            return await _repository.AddEventAsync(model);
        }

        [HttpPost("/PostponeEventAsync")]
        public async Task<long> PostponeEventAsync(PostponeEvent model)
        {
            return await _repository.PostponeEventAsync(model);
        }

        [HttpPost("/UpdateEventAsync")]
        public async Task<long> UpdateEventAsync(Event model)
        {
            return await _repository.UpdateEventAsync(model);
        }

        [HttpPost("/AddSponsorRating")]
        public async Task<long> AddSponsorRating(SponsorRating model)
        {
            return await _repository.AddSponsorRating(model);
        }

        [HttpGet("/GetSponsorComments/{sponsorId}")]
        public async Task<List<string>> GetSponsorComments(long sponsorId)
        {
            return await _repository.GetSponsorComments(sponsorId);
        }

        [HttpGet("/GetSponsorLevel/{sponsorId}")]
        public async Task<int> GetSponsorLevel(long sponsorId)
        {
            return await _repository.GetSponsorLevel(sponsorId);
        }

        [HttpGet("/GetSponsorStats/{sponsorId}")]
        public async Task<Stats> GetSponsorStats(long sponsorId)
        {
            return await _repository.GetSponsorStats(sponsorId);
        }

        [HttpGet("/GetSponsorLevel/{sponsorId}/{progress}")]
        public async Task<int> AddSponsorProgress(long sponsorId, double progress)
        {
            return await _repository.AddSponorProgress(sponsorId, progress);
        }

        [HttpGet("/GetSponsorLevel/{sponsorId}/{level}")]
        public async Task<int> GetSponsorLevel(long sponsorId, int level)
        {
            return await _repository.UpdateSponsorLevel(sponsorId, level);
        }

        [HttpPost("/AddSponsorLanguage")]
        public async Task<long> AddSponsorLanguage(SponsorLanguage model)
        {
            return await _repository.AddSponsorLanguage(model);
        }

        [HttpPost("/AddEventTemplate")]
        public async Task<long> AddEventTemplate(EventTemplate model)
        {
            return await _repository.AddEventTemplate(model);
        }

        [HttpGet("/GetEventTemplateById/{templateId}")]
        public async Task<EventTemplate> GetEventTemplateById(long templateId)
        {
            return await _repository.GetEventTemplateById(templateId);
        }

        [HttpGet("/GetEventTemplateByName/{templateName}")]
        public async Task<EventTemplate> GetEventTemplateByName(string templateName)
        {
            return await _repository.GetEventTemplateByName(templateName);
        }

        [HttpGet("/GetSponsorEventTemplates/{sponsorId}")]
        public async Task<List<EventTemplate>> GetSponsorEventTemplates(long sponsorId)
        {
            return await _repository.GetSponsorEventTemplates(sponsorId);
        }

        [HttpDelete("/DeleteEventTemplate/{templateId}")]
        public async Task<bool> DeleteEventTemplate(long templateId)
        {
            return await _repository.DeleteEventTemplate(templateId);
        }

    }
}
