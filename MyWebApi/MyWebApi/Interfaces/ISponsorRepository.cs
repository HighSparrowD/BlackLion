using WebApi.Entities.SponsorEntities;
using WebApi.Entities.UserActionEntities;
using WebApi.Entities.UserInfoEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Enums;

namespace WebApi.Interfaces
{
    public interface ISponsorRepository
    {
        Task<bool> CheckUserIsAwaitingAsync(long userId);
        Task<bool> CheckUserIsAwaitingAsync(string username);
        Task<bool> CheckUserIsSponsorAsync(long userId);
        Task<bool> CheckSponsorIsMaxedAsync(long userId);
        Task<bool> CheckSponsorHasViewsLeftAsync(long userId);
        Task<bool> CheckUserIsPostponed(long userId);
        Task<bool> CheckUserKeyWordIsCorrect(long userId, string keyword);
        Task<List<Ad>> GetSponsorAdsAsync(long sponsorId);
        Task<List<Sponsor>> GetSponsorsAsync();
        Task<Sponsor> GetAwaitingUserAsync(string username);
        Task<Ad> GetSingleAdAsync(long sponsorId, long adId);
        Task<Sponsor> GetSingleSponsorAsync(long userId);
        Task<byte> RegisterAwaitingUserAsync(AwaitingUserRegistration user);
        Task<long> RegisterSponsorAsync(RegisterSponsor model);
        Task<long> AddAdAsync(Ad model);
        Task<long> AddContactInfoAsync(SponsorContactInfo model);
        Task<long> UpdateContactInfoAsync(SponsorContactInfo model);
        Task<Sponsor> GetSponsorInfo(long userId);
        Task<Guid> RegisterUserEventNotification(UserNotification model);
        Task<long> RegisterSponsorEventNotification(SponsorNotification model);
        Task<long> UpdateSponsorAsync(Sponsor model);
        Task<long> UpdateAdAsync(Ad model);
        Task<byte> RemoveSponsorAsync(long id );
        Task<byte> RemoveAdAsync(long adId, long sponsorId);
        Task<long> AddSponsorRating(SponsorRating model);
        Task<long> UpdateSponsorAverageRating(long sponsorId);
        Task<List<string>> GetSponsorComments(long sponsorId);
        Task<long> CreateSponorStats(long sponsorId);
        Task<int> AddSponorProgress(long sponsorId, double progress);
        Task<int> UpdateSponsorLevel(long sponsorId, int level);
        Task<int> GetSponsorLevel(long sponsorId);
        Task<Stats> GetSponsorStats(long sponsorId);
        Task<long> AddSponsorLanguage(SponsorLanguage model);
        Task<List<AppLanguage>> GetSponsorLanguagesAsync(long sponsorId);
    }
}
