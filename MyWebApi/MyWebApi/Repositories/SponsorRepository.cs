using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApi.Data;
using WebApi.Entities.SponsorEntities;
using WebApi.Entities.UserActionEntities;
using WebApi.Entities.UserInfoEntities;
using WebApi.Enums;
using WebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static WebApi.Enums.SystemEnums;

namespace WebApi.Repositories
{
    public class SponsorRepository : ISponsorRepository
    {
        private UserContext _contx { get; set; }

        public SponsorRepository(UserContext context)
        {
            _contx = context;
        }

        public async Task<bool> CheckUserIsSponsorAsync(long userId)
        { 
            var user = await _contx.Sponsors.Where(u => u.Id == userId).SingleOrDefaultAsync();
            if (user != null && !user.IsAwaiting) {
                return true;
            }

            return false;
        }

        public async Task<List<Ad>> GetSponsorAdsAsync(long sponsorId)
        {
            return await _contx.Ads.Where(a => a.SponsorId == sponsorId).ToListAsync();
        }

        public async Task<Ad> GetSingleAdAsync(long sponsorId, long adId)
        {
            return await _contx.Ads.Where(a => a.SponsorId == sponsorId && a.Id == adId).SingleAsync();
        }

        public async Task<long> RegisterSponsorAsync(RegisterSponsor model)
        {
            try
            {
                await RemoveSponsorByCodeWordAsync(model.CodeWord);
                var contactInfoId = await AddContactInfoAsync(new SponsorContactInfo {Id = model.Id, SponsorId = model.Id, Email = model.Email, Facebook = model.Facebook, Instagram = model.Instagram, Tel = model.Tel });
                var statsId = await CreateSponorStats(model.Id);
                var hasBaseAccount = await _contx.Users.FindAsync(model.Id) != null;

                var user = new Sponsor
                {
                    Id = model.Id,
                    Username = model.Username,
                    UserMaxAdCount = model.UserMaxAdCount,
                    UserMaxAdViewCount = model.UserMaxAdViewCount,
                    Age = model.Age,
                    IsPostponed = false,
                    IsAwaiting = false,
                    UserAppLanguage = model.UserAppLanguage,
                    UserCountryId = model.UserCountryId,
                    UserCityId = model.UserCityId,
                    ContactInfoId = contactInfoId,
                    StatsId = statsId,
                    HasBaseAccount = hasBaseAccount
                };

                user.IsAwaiting = false;
                await _contx.Sponsors.AddAsync(user);
                await _contx.SaveChangesAsync();

                for (int i = 0; i <= model.Languages.Count - 1; i++)
                {
                    await AddSponsorLanguage(new SponsorLanguage { SponsorId = model.Id, LanguageClassLocalisationId = model.UserAppLanguage, LanguageId = model.Languages[i], Level = (short)(LanguageLevels)Enum.Parse(typeof(LanguageLevels), model.LanguageLevels[i]) }); ;
                }

                return user.Id;
            }
            catch { return 0; }
        }

        public async Task<long> AddAdAsync(Ad model)
        {
            model.Id = await _contx.Ads.CountAsync() + 1;
            model.Description = Ad.TrancateDescription(model.Text, 15);
            await _contx.Ads.AddAsync(model);
            await _contx.SaveChangesAsync();
            return model.Id;
        }

        public async Task<bool> CheckUserIsPostponed(long userId)
        {
            if (await _contx.Sponsors.SingleOrDefaultAsync(u => u.Id == userId) is null)
            { return false; }
            var sponsor = await _contx.Sponsors.SingleOrDefaultAsync(u => u.Id == userId);
            return sponsor.IsPostponed;
        }

        public async Task<byte> RemoveSponsorAsync(long id)
        {
            try
            {
                //Delete all related sponsors ads
                _contx.RemoveRange(await _contx.Ads.Where(a => a.SponsorId == id).ToListAsync()); //Consider achieving this in the other way

                var user = _contx.Sponsors.Where(u => u.Id == id).SingleOrDefault();
                _contx.Remove(user);
                await _contx.SaveChangesAsync();
                return 1;
            }
            catch { return 0; }
        }

        public async Task<byte> RemoveSponsorByCodeWordAsync(string codeWord)
        {
            try
            {
                var user = await _contx.Sponsors.Where(u => u.CodeWord == codeWord).FirstOrDefaultAsync();
                _contx.Remove(user);
                await _contx.SaveChangesAsync();
                return 1;
            }
            catch { return 0; }
        }

        public async Task<Sponsor> GetSingleSponsorAsync(long userId)
        {
            return await _contx.Sponsors.SingleOrDefaultAsync(s => s.Id == userId);
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
            return await _contx.Sponsors.ToListAsync();
        }

        public async Task<long> UpdateSponsorAsync(Sponsor model)
        {
            _contx.Update(model);
            await _contx.SaveChangesAsync();
            return model.Id;
        }

        public async Task<bool> CheckUserIsAwaitingAsync(long userId)
        {
            var user = await _contx.Sponsors.Where(s => s.Id == userId).SingleOrDefaultAsync();

            if (user != null)
            {
                return user.IsAwaiting;
            }

            return false;
        }

        public async Task<Sponsor> GetAwaitingUserAsync(string username)
        {
            return (await _contx.Sponsors.Where(u => u.Username == username && u.IsAwaiting).SingleOrDefaultAsync());
        }

        public async Task<byte> RegisterAwaitingUserAsync(AwaitingUserRegistration user)
        {
            try
            {
                var sponsor = new Sponsor{
                    Id = await _contx.Sponsors.CountAsync() +1, 
                    Username = user.Username,
                    CodeWord = user.CodeWord,
                    UserMaxAdCount = user.UserMaxAdCount, 
                    UserMaxAdViewCount= user.UserMaxAdViewCount,
                    UserAppLanguage = user.UserAppLanguage,
                    IsAwaiting = true, 
                    IsPostponed = false
                };
                _contx.Sponsors.Add(sponsor);
                await _contx.SaveChangesAsync();
                return 1;
            }
            catch { return 0; }
        }

        public async Task<byte> RemoveAdAsync(long adId, long sponsorId)
        {
            try
            {
                var ad =await _contx.Ads.Where(a => a.Id == adId && a.SponsorId == sponsorId).SingleOrDefaultAsync();
                _contx.Remove(ad);
                await _contx.SaveChangesAsync();
                return 1;
            }
            catch { return 0; }
        }

        public async Task<bool> CheckUserIsAwaitingAsync(string username)
        {
            var user = await _contx.Sponsors.Where(s => s.Username == username).SingleOrDefaultAsync();

            if (user == null)
            {
                return false;
            }

            return user.IsAwaiting;
        }

        public async Task<bool> CheckSponsorIsMaxedAsync(long userId)
        {
            var user = await _contx.Sponsors.Where(s => s.Id == userId).SingleOrDefaultAsync();
            var userAdsCount = await _contx.Ads.Where(a => a.SponsorId == userId).CountAsync() +1;

            if (user == null)
                return false;

            if (userAdsCount > user.UserMaxAdCount)
                return true; 

            return false;
        }

        public async Task<bool> CheckSponsorHasViewsLeftAsync(long userId)
        {
            var user = await _contx.Sponsors.Where(s => s.Id == userId).SingleOrDefaultAsync();

            if (user == null)
                return false;

            return user.UserMaxAdViewCount > 0;
        }

        public async Task<long> AddContactInfoAsync(SponsorContactInfo model)
        {
            await _contx.AddAsync(model);
            await _contx.SaveChangesAsync();

            return model.SponsorId;
        }

        public async Task<long> UpdateContactInfoAsync(SponsorContactInfo model)
        {
            _contx.SponsorContactInfo.Update(model);
            await _contx.SaveChangesAsync();

            return model.SponsorId;
        }

        public async Task<Sponsor> GetSponsorInfo(long userId)
        {
            return await _contx.Sponsors
                .Where(s => s.Id == userId)
                .Include(s => s.SponsorContactInfo)
                .Include(s => s.Stats)
                .Include("SponsorLanguages")
                .FirstOrDefaultAsync();
        }

        public async Task<Guid> RegisterUserEventNotification(UserNotification model)
        {
            model.Id = Guid.NewGuid();
            await _contx.Notifications.AddAsync(model);
            await _contx.SaveChangesAsync();

            return model.Id;
        }
        public async Task<long> RegisterSponsorEventNotification(SponsorNotification model)
        {
            model.Id = (await _contx.SponsorNotifications.CountAsync()) + 1;
            await _contx.SponsorNotifications.AddAsync(model);
            await _contx.SaveChangesAsync();

            return model.Id;
        }

        public async Task<long> AddSponsorRating(SponsorRating model)
        {
            model.Id = (await _contx.SponsorRatings.CountAsync()) + 1;
            model.Comment = $"{model.UserId}\n⭐️{model.Rating} / 5⭐️\n{model.CommentTime.ToShortDateString()}\n\n{model.Comment}";

            await _contx.SponsorRatings.AddAsync(model);
            await RegisterSponsorEventNotification(new SponsorNotification { Description = $"You have a new event comment !\n{model.Comment}", SponsorId = model.SponsorId, NotificationReason = (short)NotificationReasons.Comment});

            await UpdateSponsorAverageRating(model.SponsorId);
            return model.Id;
        }

        public async Task<long> UpdateSponsorAverageRating(long sponsorId)
        {
            var sponsor = await _contx.Sponsors
                .Where(s => s.Id == sponsorId)
                //.Include(s => s.Stats)
                .FirstOrDefaultAsync();

            var newAwerageRating = 0d;
            var ratingsCount = 0;

            await _contx.SponsorRatings
                .Where(s => s.SponsorId == sponsorId)
                .ForEachAsync(r => 
                {
                    newAwerageRating += (double)r.Rating;
                    ratingsCount++;
                });

            sponsor.Stats.AverageRating = (double)Math.Round(newAwerageRating / ratingsCount, 2);

            _contx.Sponsors.Update(sponsor);
            await _contx.SaveChangesAsync();

            return sponsor.Id;
        }

        public async Task<List<string>> GetSponsorComments(long sponsorId)
        {
            return await _contx.SponsorRatings
                .Where(r => r.SponsorId == sponsorId)
                .Select(r => r.Comment)
                .ToListAsync();
        }

        public async Task<int> AddSponorProgress(long sponsorId, double progress)
        {
            var model = await _contx.SponsorStats  
                .Where(l => l.SponsorId == sponsorId)
                .FirstOrDefaultAsync();

            if (model != null)
            {
                if (model.LevelProgress + progress >= model.LevelGoal)
                {
                    model.LevelProgress = (model.LevelProgress + progress) - model.LevelGoal;
                    model.Level += 1;
                    model.LevelGoal *= 1.6;
                }
                else
                {
                    model.LevelProgress += progress;
                }

                _contx.SponsorStats.Update(model);
                await _contx.SaveChangesAsync();

                return model.Level;
            }
            return -1;
        }

        public async Task<int> UpdateSponsorLevel(long sponsorId, int level)
        {
            var model = await _contx.SponsorStats
                .Where(l => l.SponsorId == sponsorId)
                .FirstOrDefaultAsync();

            model.Level = level;

            _contx.SponsorStats.Update(model);
            await _contx.SaveChangesAsync();

            return model.Level;
        }

        public async Task<int> GetSponsorLevel(long sponsorId)
        {
            return (await _contx.SponsorStats.FindAsync(sponsorId)).Level;
        }

        public async Task<Stats> GetSponsorStats(long sponsorId)
        {
            return await _contx.SponsorStats.FindAsync(sponsorId);
        }

        public async Task<long> CreateSponorStats(long sponsorId)
        {
            try
            {
                await _contx.AddAsync(Stats.CreateDefaultStats(sponsorId));
                await _contx.SaveChangesAsync();

                return sponsorId;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<bool> CheckUserKeyWordIsCorrect(long userId, string keyword)
        {
            try
            {
                return await _contx.Sponsors.Where(u => u.Id == userId && u.CodeWord == keyword).FirstOrDefaultAsync() != null;
            }
            catch { throw new NullReferenceException($"User {userId} does not exists!"); }
        }

        public async Task<long> AddSponsorLanguage(SponsorLanguage model)
        {
                model.Id = (await _contx.SponsorLanguages.CountAsync()) +1;
                await _contx.SponsorLanguages.AddAsync(model);
                await _contx.SaveChangesAsync();
            return model.Id;
        }

        public async Task<List<int>> GetSponsorLanguagesAsync(long sponsorId)
        {
            return await _contx.SponsorLanguages
                .Where(l => l.SponsorId == sponsorId)
                .Select(l => l.LanguageId)
                .ToListAsync();
        }
    }
}
