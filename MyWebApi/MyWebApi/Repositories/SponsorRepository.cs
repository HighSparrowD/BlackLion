using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyWebApi.Data;
using MyWebApi.Entities.SponsorEntities;
using MyWebApi.Entities.UserActionEntities;
using MyWebApi.Entities.UserInfoEntities;
using MyWebApi.Enums;
using MyWebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MyWebApi.Enums.SystemEnums;

namespace MyWebApi.Repositories
{
    public class SponsorRepository : ISponsorRepository
    {
        private SponsorContext _contx { get; set; }

        public SponsorRepository(SponsorContext context)
        {
            _contx = context;
        }

        public async Task<bool> CheckUserIsSponsorAsync(long userId)
        { 
            var user = await _contx.SYSTEM_SPONSORS.Where(u => u.Id == userId).SingleOrDefaultAsync();
            if (user != null && !user.IsAwaiting) {
                return true;
            }

            return false;
        }

        public async Task<List<Ad>> GetSponsorAdsAsync(long sponsorId)
        {
            return await _contx.SPONSOR_ADS.Where(a => a.SponsorId == sponsorId).ToListAsync();
        }

        public async Task<Ad> GetSingleAdAsync(long sponsorId, long adId)
        {
            return await _contx.SPONSOR_ADS.Where(a => a.SponsorId == sponsorId && a.Id == adId).SingleAsync();
        }

        public async Task<long> RegisterSponsorAsync(RegisterSponsor model)
        {
            try
            {
                await RemoveSponsorByUsernameAsync(model.Username);
                await AddContactInfoAsync(new ContactInfo { SponsorId = model.Id, Email = model.Email, Facebook = model.Facebook, Instagram = model.Instagram, Tel = model.Tel });

                var user = new Sponsor
                {
                    Id = model.Id,
                    Username = model.Username,
                    UserMaxAdCount = model.UserMaxAdCount,
                    UserMaxAdViewCount = model.UserMaxAdViewCount,
                    IsPostponed = false,
                    IsAwaiting = false,
                    UserAppLanguage = model.UserAppLanguage,
                    SponsorContactInfoId = model.Id,
                    AverageRating = null
                };

                user.IsAwaiting = false;
                await _contx.SYSTEM_SPONSORS.AddAsync(user);
                await _contx.SaveChangesAsync();
                return user.Id;
            }
            catch { return 0; }
        }

        public async Task<long> AddAdAsync(Ad model)
        {
            model.Id = await _contx.SPONSOR_ADS.CountAsync() + 1;
            model.Description = Ad.TrancateDescription(model.Text, 15);
            await _contx.SPONSOR_ADS.AddAsync(model);
            await _contx.SaveChangesAsync();
            return model.Id;
        }

        public async Task<bool> CheckUserIsPostponed(long userId)
        {
            if (await _contx.SYSTEM_SPONSORS.SingleOrDefaultAsync(u => u.Id == userId) is null)
            { return false; }
            var sponsor = await _contx.SYSTEM_SPONSORS.SingleOrDefaultAsync(u => u.Id == userId);
            return sponsor.IsPostponed;
        }

        public async Task<byte> RemoveSponsorAsync(long id)
        {
            try
            {
                //Delete all related sponsors ads
                _contx.RemoveRange(await _contx.SPONSOR_ADS.Where(a => a.SponsorId == id).ToListAsync()); //Consider achieving this in the other way

                var user = _contx.SYSTEM_SPONSORS.Where(u => u.Id == id).SingleOrDefault();
                _contx.Remove(user);
                await _contx.SaveChangesAsync();
                return 1;
            }
            catch { return 0; }
        }

        public async Task<byte> RemoveSponsorByUsernameAsync(string username)
        {
            try
            {
                var user = _contx.SYSTEM_SPONSORS.Where(u => u.Username == username).SingleOrDefault();
                _contx.Remove(user);
                await _contx.SaveChangesAsync();
                return 1;
            }
            catch { return 0; }
        }

        public async Task<Sponsor> GetSingleSponsorAsync(long userId)
        {
            return await _contx.SYSTEM_SPONSORS.SingleOrDefaultAsync(s => s.Id == userId);
        }

        public async Task<long> UpdateAdAsync(Ad model)
        {
            try
            {
                _contx.Update(model);
                await _contx.SaveChangesAsync();
                return model.Id;
            }
            catch { return 0; }
        }

        public async Task<List<Sponsor>> GetSponsorsAsync()
        {
            return await _contx.SYSTEM_SPONSORS.ToListAsync();
        }

        public async Task<long> UpdateSponsorAsync(Sponsor model)
        {
            _contx.Update(model);
            await _contx.SaveChangesAsync();
            return model.Id;
        }

        public async Task<bool> CheckUserIsAwaitingAsync(long userId)
        {
            var user = await _contx.SYSTEM_SPONSORS.Where(s => s.Id == userId).SingleOrDefaultAsync();

            if (user != null)
            {
                return user.IsAwaiting;
            }

            return false;
        }

        public async Task<Sponsor> GetAwaitingUserAsync(string username)
        {
            return (await _contx.SYSTEM_SPONSORS.Where(u => u.Username == username && u.IsAwaiting).SingleOrDefaultAsync());
        }

        public async Task<byte> RegisterAwaitingUserAsync(AwaitingUserRegistration user)
        {
            try
            {
                var sponsor = new Sponsor{
                    Id = await _contx.SYSTEM_SPONSORS.CountAsync() +1, 
                    Username=user.Username,
                    UserMaxAdCount = user.UserMaxAdCount, 
                    UserMaxAdViewCount= user.UserMaxAdViewCount,
                    UserAppLanguage = user.UserAppLanguage,
                    IsAwaiting = true, 
                    IsPostponed = false
                };
                _contx.SYSTEM_SPONSORS.Add(sponsor);
                await _contx.SaveChangesAsync();
                return 1;
            }
            catch { return 0; }
        }

        public async Task<byte> RemoveAdAsync(long adId, long sponsorId)
        {
            try
            {
                var ad =await _contx.SPONSOR_ADS.Where(a => a.Id == adId && a.SponsorId == sponsorId).SingleOrDefaultAsync();
                _contx.Remove(ad);
                await _contx.SaveChangesAsync();
                return 1;
            }
            catch { return 0; }
        }

        public async Task<bool> CheckUserIsAwaitingAsync(string username)
        {
            var user = await _contx.SYSTEM_SPONSORS.Where(s => s.Username == username).SingleOrDefaultAsync();

            if (user == null)
            {
                return false;
            }

            return user.IsAwaiting;
        }

        public async Task<bool> CheckSponsorIsMaxedAsync(long userId)
        {
            var user = await _contx.SYSTEM_SPONSORS.Where(s => s.Id == userId).SingleOrDefaultAsync();
            var userAdsCount = await _contx.SPONSOR_ADS.Where(a => a.SponsorId == userId).CountAsync() +1;

            if (user == null)
                return false;

            if (userAdsCount > user.UserMaxAdCount)
                return true; 

            return false;
        }

        public async Task<bool> CheckSponsorHasViewsLeftAsync(long userId)
        {
            var user = await _contx.SYSTEM_SPONSORS.Where(s => s.Id == userId).SingleOrDefaultAsync();

            if (user == null)
                return false;

            return user.UserMaxAdViewCount > 0;
        }

        public async Task<long> AddEventAsync(Event model)
        {
            var owner = await GetSponsorInfo(model.SponsorId);

            model.Id = (await _contx.SPONSOR_EVENTS.CountAsync()) + 1;
            model.Status = (short)EventStatuses.Created;
            model.StartDateTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            model.Description = $"{model.Name}\n\n{model.Description}\n\n@{owner.Username}";
            await _contx.SPONSOR_EVENTS.AddAsync(model);
            await _contx.SaveChangesAsync();

            return model.Id;
        }

        public async Task<long> UpdateEventAsync(Event model)
        {
            model.Status = (short)EventStatuses.Updated;
            _contx.Update(model);
            await _contx.SaveChangesAsync();

            await NotifyAttendees(model.Id, model.Comment);

            return model.Id;
        }

        public async Task<long> PostponeEventAsync(PostponeEvent model)
        {
            var currentEvent = await GetEventInfo(model.EventId);

            currentEvent.StartDateTime = DateTime.SpecifyKind(model.PostponeUntil, DateTimeKind.Utc);
            model.Comment = model.Comment;

            _contx.SPONSOR_EVENTS.Update(currentEvent);
            await _contx.SaveChangesAsync();

            await NotifyAttendees(model.EventId, model.Comment);

            return currentEvent.Id;
        }

        public async Task<long> CancelEventAsync(CancelEvent cancelModel)
        {
            var model = await GetEventInfo(cancelModel.EventId);

            model.Status = (short)EventStatuses.Canceled;
            _contx.Update(model);
            await _contx.SaveChangesAsync();

            await NotifyAttendees(model.Id, cancelModel.Comment);

            return model.Id;
        }

        public async Task<long> AddContactInfoAsync(ContactInfo model)
        {
            await _contx.AddAsync(model);
            await _contx.SaveChangesAsync();

            return model.SponsorId;
        }

        public async Task<long> UpdateContactInfoAsync(ContactInfo model)
        {
            _contx.SPONSOR_CONTACT_INFO.Update(model);
            await _contx.SaveChangesAsync();

            return model.SponsorId;
        }

        public async Task<Sponsor> GetEventOwnerInfo(long eventId)
        {
            var ownerId = (await GetEventInfo(eventId)).SponsorId;
            return await GetSponsorInfo(ownerId);
        }

        public async Task<Sponsor> GetSponsorInfo(long userId)
        {
            return await _contx.SYSTEM_SPONSORS
                .Where(s => s.Id == userId)
                .Include(s => s.ContactInfo)
                .Include(s => s.Languages)
                .FirstOrDefaultAsync();
        }

        public async Task<Event> GetEventInfo(long eventId)
        {
            return await _contx.SPONSOR_EVENTS
                .Where(e => e.Id == eventId)
                .FirstOrDefaultAsync();
        }

        public async Task<long> RegisterUserEventNotification(UserNotification model)
        {
            model.Id = (await _contx.USER_NOTIFICATIONS.CountAsync()) +1;
            await _contx.USER_NOTIFICATIONS.AddAsync(model);
            await _contx.SaveChangesAsync();

            return model.Id;
        }

        public async Task<List<User>> GetEventAttendees(long eventId)
        {
            return await _contx.USER_EVENTS
                .Where(e => e.EventId == eventId)
                .Include(e => e.Attendee)
                .Select(e => e.Attendee)
                .ToListAsync();
        }

        public async Task<byte> SubscribeForEvent(long userId, long eventId)
        {
            try
            {
                await _contx.USER_EVENTS.AddAsync(new UserEvent { UserId = userId, EventId = eventId });
                await _contx.SaveChangesAsync();

                var evnt = await _contx.SPONSOR_EVENTS
                    .Where(e => e.Id == eventId)
                    .FirstOrDefaultAsync();

                var notification = new SponsorNotification
                {
                    SponsorId = evnt.SponsorId,
                    NotificationReason = (int)NotificationReasons.Subscription,
                    Description = $"User {userId} has subscribed from your event {evnt.Id}/{evnt.Name}"
                };

                await RegisterSponsorEventNotification(notification);

                return 1;
            }
            catch { return 0; }
        }

        public async Task<byte> UnsubscribeFromEvent(long userId, long eventId)
        {
            try
            {
                var evnt = await _contx.USER_EVENTS
                    .Where(e => e.UserId == userId && e.EventId == eventId)
                    .Include(e => e.Event)
                    .FirstOrDefaultAsync();

                _contx.USER_EVENTS.Remove(evnt);
                await _contx.SaveChangesAsync();

                var notification = new SponsorNotification
                {
                    SponsorId = evnt.Event.SponsorId,
                    NotificationReason = (int)NotificationReasons.Unsubscription,
                    Description = $"User {userId} has unsubscribed from your event {evnt.EventId}/{evnt.Event.Name}"
                };

                await RegisterSponsorEventNotification(notification);

                return 1;
            }
            catch { return 0; }
        }

        public async Task<long> RegisterSponsorEventNotification(SponsorNotification model)
        {
            model.Id = (await _contx.SPONSOR_NOTIFICATIONS.CountAsync()) + 1;
            await _contx.SPONSOR_NOTIFICATIONS.AddAsync(model);
            await _contx.SaveChangesAsync();

            return model.Id;
        }

        private async Task NotifyAttendees(long eventId, string comment)
        {
            try
            {
                var attendees = await GetEventAttendees(eventId);
                var notification = new UserNotification { Severity = (short)Severities.Urgent, SectionId = (short)Sections.Eventer, IsLikedBack = false, Description = comment };

                foreach (var attendee in attendees)
                {
                    notification.UserId = null;
                    notification.UserId1 = attendee.UserId;
                    await RegisterUserEventNotification(notification);
                }
            }
            catch {  }
        }
    }
}
