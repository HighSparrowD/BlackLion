using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Entities.AchievementEntities;
using MyWebApi.Entities.LocationEntities;
using MyWebApi.Entities.ReasonEntities;
using MyWebApi.Entities.ReportEntities;
using MyWebApi.Entities.SecondaryEntities;
using MyWebApi.Entities.UserInfoEntities;
using MyWebApi.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace MyWebApi.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private UserContext _contx { get; set; }

        public AdminRepository(UserContext context)
        {
            _contx = context;
        }

        public async Task<long> UploadCities(List<City> cities)
        {
            cities.ForEach(async c => await _contx.CITIES.AddAsync(c));
            await _contx.SaveChangesAsync();
            return cities.Count;
        }

        public async Task<long> UploadCountries(List<Country> countries)
        {
            countries.ForEach(async c => await _contx.COUNTRIES.AddAsync(c));
            await _contx.SaveChangesAsync();

            //foreach (var item in countries)
            //{
            //    _contx.COUNTRIES.Add(item);
            //    await _contx.SaveChangesAsync();
            //}

            return countries.Count;
        }

        public async Task<long> UploadLanguages(List<Language> langs)
        {
            langs.ForEach(async l => await _contx.LANGUAGES.AddAsync(l));
            await _contx.SaveChangesAsync();

            return langs.Count;
        }

        public async Task<long> UploadFeedbackReasons(List<FeedbackReason> reasons)
        {
            reasons.ForEach(async r => await _contx.FEEDBACK_REASONS.AddAsync(r));
            await _contx.SaveChangesAsync();
            return reasons.Count;
        }

        public async Task<List<Feedback>> GetFeedbacks()
        {
            var reports = await _contx.SYSTEM_FEEDBACKS.Include(r => r.User)
                .Include(r => r.Reason).ToListAsync();

            return reports;
        }

        public async Task<bool> CheckUserIsAdmin(long userId)
        {
            var admin = await _contx.SYSTEM_ADMINS.FindAsync(userId);
            if (admin == null) { return false; }

            return admin.IsEnabled;
        }

        public async Task<byte> SwitchAdminStatus(long userId)
        {
            var admin = await _contx.SYSTEM_ADMINS.Where(a => a.Id == userId).SingleOrDefaultAsync();
            if (admin == null) { return 0; }

            admin.IsEnabled = admin.IsEnabled ? false : true;
            _contx.Update(admin);
            await _contx.SaveChangesAsync();
            return 1;
        }

        public async Task<bool?> GetAdminStatus(long userId)
        {
            var admin = await _contx.SYSTEM_ADMINS.Where(a => a.Id == userId).SingleOrDefaultAsync();
            if (admin == null){ return null; }

            return admin.IsEnabled;
        }

        public async Task<long> DeleteUser(long userId)
        {
            try
            {
                var user = await _contx.SYSTEM_USERS.Where(u => u.UserId == userId).SingleOrDefaultAsync();
                var userBase = await _contx.SYSTEM_USERS_BASES.Where(u => u.Id == userId).SingleOrDefaultAsync();
                var userData = await _contx.SYSTEM_USERS_DATA.Where(u => u.Id == userId).SingleOrDefaultAsync();
                var userPrefs = await _contx.SYSTEM_USERS_PREFERENCES.Where(u => u.Id == userId).SingleOrDefaultAsync();
                var userLocation = await _contx.USER_LOCATIONS.Where(u => u.Id == userId).SingleOrDefaultAsync();
                var userAchievements = await _contx.USER_ACHIEVEMENTS.Where(u => u.UserBaseInfoId == userId).ToListAsync();
                var userPurchases = await _contx.USER_WALLET_PURCHASES.Where(u => u.UserId == userId).ToListAsync();
                var userBalances = await _contx.USER_WALLET_BALANCES.Where(u => u.UserId == userId).ToListAsync();
                var userNotifications = await _contx.USER_NOTIFICATIONS.Where(u => u.UserId == userId).ToListAsync();
                var userNotifications1 = await _contx.USER_NOTIFICATIONS.Where(u => u.UserId1 == userId).ToListAsync();
                var sponsorRatings = await _contx.SPONSOR_RATINGS.Where(u => u.UserId == userId).ToListAsync();
                var userTrustLevel = await _contx.USER_TRUST_LEVELS.Where(u => u.Id == userId).SingleOrDefaultAsync();
                var userInvitations = await _contx.USER_INVITATIONS.Where(u => u.InvitorCredentials.UserId == userId).ToListAsync();
                var userInvitationCreds = await _contx.USER_INVITATION_CREDENTIALS.Where(u => u.UserId == userId).SingleOrDefaultAsync();

                if (userInvitations.Count > 0)
                {
                    _contx.USER_INVITATIONS.RemoveRange(userInvitations);
                    await _contx.SaveChangesAsync();
                }

                if (userInvitationCreds != null)
                {
                    _contx.USER_INVITATION_CREDENTIALS.Remove(userInvitationCreds);
                    await _contx.SaveChangesAsync();
                }

                if (userLocation != null)
                {
                    _contx.USER_LOCATIONS.Remove(userLocation);
                    await _contx.SaveChangesAsync();
                }
                if (userAchievements.Count > 0)
                {
                    _contx.USER_ACHIEVEMENTS.RemoveRange(userAchievements);
                    await _contx.SaveChangesAsync();
                }
                if (userBalances.Count > 0)
                {
                    _contx.USER_WALLET_BALANCES.RemoveRange(userBalances);
                    await _contx.SaveChangesAsync();
                }
                if (userPurchases.Count > 0)
                {
                    _contx.USER_WALLET_PURCHASES.RemoveRange(userPurchases);
                    await _contx.SaveChangesAsync();
                }
                if (userNotifications.Count > 0)
                {
                    _contx.USER_NOTIFICATIONS.RemoveRange(userNotifications);
                    await _contx.SaveChangesAsync();
                }
                if (userNotifications1.Count > 0)
                {
                    _contx.USER_NOTIFICATIONS.RemoveRange(userNotifications1);
                    await _contx.SaveChangesAsync();
                }
                if (sponsorRatings.Count > 0)
                {
                    _contx.SPONSOR_RATINGS.RemoveRange(sponsorRatings);
                    await _contx.SaveChangesAsync();
                }
                if (userTrustLevel != null)
                {
                    _contx.USER_TRUST_LEVELS.Remove(userTrustLevel);
                    await _contx.SaveChangesAsync();
                }
                if (userBase != null)
                {
                    _contx.SYSTEM_USERS_BASES.Remove(userBase);
                    await _contx.SaveChangesAsync();
                }
                if (userPrefs != null)
                {
                    _contx.SYSTEM_USERS_PREFERENCES.Remove(userPrefs);
                    await _contx.SaveChangesAsync();
                }
                if (userData != null)
                {
                    _contx.SYSTEM_USERS_DATA.Remove(userData);
                    await _contx.SaveChangesAsync();
                }
                if (user != null)
                {
                    _contx.SYSTEM_USERS.Remove(user);
                    await _contx.SaveChangesAsync();
                }

                await _contx.SaveChangesAsync();

                return userId;
            }
            catch { return -1; }
        }

        public async Task<int> DeleteAllUsers()
        {
            try
            {
                var usersCount = await _contx.SYSTEM_USERS.CountAsync();

                var user = await _contx.SYSTEM_USERS.ToListAsync();
                var userBase = await _contx.SYSTEM_USERS_BASES.ToListAsync();
                var userData = await _contx.SYSTEM_USERS_DATA.ToListAsync();
                var userPrefs = await _contx.SYSTEM_USERS_PREFERENCES.ToListAsync();
                var userLocation = await _contx.USER_LOCATIONS.ToListAsync();
                var userAchievements = await _contx.USER_ACHIEVEMENTS.ToListAsync();
                var userPurchases = await _contx.USER_WALLET_PURCHASES.ToListAsync();
                var userBalances = await _contx.USER_WALLET_BALANCES.ToListAsync();
                var userNotifications = await _contx.USER_NOTIFICATIONS.ToListAsync();
                var userNotifications1 = await _contx.USER_NOTIFICATIONS.ToListAsync();
                var sponsorRatings = await _contx.SPONSOR_RATINGS.ToListAsync();

                _contx.USER_LOCATIONS.RemoveRange(userLocation);
                _contx.USER_ACHIEVEMENTS.RemoveRange(userAchievements);
                _contx.USER_WALLET_BALANCES.RemoveRange(userBalances);
                _contx.USER_WALLET_PURCHASES.RemoveRange(userPurchases);
                _contx.USER_NOTIFICATIONS.RemoveRange(userNotifications);
                _contx.USER_NOTIFICATIONS.RemoveRange(userNotifications1);
                _contx.SPONSOR_RATINGS.RemoveRange(sponsorRatings);
                _contx.SYSTEM_USERS_BASES.RemoveRange(userBase);
                _contx.SYSTEM_USERS_PREFERENCES.RemoveRange(userPrefs);
                _contx.SYSTEM_USERS_DATA.RemoveRange(userData);
                _contx.SYSTEM_USERS.RemoveRange(user);

                await _contx.SaveChangesAsync();

                return usersCount;
            }
            catch { return -1; }
        }

        public async Task<byte> UploadAchievements(List<Achievement> achievements)
        {
            try
            {
                var ach = await _contx.SYSTEM_ACHIEVEMENTS.ToListAsync();

                if (ach != null)
                    _contx.SYSTEM_ACHIEVEMENTS.RemoveRange(ach);

                await _contx.SYSTEM_ACHIEVEMENTS.AddRangeAsync(achievements);
                await _contx.SaveChangesAsync();

                return 1;
            }
            catch { return 0; }
        }

        public async Task<byte> AddNewAchievements(List<Achievement> achievements)
        {
            try
            {
                await _contx.SYSTEM_ACHIEVEMENTS.AddRangeAsync(achievements);
                await _contx.SaveChangesAsync();
            }
            catch { return 0; }
        }
    }
}
