using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Entities.AchievementEntities;
using MyWebApi.Entities.DailyTaskEntities;
using MyWebApi.Entities.LocationEntities;
using MyWebApi.Entities.ReasonEntities;
using MyWebApi.Entities.ReportEntities;
using MyWebApi.Entities.SponsorEntities;
using MyWebApi.Entities.TestEntities;
using MyWebApi.Entities.UserActionEntities;
using MyWebApi.Entities.UserInfoEntities;
using MyWebApi.Enums;
using MyWebApi.Interfaces;
using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static MyWebApi.Enums.SystemEnums;
using static System.Net.Mime.MediaTypeNames;

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

            baseModel.UserRawDescription = baseModel.UserDescription;
            baseModel.UserDescription = baseModel.GenerateUserDescription(baseModel.UserRealName, dataModel.UserAge, country, city, baseModel.UserDescription);

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
            await TopUpUserWalletPointsBalance(model.UserId, 180, "Starting Pack"); //180 is a starting user point pack
            await AddUserTrustLevel(model.UserId);
            await AddUserTrustProgressAsync(model.UserId, 0.000012);

            if (prefsModel.ShouldUsePersonalityFunc)
            {
                var personalityStats = new UserPersonalityStats(model.UserId);
                var personalityPoints = new UserPersonalityPoints(model.UserId);
            }

            var invitation = await GetInvitationAsync(model.UserId);

            if(invitation != null)
            {
                var invitor = invitation.InvitorCredentials.Invitor;
                invitor.InvitedUsersCount++;

                var bonus = invitor.HasPremium ? 0.05 : 0;

                if (invitor.InvitedUsersCount == 3)
                {
                    invitor.InvitedUsersBonus = 0.25 + bonus;
                    await TopUpUserWalletPointsBalance(invitor.UserId, 1199, $"User {invitor.UserId} has invited 3 users");
                }
                else if (invitor.InvitedUsersCount == 7)
                {
                    invitor.InvitedUsersBonus = 0.45 + bonus;
                    await TopUpUserWalletPointsBalance(invitor.UserId, 1499, $"User {invitor.UserId} has invited 7 users");
                }
                else if (invitor.InvitedUsersBonus == 10)
                {
                    invitor.InvitedUsersBonus = 0.7 + bonus;
                    // 1499 will then turn into 1999 due to premium purchase reward
                    await TopUpUserWalletPointsBalance(invitor.UserId, 1499, $"User {invitor.UserId} has invited 10 users");
                    //TODO: apply effect later
                    await GrantPremiumToUser(model.UserId, 0, 30, (short)Currencies.Points);
                }
                else
                {
                    await TopUpUserWalletPointsBalance(invitor.UserId, (int)(200 + (200 * bonus)), $"User {model.UserId} was invited by user {invitor.UserId}");
                }

                model.BonusIndex = 1.5;
                model.ParentId = invitor.UserId;

                _contx.SYSTEM_USERS.Update(model);
                await _contx.SaveChangesAsync();

                //User is instantly liked by an invitor (Possibly let users turn of that feature)
                await RegisterUserRequest(new UserNotification { UserId = invitor.UserId, UserId1 = model.UserId, IsLikedBack = false });
                //Invitor is notified about referential registration
                await NotifyUserAboutReferentialRegistrationAsync(invitor.UserId, model.UserId);
            }

            if (await CheckUserHasTasksInSectionAsync(model.UserId, (int)Sections.Registration))
            {
                //TODO find and topup user's task progress
            }

            return model.UserId;
        }

        public async Task<List<long>> GetAllUsersAsync()
        {
            return await _contx.SYSTEM_USERS
                .Where(u => u.UserId != 1324407781 && u.UserId != 576569499) //All except Sania's accounts
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

        public async Task<List<User>> GetUsersAsync(long userId, bool turnOffPersonalityFunc = false, bool isRepeated=false, bool isFreeSearch = false)
        {
            const byte miminalProfileCount = 5;

            var currentUser = await GetUserInfoAsync(userId);
            var currentUserEncounters = await GetUserEncounters(userId, (int)SystemEnums.Sections.Familiator); //I am not sure if it is 2 or 3 section

            //If user has elected to temporarily dissable PERSONALITY functionality (Change shold NOT be changed in th DB) 
            if (turnOffPersonalityFunc)
                currentUser.UserPreferences.ShouldUsePersonalityFunc = false;

            var data = await _contx.SYSTEM_USERS
                .Where(u => u.UserId != currentUser.UserId)
                .Where(u => u.UserDataInfo.ReasonId == currentUser.UserDataInfo.ReasonId)
                .Where(u => u.UserPreferences.CommunicationPrefs == currentUser.UserPreferences.CommunicationPrefs)
                .Where(u => u.UserPreferences.AgePrefs.Contains(currentUser.UserDataInfo.UserAge))
                .Where(u => u.UserPreferences.UserLocationPreferences.Contains(currentUser.UserDataInfo.Location.CountryId))
                //Check if users gender preferences correspond to current user gender prefs or are equal to 'Does not matter'
                .Where(u => u.UserPreferences.UserGenderPrefs == currentUser.UserDataInfo.UserGender || u.UserPreferences.UserGenderPrefs == 2)
                .Where(u => u.UserDataInfo.UserLanguages.Any(l => currentUser.UserPreferences.UserLanguagePreferences.Contains(l)))
                .Where(u => currentUser.UserPreferences.AgePrefs.Contains(u.UserDataInfo.UserAge))
                .Where(u => currentUser.UserPreferences.UserLocationPreferences.Contains(u.UserDataInfo.Location.CountryId))
                .Where(u => currentUser.UserDataInfo.UserLanguages.Any(l => u.UserPreferences.UserLanguagePreferences.Contains(l)))
                .Include(u => u.UserBaseInfo)
                .Include(u => u.UserDataInfo)
                .ThenInclude(u => u.Location)
                .Include(u => u.UserPreferences)
                .Include(u => u.UserBlackList)
                .ToListAsync();

            //Check if users had encountered one another
            data = data.Where(u => !currentUser.CheckIfHasEncountered(currentUserEncounters, u.UserId)).ToList();

            //Check if users are in each others black lists
            data = data.Where(u => u.UserBlackList.Where(u => u.BannedUserId == userId).SingleOrDefault() == null).ToList();
            data = data.Where(u => currentUser.UserBlackList.Where(l => l.BannedUserId == u.UserId).SingleOrDefault() == null).ToList();

            //Check if request already exists
            data = data.Where(u => !CheckRequestExists(userId, u.UserId)).ToList();

            //Check if current user had already recieved request from user
            data = data.Where(u => !CheckRequestExists(u.UserId, userId)).ToList();

            //If user does NOT have gender prederences
            if (currentUser.UserPreferences.UserGenderPrefs != 2)
            {
                data = data.Where(u => u.UserDataInfo.UserGender == currentUser.UserPreferences.UserGenderPrefs)
                .Where(u => currentUser.UserPreferences.UserGenderPrefs == u.UserDataInfo.UserGender)
                .ToList();
            }

            //If user wants to find only people who are free today
            if (isFreeSearch)
                data = data.Where(u => u.IsFree).ToList();


            //If user uses PERSONALITY functionality and free search is disabled
            if (currentUser.UserPreferences.ShouldUsePersonalityFunc && !isFreeSearch)
            {
                //TODO: Change it for users with premium ?
                var deviation = 0.15;

                //TODO: do not apply if users parameter percentage will be negative as the result
                var minDeviation = 0.05;

                if (isRepeated)
                {
                    deviation *= 1.5;
                    minDeviation *= 3.2;
                }

                var userPoints = await _contx.USER_PERSONALITY_POINTS.Where(p => p.UserId == currentUser.UserId)
                .SingleOrDefaultAsync();

                var userStats = await _contx.USER_PERSONALITY_STATS.Where(s => s.UserId == currentUser.UserId)
                .SingleOrDefaultAsync();

                for (int i = 0; i < data.Count; i++)
                {
                    var u = data[i];

                    //Check if user uses personality functionality and remove him from the list if he does not
                    if (!u.UserPreferences.ShouldUsePersonalityFunc)
                        data.Remove(u);

                    var user2Points = await _contx.USER_PERSONALITY_POINTS.Where(p => p.UserId == u.UserId)
                    .SingleOrDefaultAsync();

                    var user2Stats = await _contx.USER_PERSONALITY_STATS.Where(s => s.UserId == u.UserId)
                    .SingleOrDefaultAsync();

                    //TODO: create its own deviation variable depending on the number of personalities (It is likely to be grater than the nornal one)
                    var personalitySim = await CalculateSimilarityAsync(userStats.Personality, user2Stats.Personality);
                    //Negative conditions are applied, cuz this is an exclussive condition
                    if (personalitySim >= userPoints.PersonalityPercentage + deviation || personalitySim <= userPoints.PersonalityPercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }

                    if (personalitySim >= user2Points.PersonalityPercentage + deviation || personalitySim <= user2Points.PersonalityPercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue; 
                    }

                    var emIntellectSim = await CalculateSimilarityAsync(userStats.EmotionalIntellect, user2Stats.EmotionalIntellect);
                    if (emIntellectSim >= userPoints.EmotionalIntellectPercentage + deviation || emIntellectSim <= userPoints.EmotionalIntellectPercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }

                    if (emIntellectSim >= user2Points.EmotionalIntellectPercentage + deviation || emIntellectSim <= user2Points.EmotionalIntellectPercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }

                    var reliabilitySim = await CalculateSimilarityAsync(userStats.Reliability, user2Stats.Reliability);
                    if (reliabilitySim >= userPoints.ReliabilityPercentage + deviation || reliabilitySim <= userPoints.ReliabilityPercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }

                    if (reliabilitySim >= user2Points.ReliabilityPercentage + deviation || reliabilitySim <= user2Points.ReliabilityPercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }

                    var compassionSim = await CalculateSimilarityAsync(userStats.Reliability, user2Stats.Reliability);
                    if (compassionSim >= userPoints.CompassionPercentage + deviation || compassionSim <= userPoints.CompassionPercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }

                    if (compassionSim >= user2Points.CompassionPercentage + deviation || compassionSim <= user2Points.CompassionPercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }

                    var openMindSim = await CalculateSimilarityAsync(userStats.OpenMindedness, user2Stats.OpenMindedness);
                    if (openMindSim >= userPoints.OpenMindednessPercentage + deviation || openMindSim <= userPoints.OpenMindednessPercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }

                    if (openMindSim >= user2Points.OpenMindednessPercentage + deviation || openMindSim <= user2Points.OpenMindednessPercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }

                    var agreeablenessSim = await CalculateSimilarityAsync(userStats.Agreeableness, user2Stats.Agreeableness);
                    if (agreeablenessSim >= userPoints.AgreeablenessPercentage + deviation || agreeablenessSim <= userPoints.AgreeablenessPercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }

                    if (agreeablenessSim >= user2Points.AgreeablenessPercentage + deviation || agreeablenessSim <= user2Points.AgreeablenessPercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }

                    var selfAwerenessSim = await CalculateSimilarityAsync(userStats.SelfAwareness, user2Stats.SelfAwareness);
                    if (selfAwerenessSim >= userPoints.AgreeablenessPercentage + deviation || selfAwerenessSim <= userPoints.AgreeablenessPercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }

                    if (selfAwerenessSim >= user2Points.AgreeablenessPercentage + deviation || selfAwerenessSim <= user2Points.AgreeablenessPercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }

                    var levelOfSense = await CalculateSimilarityAsync(userStats.LevelOfSense, user2Stats.LevelOfSense);
                    if (levelOfSense >= userPoints.LevelOfSensePercentage + deviation || levelOfSense <= userPoints.LevelOfSensePercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }

                    if (levelOfSense >= user2Points.LevelOfSensePercentage + deviation || levelOfSense <= user2Points.LevelOfSensePercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }

                    var intellectSim = await CalculateSimilarityAsync(userStats.Intellect, user2Points.Intellect);
                    if (intellectSim >= userPoints.IntellectPercentage + deviation || intellectSim <= userPoints.IntellectPercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }

                    if (intellectSim >= user2Points.IntellectPercentage + deviation || intellectSim <= user2Points.IntellectPercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }

                    var natureSim = await CalculateSimilarityAsync(userStats.Nature, user2Stats.Nature);
                    if (natureSim >= userPoints.Nature + deviation || natureSim <= userPoints.Nature - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }

                    if (natureSim >= user2Points.NaturePercentage + deviation || natureSim <= user2Points.NaturePercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }

                    var creativitySim = await CalculateSimilarityAsync(userStats.Creativity, user2Stats.Creativity);
                    if (creativitySim >= userPoints.CreativityPercentage + deviation || creativitySim <= userPoints.CreativityPercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }

                    if (creativitySim >= user2Points.CreativityPercentage + deviation || creativitySim <= user2Points.CreativityPercentage - minDeviation)
                    {
                        data.Remove(u);
                        continue;
                    }
                }
            }
            else
            {
                if(!isFreeSearch)
                {
                    //Remove users using PERSONALITY fucntionality
                    data.AsParallel().ForAll(u =>
                    {
                        if (u.UserPreferences.ShouldUsePersonalityFunc)
                            data.Remove(u);
                    });
                }
            }

            //Check if method wasnt already repeated
            if (!isRepeated)
            {
                //Check if users count is less than the limit
                if (data.Count <= miminalProfileCount)
                    data = await GetUsersAsync(userId, turnOffPersonalityFunc:turnOffPersonalityFunc, isRepeated: true, isFreeSearch:isFreeSearch);

                //Add user trust exp only if method was not repeated
                await AddUserTrustProgressAsync(userId, 0.000003);

                //Return users PERSONALITY usage property to normal (In case it was temporarily turned off)
                if (turnOffPersonalityFunc)
                {
                    currentUser.UserPreferences.ShouldUsePersonalityFunc = true;
                    await _contx.SaveChangesAsync();
                }

                //Todo: Probably remove AsParallel query 
                data.AsParallel().ForAll(u =>
                {
                    if (u.Nickname != "" && (bool)u.HasPremium)
                        u.UserBaseInfo.UserDescription = $"{u.Nickname}\n\n{u.UserBaseInfo.UserDescription}";
                });

                //Order user list randomly 
                data = data.OrderBy(u => new Random().Next())
                    .ToList();

                data.OrderByDescending(u => u.UserDataInfo.Location.CityId == currentUser.UserDataInfo.Location.CityId)
                    .ToList();
            }

            await _contx.SaveChangesAsync();
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

            if (await CheckUserHasTasksInSectionAsync(report.UserBaseInfoId, (int)Sections.Reporter))
            {
                //TODO find and topup user's task progress
            }

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

                if (await CheckUserHasTasksInSectionAsync(report.UserBaseInfoId, (int)Sections.Reporter))
                {
                    //TODO find and topup user's task progress
                }

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
            catch { throw new TimeZoneNotFoundException("Error has occured"); }
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

        public async Task<bool> AddUserToBlackListAsync(long userId, long bannedUserId)
        {
            try
            {
                long id = await _contx.USER_BLACKLISTS.Where(l => l.UserId == userId).CountAsync() +1;
                await _contx.USER_BLACKLISTS.AddAsync(new BlackList {Id = id, UserId = userId, BannedUserId = bannedUserId });
                await _contx.SaveChangesAsync();
                return true;
            }
            catch {
                return false; 
            }
        }

        public async Task<bool> RemoveUserFromBlackListAsync(long userId, long bannedUserId)
        {
            var bannedUser = await _contx.USER_BLACKLISTS
                .Where(u => u.UserId == userId && u.BannedUserId == bannedUserId)
                .SingleOrDefaultAsync();

            if(bannedUser != null)
            {
                _contx.USER_BLACKLISTS.Remove(bannedUser);
                await _contx.SaveChangesAsync();
                return true;
            }

            return false;
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
                    .Where(a => a.UserBaseInfoId == userId && a.AchievementId == achievementId && !a.IsAcquired)
                    .Include(a => a.Achievement)
                    .SingleOrDefaultAsync();

                if (achievement == null)
                    throw new Exception($"User have already acquired achievement #{achievementId} or it does not exist");

                achievement.IsAcquired = true;

                await TopUpUserWalletPointsBalance(userId, achievement.Achievement.Value, "Achievement acquiering");

                await AddUserNotificationAsync(new UserNotification
                {
                    UserId1 = userId,
                    IsLikedBack = false,
                    SectionId = achievement.Achievement.SectionId,
                    Severity = (byte)Severities.Minor,
                    Description = achievement.AcquireMessage
                });


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

            var user1Encounters = await GetUserEncounters(user1, (int)Sections.RT);

            //Check if users are not in each others blacklists
            var usersAreNotInBlackList =
                (await _contx.USER_BLACKLISTS.Where(l => l.UserId == user1 && l.BannedUserId == user2).FirstOrDefaultAsync()) == null
                &&
                (await _contx.USER_BLACKLISTS.Where(l => l.UserId == user2 && l.BannedUserId == user1).FirstOrDefaultAsync()) == null;

            if (usersAreNotInBlackList)
            {
                //Check if user1 has encountered user2
                //In that case, checking 1 encounter is enough
                if (user1Encounters.Where(e => e.EncounteredUserId == user2).SingleOrDefault() == null)
                {   
                    //If both consider having the same languages
                    if((bool)userInfo1.ShouldConsiderLanguages && (bool)userInfo2.ShouldConsiderLanguages)
                    {
                        await AddUserTrustProgressAsync(user1, 0.000005 * (double)userInfo1.BonusIndex);
                        await AddUserTrustProgressAsync(user2, 0.000005 * (double)userInfo2.BonusIndex);
                        
                        var result = (await _contx.SYSTEM_USERS
                            .Where(u => u.UserId == user2)
                            .Where(u => userInfo1.UserDataInfo.UserLanguages.Any(l => u.UserPreferences.UserLanguagePreferences.Contains(l)))
                            .Where(u => u.UserDataInfo.UserLanguages.Any(l => userInfo1.UserPreferences.UserLanguagePreferences.Contains(l)))
                            .SingleOrDefaultAsync()) != null;

                        if (result)
                        {
                            await RegisterUserEncounter(new Encounter { UserId = user1, EncounteredUserId = user2 });
                            await RegisterUserEncounter(new Encounter { UserId = user2, EncounteredUserId = user1 });
                        }

                        return result;
                    }
                    //If user1 considers having the same languages
                    else if ((bool)userInfo1.ShouldConsiderLanguages)
                    {
                        await AddUserTrustProgressAsync(user1, 0.000005 * (double)userInfo1.BonusIndex);
                        await AddUserTrustProgressAsync(user2, 0.000005 * (double)userInfo2.BonusIndex);

                        var result = (await _contx.SYSTEM_USERS
                            .Where(u => u.UserId == user2)
                            .Where(u => u.UserDataInfo.UserLanguages.Any(l => userInfo1.UserPreferences.UserLanguagePreferences.Contains(l)))
                            .SingleOrDefaultAsync()) != null;

                        if (result)
                        {
                            await RegisterUserEncounter(new Encounter { UserId = user1, EncounteredUserId = user2 });
                            await RegisterUserEncounter(new Encounter { UserId = user2, EncounteredUserId = user1 });
                        }

                        return result;
                    }
                    //If user2 considers having the same languages
                    else if ((bool)userInfo2.ShouldConsiderLanguages)
                    {
                        await AddUserTrustProgressAsync(user1, 0.000005 * (double)userInfo1.BonusIndex);
                        await AddUserTrustProgressAsync(user2, 0.000005 * (double)userInfo2.BonusIndex);

                        var result = (await _contx.SYSTEM_USERS
                            .Where(u => u.UserId == user2)
                            .Where(u => userInfo1.UserDataInfo.UserLanguages.Any(l => u.UserPreferences.UserLanguagePreferences.Contains(l)))
                            .SingleOrDefaultAsync()) != null; ;

                        if (result)
                        {
                            await RegisterUserEncounter(new Encounter { UserId = user1, EncounteredUserId = user2 });
                            await RegisterUserEncounter(new Encounter { UserId = user2, EncounteredUserId = user1 });
                        }

                        return result;
                    }

                    await AddUserTrustProgressAsync(user1, 0.000005 * (double)userInfo1.BonusIndex);
                    await AddUserTrustProgressAsync(user2, 0.000005 * (double)userInfo2.BonusIndex);

                    if (await CheckUserHasTasksInSectionAsync(user1, (int)Sections.RT))
                    {
                        //TODO find and topup user's task progress
                    }

                    if (await CheckUserHasTasksInSectionAsync(user2, (int)Sections.RT))
                    {
                        //TODO find and topup user's task progress
                    }

                    await RegisterUserEncounter(new Encounter { UserId = user1, EncounteredUserId = user2 });
                    await RegisterUserEncounter(new Encounter { UserId = user2, EncounteredUserId = user1 });

                    //If neither considers having the same languages
                    return true;
                }
                return false;
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

        public async Task<int> TopUpUserWalletPointsBalance(long userId, int points, string description = "")
        {
            var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            var userBalance = await GetUserWalletBalance(userId, time);

            if (userBalance != null)
            {            
                if (userBalance.Points + points <= 0)
                    userBalance.Points = 0;
                else if(userBalance.Points + points >= int.MaxValue)
                    userBalance.Points = int.MaxValue;
                else
                    userBalance.Points += points;

                userBalance.PointInTime = time;

                _contx.USER_WALLET_BALANCES.Update(userBalance);
                await _contx.SaveChangesAsync();
            }
            else
            {
                userBalance = new Balance
                {
                    Id = Guid.NewGuid(),
                    Points = points,
                    PersonalityPoints = 15,
                    UserId = userId,
                    PointInTime = time
                };

                await _contx.USER_WALLET_BALANCES.AddAsync(userBalance);
                await _contx.SaveChangesAsync();
            }

            var userParentId = (await _contx.SYSTEM_USERS.FindAsync(userId)).ParentId;

            if (points > 0 && userParentId != null && userParentId > 0)
            {
                var parent = await GetUserInfoAsync((long)userParentId);

                if (parent != null)
                    await TopUpUserWalletPointsBalance((long)userParentId, (int)(points * parent.InvitedUsersBonus), $"Referential reward for users {userId} action");
            }

            await _contx.SaveChangesAsync();
            await RegisterUserWalletPurchaseInPoints(userId, points, description); //Registers info about amount of points decremented / incremented

            return userBalance.Points;
        }

        public async Task<int> TopUpUserWalletPPBalance(long userId, int points, string description = "")
        {
            var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            var userBalance = await GetUserWalletBalance(userId, time);

            if (userBalance != null)
            {
                if (userBalance.PersonalityPoints + points <= 0)
                    userBalance.PersonalityPoints = 0;
                else if (userBalance.PersonalityPoints + points >= int.MaxValue)
                    userBalance.PersonalityPoints = int.MaxValue;
                else
                    userBalance.PersonalityPoints += points;

                userBalance.PointInTime = time;

                _contx.USER_WALLET_BALANCES.Update(userBalance);
                await _contx.SaveChangesAsync();
            }
            else
            {
                userBalance = new Balance
                {
                    Id = Guid.NewGuid(),
                    Points = points,
                    PersonalityPoints = 15,
                    UserId = userId,
                    PointInTime = time
                };

                await _contx.USER_WALLET_BALANCES.AddAsync(userBalance);
                await _contx.SaveChangesAsync();
            }

            var userParentId = (await _contx.SYSTEM_USERS.FindAsync(userId)).ParentId;

            if (userParentId != null && userParentId > 0)
                await TopUpUserWalletPointsBalance((long)userParentId, 1, $"Referential reward for user's {userParentId} action");

            await _contx.SaveChangesAsync();
            await RegisterUserWalletPurchaseInPP(userId, points, description); //Registers info about amount of points decremented / incremented

            return userBalance.PersonalityPoints;
        }

        private async Task<bool> RegisterUserWalletPurchaseInPoints(long userId, int points, string description)
        {
            return await RegisterUserWalletPurchase(userId, points, description, (short)Currencies.Points);
        }

        private async Task<bool> RegisterUserWalletPurchaseInPP(long userId, int points, string description)
        {
            return await RegisterUserWalletPurchase(userId, points, description, (short)Currencies.PersonalityPoints);
        }

        private async Task<bool> RegisterUserWalletPurchaseInRealMoney(long userId, int points, string description)
        {
            return await RegisterUserWalletPurchase(userId, points, description, (short)Currencies.RealMoney);
        }

        private async Task<bool> RegisterUserWalletPurchase(long userId, int points, string description, short currency)
        {
            try
            {
                var purchase = new Purchase
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    PointInTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                    Amount = points,
                    Description = description,
                    Currency = currency
                };

                await _contx.USER_WALLET_PURCHASES.AddAsync(purchase);
                await _contx.SaveChangesAsync();

                return true;
            }
            catch { return false; }
        }

        public async Task<bool> CheckUserHasPremium(long userId)
        {
            var timeNow = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            var user = await _contx.SYSTEM_USERS.Where(u => u.UserId == userId).SingleOrDefaultAsync();

            if ((bool)user.HasPremium && user.PremiumExpirationDate > timeNow)
                user.HasPremium = false;

            //TODO: Notify user that his premium access has expired

            return user.HasPremium;
        }

        public async Task<DateTime> GetPremiumExpirationDate(long userId)
        {
            var timeNow = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            var user = await GetUserWithPremium(userId, timeNow);

            if (user != null)
            { 
                if (user.PremiumExpirationDate != null)
                    return user.PremiumExpirationDate.Value;
                return (DateTime)user.PremiumExpirationDate;
            }
            else
                return DateTime.MinValue;
        }

        public async Task<DateTime> GrantPremiumToUser(long userId, int cost, int dayDuration, short currency)
        {
            var timeNow = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            var premiumFutureExpirationDate = DateTime.SpecifyKind(DateTime.Now.AddDays(dayDuration), DateTimeKind.Utc); //TODO: Possible feature: Countdown starts from 00:00 at the next day of purchase

            var user = await _contx.SYSTEM_USERS
                .Where(u => u.UserId == userId)
                .SingleOrDefaultAsync();

            user.HasPremium = true;
            user.BonusIndex = 2;

            //If transaction was made for points
            if (currency == (short)Currencies.Points)
                await TopUpUserWalletPointsBalance(userId, -cost, $"Purchase premium for {dayDuration} days");
            //If transaction was made for real money
            else if (currency == (short)Currencies.RealMoney)
                await RegisterUserWalletPurchaseInRealMoney(userId, cost, $"Purchase premium for {dayDuration} days");

            //Reward for premium purchase
            await TopUpUserWalletPointsBalance(userId, 500);

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
            return (await GetUserWalletBalance(userId, DateTime.Now)).Points >= cost;
        }

        public async Task<byte> UpdateUserAppLanguageAsync(long userId, int appLanguage)
        {
            try
            {
                var userData = await _contx.SYSTEM_USERS_DATA.Where(u => u.Id == userId).SingleOrDefaultAsync();

                if (userData != null)
                {
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
                return 0;
            }
            catch { return 0; }
        }

        public async Task<byte> UpdateUserBaseAsync(UserBaseInfo user)
        {
            try
            {
                var model = await _contx.SYSTEM_USERS_BASES.FindAsync(user.Id);
                var data = await _contx.SYSTEM_USERS_DATA.FindAsync(user.Id);
                var location = await _contx.USER_LOCATIONS.Where(l => l.Id == user.Id)
                    .SingleOrDefaultAsync();

                var country = await _contx.COUNTRIES.Where(c => c.ClassLocalisationId == location.CountryClassLocalisationId && c.Id == location.CountryId)
                    .SingleOrDefaultAsync();
                var city = await _contx.CITIES.Where(c => c.CountryClassLocalisationId == location.CityCountryClassLocalisationId && c.Id == location.CityId)
                    .SingleOrDefaultAsync();

                model.UserRawDescription = user.UserRawDescription;
                model.UserDescription = user.GenerateUserDescription(user.UserName, data.UserAge, country.CountryName, city.CityName, user.UserRawDescription);
                model.UserPhoto = user.UserPhoto;
                model.UserName = user.UserName;
                model.UserRealName = user.UserRealName;

                _contx.SYSTEM_USERS_BASES.Update(model);
                await _contx.SaveChangesAsync();
                return 1;
            }
            catch { return 0; }
        }

        public async Task<byte> UpdateUserDataAsync(UserDataInfo user)
        {
            try
            {
                var data = await _contx.SYSTEM_USERS_DATA.FindAsync(user.Id);

                data.UserAge = user.UserAge;
                data.UserGender = user.UserGender;
                data.UserLanguages = user.UserLanguages;
                data.ReasonId = user.ReasonId;

                _contx.SYSTEM_USERS_DATA.Update(data);
                await _contx.SaveChangesAsync();
                return 1;
            }
            catch { return 0; }
        }

        public async Task<byte> UpdateUserPreferencesAsync(UserPreferences user)
        {
            try
            {
                var prefs = await _contx.SYSTEM_USERS_PREFERENCES.FindAsync(user.Id);

                prefs.AgePrefs = user.AgePrefs;
                prefs.UserLanguagePreferences = user.UserLanguagePreferences;
                prefs.UserLocationPreferences = user.UserLocationPreferences;
                prefs.CommunicationPrefs = user.CommunicationPrefs;
                prefs.UserGenderPrefs = user.UserGenderPrefs;

                _contx.SYSTEM_USERS_PREFERENCES.Update(prefs);
                await _contx.SaveChangesAsync();
                return 1;
            }
            catch { return 0; }
        }

        public async Task<byte> UpdateUserLocationAsync(Location location)
        {
            try
            {
                var model = await _contx.USER_LOCATIONS.FindAsync(location.Id);

                model.CityId = location.CityId;
                model.CityCountryClassLocalisationId = location.CityCountryClassLocalisationId;
                model.CountryId = location.CountryId;
                model.CountryClassLocalisationId = location.CountryClassLocalisationId;

                _contx.USER_LOCATIONS.Update(model);
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


                    //Possible task -> visit any section x times
                    if (await CheckUserHasTasksInSectionAsync(userId, (int)Sections.Neutral))
                    {
                        //TODO find and topup user's task progress
                    }

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

            if (model.SectionId == (int)Sections.Familiator || model.SectionId == (int)Sections.Requester)
            {
                var user = await _contx.SYSTEM_USERS.FindAsync(model.UserId);
                user.ProfileViewsCount++;

                if (user.ProfileViewsCount == 15)
                    await TopUpUserWalletPointsBalance(user.UserId, 9, "User has viewed 15 profiles");
                else if (user.ProfileViewsCount == 30)
                    await TopUpUserWalletPointsBalance(user.UserId, 15, "User has viewed 30 profiles");
                else if (user.ProfileViewsCount == 50)
                    await TopUpUserWalletPointsBalance(user.UserId, 22, "User has viewed 50 profiles");
            }

            await _contx.USER_ENCOUNTERS.AddAsync(model);
            await _contx.SaveChangesAsync();

            return model.Id;
        }

        public async Task<Encounter> GetUserEncounter(long encounterId)
        {
            return await _contx.USER_ENCOUNTERS.FindAsync(encounterId);
        }

        public async Task<List<Encounter>> GetUserEncounters(long userId, int sectionId)
        {
            var encounters = await _contx.USER_ENCOUNTERS
                .Where(e => e.UserId == userId)
                .Where(e => e.SectionId == sectionId)
                .Include(e => e.EncounteredUser)
                .ToListAsync();

            return encounters != null ? encounters : new List<Encounter>();
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


                //Possible task -> increse your trust level progress on x points
                if (await CheckUserHasTasksInSectionAsync(userId, (int)Sections.Neutral))
                {
                    //TODO find and topup user's task progress
                }

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
                //Possible task -> Update your nickname
                //Only for premium users
                if (await CheckUserHasTasksInSectionAsync(userId, (int)Sections.Registration))
                {
                    //TODO find and topup user's task progress
                }

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

                await TopUpUserWalletPointsBalance(userId, reward * (short)user.BonusIndex, "Daily reward");
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
                return !user.HadReceivedReward && user.DailyRewardPoint < 30 && user.DailyRewardPoint > 0;
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
                };

                await _contx.USER_INVITATION_CREDENTIALS.AddAsync(invitationCreds);
                await _contx.SaveChangesAsync();
            }

            return invitationCreds;
        }

        //Generate QR code on user request 
        public async Task<string> GetQRCode(string link, long userId)
        {
            string data;

            var creds = await _contx.USER_INVITATION_CREDENTIALS.Where(c => c.UserId == userId)
                .SingleOrDefaultAsync();

            if (creds != null)
            {
                if (creds.QRCode != null)
                    return creds.QRCode;

                link = link.Replace("%2F", "/");

                var generator = new QRCodeGenerator();
                var d = generator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
                var code = new PngByteQRCode(d).GetGraphic(15);

                data = Convert.ToBase64String(code);

                creds.QRCode = data;
                await _contx.SaveChangesAsync();

                return data;
            }

            if(await CheckUserIsRegistered(userId))
            {
                //If credentials do not exist. Create them and try generationg QrCode again
                await GenerateInvitationCredentialsAsync(userId);
                return await GetQRCode(link, userId);
            }
            return "";
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

                //Possible task -> invite someone
                if (await CheckUserHasTasksInSectionAsync(userId, (int)Sections.Registration))
                {
                    //TODO find and topup user's task progress
                }

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
                        SectionId = (int)Sections.Registration,
                        Severity = (short)Severities.Moderate
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

                if (model.UserId != null)
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

        public async Task<bool> DeleteUserNotification(long userId, long notificationId)
        {
            try
            {
                var notification = await GetUserNotificationAsync(userId, notificationId);

                if (notification != null)
                {
                    return await DeleteUserNotification(notification);
                }
                return false;
            }
            catch { return false; }
        }

        public async Task<bool> DeleteUserNotification(UserNotification notification)
        {
            try
            {
                if (notification != null)
                {
                    _contx.USER_NOTIFICATIONS.Remove(notification);
                    await _contx.SaveChangesAsync();

                    return true;
                }
                return false;
            }
            catch { return false; }
        }

        public async Task<List<UserAchievement>> GetRandomAchievements(long userId)
        {
            var achievents = await _contx.USER_ACHIEVEMENTS
                .Where(a => a.UserBaseInfoId == userId)
                .Where(a => !a.IsAcquired)
                .Include(a => a.Achievement)
                .ToListAsync();

            //Shuffle the achievement list
            achievents = achievents.OrderBy(a => new Random().Next()).ToList();

            //Normal scenario. User still has more than 3 achievements to claim
            if (achievents.Count > 3)
                return achievents.Take(3).ToList();

            if (achievents.Count < 3)
                return achievents.Take(achievents.Count).ToList();

            //If there is no achievements left to claim
            return achievents;
        }

        public async Task<double> CalculateSimilarityAsync(double param1, double param2)
        {
            double difference = 0;
            double x = 0;
            double c = 0;

            //Remove negative values. We are all possitive here ;)
            if (param1 < 0)
                param1 *= -1;

            if (param2 < 0)
                param2 *= -1;

            await Task.Run(() =>
            {
                if (param1 > param2)
                {
                    difference = param1 - param2;
                    x = (difference * 100) / param1;
                    c = 0 + (x / 100);
                }
                else if (param2 > param1)
                {
                    difference = param2 - param1;
                    x = (difference * 100) / param2;
                    c = 0 + (x / 100);
                }
                else if (param1 == param2)
                    c = 0.00000001; //Similarity percentage will never be equal to 100% !
            });

            return c;
        }

        public async Task<DailyTask> GetDailyTaskByIdAsync(long id)
        {
            return await _contx.DAILY_TASKS
                .Where(t => t.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<UserDailyTask> GetUserDailyTaskByIdAsync(long userId, long taskId)
        {
            return await _contx.USER_DAILY_TASKS
                .Where(t => t.UserId == userId && t.DailyTaskId == taskId)
                .Include(t => t.DailyTask)
                .SingleOrDefaultAsync();
        }

        public async Task<int> UpdateUserDailyTaskProgressAsync(long userId, long id, int progress)
        {
            var task = await GetUserDailyTaskByIdAsync(userId, id);

            //Do nothing if task does not exists or it was acquired
            if (task == null || task.IsAcquired)
                return 0;

            //Remove negative values
            if (progress < 0)
                progress *= -1;

            if (task.Progress + progress >= task.DailyTask.Condition)
            {
                return await GiveDailyTaskRewardToUserAsync(userId, task);
            }

            task.Progress += progress;

            _contx.USER_DAILY_TASKS.Update(task);
            await _contx.SaveChangesAsync();

            return task.Progress;
        }

        public async Task<int> GiveDailyTaskRewardToUserAsync(long userId, long taskId)
        {
            var task = await GetUserDailyTaskByIdAsync(userId, taskId);

            if (task == null)
                return 0;

            return await GiveDailyTaskRewardToUserAsync(userId, task);
        }

        public async Task<int> GiveDailyTaskRewardToUserAsync(long userId, UserDailyTask task)
        {
            task.Progress = task.DailyTask.Condition;
            task.IsAcquired = true;
            await AddUserNotificationAsync(new UserNotification
            {
                UserId1 = userId,
                IsLikedBack = false,
                Severity = (short)Severities.Moderate,
                Description = task.AcquireMessage,
                SectionId = task.DailyTask.SectionId
            });

            if (task.DailyTask.RewardCurrency == (byte)Currencies.Points)
                await TopUpUserWalletPointsBalance(userId, task.DailyTask.Reward, description:"Daily task acquiering");
            else if (task.DailyTask.RewardCurrency == (byte)Currencies.PersonalityPoints) { }
            //TODO: Topup user Personality points wallet balace
            else if (task.DailyTask.RewardCurrency == (byte)Currencies.Premium) { }
            //TODO: Add premium to user

            _contx.USER_DAILY_TASKS.Update(task);
            await _contx.SaveChangesAsync();

            return 1;
        }

        public async Task<bool> CheckUserHasTasksInSectionAsync(long userId, int sectionId)
        {
            var count = await _contx.USER_DAILY_TASKS
                .Where(t => t.UserId == userId && t.DailyTask.SectionId == sectionId && !t.IsAcquired)
                .CountAsync();

            return count > 0;
        }

        public async Task<byte> GenerateUserDailyTaskListAsync(long userId)
        {
            var rand = new Random();

            var user = await GetUserInfoAsync(userId);
            var taskCount = 2;

            //Remove previous users tasks 
            var prevTasks = await _contx.USER_DAILY_TASKS
                .Where(t => t.UserId == userId)
                .ToListAsync();

            if (prevTasks.Count > 0)
            {
                _contx.USER_DAILY_TASKS.RemoveRange(prevTasks);
                await _contx.SaveChangesAsync();
            }

            if (user == null)
                return 0;

            var userDailyTasks = new List<UserDailyTask>();

            //Take common tasks
            var tasks = (await _contx.DAILY_TASKS
                .Where(t => t.TaskType == (byte)TaskType.Common)
                .ToListAsync())
                .OrderBy(t => rand.Next())
                .Take(35)
                .ToList();

            //Take rare tasks
            tasks.AddRange((await _contx.DAILY_TASKS.Where(t => t.TaskType == (byte)TaskType.Rare)
                .ToListAsync())
                .OrderBy(t => rand.Next())
                .Take(15)
                );

            //Give an inclresed probability of getting a premium daily task if user has premium
            if (user.HasPremium)
            {
                //Add an additional task
                taskCount = 3;

                //Add 8 premium tasks to a list
                tasks.AddRange((await _contx.DAILY_TASKS.Where(t => t.TaskType == (byte)TaskType.Premium)
                    .ToListAsync())
                    .OrderBy(t => rand.Next())
                    .Take(8)
                    .ToList()
                    );
            }
            else
            {
                //Add only 2 premium tasks to a list
                tasks.AddRange((await _contx.DAILY_TASKS.Where(t => t.TaskType == (byte)TaskType.Premium)
                    .ToListAsync())
                    .OrderBy(t => rand.Next())
                    .Take(2)
                    .ToList()
                    );
            }

            tasks = tasks.OrderBy(t => new Random().Next())
                .Take(taskCount)
                .ToList();


            for (int i = 0; i < taskCount; i++)
            {
                var t = tasks[i];
                userDailyTasks.Add(new UserDailyTask
                {
                    UserId = userId,
                    DailyTaskId = t.Id,
                    DailyTaskClassLocalisationId = t.ClassLocalisationId,
                    Progress = 0,
                    AcquireMessage = await t.GenerateAcquireMessage(t),
                    IsAcquired = false
                });
            }

            await _contx.USER_DAILY_TASKS.AddRangeAsync(userDailyTasks);
            await _contx.SaveChangesAsync();

            return 1;      
        }

        public async Task<string> ShowDailyTaskProgressAsync(long userId, long taskId)
        {
            var task = await GetUserDailyTaskByIdAsync(userId, taskId);

            if (task == null)
                throw new NullReferenceException($"User does not have a task #{taskId}");

            return $"{task.Progress} / {task.DailyTask.Condition}";
        }

        public async Task<int> GetUserMaximumLanguageCountAsync(long userId)
        {
            var user = await _contx.SYSTEM_USERS.FindAsync(userId);

            if (user == null)
                return GetMaximumLanguageCount(null);

            return GetMaximumLanguageCount(user.HasPremium);
        }

        public int GetMaximumLanguageCount(bool? hasPremium)
        {
            if (hasPremium == null || !(bool)hasPremium)
                return 3;

            return 250; //TODO: Think if value should be hardcoded 
        }

        public async Task<UserNotification> GetUserNotificationAsync(long userId, long notificationId)
        {
            var notification = await _contx.USER_NOTIFICATIONS.Where(n => n.UserId1 == userId && n.Id == notificationId)
                .FirstOrDefaultAsync();

            return notification;
        }

        public async Task<byte> SendNotificationConfirmationCodeAsync(long userId, long notificationId)
        {
            var notification = await _contx.USER_NOTIFICATIONS.Where(n => n.UserId1 == userId && n.Id == notificationId)
                .FirstOrDefaultAsync();

            if (notification == null)
                return 0;

            if (await DeleteUserNotification(notification))
                return 1;

            return 0;
        }

        public async Task<List<long>> GetUserNotificationsIdsAsync(long userId)
        {
            var ids = await _contx.USER_NOTIFICATIONS.Where(n => n.UserId1 == userId)
                .Select(n => n.Id)
                .ToListAsync();

            return ids;
        }

        public async Task<int> GetUserPersonalityPointsAmount(long userId)
        {
            try
            {
                return await _contx.USER_WALLET_BALANCES.Where(b => b.UserId == userId)
                    .Select(b => b.PersonalityPoints)
                    .SingleOrDefaultAsync();
            }
            catch(NullReferenceException) { return 0; }
        }

        //Is used when user has passed some tests
        public async Task<bool> UpdateUserPersonalityStats(TestPayload model)
        {
            try
            {
                //Breake if test result wasnt saved
                if (!await RegisterTestPassingAsync(model))
                    return false;

                var userStats = await RecalculateUserStats(model);
                _contx.USER_PERSONALITY_STATS.Update(userStats);
                await _contx.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }

        public async Task<UserPersonalityPoints> UpdateUserPersonalityPoints(UserPersonalityPoints model)
        {
            try
            {
                model = await RecalculateSimilarityPercentage(model);
                _contx.Update(model);
                await _contx.SaveChangesAsync();

                return model;
            }
            catch { return null; }
        }

        public async Task<UserPersonalityStats> GetUserPersonalityStats(long userId)
        {
            try
            {
                return await _contx.USER_PERSONALITY_STATS
                    .Where(s => s.UserId == userId)
                    .SingleOrDefaultAsync();
            }
            catch { return null; }
        }

        public async Task<UserPersonalityPoints> GetUserPersonalityPoints(long userId)
        {
            try
            {
                return await _contx.USER_PERSONALITY_POINTS
                    .Where(s => s.UserId == userId)
                    .SingleOrDefaultAsync();
            }
            catch { return null; }
        }

        private async Task<UserPersonalityPoints> RecalculateSimilarityPercentage(UserPersonalityPoints model)
        {
            try
            {
                model.PersonalityPercentage = await CalculateSimilarityPreferences(model.Personality, model.PersonalityPercentage);
                model.CreativityPercentage = await CalculateSimilarityPreferences(model.Creativity, model.CreativityPercentage);
                model.ReliabilityPercentage = await CalculateSimilarityPreferences(model.Reliability, model.ReliabilityPercentage);
                model.NaturePercentage = await CalculateSimilarityPreferences(model.Nature, model.NaturePercentage);
                model.AgreeablenessPercentage = await CalculateSimilarityPreferences(model.Agreeableness, model.AgreeablenessPercentage);
                model.CompassionPercentage = await CalculateSimilarityPreferences(model.Compassion, model.CompassionPercentage);
                model.EmotionalIntellectPercentage = await CalculateSimilarityPreferences(model.EmotionalIntellect, model.EmotionalIntellectPercentage);
                model.IntellectPercentage = await CalculateSimilarityPreferences(model.Intellect, model.IntellectPercentage);
                model.LevelOfSensePercentage = await CalculateSimilarityPreferences(model.LevelOfSense, model.LevelOfSensePercentage);
                model.OpenMindednessPercentage = await CalculateSimilarityPreferences(model.OpenMindedness, model.OpenMindednessPercentage);
                model.SelfAwarenessPercentage = await CalculateSimilarityPreferences(model.SelfAwareness, model.SelfAwarenessPercentage);
                return model;
            }
            catch { return null; }
        }

        private async Task<UserPersonalityStats> RecalculateUserStats(TestPayload model)
        {
            var devider = 1;
            var userStats = await _contx.USER_PERSONALITY_STATS.Where(s => s.UserId == model.UserId)
                .SingleOrDefaultAsync();

            //If user have passed some test before - set the devider to 2, to find an average value
            if ((await _contx.USER_TESTS_RESULTS.Where(r => r.UserId == model.UserId).ToListAsync()).Count > 1)
                devider = 2;

            if (model.Personality != 0)
                userStats.Personality = (userStats.Personality + model.Personality) / devider;
            if (model.EmotionalIntellect != 0)
                userStats.EmotionalIntellect = (userStats.EmotionalIntellect + model.EmotionalIntellect) / devider;
            if (model.Reliability != 0)
                userStats.Reliability = (userStats.Reliability + model.Reliability) / devider;
            if (model.Compassion != 0)
                userStats.Compassion = (userStats.Compassion + model.Compassion) / devider;
            if (model.OpenMindedness != 0)
                userStats.OpenMindedness = (userStats.OpenMindedness + model.OpenMindedness) / devider;
            if (model.Agreeableness != 0)
                userStats.Agreeableness = (userStats.Agreeableness + model.Agreeableness) / devider;
            if (model.SelfAwareness != 0)
                userStats.SelfAwareness = (userStats.SelfAwareness + model.SelfAwareness) / devider;
            if (model.LevelOfSense != 0)
                userStats.LevelOfSense = (userStats.LevelOfSense + model.LevelOfSense) / devider;
            if (model.Intellect != 0)
                userStats.Intellect = (userStats.Intellect + model.Intellect) / devider;
            if (model.Nature != 0)
                userStats.Nature = (userStats.Nature + model.Nature) / devider;
            if (model.Creativity != 0)
                userStats.Creativity = (userStats.Creativity + model.Creativity) / devider;

            return userStats;
        }

        private async Task<double> CalculateSimilarityPreferences(int points, double similarityCoefficient)
        {
            if (points == 0)
                return 0;

            await Task.Run(() =>
            {
                var deviationCoefficient = 0d;
                for (int i = 1; i < points; i++)
                {
                    var speedCoefficient = 10;

                    deviationCoefficient = (i * similarityCoefficient) / speedCoefficient;
                    similarityCoefficient -= deviationCoefficient / (points / deviationCoefficient);
                }
            });

            return similarityCoefficient;
        }

        public async Task<bool> SwitchPersonalityUsage(long userId)
        {
            try
            {
                var userPrefs = await _contx.SYSTEM_USERS_PREFERENCES.Where(p => p.Id == userId)
                    .SingleOrDefaultAsync();
                
                if (userPrefs.ShouldUsePersonalityFunc)
                {
                    userPrefs.ShouldUsePersonalityFunc = false;
                }
                else
                {
                    userPrefs.ShouldUsePersonalityFunc = true;
                    UserPersonalityStats personalityStats;
                    UserPersonalityPoints personalityPoints;

                    //Add personality stats, if none were created when user was registering
                    if (await GetUserPersonalityStats(userId) == null)
                    {
                        personalityStats = new UserPersonalityStats(userId);
                        await _contx.USER_PERSONALITY_STATS.AddAsync(personalityStats);
                        await _contx.SaveChangesAsync();
                    }

                    //Add personality points, if none were created when user was registering
                    if (await GetUserPersonalityPoints(userId) == null)
                    {
                        personalityPoints = new UserPersonalityPoints(userId);
                        await _contx.USER_PERSONALITY_POINTS.AddAsync(personalityPoints);
                        await _contx.SaveChangesAsync();
                    }
                }

                _contx.SYSTEM_USERS_PREFERENCES.Update(userPrefs);
                await _contx.SaveChangesAsync();

                return userPrefs.ShouldUsePersonalityFunc;
            }
            catch { throw new NullReferenceException($"User {userId} does not exist !"); }
        }

        public async Task<bool> RegisterTestPassingAsync(TestPayload model)
        {
            try
            {
                await _contx.USER_TESTS_RESULTS.AddAsync(model);
                await _contx.SaveChangesAsync();

                return true;
            }
            catch { return false; }

        }

        public async Task<bool> UpdateTags(UpdateTags model)
        {
            try
            {
                if(await CheckUserIsRegistered(model.UserId))
                {
                    //Comas are removed to avoid bugs
                    var tags = model.RawTags.ToLower().Replace(",", "").Split(" ").ToList();

                    if(tags.Count > 0)
                    {
                        var user = await GetUserInfoAsync(model.UserId);
                        user.UserDataInfo.Tags = tags;

                        _contx.SYSTEM_USERS_DATA.Update(user.UserDataInfo);
                        await _contx.SaveChangesAsync();
                        return true;
                    }    
                }

                return false;
            }
            catch (NullReferenceException ) 
            { return false; }
        }

        public async Task<List<string>> GetTags(long userId)
        {
            try
            {
                if (await CheckUserIsRegistered(userId))
                {
                     var userData = await _contx.SYSTEM_USERS_DATA.Where(u => u.Id == userId).FirstOrDefaultAsync();
                    return userData.Tags;
                }

                return null;
            }
            catch (NullReferenceException)
            { return null; }
        }

        public async Task<User> GetUserListByTagsAsync(long userId)
        {
            var currentUser = await GetUserInfoAsync(userId);

            //Throw exception if user has reached his tag search limit
            if (!currentUser.HasPremium && currentUser.TagSearchesCount + 1 > 3)
                throw new ApplicationException($"User {userId} has already reached his tag-search limit");

            var tags = await GetTags(userId);
            var currentUserEncounters = await GetUserEncounters(userId, (int)Sections.Familiator); //I am not sure if it is 2 or 3 section

            var data = await _contx.SYSTEM_USERS
                .Where(u => u.UserId != currentUser.UserId)
                .Where(u => u.UserPreferences.AgePrefs.Contains(currentUser.UserDataInfo.UserAge))
                //Check if users gender preferences correspond to current user gender prefs or are equal to 'Does not matter'
                .Where(u => u.UserPreferences.UserGenderPrefs == currentUser.UserDataInfo.UserGender || u.UserPreferences.UserGenderPrefs == 2)
                .Where(u => u.UserDataInfo.UserLanguages.Any(l => currentUser.UserPreferences.UserLanguagePreferences.Contains(l)))
                .Where(u => currentUser.UserPreferences.AgePrefs.Contains(u.UserDataInfo.UserAge))
                .Where(u => currentUser.UserDataInfo.UserLanguages.Any(l => u.UserPreferences.UserLanguagePreferences.Contains(l)))
                .Include(u => u.UserBaseInfo)
                .Include(u => u.UserDataInfo)
                .ThenInclude(u => u.Location)
                .Include(u => u.UserPreferences)
                .Include(u => u.UserBlackList)
                .ToListAsync();

            //Check if users had encountered one another
            data = data.Where(u => !currentUser.CheckIfHasEncountered(currentUserEncounters, u.UserId)).ToList();

            //Check if users are in each others black lists
            data = data.Where(u => u.UserBlackList.Where(u => u.BannedUserId == userId).SingleOrDefault() == null).ToList();
            data = data.Where(u => currentUser.UserBlackList.Where(l => l.BannedUserId == u.UserId).SingleOrDefault() == null).ToList();

            //Check if request already exists
            data = data.Where(u => !CheckRequestExists(userId, u.UserId)).ToList();

            //Check if current user had already recieved request from user
            data = data.Where(u => !CheckRequestExists(u.UserId, userId)).ToList();

            //If user does NOT have gender prederences
            if (currentUser.UserPreferences.UserGenderPrefs != 2)
            {
                data = data.Where(u => u.UserDataInfo.UserGender == currentUser.UserPreferences.UserGenderPrefs)
                .Where(u => currentUser.UserPreferences.UserGenderPrefs == u.UserDataInfo.UserGender)
                .ToList();
            }

            currentUser.TagSearchesCount++;
            await _contx.SaveChangesAsync();

            ////TODO: remove in production
            //var tags1 = new List<string>();
            //tags1.Add("#starcraft");

            //var tags2 = new List<string>();
            //tags2.Add("#starcraft");
            //tags2.Add("#code");

            //var tags3 = new List<string>();
            //tags3.Add("#code");

            //data.Add(new User(5) { UserDataInfo = new UserDataInfo { Tags = tags1 } });
            //data.Add(new User(54) { UserDataInfo = new UserDataInfo { Tags = tags2 } });
            //data.Add(new User(57) { UserDataInfo = new UserDataInfo { Tags = tags3 } });

            //data.ForEach(d =>
            //{
            //    var t = d.UserDataInfo.Tags.Intersect(tags).Count();
            //    Console.WriteLine(t);
            //});

            var user = data.OrderByDescending(u => u.UserDataInfo.Tags.Intersect(tags).Count())
                .FirstOrDefault();

            return user;
        }

        public async Task<bool?> CheckUserUsesPersonality(long userId)
        {
            return await _contx.SYSTEM_USERS_PREFERENCES
                .Where(p => p.Id == userId)
                .Select(p => p.ShouldUsePersonalityFunc)
                .SingleOrDefaultAsync();
        }

        public async Task<List<BlackList>> GetBlackList(long userId)
        {
            return await _contx.USER_BLACKLISTS.Where(l => l.UserId == userId)
                .Include(l => l.BannedUser)
                .ToListAsync();
        }

        public async Task<bool> CheckEncounteredUserIsInBlackList(long userId, long encounteredUser)
        {
            return await _contx.USER_BLACKLISTS
                .Where(l => l.UserId == userId && l.BannedUserId == encounteredUser)
                .FirstOrDefaultAsync() != null;
        }
    }
}
