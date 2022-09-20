using MyWebApi.Entities.SponsorEntities;
using MyWebApi.Entities.UserActionEntities;
using MyWebApi.Entities.UserInfoEntities;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace MyWebApi.Interfaces
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
        Task<long> AddEventAsync(Event model);
        Task<long> UpdateEventAsync(Event model);
        Task<long> PostponeEventAsync(PostponeEvent model);
        Task<long> CancelEventAsync(CancelEvent cancelModel);
        Task<long> AddContactInfoAsync(SponsorContactInfo model);
        Task<long> UpdateContactInfoAsync(SponsorContactInfo model);
        Task<Sponsor> GetEventOwnerInfo(long eventId);
        Task<Sponsor> GetSponsorInfo(long userId);
        Task<Event> GetEventInfo(long eventId);
        Task<List<User>> GetEventAttendees(long eventId);
        Task<long> RegisterUserEventNotification(UserNotification model);
        Task<long> RegisterSponsorEventNotification(SponsorNotification model);
        Task<long> UpdateSponsorAsync(Sponsor model);
        Task<long> UpdateAdAsync(Ad model);
        Task<byte> RemoveSponsorAsync(long id );
        Task<byte> RemoveAdAsync(long adId, long sponsorId);
        Task<byte> SubscribeForEvent(long userId, long eventId);
        Task<byte> UnsubscribeFromEvent(long userId, long eventId);
        Task<long> AddSponsorRating(SponsorRating model);
        Task<long> UpdateSponsorAverageRating(long sponsorId);
        Task<List<string>> GetSponsorComments(long sponsorId);
        Task<long> CreateSponorStats(long sponsorId);
        Task<int> AddSponorProgress(long sponsorId, double progress);
        Task<int> UpdateSponsorLevel(long sponsorId, int level);
        Task<int> GetSponsorLevel(long sponsorId);
        Task<Stats> GetSponsorStats(long sponsorId);
    }
}
