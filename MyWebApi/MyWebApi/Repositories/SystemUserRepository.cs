using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations;
using MyWebApi.Data;
using MyWebApi.Entities.AchievementEntities;
using MyWebApi.Entities.LocationEntities;
using MyWebApi.Entities.ReasonEntities;
using MyWebApi.Entities.ReportEntities;
using MyWebApi.Entities.SecondaryEntities;
using MyWebApi.Entities.SponsorEntities;
using MyWebApi.Entities.UserActionEntities;
using MyWebApi.Entities.UserInfoEntities;
using MyWebApi.Enums;
using MyWebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace MyWebApi.Repositories
{
    public class SystemUserRepository : IUserRepository
    {
        private UserContext _contx { get; set; }

        public SystemUserRepository(UserContext context)
        {
            _contx = context;
        }

        public Task<long> AddFriendUserAsync(long id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<long> RegisterUserAsync(User model, UserBaseInfo baseModel, UserDataInfo dataModel, UserPreferences prefsModel, Location location, bool wasRegistered = false)
        {
            await _contx.USER_LOCATIONS.AddAsync(location);

            var country = (await _contx.COUNTRIES.Where(c => c.Id == location.CountryId && c.ClassLocalisationId == dataModel.LanguageId).SingleOrDefaultAsync()).CountryName;
            var city = (await _contx.CITIES.Where(c => c.Id == location.CountryId && c.CountryClassLocalisationId == dataModel.LanguageId).SingleOrDefaultAsync()).CityName;

            baseModel.UserDescription = model.GenerateUserDescription(baseModel.UserRealName, dataModel.UserAge, country, city, baseModel.UserDescription);

            model.HadReceivedReward = false;
            model.DailyRewardPoint = 1;
            model.BonusIndex = 1;

            await _contx.SYSTEM_USERS_BASES.AddAsync(baseModel);
            await _contx.SaveChangesAsync();
            await _contx.SYSTEM_USERS_DATA.AddAsync(dataModel);
            await _contx.SaveChangesAsync();
            await _contx.SYSTEM_USERS_PREFERENCES.AddAsync(prefsModel);
            await _contx.SaveChangesAsync();
            await _contx.SYSTEM_USERS.AddAsync(model);
            await _contx.SaveChangesAsync();

            await GenerateUserAchievementList(baseModel.Id, dataModel.LanguageId, wasRegistered);
            await TopUpUserWalletBalance(model.UserId, 180, "Starting Pack"); //180 is a starting user point pack
            await AddUserTrustLevel(model.UserId);
            await AddUserTrustProgressAsync(model.UserId, 0.000012);

            var invitation = await GetInvitationAsync(model.UserId);

            if(invitation != null)
            {
                var invitor = invitation.InvitorCredentials.Invitor;
                model.BonusIndex = 2;
                model.ParentId = invitor.UserId;

                _contx.SYSTEM_USERS.Update(model);
                await _contx.SaveChangesAsync();

                //User is instantly liked by an invitor (Possibly let users turn of that feature)
                await RegisterUserRequest(new UserNotification { UserId = invitor.UserId, UserId1 = model.UserId, IsLikedBack = false });
                //Invitor is notified about referential registration
                await NotifyUserAboutReferentialRegistrationAsync(model.UserId, invitor.UserId);
            }

            return model.UserId;
        }

        public async Task<List<long>> GetAllUsersAsync()
        {
            return await _contx.SYSTEM_USERS
                .Where(u => u.UserId != 1324407781) //All except Sania's account
                .Select(u => u.UserId)
                .ToListAsync(); 
        }

        public Task<User> GetFriendInfoAsync(long id)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<FriendModel>> GetFriendsAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<User> GetUserInfoAsync(long id)
        {
            return await _contx.SYSTEM_USERS.Where(u => u.UserId == id).Include(s => s.UserBaseInfo)
                .Include(s => s.UserBaseInfo)
                .Include(s => s.UserDataInfo).ThenInclude(s => s.Location)
                .Include(s => s.UserDataInfo).ThenInclude(s => s.Reason)
                .Include(s => s.UserPreferences)
                .Include(s => s.UserBlackList)
                .SingleOrDefaultAsync();
        }

        public async Task<List<User>> GetUsersAsync(long userId)
        {
            var currentUser = await GetUserInfoAsync(userId);
            var currentUserEncounters = await GetUserEncounters(userId, (int)SystemEnums.Sections.Familiator); //I am not sure if it is 2 or 3 section

            var data = await _contx.SYSTEM_USERS
                .Where(u => u.UserDataInfo.ReasonId == currentUser.UserDataInfo.ReasonId)
                .Where(u => u.UserPreferences.CommunicationPrefs == currentUser.UserPreferences.CommunicationPrefs)
                .Where(u => u.UserPreferences.AgePrefs.Contains(currentUser.UserDataInfo.UserAge))
                .Where(u => u.UserPreferences.UserLocationPreferences.Contains(currentUser.UserDataInfo.Location.CountryId))
                .Where(u => u.UserPreferences.UserGenderPrefs == currentUser.UserDataInfo.UserGender)
                .Where(u => u.UserDataInfo.UserLanguages.Any(l => currentUser.UserPreferences.UserLanguagePreferences.Contains(l)))
                .Where(u => currentUser.UserPreferences.AgePrefs.Contains(u.UserDataInfo.UserAge))
                .Where(u => currentUser.UserPreferences.UserLocationPreferences.Contains(u.UserDataInfo.Location.CountryId))
                .Where(u => currentUser.UserDataInfo.UserLanguages.Any(l => u.UserPreferences.UserLanguagePreferences.Contains(l)))
                .Where(u => u.UserId != currentUser.UserId)
                .Include(u => u.UserBaseInfo)
                .Include(u => u.UserDataInfo)
                .Include(u => u.UserPreferences)
                .Include(u => u.UserBlackList)
                .ToListAsync();

            //Check if users had encountered one another
            data = data.Where(u => !currentUser.CheckIfHasEncountered(currentUserEncounters, u.UserId)).ToList();

            //Check if users are in each others black lists
            data = data.Where(u => u.UserBlackList.Where(u => u.UserId1 == userId).SingleOrDefault() == null).ToList();
            data = data.Where(u => currentUser.UserBlackList.Where(l => l.UserId1 == u.UserId).SingleOrDefault() == null).ToList();

            //Check if request already exists
            data = data.Where(u => !CheckRequestExists(userId, u.UserId)).ToList();

            //Check if current user had already recieved request from user
            data = data.Where(u => !CheckRequestExists(u.UserId, userId)).ToList();

            //If user does NOT have gender prederences
            if (currentUser.UserPreferences.UserGenderPrefs == 2)
            {
                data = data
                    .Where(u => u.UserPreferences.UserGenderPrefs == currentUser.UserDataInfo.UserGender || u.UserPreferences.UserGenderPrefs == currentUser.UserPreferences.UserGenderPrefs)
                    .ToList();
            }
            //If he does
            else
            {
                data = data.Where(u => u.UserDataInfo.UserGender == currentUser.UserPreferences.UserGenderPrefs)
                .Where(u => currentUser.UserPreferences.UserGenderPrefs == u.UserDataInfo.UserGender)
                .ToList();
            }

            //Todo: Probably remove AsParallel query 
            data.AsParallel().ForAll(u => 
            {
                if (u.Nickname != "" && (bool)u.HasPremium)
                    u.UserBaseInfo.UserDescription = $"{u.Nickname}\n\n{u.UserBaseInfo.UserDescription}";
            });

            await AddUserTrustProgressAsync(userId, 0.000003);

            return data;
        }

        public async Task<Country> GetCountryAsync(long id)
        {
            var c = await _contx.COUNTRIES.Include(c => c.Cities).SingleAsync(c => c.Id == id);
            return c;
        }

        public async Task<long> AddFeedbackAsync(Feedback report)
        {
            report.Id = await _contx.SYSTEM_FEEDBACKS.CountAsync() + 1;
            await _contx.SYSTEM_FEEDBACKS.AddAsync(report);
            await _contx.SaveChangesAsync();
            return report.Id;
        }

        public async Task<bool> CheckUserExists(long id)
        {
            if (await _contx.SYSTEM_USERS.FindAsync(id) == null)
            { return false; }
            return true;
        }

        public async Task<int> GetUserAppLanguage(long id)
        {
            var data = await _contx.SYSTEM_USERS_DATA.Where(u => u.Id == id).SingleAsync();
            return data.LanguageId;
        }

        public async Task<List<FeedbackReason>> GetFeedbackReasonsAsync(int localisationId)
        {
            return await _contx.FEEDBACK_REASONS.Where(r => r.ClassLocalisationId == localisationId).ToListAsync();
        }

        public async Task<bool> CheckUserIsRegistered(long userId)
        {
            return await _contx.SYSTEM_USERS.FindAsync(userId) != null;
        }

        public async Task<UserBaseInfo> GetUserBaseInfoAsync(long id)
        {
            return await _contx.SYSTEM_USERS_BASES.FindAsync(id);
        }

        public async Task<bool> CheckUserHasVisitedSection(long userId, int sectionId)
        {
            var visit = await _contx.USER_VISITS.
                Where(v => v.UserId == userId && v.SectionId == sectionId)
                .FirstOrDefaultAsync();

            if (visit != null)
            {
                await AddUserTrustProgressAsync(userId, 0.000002);
                return true;
            }

            if (await CheckUserIsRegistered(userId))
            {
                await AddUserTrustProgressAsync(userId, 0.000002);
                await _contx.USER_VISITS.AddAsync(new Visit { UserId = userId, SectionId = sectionId });
                await _contx.SaveChangesAsync();
                return false;
            }

            return false;
        }

        public async Task<User> GetUserInfoByUsrnameAsync(string username)
        {
            return await _contx.SYSTEM_USERS
                .Where(u => u.UserBaseInfo.UserName == username)
                .Include(s => s.UserBaseInfo)
                .Include(s => s.UserDataInfo).ThenInclude(s => s.Location)
                .Include(s => s.UserDataInfo).ThenInclude(s => s.Reason)
                .Include(s => s.UserPreferences)
                .SingleOrDefaultAsync();
        }

        public async Task<List<Feedback>> GetMostRecentFeedbacks()
        {
            var pointInTime = DateTime.SpecifyKind(DateTime.Now.AddDays(-2), DateTimeKind.Utc);
            return await _contx.SYSTEM_FEEDBACKS
                .Where(f => f.InsertedUtc >= pointInTime)
                .Include(f => f.User)
                .ToListAsync();
        }

        public async Task<List<Feedback>> GetMostRecentFeedbacksByUserId(long userId)
        {
            var pointInTime = DateTime.SpecifyKind(DateTime.Now.AddDays(-2), DateTimeKind.Utc);
            return await _contx.SYSTEM_FEEDBACKS
                .Where(f => f.InsertedUtc >= pointInTime && f.UserBaseInfoId == userId)
                .Include(f => f.User)
                .Include(f => f.Reason)
                .ToListAsync();
        }

        public async Task<Feedback> GetFeedbackById(long id)
        {
            return await _contx.SYSTEM_FEEDBACKS
                .Where(f => f.Id == id)
                .Include(f => f.User)
                .Include(f => f.Reason)
                .SingleOrDefaultAsync();
        }

        public async Task<long> AddUserReportAsync(Report report)
        {
            try
            {
                report.Id = await _contx.USER_REPORTS.CountAsync() +1;
                await _contx.USER_REPORTS.AddAsync(report);
                await _contx.SaveChangesAsync();
                return report.Id;
            }
            catch { return 0; }
        }

        public async Task<List<Report>> GetMostRecentReports()
        {
            try
            {
                var pointInTime = DateTime.SpecifyKind(DateTime.Now.AddDays(-1), DateTimeKind.Utc);
                return await _contx.USER_REPORTS.Where(r => r.InsertedUtc > pointInTime).ToListAsync();
            }
            catch { throw new TimeZoneNotFoundException("Error has occures"); }
        }

        public async Task<Report> GetSingleUserReportByIdAsync(long id)
        {
            try
            {
                return await _contx.USER_REPORTS.Where(r => r.Id == id)
                    .Include(r => r.User)
                    .Include(r => r.Sender)
                    .Include(r => r.Reason)
                    .SingleOrDefaultAsync();
            }
            catch { throw new ArgumentException("Id was not supplied"); }
        }

        public async Task<List<Report>> GetAllReportsOnUserAsync(long userId)
        {
            try
            {
                return await _contx.USER_REPORTS.Where(r => r.UserBaseInfoId1 == userId).ToListAsync();
            }
            catch { throw new ArgumentException("User id was not supplied"); }
        }

        public async Task<List<ReportReason>> GetReportReasonsAsync(int localisationId)
        {
            try
            {
                return await _contx.REPORT_REASONS.Where(r => r.ClassLocalisationId == localisationId).ToListAsync();
            }
            catch { throw new ArgumentException("Localisation was not supplied"); }
        }

        public async Task<long> AddUserToBlackListAsync(long userId, long bannedUserId)
        {
            long id = await _contx.USER_BLACKLISTS.Where(l => l.UserId == userId).CountAsync() +1;
            await _contx.USER_BLACKLISTS.AddAsync(new BlackList {Id = id, UserId = userId, UserId1 = bannedUserId });
            await _contx.SaveChangesAsync();
            return bannedUserId;
        }

        public async Task<long> RemoveUserFromBlackListAsync(long userId, long bannedUserId)
        {
            var bannedUser = await _contx.USER_BLACKLISTS
                .Where(u => u.UserId == userId && u.UserId1 == bannedUserId)
                .SingleOrDefaultAsync();

            if(bannedUser != null)
            {
                _contx.USER_BLACKLISTS.Remove(bannedUser);
                await _contx.SaveChangesAsync();
                return bannedUserId;
            }

            return 0;
        }

        public async Task<byte> RemoveUserAsync(long userId)
        {
            try
            {
                var user = await _contx.SYSTEM_USERS.Where(u => u.UserId == userId).SingleOrDefaultAsync();
                user.IsDeleted = true;
                _contx.SYSTEM_USERS.Update(user);
                await _contx.SaveChangesAsync();
                return 1;
            }
            catch { return 0; }
        }

        public async Task<List<Report>> GetAllUserReportsAsync(long userId)
        {
            return await _contx.USER_REPORTS.Where(u => u.UserBaseInfoId == userId)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<byte> BanUserAsync(long userId)
        {
            try
            {
                var user = await _contx.SYSTEM_USERS.Where(u => u.UserBaseInfoId == userId).SingleOrDefaultAsync();
                if (!user.IsBanned)
                {
                    user.IsBanned = true;
                    _contx.SYSTEM_USERS.Update(user);
                    await _contx.SaveChangesAsync();
                    return 1;
                }

                return 0;
            }
            catch { return 0; }
        }

        public async Task<byte> UnbanUserAsync(long userId)
        {
            try
            {
                var user = await _contx.SYSTEM_USERS.Where(u => u.UserBaseInfoId == userId).SingleOrDefaultAsync();
                if (user.IsBanned)
                {
                    user.IsBanned = false;
                    _contx.SYSTEM_USERS.Update(user);
                    await _contx.SaveChangesAsync();
                    return 1;
                }

                return 0;
            }
            catch { return 0; }
        }

        public async Task<bool> CheckUserIsBanned(long userId)
        {
            try
            {
                return (await _contx.SYSTEM_USERS.Where(u => u.UserId == userId).SingleOrDefaultAsync()).IsBanned; ;
            }
            catch { throw new NullReferenceException($"User {userId} Was not found"); }
        }

        public async Task<bool> CheckUserIsDeleted(long userId)
        {
            try
            {
                return (await _contx.SYSTEM_USERS.Where(u => u.UserId == userId).SingleOrDefaultAsync()).IsDeleted;
            }
            catch { throw new NullReferenceException($"User {userId} Was not found"); }
        }

        public async Task<string> AddAchievementProgress(long userId, long achievementId, int progress)
        {
            try
            {
                var achievement = await _contx.USER_ACHIEVEMENTS
                    .Where(a => a.UserBaseInfoId == userId && a.AchievementId == achievementId)
                    .Include(a => a.Achievement)
                    .SingleOrDefaultAsync();

                achievement.Progress += progress;
                _contx.USER_ACHIEVEMENTS.Update(achievement);
                await _contx.SaveChangesAsync();

                if (achievement.Progress >= achievement.Achievement.ConditionValue)
                    return achievement.AcquireMessage;

                return "";
            }
            catch
            { throw new NullReferenceException($"User {userId} or achievement {achievementId} was not found"); }
        }

        public async Task<string> GrantAchievementToUser(long userId, long achievementId)
        {
            try
            {
                var achievement = await _contx.USER_ACHIEVEMENTS
                    .Where(a => a.UserBaseInfoId == userId && a.AchievementId == achievementId)
                    .Include(a => a.Achievement)
                    .SingleOrDefaultAsync();
                achievement.IsAcquired = true;

                await TopUpUserWalletBalance(userId, achievement.Achievement.Value, "Achievement acquiering");

                _contx.USER_ACHIEVEMENTS.Update(achievement);
                await _contx.SaveChangesAsync();

                return achievement.AcquireMessage;
            }
            catch { throw new NullReferenceException($"User {userId} or achievement {achievementId} was not found"); }
        }

        public async Task<List<UserAchievement>> GetUserAchievements(long userId)
        {
            try
            {
                return await _contx.USER_ACHIEVEMENTS
                    .Where(a => a.UserBaseInfoId == userId)
                    .ToListAsync();
            }
            catch { throw new NullReferenceException($"User {userId} was not found"); }
        }

        public async Task<UserAchievement> GetSingleUserAchievement(long userId, long achievementId)
        {
            try
            {
                return await _contx.USER_ACHIEVEMENTS
                    .Where(a => a.UserBaseInfoId == userId && a.AchievementId == achievementId)
                    .Include(a => a.Achievement)
                    .SingleOrDefaultAsync();
            }
            catch { throw new NullReferenceException($"Achievement {achievementId} of user {userId} was not found"); }
        }

        public async Task<byte> ReRegisterUser(long userId)
        {
            var startingUser = await _contx.SYSTEM_USERS.Where(u => u.UserId == userId).SingleOrDefaultAsync();
            var startingUserBase = await _contx.SYSTEM_USERS_BASES.Where(u => u.Id == userId).SingleOrDefaultAsync();
            var startingUserData = await _contx.SYSTEM_USERS_DATA.Where(u => u.Id == userId).SingleOrDefaultAsync();
            var startingUserPrefs = await _contx.SYSTEM_USERS_PREFERENCES.Where(u => u.Id == userId).SingleOrDefaultAsync();

            var userBalances = await _contx.USER_WALLET_BALANCES.Where(u => u.UserId == userId).ToListAsync();
            if (userBalances.Count > 0 && userBalances != null)
            {
                _contx.USER_WALLET_BALANCES.RemoveRange(userBalances);
                await _contx.SaveChangesAsync();
            }

            var userLocation = await _contx.USER_LOCATIONS.Where(u => u.Id == userId).SingleOrDefaultAsync();
            if (userLocation != null)
            {
                _contx.USER_LOCATIONS.Remove(userLocation);
                await _contx.SaveChangesAsync();
            }

            var userAchievements = await _contx.USER_ACHIEVEMENTS.Where(u => u.UserBaseInfoId == userId).ToListAsync();
            if (userAchievements.Count > 0 && userAchievements != null)
            {
                _contx.USER_ACHIEVEMENTS.RemoveRange(userAchievements);
                await _contx.SaveChangesAsync();
            }


            var userPurchases = await _contx.USER_WALLET_PURCHASES.Where(u => u.UserId == userId).ToListAsync();
            if (userPurchases.Count > 0 && userPurchases != null )
            {
                _contx.USER_WALLET_PURCHASES.RemoveRange(userPurchases);
                await _contx.SaveChangesAsync();
            }

            var userVisits = await _contx.USER_VISITS.Where(u => u.UserId == userId).ToListAsync();
            if (userVisits.Count > 0 && userVisits != null)
            {
                _contx.USER_VISITS.RemoveRange(userVisits);
                await _contx.SaveChangesAsync();
            }

            var userNotifications = await _contx.USER_NOTIFICATIONS.Where(u => u.UserId == userId).ToListAsync();
            if (userNotifications.Count > 0 && userNotifications != null)
            {
                _contx.USER_NOTIFICATIONS.RemoveRange(userNotifications);
                await _contx.SaveChangesAsync();
            }

            var userNotifications1 = await _contx.USER_NOTIFICATIONS.Where(u => u.UserId1 == userId).ToListAsync();
            if (userNotifications1.Count > 0 && userNotifications1 != null)
            {
                _contx.USER_NOTIFICATIONS.RemoveRange(userNotifications1);
                await _contx.SaveChangesAsync();
            }

            var sponsorRatings = await _contx.SPONSOR_RATINGS.Where(u => u.UserId == userId).ToListAsync();
            if (sponsorRatings.Count > 0 && sponsorRatings != null)
            {
                _contx.SPONSOR_RATINGS.RemoveRange(sponsorRatings);
                await _contx.SaveChangesAsync();
            }

            var userTrustLevel = await _contx.USER_TRUST_LEVELS.Where(u => u.Id == userId).SingleOrDefaultAsync();
            if (userTrustLevel != null)
            {
                _contx.USER_TRUST_LEVELS.Remove(userTrustLevel);
                await _contx.SaveChangesAsync();
            }

            var userBase = await _contx.SYSTEM_USERS_BASES.Where(u => u.Id == userId).SingleOrDefaultAsync();
            if (userBase != null)
            {
                _contx.SYSTEM_USERS_BASES.Remove(userBase);
            }

            var userPrefs = await _contx.SYSTEM_USERS_PREFERENCES.Where(u => u.Id == userId).SingleOrDefaultAsync();
            if (userPrefs != null)
            {
                _contx.SYSTEM_USERS_PREFERENCES.Remove(userPrefs);
            }

            var userData = await _contx.SYSTEM_USERS_DATA.Where(u => u.Id == userId).SingleOrDefaultAsync();
            if (userData != null)
            {
                _contx.SYSTEM_USERS_DATA.Remove(userData);
            }

            var user = await _contx.SYSTEM_USERS.Where(u => u.UserId == userId).SingleOrDefaultAsync();
            if (user != null)
            {
                _contx.SYSTEM_USERS.Remove(user);
            }

            await _contx.SaveChangesAsync();

            if (startingUser != null)
            {
                startingUser.IsBusy = false;
                startingUser.IsBanned = false;
                startingUser.IsDeleted = false;

                await RegisterUserAsync(startingUser, startingUserBase, startingUserData, startingUserPrefs, userLocation, wasRegistered:true);
            }

            return 1;
        }

        public async Task<byte> GenerateUserAchievementList(long userId, int localisationId, bool wasRegistered=false)
        {
            try
            {
                List<UserAchievement> userAchievements;

                if (wasRegistered)
                {
                    userAchievements = await  _contx.USER_ACHIEVEMENTS
                        .Where(u => u.UserBaseInfoId == userId)
                        .ToListAsync();
                    _contx.USER_ACHIEVEMENTS.RemoveRange(userAchievements);
                }

                userAchievements = new List<UserAchievement>();
                var sysAchievements = await _contx.SYSTEM_ACHIEVEMENTS.Where(a => a.ClassLocalisationId == localisationId).ToListAsync();
                sysAchievements.ForEach(a => userAchievements.Add(new UserAchievement(a.Id, userId, a.ClassLocalisationId, a.Name, a.Description, a.Value, a.ClassLocalisationId)));

                await _contx.USER_ACHIEVEMENTS.AddRangeAsync(userAchievements);
                await _contx.SaveChangesAsync();

                return 1;
            }
            catch { throw new NullReferenceException($"User {userId} was not found"); }
        }

        public async Task<List<UserAchievement>> GetUserAchievementsAsAdmin(long userId)
        {
            try
            {
                return await _contx.USER_ACHIEVEMENTS
                    .Where(a => a.UserBaseInfoId == userId && !a.IsAcquired)
                    .ToListAsync();
            }
            catch { throw new NullReferenceException($"User {userId} was not found"); }
        }

        public async Task<bool> SetUserRtLanguagePrefs(long userId, bool shouldBeConcidered)
        {
            try
            {                
                var user = await _contx.SYSTEM_USERS.Where(u => u.UserId == userId).SingleOrDefaultAsync();
                user.ShouldConsiderLanguages = shouldBeConcidered;

                _contx.SYSTEM_USERS.Update(user);
                await _contx.SaveChangesAsync();

                return true;
            }
            catch { return false; }
        }

        public async Task<bool> CheckUsersAreCombinableRT(long user1, long user2)
        {
            var userInfo1 = await GetUserInfoAsync(user1);
            var userInfo2 = await GetUserInfoAsync(user2);

            var user1Encounters = await GetUserEncounters(user1, (int)SystemEnums.Sections.RT);

            if (user1Encounters.Where(e => e.UserId1 == user2).SingleOrDefault() == null)
            {            
                if((bool)userInfo1.ShouldConsiderLanguages && (bool)userInfo2.ShouldConsiderLanguages)
                {
                    await AddUserTrustProgressAsync(user1, 0.000005 * (double)userInfo1.BonusIndex);
                    await AddUserTrustProgressAsync(user2, 0.000005 * (double)userInfo2.BonusIndex);

                    return (await _contx.SYSTEM_USERS
                        .Where(u => u.UserId == user2)
                        .Where(u => userInfo1.UserDataInfo.UserLanguages.Any(l => u.UserPreferences.UserLanguagePreferences.Contains(l)))
                        .Where(u => u.UserDataInfo.UserLanguages.Any(l => userInfo1.UserPreferences.UserLanguagePreferences.Contains(l)))
                        .SingleOrDefaultAsync()) != null;
                }
                else if ((bool)userInfo1.ShouldConsiderLanguages)
                {
                    await AddUserTrustProgressAsync(user1, 0.000005 * (double)userInfo1.BonusIndex);
                    await AddUserTrustProgressAsync(user2, 0.000005 * (double)userInfo2.BonusIndex);

                    return (await _contx.SYSTEM_USERS
                        .Where(u => u.UserId == user2)
                        .Where(u => u.UserDataInfo.UserLanguages.Any(l => userInfo1.UserPreferences.UserLanguagePreferences.Contains(l)))
                        .SingleOrDefaultAsync()) != null;
                }
                else if ((bool)userInfo2.ShouldConsiderLanguages)
                {
                    await AddUserTrustProgressAsync(user1, 0.000005 * (double)userInfo1.BonusIndex);
                    await AddUserTrustProgressAsync(user2, 0.000005 * (double)userInfo2.BonusIndex);

                    return (await _contx.SYSTEM_USERS
                        .Where(u => u.UserId == user2)
                        .Where(u => userInfo1.UserDataInfo.UserLanguages.Any(l => u.UserPreferences.UserLanguagePreferences.Contains(l)))
                        .SingleOrDefaultAsync()) != null;
                }

                await AddUserTrustProgressAsync(user1, 0.000005 * (double)userInfo1.BonusIndex);
                await AddUserTrustProgressAsync(user2, 0.000005 * (double)userInfo2.BonusIndex);

                return true;
            }
            return false;
        }

        public async Task<Balance> GetUserWalletBalance(long userId, DateTime pointInTime)
        {
            var time = DateTime.SpecifyKind(pointInTime, DateTimeKind.Utc);
            return await _contx.USER_WALLET_BALANCES
                .Where(b => b.UserId == userId && b.PointInTime <= time)
                .OrderByDescending(b => b.PointInTime)
                .FirstOrDefaultAsync();
        }

        public async Task<int> TopUpUserWalletBalance(long userId, int points, string description = "")
        {
            var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            var userBalance = await GetUserWalletBalance(userId, time);

            if (userBalance != null)
            {            
                if (userBalance.Amount + points <= 0)
                    userBalance.Amount = 0;
                else if(userBalance.Amount + points >= int.MaxValue)
                    userBalance.Amount = int.MaxValue;
                else
                    userBalance.Amount += points;

                userBalance.Id = await _contx.USER_WALLET_BALANCES.CountAsync() +1;
                userBalance.PointInTime = time;
            }
            else
            {
                userBalance = new Balance
                {
                    Id = await _contx.USER_WALLET_BALANCES.CountAsync() +1,
                    Amount = points,
                    UserId = userId,
                    PointInTime = time
                };
            }

            await _contx.USER_WALLET_BALANCES.AddAsync(userBalance);
            await _contx.SaveChangesAsync();
            await RegisterUserWalletPurchase(userId, points, description); //Registers info about amount of points decremented / incremented

            return userBalance.Amount;
        }

        private async Task RegisterUserWalletPurchase(long userId, int points, string description)
        {
            var purchase = new Purchase
            {
                Id = await _contx.USER_WALLET_PURCHASES.CountAsync() +1,
                UserId = userId,
                PointInTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                Amount = points,
                Description = description
            };

            await _contx.USER_WALLET_PURCHASES.AddAsync(purchase);
            await _contx.SaveChangesAsync();
        }

        public async Task<bool> CheckUserHasPremium(long userId)
        {
            var timeNow = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            var user = await GetUserWithPremium(userId, timeNow);

            return user != null;
        }

        public async Task<DateTime> GetPremiumExpirationDate(long userId)
        {
            var timeNow = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            var user = await GetUserWithPremium(userId, timeNow);

            if (user != null)
            { 
                if (user.PremiumExpirationDate != null)
                    return user.PremiumExpirationDate.Value;
                return DateTime.MinValue; //TODO: Change return data in that case
            }
            else
                return DateTime.MinValue;
        }

        public async Task<DateTime> GrantPremiumToUser(long userId, int cost, int dayDuration)
        {
            var timeNow = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            var premiumFutureExpirationDate = DateTime.SpecifyKind(DateTime.Now.AddDays(dayDuration), DateTimeKind.Utc); //TODO: Possible feature: Countdown starts from 00:00 at the next day of purchase

            var user = await _contx.SYSTEM_USERS
                .Where(u => u.UserId == userId)
                .SingleOrDefaultAsync();

            user.HasPremium = true;
            user.BonusIndex = 2;
            await TopUpUserWalletBalance(userId, -cost, $"Purchase premium for {dayDuration} days");

            if (user.PremiumExpirationDate < timeNow || user.PremiumExpirationDate == null)
                user.PremiumExpirationDate = premiumFutureExpirationDate;
            else
                user.PremiumExpirationDate.Value.AddDays(dayDuration);

            _contx.Update(user);
            await _contx.SaveChangesAsync();
            return user.PremiumExpirationDate.Value;
        }

        private async Task<User> GetUserWithPremium(long userId, DateTime timeNow)
        {
            return await _contx.SYSTEM_USERS
                .Where(u => u.UserId == userId && (bool)u.HasPremium && u.PremiumExpirationDate > timeNow)
                .SingleOrDefaultAsync();
        }

        public async Task<bool> CheckBalanceIsSufficient(long userId, int cost)
        {
            cost = cost < 0 ? cost * -1 : cost; //Makes sure the cost amount wasnt minus value
            return (await GetUserWalletBalance(userId, DateTime.Now)).Amount >= cost;
        }

        public async Task<byte> UpdateUserAppLanguageAsync(long userId, int appLanguage)
        {
            try
            {
                var userData = await _contx.SYSTEM_USERS_DATA.Where(u => u.Id == userId).SingleOrDefaultAsync();

                if (userData.LanguageId != appLanguage) // Check if user changes app language to a different one
                {
                    var userAchievements = await _contx.USER_ACHIEVEMENTS.Where(a => a.UserBaseInfoId == userId).ToListAsync();
                    userAchievements.ForEach(async a =>
                    {
                        a.AchievementClassLocalisationId = appLanguage;
                        var achievement = await _contx.SYSTEM_ACHIEVEMENTS
                        .Where(achievement => achievement.Id == a.AchievementId && achievement.ClassLocalisationId == appLanguage)
                        .SingleOrDefaultAsync();

                        a.RetranslateAquireMessage(achievement, appLanguage);
                    });
                    _contx.USER_ACHIEVEMENTS.UpdateRange(userAchievements);
                    await _contx.SaveChangesAsync();

                    var userLocation = await _contx.USER_LOCATIONS.Where(l => l.Id == userId).SingleOrDefaultAsync();
                    userLocation.CountryClassLocalisationId = appLanguage;
                    //userLocation.CityCountryClassLocalisationId = appLanguage; // Uncomment when Cities will be translated on another languages

                    _contx.USER_LOCATIONS.Update(userLocation);
                    await _contx.SaveChangesAsync();

                    userData.LanguageId = appLanguage;
                    _contx.SYSTEM_USERS_DATA.Update(userData);
                    await _contx.SaveChangesAsync();
                }
                return 1;
            }
            catch { return 0; }
        }

        public async Task<byte> UpdateUserBaseAsync(UserBaseInfo user)
        {
            try
            {
                _contx.SYSTEM_USERS_BASES.Update(user);
                await _contx.SaveChangesAsync();
                return 1;
            }
            catch { return 0; }
        }

        public async Task<byte> UpdateUserDataAsync(UserDataInfo user)
        {
            try
            {
                _contx.SYSTEM_USERS_DATA.Update(user);
                await _contx.SaveChangesAsync();
                return 1;
            }
            catch { return 0; }
        }

        public async Task<byte> UpdateUserPreferencesAsync(UserPreferences user)
        {
            try
            {
                _contx.SYSTEM_USERS_PREFERENCES.Update(user);
                await _contx.SaveChangesAsync();
                return 1;
            }
            catch { return 0; }
        }

        public async Task<byte> UpdateUserLocationAsync(Location location)
        {
            try
            {
                _contx.USER_LOCATIONS.Update(location);
                await _contx.SaveChangesAsync();
                return 1;
            }
            catch { return 0; }
        }

        public async Task<bool> CheckUserIsBusy(long userId)
        {
            try
            {
                var user = await _contx.SYSTEM_USERS.Where(u => u.UserId == userId).SingleOrDefaultAsync();
                return (bool)user.IsBusy;
            }
            catch { throw new NullReferenceException($"User {userId} was not found !"); }
        }

        public async Task<bool> SwhitchUserBusyStatus(long userId)
        {
            try
            {
                var user = await _contx.SYSTEM_USERS.Where(u => u.UserId == userId).SingleOrDefaultAsync();
                if  (user != null)
                {
                    user.IsBusy = !user.IsBusy;

                    _contx.Update(user);
                    await _contx.SaveChangesAsync();

                    return (bool)user.IsBusy;
                }
                return false;
            }
            catch { throw new NullReferenceException($"User {userId} was not found !"); }
        }

        public async Task<List<UserNotification>> GetUserRequests(long userId)
        {
            try
            {
                return await _contx.USER_NOTIFICATIONS
                    .Where(r => r.UserId1 == userId)
                    .Where(r => r.SectionId == (int)SystemEnums.Sections.Familiator || r.SectionId == (int)SystemEnums.Sections.Requester)
                    .Include(r => r.Sender).ThenInclude(s => s.UserBaseInfo)
                    .ToListAsync();
            }
            catch { throw new NullReferenceException($"User {userId} was not found"); }
        }

        public async Task<UserNotification> GetUserRequest(long requestId)
        {
            try
            {
                return await _contx.USER_NOTIFICATIONS
                    .Where(r => r.Id == requestId)
                    .Where(r => r.SectionId == (int)SystemEnums.Sections.Familiator || r.SectionId == (int)SystemEnums.Sections.Requester)
                    .Include(r => r.Sender).ThenInclude(s => s.UserBaseInfo)
                    .SingleOrDefaultAsync();
            }
            catch { throw new NullReferenceException($"Request {requestId} was not found"); }
        }

        public async Task<long> RegisterUserRequest(UserNotification request)
        {
            request.Severity = (short)SystemEnums.Severities.Moderate;

            if (request.IsLikedBack)
                request.SectionId = (short)SystemEnums.Sections.Requester;
            else
                request.SectionId = (short)SystemEnums.Sections.Familiator;

            await AddUserNotificationAsync(request);

            return request.Id;
        }

        public async Task<byte> DeleteUserRequests(long userId)
        {
            try
            {
                var requests = await _contx.USER_NOTIFICATIONS
                    .Where(r => r.UserId1 == userId)
                    .Where(r => r.SectionId == (int)SystemEnums.Sections.Familiator || r.SectionId == (int)SystemEnums.Sections.Requester)
                    .ToListAsync();

                _contx.RemoveRange(requests);
                await _contx.SaveChangesAsync();

                return 1;
            }
            catch { return 0; }
        }

        public async Task<byte> DeleteUserRequest(long requestId)
        {
            try
            {
                var request = await _contx.USER_NOTIFICATIONS
                    .Where(r => r.Id == requestId)
                    .Where(r => r.SectionId == (int)SystemEnums.Sections.Familiator || r.SectionId == (int)SystemEnums.Sections.Requester)
                    .SingleOrDefaultAsync();

                _contx.Remove(request);
                await _contx.SaveChangesAsync();

                return 1;
            }
            catch { return 0; }
        }

        public async Task<bool> CheckUserHasRequests(long userId)
        {
            try
            {
                var requests = await _contx.USER_NOTIFICATIONS
                    .Where(r => r.UserId1 == userId)
                    .Where(r => r.SectionId == (int)SystemEnums.Sections.Familiator || r.SectionId == (int)SystemEnums.Sections.Requester)
                    .ToListAsync();
                
                return requests.Count > 0;
            }
            catch { throw new NullReferenceException($"User {userId} was not found !"); }
        }

        public async Task<bool> SetDebugProperties()
        {
            try
            {
                var encounters = await _contx.USER_ENCOUNTERS.ToListAsync();
                _contx.USER_ENCOUNTERS.RemoveRange(encounters);
                await _contx.SaveChangesAsync();
                var users = await _contx.SYSTEM_USERS.ToListAsync();
                users.ForEach(u => u.IsBusy = false);
                _contx.SYSTEM_USERS.UpdateRange(users);
                await _contx.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }

        public async Task<long> RegisterUserEncounter(Encounter model)
        {
            model.Id = await _contx.USER_ENCOUNTERS.CountAsync() + 1;
            model.EncounterDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            await _contx.USER_ENCOUNTERS.AddAsync(model);
            await _contx.SaveChangesAsync();

            return model.Id;
        }

        public async Task<Encounter> GetUserEncounter(long userId, long encounterId, int sectionId)
        {
            return await _contx.USER_ENCOUNTERS
                .Where(e => e.UserId == userId || e.UserId1 == userId)
                .Where(e => e.Id == encounterId)
                .Where(e => e.SectionId == sectionId)
                .SingleOrDefaultAsync();
        }

        public async Task<List<Encounter>> GetUserEncounters(long userId, int sectionId)
        {
            return await _contx.USER_ENCOUNTERS
                .Where(e => e.UserId == userId || e.UserId1 == userId)
                .Where(e => e.SectionId == sectionId)
                .ToListAsync();
        }

        public bool CheckRequestExists(long senderId, long recieverId)
        {
            return _contx.USER_NOTIFICATIONS
                .Where(r => r.UserId == senderId && r.UserId1 == recieverId)
                .Where(r => r.SectionId == (int)SystemEnums.Sections.Requester || r.SectionId == (int)SystemEnums.Sections.Familiator)
                .FirstOrDefault() != null;
        }

        public async Task<int> AddUserTrustProgressAsync(long userId, double progress)
        {

            var userBonus = await GetUserBonusIndex(userId);

            var model = await _contx.USER_TRUST_LEVELS
                .FindAsync(userId);

            if (model != null)
            {
                if((model.Progress + progress) >= model.Goal)
                {
                    model.Progress = model.Progress + (progress * userBonus) - model.Goal;
                    model.Level++;
                    model.Goal *= 2 * 1.2;
                }
                else
                {
                    model.Progress += progress;
                }

                _contx.USER_TRUST_LEVELS.Update(model);
                await _contx.SaveChangesAsync();

                return model.Level;
            }
            return -1;
        }

        public async Task<int> UpdateUserTrustLevelAsync(long userId, int level)
        {
            var model = await _contx.USER_TRUST_LEVELS
                .FindAsync(userId);

            model.Level = level;

            _contx.Update(model);
            await _contx.SaveChangesAsync();
            return model.Level;
        }

        public async Task<UserTrustLevel> GetUserTrustLevel(long userId)
        {
            return await _contx.USER_TRUST_LEVELS
                .FindAsync(userId);
        }

        private async Task<long> AddUserTrustLevel(long userId)
        {
            await _contx.USER_TRUST_LEVELS.AddAsync(UserTrustLevel.CreateDefaultTrustLevel(userId));
            await _contx.SaveChangesAsync();
            return userId;
        }

        public async Task<List<Event>> GetEventList(long userId, bool IsOnline)
        {
            var user = await GetUserInfoAsync(userId);

            var events = await _contx.SPONSOR_EVENTS
                .Where(e => e.StartDateTime > DateTime.Now.AddDays(-1)) //Check if event starts at minimum of the day before todays date
                .Where(e => e.IsOnline == IsOnline) // Todo: Apply some more filters
                .Where(e => e.MaxAge >= user.UserDataInfo.UserAge && user.UserDataInfo.UserAge >= e.MinAge)
                .Where(e => e.Languages.Any(l => user.UserDataInfo.UserLanguages.Contains((int)l)))
                .Where(e => e.CityId == user.UserDataInfo.Location.CityId)
                .ToListAsync();

            events.ForEach(e => //Check if event has country and city, if true -> check if applies to user 
            {
                if (e.CountryId != null)
                {
                    if (e.CountryId != user.UserDataInfo.Location.CountryId)
                        events.Remove(e);

                    if (e.CityId != null)
                        if (e.CityId != user.UserDataInfo.Location.CityId)
                            events.Remove(e);
                }
            });
    
            return events;
        }

        public async Task<bool> UpdateUserNickname(long userId, string nickname)
        {
            var currentUser = await _contx.SYSTEM_USERS.FindAsync(userId);

            if ((bool)currentUser.HasPremium)
            {
                currentUser.Nickname = nickname;
                _contx.SYSTEM_USERS.Update(currentUser);
                await _contx.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<string> GetUserNickname(long userId)
        {
            var currentUser = await _contx.SYSTEM_USERS.FindAsync(userId);

            if (currentUser != null)
                return currentUser.Nickname;
            return "";
        }

        public async Task<string> ClaimDailyReward(long userId)
        {
            var user = await GetUserInfoAsync(userId);

            try
            {
                var reward = await _contx.DAILY_REWARDS
                    .Where(r => r.Index == user.DailyRewardPoint)
                    .Select(r => r.PointReward)
                    .FirstOrDefaultAsync();

                await TopUpUserWalletBalance(userId, reward * (short)user.BonusIndex, "Daily reward");
                user.HadReceivedReward = true;
                user.DailyRewardPoint += 1;

                _contx.SYSTEM_USERS.Update(user);
                await _contx.SaveChangesAsync();

                return $"You have revieved {reward}p as a daily reward"; //TODO: load first part hard-coded part of a string, from localisation based on user app language
            }
            catch (NullReferenceException)
            {
                throw new NullReferenceException($"User {userId} was not found");
            }
        }

        public async Task<bool> CheckUserCanClaimReward(long userId)
        {
            var user = await GetUserInfoAsync(userId);

            if (user != null)
            {
                return !(bool)user.HadReceivedReward && user.DailyRewardPoint < 30 && user.DailyRewardPoint > 0;
            }

            return false;
        }

        public async Task<short> GetUserBonusIndex(long userId)
        {
            try
            {
                return (short)await _contx.SYSTEM_USERS
                    .Where(u => u.UserId == userId)
                    .Select(u => u.BonusIndex)
                    .FirstOrDefaultAsync();
            }
            catch(NullReferenceException)
            {
                throw new NullReferenceException($"User {userId} was not found");
            }
        }

        public async Task<InvitationCredentials> GenerateInvitationCredentialsAsync(long userId)
        {
            var invitationCreds = await GetInvitationCredentialsByUserIdAsync(userId);

            if (invitationCreds == null)
            {
                var id = Guid.NewGuid();
                var linkBase = "https://t.me/PersonalityDatingNiceBot?start=";

                invitationCreds = new InvitationCredentials
                {
                    Id = id,
                    UserId = userId,
                    Link = linkBase + id,
                    //TODO: Generate QR code via an external (or internal) service
                };

                await _contx.USER_INVITATION_CREDENTIALS.AddAsync(invitationCreds);
                await _contx.SaveChangesAsync();
            }

            return invitationCreds;
        }

        public async Task<InvitationCredentials> GetInvitationCredentialsByUserIdAsync(long userId)
        {
            return await _contx.USER_INVITATION_CREDENTIALS
                .Where(i => i.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<string> GetUserInvitationLinkAsync(long userId)
        {
            var invitation = await GetInvitationCredentialsByUserIdAsync(userId);

            if (invitation != null)
                return invitation.Link;

            return null;
        }

        public async Task<string> GetUserInvitationQRCodeAsync(long userId)
        {
            var invitation = await GetInvitationCredentialsByUserIdAsync(userId);

            if (invitation != null)
                return invitation.QRCode;

            return null;
        }

        public async Task<bool> InviteUserAsync(Guid invitationId, long userId)
        {
            var invitationCreds = await _contx.USER_INVITATION_CREDENTIALS.FindAsync(invitationId);

            if (invitationCreds != null)
            {
                var invitedUser = await GetUserInfoAsync(userId);
                var invitation = await GetInvitationAsync(userId);

                if (invitedUser != null)
                    return false;
                else if (invitation != null)
                    return false;

                invitation = new Invitation
                {
                    Id = Guid.NewGuid(),
                    InvitorCredentialsId = invitationCreds.Id,
                    InvitedUserId = userId,
                    InvitationTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                };

                await _contx.USER_INVITATIONS.AddAsync(invitation);
                await _contx.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<Invitation> GetInvitationAsync(long userId)
        {
            return await _contx.USER_INVITATIONS
                .Where(i => i.InvitedUserId == userId)
                .Include(i => i.InvitorCredentials).ThenInclude(i => i.Invitor)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> NotifyUserAboutReferentialRegistrationAsync(long userId, long invitedUserId)
        {
            if (await CheckUserIsRegistered(userId))
            {
                var invitedUsersCount = await GetInvitedUsersCountAsync(userId);
                return await AddUserNotificationAsync(new UserNotification
                    {
                        UserId1 = userId,
                        IsLikedBack = false,
                        Description = $"Hey! new user had been registered via your link. Thanks for helping us grow!\nSo far, you have invited: {invitedUsersCount} people. \nYou receive 1p for every action they are maiking ;-)",
                        SectionId = (int)SystemEnums.Sections.Registration,
                        Severity = (short)SystemEnums.Severities.Moderate
                    });
            }

            return false;
        }

        public async Task<bool> AddUserNotificationAsync(UserNotification model)
        {
            try
            {
                model.Id = await _contx.USER_NOTIFICATIONS.CountAsync() + 1;
                await _contx.USER_NOTIFICATIONS.AddAsync(model);
                await _contx.SaveChangesAsync();

                await AddUserTrustProgressAsync((long)model.UserId, 0.000002);

                return true;
            }
            catch { throw new Exception("Something went wrong when adding notification"); }
        }

        public async Task<int> GetInvitedUsersCountAsync(long userId)
        {
            return await _contx.USER_INVITATIONS
                .Where(i => i.InvitorCredentials.UserId == userId)
                .CountAsync();
        }

        public async Task<bool> CheckUserHasNotificationsAsync(long userId)
        {
            return await _contx.USER_NOTIFICATIONS
                .Where(n => n.UserId1 == userId && n.SectionId != (int)SystemEnums.Sections.Familiator && n.SectionId != (int)SystemEnums.Sections.Requester)
                .CountAsync() > 0;
        }

        public async Task<List<UserNotification>> GetUserNotifications(long userId)
        {
            return await _contx.USER_NOTIFICATIONS
                .Where(n => n.UserId1 == userId)
                .ToListAsync();
        }

        public async Task<bool> DeleteUserNotification(long notificationId)
        {
            try
            {
                var notification = await _contx.USER_NOTIFICATIONS
                    .FindAsync(notificationId);

                if (notification != null)
                {
                    _contx.USER_NOTIFICATIONS.Remove(notification);
                    await _contx.SaveChangesAsync();

                    return true;
                }
                return true;
            }
            catch { return false; }
        }
    }
}
