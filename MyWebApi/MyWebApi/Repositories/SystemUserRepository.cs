using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using MyWebApi.Data;
using MyWebApi.Entities.AchievementEntities;
using MyWebApi.Entities.AdminEntities;
using MyWebApi.Entities.AdventureEntities;
using MyWebApi.Entities.DailyTaskEntities;
using MyWebApi.Entities.EffectEntities;
using MyWebApi.Entities.LocationEntities;
using MyWebApi.Entities.ReasonEntities;
using MyWebApi.Entities.ReportEntities;
using MyWebApi.Entities.SecondaryEntities;
using MyWebApi.Entities.SponsorEntities;
using MyWebApi.Entities.TestEntities;
using MyWebApi.Entities.UserActionEntities;
using MyWebApi.Entities.UserInfoEntities;
using MyWebApi.Enums;
using MyWebApi.Interfaces;
using MyWebApi.Utilities;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static MyWebApi.Enums.SystemEnums;

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
                    country = (await _contx.countries.Where(c => c.Id == location.CountryId && c.ClassLocalisationId == location.CountryClassLocalisationId).Select(c => c.CountryName).SingleOrDefaultAsync());
                    city = (await _contx.cities.Where(c => c.Id == location.CountryId && c.CountryClassLocalisationId == location.CityCountryClassLocalisationId).Select(c => c.CityName).SingleOrDefaultAsync());
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
                            await GrantPremiumToUser(invitor.UserId, 0, 30, (short)Currencies.Points);
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

                    //User is instantly liked by an invitor if he approves it
                    if (invitor.IncreasedFamiliarity)
                        await RegisterUserRequestAsync(new UserNotification { UserId = invitor.UserId, UserId1 = model.UserId, IsLikedBack = false });
                    //Invitor is notified about referential registration
                    await NotifyUserAboutReferentialRegistrationAsync(invitor.UserId, model.UserId);
                }

                if (await CheckUserHasTasksInSectionAsync(model.UserId, (int)Sections.Registration))
                {
                    //TODO find and topup user's task progress
                }

                //Enter promo if it was supplied
                if (model.EnteredPromoCodes != null)
                    await CheckPromoIsCorrectAsync(model.UserId, model.EnteredPromoCodes, false);
                else
                    model.EnteredPromoCodes = "";

                //Add Starting test pack
                //TODO: Add more tests here
                await PurchaseTestAsync(model.UserId, 1, dataModel.LanguageId);
                //await PurchaseTestAsync(model.UserId, 3, dataModel.LanguageId);

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
                //Actualize premium information
                await CheckUserHasPremiumAsync(id);

                return await _contx.SYSTEM_USERS.Where(u => u.UserId == id)
                    .Include(u => u.UserBaseInfo)
                    .Include(u => u.UserDataInfo).ThenInclude(s => s.Location)
                    .Include(u => u.UserDataInfo).ThenInclude(s => s.Reason)
                    .Include(u => u.UserPreferences)
                    .Include(u => u.UserBlackList)
                    .SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(null, ex.Message, (int)Sections.Neutral);
                return null;
            }
        }

        public async Task<List<GetUserData>> GetUsersAsync(long userId, bool isRepeated=false, bool isFreeSearch = false)
        {
            try
            {
                const byte miminalProfileCount = 5;
                var returnData = new List<GetUserData>();

                var currentUser = await GetUserInfoAsync(userId);

                //Check if user STILL has premium
                await CheckUserHasPremiumAsync(currentUser.UserId);

                var currentUserEncounters = await GetUserEncounters(userId, (int)Sections.Familiator); //I am not sure if it is 2 or 3 section

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
                    data = data.Where(u => u.UserBaseInfo.IsPhotoReal)
                        .ToList();
                }    
                
                if (currentUser.UserDataInfo.Location.CountryId != null)
                {
                    data = data.Where(u => u.UserDataInfo.Location.CountryId != null)
                            .Where(u => u.UserPreferences.UserLocationPreferences.Contains((int)currentUser.UserDataInfo.Location.CountryId))
                            .Where(u => currentUser.UserPreferences.UserLocationPreferences.Contains((int)u.UserDataInfo.Location.CountryId))
                            .ToList();
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

                //Don't check genders user does NOT have gender prederences
                if (currentUser.UserPreferences.UserGenderPrefs != 3)
                {
                    data = data.Where(u => u.UserDataInfo.UserGender == currentUser.UserPreferences.UserGenderPrefs)
                    .Where(u => currentUser.UserPreferences.UserGenderPrefs == u.UserDataInfo.UserGender)
                    .ToList();
                }

                //If user wants to find only people who are free today
                if (isFreeSearch)
                    data = data.Where(u => u.IsFree != null && (bool)u.IsFree).ToList();


                //If user uses PERSONALITY functionality
                if (currentUser.UserPreferences.ShouldUsePersonalityFunc)
                {

                    for (int i = 0; i < data.Count; i++)
                    {
                        returnData.Add(await GetPersonalityMatchResult(userId, currentUser, data[i], isRepeated));
                    }
                }
                else
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        var u = data[i];
                        var user = new GetUserData(u);
                        var bonus = "";

                        //Add comment if user wants it
                        if (currentUser.ShouldComment)
                            user.Comment = await GetRandomHintAsync(currentUser.UserDataInfo.LanguageId, HintType.Search);

                        if (u.HasPremium && u.Nickname != "")
                            bonus += $"<b>{u.Nickname}</b>\n";

                        if (u.IdentityType == IdentityConfirmationType.Partial)
                             bonus += $"☑️☑️☑️\n\n";
                        else if (u.IdentityType == IdentityConfirmationType.Full)
                             bonus += $"✅✅✅\n\n";


                        user.AddDescriptionBonus(bonus);
                        returnData.Add(user);
                    }
                }

                //Check if method wasnt already repeated
                if (!isRepeated)
                {
                    //Check if users count is less than the limit
                    if (returnData.Count <= miminalProfileCount)
                    {
                        returnData = await GetUsersAsync(userId, isRepeated: true, isFreeSearch:isFreeSearch);
                    }

                    //Add user trust exp only if method was not repeated
                    await AddUserTrustProgressAsync(userId, 0.000003);

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

        private async Task<GetUserData> GetPersonalityMatchResult(long userId, User currentUser, User managedUser, bool isRepeated)
        {
            var returnUser = new GetUserData(managedUser);

            var userActiveEffects = await GetUserActiveEffects(userId);
            var deviation = 0.15;
            var minDeviation = 0.05;

            var currentValueMax = 0d;
            var currentValueMin = 0d;

            var valentineBonus = 1d;

            var importantMatches = 0;
            var secondaryMatches = 0;
            var matchedBy = "";

            var hasActiveValentine = userActiveEffects.Where(e => e.EffectId == (int)Currencies.TheValentine).SingleOrDefault() != null;

            var userHasDetectorOn = userActiveEffects.Where(e => e.EffectId == (int)Currencies.TheDetector).SingleOrDefault() != null;

            if (hasActiveValentine)
                valentineBonus = 2;

            if (isRepeated)
            {
                deviation *= 1.5;
                minDeviation *= 3.2;
            }

            var userPoints = await _contx.USER_PERSONALITY_POINTS.Where(p => p.UserId == currentUser.UserId)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            var userStats = await _contx.USER_PERSONALITY_STATS.Where(s => s.UserId == currentUser.UserId)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            //Enhanse users PP if condition is met
            if(currentUser.ShouldEnhance)
            {
                userPoints.PersonalityPercentage = 0.1;
                userPoints.EmotionalIntellectPercentage = 0.1;
                userPoints.ReliabilityPercentage = 0.1;
                userPoints.CompassionPercentage = 0.1;
                userPoints.OpenMindednessPercentage = 0.1;
                userPoints.SelfAwarenessPercentage = 0.1;
                userPoints.AgreeablenessPercentage = 0.1;
                userPoints.LevelOfSensePercentage = 0.1;
                userPoints.IntellectPercentage = 0.1;
                userPoints.NaturePercentage = 0.1;
                userPoints.CreativityPercentage = 0.1;
            }

            var important = await userPoints.GetImportantParams();

            //Pass if user does not uses personality
            if (!managedUser.UserPreferences.ShouldUsePersonalityFunc)
                return returnUser;

            var user2Points = await _contx.USER_PERSONALITY_POINTS.Where(p => p.UserId == managedUser.UserId)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            var user2Stats = await _contx.USER_PERSONALITY_STATS.Where(s => s.UserId == managedUser.UserId)
                .AsNoTracking()
                .SingleOrDefaultAsync();


            //Turns off the parameter if it is 0
            if (userPoints.Personality > 0 && userStats.Personality > 0 && user2Points.Personality > 0 && user2Stats.Personality > 0)
            {
                //TODO: create its own deviation variable depending on the number of personalities (It is likely to be grater than the normal one)
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
                        matchedBy += "[P] ";
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
                        matchedBy += "[E] ";
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
                        matchedBy += "[R] ";
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
                        matchedBy += "[S] ";
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
                        matchedBy += "[O] ";
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
                        matchedBy += "[N] ";
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
                        matchedBy += "[A] ";
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
                        matchedBy += "[L] ";
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
                        matchedBy += "[I] ";
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
                        matchedBy += "[T] ";
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
                        matchedBy += "[Y] ";
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
                return returnUser;
            };

            var bonus = "";

            if (userHasDetectorOn)
                bonus += $"<b>PERSONALITY match!</b>\n<b>{matchedBy}</b>";
            else
                bonus += "<b>PERSONALITY match!</b>";

            returnUser.AddDescriptionBonus(bonus);

            return returnUser;
        }

        public async Task<Country> GetCountryAsync(long id)
        {
            try
            {
                var c = await _contx.countries.Include(c => c.Cities).SingleAsync(c => c.Id == id);
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

        public async Task<Guid> AddUserReportAsync(SendUserReport request)
        {
            try
            {
                var reportedUser = await _contx.SYSTEM_USERS.Where(u => u.UserId == request.ReportedUser)
                    .FirstOrDefaultAsync();

                reportedUser.ReportCount++;

                //Ban user if dailly report count is too high
                if (reportedUser.ReportCount >= 5)
                {
                    reportedUser.IsBanned = true;
                    reportedUser.BanDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                }

                var report = new Report
                {
                    Id = Guid.NewGuid(),
                    UserBaseInfoId = request.Sender,
                    UserBaseInfoId1 = request.ReportedUser,
                    Text = request.Text,
                    Reason = request.Reason,
                    InsertedUtc = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                };

                await _contx.USER_REPORTS.AddAsync(report);

                await _contx.SaveChangesAsync();

                return report.Id;
            }
            catch {return Guid.Empty;}
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

        public async Task<Report> GetSingleUserReportByIdAsync(Guid id)
        {
            try
            {
                return await _contx.USER_REPORTS.Where(r => r.Id == id)
                    .Include(r => r.User)
                    .Include(r => r.Sender)
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

        public List<GetReportReason> GetReportReasonsAsync()
        {
            var reasons = new List<GetReportReason>();

            foreach (var reason in Enum.GetValues(typeof(ReportReason)))
            {
                reasons.Add(new GetReportReason
                {
                    Id = (short)reason,
                    Name = EnumLocalizer.GetLocalizedValue((ReportReason)reason)
                });
            }

            return reasons;
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
                    user.BanDate = null;

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
                    Section = (Sections)achievement.Achievement.SectionId,
                    Severity = Severities.Minor,
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
                    startingUser.IsUpdated = false;
                    startingUser.ShouldEnhance = false;

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

        public async Task<bool> CheckUserHasPremiumAsync(long userId)
        {
            try
            {
                var timeNow = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

                var user = await _contx.SYSTEM_USERS.Where(u => u.UserId == userId).SingleOrDefaultAsync();
                
                if (user != null)
                {
                    if ((user.HasPremium && user.PremiumExpirationDate < timeNow) || (user.HasPremium && user.PremiumExpirationDate == null))
                    {
                        user.HasPremium = false;
                        user.PremiumDuration = null;
                        user.PremiumExpirationDate = null;
                        //TODO: Notify user that his premium access has expired
                    }
                    else if (user.PremiumExpirationDate > timeNow)
                        user.HasPremium = true;


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
                    //if (user.PremiumExpirationDate != null)
                    //    return user.PremiumExpirationDate.Value;
                    return user.PremiumExpirationDate.Value;
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
                var premiumFutureExpirationDate = DateTime.SpecifyKind(DateTime.Now.AddDays(dayDuration), DateTimeKind.Utc);

                var user = await _contx.SYSTEM_USERS
                    .Where(u => u.UserId == userId)
                    .SingleOrDefaultAsync();

                user.HasPremium = true;
                user.BonusIndex = 2;

                if (user.PremiumDuration != null)
                {
                    user.PremiumDuration += (short)dayDuration;
                }
                else
                {
                    user.PremiumDuration = (short)dayDuration;
                }

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
                    user.PremiumExpirationDate = user.PremiumExpirationDate.Value.AddDays(dayDuration);

                _contx.Update(user);
                await _contx.SaveChangesAsync();

                await AddUserNotificationAsync(new UserNotification { UserId1 = user.UserId, IsLikedBack = false, Severity = Severities.Moderate, Section = Sections.Neutral, Description = $"You have been granted premium access. Enjoy your benefits :)\nPremium expiration {user.PremiumExpirationDate.Value.ToString("dd.MM.yyyy")}" });

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
                    country = await _contx.countries
                        .Where(c => c.Id == location.CountryId && c.ClassLocalisationId == location.CountryClassLocalisationId)
                        .Select(c => c.CountryName)
                        .SingleOrDefaultAsync();
                    city = await _contx.cities
                        .Where(c => c.Id == location.CityId && c.CountryClassLocalisationId == location.CityCountryClassLocalisationId)
                        .Select(c => c.CityName)
                        .SingleOrDefaultAsync(); ;
                }

                model.UserRawDescription = user.UserRawDescription;
                model.UserDescription = user.GenerateUserDescription(user.UserRealName, data.UserAge, country, city, user.UserRawDescription);

                //Reactivate user tick request if user photo was changed
                if (model.UserMedia != user.UserMedia)
                {
                    model.UserMedia = user.UserMedia;
                    model.IsMediaPhoto = user.IsMediaPhoto;
                    await ReactivateTickRequest(user.Id);
                }

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

                if (user == null)
                    return false;

                return user.IsBusy;
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(userId, ex.Message, (int)Sections.Neutral);
                return false;
            }
        }

        public async Task<SwitchBusyStatusResponse> SwhitchUserBusyStatus(long userId, int sectionId)
        {
            var user = await _contx.SYSTEM_USERS.Where(u => u.UserId == userId)
                .Include(u => u.UserDataInfo)
                .SingleOrDefaultAsync();

            if  (user != null)
            {
                var hint = "";

                user.IsBusy = !user.IsBusy;
                user.IsUpdated = false;

                _contx.Update(user);
                await _contx.SaveChangesAsync();

                if (!user.IsBusy) // Negation <= Was busy before update
                {
                    return new SwitchBusyStatusResponse
                    {
                        Status = SwitchBusyStatusResult.IsBusy
                    };
                }

                if (user.IsDeleted)
                {
                    return new SwitchBusyStatusResponse
                    {
                        Status = SwitchBusyStatusResult.IsDeleted,
                    };
                }

                if (user.ShouldSendHints)
                    hint = await GetRandomHintAsync(user.UserDataInfo.LanguageId, null);


                return new SwitchBusyStatusResponse
                {
                    Status = SwitchBusyStatusResult.Success,
                    Comment = hint,
                    HasVisited = await CheckUserHasVisitedSection(userId, sectionId)
                };
            }
            return new SwitchBusyStatusResponse
            {
                Status = SwitchBusyStatusResult.DoesNotExist,
                HasVisited = false
            };
        }

        public async Task<List<UserNotification>> GetUserRequests(long userId)
        {

            return await _contx.USER_NOTIFICATIONS
                .Where(r => r.UserId1 == userId && r.UserId != null)
                .Where(r => r.Section == Sections.Familiator || r.Section == Sections.Requester)
                .ToListAsync();

        }

        public async Task<UserNotification> GetUserRequest(Guid requestId)
        {
            try
            {
                return await _contx.USER_NOTIFICATIONS
                    .Where(r => r.Id == requestId)
                    .Where(r => r.Section == Sections.Familiator || r.Section == Sections.Requester)
                    .SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                await LogAdminErrorAsync(0, ex.Message, (int)Sections.Requester);
                return null;
            }
        }

        public async Task<string> RegisterUserRequestAsync(UserNotification request)
        {
            request.Severity = Severities.Moderate;
            var returnMessage = "";

            if (request.IsLikedBack)
            {
                request.Section = Sections.Requester;

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
                request.Section = Sections.Familiator;

            await RegisterUserEncounter(new Encounter { UserId = (long)request.UserId, EncounteredUserId = request.UserId1, SectionId = (int)Sections.Requester });

            var id = await AddUserNotificationAsync(request);

            return returnMessage;
            
        }

        //TODO: Make more informative and interesting
        public async Task<string> DeclineRequestAsync(long user1, long user2)
        {
            var sim = await GetSimilarityBetweenUsersAsync(user1, user2);

            //Encounter is not registered anywhere but here in that case
            await RegisterUserEncounter(new Encounter
            {
                UserId = user1,
                EncounteredUserId = user2,
                SectionId = (short)Sections.Familiator,
                EncounterDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
            });

            switch (sim.SimilarBy.Count)
            {
                case 3:
                    return "It is such a shame, you were a perfect match!";
                case 2:
                    return "Oh... You two were so alike :(";
                case 1:
                    return "It's a shame";
                default:
                    return null;
            }


        }

        public async Task<byte> DeleteUserRequests(long userId)
        {
            try
            {
                var requests = await _contx.USER_NOTIFICATIONS
                    .Where(r => r.UserId1 == userId)
                    .Where(r => r.Section == Sections.Familiator || r.Section == Sections.Requester)
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
                    .Where(r => r.Section == Sections.Familiator || r.Section == Sections.Requester)
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
                    .Where(r => r.Section == Sections.Familiator || r.Section == Sections.Requester)
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

                var user = await _contx.SYSTEM_USERS.FindAsync(model.UserId);

                if (model.SectionId == (int)Sections.Familiator || model.SectionId == (int)Sections.Requester)
                {
                    user.ProfileViewsCount++;

                    if (user.ProfileViewsCount == 15)
                        await TopUpUserWalletPointsBalance(user.UserId, 9, "User has viewed 15 profiles");
                    else if (user.ProfileViewsCount == 30)
                        await TopUpUserWalletPointsBalance(user.UserId, 15, "User has viewed 30 profiles");
                    else if (user.ProfileViewsCount == 50)
                        await TopUpUserWalletPointsBalance(user.UserId, 22, "User has viewed 50 profiles");
                }
                else if (model.SectionId == (int)Sections.RT)
                    user.MaxRTViewsCount++;

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
                .Where(r => r.Section == Sections.Requester || r.Section == Sections.Familiator)
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
                    QRCode = await GetQRCode(userId)
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
                    Section = Sections.Registration,
                    Severity = Severities.Moderate
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
                await AddUserNotificationAsync(new UserNotification {UserId1=userId, Severity=Severities.Urgent, Section=Sections.Neutral, Description="Your premium access has expired"});
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
                .Where(n => n.UserId1 == userId && n.Section != Sections.Familiator && n.Section != Sections.Requester)
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

        public async Task<List<string>> GetRandomAchievements(long userId)
        {
            var achievents = await _contx.USER_ACHIEVEMENTS
                .Where(a => a.UserBaseInfoId == userId)
                .Where(a => !a.IsAcquired)
                .Select(a => $"{a.Achievement.Name}\n{a.Achievement.Description}\n\n{a.Achievement.ConditionValue} / {a.Achievement.Value}")
                .ToListAsync();

            //Shuffle achievement list
            achievents = achievents.OrderBy(a => Guid.NewGuid()).ToList();

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
                Severity = Severities.Moderate,
                Description = task.AcquireMessage,
                Section = (Sections)task.DailyTask.SectionId
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

            return 450; //TODO: Think if value should be hardcoded 
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

                var result = await RecalculateUserStats(model);

                //Break if test result wasnt saved
                if (!await RegisterTestPassingAsync(model, result.TestResult))
                    return false;

                _contx.USER_PERSONALITY_STATS.Update(result.Stats);
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

        private async Task<RecalculationResult> RecalculateUserStats(TestPayload model)
        {
            var devider = 1;
            var result = 0;
            var userStats = await _contx.USER_PERSONALITY_STATS.Where(s => s.UserId == model.UserId)
                .SingleOrDefaultAsync();

            //Create user stats if they werent created before
            if (userStats == null)
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

                result = model.Personality;
                userStats.Personality = (userStats.Personality + model.Personality) / devider;
                //Return the devider to its normal state
                devider = 1;
            }
            if (model.EmotionalIntellect != 0)
            {
                if (userStats.EmotionalIntellect != 0)
                    devider = 2;

                result = model.EmotionalIntellect;
                userStats.EmotionalIntellect = (userStats.EmotionalIntellect + model.EmotionalIntellect) / devider;
                devider = 1;
            }
            if (model.Reliability != 0)
            {
                if (userStats.Reliability != 0)
                    devider = 2;

                result = model.Reliability;
                userStats.Reliability = (userStats.Reliability + model.Reliability) / devider;
                devider = 1;
            }
            if (model.Compassion != 0)
            {
                if (userStats.Compassion != 0)
                    devider = 2;

                result = model.Compassion;
                userStats.Compassion = (userStats.Compassion + model.Compassion) / devider;
                devider = 1;
            }
                
            if (model.OpenMindedness != 0)
            {
                if (userStats.OpenMindedness != 0)
                    devider = 2;

                result = model.OpenMindedness;
                userStats.OpenMindedness = (userStats.OpenMindedness + model.OpenMindedness) / devider;
                devider = 1;
            }
            if (model.Agreeableness != 0)
            {
                if (userStats.Agreeableness != 0)
                    devider = 2;

                result = model.Agreeableness;
                userStats.Agreeableness = (userStats.Agreeableness + model.Agreeableness) / devider;
                devider = 1;
            }
            if (model.SelfAwareness != 0)
            {
                if (userStats.SelfAwareness != 0)
                    devider = 2;

                result = model.SelfAwareness;
                userStats.SelfAwareness = (userStats.SelfAwareness + model.SelfAwareness) / devider;
                devider = 1;
            }
                
            if (model.LevelOfSense != 0)
            {
                if (userStats.LevelOfSense != 0)
                    devider = 2;

                result = model.LevelOfSense;
                userStats.LevelOfSense = (userStats.LevelOfSense + model.LevelOfSense) / devider;
                devider = 1;
            }
            if (model.Intellect != 0)
            {
                if (userStats.Intellect != 0)
                    devider = 2;

                result = model.Intellect;
                userStats.Intellect = (userStats.Intellect + model.Intellect) / devider;
                devider = 1;
            }
            if (model.Nature != 0)
            {
                if (userStats.Nature != 0)
                    devider = 2;

                result = model.Nature;
                userStats.Nature = (userStats.Nature + model.Nature) / devider;
                devider = 1;
            }
            if (model.Creativity != 0)
            {
                if (userStats.Creativity != 0)
                    devider = 2;

                result = model.Creativity;
                userStats.Creativity = (userStats.Creativity + model.Creativity) / devider;
                devider = 1;
            }

            return new RecalculationResult { Stats = userStats, TestResult = result};
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

        public async Task<bool> RegisterTestPassingAsync(TestPayload model, int testResult)
        {
            try
            {
                var userTest = await _contx.user_tests.Where(t => t.UserId == model.UserId && t.TestId == model.TestId)
                    .SingleOrDefaultAsync();

                //Give user 1 PP for passing the test for the first time
                if (userTest.PassedOn == null)
                {
                    var userBalance = await GetUserWalletBalance(model.UserId);
                    userBalance.PersonalityPoints++;
                    await _contx.SaveChangesAsync();
                }

                userTest.PassedOn = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                userTest.Result = testResult;
                userTest.Tags = model.Tags;

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
                var userTags = await _contx.user_tags.Where(t => t.UserId == model.UserId)
                    .ToListAsync();

                var newTags = UserTag.CreateTagList(model.UserId, model.RawTags, " ", TagType.Tags);

                _contx.user_tags.RemoveRange(userTags);
                    
                await _contx.user_tags.AddRangeAsync(newTags);
                    
                await _contx.SaveChangesAsync();
                return true;
            }
            catch (NullReferenceException ) 
            { return false; }
        }

        public async Task<List<UserTag>> GetTags(long userId)
        {
            try
            {
                return await _contx.user_tags.Where(t => t.UserId == userId && t.TagType == TagType.Tags)
                    .ToListAsync();
            }
            catch (NullReferenceException)
            { return null; }
        }

        public async Task<GetUserData> GetUserListByTagsAsync(GetUserByTags model)
        {
            var currentUser = await GetUserInfoAsync(model.UserId);
            var hasActiveDetector = await CheckEffectIsActiveAsync(currentUser.UserId, (int)Currencies.TheDetector);

            //User has already reached his limit;
            if (currentUser.TagSearchesCount > currentUser.MaxTagSearchCount)
                return null;

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
                data = data.Where(u => u.UserBaseInfo.IsPhotoReal)
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

            var usersTags = await GetUsersTagsAsync(data.Select(d => d.UserId).ToList(), TagType.Tags);

            var users = new List<User>();

            foreach (var tags in usersTags.Values)
            {
                var tagList = tags.Select(t => t.Tag).ToList();

                if (tagList.Intersect(model.Tags).Count() >= 1)
                    users.Add(data.Where(d => d.UserId == tags.FirstOrDefault().UserId).FirstOrDefault());
            }

            if (users.Count == 0)
                return null;

            //Shuffle users randomly
            var user = users.OrderBy(u => Guid.NewGuid())
                .FirstOrDefault();

            var returnUser = new GetUserData(user);

            if (currentUser.UserPreferences.ShouldUsePersonalityFunc)
                returnUser = await GetPersonalityMatchResult(model.UserId, currentUser, user, false);

            if (currentUser.ShouldComment)
                returnUser.Comment = await GetRandomHintAsync(currentUser.UserDataInfo.LanguageId, HintType.Search);

            if (user.HasPremium && user.Nickname != null)
                returnUser.UserBaseInfo.UserDescription = $"<b>{user.Nickname}</b>\n\n{user.UserBaseInfo.UserDescription}";

            if (user.IdentityType == IdentityConfirmationType.Partial)
                returnUser.AddDescriptionBonus($"☑️☑️☑️\n\n");

            else if (user.IdentityType == IdentityConfirmationType.Full)
                returnUser.AddDescriptionBonus($"✅✅✅\n\n");

            //Show tags if user has detector activated
            if (hasActiveDetector)
                user.UserBaseInfo.UserDescription += String.Join(" ", usersTags[user.UserId]);

            return returnUser;
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
                if (await CheckUserHasPremiumAsync(userId))
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

            if (await CheckUserHasPremiumAsync(userId))
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
                            await ActivateToggleEffectAsync(userId, effectId);
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
                                if (userBalance.Valentines > 0)
                                {
                                    userBalance.Valentines--;
                                    effect = new TheValentine(userId);
                                    break;
                                }
                                return null;
                            }
                            return null;
                        }
                        return null;
                    case 7:
                        //Already checked on the Frontend

                        //if (!(bool)await CheckUserUsesPersonality(userId))
                        //    return null;

                        if (userBalance.Detectors > 0)
                        {
                            userBalance.Detectors--;
                            effect = new TheDetector(userId);
                            break;
                        }
                        return null;
                    default:
                        return null;
                }

                var activeEffect = await _contx.USER_ACTIVE_EFFECTS.Where(e => e.EffectId == effectId && e.UserId == userId)
                    .SingleOrDefaultAsync();

                if (activeEffect == null)
                    await _contx.USER_ACTIVE_EFFECTS.AddAsync(effect);
                else
                    effect.ExpirationTime = effect.ExpirationTime;

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
                        if (userBalance.SecondChances > 0)
                        {
                            userBalance.SecondChances--;
                            await RegisterUserRequestAsync(new UserNotification
                            {
                                UserId = userId,
                                UserId1 = (long)user2Id,
                                IsLikedBack = false,
                                Description = description,
                                Section = Sections.Familiator,
                                Severity = Severities.Moderate
                            });
                            _contx.Update(userBalance);
                            await _contx.SaveChangesAsync();
                            return true;
                        }
                        return false;
                    case 8:
                        if (userBalance.Nullifiers > 0)
                        {
                            userBalance.Nullifiers --;
                            _contx.Update(userBalance);
                            await _contx.SaveChangesAsync();
                            return true;
                        }
                        return false;
                    case 9:
                        if (userBalance.CardDecksMini > 0)
                        {
                            userBalance.CardDecksMini--;
                            await AddMaxUserProfileViewCountAsync(userId, 20);
                            await AddMaxRTProfileViewCountAsync(userId, 20);
                            _contx.Update(userBalance);
                            await _contx.SaveChangesAsync();
                            return true;
                        }
                        return false;
                    case 10:
                        if (userBalance.CardDecksPlatinum > 0)
                        {
                            userBalance.CardDecksPlatinum--;
                            await AddMaxUserProfileViewCountAsync(userId, 50);
                            await AddMaxRTProfileViewCountAsync(userId, 50);
                            _contx.Update(userBalance);
                            await _contx.SaveChangesAsync();
                            return true;
                        }
                        return false;
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

        private async Task<int> AddMaxUserProfileViewCountAsync(long userId, int profileCount)
        {
            var userInfo = await _contx.SYSTEM_USERS.FindAsync(userId);
            userInfo.ProfileViewsCount -= profileCount;
            await _contx.SaveChangesAsync();

            return userInfo.MaxProfileViewsCount;
        }

        private async Task<int> AddMaxRTProfileViewCountAsync(long userId, int increment)
        {
            var userInfo = await _contx.SYSTEM_USERS.FindAsync(userId);
            userInfo.RTViewsCount -= increment;
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
                    existingRequest.Photo = request.Photo;
                    existingRequest.Video = request.Video;
                    existingRequest.Circle = request.Circle;
                    existingRequest.Gesture = request.Gesture;
                    existingRequest.Type = request.Type;
                    existingRequest.State = TickRequestStatus.Changed;

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
                    Photo = request.Photo,
                    Video = request.Video,
                    Circle = request.Circle,
                    Gesture = request.Gesture,
                    Type = request.Type
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
                .Include(t => t.Test)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (test == null)
                throw new NullReferenceException($"User #{userId} does not have test #{testId} available");

            //If date of passing is equal to null => test had not been passed so far
            if (test.PassedOn == null)
                return 0;

            //Every test has its own passing range. If date of passing is out of it => Test can be passed again
            if ((DateTime.Now - test.PassedOn).Value.Days > test.Test.CanBePassedInDays)
                return 0;

            //Get number of days in which this test can be passed again
            var result = (test.Test.CanBePassedInDays - (DateTime.Now - test.PassedOn).Value.Days);
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
                case TickRequestStatus.Added:
                    return "Added";
                case TickRequestStatus.Changed:
                    return "Changed";
                case TickRequestStatus.InProcess:
                    return "In Process";
                case TickRequestStatus.Declined:
                    return "Declined";
                case TickRequestStatus.Accepted:
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
            await _contx.SaveChangesAsync();

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

            await _contx.SaveChangesAsync();

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

            if (sender.IdentityType == IdentityConfirmationType.Partial)
                bonus += $"☑️☑️☑️\n\n";
            else if (sender.IdentityType == IdentityConfirmationType.Full)
                bonus += $"✅✅✅\n\n";

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

            // There is no way the promo can be applied before Registration, due to the absence of user data.
            // Thus we are only checking its presence
            if (isActivatedBeforeRegistration)
                return promo != null;

            //Enter promo right await if it had not been inputed before registration
            return await EnterPromo(userId, promo);
        }

        private async Task<bool> EnterPromo(long userId, PromoCode promo)
        {
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

        public async Task<bool> AddUserCommercialVector(long userId, string tagString)
        {
            var tags = tagString.Replace("#", "").Trim().Replace(" ", "").Split(",");

            foreach (var tag in tags)
            {
                await _contx.user_tags.AddAsync(new UserTag
                {
                    UserId = userId, 
                    Tag = tag,
                    TagType = TagType.Interests
                });

            }

            await _contx.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SwitchUserFreeSearchParamAsync(long userId)
        {
            var user = await _contx.SYSTEM_USERS.FindAsync(userId);

            if (user == null)
                throw new NullReferenceException($"User #{userId} does not exist");

            //null is the same thing as false in that case
            if (user.IsFree == null)
            {
                user.IsFree = true;
                await _contx.SaveChangesAsync();

                return true;
            }

            user.IsFree = !user.IsFree;
            await _contx.SaveChangesAsync();

            return (bool)user.IsFree;
        }

        public async Task<string> RegisterAdventureAsync(ManageAdventure model)
        {
            var adventure = new Adventure
            {
                Id = Guid.NewGuid(),
                UserId = model.UserId,
                Name = model.Name,
                Address = model.Address,
                Application = model.Application,
                AttendeesDescription = model.AttendeesDescription,
                CityId = model.CityId,
                CountryId = model.CountryId,
                Date = model.Date,
                Time = model.Time,
                Description = model.Description,
                Duration = model.Duration,
                Experience = model.Experience,
                UnwantedAttendeesDescription = model.UnwantedAttendeesDescription,
                Gratitude = model.Gratitude,
                IsMediaPhoto = model.IsMediaPhoto,
                Media = model.Media,
                IsAutoReplyText = model.IsAutoReplyText,
                AutoReply = model.AutoReply,
                IsOffline = model.IsOffline,
                IsAwaiting = model.IsAwaiting,
                UniqueLink = Guid.NewGuid().ToString("N").Substring(0, 7).ToUpper(),
                Status = AdventureStatus.New
            };

            await _contx.adventures.AddAsync(adventure);
            await _contx.SaveChangesAsync();

            return adventure.UniqueLink;
        }

        public async Task ChangeAdventureAsync(ManageAdventure model)
        {
            var adventure = await _contx.adventures.Where(a => a.Id == model.Id)
                .FirstOrDefaultAsync();

            if (adventure == null)
                throw new InvalidOperationException($"Adventure with id #{model.Id} does not exist");

            adventure.Duration = model.Duration;
            adventure.Name = model.Name;
            adventure.Address = model.Address;
            adventure.Application = model.Application;
            adventure.Gratitude = model.Gratitude;
            adventure.AttendeesDescription = model.AttendeesDescription;
            adventure.UnwantedAttendeesDescription = model.UnwantedAttendeesDescription;
            adventure.Media = model.Media;
            adventure.IsMediaPhoto = model.IsMediaPhoto;
            adventure.CityId = model.CityId;
            adventure.CountryId = model.CountryId;
            adventure.Description = model.Description;
            adventure.Experience = model.Experience;
            adventure.Date = model.Date;
            adventure.Time = model.Time;
            adventure.IsAutoReplyText = model.IsAutoReplyText;
            adventure.AutoReply = model.AutoReply;
            adventure.Status = AdventureStatus.Changed;
            adventure.IsAwaiting = model.IsAwaiting;

            await _contx.SaveChangesAsync();
        }

        public async Task<ParticipationRequestStatus> SendAdventureRequestByCodeAsync(ParticipationRequest request)
        {
            var adventure = await _contx.adventures.Where(a => a.UniqueLink == request.InvitationCode)
                .FirstOrDefaultAsync();

            if (adventure == null)
                return ParticipationRequestStatus.AdventureNotFound;

            return await SendAdventureRequestAsync(adventure.Id, request.UserId);
        }
        public async Task<ParticipationRequestStatus> SendAdventureRequestAsync(Guid adventureId, long userId)
        {
            var adventure = await _contx.adventures.Where(a => a.Id == adventureId)
                .FirstOrDefaultAsync();

            if (adventure == null)
                return ParticipationRequestStatus.AdventureNotFound;

            if (adventure.UserId == userId)
                return ParticipationRequestStatus.AdventuresOwner;

            var existingAttendee = await _contx.adventure_attendees
                .Where(a => a.AdventureId == adventure.Id && a.UserId == userId)
                .FirstOrDefaultAsync();

            if (existingAttendee != null)
                return ParticipationRequestStatus.AlreadyParticipating;

            var userName = await _contx.SYSTEM_USERS_BASES.Where(u => u.Id == userId)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();

            await AddUserNotificationAsync(new UserNotification
            {
                Section = Sections.Adventurer,
                Severity = Severities.Urgent,
                Description = "Someone had requested participation in your adventure", //TODO: Perhaps clarify if actions had been done with use of unique code
                UserId = userId,
                UserId1 = adventure.UserId
            });

            var newAttendee = new AdventureAttendee
            {
                Status = AdventureAttendeeStatus.New,
                AdventureId = adventure.Id,
                UserId = userId,
                Username = userName
            };

            await _contx.adventure_attendees.AddAsync(newAttendee);
            await _contx.SaveChangesAsync();

            return ParticipationRequestStatus.Ok;
        }

        public async Task<bool> DeleteAdventureAsync(Guid adventureId, long userId)
        {
            return true;
        }

        public async Task<bool> ProcessSubscriptionRequestAsync(Guid adventureId, long userId, AdventureAttendeeStatus status)
        {
            var attendee = await _contx.adventure_attendees.Where(a => a.UserId == userId && a.AdventureId == adventureId)
                .SingleOrDefaultAsync();


            if (attendee == null)
                throw new NullReferenceException($"No attendee with id #{userId} had been subscribed to adventure {adventureId}");

            var adventure = await _contx.adventures.Where(a => a.Id == adventureId)
                .Include(a => a.Creator).ThenInclude(u => u.UserBaseInfo)
                .FirstOrDefaultAsync();

            attendee.Status = status;

            if (status == AdventureAttendeeStatus.Accepted)
            {
                var contact = string.IsNullOrEmpty(adventure.GroupLink) ? 
                    $"You may contact its creator @{adventure.Creator.UserBaseInfo.UserName} and discuss details" : 
                    $"You may join creator's group via this link\n{adventure.GroupLink}" ;

                await AddUserNotificationAsync(new UserNotification
                {
                    UserId1 = userId,
                    Section = Sections.Adventurer,
                    Severity = Severities.Moderate,
                    Description = $"Your request to join adventure {adventure.Name} had been accepted.\n{contact}"
                });
            }

            await _contx.SaveChangesAsync();
            return true;
        }

        public async Task<List<AttendeeInfo>> GetAdventureAttendeesAsync(Guid adventureId)
        {
            return await _contx.adventure_attendees.Where(a => a.AdventureId == adventureId && (a.Status == AdventureAttendeeStatus.New || a.Status == AdventureAttendeeStatus.Accepted))
            .Select(a => new AttendeeInfo
            {
                UserId = a.UserId,
                Username = a.Username,
                Status = a.Status
            }).ToListAsync();
        }

        public async Task<List<Adventure>> GetUsersSubscribedAdventuresAsync(long userId)
        {
            var adventureIds = await _contx.adventure_attendees.Where(a => a.UserId == userId)
                .Select(a => a.AdventureId)
                .ToListAsync();

            return await _contx.adventures.Where(a => adventureIds.Contains(a.Id))
                .ToListAsync();
        }

        public async Task<List<GetAdventure>> GetUserAdventuresAsync(long userId)
        {
            return await _contx.adventures.Where(a => a.UserId == userId)
                .Select(a => new GetAdventure
                {
                    Id = a.Id,
                    Name = a.Name,
                    Status = a.Status
                })
                .ToListAsync();
        }

        public async Task<GetAdventureCount> GetAdventureCountAsync(long userId)
        {
            var createdCount = await _contx.adventures.Where(a => a.UserId == userId)
                .CountAsync();

            var subscribedCount = await _contx.adventure_attendees.Where(a => a.UserId == userId)
                .CountAsync();

            return new GetAdventureCount
            {
                Created = createdCount,
                Subscribed = subscribedCount
            };
        }

        private async Task ReactivateTickRequest(long userId)
        {
            var tickRequest = await _contx.tick_requests.Where(r => r.UserId == userId)
                .SingleOrDefaultAsync();

            //Return if tick request does not exists, because not all users has tick request
            if (tickRequest == null)
                return;

            var user = await _contx.SYSTEM_USERS.Where(u => u.UserId == userId)
                .SingleOrDefaultAsync();

            //Throw if user does not exist, because that method should not be called by not existing user
            if (user == null)
                throw new NullReferenceException($"User {userId} does not exist");

            tickRequest.State = TickRequestStatus.Changed;
            user.IdentityType = IdentityConfirmationType.None;

            await _contx.SaveChangesAsync();
        }

        public async Task<SimilarityBetweenUsers> GetSimilarityBetweenUsersAsync(long user1, long user2)
        {
            var userTags1 = await GetUserTagsAsync(user1);
            var userTags2 = await GetUserTagsAsync(user2);

            var intersections = userTags1.FullTags.Where(tag => userTags2.FullTags
            .Any(t => t.Tag == tag.Tag && t.TagType == tag.TagType))
            .Select(t => t.TagType)
            .ToList();

            var similarity = new SimilarityBetweenUsers
            {
                SimilarBy = intersections,

                SimilarityCount = userTags1.Tags.Intersect(userTags2.Tags).Count()
            };

            return similarity;
        }

        private async Task<GetUserTags> GetUserTagsAsync(long userId)
        {
            var tags = await _contx.user_tags.Where(t => t.UserId == userId)
                .ToListAsync();

            return new GetUserTags
            {
                FullTags = tags.Select(t => new UserTags
                {
                    Tag = t.Tag,
                    TagType = t.TagType
                }).ToList(),

                Tags = tags.Select(t => t.Tag)
                .ToList()
            };
        }

        private async Task<List<UserTag>> GetUserTagsAsync(long userId, TagType tagType)
        {
            return await _contx.user_tags.Where(t => t.UserId == userId && t.TagType == tagType)
                .ToListAsync();
        }

        private async Task<Dictionary<long, List<UserTag>>> GetUsersTagsAsync(List<long> userIds, TagType tagType)
        {
            var list = await _contx.user_tags.Where(t => userIds.Contains(t.UserId) && t.TagType == tagType)
                .ToListAsync();

            return list.GroupBy(t => t.UserId).ToDictionary(t => t.FirstOrDefault().UserId, t => t.ToList());
        }

        public async Task<GetLimitations> GetUserSearchLimitations(long userId)
        {
            //Refresh data regarding user premium status
            var hasPremium = await CheckUserHasPremiumAsync(userId);

            var limitations = await _contx.SYSTEM_USERS.Where(u => u.UserId == userId).Select(u => new GetLimitations
            {
                MaxTagViews = u.MaxTagSearchCount,
                MaxProfileViews = u.MaxProfileViewsCount,
                MaxRtViews = u.MaxRTViewsCount,
                MaxTagsPerSearch = hasPremium ? 50 : 25,
                ActualTagViews = u.TagSearchesCount,
                ActualProfileViews = u.ProfileViewsCount,
                ActualRtViews = u.RTViewsCount
            }).SingleOrDefaultAsync();

            if (limitations != null)
                return limitations;

            return new GetLimitations
            {
                MaxTagViews = 25,
                MaxProfileViews = 50,
                MaxRtViews = 25,
                MaxTagsPerSearch = 25,
                ActualTagViews = 25,
                ActualProfileViews = 25,
                ActualRtViews = 25
            };
        }

        public async Task<string> GetRandomHintAsync(int localisation, HintType? type)
        {
            if (type == null)
            {
                return await _contx.hints.Where(h => h.ClassLocalisationId == localisation && h.Type != HintType.Search)
                    .OrderBy(r => EF.Functions.Random()).Take(1)
                    .Select(h => h.Text)
                    .FirstOrDefaultAsync();
            }

            //25% chance to send a hint
            if (new Random().Next(1, 4) != 1)
                return null;

            return await _contx.hints.Where(h => h.ClassLocalisationId == localisation && h.Type == type)
                    .OrderBy(r => EF.Functions.Random())
                    .Select(h => h.Text)
                    .FirstOrDefaultAsync();
        }

        public async Task<BasicUserInfo> GetUserBasicInfo(long userId)
        {
            var limitations = await GetUserSearchLimitations(userId);

            return await _contx.SYSTEM_USERS.Where(u => u.UserId == userId).Select(u => new BasicUserInfo
            {
                Id = u.UserId,
                Username = u.UserBaseInfo.UserName,
                UserRealName = u.UserBaseInfo.UserRealName,
                HasPremium = u.HasPremium,
                IsBanned = u.IsBanned,
                IsBusy = u.IsBusy,
                Limitations = limitations
            }).FirstOrDefaultAsync();
        }

        public async Task SwitchHintsVisibilityAsync(long userId)
        {
            var user = await  _contx.SYSTEM_USERS.Where(u => u.UserId == userId)
                .FirstOrDefaultAsync();

            if (user == null)
                return;

            user.ShouldSendHints = !user.ShouldSendHints;
            await _contx.SaveChangesAsync();
        }

        public async Task SwitchSearchCommentsVisibilityAsync(long userId)
        {
            var user = await _contx.SYSTEM_USERS.Where(u => u.UserId == userId)
                .FirstOrDefaultAsync();

            if (user == null)
                return;

            user.ShouldComment = !user.ShouldComment;
            await _contx.SaveChangesAsync();
        }

        public async Task<GetUserMedia> GetUserMediaAsync(long userId)
        {
            return await _contx.SYSTEM_USERS_BASES.Where(u => u.Id == userId).Select(u => new GetUserMedia
            {
                Media = u.UserMedia,
                IsPhoto = u.IsMediaPhoto
            }).FirstOrDefaultAsync();
        }

        public async Task<UserPartialData> GetUserPartialData(long userId)
        {
            return await _contx.SYSTEM_USERS.Where(u => u.UserId == userId).Select(u => new UserPartialData
            {
                Id = u.UserId,
                AppLanguage = u.UserDataInfo.LanguageId,
                Media = u.UserBaseInfo.UserMedia,
                IsPhoto = u.UserBaseInfo.IsMediaPhoto
            }).FirstOrDefaultAsync();
        }

        public async Task<ManageAdventure> GetAdventureAsync(Guid id)
        {
            return await _contx.adventures.Where(a => a.Id == id)
                .Select(a => new ManageAdventure(a))
                .FirstOrDefaultAsync();
        }

        public async Task<bool> SaveAdventureTemplateAsync(ManageTemplate model)
        {
            var existingTemplate = await _contx.adventure_templates.Where(t => t.Id == model.Id)
                .FirstOrDefaultAsync();

            //Update template instead of creating it, if exists
            if (existingTemplate != null)
            {
                existingTemplate.Name = model.Name;
                existingTemplate.UserId = model.UserId;
                existingTemplate.Address = model.Address;
                existingTemplate.Application = model.Application;
                existingTemplate.AttendeesDescription = model.AttendeesDescription;
                existingTemplate.UnwantedAttendeesDescription = model.UnwantedAttendeesDescription;
                existingTemplate.CountryId = model.CountryId;
                existingTemplate.CityId = model.CityId;
                existingTemplate.Description = model.Description;
                existingTemplate.Duration = model.Duration;
                existingTemplate.Date = model.Date;
                existingTemplate.Time = model.Time;
                existingTemplate.Experience = model.Experience;
                existingTemplate.Gratitude = model.Gratitude;
                existingTemplate.Media = model.Media;
                existingTemplate.IsMediaPhoto = model.IsMediaPhoto;
                existingTemplate.AutoReply = model.AutoReply;
                existingTemplate.IsAutoReplyText = model.IsAutoReplyText;

                await _contx.SaveChangesAsync();
                return true;
            }

            //Create template
            var template = new AdventureTemplate
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                UserId = model.UserId,
                IsOffline = model.IsOffline,
                Address = model.Address,
                Application = model.Application,
                AttendeesDescription = model.AttendeesDescription,
                UnwantedAttendeesDescription = model.UnwantedAttendeesDescription,
                CountryId = model.CountryId,
                CityId = model.CityId,
                Description = model.Description,
                Duration = model.Duration,
                Date = model.Date,
                Time = model.Time,
                Experience = model.Experience,
                Gratitude = model.Gratitude,
                Media = model.Media,
                IsMediaPhoto = model.IsMediaPhoto,
                AutoReply = model.AutoReply,
                IsAutoReplyText = model.IsAutoReplyText
            };

            await _contx.AddAsync(template);
            await _contx.SaveChangesAsync();

            return true;
        }

        public async Task<List<GetTemplateShort>> GetAdventureTemplatesAsync(long userId)
        {
            return await _contx.adventure_templates.Where(t => t.UserId == userId).Select(t => new GetTemplateShort
            {
                Id = t.Id,
                Name = t.Name
            }).ToListAsync();
        }

        public async Task<ManageTemplate> GetAdventureTemplateAsync(Guid id)
        {
            return await _contx.adventure_templates.Where(t => t.Id == id).Select(t => new ManageTemplate
            {
                Id = t.Id,
                Date = t.Date,
                Time = t.Time,
                Description = t.Description,
                Duration = t.Duration,
                Address = t.Address,
                AttendeesDescription = t.AttendeesDescription,
                Application = t.Application,
                AutoReply = t.AutoReply,
                IsAutoReplyText = t.IsAutoReplyText,
                Media = t.Media,
                IsMediaPhoto = t.IsMediaPhoto,
                CityId = t.CityId,
                CountryId = t.CountryId,
                Experience = t.Experience,
                Gratitude = t.Gratitude,
                UnwantedAttendeesDescription = t.UnwantedAttendeesDescription,
                IsOffline = t.IsOffline,
                Name = t.Name,
                UserId = t.UserId
            }).FirstOrDefaultAsync();
        }

        public async Task<DeleteResult> DeleteAdventureTemplateAsync(Guid templateId)
        {
            var template = await _contx.adventure_templates.Where(t => t.Id == templateId)
                .FirstOrDefaultAsync();

            if (template == null)
                return DeleteResult.DoesNotExist;

            _contx.adventure_templates.Remove(template);
            await _contx.SaveChangesAsync();

            return DeleteResult.Success;
        }

        public async Task<DeleteResult> DeleteAdventureAttendeeAsync(Guid adventureId, long attendeeId)
        {
            var attendee = await _contx.adventure_attendees.Where(a => a.AdventureId == adventureId && a.UserId == attendeeId)
                .FirstOrDefaultAsync();

            if (attendee == null)
                return DeleteResult.DoesNotExist;

            _contx.adventure_attendees.Remove(attendee);
            await _contx.SaveChangesAsync();

            await AddUserNotificationAsync(new UserNotification
            {
                UserId1 = attendee.UserId,
                Section = Sections.Adventurer,
                Severity = Severities.Urgent,
                Description = "You have been removed from one of the adventures" // TODO: More precise ?
            });

            return DeleteResult.Success;
        }

        public async Task<SetGroupIdResult> SetAdventureGroupIdAsync(SetGroupIdRequest request)
        {
            var hasName = !string.IsNullOrEmpty(request.AdventureName);
            var adventure = await _contx.adventures.Where(a => a.UserId == request.UserId && a.IsAwaiting && ((hasName && a.Name == request.AdventureName) || !hasName))
                .FirstOrDefaultAsync();

            if (adventure == null)
                return SetGroupIdResult.AdventureDoesNotExist;

            adventure.GroupLink = request.GroupLink;
            adventure.GroupId = request.GroupId;

            await _contx.SaveChangesAsync();
            return SetGroupIdResult.Success;
        }
    }
}
