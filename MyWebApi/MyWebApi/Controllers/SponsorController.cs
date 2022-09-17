using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyWebApi.Entities.SponsorEntities;
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
        public async Task<long> RegisterSponsorAsync(Sponsor user)
        {
            return await _repository.RegisterSponsorAsync(user);
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
    }
}
