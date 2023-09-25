using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Entities.AchievementEntities;
using WebApi.Entities.AdminEntities;
using WebApi.Entities.AdventureEntities;
using WebApi.Entities.EffectEntities;
using WebApi.Entities.LocationEntities;
using WebApi.Entities.ReportEntities;
using WebApi.Entities.SecondaryEntities;
using WebApi.Entities.TestEntities;
using WebApi.Entities.UserActionEntities;
using WebApi.Entities.UserInfoEntities;
using WebApi.Enums;
using WebApi.Interfaces;
using WebApi.Utilities;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;
using OceanStats = WebApi.Enums.OceanStats;
using WebApi.Entities.SystemEntitires;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace WebApi.Repositories
{
    public class SystemUserRepository : IUserRepository
    {
        private UserContext _contx { get; set; }

        public SystemUserRepository(UserContext context)
        {
            _contx = context;
        }

        public async Task<long> RegisterUserAsync(UserRegistrationModel model, bool wasRegistered = false)
        {
            var country = "---";
            var city = "---";

            Location location = null;
            var user = new User(model.Id);
            user.EnteredPromoCodes = model.Promo;

            if (model.CityCode != null && model.CountryCode != null)
            {
                location = new Location { Id = model.Id, CityId = (int)model.CityCode, CountryId = (int)model.CountryCode, CountryLang = model.AppLanguage, CityCountryLang = model.AppLanguage };

                country = await _contx.Countries.Where(c => c.Id == model.CountryCode && c.Lang == model.AppLanguage)
                    .Select(c => c.CountryName)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                city = await _contx.Cities.Where(c => c.Id == model.CityCode && c.CountryLang == model.AppLanguage)
                    .Select(c => c.CityName)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            else
            {
                location = new Location { Id = model.Id };
            }

            var uData = new UserData
            {
                Id = model.Id,
                UserLanguages = model.Languages,
                Reason = model.Reason,
                UserAge = model.Age,
                UserGender = model.Gender,
                Language = model.AppLanguage,
                AgePrefs = model.AgePrefs,
                UserDescription = user.GenerateUserDescription(model.RealName, model.Age, country, city, model.Description),
                UserGenderPrefs = model.GenderPrefs,
                UserMedia = model.Media,
                MediaType = model.MediaType,
                CommunicationPrefs = model.CommunicationPrefs,
                LanguagePreferences = model.LanguagePreferences,
                LocationPreferences = model.UserLocationPreferences,
                UserName = model.UserName,
                UserRawDescription = model.Description,
                UserRealName = model.RealName,
            };

            var uSettings = new UserSettings(model.Id, model.UsesOcean);

            user.LocationId = location.Id;

            await _contx.Users.AddAsync(user);
            await _contx.UserData.AddAsync(uData);
            await _contx.UsersSettings.AddAsync(uSettings);
            await _contx.UserLocations.AddAsync(location);
            await _contx.SaveChangesAsync();

            await GenerateUserAchievementList(user.Id, uData.Language, wasRegistered);
            await TopUpPointBalance(model.Id, 180, "Starting Pack"); //180 is a starting user point pack
            await AddUserTrustLevel(model.Id);
            await AddUserTrustProgressAsync(model.Id, 0.000012);

            // Set tags
            if (!string.IsNullOrEmpty(model.Tags))
                await UpdateTags(new UpdateTags { UserId = model.Id, RawTags = model.Tags });

            // Set auto reply
            if (!string.IsNullOrEmpty(model.Voice))
                await SetAutoReplyVoiceAsync(model.Id, model.Voice);
            else if (!string.IsNullOrEmpty(model.Text))
                await SetAutoReplyTextAsync(model.Id, model.Text);

            if (model.UsesOcean)
            {
                var personalityStats = new Entities.UserInfoEntities.OceanStats(model.Id);
                var personalityPoints = new OceanPoints(model.Id);
            }

            var invitation = await GetInvitationAsync(model.Id);

            if(invitation != null)
            {
                var invitor = await _contx.Users.Where(u => u.Id == invitation.InviterCredentials.UserId)
                    .Include(u => u.Settings)
                    .FirstOrDefaultAsync();

                invitor.InvitedUsersCount++;

                var bonus = invitor.HasPremium ? 0.05f : 0f;
                var multiplier = 1;

                if (invitor.InvitedUsersCount > 10)
                    multiplier = 2;

                if (invitor.InvitedUsersCount == 1)
                {
                    await TopUpPointBalance(invitor.Id, 250 * multiplier, $"User {invitor.Id} has invited his first user");
                    var effecId = new Random().Next(5, 10);
                    await PurchaseEffectAsync(invitor.Id, effecId, 0, Currency.Points);
                }
                else if (invitor.InvitedUsersCount == 3 || invitor.InvitedUsersCount % 3 == 0)
                {
                    if (multiplier == 1)
                        invitor.InvitedUsersBonus = 0.15f + bonus;
                    await TopUpPointBalance(invitor.Id, 1199 * multiplier, $"User {invitor.Id} has invited % 3 users");
                }
                else if (invitor.InvitedUsersCount == 7 || invitor.InvitedUsersCount % 7 == 0)
                {
                    if (multiplier == 1)
                        invitor.InvitedUsersBonus = 0.35f + bonus;
                    await TopUpPointBalance(invitor.Id, 1499 * multiplier, $"User {invitor.Id} has invited % 7 users");
                }
                else if (invitor.InvitedUsersCount == 10 || invitor.InvitedUsersCount % 10 == 0)
                {
                    if (multiplier == 1)
                    {
                        invitor.InvitedUsersBonus = 0.5f + bonus;
                        // 1499 will then turn into 1999 due to premium purchase reward
                        await TopUpPointBalance(invitor.Id, 1499, $"User {invitor.Id} has invited % 10 users");
                        //Adds + 10 random effects to users inventory
                        var effecId = new Random().Next(5, 10);
                        await PurchaseEffectAsync(invitor.Id, effecId, 0, Currency.Points, 10);
                        await GrantPremiumToUser(invitor.Id, 0, 30, Currency.Points);
                    }
                    else
                        await TopUpPointBalance(invitor.Id, 1999 * multiplier, $"User {invitor.Id} has invited more than 10 users");
                }
                else
                {
                    await TopUpPointBalance(invitor.Id, (int)(200 + (200 * bonus) * multiplier), $"User {model.Id} was invited by user {invitor.Id}");
                }

                user.BonusIndex = 1.5f;
                user.ParentId = invitor.Id;

                _contx.Users.Update(user);
                await _contx.SaveChangesAsync();

                //User instantly receives a like by an invitor if he approves it
                if (invitor.Settings.IncreasedFamiliarity)
                    await RegisterUserRequestAsync(new AddRequest { SenderId = invitor.Id, UserId = model.Id, IsMatch = false });

                //Invitor is notified about referential registration
                await NotifyUserAboutReferentialRegistrationAsync(invitor.Id, model.Id);
            }

            //Enter promo if it was supplied
            if (user.EnteredPromoCodes != null)
                await CheckPromoIsCorrectAsync(model.Id, user.EnteredPromoCodes, false);
            else
                user.EnteredPromoCodes = "";

            //Add Starting test pack
            //TODO: Set langauge when tests are localized
            await PurchaseTestAsync(model.Id, 38, 0, Currency.Points, AppLanguage.RU); // Approved
            await PurchaseTestAsync(model.Id, 32, 0, Currency.Points, AppLanguage.RU); // Approved
            await PurchaseTestAsync(model.Id, 23, 0, Currency.Points, AppLanguage.RU); // Approved
            await PurchaseTestAsync(model.Id, 54, 0, Currency.Points, AppLanguage.RU); // Approved
            await PurchaseTestAsync(model.Id, 29, 0, Currency.Points, AppLanguage.RU); // Approved
            await PurchaseTestAsync(model.Id, 49, 0, Currency.Points, AppLanguage.RU); // Approved
            await PurchaseTestAsync(model.Id, 34, 0, Currency.Points, AppLanguage.RU);

            return model.Id;
        }

        public async Task<GetUserInfo> GetUserInfoAsync(long id)
        {
            //Actualize premium information
            await CheckUserHasPremiumAsync(id);

            return await _contx.Users.Where(u => u.Id == id).Select(u => new GetUserInfo
            {
               Id = u.Data.Id,
               UserName = u.Data.UserName,
               AgePrefs = u.Data.AgePrefs,
               AutoReplyText = u.Data.AutoReplyText,
               AutoReplyVoice = u.Data.AutoReplyVoice,
               UserAge = u.Data.UserAge,
               UserDescription = u.Data.UserDescription,
               UserRawDescription = u.Data.UserRawDescription,
               CommunicationPrefs = u.Data.CommunicationPrefs,
               Language = u.Data.Language,
               LanguagePreferences = u.Data.LanguagePreferences,
               LocationPreferences = u.Data.LocationPreferences,
               MediaType = u.Data.MediaType,
               UserMedia = u.Data.UserMedia,
               Reason = u.Data.Reason,
               UserGender = u.Data.UserGender,
               UserGenderPrefs = u.Data.UserGenderPrefs,
               UserLanguages = u.Data.UserLanguages,
               UserRealName = u.Data.UserRealName,
               CityId = u.Location.CityId,
               CountryId = u.Location.CountryId,
               CityCountryLang = u.Data.Language,
               CountryLang = u.Data.Language,
               IdentityType = u.IdentityType,
               HasPremium = u.HasPremium
            }).FirstOrDefaultAsync();
        }

        public async Task<GetUserSettings> GetUserSettingsAsync(long id)
        {
            //Actualize premium information
            await CheckUserHasPremiumAsync(id);

            return await _contx.Users.Where(u => u.Id == id).Select(s => new GetUserSettings
            {
                ShouldComment = s.Settings.ShouldComment,
                ShouldConsiderLanguages = s.Settings.ShouldConsiderLanguages,
                ShouldFilterUsersWithoutRealPhoto = s.Settings.ShouldFilterUsersWithoutRealPhoto,
                ShouldSendHints = s.Settings.ShouldSendHints,
                IncreasedFamiliarity = s.Settings.IncreasedFamiliarity,
                IsFree = s.Settings.IsFree,
                Language = s.Data.Language,
                UsesOcean = s.Settings.UsesOcean,
                HasPremium = s.HasPremium,
            }).FirstOrDefaultAsync();
        }

        public async Task<SearchResponse> GetUsersAsync(long userId, bool isRepeated=false, bool isFreeSearch = false)
        {
            var currentUser = await _contx.Users.Where(u => u.Id == userId)
                .Include(u => u.Data)
                .Include(u => u.Settings)
                .Include(u => u.Location)
                .FirstOrDefaultAsync();

            var returnData = new List<GetUserData>();

            if (currentUser.ProfileViewsCount >= currentUser.MaxProfileViewsCount)
                return new SearchResponse();

            var profileCount = currentUser.MaxProfileViewsCount - currentUser.ProfileViewsCount;

            //Check if user STILL has premium
            await CheckUserHasPremiumAsync(currentUser.Id);

            var query = _contx.Users
                .Where(u => u.Id != currentUser.Id)
                .Where(u => u.Data.Reason == currentUser.Data.Reason)
                .Where(u => u.Data.CommunicationPrefs == currentUser.Data.CommunicationPrefs)
                .Where(u => u.Data.AgePrefs.Contains(currentUser.Data.UserAge))
                //Check if users gender preferences correspond to current user gender prefs or are equal to 'Does not matter'
                .Where(u => u.Data.UserGenderPrefs == currentUser.Data.UserGender || u.Data.UserGenderPrefs == Gender.RatherNotSay)
                .Where(u => u.Data.UserLanguages.Any(l => currentUser.Data.LanguagePreferences.Contains(l)))
                .Where(u => currentUser.Data.AgePrefs.Contains(u.Data.UserAge))
                .Where(u => currentUser.Data.UserLanguages.Any(l => u.Data.LanguagePreferences.Contains(l)))
                //Check if users had encountered one another
                .Where(u => u.Encounters.Where(e => e.Section == Section.Requester || e.Section == Section.Familiator)
                    .All(e => e.EncounteredUserId != currentUser.Id)) //May casuse errors
                //Check if request already exists
                .Where(u => u.Requests.All(n => n.SenderId != currentUser.Id && n.UserId != currentUser.Id)) //May casuse errors
                .Include(u => u.Data)
                .Include(u => u.Location)
                .Include(u => u.Settings)
                .AsNoTracking();

            //Free search
            if (isFreeSearch)
                query = query.Where(u => u.Settings.IsFree != null && (bool)u.Settings.IsFree);

            //Identity check
            if (currentUser.Settings.ShouldFilterUsersWithoutRealPhoto && currentUser.HasPremium)
                query = query.Where(u => u.IdentityType != IdentityConfirmationType.None);

            //Don't check genders if user does NOT have gender preferences
            if (currentUser.Data.UserGenderPrefs != Gender.RatherNotSay)
            {
                query = query.Where(u => u.Data.UserGender == currentUser.Data.UserGenderPrefs)
                    .Where(u => currentUser.Data.UserGenderPrefs == u.Data.UserGender);
            }

            if (currentUser.Location.CountryId != null)
            {
                query = query.Where(u => u.Location.CountryId != null)
                        .Where(u => u.Data.LocationPreferences.Contains((int)currentUser.Location.CountryId))
                        .Where(u => currentUser.Data.LocationPreferences.Contains((int)u.Location.CountryId));
            }

            //Check if users are in each others black lists
            query = query.Where(u => u.BlackList.All(l => l.BannedUserId != userId));
            //TODO: Check current user's blacklist
            //query = query.Where(u => currentUser.BlackList.All(l => l.BannedUserId != u.Id)); 

            var data = await query.OrderBy(q => EF.Functions.Random())
                .Select(u => new GetUserData(u, ""))
                .Take(profileCount)
                .ToListAsync();

            //If user uses OCEAN+ functionality
            if (currentUser.Settings.UsesOcean)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    returnData.Add(await GetOceanMatchResult(userId, currentUser, data[i], isRepeated));
                }
            }
            else
            {
                for (int i = 0; i < data.Count; i++)
                {
                    var u = data[i];

                    var outputUser = await AssembleProfileAsync(currentUser, u);

                    returnData.Add(outputUser);
                }
            }

            ////TODO: Reconsider usefulness
            ////Check if method wasnt already repeated
            //if (!isRepeated)
            //{
            //    //Check if users count is less than the limit
            //    if (returnData.Count <= profileCount)
            //    {
            //        returnData = await GetUsersAsync(userId, isRepeated: true, isFreeSearch: isFreeSearch);
            //    }

            //    //Add user trust exp only if method was not repeated
            //    await AddUserTrustProgressAsync(userId, 0.000003);

            //    //Order user list randomly 
            //    returnData = returnData.OrderBy(u => new Random().Next())
            //        .ToList();

            //    returnData.OrderByDescending(u => u.CityId == currentUser.Location.CityId)
            //        .ToList();
            //}

            return new SearchResponse(returnData);
        }

        private async Task<GetUserData> AssembleProfileAsync(User currentUser, GetUserData outputUser)
        {
            var bonus = "";

            //Add comment if user wants to see them
            if (currentUser.Settings.ShouldComment)
                outputUser.Comment = await GetRandomHintAsync(currentUser.Data.Language, HintType.Search);

            if (outputUser.HasPremium && outputUser.Nickname != "")
                bonus += $"<b>{outputUser.Nickname}</b>\n";

            if (outputUser.IdentityType == IdentityConfirmationType.Partial)
                bonus += $"☑️☑️☑️\n\n";
            else if (outputUser.IdentityType == IdentityConfirmationType.Full)
                bonus += $"✅✅✅\n\n";

            outputUser.AddDescriptionUpwards(bonus);
            return outputUser;
        }

        private async Task<GetUserData> GetOceanMatchResult(long userId, User currentUser, GetUserData managedUser, bool isRepeated)
        {
            var returnUser = await AssembleProfileAsync(currentUser, managedUser);

            var userActiveEffects = await GetUserActiveEffects(userId);
            var deviation = 0.15;
            var minDeviation = 0.05;

            var currentValueMax = 0d;
            var currentValueMin = 0d;

            var valentineBonus = 1d;

            var importantMatches = 0;
            var secondaryMatches = 0;
            var bonus = "";

            var hasActiveValentine = userActiveEffects.Any(e => e.Effect == Currency.TheValentine);

            var userHasDetectorOn = userActiveEffects.Any(e => e.Effect == Currency.TheDetector);

            if (hasActiveValentine)
                valentineBonus = 2;

            if (isRepeated)
            {
                deviation *= 1.5;
                minDeviation *= 3.2;
            }

            var userPoints = await _contx.OceanPoints.Where(p => p.UserId == currentUser.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var userStats = await _contx.OceanStats.Where(s => s.UserId == currentUser.Id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            //Enhanse users OP if condition is met
            if(currentUser.ShouldEnhance)
            {
                userPoints.OpennessPercentage = 0.1f;
                userPoints.ConscientiousnessPercentage = 0.1f;
                userPoints.ExtroversionPercentage = 0.1f;
                userPoints.AgreeablenessPercentage = 0.1f;
                userPoints.NeuroticismPercentage = 0.1f;
                userPoints.NaturePercentage = 0.1f;
            }

            var important = await userPoints.GetImportantParams();

            //Pass if user does not use OCEAN+
            if (!managedUser.UsesOcean)
                return returnUser;

            var user2Points = await _contx.OceanPoints.Where(p => p.UserId == managedUser.UserId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var user2Stats = await _contx.OceanStats.Where(s => s.UserId == managedUser.UserId)
                .AsNoTracking()
                .FirstOrDefaultAsync();


            //Turns off the parameter if 1. User has no relative tests passed; 2. No points where invested in it
            if (userPoints.Openness > 0 && userStats.Openness > 0 && user2Points.Openness > 0 && user2Stats.Openness > 0)
            {
                //TODO: create its own deviation variable depending on the number of personalities (It is likely to be grater than the normal one)
                var personalitySim = await CalculateSimilarityAsync(userStats.Openness * valentineBonus, user2Stats.Openness);

                currentValueMax = ApplyMaxDeviation(userPoints.OpennessPercentage, deviation);
                currentValueMin = ApplyMinDeviation(userPoints.OpennessPercentage, minDeviation);

                //Negative conditions are applied, cuz this is an exclussive condition
                if (personalitySim <= currentValueMax && personalitySim >= currentValueMin)
                {
                    currentValueMax = ApplyMaxDeviation(user2Points.OpennessPercentage, deviation);
                    currentValueMin = ApplyMinDeviation(user2Points.OpennessPercentage, minDeviation);

                    if (personalitySim <= currentValueMax && personalitySim >= currentValueMin)
                    {
                        bonus += "[O] ";
                        if (important.Contains(Enums.OceanStats.Openness))
                        {
                            importantMatches++;
                        }
                        else
                            secondaryMatches++;
                    }
                }
            }

            if (userPoints.Conscientiousness > 0 && userStats.Conscientiousness > 0 && user2Points.Conscientiousness > 0 && user2Stats.Conscientiousness > 0)
            {
                var emIntellectSim = await CalculateSimilarityAsync(userStats.Conscientiousness * valentineBonus, user2Stats.Conscientiousness);

                currentValueMax = ApplyMaxDeviation(userPoints.ConscientiousnessPercentage, deviation);
                currentValueMin = ApplyMinDeviation(userPoints.ConscientiousnessPercentage, minDeviation);

                if (emIntellectSim <= currentValueMax && emIntellectSim >= currentValueMin)
                {
                    currentValueMax = ApplyMaxDeviation(user2Points.ConscientiousnessPercentage, deviation);
                    currentValueMin = ApplyMinDeviation(user2Points.ConscientiousnessPercentage, minDeviation);

                    if (emIntellectSim <= currentValueMax && emIntellectSim >= currentValueMin)
                    {
                        bonus += "[C] ";
                        if (important.Contains(Enums.OceanStats.Conscientiousness))
                        {
                            importantMatches++;
                        }
                        else
                            secondaryMatches++;
                    }
                }
            }

            if (userPoints.Extroversion > 0 && userStats.Extroversion > 0 && user2Points.Extroversion > 0 && user2Stats.Extroversion > 0)
            {
                var reliabilitySim = await CalculateSimilarityAsync(userStats.Extroversion * valentineBonus, user2Stats.Extroversion);

                currentValueMax = ApplyMaxDeviation(userPoints.ExtroversionPercentage, deviation);
                currentValueMin = ApplyMinDeviation(userPoints.ExtroversionPercentage, minDeviation);

                if (reliabilitySim <= currentValueMax && reliabilitySim >= currentValueMin)
                {
                    currentValueMax = ApplyMaxDeviation(user2Points.ExtroversionPercentage, deviation);
                    currentValueMin = ApplyMinDeviation(user2Points.ExtroversionPercentage, minDeviation);

                    if (reliabilitySim <= currentValueMax && reliabilitySim >= currentValueMin)
                    {
                        bonus += "[E] ";
                        if (important.Contains(Enums.OceanStats.Extroversion))
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
                var compassionSim = await CalculateSimilarityAsync(userStats.Agreeableness * valentineBonus, user2Stats.Agreeableness);

                currentValueMax = ApplyMaxDeviation(userPoints.AgreeablenessPercentage, deviation);
                currentValueMin = ApplyMinDeviation(userPoints.AgreeablenessPercentage, minDeviation);

                if (compassionSim <= currentValueMax && compassionSim >= currentValueMin)
                {
                    currentValueMax = ApplyMaxDeviation(user2Points.AgreeablenessPercentage, deviation);
                    currentValueMin = ApplyMinDeviation(user2Points.AgreeablenessPercentage, minDeviation);

                    if (compassionSim <= currentValueMax && compassionSim >= currentValueMin)
                    {
                        bonus += "[A] ";
                        if (important.Contains(Enums.OceanStats.Agreeableness))
                        {
                            importantMatches++;
                        }
                        else
                            secondaryMatches++;
                    }
                }
            }


            if (userPoints.Neuroticism > 0 && userStats.Neuroticism > 0 && user2Points.Neuroticism > 0 && user2Stats.Neuroticism > 0)
            {
                var openMindSim = await CalculateSimilarityAsync(userStats.Neuroticism * valentineBonus, user2Stats.Neuroticism);

                currentValueMax = ApplyMaxDeviation(userPoints.NeuroticismPercentage, deviation);
                currentValueMin = ApplyMinDeviation(userPoints.NeuroticismPercentage, minDeviation);

                if (openMindSim <= currentValueMax && openMindSim >= currentValueMin)
                {
                    currentValueMax = ApplyMaxDeviation(user2Points.NeuroticismPercentage, deviation);
                    currentValueMin = ApplyMinDeviation(user2Points.NeuroticismPercentage, minDeviation);

                    if (openMindSim <= currentValueMax && openMindSim >= currentValueMin)
                    {
                        bonus += "[N] ";
                        if (important.Contains(Enums.OceanStats.Neuroticism))
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
                        bonus += "[+] ";
                        if (important.Contains(Enums.OceanStats.Nature))
                        {
                            importantMatches++;
                        }
                        else
                            secondaryMatches++;
                    }
                }
            }

            // Ocean+ match counts only if there are 1 important param match or 3 secondary match
            if (importantMatches < 1 && secondaryMatches < 3)
            {
                bonus = "";
            };

            //Add comment if user wants to see them
            if (currentUser.Settings.ShouldComment)
                returnUser.Comment = await GetRandomHintAsync(currentUser.Data.Language, HintType.Search);

            if (managedUser.HasPremium && managedUser.Nickname != "")
                bonus += $"<b>{managedUser.Nickname}</b>\n";

            if (managedUser.IdentityType == IdentityConfirmationType.Partial)
                bonus += $"☑️☑️☑️\n\n";
            else if (managedUser.IdentityType == IdentityConfirmationType.Full)
                bonus += $"✅✅✅\n\n";

            if (userHasDetectorOn)
                bonus += $"<b>OCEAN+ match!</b>\n<b>{bonus}</b>";
            else
                bonus += "<b>OCEAN+ match!</b>";

            returnUser.AddDescriptionUpwards(bonus);

            return returnUser;
        }

        public async Task<Country> GetCountryAsync(long id)
        {
            var c = await _contx.Countries.Include(c => c.Cities).FirstOrDefaultAsync(c => c.Id == id);
            return c;
        }

        public async Task<long> AddFeedbackAsync(AddFeedback request)
        {
            var feedback = new Feedback
            {
                UserId = request.UserId,
                Reason = request.Reason,
                Text = request.Text,
                InsertedUtc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
            };

            await _contx.Feedbacks.AddAsync(feedback);
            await _contx.SaveChangesAsync();

            return feedback.Id;
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
            if (await _contx.Users.FindAsync(id) == null)
            { return false; }
            return true;
        }

        public async Task<string> GetUserAppLanguage(long id)
        {
            var language = await _contx.UserData.Where(u => u.Id == id)
                .Select(u => u.Language.ToString())
                .FirstOrDefaultAsync();

            return language;
        }

        public async Task<bool> CheckUserIsRegistered(long userId)
        {
            return await _contx.Users.FindAsync(userId) != null;
        }

        public async Task<bool> CheckUserHasVisitedSection(long userId, Section section)
        {
            var visit = await _contx.UserVisits.
                Where(v => v.UserId == userId && v.Section == section)
                .FirstOrDefaultAsync();

            if (visit != null)
            {
                await AddUserTrustProgressAsync(userId, 0.000002);
                return true;
            }

            if (await CheckUserIsRegistered(userId))
            {
                await AddUserTrustProgressAsync(userId, 0.000002);
                await _contx.UserVisits.AddAsync(new Visit { UserId = userId, Section = section });
                await _contx.SaveChangesAsync();
                return false;
            }

            return false;
        }

        public async Task<User> GetUserInfoByUsrnameAsync(string username)
        {
            return await _contx.Users
                .Where(u => u.Data.UserName == username)
                .Include(s => s.Data)
                .Include(s => s.Location)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Feedback>> GetMostRecentFeedbacks()
        {
            var pointInTime = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-2), DateTimeKind.Utc);
            return await _contx.Feedbacks
                .Where(f => f.InsertedUtc >= pointInTime)
                .Include(f => f.User)
                .ToListAsync();
        }

        public async Task<List<Feedback>> GetMostRecentFeedbacksByUserId(long userId)
        {
            var pointInTime = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-2), DateTimeKind.Utc);
            return await _contx.Feedbacks
                .Where(f => f.InsertedUtc >= pointInTime && f.UserId == userId)
                .Include(f => f.User)
                .Include(f => f.Reason)
                .ToListAsync();
        }

        public async Task<Feedback> GetFeedbackById(long id)
        {
            return await _contx.Feedbacks
                .Where(f => f.Id == id)
                .Include(f => f.User)
                .Include(f => f.Reason)
                .FirstOrDefaultAsync();
        }

        public async Task<long> AddUserReportAsync(SendUserReport request)
        {
            try
            {
                var reportedUser = await _contx.Users.Where(u => u.Id == request.ReportedUser)
                    .FirstOrDefaultAsync();

                reportedUser.ReportCount++;

                //Ban user if dailly report count is too high
                if (reportedUser.ReportCount >= 5)
                {
                    reportedUser.IsBanned = true;
                    reportedUser.BanDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                }

                var report = new Report
                {
                    SenderId = request.Sender,
                    UserId = request.ReportedUser,
                    Text = request.Text,
                    Reason = request.Reason,
                    InsertedUtc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
                };

                await _contx.UserReports.AddAsync(report);

                await _contx.SaveChangesAsync();

                return report.Id;
            }
            catch {return 0;}
        }

        public async Task<long> AddAdventureReportAsync(SendAdventureReport request)
        {
            try
            {
                var reportedUser = await _contx.Adventures.Where(u => u.Id == request.Adventure)
                    .Select(a => a.Creator)
                    .FirstOrDefaultAsync();

                reportedUser.ReportCount++;

                //Ban user if dailly report count is too high
                if (reportedUser.ReportCount >= 5)
                {
                    reportedUser.IsBanned = true;
                    reportedUser.BanDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                }

                var report = new Report
                {
                    SenderId = request.Sender,
                    AdventureId = request.Adventure,
                    Text = request.Text,
                    Reason = request.Reason,
                    InsertedUtc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
                };

                await _contx.UserReports.AddAsync(report);

                await _contx.SaveChangesAsync();

                return report.Id;
            }
            catch { return 0; }
        }

        public async Task<List<Report>> GetMostRecentReports()
        {
            var pointInTime = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-1), DateTimeKind.Utc);
            return await _contx.UserReports.Where(r => r.InsertedUtc > pointInTime).ToListAsync();
        }

        public async Task<Report> GetSingleUserReportByIdAsync(long id)
        {
            return await _contx.UserReports.Where(r => r.Id == id)
                .Include(r => r.User)
                .Include(r => r.Sender)
                .SingleOrDefaultAsync();
        }

        public async Task<List<Report>> GetAllReportsOnUserAsync(long userId)
        {
            return await _contx.UserReports.Where(r => r.UserId == userId).ToListAsync();
        }

        public List<GetLocalizedEnum> GetReportReasonsAsync()
        {
            var reasons = new List<GetLocalizedEnum>();

            foreach (var reason in Enum.GetValues(typeof(ReportReason)))
            {
                reasons.Add(new GetLocalizedEnum
                {
                    Id = (short)reason,
                    Name = EnumLocalizer.GetLocalizedValue((ReportReason)reason)
                });
            }

            return reasons;
        }

        public async Task<bool> AddUserToBlackListAsync(long userId, long bannedUserId)
        {
            long id = await _contx.UserBlacklists.Where(l => l.UserId == userId).CountAsync() +1;
            await _contx.UserBlacklists.AddAsync(new BlackList {Id = id, UserId = userId, BannedUserId = bannedUserId });
            await _contx.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveUserFromBlackListAsync(long userId, long bannedUserId)
        {
            var bannedUser = await _contx.UserBlacklists
                .Where(u => u.UserId == userId && u.BannedUserId == bannedUserId)
                .SingleOrDefaultAsync();

            if(bannedUser != null)
            {
                _contx.UserBlacklists.Remove(bannedUser);
                await _contx.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<List<Report>> GetAllUserReportsAsync(long userId)
        {
            return await _contx.UserReports.Where(u => u.SenderId == userId)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<byte> BanUserAsync(long userId)
        {
            var user = await _contx.Users.Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (!user.IsBanned)
            {
                user.IsBanned = true;
                _contx.Users.Update(user);
                await _contx.SaveChangesAsync();
                return 1;
            }

            return 0;
        }

        public async Task<byte> UnbanUserAsync(long userId)
        {
            var user = await _contx.Users.Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user.IsBanned)
            {
                user.IsBanned = false;
                user.BanDate = null;

                _contx.Users.Update(user);
                await _contx.SaveChangesAsync();
                return 1;
            }

            return 0;
        }

        public async Task<bool> CheckUserIsBanned(long userId)
        {
            return (await _contx.Users.Where(u => u.Id == userId)
                .Select(u => u.IsBanned)
                .FirstOrDefaultAsync());
        }

        public async Task<bool> CheckUserIsDeleted(long userId)
        {
            return (await _contx.Users.Where(u => u.Id == userId).SingleOrDefaultAsync()).IsDeleted;
        }

        public async Task<string> AddAchievementProgress(long userId, long achievementId, int progress)
        {
            var achievement = await _contx.UserAchievements
                .Where(a => a.UserId == userId && a.AchievementId == achievementId)
                .Include(a => a.Achievement)
                .SingleOrDefaultAsync();

            achievement.Progress += progress;
            _contx.UserAchievements.Update(achievement);
            await _contx.SaveChangesAsync();

            if (achievement.Progress >= achievement.Achievement.ConditionValue)
                return achievement.AcquireMessage;

            return "";
        }

        public async Task<string> GrantAchievementToUser(long userId, long achievementId)
        {
            var achievement = await _contx.UserAchievements
                .Where(a => a.UserId == userId && a.AchievementId == achievementId && !a.IsAcquired)
                .Include(a => a.Achievement)
                .SingleOrDefaultAsync();

            if (achievement == null)
                throw new Exception($"User have already acquired achievement #{achievementId} or it does not exist");

            achievement.IsAcquired = true;

            await TopUpPointBalance(userId, achievement.Achievement.Value, "Achievement acquiring");

            await AddUserNotificationAsync(new UserNotification
            {
                UserId = userId,
                Section = (Section)achievement.Achievement.SectionId,
                Type = NotificationType.Other,
                Description = achievement.AcquireMessage
            });


            _contx.UserAchievements.Update(achievement);
            await _contx.SaveChangesAsync();

            return achievement.AcquireMessage;
        }

        public async Task<List<UserAchievement>> GetUserAchievements(long userId)
        {
            return await _contx.UserAchievements
                .Where(a => a.UserId == userId)
                .Include(a => a.Achievement)
                .ToListAsync();
        }

        public async Task<UserAchievement> GetSingleUserAchievement(long userId, long achievementId)
        {
            return await _contx.UserAchievements
                .Where(a => a.UserId == userId && a.AchievementId == achievementId)
                .Include(a => a.Achievement)
                .FirstOrDefaultAsync();
        }

        public async Task<byte> ReRegisterUser(long userId)
        {

            var sUser = await _contx.Users.Where(u => u.Id == userId)
                .Include(u => u.Data)
                .Include(u => u.Settings)
                .Include(u => u.Location)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var sData = sUser.Data;
            var sSettings = sUser.Settings;

            var userBalance = await _contx.Balances.Where(u => u.UserId == userId)
                .FirstOrDefaultAsync();

            if (userBalance != null)
            {
                _contx.Balances.Remove(userBalance);
                await _contx.SaveChangesAsync();
            }

            var userAchievements = await _contx.UserAchievements.Where(u => u.UserId == userId)
                .ToListAsync();

            if (userAchievements.Count > 0 && userAchievements != null)
            {
                _contx.UserAchievements.RemoveRange(userAchievements);
                await _contx.SaveChangesAsync();
            }

            var userPurchases = await _contx.Transaction.Where(u => u.UserId == userId)
                .ToListAsync();

            if (userPurchases.Count > 0 && userPurchases != null)
            {
                _contx.Transaction.RemoveRange(userPurchases);
                await _contx.SaveChangesAsync();
            }

            var userVisits = await _contx.UserVisits.Where(u => u.UserId == userId)
                .ToListAsync();

            if (userVisits.Count > 0 && userVisits != null)
            {
                _contx.UserVisits.RemoveRange(userVisits);
                await _contx.SaveChangesAsync();
            }

            var userRequests = await _contx.Requests.Where(u => u.SenderId == userId)
                .ToListAsync();

            if (userRequests.Count > 0 && userRequests != null)
            {
                _contx.Requests.RemoveRange(userRequests);
                await _contx.SaveChangesAsync();
            }

            var userNotifications1 = await _contx.Notifications.Where(u => u.UserId == userId)
                .ToListAsync();

            if (userNotifications1.Count > 0 && userNotifications1 != null)
            {
                _contx.Notifications.RemoveRange(userNotifications1);
                await _contx.SaveChangesAsync();
            }

            var sponsorRatings = await _contx.SponsorRatings.Where(u => u.UserId == userId)
                .ToListAsync();

            if (sponsorRatings.Count > 0 && sponsorRatings != null)
            {
                _contx.SponsorRatings.RemoveRange(sponsorRatings);
                await _contx.SaveChangesAsync();
            }

            var userTrustLevel = await _contx.TrustLevels.Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (userTrustLevel != null)
            {
                _contx.TrustLevels.Remove(userTrustLevel);
                await _contx.SaveChangesAsync();
            }

            if (userTrustLevel != null)
            {
                _contx.TrustLevels.Remove(userTrustLevel);
                await _contx.SaveChangesAsync();
            }

            var user = await _contx.Users.Where(u => u.Id == userId)
                .Include(u => u.Data)
                .Include(u => u.Settings)
                .Include(u => u.Location)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                _contx.UserLocations.Remove(user.Location);
                _contx.UserData.Remove(user.Data);
                _contx.UsersSettings.Remove(user.Settings);
                _contx.Users.Remove(user);
            }

            await _contx.SaveChangesAsync();

            if (sUser != null)
            {

                await RegisterUserAsync(new UserRegistrationModel{
                    UserName = sData.UserName,
                    RealName = sData.UserRealName,
                    Age = sData.UserAge,
                    AgePrefs = sData.AgePrefs,
                    AppLanguage = sData.Language,
                    CityCode = sUser.Location.CityId,
                    CountryCode = sUser.Location.CountryId,
                    CommunicationPrefs = sData.CommunicationPrefs,
                    Description = sData.UserDescription,
                    Gender = sData.UserGender,
                    GenderPrefs = sData.UserGenderPrefs,
                    MediaType = sData.MediaType,
                    Media = sData.UserMedia,
                    LanguagePreferences = sData.LanguagePreferences,
                    Languages = sData.UserLanguages,
                    UserLocationPreferences = sData.LocationPreferences,
                    Reason = sData.Reason,
                    UsesOcean = sSettings.UsesOcean,
                });
            }

            return 1;
        }

        public async Task<byte> GenerateUserAchievementList(long userId, AppLanguage localisationId, bool wasRegistered=false)
        {

            List<UserAchievement> userAchievements;

            if (wasRegistered)
            {
                userAchievements = await  _contx.UserAchievements
                    .Where(u => u.UserId == userId)
                    .ToListAsync();
                _contx.UserAchievements.RemoveRange(userAchievements);
            }

            userAchievements = new List<UserAchievement>();
            var sysAchievements = await _contx.Achievements.Where(a => a.Language == localisationId).ToListAsync();
            sysAchievements.ForEach(a => userAchievements.Add(new UserAchievement(a.Id, userId, a.Language, a.Name, a.Description, a.Value, a.Language)));

            await _contx.UserAchievements.AddRangeAsync(userAchievements);
            await _contx.SaveChangesAsync();

            return 1;
        }

        public async Task<List<UserAchievement>> GetUserAchievementsAsAdmin(long userId)
        {
            return await _contx.UserAchievements
                .Where(a => a.UserId == userId && !a.IsAcquired)
                .ToListAsync();
        }

        public async Task<bool> SetUserRtLanguagePrefs(long userId, bool shouldBeConcidered)
        {
            var user = await _contx.UsersSettings.Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            user.ShouldConsiderLanguages = shouldBeConcidered;

            _contx.UsersSettings.Update(user);
            await _contx.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CheckUsersAreCombinableRT(long user1, long user2)
        {
            var userInfo1 = await _contx.Users.Where(u => u.Id == user1)
                .Include(u => u.Data)
                .Include(u => u.Settings)
                .Include(u => u.BlackList)
                .Include(u => u.Encounters)
                .FirstOrDefaultAsync();

            var userInfo2 = await _contx.Users.Where(u => u.Id == user1)
                .Include(u => u.Data)
                .Include(u => u.Settings)
                .Include(u => u.BlackList)
                .Include(u => u.Encounters)
                .FirstOrDefaultAsync();

            //Check if users are not in each others blacklists
            var usersAreNotInBlackList = !userInfo1.BlackList.Any(u => u.UserId == user2) 
                &&
                !userInfo2.BlackList.Any(u => u.UserId == user1);

            if (usersAreNotInBlackList)
            {
                //Check if user1 has encountered user2
                //In that case, checking 1 encounter is enough
                if (!userInfo1.Encounters.Any(e => e.UserId == user2))
                {   
                    //If both consider having the same languages
                    if(userInfo1.Settings.ShouldConsiderLanguages && userInfo2.Settings.ShouldConsiderLanguages)
                    {
                        await AddUserTrustProgressAsync(user1, 0.000005 * (double)userInfo1.BonusIndex);
                        await AddUserTrustProgressAsync(user2, 0.000005 * (double)userInfo2.BonusIndex);

                        var result = userInfo1.Data.UserLanguages.Any(l => userInfo2.Data.LanguagePreferences.Contains(l))
                            && userInfo2.Data.UserLanguages.Any(l => userInfo1.Data.LanguagePreferences.Contains(l));

                        if (result)
                        {
                            await RegisterUserEncounter(new RegisterEncounter { UserId = user1, EncounteredUserId = user2, Section = Section.RT });
                            await RegisterUserEncounter(new RegisterEncounter { UserId = user2, EncounteredUserId = user1, Section = Section.RT });
                        }

                        return result;
                    }
                    //If user1 considers having the same languages
                    else if (userInfo1.Settings.ShouldConsiderLanguages)
                    {
                        await AddUserTrustProgressAsync(user1, 0.000005 * (double)userInfo1.BonusIndex);
                        await AddUserTrustProgressAsync(user2, 0.000005 * (double)userInfo2.BonusIndex);

                        var result = userInfo2.Data.UserLanguages.Any(l => userInfo1.Data.LanguagePreferences.Contains(l));

                        if (result)
                        {
                            await RegisterUserEncounter(new RegisterEncounter { UserId = user1, EncounteredUserId = user2, Section = Section.RT});
                            await RegisterUserEncounter(new RegisterEncounter { UserId = user2, EncounteredUserId = user1, Section = Section.RT });
                        }

                        return result;
                    }
                    //If user2 considers having the same languages
                    else if (userInfo2.Settings.ShouldConsiderLanguages)
                    {
                        await AddUserTrustProgressAsync(user1, 0.000005 * (double)userInfo1.BonusIndex);
                        await AddUserTrustProgressAsync(user2, 0.000005 * (double)userInfo2.BonusIndex);

                        var result = userInfo1.Data.UserLanguages.Any(l => userInfo2.Data.LanguagePreferences.Contains(l));

                        if (result)
                        {
                            await RegisterUserEncounter(new RegisterEncounter { UserId = user1, EncounteredUserId = user2, Section = Section.RT});
                            await RegisterUserEncounter(new RegisterEncounter { UserId = user2, EncounteredUserId = user1, Section = Section.RT });
                        }

                        return result;
                    }

                    await AddUserTrustProgressAsync(user1, 0.000005 * (double)userInfo1.BonusIndex);
                    await AddUserTrustProgressAsync(user2, 0.000005 * (double)userInfo2.BonusIndex);

                    await RegisterUserEncounter(new RegisterEncounter { UserId = user1, EncounteredUserId = user2, Section = Section.RT });
                    await RegisterUserEncounter(new RegisterEncounter { UserId = user2, EncounteredUserId = user1, Section = Section.RT });

                    //If neither considers having the same languages
                    return true;
                }
                return false;
            }
            return false;
        }

        public async Task<Balance> GetUserWalletBalance(long userId)
        {
            return await _contx.Balances
                .Where(b => b.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<float> TopUpPointBalance(long userId, float points, string description = "")
        {
            var time = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
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

                _contx.Balances.Update(userBalance);
                await _contx.SaveChangesAsync();
            }
            else
            {
                await CreateUserBalance(userId, points, time);
                userBalance = await GetUserWalletBalance(userId);
            }

            var userParentId = await _contx.Users.Where(u => u.Id == userId)
                .Select(u => u.ParentId)
                .FirstOrDefaultAsync();

            if (points > 0 && userParentId != null && userParentId > 0)
            {
                var parent = await _contx.Users.Where(u => u.Id == userParentId)
                    .Include(u => u.Data)
                    .Include(u => u.Settings)
                    .Include(u => u.Location)
                    .FirstOrDefaultAsync();

                if (parent != null)
                    await TopUpPointBalance((long)userParentId, (int)(points + points * parent.InvitedUsersBonus), $"Referential reward for user's {userId} action");
            }

            await _contx.SaveChangesAsync();
            await RegisterUserPurchaseInPoints(userId, points, description); //Registers info regarding amount of points decremented / incremented

            return userBalance.Points;
        }

        private async Task CreateUserBalance(long userId, float points, DateTime time)
        {
           var userBalance = new Balance(userId, points, time);

            await _contx.Balances.AddAsync(userBalance);
            await _contx.SaveChangesAsync();
        }

        public async Task<int> TopUpOPBalance(long userId, int points, string description = "")
        {
            var time = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            var userBalance = await GetUserWalletBalance(userId);

            if (userBalance != null)
            {
                if (userBalance.OceanPoints + points <= 0)
                    userBalance.OceanPoints = 0;
                else if (userBalance.OceanPoints + points >= int.MaxValue)
                    userBalance.OceanPoints = int.MaxValue;
                else
                    userBalance.OceanPoints += points;

                userBalance.PointInTime = time;

                _contx.Balances.Update(userBalance);
                await _contx.SaveChangesAsync();
            }
            else
            {
                await CreateUserBalance(userId, points, time);
            }

            var userParentId = await _contx.Users.Where(u => u.Id == userId)
                .Select(u => u.ParentId)
                .FirstOrDefaultAsync();

            if (userParentId != null && userParentId > 0)
                await TopUpPointBalance((long)userParentId, 1, $"Referential reward for user's {userParentId} action");

            await _contx.SaveChangesAsync();
            await RegisterUserPurchaseInPP(userId, points, description); //Registers info about amount of points decremented / incremented

            return userBalance.OceanPoints;
        }

        private async Task<bool> RegisterUserPurchaseInPoints(long userId, float points, string description)
        {
            return await RegisterUserPurchase(userId, points, description, Currency.Points);
        }

        private async Task<bool> RegisterUserPurchaseInPP(long userId, float points, string description)
        {
            return await RegisterUserPurchase(userId, points, description, Currency.OceanPoints);
        }

        private async Task<bool> RegisterPurchaseInRealMoney(long userId, float points, string description, Currency currency)
        {
            return await RegisterUserPurchase(userId, points, description, currency);
        }

        private async Task<bool> RegisterUserPurchase(long userId, float amount, string description, Currency currency)
        {
            var purchase = new Transaction
            {
                UserId = userId,
                PointInTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                Amount = amount,
                Description = description,
                Currency = currency
            };

            await _contx.Transaction.AddAsync(purchase);
            await _contx.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CheckUserHasPremiumAsync(long userId)
        {
            var timeNow = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

            var user = await _contx.Users.Where(u => u.Id == userId)
                .FirstOrDefaultAsync();
                
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

        public async Task<DateTime> GetPremiumExpirationDate(long userId)
        {

            var timeNow = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
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

        public async Task<DateTime> GrantPremiumToUser(long userId, float cost, int dayDuration, Currency currency)
        {
            var timeNow = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            var premiumFutureExpirationDate = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(dayDuration), DateTimeKind.Utc);

            var user = await _contx.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            var balance = await GetUserWalletBalance(userId);

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
            if (currency == Currency.Points)
                await TopUpPointBalance(userId, -cost, $"Purchase premium for {dayDuration} days");
            //If transaction was made for real money
            else if (currency == Currency.RealMoney)
                await RegisterPurchaseInRealMoney(userId, -cost, $"Purchase premium for {dayDuration} days", (Currency)balance.Currency);

            //Reward for premium purchase
            await TopUpPointBalance(userId, 500, "Points received for premium purchase");

            //OP Reward for purchasing long-term premium
            //TODO: Think if the amount is properly set...
            if (dayDuration >= 30)
                await TopUpOPBalance(userId, 5, "Ocean+ points received for premium purchase");

            if (user.PremiumExpirationDate < timeNow || user.PremiumExpirationDate == null)
                user.PremiumExpirationDate = premiumFutureExpirationDate;
            else
                user.PremiumExpirationDate = user.PremiumExpirationDate.Value.AddDays(dayDuration);

            await RegisterUserPurchase(userId, dayDuration, $"{dayDuration} of Premium received for {cost} {currency}", Currency.Premium);

            _contx.Update(user);
            await _contx.SaveChangesAsync();

            await AddUserNotificationAsync(new UserNotification { UserId = user.Id, Type = NotificationType.PremiumAcquire, Section = Section.Neutral, Description = $"You have been granted premium access. Enjoy your benefits :)\nPremium expiration {user.PremiumExpirationDate.Value.ToString("dd.MM.yyyy")}" });

            return user.PremiumExpirationDate.Value;
        }

        private async Task<User> GetUserWithPremium(long userId, DateTime timeNow)
        {
            return await _contx.Users
                .Where(u => u.Id == userId && (bool)u.HasPremium && u.PremiumExpirationDate > timeNow)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CheckBalanceIsSufficient(long userId, int cost)
        {
            cost = cost < 0 ? cost * -1 : cost; //Makes sure the cost amount wasnt minus value
            return (await GetUserWalletBalance(userId)).Points >= cost;
        }

        public async Task<byte> UpdateUserAppLanguageAsync(long userId, AppLanguage appLanguage)
        {
            var userData = await _contx.UserData.Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (userData != null)
            {
                if (userData.Language != appLanguage) // Check if user had changed an app language to a different one
                {
                    var userAchievements = await _contx.UserAchievements.Where(a => a.UserId == userId).ToListAsync();
                    userAchievements.ForEach(async a =>
                    {
                        a.AchievementLanguage = appLanguage;
                        var achievement = await _contx.Achievements
                        .Where(achievement => achievement.Id == a.AchievementId && achievement.Language == appLanguage)
                        .SingleOrDefaultAsync();

                        a.RetranslateAquireMessage(achievement, appLanguage);
                    });
                    _contx.UserAchievements.UpdateRange(userAchievements);
                    await _contx.SaveChangesAsync();

                    var userLocation = await _contx.UserLocations.Where(l => l.Id == userId).SingleOrDefaultAsync();
                    userLocation.CityCountryLang = appLanguage;
                    //userLocation.CityCountryClassLocalisationId = appLanguage; // Uncomment when Cities will be translated on another languages

                    _contx.UserLocations.Update(userLocation);
                    await _contx.SaveChangesAsync();

                    userData.Language = appLanguage;
                    _contx.UserData.Update(userData);
                    await _contx.SaveChangesAsync();
                }
                return 1;
            }
            return 0;
        }

        public async Task UpdateUserAsync(UpdateUserProfile model)
        {
            var countryName = "---";
            var cityName = "---";

            var user = await _contx.Users.Where(u => u.Id == model.Id)
                .Include(u => u.Data)
                .Include(u => u.Location)
                .FirstOrDefaultAsync();

            var location = user.Location;

            if (location.CountryId != null)
            {
                countryName = await _contx.Countries
                    .Where(c => c.Id == location.CountryId && c.Lang == location.CityCountryLang)
                    .Select(c => c.CountryName)
                    .FirstOrDefaultAsync();
                cityName = await _contx.Cities
                    .Where(c => c.Id == location.CityId && c.CountryLang == location.CountryLang)
                    .Select(c => c.CityName)
                    .FirstOrDefaultAsync(); ;
            }

            user.Data.UserRawDescription = model.Description;
            user.Data.UserDescription = user.GenerateUserDescription(model.RealName, model.Age, countryName, cityName, model.Description);

            //Reactivate user tick request if user's media had been changed
            if (model.Media != user.Data.UserMedia)
            {
                user.Data.UserMedia = model.Media;
                user.Data.MediaType = model.MediaType;
                await ReactivateTickRequest(user.Id);
            }

            user.Data.UserRealName = model.RealName;
            user.Data.UserAge = model.Age;
            user.Data.UserGender = model.Gender;
            user.Data.UserLanguages = model.Languages;
            user.Data.Reason = model.Reason;

            user.Data.AgePrefs = model.AgePrefs;
            user.Data.LanguagePreferences = model.LanguagePreferences;
            user.Data.LocationPreferences = model.LocationPreferences;
            user.Data.CommunicationPrefs = model.CommunicationPrefs;
            user.Data.UserGenderPrefs = model.GenderPrefs;

            location.CountryId = model.CountryCode;
            location.CityCountryLang = model.CountryCode != null? model.AppLanguageId : null;
            location.CityId = model.CityCode;
            location.CountryLang = model.CityCode != null? model.AppLanguageId : null;

            // Set tags
            if (!string.IsNullOrEmpty(model.Tags))
                await UpdateTags(new UpdateTags { UserId = model.Id, RawTags = model.Tags });

            //Set auto reply
            if (string.IsNullOrEmpty(model.Voice))
                await SetAutoReplyVoiceAsync(model.Id, model.Voice);
            else if (string.IsNullOrEmpty(model.Text))
                await SetAutoReplyTextAsync(model.Id, model.Text);

            _contx.Users.Update(user);
            _contx.UserData.Update(user.Data);
            _contx.UserLocations.Update(location);
            await _contx.SaveChangesAsync();
        }

        public async Task<bool> CheckUserIsBusy(long userId)
        {
            var user = await _contx.Users.Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user == null)
                return false;

            return user.IsBusy; 
        }

        public async Task<SwitchBusyStatusResponse> SwhitchUserBusyStatus(long userId, Section section)
        {
            var user = await _contx.Users.Where(u => u.Id == userId)
                .Include(u => u.Data)
                .Include(u => u.Settings)
                .FirstOrDefaultAsync();

            if  (user != null)
            {
                var hint = "";

                user.IsUpdated = false;

                _contx.Update(user);
                await _contx.SaveChangesAsync();

                if (user.IsBusy && section != Section.MainMenu)
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

                if(section == Section.MainMenu)
                    user.IsBusy = false;
                else
                    user.IsBusy = true;

                if (user.Settings.ShouldSendHints)
                    hint = await GetRandomHintAsync(user.Data.Language, null);


                return new SwitchBusyStatusResponse
                {
                    Status = SwitchBusyStatusResult.Success,
                    Comment = hint,
                    HasVisited = await CheckUserHasVisitedSection(userId, section),

                };
            }
            return new SwitchBusyStatusResponse
            {
                Status = SwitchBusyStatusResult.DoesNotExist,
                HasVisited = false
            };
        }

        public async Task<List<GetRequests>> GetUserRequests(long userId)
        {
            var requests = await _contx.Requests
                .Where(r => r.UserId == userId && r.Answer == null)
                .OrderByDescending(r => r.Type)
                .Select(r => new GetRequests
                {
                    Id = r.Id,
                    SenderId = r.SenderId,
                    Message = r.Message,
                    SystemMessage = r.SystemMessage,
                    IsMatch = r.IsMatch,
                    Type = r.Type,
                    UserId = r.UserId
                })
                .ToListAsync();

            return requests;
        }

        public async Task<UserNotification> GetUserRequest(long requestId)
        {
            return await _contx.Notifications
                .Where(r => r.Id == requestId)
                .Where(r => r.Section == Section.Familiator || r.Section == Section.Requester)
                .FirstOrDefaultAsync();
        }

        public async Task<string> RegisterUserRequestAsync(AddRequest model)
        {
            //var notification = new UserNotification {
            //    UserId = model.UserId,
            //    Type = NotificationType.LikeNotification,
            //    Description = "<b>Someone had liked you</b>"
            //};

            var request = new Request
            {
                UserId = model.UserId,
                SenderId = model.SenderId,
                IsMatch = model.IsMatch,
                Message = model.Message,
                Type = model.MessageType
            };

            var returnMessage = "";

            //notification.Section = Section.Familiator;


            await RegisterUserEncounter(new RegisterEncounter { UserId = (long)model.SenderId, EncounteredUserId = model.UserId, Section = Section.Familiator});


            //Register request
            await _contx.Requests.AddAsync(request);

            var requestsCount = await _contx.Notifications.Where(n => n.Type == NotificationType.Like)
                .AsNoTracking()
                .CountAsync();

            //if (requestsCount > 3 && requestsCount < 7)
            //    notification.Description = "Your profile got some attention! See who has shown interest in you!";
            //else if (requestsCount > 7 && requestsCount < 10)
            //    notification.Description = "Your attractiveness has not gone unnoticed! See who's interested in you";
            //else if (requestsCount > 10)
            //    notification.Description = "Your profile is the center of attention! Check out who's showing interest in you!";

            ////Register notification
            //await AddUserNotificationAsync(notification);

            return returnMessage;
        }

        //TODO: Make more informative and interesting
        public async Task<string> DeclineRequestAsync(long user, long encounteredUser)
        {
            var sim = await GetSimilarityBetweenUsersAsync(user, encounteredUser);

            //Encounter is not registered anywhere but here in that case
            await RegisterUserEncounter(new RegisterEncounter
            {
                UserId = user,
                EncounteredUserId = encounteredUser,
                Section = Section.Familiator
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

        private async Task<string> AcceptUserRequestAsync(long userId, long senderId)
        {
            var returnMessage = "";
            var request = new Request
            {
                SenderId = userId,
                UserId = senderId,
                IsMatch = true
            };

            if (new Random().Next(0, 2) == 0)
            {
                var senderUserName = await _contx.UserData.Where(d => d.Id == userId)
                    .AsNoTracking()
                    .Select(d => d.UserName)
                    .FirstOrDefaultAsync();

                //Delete request, user had just responded to
                //var requestId = await _contx.Requests.Where(n => n.SenderId == model.UserId && n.UserId == model.SenderId)
                //    .AsNoTracking()
                //    .Select(n => n.Id)
                //    .FirstOrDefaultAsync();
                //await DeleteUserRequest(requestId);


                //TODO: Get message from localizer based on users`s localization 
                request.SystemMessage = $"Hey! I have got a match for you. This person was notified about it, but did not receive your username, thus he / she cannot write you first everything is in your hands, do not miss your chance!\n\n@{senderUserName}";
                returnMessage = "Hey! I have a match for you. Right now this person is deciding whether or not to write you Just wait for it!\n\n";
            }
            else
            {
                var receiverUserName = await _contx.UserData
                    .Where(d => d.Id == senderId)
                    .AsNoTracking()
                    .Select(d => d.UserName)
                    .FirstOrDefaultAsync();

                //Delete request, user had just answered
                //var requestId = await _contx.Notifications.Where(n => n.SenderId == model.UserId && n.UserId == model.SenderId)
                //    .AsNoTracking()
                //    .Select(n => n.Id)
                //    .FirstOrDefaultAsync();
                //await DeleteUserRequest(requestId);


                //TODO: Get message from localizer based on users`s localization 
                request.SystemMessage = "Hey! I have a match for you. Right now this person is deciding whether or not to write you Just wait for it!\n\n";
                returnMessage = $"Hey! I have got a match for you. This person was notified about it, but he did not receive your username, thus he cannot write you first everything is in your hands, do not miss your chance!\n\n@{receiverUserName}";
            }

            await _contx.Requests.AddAsync(request);
            await _contx.SaveChangesAsync();

            return returnMessage;
        }

        public async Task<string> AnswerUserRequestAsync(long requestId, RequestAnswer reaction)
        {
            var request = await _contx.Requests.Where(r => r.Id == requestId)
                .FirstOrDefaultAsync();

            request.Answer = reaction;
            request.AnsweredTimeStamp = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            await _contx.SaveChangesAsync();

            switch (reaction)
            {
                case RequestAnswer.Accept:
                    return await AcceptUserRequestAsync(request.UserId, request.SenderId);
                case RequestAnswer.Decline:
                    return await DeclineRequestAsync(request.UserId, request.SenderId);
                default:
                    return "";
            }
        }

        public async Task<byte> DeleteUserRequests(long userId)
        {
            var requests = await _contx.Notifications
                .Where(r => r.UserId == userId)
                .Where(r => r.Section == Section.Familiator || r.Section == Section.Requester)
                .ToListAsync();

            _contx.RemoveRange(requests);
            await _contx.SaveChangesAsync();

            return 1;
        }

        public async Task DeleteUserRequest(long requestId)
        {
            var request = await _contx.Requests
                .Where(r => r.Id == requestId)
                .FirstOrDefaultAsync();

            if (request != null)
            {
                _contx.Remove(request);
                await _contx.SaveChangesAsync();
            }
        }

        public async Task<bool> CheckUserHasRequests(long userId)
        {
            var requests = await _contx.Notifications
                .Where(r => r.UserId == userId)
                .Where(r => r.Section == Section.Familiator || r.Section == Section.Requester)
                .ToListAsync();
                
            return requests.Count > 0;
        }

        public async Task<bool> SetDebugProperties()
        {
            var encounters = await _contx.Encounters.ToListAsync();

            _contx.Encounters.RemoveRange(encounters);

            var users = await _contx.Users.ToListAsync();

            users.ForEach(u => u.IsBusy = false);
            _contx.Users.UpdateRange(users);

            await _contx.SaveChangesAsync();
            return true;
        }

        public async Task RegisterUserEncounter(RegisterEncounter model)
        {
            var user = await _contx.Users.FindAsync(model.UserId);

            if (model.Section == Section.Familiator || model.Section == Section.Requester)
            {
                user.ProfileViewsCount++;

                if (user.ProfileViewsCount == 15)
                    await TopUpPointBalance(user.Id, 9, "User viewed 15 profiles");
                else if (user.ProfileViewsCount == 30)
                    await TopUpPointBalance(user.Id, 15, "User viewed 30 profiles");
                else if (user.ProfileViewsCount == 50)
                    await TopUpPointBalance(user.Id, 22, "User viewed 50 profiles");
            }
            else if (model.Section == Section.RT)
                user.RTViewsCount++;

            await _contx.Encounters.AddAsync(new Encounter(model));
            
            await _contx.SaveChangesAsync();
        }

        public async Task<Encounter> GetUserEncounter(long encounterId)
        {
            return await _contx.Encounters.FindAsync(encounterId);
        }

        public async Task<List<Encounter>> GetUserEncounters(long userId, Section section)
        {
            var encounters = await _contx.Encounters
                .Where(e => e.UserId == userId)
                .Where(e => e.Section == section)
                .Include(e => e.EncounteredUser)
                .AsNoTracking()
                .ToListAsync();

            return encounters != null ? encounters : new List<Encounter>();
        }

        public async Task<bool> CheckRequestExists(long senderId, long recieverId)
        {
            return await _contx.Requests
                .Where(r => r.SenderId == senderId && r.UserId == recieverId)
                .AnyAsync();
        }

        public async Task<int> AddUserTrustProgressAsync(long userId, double progress)
        {

            var userBonus = await GetUserBonusIndex(userId);

            var model = await _contx.TrustLevels
                .Where(l => l.Id == userId).FirstOrDefaultAsync();

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

                _contx.TrustLevels.Update(model);
                await _contx.SaveChangesAsync();

                return model.Level;
            }
            return -1;
        }

        public async Task<int> UpdateUserTrustLevelAsync(long userId, int level)
        {
            var model = await _contx.TrustLevels
                .FindAsync(userId);

            model.Level = level;

            _contx.Update(model);
            await _contx.SaveChangesAsync();
            return model.Level;
        }

        public async Task<UserTrustLevel> GetUserTrustLevel(long userId)
        {
            return await _contx.TrustLevels
                .FindAsync(userId);
        }

        private async Task<long> AddUserTrustLevel(long userId)
        {
            await _contx.TrustLevels.AddAsync(UserTrustLevel.CreateDefaultTrustLevel(userId));
            await _contx.SaveChangesAsync();
            return userId;
        }

        public async Task<bool> UpdateUserNickname(long userId, string nickname)
        {
            var currentUser = await _contx.Users.FindAsync(userId);

            if (currentUser.HasPremium)
            {
                currentUser.Nickname = nickname;
                _contx.Users.Update(currentUser);
                await _contx.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<string> GetUserNickname(long userId)
        {
            var currentUser = await _contx.Users.FindAsync(userId);

            if (currentUser != null)
                return currentUser.Nickname;
            return "";
        }

        public async Task<string> ClaimDailyReward(long userId)
        {
            var user = await _contx.Users.Where(u => u.Id == userId)
                .Include(u => u.Data)
                .Include(u => u.Settings)
                .Include(u => u.Location)
                .FirstOrDefaultAsync();

            try
            {
                var reward = await _contx.DailyRewards
                    .Where(r => r.Index == user.DailyRewardPoint)
                    .Select(r => r.PointReward)
                    .FirstOrDefaultAsync();

                await TopUpPointBalance(userId, reward * (short)user.BonusIndex, "Daily reward");
                user.HadReceivedReward = true;
                user.DailyRewardPoint += 1;

                _contx.Users.Update(user);
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
            var user = await _contx.Users.Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

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
                return (short)await _contx.Users
                    .Where(u => u.Id == userId)
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

                invitationCreds.QRCode = GenerateQrCode(invitationCreds.Link);

                await _contx.InvitationCredentials.AddAsync(invitationCreds);
                await _contx.SaveChangesAsync();
            }

            return invitationCreds;
        }

        //Generate QR code on user request 
        public async Task<string> GetQRCode(long userId)
        {
            string data;

            var creds = await _contx.InvitationCredentials.Where(c => c.UserId == userId)
                .FirstOrDefaultAsync();

            if (creds != null)
            {
                if (creds.QRCode != null)
                    return creds.QRCode;

                data = GenerateQrCode(creds.Link);

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

        private string GenerateQrCode(string link)
        {
            link = link.Replace("%2F", "/");

            var generator = new QRCodeGenerator();
            var d = generator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
            var code = new PngByteQRCode(d).GetGraphic(5);

            var data = Convert.ToBase64String(code);

            return data;
        }

        public async Task<InvitationCredentials> GetInvitationCredentialsByUserIdAsync(long userId)
        {
            return await _contx.InvitationCredentials
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
            var invitationCreds = await _contx.InvitationCredentials.FindAsync(invitationId);

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
                    InviterCredentialsId = invitationCreds.Id,
                    InvitedUserId = userId,
                    InvitationTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
                };

                await _contx.Invitations.AddAsync(invitation);
                await _contx.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<Invitation> GetInvitationAsync(long userId)
        {
            return await _contx.Invitations
                .Where(i => i.InvitedUserId == userId)
                .Include(i => i.InviterCredentials).ThenInclude(i => i.Inviter)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> NotifyUserAboutReferentialRegistrationAsync(long userId, long invitedUserId)
        {
            if (await CheckUserIsRegistered(userId))
            {
                var invitedUsersCount = await GetInvitedUsersCountAsync(userId);

                await AddUserNotificationAsync(new UserNotification
                {
                    UserId = userId,
                    Description = $"Hey! new user had been registered via your link. Thanks for helping us grow!\nSo far, you have invited: {invitedUsersCount} people. \nYou receive 1p for every action they are maiking ;-)",
                    Section = Section.Registration,
                    Type = NotificationType.ReferentialRegistration
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
                await AddUserNotificationAsync(new UserNotification {UserId=userId, Type=NotificationType.PremiumEnd, Section=Section.Neutral, Description="Your premium access has expired"});
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<long> AddUserNotificationAsync(UserNotification model)
        {
            try
            {
                if (model.Type == NotificationType.PremiumAcquire ||
                    model.Type == NotificationType.TickRequest || 
                    model.Type == NotificationType.PremiumEnd ||
                    model.Type == NotificationType.LikeNotification ||
                    model.Type == NotificationType.ReferentialRegistration)
                {
                    var notification = await _contx.Notifications.Where(n => n.Type == model.Type)
                        .FirstOrDefaultAsync();

                    if (notification != null)
                        notification.Description = model.Description;
                    else
                        await _contx.Notifications.AddAsync(model);

                    await _contx.SaveChangesAsync();
                }

                else if (model.Type == NotificationType.AdventureParticipation)
                {
                    var notification = await _contx.Notifications.Where(n => n.Type == model.Type)
                        .FirstOrDefaultAsync();

                    if (notification != null)
                    {
                        var attendeesCount = await _contx.Adventures.Where(a => a.UserId == model.UserId)
                            .SelectMany(a => a.Attendees.Where(at => at.Status == AdventureAttendeeStatus.New))
                            .CountAsync();

                        // +1 existing one
                        notification.Description = $"{attendeesCount +1} people had requested participation in your adventure";
                    }
                    else
                    {
                        await _contx.Notifications.AddAsync(model);
                    }

                    await _contx.SaveChangesAsync();
                }

                else if (model.Type == NotificationType.AdventureParticipationByCode)
                {
                    var notification = await _contx.Notifications.Where(n => n.Type == model.Type)
                        .FirstOrDefaultAsync();

                    if (notification != null)
                    {
                        var attendeesCount = await _contx.Adventures.Where(a => a.UserId == model.UserId)
                            .SelectMany(a => a.Attendees.Where(at => at.Status == AdventureAttendeeStatus.NewByCode))
                            .CountAsync();

                        // +1 existing one
                        notification.Description = $"{attendeesCount +1} people had requested participation in your adventure by unique code";
                    }
                    else
                    {
                        await _contx.Notifications.AddAsync(model);
                    }

                    await _contx.SaveChangesAsync();
                }

                else
                {
                    await _contx.Notifications.AddAsync(model);
                    await _contx.SaveChangesAsync();
                }

                return model.Id;
            }
            catch { throw new Exception("Something went wrong while adding notification"); }
        }

        public async Task<int> GetInvitedUsersCountAsync(long userId)
        {
            return await _contx.Invitations
                .Where(i => i.InviterCredentials.UserId == userId)
                .CountAsync();
        }

        public async Task<bool> CheckUserHasNotificationsAsync(long userId)
        {
            return await _contx.Notifications
                .Where(n => n.UserId == userId && n.Section != Section.Familiator && n.Section != Section.Requester)
                .CountAsync() > 0;
        }

        public async Task<List<UserNotification>> GetUserNotifications(long userId)
        {
            return await _contx.Notifications
                .Where(n => n.UserId == userId)
                .Where(n => n.Type != NotificationType.Like)
                .ToListAsync();
        }

        private async Task<bool> DeleteUserNotification(UserNotification notification)
        {
            try
            {
                if (notification != null)
                {
                    _contx.Notifications.Remove(notification);
                    await _contx.SaveChangesAsync();

                    return true;
                }
                return false;
            }
            catch { return false; }
        }

        public async Task<List<string>> GetRandomAchievements(long userId)
        {
            var achievents = await _contx.UserAchievements
                .Where(a => a.UserId == userId)
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

        public async Task<int> GetUserMaximumLanguageCountAsync(long userId)
        {
            var user = await _contx.Users.FindAsync(userId);

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

        public async Task<UserNotification> GetUserNotificationAsync(long notificationId)
        {
            var notification = await _contx.Notifications.Where(n => n.Id == notificationId)
                .FirstOrDefaultAsync();

            return notification;
        }

        public async Task<byte> DeleteNotificationAsync(long notificationId)
        {
            var notification = await _contx.Notifications.Where(n => n.Id == notificationId)
                .FirstOrDefaultAsync();

            if (notification == null)
                return 0;

            if (await DeleteUserNotification(notification))
                return 1;

            return 0;
        }

        public async Task<List<long>> GetUserNotificationsIdsAsync(long userId)
        {
            var ids = await _contx.Notifications.Where(n => n.UserId == userId)
                .Select(n => n.Id)
                .ToListAsync();

            return ids;
        }

        public async Task<int> GetUserPersonalityPointsAmount(long userId)
        {
            try
            {
                return await _contx.Balances.Where(b => b.UserId == userId)
                    .Select(b => b.OceanPoints)
                    .SingleOrDefaultAsync();
            }
            catch(NullReferenceException) { return 0; }
        }

        //Is used when user has passed a test
        public async Task<bool> UpdateOceanStatsAsync(TestPayload model)
        {
            try
            {
                var result = await RecalculateUserStats(model);

                //Break if test result wasn't saved
                if (!await RegisterTestPassingAsync(model, result.TestResult))
                    return false;

                // Add tags without duplciates
                if (model.Tags != null)
                {
                    var userTags = await _contx.UserTags.Where(t => t.UserId == model.UserId)
                        .Select(t => t.TagId)
                        .ToListAsync();

                    var newTags = model.Tags.Distinct()
                        .Where(t => !userTags.Contains(t))
                        .Select(t => new UserTag(t, model.UserId, TagType.Tests));

                    await _contx.UserTags.AddRangeAsync(newTags);
                }

                await _contx.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> UpdateUserPersonalityPoints(PointsPayload model)
        {
            var points = await RecalculateSimilarityPercentage(model);
            var balance = await _contx.Balances.Where(b => b.UserId == model.UserId)
                .FirstOrDefaultAsync();

            balance.OceanPoints = model.Balance;

            await _contx.SaveChangesAsync();

            return true;
        }

        public async Task<OceanPoints> GetUserPersonalityPoints(long userId)
        {
            var points = await _contx.OceanPoints
                    .Where(s => s.UserId == userId)
                    .FirstOrDefaultAsync();

            if (points == null)
            {
                points = new OceanPoints(userId);
                await _contx.OceanPoints.AddAsync(points);
            }

            return points;
        }

        private async Task<OceanPoints> RecalculateSimilarityPercentage(PointsPayload model)
        {
            try
            {
                var points = await GetUserPersonalityPoints(model.UserId);

                if (model.Openness != null)
                {
                    points.OpennessPercentage = await CalculateSimilarityPreferences((int)model.Openness, points.OpennessPercentage);
                    points.Openness = (int)model.Openness;
                }

                if (model.Conscientiousness != null)
                {
                    points.ConscientiousnessPercentage = await CalculateSimilarityPreferences((int)model.Conscientiousness, points.ConscientiousnessPercentage);
                    points.Conscientiousness = (int)model.Conscientiousness;
                }

                if (model.Extroversion != null)
                {
                    points.ExtroversionPercentage = await CalculateSimilarityPreferences((int)model.Extroversion, points.ExtroversionPercentage);
                    points.Extroversion = (int)model.Extroversion;
                }

                if (model.Agreeableness != null)
                {
                    points.AgreeablenessPercentage = await CalculateSimilarityPreferences((int)model.Agreeableness, points.AgreeablenessPercentage);
                    points.Agreeableness = (int)model.Agreeableness;
                }


                if (model.Neuroticism != null)
                {
                    points.NeuroticismPercentage = await CalculateSimilarityPreferences((int)model.Neuroticism, points.NeuroticismPercentage);
                    points.Nature = (int)model.Neuroticism;
                }

                if (model.Nature != null)
                {
                    points.NaturePercentage = await CalculateSimilarityPreferences((int)model.Nature, points.NaturePercentage);
                    points.Nature = (int)model.Nature;
                }

                return points;
            }
            catch { return null; }
        }

        private async Task<RecalculationResult> RecalculateUserStats(TestPayload model)
        {
            var devider = 1;
            var result = 0f;
            var userStats = await _contx.OceanStats.Where(s => s.UserId == model.UserId)
                .FirstOrDefaultAsync();

            //Create user stats if they werent created before
            if (userStats == null)
            {
                userStats = new Entities.UserInfoEntities.OceanStats(model.UserId);
                await _contx.OceanStats.AddAsync(userStats);
                await _contx.SaveChangesAsync();
            }

            if (model.Openness != 0)
            {
                //Set the devider to 2 if user passed the tests of the same type before
                if (userStats.Openness != 0)
                    devider = 2;

                result = (float)model.Openness;
                userStats.Openness = (userStats.Openness + (float)model.Openness) / devider;
                //Return the devider to its normal state
                devider = 1;
            }
            if (model.Conscientiousness != 0)
            {
                if (userStats.Conscientiousness != 0)
                    devider = 2;

                result = (float)model.Conscientiousness;
                userStats.Conscientiousness = (userStats.Conscientiousness + (float)model.Conscientiousness) / devider;
                devider = 1;
            }
            if (model.Extroversion != 0)
            {
                if (userStats.Extroversion != 0)
                    devider = 2;

                result = (float)model.Extroversion;
                userStats.Extroversion = (userStats.Extroversion + (float)model.Extroversion) / devider;
                devider = 1;
            }
            if (model.Agreeableness != 0)
            {
                if (userStats.Agreeableness != 0)
                    devider = 2;

                result = (float)model.Agreeableness;
                userStats.Agreeableness = (userStats.Agreeableness + (float)model.Agreeableness) / devider;
                devider = 1;
            }
            if (model.Neuroticism != 0)
            {
                if (userStats.Neuroticism != 0)
                    devider = 2;

                result = (float)model.Neuroticism;
                userStats.Neuroticism = (userStats.Neuroticism + (float)model.Neuroticism) / devider;
                devider = 1;
            }
            if (model.Nature != 0)
            {
                if (userStats.Nature != 0)
                    devider = 2;

                result = (float)model.Nature;
                userStats.Nature = (userStats.Nature + (float)model.Nature) / devider;
                devider = 1;
            }

            await _contx.SaveChangesAsync();
            return new RecalculationResult { Stats = userStats, TestResult = result};
        }

        private async Task<float> CalculateSimilarityPreferences(int points, float similarityCoefficient)
        {
            if (points == 0)
                return 1;

            await Task.Run(() =>
            {
                var deviationCoefficient = 0f;
                for (int i = 1; i < points; i++)
                {
                    var speedCoefficient = 10;

                    deviationCoefficient = (i * similarityCoefficient) / speedCoefficient;
                    similarityCoefficient -= deviationCoefficient / (points / deviationCoefficient);
                }
            });

            return similarityCoefficient;
        }

        public async Task<bool> SwitchOceanUsageAsync(long userId)
        {
            try
            {
                var settings = await _contx.UsersSettings.Where(s => s.Id == userId)
                    .FirstOrDefaultAsync();
                
                if (settings.UsesOcean)
                {
                    settings.UsesOcean = false;
                }
                else
                {
                    settings.UsesOcean = true;

                    var oceanStats = await _contx.OceanStats.Where(s => s.UserId == userId)
                        .FirstOrDefaultAsync();

                    var oceanPoints = await _contx.OceanPoints.Where(s => s.UserId == userId)
                        .FirstOrDefaultAsync();

                    //Add ocean stats, if they were not created when user was registered
                    if (oceanStats == null)
                    {
                        oceanStats = new Entities.UserInfoEntities.OceanStats(userId);
                        await _contx.OceanStats.AddAsync(oceanStats);
                        await _contx.SaveChangesAsync();
                    }

                    //Add ocean points, if they were not created when user was registered
                    if (oceanPoints == null)
                    {
                        oceanPoints = new OceanPoints(userId);
                        await _contx.OceanPoints.AddAsync(oceanPoints);
                        await _contx.SaveChangesAsync();
                    }
                }

                _contx.UsersSettings.Update(settings);
                await _contx.SaveChangesAsync();

                return settings.UsesOcean;
            }
            catch { throw new NullReferenceException($"User {userId} does not exist !"); }
        }

        public async Task<bool> RegisterTestPassingAsync(TestPayload model, float testResult)
        {
            try
            {
                var userTest = await _contx.UserTests.Where(t => t.UserId == model.UserId && t.TestId == model.TestId)
                    .FirstOrDefaultAsync();

                if (userTest.PassedOn == null)
                {
                    //Give user 1 OP for passing the test for the first time
                    var userBalance = await GetUserWalletBalance(model.UserId);
                    userBalance.OceanPoints++;
                    await _contx.SaveChangesAsync();
                }
                else
                {
                    // Remove old tags related to this test
                    var tagsToRemove = await (
                        from tq in _contx.TestsQuestions
                        where tq.TestId == model.TestId
                        from ta in _contx.TestsAnswers
                        where ta.TestQuestionId == tq.Id
                        from ut in _contx.UserTags
                        where ut.TagType == TagType.Tests && ut.UserId == model.UserId && ta.Tags.Any(t => t == ut.TagId)
                        select ut).ToListAsync();

                    var resultTags = await (
                        from tr in _contx.TestsResults
                        where tr.TestId == model.TestId
                        from ut in _contx.UserTags
                        where ut.TagType == TagType.Tests && ut.UserId == model.UserId && tr.Tags.Any(t => t == ut.TagId)
                        select ut).ToListAsync();

                    _contx.UserTags.RemoveRange(tagsToRemove);
                    _contx.UserTags.RemoveRange(resultTags);
                }

                userTest.PassedOn = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                userTest.Result = testResult;

                _contx.UserTests.Update(userTest);
                await _contx.SaveChangesAsync();

                return true;
            }
            catch { return false; }

        }

        public async Task<bool> UpdateTags(UpdateTags model)
        {
            _contx.UserTags.RemoveRange(_contx.UserTags.Where(t => t.UserId == model.UserId));

            var tags = await AddTagsAsync(model.RawTags, TagType.Tags);
            var newTags = tags.Select(t => new UserTag(t, model.UserId, TagType.Tags));

            await _contx.UserTags.AddRangeAsync(newTags);
                    
            await _contx.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserTag>> GetTags(long userId)
        {
            try
            {
                return await _contx.UserTags
                    .Where(t => t.UserId == userId && t.TagType == TagType.Tags)
                    .Include(t => t.Tag)
                    .ToListAsync();
            }
            catch (NullReferenceException)
            { return null; }
        }

        public async Task<SearchResponse> GetUserByTagsAsync(GetUserByTags model)
        {
            var currentUser = await _contx.Users.Where(u => u.Id == model.UserId)
                .Include(u => u.Data)
                .Include(u => u.Settings)
                .Include(u => u.Location)
                .FirstOrDefaultAsync();

            var hasActiveDetector = await CheckEffectIsActiveAsync(currentUser.Id, Currency.TheDetector);
            
            //Actualize premium property
            await CheckUserHasPremiumAsync(model.UserId);

            //User has already reached his limit;
            if (currentUser.TagSearchesCount >= currentUser.MaxTagSearchCount)
                return new SearchResponse();

            var query = _contx.Users
                    .Where(u => u.Id != currentUser.Id)
                    .Where(u => u.Data.AgePrefs.Contains(currentUser.Data.UserAge))
                    .Where(u => u.Data.UserLanguages.Any(l => currentUser.Data.LanguagePreferences.Contains(l)))
                    .Where(u => currentUser.Data.AgePrefs.Contains(u.Data.UserAge))
                    .Where(u => currentUser.Data.UserLanguages.Any(l => u.Data.LanguagePreferences.Contains(l)))
                    //Check if users had encountered one another
                    .Where(u => u.Encounters.Where(e => e.Section == Section.Requester || e.Section == Section.Familiator)
                        .All(e => e.EncounteredUserId != currentUser.Id)) //May casuse errors
                    //Check if users are in each others black lists
                    .Where(u => u.BlackList.All(l => l.BannedUserId != currentUser.Id))
                    //.Where(u => currentUser.BlackList.Where(l => l.BannedUserId == u.Id).FirstOrDefault() == null)
                    //Check if request already exists
                    .Where(u => u.Requests.All(n => n.SenderId != currentUser.Id && n.UserId != currentUser.Id)) //May casuse errors
                    //.Where(u => u.Tags.Intersect(u.Tags).Count() >= 1)
                    .Include(u => u.Data)
                    .Include(u => u.Settings)
                    .Include(u => u.Location)
                    .Include(u => u.BlackList)
                    .Include(u => u.Tags)
                    .AsNoTracking();

            //Identity check
            if (currentUser.Settings.ShouldFilterUsersWithoutRealPhoto && currentUser.HasPremium)
                query = query.Where(u => u.IdentityType != IdentityConfirmationType.None);

            //Add user search
            currentUser.TagSearchesCount++;

            var extractedTags = await AddTagsAsync(model.Tags, TagType.Tags);
            
            // TODO: Come up with some premium benifits
            if(currentUser.HasPremium)
            {   
                //TODO: Perpabs order by descending afterwards
                foreach (var tag in extractedTags)
                {   
                    query = query.Where(u => u.Tags.Any(t => t.TagId == tag));
                }
            }
            else
            {
                foreach (var tag in extractedTags)
                {
                    query = query.Where(u => u.Tags.Any(t => t.TagId == tag));
                }
            }

            //Shuffle users randomly
            query = query.OrderBy(u => EF.Functions.Random());

            var user = await query.Select(u => new GetUserData(u, ""))
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            GetUserData outputUser;

            if (currentUser.Settings.UsesOcean)
                outputUser = await GetOceanMatchResult(model.UserId, currentUser, user, false);
            else
                outputUser = await AssembleProfileAsync(currentUser, user);

            //Show tags if user has detector activated
            if (hasActiveDetector)
            {
                var tags = await _contx.UserTags.Where(t => t.UserId == currentUser.Id && t.TagType == TagType.Tags)
                    .Select(t => t.Tag.Text)
                    .ToListAsync();

                outputUser.AddDescriptionBonusDownwards(String.Join(" ", tags));
            }

            return new SearchResponse(outputUser);
        }

        public async Task<bool?> CheckUserUsesPersonality(long userId)
        {
            return await _contx.UsersSettings
                .Where(p => p.Id == userId)
                .Select(p => p.UsesOcean)
                .FirstOrDefaultAsync();
        }

        public async Task<List<BlackList>> GetBlackList(long userId)
        {
            return await _contx.UserBlacklists.Where(l => l.UserId == userId)
                .Include(l => l.BannedUser)
                .ToListAsync();
        }

        public async Task<bool> CheckEncounteredUserIsInBlackList(long userId, long encounteredUser)
        {
            return await _contx.UserBlacklists
                .Where(l => l.UserId == userId && l.BannedUserId == encounteredUser)
            .FirstOrDefaultAsync() != null;
        }

        public async Task<string> RetreiveCommonLanguagesAsync(long user1Id, long user2Id, int localisationId)
        {
            var user1Langs = await _contx.UserData.Where(u => u.Id == user1Id)
                .Select(u => u.UserLanguages)
                .FirstOrDefaultAsync();

            var user2Langs = await _contx.UserData.Where(u => u.Id == user2Id)
                .Select(u => u.UserLanguages)
                .FirstOrDefaultAsync();

            var commonIds = user1Langs.Intersect(user2Langs);
            var commons = await _contx.Languages.Where(l => commonIds
                .Any(i => i == l.Id) && l.Lang == 0)
                .Select(l => l.LanguageName)
                .ToListAsync();

            return String.Join(", ", commons);
        }

        public async Task SetAutoReplyTextAsync(long userId, string text)
        {
            var data = await _contx.UserData.Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            data.AutoReplyText = text;
            await _contx.SaveChangesAsync();
        }

        public async Task SetAutoReplyVoiceAsync(long userId, string voice)
        {
            var data = await _contx.UserData.Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            data.AutoReplyVoice = voice;
            await _contx.SaveChangesAsync();
        }

        public async Task<ActiveAutoReply> GetActiveAutoReplyAsync(long userId)
        {
            var reply = new ActiveAutoReply();
            var replyData = await _contx.UserData.Where(d => d.Id == userId)
                .Select(d => new { d.AutoReplyText, d.AutoReplyVoice })
                .FirstOrDefaultAsync();

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

                await CreateUserBalance(userId, 0, DateTime.UtcNow);
                return false;
            }
            catch { return false; }
        }

        public async Task<DateTime?> ActivateDurableEffectAsync(long userId, Currency effectId)
        {
            try
            {
                ActiveEffect effect;
                var userBalance = await GetUserWalletBalance(userId);

                switch (effectId)
                {
                    case Currency.TheValentine:
                        var userPoints = await _contx.OceanPoints.Where(p => p.UserId == userId)
                            .FirstOrDefaultAsync();

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
                    case Currency.TheDetector:
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

                var activeEffect = await _contx.ActiveEffects.Where(e => e.Effect == effectId && e.UserId == userId)
                    .FirstOrDefaultAsync();

                if (activeEffect == null)
                    await _contx.ActiveEffects.AddAsync(effect);
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
                            await RegisterUserRequestAsync(new AddRequest
                            {
                                SenderId = userId,
                                UserId = (long)user2Id,
                                IsMatch = false
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
                            await AddLimitationsAsync(userId, 15, 10, 5, 15);
                            _contx.Update(userBalance);
                            await _contx.SaveChangesAsync();
                            return true;
                        }
                        return false;
                    case 10:
                        if (userBalance.CardDecksPlatinum > 0)
                        {
                            userBalance.CardDecksPlatinum--;
                            await AddLimitationsAsync(userId, 35, 15, 10, 20);
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

        private bool AtLeastOneIsNotZero(OceanPoints points)
        {
            return points.Agreeableness > 0 ||
                points.Conscientiousness > 0 ||
                points.Extroversion > 0 || points.Openness > 0 ||
                points.Nature > 0 || points.Neuroticism > 0;
        }

        public async Task<List<GetActiveEffect>> GetUserActiveEffects(long userId)
        {
            try
            {
                var effectsToRemove = new List<ActiveEffect>();
                var effectsToReturn = new List<GetActiveEffect>();

                foreach (var effect in await _contx.ActiveEffects.Where(e => e.UserId == userId).ToListAsync())
                {
                    if (effect.ExpirationTime.Value <= DateTime.UtcNow)
                        effectsToRemove.Add(effect);
                    else
                        effectsToReturn.Add(new GetActiveEffect(effect));
                }

                if (effectsToRemove.Count > 0)
                    _contx.ActiveEffects.RemoveRange(effectsToRemove);

                await _contx.SaveChangesAsync();
                return effectsToReturn;
            }
            catch {
                return null;
            }
        }

        public async Task<bool> DeactivateEffectAsync(long userId, long activeEffectId)
        {
            try
            {
                var effect = await _contx.ActiveEffects.Where(e => e.UserId == userId && e.Id == activeEffectId)
                    .FirstOrDefaultAsync();

                if (effect == null)
                    return false;

                _contx.ActiveEffects.Remove(effect);
                await _contx.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }

        private async Task AddLimitationsAsync(long userId, int normalSearch, int rtSearch, int tagSearch, int adventureSearch)
        {
            var userInfo = await _contx.Users.Where(u => u.Id == userId)
                                .FirstOrDefaultAsync();

            userInfo.MaxProfileViewsCount += normalSearch;
            userInfo.MaxRTViewsCount += rtSearch;
            userInfo.MaxTagSearchCount += tagSearch;
            userInfo.MaxAdventureSearchCount += adventureSearch;

            await _contx.SaveChangesAsync();
        }

        public async Task<bool> CheckEffectIsActiveAsync(long userId, Currency effectId)
        {
            var effects = await GetUserActiveEffects(userId);

            if (effects == null)
                return false;

            return effects.Any(e => e.Effect == effectId);
        }

        public async Task<bool> SendTickRequestAsync(SendTickRequest request)
        {
            try
            {
                if (request == null)
                    throw new NullReferenceException("Request is null");

                var existingRequest = await _contx.TickRequests.Where(r => r.UserId == request.UserId)
                    .Include(r => r.User)
                    .FirstOrDefaultAsync();

                //Update existing request if one already exists
                if (existingRequest != null)
                {
                    existingRequest.Photo = request.Photo;
                    existingRequest.Video = request.Video;
                    existingRequest.Circle = request.Circle;
                    existingRequest.Gesture = request.Gesture;
                    existingRequest.Type = request.Type;
                    existingRequest.State = TickRequestStatus.Changed;
                    existingRequest.User.IdentityType = IdentityConfirmationType.Awaiting;

                    _contx.TickRequests.Update(existingRequest);
                    _contx.Users.Update(existingRequest.User);
                    await _contx.SaveChangesAsync();
                    return true;
                }

                var user = await _contx.Users.Where(u => u.Id == request.UserId)
                    .FirstOrDefaultAsync();

                user.IdentityType = IdentityConfirmationType.Awaiting;

                var model = new TickRequest
                {
                    UserId = request.UserId,
                    AdminId = null,
                    State = null,
                    Photo = request.Photo,
                    Video = request.Video,
                    Circle = request.Circle,
                    Gesture = request.Gesture,
                    Type = request.Type
                };

                await _contx.TickRequests.AddAsync(model);
                _contx.Users.Update(user);
                await _contx.SaveChangesAsync();

                return true;
            }
            catch { return false; }
        }

        public async Task<bool> SwitchUserFilteringByPhotoAsync(long userId)
        {
            var settings = await _contx.UsersSettings.Where(p => p.Id == userId)
                .FirstOrDefaultAsync();

            if (settings == null)
                throw new NullReferenceException($"User {userId} was not found");

            settings.ShouldFilterUsersWithoutRealPhoto = !settings.ShouldFilterUsersWithoutRealPhoto;
            await _contx.SaveChangesAsync();

            return settings.ShouldFilterUsersWithoutRealPhoto;
        }

        public async Task<bool> GetUserFilteringByPhotoStatusAsync(long userId)
        {
            var userPrefs = await _contx.UsersSettings.Where(p => p.Id == userId)
                .FirstOrDefaultAsync();

            if (userPrefs == null)
                throw new NullReferenceException($"User {userId} was not found");

            return userPrefs.ShouldFilterUsersWithoutRealPhoto;
        }

        public async Task<List<GetTestShortData>> GetTestDataByPropertyAsync(long userId, OceanStats param)
        {
            var localisation = await _contx.UserData.Where(u => u.Id == userId).Select(u => u.Language)
                .FirstOrDefaultAsync();

            //Get tests user already has
            var userTests = await _contx.UserTests.Where(t => t.UserId == userId && t.TestType == param)
                .Select(t => t.TestId)
                .ToListAsync();

            // TODO: Add localization when tests are localized
            return await _contx.Tests
                .Where(t => t.TestType == param && !userTests.Contains(t.Id)) //&& t.Language == localisation
                .Select(t => new GetTestShortData { Id = t.Id, Name = t.Name })
                .ToListAsync();
        }

        public async Task<GetFullTestData> GetTestFullDataByIdAsync(long testId, AppLanguage localisation)
        {
            // TODO: Add localization when tests are localized
            return await _contx.Tests.Where(t => t.Id == testId) // && t.Language == localisation
                .Select(t => new GetFullTestData {Id = t.Id, Name = t.Name, Description = t.Description, Price = t.Price, TestType = t.TestType})
                .FirstOrDefaultAsync();
        }

        public async Task<GetUserTest> GetUserTestAsync(long userId, long testId)
        {
            var test = await _contx.UserTests.Where(t => t.UserId == userId && t.TestId == testId)
                .Include(t => t.Test)
                .SingleOrDefaultAsync();

            if (test == null)
                throw new NullReferenceException($"User {userId} does not have Test {testId}");

            return new GetUserTest(test);
        }

        public async Task<int> GetPossibleTestPassRangeAsync(long userId, long testId)
        {
            var test = await _contx.UserTests.Where(t => t.UserId == userId && t.TestId == testId)
                .Include(t => t.Test)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (test == null)
                throw new NullReferenceException($"User #{userId} does not have test #{testId} available");

            //If date of passing is equal to null => test had not been passed so far
            if (test.PassedOn == null)
                return 0;

            //Every test has its own passing range. If date of passing is out of it => Test can be passed again
            if ((DateTime.UtcNow - test.PassedOn).Value.Days > test.Test.CanBePassedInDays)
                return 0;

            //Get number of days in which this test can be passed again
            var result = (test.Test.CanBePassedInDays - (DateTime.UtcNow - test.PassedOn).Value.Days);
            return result;
        }

        public async Task PurchaseTestAsync(long userId, long testId, float cost, Currency currency, AppLanguage localisation)
        {
            var balance = await GetUserWalletBalance(userId);

            // TODO: Add localization when tests are localized
            var test = await _contx.Tests.Where(t => t.Id == testId) // && t.Language == localisation
                .FirstOrDefaultAsync();

            await RegisterUserPurchase(userId, -cost, $"Purchase of test \"{test.Name}\" for {currency} amount of {cost}", currency);

            if (currency == Currency.Points)
                balance.Points -= cost;

            await _contx.UserTests.AddAsync(new UserTest
            {
                UserId = userId,
                TestId = testId,
                TestType = test.TestType,
                Result = 0,
                TestLanguage = localisation
            });
                
            await _contx.SaveChangesAsync();
        }

        public async Task<List<GetTestShortData>> GetUserTestDataByPropertyAsync(long userId, OceanStats param)
        {
            //Get users tests
            return await _contx.UserTests.Where(t => t.UserId == userId && t.TestType == param)
                .Include(t => t.Test)
                .Select(t => new GetTestShortData { Id = t.TestId, Name = t.Test.Name })
                .ToListAsync();
        }

        public async Task<string> CheckTickRequestStatusÀsync(long userId)
        {
            var request = await _contx.TickRequests.Where(r => r.UserId == userId)
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
            var user = await _contx.UsersSettings.FindAsync(userId);

            if (user == null)
                throw new NullReferenceException($"User #{userId} does not exist");

            user.IsFree = freeStatus;
            await _contx.SaveChangesAsync();

            return freeStatus;
        }

        public async Task<bool> CheckUserHaveChosenFreeParamAsync(long userId)
        {
            var user = await _contx.Users.FindAsync(userId);

            if (user == null)
                return false;

            return true;
        }

        public async Task<bool> CheckShouldTurnOffPersonalityAsync(long userId)
        {
            var user = await _contx.Users.Where(u => u.Id == userId)
                .Include(u => u.Settings)
                .SingleOrDefaultAsync();

            if (user == null)
                throw new NullReferenceException($"User #{userId} does not exist");

            //Return false if peofileViewCountIsAlreadyMaxed
            if (user.ProfileViewsCount >= user.MaxProfileViewsCount)
                return false;

            //Return false if personality is not used
            if (!user.Settings.UsesOcean)
                return false;

            return true;
        }

        public async Task<OceanCaps> GetUserPersonalityCapsAsync(long userId)
        {
            var stats = await _contx.OceanStats.Where(s => s.UserId == userId)
                .FirstOrDefaultAsync();

            if (stats == null)
            {
                stats = new Entities.UserInfoEntities.OceanStats(userId);
                await _contx.AddAsync(stats);
            }

            return new OceanCaps
            {
                CanO = stats.Openness > 0,
                CanC = stats.Conscientiousness > 0,
                CanE = stats.Extroversion > 0,
                CanA = stats.Agreeableness > 0,
                CanN = stats.Neuroticism > 0,
                CanP = stats.Nature > 0
            };

        }

        public async Task<bool> SwitchUserRTLanguageConsiderationAsync(long userId)
        {
            var settings = await _contx.UsersSettings.Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (settings == null)
                throw new NullReferenceException($"User #{userId} does not exist");

            settings.ShouldConsiderLanguages = !settings.ShouldConsiderLanguages;

            await _contx.SaveChangesAsync();

            return settings.ShouldConsiderLanguages;
        }

        public async Task<bool> GetUserRTLanguageConsiderationAsync(long userId)
        {
            var user = await _contx.UsersSettings.FindAsync(userId);

            if (user == null)
                throw new NullReferenceException($"User #{userId} does not exist");

            return user.ShouldConsiderLanguages;
        }

        public async Task SetUserCurrencyAsync(long userId, Currency currency)
        {
            var balance = await GetUserWalletBalance(userId);

            if (balance == null)
                throw new NullReferenceException($"User #{userId} does not exist");

            balance.Currency = currency;
            await _contx.SaveChangesAsync();
        }

        public async Task<bool> PurchaseEffectAsync(long userId, int effectId, float points, Currency currency, short count=1)
        {
            var balance = await GetUserWalletBalance(userId);

            if (balance != null)
            {
                if (currency == Currency.Points)
                    balance.Points -= points;

                switch (effectId)
                {
                    case 5:
                        balance.SecondChances += count;
                        if (currency == Currency.Points)
                        {
                            await RegisterUserPurchaseInPoints(userId, -points, $"User purchase of {count} Second Chance effect for point amount {points}");
                            await RegisterUserPurchase(userId, count, "Effects are received", (Currency)effectId);
                        }
                        else
                        {
                            await RegisterPurchaseInRealMoney(userId, -points, $"User purchase of {count} Second Chance effect for {currency} amount {points}", (Currency)balance.Currency);
                            await RegisterUserPurchase(userId, count, "Effects are received", (Currency)effectId);
                        }
                        break;
                    case 6:
                        balance.Valentines += count;
                        if (currency == Currency.Points)
                        {
                            await RegisterUserPurchaseInPoints(userId, -points, $"User purchase of {count} Valentine effect for point amount {points}");
                            await RegisterUserPurchase(userId, count, "Effects are received", (Currency)effectId);
                        }
                        else
                        {
                            await RegisterPurchaseInRealMoney(userId, -points, $"User purchase of {count} Valentine effect for {currency} amount {points}", (Currency)balance.Currency);
                            await RegisterUserPurchase(userId, count, "Effects are received", (Currency)effectId);
                        }
                        break;
                    case 7:
                        balance.Detectors += count;
                        if (currency == Currency.Points)
                        {
                            await RegisterUserPurchaseInPoints(userId, -points, $"User purchase of {count} Detector effect for point amount {points}");
                            await RegisterUserPurchase(userId, count, "Effects are received", (Currency)effectId);
                        }
                        else
                        {
                            await RegisterPurchaseInRealMoney(userId, -points, $"User purchase of {count} Detector effect for {currency} amount {points}", (Currency)balance.Currency);
                            await RegisterUserPurchase(userId, count, "Effects are received", (Currency)effectId);
                        }
                        break;
                    case 8:
                        balance.Nullifiers += count;
                        if (currency == Currency.Points)
                        {
                            await RegisterUserPurchaseInPoints(userId, -points, $"User purchase of {count} Nullifier effect for point amount {points}");
                            await RegisterUserPurchase(userId, count, "Effects are received", (Currency)effectId);
                        }
                        else
                        {
                            await RegisterPurchaseInRealMoney(userId, -points, $"User purchase of {count} Nullifier effect for {currency} amount {points}", (Currency)balance.Currency);
                            await RegisterUserPurchase(userId, count, "Effects are received", (Currency)effectId);
                        }
                        break;
                    case 9:
                        balance.CardDecksMini += count;
                        if (currency == Currency.Points)
                        {
                            await RegisterUserPurchaseInPoints(userId, -points, $"User purchase of {count} Card Deck Mini effect for point amount {points}");
                            await RegisterUserPurchase(userId, count, "Effects are received", (Currency)effectId);
                        }
                        else
                        {
                            await RegisterPurchaseInRealMoney(userId, -points, $"User purchase of {count} Second Card Deck Mini for {currency} amount {points}", (Currency)balance.Currency);
                            await RegisterUserPurchase(userId, count, "Effects are received", (Currency)effectId);
                        }
                        break;
                    case 10:
                        balance.CardDecksPlatinum += count;
                        if (currency == Currency.Points)
                        {
                            await RegisterUserPurchaseInPoints(userId, -points, $"User purchase of {count} Card Deck Platinum effect for point amount {points}");
                            await RegisterUserPurchase(userId, count, "Effects are received", (Currency)effectId);
                        }
                        else
                        {
                            await RegisterPurchaseInRealMoney(userId, -points, $"User purchase of {count} Card Deck Platinum effect for {currency} amount {points}", (Currency)balance.Currency);
                            await RegisterUserPurchase(userId, count, "Effects are received", (Currency)effectId);
                        }
                        break;
                    default:
                        break;

                }
                return true;
            }

            return false;
        }

        public async Task<GetUserData> GetRequestSenderAsync(long senderId)
        {
            var sender = await _contx.Users.Where(u => u.Id == senderId)
                .Include(s => s.Settings)
                .Include(s => s.Data)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var bonus = "";

            if (sender.IdentityType == IdentityConfirmationType.Partial)
                bonus += $"☑️☑️☑️\n\n";
            else if (sender.IdentityType == IdentityConfirmationType.Full)
                bonus += $"✅✅✅\n\n";

            if (sender.HasPremium && sender.Nickname != "")
                bonus += $"<b>{sender.Nickname}</b>\n";

            return new GetUserData(sender, bonus);
        }

        public async Task<bool> PurchasePersonalityPointsAsync(long userId, float amount, Currency currency, short count = 1)
        {
            try
            {
                var balance = await GetUserWalletBalance(userId);

                if (currency == Currency.Points)
                {
                    balance.Points -= amount;
                    balance.OceanPoints += count;
                    await RegisterUserPurchaseInPoints(userId, -amount, $"User purchase of {count} Ocean+ Points effect for point amount {amount}");
                    await RegisterUserPurchase(userId, count, "OP received", Currency.OceanPoints);
                }
                else
                {
                    balance.OceanPoints += count;
                    await RegisterPurchaseInRealMoney(userId, -amount, $"User purchase of {count} Ocean+ Points for {currency} amount {amount}", (Currency)balance.Currency);
                    await RegisterUserPurchase(userId, count, "OP received", Currency.OceanPoints);
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
            var promo = await _contx.PromoCodes.Where(p => p.Promo == promoText && p.UsedOnlyInRegistration == isActivatedBeforeRegistration)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            // There is no way the promo can be applied before Registration, due to the absence of user data.
            // Thus we are only checking its presence
            if (isActivatedBeforeRegistration)
                return promo != null;

            //Enter promo right away if it had not been input before registration
            return await EnterPromo(userId, promo);
        }

        private async Task<bool> EnterPromo(long userId, PromoCode promo)
        {
            if (promo == null)
                return false;

            var userBalance = await _contx.Balances.FindAsync(userId);

            userBalance.Points += promo.Points;
            userBalance.OceanPoints += promo.PersonalityPoints;
            userBalance.SecondChances += promo.SecondChance;
            userBalance.Valentines += promo.TheValentine;
            userBalance.Detectors += promo.TheDetector;
            userBalance.Nullifiers += promo.Nullifier;
            userBalance.CardDecksMini += promo.CardDeckMini;
            userBalance.CardDecksPlatinum += promo.CardDeckPlatinum;

            _contx.Balances.Update(userBalance);
            await _contx.SaveChangesAsync();
            return true;
        }

        public async Task<bool> GetUserIncreasedFamiliarityAsync(long userId)
        {
            var user = await _contx.UsersSettings.FindAsync(userId);

            if (user == null)
                throw new NullReferenceException($"User {userId} does not exist !");

            return user.IncreasedFamiliarity;
        }

        public async Task<bool> SwitchIncreasedFamiliarityAsync(long userId)
        {
            var settings = await _contx.UsersSettings.FindAsync(userId);

            if (settings == null)
                throw new NullReferenceException($"User {userId} does not exist !");

            settings.IncreasedFamiliarity = !settings.IncreasedFamiliarity;
            await _contx.SaveChangesAsync();

            return settings.IncreasedFamiliarity;
        }

        //public async Task<bool> AddUserCommercialVector(long userId, string tagString)
        //{
        //    var tags = tagString.Replace("#", "").Trim().Replace(" ", "").Split(",");

        //    foreach (var tag in tags)
        //    {
        //        await _contx.UserTags.AddAsync(new UserTag
        //        {
        //            UserId = userId, 
        //            Tag = tag,
        //            TagType = TagType.Interests
        //        });

        //    }

        //    await _contx.SaveChangesAsync();
        //    return true;
        //}

        public async Task<bool> SwitchUserFreeSearchParamAsync(long userId)
        {
            var settings = await _contx.UsersSettings.FindAsync(userId);

            if (settings == null)
                throw new NullReferenceException($"User #{userId} does not exist");

            //null is the same thing as false in that case
            if (settings.IsFree == null)
            {
                settings.IsFree = true;
                await _contx.SaveChangesAsync();

                return true;
            }

            settings.IsFree = !settings.IsFree;
            await _contx.SaveChangesAsync();

            return (bool)settings.IsFree;
        }

        public async Task<string> RegisterAdventureAsync(ManageAdventure model)
        {
            var userLang = await _contx.UserData.Where(u => u.Id == model.UserId)
                .Select(u => u.Language)
                .FirstOrDefaultAsync();

            var adventure = new Adventure
            {
                UserId = model.UserId,
                Name = model.Name,
                Address = model.Address,
                Application = model.Application,
                AttendeesDescription = model.AttendeesDescription,
                CountryId = model.CountryId,
                CityId = model.CityId,
                CountryLang = userLang,
                CityCountryLang = userLang,
                Date = model.Date,
                Time = model.Time,
                Description = model.Description,
                Duration = model.Duration,
                Experience = model.Experience,
                UnwantedAttendeesDescription = model.UnwantedAttendeesDescription,
                Gratitude = model.Gratitude,
                MediaType = model.MediaType,
                Media = model.Media,
                IsAutoReplyText = model.IsAutoReplyText,
                AutoReply = model.AutoReply,
                IsOffline = model.IsOffline,
                IsAwaiting = model.IsAwaiting,
                UniqueLink = Guid.NewGuid().ToString("N").Substring(0, 7).ToUpper(),
                Status = AdventureStatus.New
            };

            await _contx.Adventures.AddAsync(adventure);
            await _contx.SaveChangesAsync();

            return adventure.UniqueLink;
        }

        public async Task ChangeAdventureAsync(ManageAdventure model)
        {
            var adventure = await _contx.Adventures.Where(a => a.Id == model.Id)
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
            adventure.MediaType = model.MediaType;
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
            var adventure = await _contx.Adventures.Where(a => a.UniqueLink == request.InvitationCode)
                .FirstOrDefaultAsync();

            if (adventure == null)
                return ParticipationRequestStatus.AdventureNotFound;

            return await SendAdventureRequestAsync(adventure.Id, request.UserId, AdventureAttendeeStatus.NewByCode, NotificationType.AdventureParticipationByCode);
        }

        public async Task<ParticipationRequestStatus> SendAdventureRequestAsync(long adventureId, long userId, AdventureAttendeeStatus status = AdventureAttendeeStatus.New, NotificationType notificationType = NotificationType.AdventureParticipation)
        {
            var adventure = await _contx.Adventures.Where(a => a.Id == adventureId)
                .FirstOrDefaultAsync();

            if (adventure == null)
                return ParticipationRequestStatus.AdventureNotFound;

            if (adventure.UserId == userId)
                return ParticipationRequestStatus.AdventuresOwner;

            var existingAttendee = await _contx.AdventureAttendees
                .Where(a => a.AdventureId == adventure.Id && a.UserId == userId)
                .FirstOrDefaultAsync();

            //if (existingAttendee != null)
            //    return ParticipationRequestStatus.AlreadyParticipating;

            var userName = await _contx.UserData.Where(u => u.Id == userId)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();

            var description = "Someone had requested participation in your adventure";

            if (notificationType == NotificationType.AdventureParticipationByCode)
                description = "Someone had requested participation in your adventure via a unique code";

            await AddUserNotificationAsync(new UserNotification
            {
                Section = Section.Adventurer,
                Type = notificationType,
                Description = description,
                //SenderId = userId,
                UserId = adventure.UserId
            });

            var newAttendee = new AdventureAttendee
            {
                Status = status,
                AdventureId = adventure.Id,
                UserId = userId,
                Username = userName
            };

            await _contx.AdventureAttendees.AddAsync(newAttendee);
            await _contx.SaveChangesAsync();

            return ParticipationRequestStatus.Ok;
        }

        public async Task<bool> DeleteAdventureAsync(long adventureId, long userId)
        {
            var attendees = await _contx.AdventureAttendees.Where(a => a.AdventureId == adventureId)
                .Include(a => a.Adventure)
                .ToListAsync();

            var adventure = await _contx.Adventures.Where(a => a.Id == adventureId)
                .FirstOrDefaultAsync();

            foreach (var attendee in attendees)
            {
                await AddUserNotificationAsync(new UserNotification
                {
                    Section = Section.Adventurer,
                    UserId = attendee.UserId,
                    Type = NotificationType.Other,
                    Description = $"We are very sorry. Adventure {attendee.Adventure.Name} had been deleted. Please, contact creator if the reason is unknown to you"
                });
            }

            adventure.Status = AdventureStatus.Deleted;
            adventure.DeleteDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

            _contx.AdventureAttendees.RemoveRange(attendees);
            await _contx.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ProcessSubscriptionRequestAsync(long adventureId, long userId, AdventureAttendeeStatus status)
        {
            var attendee = await _contx.AdventureAttendees.Where(a => a.UserId == userId && a.AdventureId == adventureId)
                .SingleOrDefaultAsync();


            if (attendee == null)
                throw new NullReferenceException($"No attendee with id #{userId} had been subscribed to adventure {adventureId}");

            var adventure = await _contx.Adventures.Where(a => a.Id == adventureId)
                .Include(a => a.Creator).ThenInclude(u => u.Data)
                .FirstOrDefaultAsync();

            attendee.Status = status;

            if (status == AdventureAttendeeStatus.Accepted)
            {
                var contact = string.IsNullOrEmpty(adventure.GroupLink) ? 
                    $"You may contact its creator @{adventure.Creator.Data.UserName} and discuss details" : 
                    $"You may join creator's group via this link\n{adventure.GroupLink}" ;

                await AddUserNotificationAsync(new UserNotification
                {
                    UserId = userId,
                    Section = Section.Adventurer,
                    Type = NotificationType.Other,
                    Description = $"Your request to join adventure {adventure.Name} had been accepted.\n{contact}"
                });
            }

            await _contx.SaveChangesAsync();
            return true;
        }

        public async Task<List<AttendeeInfo>> GetAdventureAttendeesAsync(long adventureId)
        {
            return await _contx.AdventureAttendees.Where(a => a.AdventureId == adventureId && (a.Status == AdventureAttendeeStatus.New || a.Status == AdventureAttendeeStatus.NewByCode || a.Status == AdventureAttendeeStatus.Accepted))
            .Select(a => new AttendeeInfo
            {
                UserId = a.UserId,
                Username = a.Username,
                Status = a.Status
            }).ToListAsync();
        }

        public async Task<List<Adventure>> GetUsersSubscribedAdventuresAsync(long userId)
        {
            var adventureIds = await _contx.AdventureAttendees.Where(a => a.UserId == userId)
                .Select(a => a.AdventureId)
                .ToListAsync();

            return await _contx.Adventures.Where(a => adventureIds.Contains(a.Id))
                .ToListAsync();
        }

        public async Task<List<GetAdventure>> GetUserAdventuresAsync(long userId)
        {
            return await _contx.Adventures.Where(a => a.UserId == userId)
                .Where(a => a.Status != AdventureStatus.Deleted)
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
            var createdCount = await _contx.Adventures.Where(a => a.UserId == userId)
                .CountAsync();

            var subscribedCount = await _contx.AdventureAttendees.Where(a => a.UserId == userId)
                .CountAsync();

            return new GetAdventureCount
            {
                Created = createdCount,
                Subscribed = subscribedCount
            };
        }

        private async Task ReactivateTickRequest(long userId)
        {
            var tickRequest = await _contx.TickRequests.Where(r => r.UserId == userId)
                .FirstOrDefaultAsync();

            //Return if tick request does not exists, because not all users has tick request
            if (tickRequest == null)
                return;

            var user = await _contx.Users.Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

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
            var tags = await _contx.UserTags.Where(t => t.UserId == userId)
                .Include(t => t.Tag)
                .ToListAsync();

            return new GetUserTags
            {
                FullTags = tags.Select(t => new UserTags
                {
                    Tag = t.Tag.Text,
                    TagType = t.TagType
                }).ToList(),

                Tags = tags.Select(t => t.Tag.Text)
                .ToList()
            };
        }

        private async Task<List<UserTag>> GetUserTagsAsync(long userId, TagType tagType)
        {
            return await _contx.UserTags.Where(t => t.UserId == userId && t.TagType == tagType)
                .ToListAsync();
        }

        private async Task<Dictionary<long, List<UserTag>>> GetUsersTagsAsync(List<long> userIds, TagType tagType)
        {
            var list = await _contx.UserTags.Where(t => userIds.Contains(t.UserId) && t.TagType == tagType)
                .ToListAsync();

            return list.GroupBy(t => t.UserId).ToDictionary(t => t.FirstOrDefault().UserId, t => t.ToList());
        }

        public async Task<GetLimitations> GetUserSearchLimitations(long userId)
        {
            //Refresh data regarding user premium status
            var hasPremium = await CheckUserHasPremiumAsync(userId);

            var limitations = await _contx.Users.Where(u => u.Id == userId).Select(u => new GetLimitations
            {
                MaxTagViews = u.MaxTagSearchCount,
                MaxProfileViews = u.MaxProfileViewsCount,
                MaxRtViews = u.MaxRTViewsCount,
                MaxTagsPerSearch = hasPremium ? 10 : 5,
                ActualTagViews = u.TagSearchesCount,
                ActualProfileViews = u.ProfileViewsCount,
                ActualRtViews = u.RTViewsCount
            }).FirstOrDefaultAsync();

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

        public async Task<string> GetRandomHintAsync(AppLanguage localisation, HintType? type)
        {
            //25% chance to send a hint
            //if (new Random().Next(1, 4) != 1)
            //    return null;

            if (type == null)
            {

                return await _contx.Hints.Where(h => h.Localization == localisation && h.Type != HintType.Search)
                    .OrderBy(r => EF.Functions.Random()).Take(1)
                    .Select(h => h.Text)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }

            return await _contx.Hints.Where(h => h.Localization == localisation && h.Type == type)
                    .OrderBy(r => EF.Functions.Random())
                    .Select(h => h.Text)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
        }

        public async Task<BasicUserInfo> GetUserBasicInfo(long userId)
        {
            var limitations = await GetUserSearchLimitations(userId);

            return await _contx.Users.Where(u => u.Id == userId)
                .Select(u => new BasicUserInfo
                {
                    Id = u.Id,
                    Username = u.Data.UserName,
                    UserRealName = u.Data.UserRealName,
                    HasPremium = u.HasPremium,
                    IsBanned = u.IsBanned,
                    IsBusy = u.IsBusy,
                    IsFree = u.Settings.IsFree,
                    Limitations = limitations
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task SwitchHintsVisibilityAsync(long userId)
        {
            var settings = await  _contx.UsersSettings.Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (settings == null)
                return;

            settings.ShouldSendHints = !settings.ShouldSendHints;
            await _contx.SaveChangesAsync();
        }

        public async Task SwitchSearchCommentsVisibilityAsync(long userId)
        {
            var settings = await _contx.UsersSettings.Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (settings == null)
                return;

            settings.ShouldComment = !settings.ShouldComment;
            await _contx.SaveChangesAsync();
        }

        public async Task<GetUserMedia> GetUserMediaAsync(long userId)
        {
            return await _contx.UserData.Where(u => u.Id == userId).Select(u => new GetUserMedia
            {
                Media = u.UserMedia,
                MediaType = u.MediaType
            }).FirstOrDefaultAsync();
        }

        public async Task<UserPartialData> GetUserPartialData(long userId)
        {
            return await _contx.Users.Where(u => u.Id == userId).Select(u => new UserPartialData
            {
                Id = u.Id,
                AppLanguage = u.Data.Language,
                Media = u.Data.UserMedia,
                MediaType = u.Data.MediaType
            }).FirstOrDefaultAsync();
        }

        public async Task<ManageAdventure> GetAdventureAsync(long id)
        {
            return await _contx.Adventures.Where(a => a.Id == id)
                .Select(a => new ManageAdventure(a))
                .FirstOrDefaultAsync();
        }

        public async Task<bool> SaveAdventureTemplateAsync(ManageTemplate model)
        {
            var existingTemplate = await _contx.AdventureTemplates.Where(t => t.Id == model.Id)
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
                existingTemplate.MediaType = model.MediaType;
                existingTemplate.AutoReply = model.AutoReply;
                existingTemplate.IsAutoReplyText = model.IsAutoReplyText;

                await _contx.SaveChangesAsync();
                return true;
            }

            //Create template
            var template = new AdventureTemplate
            {
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
                MediaType = model.MediaType,
                AutoReply = model.AutoReply,
                IsAutoReplyText = model.IsAutoReplyText
            };

            await _contx.AddAsync(template);
            await _contx.SaveChangesAsync();

            return true;
        }

        public async Task<List<GetTemplateShort>> GetAdventureTemplatesAsync(long userId)
        {
            return await _contx.AdventureTemplates.Where(t => t.UserId == userId).Select(t => new GetTemplateShort
            {
                Id = t.Id,
                Name = t.Name
            }).ToListAsync();
        }

        public async Task<ManageTemplate> GetAdventureTemplateAsync(long id)
        {
            return await _contx.AdventureTemplates.Where(t => t.Id == id).Select(t => new ManageTemplate
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
                MediaType = t.MediaType,
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

        public async Task<DeleteResult> DeleteAdventureTemplateAsync(long templateId)
        {
            var template = await _contx.AdventureTemplates.Where(t => t.Id == templateId)
                .FirstOrDefaultAsync();

            if (template == null)
                return DeleteResult.DoesNotExist;

            _contx.AdventureTemplates.Remove(template);
            await _contx.SaveChangesAsync();

            return DeleteResult.Success;
        }

        public async Task<DeleteResult> DeleteAdventureAttendeeAsync(long adventureId, long attendeeId)
        {
            var attendee = await _contx.AdventureAttendees.Where(a => a.AdventureId == adventureId && a.UserId == attendeeId)
                .FirstOrDefaultAsync();

            if (attendee == null)
                return DeleteResult.DoesNotExist;

            _contx.AdventureAttendees.Remove(attendee);
            await _contx.SaveChangesAsync();

            await AddUserNotificationAsync(new UserNotification
            {
                UserId = attendee.UserId,
                Section = Section.Adventurer,
                Type = NotificationType.Other,
                Description = "You have been removed from one of the adventures" // TODO: More precise ?
            });

            return DeleteResult.Success;
        }

        public async Task<SetGroupIdResult> SetAdventureGroupIdAsync(SetGroupIdRequest request)
        {
            var hasName = !string.IsNullOrEmpty(request.AdventureName);
            var adventure = await _contx.Adventures.Where(a => a.UserId == request.UserId && a.IsAwaiting && ((hasName && a.Name == request.AdventureName) || !hasName))
                .FirstOrDefaultAsync();

            if (adventure == null)
                return SetGroupIdResult.AdventureDoesNotExist;

            adventure.GroupLink = request.GroupLink;
            adventure.GroupId = request.GroupId;

            await _contx.SaveChangesAsync();
            return SetGroupIdResult.Success;
        }

        public List<GetLocalizedEnum> GetGenders()
        {
            var genders = new List<GetLocalizedEnum>();

            foreach (var gender in Enum.GetValues(typeof(Gender)))
            {
                genders.Add(new GetLocalizedEnum
                {
                    Id = (byte)gender,
                    Name = EnumLocalizer.GetLocalizedValue((Gender)gender)
                });
            }

            return genders;
        }

        public async Task<DeleteResult> DeleteUserAsync(DeleteUserRequest request)
        {
            var user = await _contx.Users.Where(u => u.Id == request.UserId)
                .FirstOrDefaultAsync();

            if (user == null)
                return DeleteResult.DoesNotExist;

            user.IsDeleted = true;
            user.DeleteDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

            await AddFeedbackAsync(new AddFeedback
            {
                Reason = FeedbackReason.Suggestion,
                UserId = request.UserId,
                Text = request.Message
            });

            return DeleteResult.Success;
        }

        public async Task<RestoreResult> RestoreUserAsync(long userId)
        {
            var user = await _contx.Users.Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user == null)
                return RestoreResult.DoesNotExist;

            // No need to restore a user, who is not deleted :)
            if (!user.IsDeleted)
                return RestoreResult.Error;

            user.IsDeleted = false;
            user.DeleteDate = null;

            await _contx.SaveChangesAsync();

            return RestoreResult.Success;
        }

        public async Task<AdventureSearchResponse> GetAdventuresAsync(long userId)
        {
            var user = await _contx.Users.Where(u => u.Id == userId)
                .Include(u => u.Data)
                .Include(u => u.Location)
                .FirstOrDefaultAsync();

            if (user.AdventureSearchCount >= user.MaxAdventureSearchCount)
                return new AdventureSearchResponse();

            var adventuresCount = user.MaxAdventureSearchCount - user.AdventureSearchCount;

            var query = _contx.Adventures.Where(q => q.Status != AdventureStatus.Deleted && q.UserId != userId)
                .AsNoTracking();

            query = query.Where(q => q.Attendees.All(at => at.UserId != user.Id));

            //TODO: Preferences or actual location ?
            if (user.Location != null)
                query = query.Where(q => user.Data.LocationPreferences.Contains((int)q.CountryId));

            if (user.Data.CommunicationPrefs == CommunicationPreference.Online)
                query = query.Where(q => !q.IsOffline);

            else if (user.Data.CommunicationPrefs == CommunicationPreference.Offline)
                query = query.Where(q => q.IsOffline);


            query = query.Where(q => q.Creator.Data.LanguagePreferences.Any(l => user.Data.LanguagePreferences.Contains(l)));

            query = query.Include(q => q.Country).Include(q => q.City);

            query = query.Take(adventuresCount);

            var adventures = await query.Select(q => new GetAdventureSearch
            {
                Id = q.Id,
                Description = GetAdventureSearch.GenerateDescription(q),
                Media = q.Media,
                MediaType = q.MediaType,
                AutoReply = q.AutoReply,
                IsAutoReplyText = q.IsAutoReplyText
            }).ToListAsync();

            return new AdventureSearchResponse(adventures);
        }

        public async Task PurchasePointsAsync(long userId, float cost, Currency currency, int amount)
        {
            var balance = await GetUserWalletBalance(userId);
            balance.Points += amount;
            await RegisterPurchaseInRealMoney(userId, -cost, $"User purchase of {amount} Points for {currency} amount {cost}", currency);
            await RegisterUserPurchase(userId, amount, "Points received", Currency.Points);
        }

        public List<GetLocalizedEnum> GetPaymentCurrencies()
        {
            var currencies = new List<GetLocalizedEnum>();

            foreach (var currency in Enum.GetValues(typeof(PaymentCurrency)))
            {
                currencies.Add(new GetLocalizedEnum
                {
                    Id = (short)currency,
                    Name = EnumLocalizer.GetLocalizedValue((PaymentCurrency)currency)
                });
            }

            return currencies;
        }

        public async Task<List<long>> AddTagsAsync(string tags, TagType type)
        {
            // Hashset is faster than a list
            var tagList = tags.ToLower().Split(",").ToHashSet();

            var tagIds = new List<long>();

            // I think, we don't want to separate tags by type in this case
            // Separating them here makes it possible to have repeated tags in the DB = BAD
            var existingTags = await _contx.Tags.Where(t => tagList.Contains(t.Text)) // && t.Type == type
                .Select(t => new { t.Id, t.Text })
                .ToListAsync();

            foreach (var tag in tagList)
            {
                var existingTag = existingTags.FirstOrDefault(t => t.Text == tag);

                if (existingTag == null)
                {

                    var relatives = await _contx.Tags
                        .Select(t => new
                        {   
                            TagId = t.Id,
                            TagType = t.Type,
                            MatchDifference = EF.Functions.FuzzyStringMatchDifference(t.Text, tag) > 3
                        })
                        .Where(t => t.MatchDifference)
                        .OrderByDescending(t => t.MatchDifference)
                        .Take(2)
                        .Select(t => new TagRelative(t.TagId, t.TagType))
                        .ToListAsync();

                    var newTag = new Tag(tag, type);

                    if (relatives.Count != 0)
                        newTag.SetRelatives(relatives);

                    await _contx.Tags.AddAsync(newTag);
                    tagIds.Add(newTag.Id);
                    continue;
                }

                tagIds.Add(existingTag.Id);
            }

            return tagIds;
        }
    }
}
