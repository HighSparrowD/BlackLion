using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Entities.AchievementEntities;
using MyWebApi.Entities.AdminEntities;
using MyWebApi.Entities.DailyTaskEntities;
using MyWebApi.Entities.EffectEntities;
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
using System.Linq;
using System.Runtime.InteropServices;
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
            try
            {
                var country = "---";
                var city = "---";

                if (location != null)
                {
                    await _contx.USER_LOCATIONS.AddAsync(location);
                    country = (await _contx.COUNTRIES.Where(c => c.Id == location.CountryId && c.ClassLocalisationId == location.CountryClassLocalisationId).Select(c => c.CountryName).SingleOrDefaultAsync());
                    city = (await _contx.CITIES.Where(c => c.Id == location.CountryId && c.CountryClassLocalisationId == location.CityCountryClassLocalisationId).Select(c => c.CityName).SingleOrDefaultAsync());
                }

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
                    var multiplier = 1;

                    if (invitor.InvitedUsersCount > 10)
                        multiplier = 2;

                    if (invitor.InvitedUsersCount == 1)
                    {
                        await TopUpUserWalletPointsBalance(invitor.UserId, 250 * multiplier, $"User {invitor.UserId} has invited his firs user");
                        await GrantPremiumToUser(invitor.UserId, 0, 1, (short)Currencies.Points);
                    }
                    else if (invitor.InvitedUsersCount == 3 || invitor.InvitedUsersCount % 3 == 0)
                    {
                        if (multiplier == 1)
                            invitor.InvitedUsersBonus = 0.15 + bonus;
                        await TopUpUserWalletPointsBalance(invitor.UserId, 1199 * multiplier, $"User {invitor.UserId} has invited 3 users");
                    }
                    else if (invitor.InvitedUsersCount == 7 || invitor.InvitedUsersCount % 7 == 0)
                    {
                        if (multiplier == 1)
                            invitor.InvitedUsersBonus = 0.35 + bonus;
                        await TopUpUserWalletPointsBalance(invitor.UserId, 1499 * multiplier, $"User {invitor.UserId} has invited 7 users");
                    }
                    else if (invitor.InvitedUsersCount == 10 || invitor.InvitedUsersCount % 10 == 0)
                    {
                        if (multiplier == 1)
                        {
                            invitor.InvitedUsersBonus = 0.5 + bonus;
                            // 1499 will then turn into 1999 due to premium purchase reward
                            await TopUpUserWalletPointsBalance(invitor.UserId, 1499, $"User {invitor.UserId} has invited 10 users");
                            //Adds + 10 random effects to users inventory
                            var effecId = new Random().Next(5, 10);
                            await PurchaseEffectAsync(invitor.UserId, effecId, 0, (int)Currencies.Points, 10);
                            await GrantPremiumToUser(model.UserId, 0, 30, (short)Currencies.Points);
                        }
                        else
                            await TopUpUserWalletPointsBalance(invitor.UserId, 1999 * multiplier, $"User {invitor.UserId} has invited more than 10 users");
                    }
                    else
                    {
                        await TopUpUserWalletPointsBalance(invitor.UserId, (int)(200 + (200 * bonus) * multiplier), $"User {model.UserId} was invited by user {invitor.UserId}");
                    }

                    model.BonusIndex = 1.5;
                    model.ParentId = invitor.UserId;

                    _contx.SYSTEM_USERS.Update(model);
                    await _contx.SaveChangesAsync();

                    //User is instantly liked by an invitor if he is allowing it
                    if (invitor.IncreasedFamiliarity)
                        await RegisterUserRequest(new UserNotification { UserId = invitor.UserId, UserId1 = model.UserId, IsLikedBack = false });
                    //Invitor is notified about referential registration
                    await NotifyUserAboutReferentialRegistrationAsync(invitor.UserId, model.UserId);
                }

                if (await CheckUserHasTasksInSectionAsync(model.UserId, (int)Sections.Registration))
                {
                    //TODO find and topup user's task progress
                }

                //Add Starting test pack
                //TODO: Add more tests here
                await PurchaseTestAsync(model.UserId, 1, dataModel.LanguageId);

                return model.UserId;
            }
            catch(Exception ex) {
                await LogAdminErrorAsync(model.UserId, ex.Message, (int)Sections.Registration);
                return 0;
            }
        }

        public async Task<List<long>> GetAllUsersAsync()
        {
            try
            {
                List<long> list = new List<long>();

                await Task.Run(() => {
                    list.Add(790042963); 
                    list.Add(1254647653); 
                });

                return list;
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(null, ex.Message, (int)Sections.Neutral);
                return null;
            }
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
            try
            {
                return await _contx.SYSTEM_USERS.Where(u => u.UserId == id).Include(s => s.UserBaseInfo)
                    .Include(s => s.UserBaseInfo)
                    .Include(s => s.UserDataInfo).ThenInclude(s => s.Location)
                    .Include(s => s.UserDataInfo).ThenInclude(s => s.Reason)
                    .Include(s => s.UserPreferences)
                    .Include(s => s.UserBlackList)
                    .SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(null, ex.Message, (int)Sections.Neutral);
                return null;
            }
        }

        public async Task<List<GetUserData>> GetUsersAsync(long userId, bool turnOffPersonalityFunc = false, bool isRepeated=false, bool isFreeSearch = false)
        {
            try
            {
                const byte miminalProfileCount = 5;
                var returnData = new List<GetUserData>();

                var currentUser = await GetUserInfoAsync(userId);

                //Check if user STILL has premium
                await CheckUserHasPremium(currentUser.UserId);

                var currentUserEncounters = await GetUserEncounters(userId, (int)SystemEnums.Sections.Familiator); //I am not sure if it is 2 or 3 section

                //If user has elected to temporarily dissable PERSONALITY functionality (Change shold NOT be changed in th DB) 
                if (turnOffPersonalityFunc)
                    currentUser.UserPreferences.ShouldUsePersonalityFunc = false;

                var data = await _contx.SYSTEM_USERS
                    .Where(u => u.UserId != currentUser.UserId)
                    .Where(u => u.UserDataInfo.ReasonId == currentUser.UserDataInfo.ReasonId)
                    .Where(u => u.UserPreferences.CommunicationPrefs == currentUser.UserPreferences.CommunicationPrefs)
                    .Where(u => u.UserPreferences.AgePrefs.Contains(currentUser.UserDataInfo.UserAge))
                    //Check if users gender preferences correspond to current user gender prefs or are equal to 'Does not matter'
                    .Where(u => u.UserPreferences.UserGenderPrefs == currentUser.UserDataInfo.UserGender || u.UserPreferences.UserGenderPrefs == 3)
                    .Where(u => u.UserDataInfo.UserLanguages.Any(l => currentUser.UserPreferences.UserLanguagePreferences.Contains(l)))
                    .Where(u => currentUser.UserPreferences.AgePrefs.Contains(u.UserDataInfo.UserAge))
                    .Where(u => currentUser.UserDataInfo.UserLanguages.Any(l => u.UserPreferences.UserLanguagePreferences.Contains(l)))
                    .Include(u => u.UserBaseInfo)
                    .Include(u => u.UserDataInfo)
                    .ThenInclude(u => u.Location)
                    .Include(u => u.UserPreferences)
                    .Include(u => u.UserBlackList)
                    .AsNoTracking()
                    .ToListAsync();

                if (currentUser.UserPreferences.ShouldFilterUsersWithoutRealPhoto && currentUser.HasPremium)
                {
                    data.Where(u => u.UserBaseInfo.IsPhotoReal)
                        .ToList();
                }    
                
                if (currentUser.UserDataInfo.Location.CountryId != null)
                {
                    data.Where(u => u.UserDataInfo.Location.CountryId != null)
                            .Where(u => u.UserPreferences.UserLocationPreferences.Contains((int)currentUser.UserDataInfo.Location.CountryId))
                            .Where(u => currentUser.UserPreferences.UserLocationPreferences.Contains((int)u.UserDataInfo.Location.CountryId));
                }

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
                if (currentUser.UserPreferences.UserGenderPrefs != 3)
                {
                    data = data.Where(u => u.UserDataInfo.UserGender == currentUser.UserPreferences.UserGenderPrefs)
                    .Where(u => currentUser.UserPreferences.UserGenderPrefs == u.UserDataInfo.UserGender)
                    .ToList();
                }

                //If user wants to find only people who are free today
                if (isFreeSearch)
                    data = data.Where(u => u.IsFree != null && (bool)u.IsFree).ToList();


                //If user uses PERSONALITY functionality and free search is disabled
                if (currentUser.UserPreferences.ShouldUsePersonalityFunc && !isFreeSearch)
                {
                    var userActiveEffects = await GetUserActiveEffects(userId);

                    var deviation = 0.15;
                    var minDeviation = 0.05;

                    var currentValueMax = 0d;
                    var currentValueMin = 0d;

                    var valentineBonus = 1d;

                    var hasActiveValentine = await CheckEffectIsActiveAsync(userId, (int)Currencies.TheValentine);

                    var userHasDetectorOn = await CheckEffectIsActiveAsync(userId, (int)Currencies.TheDetector);

                    if (hasActiveValentine)
                        valentineBonus = 2;

                    if (isRepeated)
                    {
                        deviation *= 1.5;
                        minDeviation *= 3.2;
                    }

                    var userPoints = await _contx.USER_PERSONALITY_POINTS.Where(p => p.UserId == currentUser.UserId)
                    .SingleOrDefaultAsync();

                    var userStats = await _contx.USER_PERSONALITY_STATS.Where(s => s.UserId == currentUser.UserId)
                    .SingleOrDefaultAsync();

                    var important = await userPoints.GetImportantParams();

                    for (int i = 0; i < data.Count; i++)
                    {
                        var importantMatches = 0;
                        var secondaryMatches = 0;
                        var matchedBy = "";

                        var u = data[i];

                        //Check if user uses personality functionality and remove him from the list if he does not
                        if (!u.UserPreferences.ShouldUsePersonalityFunc)
                        {
                            data.Remove(u);
                            continue;
                        }

                        var user2Points = await _contx.USER_PERSONALITY_POINTS.Where(p => p.UserId == u.UserId)
                        .SingleOrDefaultAsync();

                        var user2Stats = await _contx.USER_PERSONALITY_STATS.Where(s => s.UserId == u.UserId)
                        .SingleOrDefaultAsync();


                        //Turns off the parameter if it is 0
                        if (userPoints.Personality > 0 && userStats.Personality > 0 && user2Points.Personality > 0 && user2Stats.Personality > 0)
                        {
                            //TODO: create its own deviation variable depending on the number of personalities (It is likely to be grater than the nornal one)
                            var personalitySim = await CalculateSimilarityAsync(userStats.Personality * valentineBonus, user2Stats.Personality);

                            currentValueMax = ApplyMaxDeviation(userPoints.PersonalityPercentage, deviation);
                            currentValueMin = ApplyMinDeviation(userPoints.PersonalityPercentage, minDeviation);

                            //Negative conditions are applied, cuz this is an exclussive condition
                            if (personalitySim <= currentValueMax && personalitySim >= currentValueMin)
                            {
                                currentValueMax = ApplyMaxDeviation(user2Points.PersonalityPercentage, deviation);
                                currentValueMin = ApplyMinDeviation(user2Points.PersonalityPercentage, minDeviation);

                                if (personalitySim <= currentValueMax && personalitySim >= currentValueMin)
                                {
                                    matchedBy += "P. ";
                                    if (important.Contains(PersonalityStats.PersonalityType))
                                    {
                                        importantMatches++;
                                    }
                                    else
                                        secondaryMatches++;
                                }
                            }
                        }


                        if (userPoints.EmotionalIntellect > 0 && userStats.EmotionalIntellect > 0 && user2Points.EmotionalIntellect > 0 && user2Stats.EmotionalIntellect > 0)
                        {
                            var emIntellectSim = await CalculateSimilarityAsync(userStats.EmotionalIntellect * valentineBonus, user2Stats.EmotionalIntellect);

                            currentValueMax = ApplyMaxDeviation(userPoints.EmotionalIntellectPercentage, deviation);
                            currentValueMin = ApplyMinDeviation(userPoints.EmotionalIntellectPercentage, minDeviation);

                            if (emIntellectSim <= currentValueMax && emIntellectSim >= currentValueMin)
                            {                            
                                currentValueMax = ApplyMaxDeviation(user2Points.EmotionalIntellectPercentage, deviation);
                                currentValueMin = ApplyMinDeviation(user2Points.EmotionalIntellectPercentage, minDeviation);

                                if (emIntellectSim <= currentValueMax && emIntellectSim >= currentValueMin)
                                {
                                    matchedBy += "E. ";
                                    if (important.Contains(PersonalityStats.EmotionalIntellect))
                                    {
                                        importantMatches++;
                                    }
                                    else
                                        secondaryMatches++;
                                }
                            }
                        }


                        if (userPoints.Reliability > 0 && userStats.Reliability > 0 && user2Points.Reliability > 0 && user2Stats.Reliability > 0)
                        {
                            var reliabilitySim = await CalculateSimilarityAsync(userStats.Reliability * valentineBonus, user2Stats.Reliability);

                            currentValueMax = ApplyMaxDeviation(userPoints.ReliabilityPercentage, deviation);
                            currentValueMin = ApplyMinDeviation(userPoints.ReliabilityPercentage, minDeviation);

                            if (reliabilitySim <= currentValueMax && reliabilitySim >= currentValueMin)
                            {
                                currentValueMax = ApplyMaxDeviation(user2Points.ReliabilityPercentage, deviation);
                                currentValueMin = ApplyMinDeviation(user2Points.ReliabilityPercentage, minDeviation);

                                if (reliabilitySim <= currentValueMax && reliabilitySim >= currentValueMin)
                                {
                                    matchedBy += "R. ";
                                    if (important.Contains(PersonalityStats.Reliability))
                                    {
                                        importantMatches++;
                                    }
                                    else
                                        secondaryMatches++;
                                }
                            }
                        }


                        if (userPoints.Compassion > 0 && userStats.Compassion > 0 && user2Points.Compassion > 0 && user2Stats.Compassion > 0)
                        {
                            var compassionSim = await CalculateSimilarityAsync(userStats.Compassion * valentineBonus, user2Stats.Compassion);

                            currentValueMax = ApplyMaxDeviation(userPoints.CompassionPercentage, deviation);
                            currentValueMin = ApplyMinDeviation(userPoints.CompassionPercentage, minDeviation);

                            if (compassionSim <= currentValueMax && compassionSim >= currentValueMin)
                            {
                                currentValueMax = ApplyMaxDeviation(user2Points.CompassionPercentage, deviation);
                                currentValueMin = ApplyMinDeviation(user2Points.CompassionPercentage, minDeviation);

                                if (compassionSim <= currentValueMax && compassionSim >= currentValueMin)
                                {
                                    matchedBy += "S. ";
                                    if (important.Contains(PersonalityStats.Compassion))
                                    {
                                        importantMatches++;
                                    }
                                    else
                                        secondaryMatches++;
                                }
                            }
                        }


                        if (userPoints.OpenMindedness > 0 && userStats.OpenMindedness > 0 && user2Points.OpenMindedness > 0 && user2Stats.OpenMindedness > 0)
                        {
                            var openMindSim = await CalculateSimilarityAsync(userStats.OpenMindedness * valentineBonus, user2Stats.OpenMindedness);

                            currentValueMax = ApplyMaxDeviation(userPoints.OpenMindednessPercentage, deviation);
                            currentValueMin = ApplyMinDeviation(userPoints.OpenMindednessPercentage, minDeviation);

                            if (openMindSim <= currentValueMax && openMindSim >= currentValueMin)
                            {
                                currentValueMax = ApplyMaxDeviation(user2Points.OpenMindednessPercentage, deviation);
                                currentValueMin = ApplyMinDeviation(user2Points.OpenMindednessPercentage, minDeviation);

                                if (openMindSim <= currentValueMax && openMindSim >= currentValueMin)
                                {
                                    matchedBy += "O. ";
                                    if (important.Contains(PersonalityStats.OpenMindedness))
                                    {
                                        importantMatches++;
                                    }
                                    else
                                        secondaryMatches++;
                                }
                            }
                        }


                        if (userPoints.Agreeableness > 0 && userStats.Agreeableness > 0 && user2Points.Agreeableness > 0 && user2Stats.Agreeableness > 0)
                        {
                            var agreeablenessSim = await CalculateSimilarityAsync(userStats.Agreeableness * valentineBonus, user2Stats.Agreeableness);

                            currentValueMax = ApplyMaxDeviation(userPoints.AgreeablenessPercentage, deviation);
                            currentValueMin = ApplyMinDeviation(userPoints.AgreeablenessPercentage, minDeviation);

                            if (agreeablenessSim <= currentValueMax && agreeablenessSim >= currentValueMin)
                            {
                                currentValueMax = ApplyMaxDeviation(user2Points.AgreeablenessPercentage, deviation);
                                currentValueMin = ApplyMinDeviation(user2Points.AgreeablenessPercentage, minDeviation);

                                if (agreeablenessSim <= currentValueMax && agreeablenessSim >= currentValueMin)
                                {
                                    matchedBy += "N. ";
                                    if (important.Contains(PersonalityStats.Agreeableness))
                                    {
                                        importantMatches++;
                                    }
                                    else
                                        secondaryMatches++;
                                }
                            }
                        }


                        if (userPoints.SelfAwareness > 0 && userStats.SelfAwareness > 0 && user2Points.SelfAwareness > 0 && user2Stats.SelfAwareness > 0)
                        {
                            var selfAwerenessSim = await CalculateSimilarityAsync(userStats.SelfAwareness * valentineBonus, user2Stats.SelfAwareness);

                            currentValueMax = ApplyMaxDeviation(userPoints.SelfAwarenessPercentage, deviation);
                            currentValueMin = ApplyMinDeviation(userPoints.SelfAwarenessPercentage, minDeviation);

                            if (selfAwerenessSim <= currentValueMax && selfAwerenessSim >= currentValueMin)
                            {
                                currentValueMax = ApplyMaxDeviation(user2Points.SelfAwarenessPercentage, deviation);
                                currentValueMin = ApplyMinDeviation(user2Points.SelfAwarenessPercentage, minDeviation);

                                if (selfAwerenessSim <= currentValueMax && selfAwerenessSim >= currentValueMin)
                                {
                                    matchedBy += "A. ";
                                    if (important.Contains(PersonalityStats.SelfAwareness))
                                    {
                                        importantMatches++;
                                    }
                                    else
                                        secondaryMatches++;
                                }
                            }
                        }


                        if (userPoints.LevelOfSense > 0 && userStats.LevelOfSense > 0 && user2Points.LevelOfSense > 0 && user2Stats.LevelOfSense > 0)
                        {
                            var levelOfSense = await CalculateSimilarityAsync(userStats.LevelOfSense * valentineBonus, user2Stats.LevelOfSense);

                            currentValueMax = ApplyMaxDeviation(userPoints.LevelOfSensePercentage, deviation);
                            currentValueMin = ApplyMinDeviation(userPoints.LevelOfSensePercentage, minDeviation);

                            if (levelOfSense <= currentValueMax && levelOfSense >= currentValueMin)
                            {
                                currentValueMax = ApplyMaxDeviation(user2Points.LevelOfSensePercentage, deviation);
                                currentValueMin = ApplyMinDeviation(user2Points.LevelOfSensePercentage, minDeviation);

                                if (levelOfSense <= currentValueMax && levelOfSense >= currentValueMin)
                                {
                                    matchedBy += "L. ";
                                    if (important.Contains(PersonalityStats.LevelsOfSense))
                                    {
                                        importantMatches++;
                                    }
                                    else
                                        secondaryMatches++;
                                }
                            }
                        }


                        if (userPoints.Intellect > 0 && userStats.Intellect > 0 && user2Points.Intellect > 0 && user2Stats.Intellect > 0)
                        {
                            var intellectSim = await CalculateSimilarityAsync(userStats.Intellect * valentineBonus, user2Points.Intellect);

                            currentValueMax = ApplyMaxDeviation(userPoints.IntellectPercentage, deviation);
                            currentValueMin = ApplyMinDeviation(userPoints.IntellectPercentage, minDeviation);

                            if (intellectSim <= currentValueMax && intellectSim >= currentValueMin)
                            {
                                currentValueMax = ApplyMaxDeviation(user2Points.IntellectPercentage, deviation);
                                currentValueMin = ApplyMinDeviation(user2Points.IntellectPercentage, minDeviation);

                                if (intellectSim <= currentValueMax && intellectSim >= currentValueMin)
                                {
                                    matchedBy += "I. ";
                                    if (important.Contains(PersonalityStats.Intellect))
                                    {
                                        importantMatches++;
                                    }
                                    else
                                        secondaryMatches++;
                                }
                            }
                        }

                        if (userPoints.Nature > 0 && userStats.Nature > 0 && user2Points.Nature > 0 && user2Stats.Nature > 0)
                        {
                            var natureSim = await CalculateSimilarityAsync(userStats.Nature * valentineBonus, user2Stats.Nature);

                            currentValueMax = ApplyMaxDeviation(userPoints.NaturePercentage, deviation);
                            currentValueMin = ApplyMinDeviation(userPoints.NaturePercentage, minDeviation);

                            if (natureSim <= currentValueMax && natureSim >= currentValueMin)
                            {
                                currentValueMax = ApplyMaxDeviation(user2Points.NaturePercentage, deviation);
                                currentValueMin = ApplyMinDeviation(user2Points.NaturePercentage, minDeviation);

                                if (natureSim <= currentValueMax && natureSim >= currentValueMin)
                                {
                                    matchedBy += "T. ";
                                    if (important.Contains(PersonalityStats.Nature))
                                    {
                                        importantMatches++;
                                    }
                                    else
                                        secondaryMatches++;
                                }
                            }
                        }


                        if (userPoints.Creativity > 0 && userStats.Creativity > 0 && user2Points.Creativity > 0 && user2Stats.Creativity > 0)
                        {
                            var creativitySim = await CalculateSimilarityAsync(userStats.Creativity * valentineBonus, user2Stats.Creativity);

                            currentValueMax = ApplyMaxDeviation(userPoints.CreativityPercentage, deviation);
                            currentValueMin = ApplyMinDeviation(userPoints.CreativityPercentage, minDeviation);

                            if (creativitySim <= currentValueMax && creativitySim >= currentValueMin)
                            {
                                currentValueMax = ApplyMaxDeviation(user2Points.CreativityPercentage, deviation);
                                currentValueMin = ApplyMinDeviation(user2Points.CreativityPercentage, minDeviation);

                                if (creativitySim <= currentValueMax && creativitySim >= currentValueMin)
                                {
                                    matchedBy += "Y. ";
                                    if (important.Contains(PersonalityStats.Creativity))
                                    {
                                        importantMatches++;
                                    }
                                    else
                                        secondaryMatches++;
                                }
                            }
                        }

                        if (importantMatches < 1 && secondaryMatches < 3)
                        {
                            data.Remove(u);
                            continue;
                        };

                        var returnUser = new GetUserData(u, $"{u.UserBaseInfo.UserDescription}");

                        if (userHasDetectorOn)
                            returnUser.AddDescriptionBonus("<b>{matchedBy}</b>");

                        returnData.Add(returnUser);
                    }
                }
                else
                {
                    if(!isFreeSearch)
                    {
                        //Remove users, who is using PERSONALITY fucntionality
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
                    {
                        returnData = await GetUsersAsync(userId, turnOffPersonalityFunc:turnOffPersonalityFunc, isRepeated: true, isFreeSearch:isFreeSearch);
                    }

                    //Add user trust exp only if method was not repeated
                    await AddUserTrustProgressAsync(userId, 0.000003);

                    //Return users PERSONALITY usage property to normal (In case it was temporarily turned off)
                    if (turnOffPersonalityFunc)
                    {
                        currentUser.UserPreferences.ShouldUsePersonalityFunc = true;
                        await _contx.SaveChangesAsync();
                    }

                    //Fill-up return data if it wasnt filled in PERSONALITY check
                    if (!currentUser.UserPreferences.ShouldUsePersonalityFunc)
                    {
                        await Task.Run(async () =>
                        {
                            for (int i = 0; i < data.Count; i++)
                            {
                                var u = data[i];
                                string bonus = "";

                                //Register user encounter
                                await RegisterUserEncounter(new Encounter { UserId = userId, EncounteredUserId = u.UserId, SectionId = (int)Sections.Familiator });

                                if (u.IsIdentityConfirmed)
                                    bonus += $"✔️\n\n";
                                if (u.HasPremium && u.Nickname != "")
                                    bonus += $"<b>{u.Nickname}</b>\n";
                                returnData.Add(new GetUserData(u, bonus));
                            }
                        });
                    }

                    //Order user list randomly 
                    returnData = returnData.OrderBy(u => new Random().Next())
                        .ToList();

                    returnData.OrderByDescending(u => u.UserDataInfo.Location.CityId == currentUser.UserDataInfo.Location.CityId)
                        .ToList();
                }

                //await _contx.SaveChangesAsync();
                return returnData;
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Familiator);
                return null;
            }
        }

        public async Task<Country> GetCountryAsync(long id)
        {
            try
            {
                var c = await _contx.COUNTRIES.Include(c => c.Cities).SingleAsync(c => c.Id == id);
                return c;
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(null, ex.Message, (int)Sections.Neutral);
                return null;
            }
        }

        public async Task<long> AddFeedbackAsync(Feedback report)
        {
            try
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
            catch (Exception ex)
            {
                _contx.SYSTEM_FEEDBACKS.Remove(report);
                await LogAdminErrorAsync(report.UserBaseInfoId, ex.Message, (int)Sections.Reporter);
                return 0;
            }
        }

        private double ApplyMaxDeviation(double value, double deviation)
        {
            var currentValueMax = value + deviation;

            if (currentValueMax > 1)
                currentValueMax = 1;

            return currentValueMax;
        }

        private double ApplyMinDeviation(double value, double deviation)
        {
            var currentValueMin = value - deviation;

            if (currentValueMin < 0)
                currentValueMin = 0;

            return currentValueMin;
        }

        public async Task<bool> CheckUserExists(long id)
        {
            try
            {
                if (await _contx.SYSTEM_USERS.FindAsync(id) == null)
                { return false; }
                return true;
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(id, ex.Message, (int)Sections.Neutral);
                return false;
            }
        }

        public async Task<int> GetUserAppLanguage(long id)
        {
            try
            {
                var data = await _contx.SYSTEM_USERS_DATA.Where(u => u.Id == id).SingleAsync();
                return data.LanguageId;
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(id, ex.Message, (int)Sections.Registration);
                return 0;
            }
        }

        public async Task<List<FeedbackReason>> GetFeedbackReasonsAsync(int localisationId)
        {
            try
            {
                return await _contx.FEEDBACK_REASONS.Where(r => r.ClassLocalisationId == localisationId).ToListAsync();
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(null, ex.Message, (int)Sections.Neutral);
                return null;
            }
        }

        public async Task<bool> CheckUserIsRegistered(long userId)
        {
            try
            {
                return await _contx.SYSTEM_USERS.FindAsync(userId) != null;
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return false;
            }
        }

        public async Task<UserBaseInfo> GetUserBaseInfoAsync(long id)
        {
            try
            {
                return await _contx.SYSTEM_USERS_BASES.FindAsync(id);
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(id, ex.Message, (int)Sections.Neutral);
                return null;
            }
        }

        public async Task<bool> CheckUserHasVisitedSection(long userId, int sectionId)
        {
            try
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return false;
            }
        }

        public async Task<User> GetUserInfoByUsrnameAsync(string username)
        {
            try
            {
                return await _contx.SYSTEM_USERS
                    .Where(u => u.UserBaseInfo.UserName == username)
                    .Include(s => s.UserBaseInfo)
                    .Include(s => s.UserDataInfo).ThenInclude(s => s.Location)
                    .Include(s => s.UserDataInfo).ThenInclude(s => s.Reason)
                    .Include(s => s.UserPreferences)
                    .SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(null, ex.Message, (int)Sections.Neutral);
                return null;
            }
        }

        public async Task<List<Feedback>> GetMostRecentFeedbacks()
        {
            try
            {
                var pointInTime = DateTime.SpecifyKind(DateTime.Now.AddDays(-2), DateTimeKind.Utc);
                return await _contx.SYSTEM_FEEDBACKS
                    .Where(f => f.InsertedUtc >= pointInTime)
                    .Include(f => f.User)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(null, ex.Message, (int)Sections.Neutral);
                return null;
            }
        }

        public async Task<List<Feedback>> GetMostRecentFeedbacksByUserId(long userId)
        {
            try
            {
                var pointInTime = DateTime.SpecifyKind(DateTime.Now.AddDays(-2), DateTimeKind.Utc);
                return await _contx.SYSTEM_FEEDBACKS
                    .Where(f => f.InsertedUtc >= pointInTime && f.UserBaseInfoId == userId)
                    .Include(f => f.User)
                    .Include(f => f.Reason)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Reporter);
                return null;
            }
        }

        public async Task<Feedback> GetFeedbackById(long id)
        {
            try
            {
                return await _contx.SYSTEM_FEEDBACKS
                    .Where(f => f.Id == id)
                    .Include(f => f.User)
                    .Include(f => f.Reason)
                    .SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(null, ex.Message, (int)Sections.Reporter);
                return null;
            }
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(report.UserBaseInfoId, ex.Message, (int)Sections.Reporter);
                return 0;
            }
        }

        public async Task<List<Report>> GetMostRecentReports()
        {
            try
            {
                var pointInTime = DateTime.SpecifyKind(DateTime.Now.AddDays(-1), DateTimeKind.Utc);
                return await _contx.USER_REPORTS.Where(r => r.InsertedUtc > pointInTime).ToListAsync();
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(null, ex.Message, (int)Sections.Neutral);
                return null;
            }
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(null, ex.Message, (int)Sections.Neutral);
                return null;
            }
        }

        public async Task<List<Report>> GetAllReportsOnUserAsync(long userId)
        {
            try
            {
                return await _contx.USER_REPORTS.Where(r => r.UserBaseInfoId1 == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return null;
            }
        }

        public async Task<List<ReportReason>> GetReportReasonsAsync(int localisationId)
        {
            try
            {
                return await _contx.REPORT_REASONS.Where(r => r.ClassLocalisationId == localisationId).ToListAsync();
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(null, ex.Message, (int)Sections.Neutral);
                return null;
            }
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(null, ex.Message, (int)Sections.Settings);
                return false;
            }
        }

        public async Task<bool> RemoveUserFromBlackListAsync(long userId, long bannedUserId)
        {
            try
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Settings);
                return false;
            }
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return 0;
            }
        }

        public async Task<List<Report>> GetAllUserReportsAsync(long userId)
        {
            try
            {
                return await _contx.USER_REPORTS.Where(u => u.UserBaseInfoId == userId)
                    .Include(r => r.User)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return null;
            }
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return 0;
            }
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return 0;
            }
        }

        public async Task<bool> CheckUserIsBanned(long userId)
        {
            try
            {
                return (await _contx.SYSTEM_USERS.Where(u => u.UserId == userId).SingleOrDefaultAsync()).IsBanned; ;
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(null, ex.Message, (int)Sections.Neutral);
                return false;
            }
        }

        public async Task<bool> CheckUserIsDeleted(long userId)
        {
            try
            {
                return (await _contx.SYSTEM_USERS.Where(u => u.UserId == userId).SingleOrDefaultAsync()).IsDeleted;
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return false;
            }
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return null;
            }
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return null;
            }
        }

        public async Task<List<UserAchievement>> GetUserAchievements(long userId)
        {
            try
            {
                return await _contx.USER_ACHIEVEMENTS
                    .Where(a => a.UserBaseInfoId == userId)
                    .Include(a => a.Achievement)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Settings);
                return null;
            }
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return null;
            }
        }

        public async Task<byte> ReRegisterUser(long userId)
        {
            try
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Registration);
                return 0;
            }
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return 0;
            }
        }

        public async Task<List<UserAchievement>> GetUserAchievementsAsAdmin(long userId)
        {
            try
            {
                return await _contx.USER_ACHIEVEMENTS
                    .Where(a => a.UserBaseInfoId == userId && !a.IsAcquired)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return null;
            }
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return false;
            }
        }

        public async Task<bool> CheckUsersAreCombinableRT(long user1, long user2)
        {
            try
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(null, ex.Message, (int)Sections.RT);
                return false;
            }
        }

        public async Task<Balance> GetUserWalletBalance(long userId)
        {
            try
            {
                return await _contx.USER_WALLET_BALANCES
                    .Where(b => b.UserId == userId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Settings);
                return null;
            }
        }

        public async Task<int> TopUpUserWalletPointsBalance(long userId, int points, string description = "")
        {
            try
            {
                var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                var userBalance = await GetUserWalletBalance(userId);

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
                    await CreateUserBalance(userId, points, time);
                    userBalance = await GetUserWalletBalance(userId);
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return 0;
            }
        }

        private async Task CreateUserBalance(long userId, int points, DateTime time)
        {
           var userBalance = new Balance(userId, points, time);

            await _contx.USER_WALLET_BALANCES.AddAsync(userBalance);
            await _contx.SaveChangesAsync();
        }

        public async Task<int> TopUpUserWalletPPBalance(long userId, int points, string description = "")
        {
            try
            {
                var time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                var userBalance = await GetUserWalletBalance(userId);

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
                    await CreateUserBalance(userId, points, time);
                }

                var userParentId = (await _contx.SYSTEM_USERS.FindAsync(userId)).ParentId;

                if (userParentId != null && userParentId > 0)
                    await TopUpUserWalletPointsBalance((long)userParentId, 1, $"Referential reward for user's {userParentId} action");

                await _contx.SaveChangesAsync();
                await RegisterUserWalletPurchaseInPP(userId, points, description); //Registers info about amount of points decremented / incremented

                return userBalance.PersonalityPoints;
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return 0;
            }
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return false;
            }
        }

        public async Task<bool> CheckUserHasPremium(long userId)
        {
            try
            {
                var timeNow = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

                var user = await _contx.SYSTEM_USERS.Where(u => u.UserId == userId).SingleOrDefaultAsync();
                
                if (user != null)
                {
                    if ((bool)user.HasPremium && user.PremiumExpirationDate > timeNow)
                        user.HasPremium = false;

                    //TODO: Notify user that his premium access has expired

                    await _contx.SaveChangesAsync();
                    return user.HasPremium;
                }
                return false;
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return false;
            }
        }

        public async Task<DateTime> GetPremiumExpirationDate(long userId)
        {
            try
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Settings);
                return DateTime.MinValue;
            }
        }

        public async Task<DateTime> GrantPremiumToUser(long userId, int cost, int dayDuration, short currency)
        {
            try
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

                //PP Reward for purchasing long-term premium
                //TODO: Think if the amount is properly set...
                if (dayDuration >= 30)
                    await TopUpUserWalletPPBalance(userId, 5);

                if (user.PremiumExpirationDate < timeNow || user.PremiumExpirationDate == null)
                    user.PremiumExpirationDate = premiumFutureExpirationDate;
                else
                    user.PremiumExpirationDate.Value.AddDays(dayDuration);

                _contx.Update(user);
                await _contx.SaveChangesAsync();

                await AddUserNotificationAsync(new UserNotification { UserId = user.UserId, IsLikedBack = false, Severity = (short)Severities.Moderate, SectionId = (int)Sections.Neutral, Description = $"You have been granted premium access. Enjoy your benefits :)\nPremium expiration {user.PremiumExpirationDate.Value.ToShortTimeString()}" });

                return user.PremiumExpirationDate.Value;
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return DateTime.MinValue;
            }
        }

        private async Task<User> GetUserWithPremium(long userId, DateTime timeNow)
        {
            try
            {
                return await _contx.SYSTEM_USERS
                    .Where(u => u.UserId == userId && (bool)u.HasPremium && u.PremiumExpirationDate > timeNow)
                    .SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return null;
            }
        }

        public async Task<bool> CheckBalanceIsSufficient(long userId, int cost)
        {
            try
            {
                cost = cost < 0 ? cost * -1 : cost; //Makes sure the cost amount wasnt minus value
                return (await GetUserWalletBalance(userId)).Points >= cost;
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Shop);
                return false;
            }
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Registration);
                return 0;
            }
        }

        public async Task<byte> UpdateUserBaseAsync(UserBaseInfo user)
        {
            try
            {
                var country = "---";
                var city = "---";

                var model = await _contx.SYSTEM_USERS_BASES.FindAsync(user.Id);
                var data = await _contx.SYSTEM_USERS_DATA.FindAsync(user.Id);
                var location = await _contx.USER_LOCATIONS.Where(l => l.Id == user.Id)
                    .Include(l => l.Country)
                    .Include(l => l.City)
                    .SingleOrDefaultAsync();

                if (location.CountryId != null)
                {
                    country = await _contx.COUNTRIES
                        .Where(c => c.Id == location.CountryId && c.ClassLocalisationId == location.CountryClassLocalisationId)
                        .Select(c => c.CountryName)
                        .SingleOrDefaultAsync();
                    city = await _contx.CITIES
                        .Where(c => c.Id == location.CityId && c.CountryClassLocalisationId == location.CityCountryClassLocalisationId)
                        .Select(c => c.CityName)
                        .SingleOrDefaultAsync(); ;
                }

                model.UserRawDescription = user.UserRawDescription;
                model.UserDescription = user.GenerateUserDescription(user.UserName, data.UserAge, country, city, user.UserRawDescription);
                model.UserPhoto = user.UserPhoto;
                model.UserName = user.UserName;
                model.UserRealName = user.UserRealName;

                _contx.SYSTEM_USERS_BASES.Update(model);
                await _contx.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(user.Id, ex.Message, (int)Sections.Registration);
                return 0;
            }
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(user.Id, ex.Message, (int)Sections.Registration);
                return 0;
            }
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(user.Id, ex.Message, (int)Sections.Registration);
                return 0;
            }
        }

        public async Task<byte> UpdateUserLocationAsync(Location location)
        {
            try
            {
                var model = await _contx.USER_LOCATIONS.FindAsync(location.Id);

                model.CityId = location.CityId;
                model.CityCountryClassLocalisationId = location.CountryId != null? location.CountryClassLocalisationId : null;
                model.CountryId = location.CountryId;
                model.CountryClassLocalisationId = location.CityId != null ? location.CountryClassLocalisationId : null;

                _contx.USER_LOCATIONS.Update(model);
                await _contx.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(location.Id, ex.Message, (int)Sections.Registration);
                return 0;
            }
        }

        public async Task<bool> CheckUserIsBusy(long userId)
        {
            try
            {
                var user = await _contx.SYSTEM_USERS.Where(u => u.UserId == userId)
                    .SingleOrDefaultAsync();

                if (user != null)
                    return false;

                return (bool)user.IsBusy;
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return false;
            }
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return false;
            }
        }

        public async Task<List<UserNotification>> GetUserRequests(long userId)
        {
            try
            {
                return await _contx.USER_NOTIFICATIONS
                    .Where(r => r.UserId1 == userId)
                    .Where(r => r.SectionId == (int)SystemEnums.Sections.Familiator || r.SectionId == (int)SystemEnums.Sections.Requester)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Requester);
                return null;
            }
        }

        public async Task<UserNotification> GetUserRequest(Guid requestId)
        {
            try
            {
                return await _contx.USER_NOTIFICATIONS
                    .Where(r => r.Id == requestId)
                    .Where(r => r.SectionId == (int)SystemEnums.Sections.Familiator || r.SectionId == (int)SystemEnums.Sections.Requester)
                    .SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(0, ex.Message, (int)Sections.Requester);
                return null;
            }
        }

        public async Task<string> RegisterUserRequest(UserNotification request)
        {
            try
            {
                request.Severity = (short)Severities.Moderate;
                var returnMessage = "";

                if (request.IsLikedBack)
                {
                    request.SectionId = (short)Sections.Requester;

                    if ((byte)new Random().Next(0, 2) == 0)
                    {
                        var senderUserName = await _contx.SYSTEM_USERS_BASES.Where(d => d.Id == request.UserId).Select(d => d.UserName).SingleOrDefaultAsync();

                        //Delete request, user had just answered
                        var requestId = await _contx.USER_NOTIFICATIONS.Where(n => n.UserId == request.UserId1 && n.UserId1 == request.UserId).Select(n => n.Id).SingleOrDefaultAsync();
                        await DeleteUserRequest(requestId);

                        //TODO: Get message from localizer based on users`s localization 
                        request.Description = $"Hey! I have got a match for you. This person was notified about it, but he did not receive your username, thus he cannot write you first everything is in your hands, do not miss your chance!\n\n@{senderUserName}";
                        returnMessage = "Hey! I have a match for you. Right now this person is deciding whether or not to write you Just wait for it!\n\n";
                    }
                    else
                    {
                        var receiverUserName = await _contx.SYSTEM_USERS_BASES.Where(d => d.Id == request.UserId1).Select(d => d.UserName).SingleOrDefaultAsync();

                        //Delete request, user had just answered
                        var requestId = await _contx.USER_NOTIFICATIONS.Where(n => n.UserId == request.UserId1 && n.UserId1 == request.UserId).Select(n => n.Id).SingleOrDefaultAsync();
                        await DeleteUserRequest(requestId);

                        //TODO: Get message from localizer based on users`s localization 
                        request.Description = "Hey! I have a match for you. Right now this person is deciding whether or not to write you Just wait for it!\n\n";
                        returnMessage = $"Hey! I have got a match for you. This person was notified about it, but he did not receive your username, thus he cannot write you first everything is in your hands, do not miss your chance!\n\n@{receiverUserName}";
                    }
                }
                else
                    request.SectionId = (short)Sections.Familiator;

                await RegisterUserEncounter(new Encounter { UserId = (long)request.UserId, EncounteredUserId = request.UserId1, SectionId = (int)Sections.Requester });

                var id = await AddUserNotificationAsync(request);

                return returnMessage;
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(request.UserId, ex.Message, (int)Sections.Requester);
                return null;
            }
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Requester);
                return 0;
            }
        }

        public async Task<byte> DeleteUserRequest(Guid requestId)
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(0, ex.Message, (int)Sections.Requester);
                return 0;
            }
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Requester);
                return false;
            }
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(null, ex.Message, (int)Sections.Neutral);
                return false;
            }
        }

        public async Task<Guid?> RegisterUserEncounter(Encounter model)
        {
            try
            {
                model.Id = Guid.NewGuid();
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
            catch (Exception ex)
            {
                await LogAdminErrorAsync(model.UserId, ex.Message, (int)Sections.Familiator);
                return null;
            }
        }

        public async Task<Encounter> GetUserEncounter(long encounterId)
        {
            try
            {
                return await _contx.USER_ENCOUNTERS.FindAsync(encounterId);
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(null, ex.Message, (int)Sections.Neutral);
                return null;
            }
        }

        public async Task<List<Encounter>> GetUserEncounters(long userId, int sectionId)
        {
            try
            {
                var encounters = await _contx.USER_ENCOUNTERS
                    .Where(e => e.UserId == userId)
                    .Where(e => e.SectionId == sectionId)
                    .Include(e => e.EncounteredUser)
                    .ToListAsync();

                return encounters != null ? encounters : new List<Encounter>();
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return null;
            }
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
        public async Task<string> GetQRCode(long userId)
        {
            string data;

            var creds = await _contx.USER_INVITATION_CREDENTIALS.Where(c => c.UserId == userId)
                .SingleOrDefaultAsync();

            if (creds != null)
            {
                if (creds.QRCode != null)
                    return creds.QRCode;

                string link = creds.Link.Replace("%2F", "/");

                var generator = new QRCodeGenerator();
                var d = generator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
                var code = new PngByteQRCode(d).GetGraphic(5);

                data = Convert.ToBase64String(code);

                creds.QRCode = data;
                await _contx.SaveChangesAsync();

                return data;
            }

            if(await CheckUserIsRegistered(userId))
            {
                //If credentials do not exist. Create them and try generationg QrCode again
                await GenerateInvitationCredentialsAsync(userId);
                return await GetQRCode(userId);
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

            //Generate link if it wast generated earlier for some reason
            invitation = await GenerateInvitationCredentialsAsync(userId);

            return invitation.Link;
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

                await AddUserNotificationAsync(new UserNotification
                {
                    UserId1 = userId,
                    IsLikedBack = false,
                    Description = $"Hey! new user had been registered via your link. Thanks for helping us grow!\nSo far, you have invited: {invitedUsersCount} people. \nYou receive 1p for every action they are maiking ;-)",
                    SectionId = (int)Sections.Registration,
                    Severity = (short)Severities.Moderate
                });

                return true;
            }

            return false;
        }

        //TODO: call when premium has expired
        public async Task<bool> NotifyPremiumExpiration(long userId)
        {
            try
            {
                await AddUserNotificationAsync(new UserNotification {UserId=userId, Severity=(short)Severities.Urgent, SectionId=(int)Sections.Neutral, Description="Your premium access has expired"});
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Guid> AddUserNotificationAsync(UserNotification model)
        {
            try
            {
                model.Id = Guid.NewGuid();
                await _contx.USER_NOTIFICATIONS.AddAsync(model);
                await _contx.SaveChangesAsync();

                if (model.UserId != null)
                    await AddUserTrustProgressAsync((long)model.UserId, 0.000002);

                return model.Id;
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

        public async Task<bool> DeleteUserNotification(Guid notificationId)
        {
            try
            {
                var notification = await GetUserNotificationAsync(notificationId);

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
                .Where(t => t.TaskType == (byte)TaskTypes.Common)
                .ToListAsync())
                .OrderBy(t => rand.Next())
                .Take(35)
                .ToList();

            //Take rare tasks
            tasks.AddRange((await _contx.DAILY_TASKS.Where(t => t.TaskType == (byte)TaskTypes.Rare)
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
                tasks.AddRange((await _contx.DAILY_TASKS.Where(t => t.TaskType == (byte)TaskTypes.Premium)
                    .ToListAsync())
                    .OrderBy(t => rand.Next())
                    .Take(8)
                    .ToList()
                    );
            }
            else
            {
                //Add only 2 premium tasks to a list
                tasks.AddRange((await _contx.DAILY_TASKS.Where(t => t.TaskType == (byte)TaskTypes.Premium)
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

        public async Task<UserNotification> GetUserNotificationAsync(Guid notificationId)
        {
            var notification = await _contx.USER_NOTIFICATIONS.FindAsync(notificationId);

            return notification;
        }

        public async Task<byte> SendNotificationConfirmationCodeAsync(Guid notificationId)
        {
            var notification = await _contx.USER_NOTIFICATIONS.FindAsync(notificationId);

            if (notification == null)
                return 0;

            if (await DeleteUserNotification(notification))
                return 1;

            return 0;
        }

        public async Task<List<Guid>> GetUserNotificationsIdsAsync(long userId)
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
                //Break if test result wasnt saved
                if (!await RegisterTestPassingAsync(model))
                    return false;

                var userStats = await RecalculateUserStats(model);
                _contx.USER_PERSONALITY_STATS.Update(userStats);
                await _contx.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> UpdateUserPersonalityPoints(PointsPayload model)
        {
            try
            {
                var points = await RecalculateSimilarityPercentage(model);
                var balance = await _contx.USER_WALLET_BALANCES.Where(b => b.UserId == model.UserId)
                    .SingleOrDefaultAsync();

                balance.PersonalityPoints = model.Balance;

                _contx.Update(balance);
                _contx.Update(points);
                await _contx.SaveChangesAsync();

                return true;
            }
            catch { return false; }
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

        private async Task<UserPersonalityPoints> RecalculateSimilarityPercentage(PointsPayload model)
        {
            try
            {
                var points = await GetUserPersonalityPoints(model.UserId);

                if (model.Personality != null)
                {
                    points.PersonalityPercentage = await CalculateSimilarityPreferences((int)model.Personality, points.PersonalityPercentage);
                    points.Personality = (int)model.Personality;
                }

                if (model.Creativity != null)
                {
                    points.CreativityPercentage = await CalculateSimilarityPreferences((int)model.Creativity, points.CreativityPercentage);
                    points.Creativity = (int)model.Creativity;
                }

                if (model.Reliability != null)
                {
                    points.ReliabilityPercentage = await CalculateSimilarityPreferences((int)model.Reliability, points.ReliabilityPercentage);
                    points.Reliability = (int)model.Reliability;
                }

                if (model.Nature != null)
                {
                    points.NaturePercentage = await CalculateSimilarityPreferences((int)model.Nature, points.NaturePercentage);
                    points.Nature = (int)model.Nature;
                }

                if (model.Agreeableness != null)
                {
                    points.AgreeablenessPercentage = await CalculateSimilarityPreferences((int)model.Agreeableness, points.AgreeablenessPercentage);
                    points.Agreeableness = (int)model.Agreeableness;
                }

                if (model.Compassion != null)
                {
                    points.CompassionPercentage = await CalculateSimilarityPreferences((int)model.Compassion, points.CompassionPercentage);
                    points.Compassion = (int)model.Compassion;
                }

                if (model.EmotionalIntellect != null)
                {
                    points.EmotionalIntellectPercentage = await CalculateSimilarityPreferences((int)model.EmotionalIntellect, points.EmotionalIntellectPercentage);
                    points.EmotionalIntellect = (int)model.EmotionalIntellect;
                }

                if (model.Intellect != null)
                {
                    points.IntellectPercentage = await CalculateSimilarityPreferences((int)model.Intellect, points.IntellectPercentage);
                    points.Intellect = (int)model.Intellect;
                }

                if (model.LevelOfSense != null)
                {
                    points.LevelOfSensePercentage = await CalculateSimilarityPreferences((int)model.LevelOfSense, points.LevelOfSensePercentage);
                    points.LevelOfSense = (int)model.LevelOfSense;
                }

                if (model.OpenMindedness != null)
                {
                    points.OpenMindednessPercentage = await CalculateSimilarityPreferences((int)model.OpenMindedness, points.OpenMindednessPercentage);
                    points.OpenMindedness = (int)model.OpenMindedness;
                }

                if (model.SelfAwareness != null)
                {
                    points.SelfAwarenessPercentage = await CalculateSimilarityPreferences((int)model.SelfAwareness, points.SelfAwarenessPercentage);
                    points.SelfAwareness = (int)model.SelfAwareness;
                }

                return points;
            }
            catch { return null; }
        }

        private async Task<UserPersonalityStats> RecalculateUserStats(TestPayload model)
        {
            var devider = 1;
            var userStats = await _contx.USER_PERSONALITY_STATS.Where(s => s.UserId == model.UserId)
                .SingleOrDefaultAsync();

            //Create user stats if they werent created before
            if (userStats != null)
            {
                userStats = new UserPersonalityStats(model.UserId);
                await _contx.USER_PERSONALITY_STATS.AddAsync(userStats);
                await _contx.SaveChangesAsync();
            }


            if (model.Personality != 0)
            {
                //Set the devider to 2 if user had passed the tests of the same type before
                if (userStats.Personality != 0)
                    devider = 2;

                userStats.Personality = (userStats.Personality + model.Personality) / devider;
                //Return the devider to its normal state
                devider = 1;
            }
            if (model.EmotionalIntellect != 0)
            {
                if (userStats.EmotionalIntellect != 0)
                    devider = 2;

                userStats.EmotionalIntellect = (userStats.EmotionalIntellect + model.EmotionalIntellect) / devider;
                devider = 1;
            }
            if (model.Reliability != 0)
            {
                if (userStats.Reliability != 0)
                    devider = 2;

                userStats.Reliability = (userStats.Reliability + model.Reliability) / devider;
                devider = 1;
            }
            if (model.Compassion != 0)
            {
                if (userStats.Compassion != 0)
                    devider = 2;

                userStats.Compassion = (userStats.Compassion + model.Compassion) / devider;
                devider = 1;
            }
                
            if (model.OpenMindedness != 0)
            {
                if (userStats.OpenMindedness != 0)
                    devider = 2;

                userStats.OpenMindedness = (userStats.OpenMindedness + model.OpenMindedness) / devider;
                devider = 1;
            }
            if (model.Agreeableness != 0)
            {
                if (userStats.Agreeableness != 0)
                    devider = 2;

                userStats.Agreeableness = (userStats.Agreeableness + model.Agreeableness) / devider;
                devider = 1;
            }
            if (model.SelfAwareness != 0)
            {
                if (userStats.SelfAwareness != 0)
                    devider = 2;

                userStats.SelfAwareness = (userStats.SelfAwareness + model.SelfAwareness) / devider;
                devider = 1;
            }
                
            if (model.LevelOfSense != 0)
            {
                if (userStats.LevelOfSense != 0)
                    devider = 2;
                
                userStats.LevelOfSense = (userStats.LevelOfSense + model.LevelOfSense) / devider;
                devider = 1;
            }
            if (model.Intellect != 0)
            {
                if (userStats.Intellect != 0)
                    devider = 2;

                userStats.Intellect = (userStats.Intellect + model.Intellect) / devider;
                devider = 1;
            }
            if (model.Nature != 0)
            {
                if (userStats.Nature != 0)
                    devider = 2;

                userStats.Nature = (userStats.Nature + model.Nature) / devider;
                devider = 1;
            }
            if (model.Creativity != 0)
            {
                if (userStats.Creativity != 0)
                    devider = 2;

                userStats.Creativity = (userStats.Creativity + model.Creativity) / devider;
                devider = 1;
            }

            return userStats;
        }

        private async Task<double> CalculateSimilarityPreferences(int points, double similarityCoefficient)
        {
            if (points == 0)
                return 1;

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
                var userTest = await _contx.user_tests.Where(t => t.UserId == model.UserId && t.TestId == model.TestId)
                    .SingleOrDefaultAsync();

                //Give user 1 PP for passing the test for the first time
                if (userTest.PassedOn != null)
                {
                    var userBalance = await GetUserWalletBalance(model.UserId);
                    userBalance.PersonalityPoints++;
                    await _contx.SaveChangesAsync();
                }

                userTest.PassedOn = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

                _contx.user_tests.Update(userTest);
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

        public async Task<User> GetUserListByTagsAsync(GetUserByTags model)
        {
            var currentUser = await GetUserInfoAsync(model.UserId);
            var hasActiveDetector = await CheckEffectIsActiveAsync(currentUser.UserId, (int)Currencies.TheDetector);

            //Throw exception if user has reached his tag search limit
            if (!currentUser.HasPremium && currentUser.TagSearchesCount + 1 > 3)
                throw new ApplicationException($"User {model.UserId} has already reached his tag-search limit");

            var currentUserEncounters = await GetUserEncounters(model.UserId, (int)Sections.Familiator); //I am not sure if it is 2 or 3 section

            var data = await _contx.SYSTEM_USERS
                    .Where(u => u.UserId != currentUser.UserId)
                    .Where(u => u.UserPreferences.AgePrefs.Contains(currentUser.UserDataInfo.UserAge))
                    .Where(u => u.UserDataInfo.UserLanguages.Any(l => currentUser.UserPreferences.UserLanguagePreferences.Contains(l)))
                    .Where(u => currentUser.UserPreferences.AgePrefs.Contains(u.UserDataInfo.UserAge))
                    .Where(u => currentUser.UserDataInfo.UserLanguages.Any(l => u.UserPreferences.UserLanguagePreferences.Contains(l)))
                    .Include(u => u.UserBaseInfo)
                    .Include(u => u.UserDataInfo)
                    .ThenInclude(u => u.Location)
                    .Include(u => u.UserPreferences)
                    .Include(u => u.UserBlackList)
                    .AsNoTracking()
                    .ToListAsync();

            if (currentUser.UserPreferences.ShouldFilterUsersWithoutRealPhoto && currentUser.HasPremium)
            {
                data.Where(u => u.UserBaseInfo.IsPhotoReal)
                    .ToList();
            }

            //Check if users had encountered one another
            data = data.Where(u => !currentUser.CheckIfHasEncountered(currentUserEncounters, u.UserId)).ToList();

            //Check if users are in each others black lists
            data = data.Where(u => u.UserBlackList.Where(u => u.BannedUserId == model.UserId).SingleOrDefault() == null).ToList();
            data = data.Where(u => currentUser.UserBlackList.Where(l => l.BannedUserId == u.UserId).SingleOrDefault() == null).ToList();

            //Check if request already exists
            data = data.Where(u => !CheckRequestExists(model.UserId, u.UserId)).ToList();

            //Check if current user had already recieved request from user
            data = data.Where(u => !CheckRequestExists(u.UserId, model.UserId)).ToList();

            currentUser.TagSearchesCount++;
            await _contx.SaveChangesAsync();

            data = data.Where(d => d.UserDataInfo.Tags != null).ToList();

            var user = data.OrderByDescending(u => u.UserDataInfo.Tags.Intersect(model.Tags).Count())
                .FirstOrDefault();

            if(user.HasPremium && user.Nickname != null)
                user.UserBaseInfo.UserDescription = $"<b>{user.Nickname}</b>\n\n{user.UserBaseInfo.UserDescription}";
            if(user.IsIdentityConfirmed)
                user.UserBaseInfo.UserDescription = $"✔️{user.UserBaseInfo.UserDescription}";
            //Show tags if user has detector activated
            if (hasActiveDetector)
                user.UserBaseInfo.UserDescription += String.Join(" ", user.UserDataInfo.Tags);

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

        public async Task<string> RetreiveCommonLanguagesAsync(long user1Id, long user2Id, int localisationId)
        {
            var user1Langs = await _contx.SYSTEM_USERS_DATA.Where(u => u.Id == user1Id)
                .Select(u => u.UserLanguages)
                .SingleOrDefaultAsync();

            var user2Langs = await _contx.SYSTEM_USERS_DATA.Where(u => u.Id == user2Id)
                .Select(u => u.UserLanguages)
                .SingleOrDefaultAsync();

            var commonIds = user1Langs.Intersect(user2Langs);
            var commons = await _contx.LANGUAGES.Where(l => commonIds
                .Any(i => i == l.Id) && l.ClassLocalisationId == 0)
                .Select(l => l.LanguageName)
                .ToListAsync();

            return String.Join(", ", commons);
        }

        public async Task<bool> LogAdminErrorAsync(long? userId, string description, int sectioId)
        {
            var error = new AdminErrorLog
            {
                Id = Guid.NewGuid(),
                ThrownByUser = userId, 
                Description = description,
                SectionId = sectioId,
                Time = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
            };

            await _contx.ADMIN_ERROR_LOGS.AddAsync(error);
            await _contx.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SetAutoReplyTextAsync(long userId, string text)
        {
            try
            {
                var user = await _contx.SYSTEM_USERS_DATA.Where(u => u.Id == userId)
                    .SingleOrDefaultAsync();

                user.AutoReplyText = text;

                _contx.SYSTEM_USERS_DATA.Update(user);
                await _contx.SaveChangesAsync();

                return true;
            }
            catch { return false; }
        }

        public async Task<bool> SetAutoReplyVoiceAsync(long userId, string voice)
        {
            try
            {
                if (await CheckUserHasPremium(userId))
                {
                    var user = await _contx.SYSTEM_USERS_DATA.Where(u => u.Id == userId)
                        .SingleOrDefaultAsync();

                    user.AutoReplyVoice = voice;

                    _contx.SYSTEM_USERS_DATA.Update(user);
                    await _contx.SaveChangesAsync();

                    return true;

                }
                return false;
            }
            catch { return false; }
        }

        public async Task<ActiveAutoReply> GetActiveAutoReplyAsync(long userId)
        {
            var reply = new ActiveAutoReply();
            var replyData = await _contx.SYSTEM_USERS_DATA.Where(d => d.Id == userId)
                .Select(d => new { d.AutoReplyText, d.AutoReplyVoice })
                .SingleOrDefaultAsync();

            if (await CheckUserHasPremium(userId))
            {
                if (replyData.AutoReplyVoice != null)
                {
                    reply.AutoReply = replyData.AutoReplyVoice;
                    reply.IsText = false;
                    reply.IsEmpty = false;
                }
                else if (replyData.AutoReplyText != null)
                {
                    reply.AutoReply = replyData.AutoReplyText;
                    reply.IsText = true;
                    reply.IsEmpty = true;
                }
                else
                {
                    reply.IsEmpty = true;
                }
                return reply;
            }

            else if (replyData.AutoReplyText != null)
            {
                reply.AutoReply = replyData.AutoReplyText;
                reply.IsText = true;
                reply.IsEmpty = true;

                return reply;
            }

            reply.IsEmpty = true;

            return reply;
        }

        public async Task<bool> CheckUserHasEffectAsync(long userId, int effectId)
        {
            try
            {
                var balance = await GetUserWalletBalance(userId);
                var value = false;

                if (balance != null)
                {
                    switch (effectId)
                    {
                        case 5:
                            value = balance.SecondChances > 0;
                            break;
                        case 6:
                            value = balance.Valentines > 0;
                            break;
                        case 7:
                            value = balance.Detectors > 0;
                            break;
                        case 8:
                            value = balance.Nullifiers > 0;
                            break;
                        case 9:
                            value = balance.CardDecksMini > 0;
                            break;
                        case 10:
                            value = balance.CardDecksPlatinum > 0;
                            break;
                        case 11:
                            value = balance.ThePersonalities > 0;
                            break;
                        default:
                            break;
                            
                    }
                    return value;
                }

                await CreateUserBalance(userId, 0, DateTime.Now);
                return false;
            }
            catch { return false; }
        }

        public async Task<DateTime?> ActivateDurableEffectAsync(long userId, int effectId)
        {
            try
            {
                ActiveEffect effect;
                var userBalance = await GetUserWalletBalance(userId);

                switch (effectId)
                {
                    case 6:
                        var userPoints = await _contx.USER_PERSONALITY_POINTS.Where(p => p.UserId == userId)
                            .SingleOrDefaultAsync();
                        if (userPoints != null)
                        {
                            if (AtLeastOneIsNotZero(userPoints))
                            {
                                userBalance.Valentines--;
                                effect = new TheValentine(userId);
                                break;
                            }
                            return null;
                        }
                        return null;
                    case 7:
                        if (!(bool)await CheckUserUsesPersonality(userId))
                            return null;
                        userBalance.Detectors--;
                        effect = new TheDetector(userId);
                        break;
                    case 8:
                        if (!(bool)await CheckUserUsesPersonality(userId))
                            return null;
                        userBalance.Nullifiers--;
                        effect = new TheWhiteDetector(userId);
                        break;
                    default:
                        return null;
                }

                await _contx.USER_ACTIVE_EFFECTS.AddAsync(effect);
                await _contx.SaveChangesAsync();
                return effect.ExpirationTime;
            }
            catch { return null; }
        }

        public async Task<bool> ActivateToggleEffectAsync(long userId, int effectId, long? user2Id = null, string description=null)
        {
            try
            {
                var userBalance = await GetUserWalletBalance(userId);

                switch (effectId)
                {
                    case 5:
                        userBalance.SecondChances--;
                        await RegisterUserRequest(new UserNotification
                        {
                            UserId = userId,
                            UserId1 = (long)user2Id,
                            IsLikedBack = false,
                            Description = description,
                            SectionId = (int)Sections.Familiator,
                            Severity = (short)Severities.Moderate
                        });
                        await _contx.SaveChangesAsync();
                        return true;
                    case 8:
                        userBalance.Nullifiers --;
                        await _contx.SaveChangesAsync();
                        return true;
                    case 9:
                        userBalance.CardDecksMini--;
                        await AddMaxUserProfileViewCount(userId, 20);
                        await _contx.SaveChangesAsync();
                        return true;
                    case 10:
                        userBalance.CardDecksPlatinum--;
                        await AddMaxUserProfileViewCount(userId, 50);
                        await _contx.SaveChangesAsync();
                        return true;
                    default:
                        return false;
                }
            }
            catch { return false; }
        }

        private bool AtLeastOneIsNotZero(UserPersonalityPoints points)
        {
            if (points.Intellect > 0 || points.SelfAwareness > 0 || 
                points.Agreeableness > 0 || points.Compassion > 0 ||
                points.Creativity > 0 || points.EmotionalIntellect > 0 ||
                points.Reliability > 0 || points.Personality > 0 || points.LevelOfSense > 0 ||
                points.Nature > 0 || points.OpenMindedness > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<List<ActiveEffect>> GetUserActiveEffects(long userId)
        {
            try
            {
                var effectsToRemove = new List<ActiveEffect>();
                var effectsToReturn = new List<ActiveEffect>();

                foreach (var effect in await _contx.USER_ACTIVE_EFFECTS.Where(e => e.UserId == userId).ToListAsync())
                {
                    if (effect.ExpirationTime.Value <= DateTime.UtcNow)
                        effectsToRemove.Add(effect);
                    else
                        effectsToReturn.Add(effect);
                }

                if (effectsToRemove.Count > 0)
                    _contx.USER_ACTIVE_EFFECTS.RemoveRange(effectsToRemove);

                await _contx.SaveChangesAsync();
                return effectsToReturn;
            }
            catch {
                return null;
            }
        }

        

        public async Task<bool> DeactivateEffectAsync(long userId, Guid activeEffectId)
        {
            try
            {
                var effect = await _contx.USER_ACTIVE_EFFECTS.Where(e => e.UserId == userId && e.Id == activeEffectId)
                    .SingleOrDefaultAsync();

                if (effect == null)
                    return false;

                _contx.USER_ACTIVE_EFFECTS.Remove(effect);
                await _contx.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }

        public async Task<int> AddMaxUserProfileViewCount(long userId, int profileCount)
        {
            var userInfo = await _contx.SYSTEM_USERS.FindAsync(userId);
            userInfo.MaxProfileViewsCount += profileCount;
            await _contx.SaveChangesAsync();

            return userInfo.MaxProfileViewsCount;
        }

        public async Task<bool> CheckEffectIsActiveAsync(long userId, int effectId)
        {
            var effects = await GetUserActiveEffects(userId);

            if (effects == null)
                return false;

            return effects.Where(e => e.EffectId == effectId).SingleOrDefault() != null;
        }

        public async Task<bool> SendTickRequestAsync(SendTickRequest request)
        {
            try
            {
                if (request == null)
                    throw new NullReferenceException("Request is null");


                var existingRequest = await _contx.tick_requests.Where(r => r.UserId == request.UserId)
                    .SingleOrDefaultAsync();

                //Update existing request if one already exists
                if (existingRequest != null)
                {
                    existingRequest.Video = request.Video;
                    existingRequest.Circle = request.Circle;
                    existingRequest.State = (short)TickRequestStatus.Changed;

                    _contx.tick_requests.Update(existingRequest);
                    await _contx.SaveChangesAsync();
                    return true;
                }

                var model = new TickRequest
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    AdminId = null,
                    State = null,
                    Circle = request.Circle,
                    Video = request.Video
                };

                await _contx.tick_requests.AddAsync(model);
                await _contx.SaveChangesAsync();

                return true;
            }
            catch { return false; }
        }

        public async Task<bool> SwitchUserFilteringByPhotoAsync(long userId)
        {
            var userPrefs = await _contx.SYSTEM_USERS_PREFERENCES.Where(p => p.Id == userId)
                .SingleOrDefaultAsync();

            if (userPrefs == null)
                throw new NullReferenceException($"User {userId} was not found");

            userPrefs.ShouldFilterUsersWithoutRealPhoto = !userPrefs.ShouldFilterUsersWithoutRealPhoto;
            await _contx.SaveChangesAsync();

            return userPrefs.ShouldFilterUsersWithoutRealPhoto;
        }

        public async Task<bool> GetUserFilteringByPhotoStatusAsync(long userId)
        {
            var userPrefs = await _contx.SYSTEM_USERS_PREFERENCES.Where(p => p.Id == userId)
                .SingleOrDefaultAsync();

            if (userPrefs == null)
                throw new NullReferenceException($"User {userId} was not found");

            return userPrefs.ShouldFilterUsersWithoutRealPhoto;
        }

        public async Task<List<GetTestShortData>> GetTestDataByPropertyAsync(long userId, short param)
        {
            var localisation = await GetUserAppLanguage(userId);

            //Get tests user already has
            var userTests = await _contx.user_tests.Where(t => t.UserId == userId && t.TestType == param)
                .Select(t => t.TestId)
                .ToListAsync();

            return await _contx.tests
                .Where(t => t.TestType == param && !userTests.Contains(t.Id) && t.ClassLocalisationId == localisation)
                .Select(t => new GetTestShortData { Id = t.Id, Name = t.Name })
                .ToListAsync();
        }

        public Task<GetFullTestData> GetTestFullDataByIdAsync(long testId, int localisation)
        {
            return _contx.tests.Where(t => t.Id == testId && t.ClassLocalisationId == localisation)
                .Select(t => new GetFullTestData {Id = t.Id, Name = t.Name, Description = t.Description, Price = t.Price, TestType = t.TestType})
                .SingleOrDefaultAsync();
        }

        public async Task<GetUserTest> GetUserTestAsync(long userId, long testId)
        {
            var test = await _contx.user_tests.Where(t => t.UserId == userId && t.TestId == testId)
                .Include(t => t.Test)
                .SingleOrDefaultAsync();

            if (test == null)
                throw new NullReferenceException($"User {userId} does not have Test {testId}");

            return new GetUserTest(test);
        }

        public async Task<int> GetPossibleTestPassRangeAsync(long userId, long testId)
        {
            var test = await _contx.user_tests.Where(t => t.UserId == userId && t.TestId == testId)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (test == null)
                throw new NullReferenceException($"User #{userId} does not have test #{testId} available");

            //If date is passing is equal to null => test had not been passed so far
            if (test.PassedOn == null)
                return 0;

            if ((DateTime.Now - test.PassedOn).Value.Days > 90)
                return 0;

            var result = (90 - (DateTime.Now - test.PassedOn).Value.Days);
            return result;

        }

        public async Task<bool> PurchaseTestAsync(long userId, long testId, int localisation)
        {
            var balance = await GetUserWalletBalance(userId);
            var sysTest = await _contx.tests.Where(t => t.Id == testId && t.ClassLocalisationId == localisation)
                .SingleOrDefaultAsync();

            if (sysTest == null)
                throw new NullReferenceException($"Test #{testId} with localisation #{localisation} does not exist");

            if (balance.Points >= sysTest.Price)
            {
                var test = await _contx.tests.Where(t => t.Id == testId && t.ClassLocalisationId == localisation)
                    .SingleOrDefaultAsync();

                if (test == null)
                    throw new NullReferenceException($"Test #{testId} with localisation #{localisation} was not found");

                await _contx.user_tests.AddAsync(new UserTest
                {
                    UserId = userId,
                    TestId = testId,
                    TestType = test.TestType,
                    Result = 0,
                    TestClassLocalisationId = localisation
                });
                
                await _contx.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<List<GetTestShortData>> GetUserTestDataByPropertyAsync(long userId, short param)
        {
            //Get users tests
            return await _contx.user_tests.Where(t => t.UserId == userId && t.TestType == param)
                .Include(t => t.Test)
                .Select(t => new GetTestShortData { Id = t.TestId, Name = t.Test.Name })
                .ToListAsync();
        }

        public async Task<string> CheckTickRequestStatusÀsync(long userId)
        {
            var request = await _contx.tick_requests.Where(r => r.UserId == userId)
                .SingleOrDefaultAsync();

            if (request == null)
                return "";

            //TODO: Get status from localizer !
            switch (request.State)
            {
                case 1:
                    return "Added";
                case 2:
                    return "Changed";
                case 3:
                    return "In Process";
                case 4:
                    return "Declined";
                case 5:
                    return "1";
                default:
                    return "Added";
            }
        }

        public async Task<bool> SetUserFreeSearchParamAsync(long userId, bool freeStatus)
        {
            var user = await _contx.SYSTEM_USERS.FindAsync(userId);

            if (user == null)
                throw new NullReferenceException($"User #{userId} does not exist");

            user.IsFree = freeStatus;
            return freeStatus;
        }

        public async Task<bool> CheckUserHaveChosenFreeParamAsync(long userId)
        {
            var user = await _contx.SYSTEM_USERS.FindAsync(userId);

            if (user == null)
                return false;

            return true;
        }

        public async Task<bool> CheckShouldTurnOffPersonalityAsync(long userId)
        {
            var user = await _contx.SYSTEM_USERS.Where(u => u.UserId == userId)
                .Include(u => u.UserPreferences)
                .SingleOrDefaultAsync();

            if (user == null)
                throw new NullReferenceException($"User #{userId} does not exist");

            //Return false if peofileViewCountIsAlreadyMaxed
            if (user.ProfileViewsCount >= user.MaxProfileViewsCount)
                return false;

            //Return false if personality is not used
            if (!user.UserPreferences.ShouldUsePersonalityFunc)
                return false;

            return true;
        }

        public async Task<PersonalityCaps> GetUserPersonalityCapsAsync(long userId)
        {
            var stats = await _contx.USER_PERSONALITY_STATS.Where(s => s.UserId == userId)
                .SingleOrDefaultAsync();

            if (stats == null)
                throw new NullReferenceException($"Use #{userId} does not have personality stats");

            return new PersonalityCaps
            {
                CanP = stats.Personality > 0,
                CanE = stats.EmotionalIntellect > 0,
                CanR = stats.Reliability > 0,
                CanS = stats.Compassion > 0,
                CanO = stats.OpenMindedness > 0,
                CanN = stats.Agreeableness > 0,
                CanA = stats.SelfAwareness > 0,
                CanL = stats.LevelOfSense > 0,
                CanI = stats.Intellect > 0,
                CanT = stats.Nature > 0,
                CanY = stats.Creativity > 0,
            };

        }

        public async Task<bool> SwitchUserRTLanguageConsiderationAsync(long userId)
        {
            var user = await _contx.SYSTEM_USERS.Where(u => u.UserId == userId)
                .SingleOrDefaultAsync();

            if (user == null)
                throw new NullReferenceException($"User #{userId} does not exist");

            user.ShouldConsiderLanguages = !user.ShouldConsiderLanguages;

            return user.ShouldConsiderLanguages;
        }

        public async Task<bool> GetUserRTLanguageConsiderationAsync(long userId)
        {
            var user = await _contx.SYSTEM_USERS.FindAsync(userId);

            if (user == null)
                throw new NullReferenceException($"User #{userId} does not exist");

            return user.ShouldConsiderLanguages;
        }

        public async Task SetUserCurrencyAsync(long userId, short currency)
        {
            var user = await _contx.SYSTEM_USERS.FindAsync(userId);

            if (user == null)
                throw new NullReferenceException($"User #{userId} does not exist");

            user.Currency = currency;
            await _contx.SaveChangesAsync();
        }

        public async Task<bool> PurchaseEffectAsync(long userId, int effectId, int points, short currency, short count=1)
        {
            var balance = await GetUserWalletBalance(userId);

            if (balance != null)
            {
                balance.Points -= points;
                switch (effectId)
                {
                    case 5:
                        balance.SecondChances += count;
                        if (currency == (short)Currencies.Points)
                            await RegisterUserWalletPurchaseInPoints(userId, points, $"User purchase of {count} Second Chance effect for point amount {points}");
                        else
                            await RegisterUserWalletPurchaseInRealMoney(userId, points, $"User purchase of {count} Second Chance effect for real money amount {points}");
                        break;
                    case 6:
                        balance.Valentines += count;
                        if (currency == (short)Currencies.Points)
                            await RegisterUserWalletPurchaseInPoints(userId, points, $"User purchase of {count} Valentine effect for point amount {points}");
                        else
                            await RegisterUserWalletPurchaseInRealMoney(userId, points, $"User purchase of {count} Valentine effect for real money amount {points}");
                        break;
                    case 7:
                        balance.Detectors += count;
                        if (currency == (short)Currencies.Points)
                            await RegisterUserWalletPurchaseInPoints(userId, points, $"User purchase of {count} Detector effect for point amount {points}");
                        else
                            await RegisterUserWalletPurchaseInRealMoney(userId, points, $"User purchase of {count} Detector effect for real money amount {points}");
                        break;
                    case 8:
                        balance.Nullifiers += count;
                        if (currency == (short)Currencies.Points)
                            await RegisterUserWalletPurchaseInPoints(userId, points, $"User purchase of {count} Nullifier effect for point amount {points}");
                        else
                            await RegisterUserWalletPurchaseInRealMoney(userId, points, $"User purchase of {count} Nullifier effect for real money amount {points}");
                        break;
                    case 9:
                        balance.CardDecksMini += count;
                        if (currency == (short)Currencies.Points)
                            await RegisterUserWalletPurchaseInPoints(userId, points, $"User purchase of {count} Card Deck Mini effect for point amount {points}");
                        else
                            await RegisterUserWalletPurchaseInRealMoney(userId, points, $"User purchase of {count} Second Card Deck Mini for real money amount {points}");
                        break;
                    case 10:
                        balance.CardDecksPlatinum += count;
                        if (currency == (short)Currencies.Points)
                            await RegisterUserWalletPurchaseInPoints(userId, points, $"User purchase of {count} Card Deck Platinum effect for point amount {points}");
                        else
                            await RegisterUserWalletPurchaseInRealMoney(userId, points, $"User purchase of {count} Card Deck Platinum effect for real money amount {points}");
                        break;
                    default:
                        break;

                }
                return true;
            }

            return false;
        }

        public async Task<GetUserData> GetRequestSenderAsync(Guid requestId)
        {
            var senderId = await _contx.USER_NOTIFICATIONS.Where(r => r.Id == requestId).Select(r => r.UserId)
                .SingleOrDefaultAsync();

            var sender = await _contx.SYSTEM_USERS.Where(u => u.UserId == senderId)
                .Include(u => u.UserBaseInfo)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            var bonus = "";

            if (sender.IsIdentityConfirmed)
                bonus += $"✔️\n\n";
            if (sender.HasPremium && sender.Nickname != "")
                bonus += $"<b>{sender.Nickname}</b>\n";

            return new GetUserData(sender, bonus);
        }

        public async Task<bool> PurchasePersonalityPointsAsync(long userId, int points, short currency, short count = 1)
        {
            try
            {
                var balance = await GetUserWalletBalance(userId);

                if (currency == (short)Currencies.Points)
                {
                    balance.Points -= points;
                    balance.PersonalityPoints += count;
                    await RegisterUserWalletPurchaseInPoints(userId, points, $"User purchase of {count} Personality Points effect for point amount {points}");
                }
                else
                {
                    balance.Points -= points;
                    balance.PersonalityPoints += count;
                    await RegisterUserWalletPurchaseInRealMoney(userId, points, $"User purchase of {count} Personality Points effect for real money amount {points}");
                }
                await _contx.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CheckPromoIsCorrectAsync(long userId, string promoText, bool isActivatedBeforeRegistration)
        {
            var promo = await _contx.promo_codes.Where(p => p.Promo == promoText && p.UserdOnlyInRegistration == isActivatedBeforeRegistration)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (promo == null)
                return false;

            var userBalance = await _contx.USER_WALLET_BALANCES.FindAsync(userId);

            userBalance.Points += promo.Points;
            userBalance.PersonalityPoints += promo.PersonalityPoints;
            userBalance.SecondChances += promo.SecondChance;
            userBalance.Valentines += promo.TheValentine;
            userBalance.Detectors += promo.TheDetector;
            userBalance.Nullifiers += promo.Nullifier;
            userBalance.CardDecksMini += promo.CardDeckMini;
            userBalance.CardDecksPlatinum += promo.CardDeckPlatinum;

            _contx.USER_WALLET_BALANCES.Update(userBalance);
            await _contx.SaveChangesAsync();
            return true;
        }

        public async Task<bool> GetUserIncreasedFamiliarityAsync(long userId)
        {
            var user = await _contx.SYSTEM_USERS.FindAsync(userId);

            if (user == null)
                throw new NullReferenceException($"User {userId} does not exist !");

            return user.IncreasedFamiliarity;
        }

        public async Task<bool> SwitchIncreasedFamiliarityAsync(long userId)
        {
            var user = await _contx.SYSTEM_USERS.FindAsync(userId);

            if (user == null)
                throw new NullReferenceException($"User {userId} does not exist !");

            user.IncreasedFamiliarity = !user.IncreasedFamiliarity;
            await _contx.SaveChangesAsync();

            return user.IncreasedFamiliarity;
        }
    }
}
