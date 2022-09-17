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
        Task<List<Ad>> GetSponsorAdsAsync(long sponsorId);
        Task<List<Sponsor>> GetSponsorsAsync();
        Task<Sponsor> GetAwaitingUserAsync(string username);
        Task<Ad> GetSingleAdAsync(long sponsorId, long adId);
        Task<Sponsor> GetSingleSponsorAsync(long userId);
        Task<byte> RegisterAwaitingUserAsync(AwaitingUserRegistration user);
        Task<long> RegisterSponsorAsync(Sponsor user);
        Task<long> AddAdAsync(Ad model);
        Task<long> AddEventAsync(Event model);
        Task<long> UpdateEventAsync(Event model);
        Task<long> PostponeEventAsync(PostponeEvent model);
        Task<long> CancelEventAsync(Event model);
        Task<long> AddContactInfoAsync(ContactInfo model);
        Task<long> UpdateContactInfoAsync(ContactInfo model);
        Task<Sponsor> GetEventOwnerInfo(long eventId);
        Task<Sponsor> GetSponsorInfo(long userId);
        Task<Event> GetEventInfo(long eventId);
        Task<List<User>> GetEventAttendees(long eventId);
        Task<long> RegisterEventNotification(UserNotification model);
        Task<long> UpdateSponsorAsync(Sponsor model);
        Task<long> UpdateAdAsync(Ad model);
        Task<byte> RemoveSponsorAsync(long id );
        Task<byte> RemoveAdAsync(long adId, long sponsorId);
    }
}
