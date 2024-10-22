﻿using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Main.Entities.User;
using WebApi.Main.Entities.Location;
using WebApi.Main.Entities.Achievement;
using WebApi.Main.Entities.Admin;
using WebApi.Main.Entities.PromoCode;
using WebApi.Main.Entities.Adventure;
using WebApi.Main.Entities.Tag;
using WebApi.Models.User;
using WebApi.Models.Models.Test;
using WebApi.Models.Models.User;
using WebApi.Models.Models.Adventure;
using WebApi.Enums.Enums.General;
using WebApi.Enums.Enums.Tag;
using WebApi.Models.Utilities;
using WebApi.Enums.Enums.User;
using WebApi.Enums.Enums.Adventure;
using WebApi.Enums.Enums.Responses;
using WebApi.Enums.Enums.Report;
using WebApi.Enums.Enums.Notification;
using WebApi.Enums.Enums.Hint;
using WebApi.Models.Models.Achievement;
using WebApi.Models.App_GlobalResources;
using WebApi.Interfaces;
using entities = WebApi.Main.Entities;
using models = WebApi.Models.Models;
using WebApi.Enums.Enums.Authentication;
using WebApi.Interfaces.Services;
using WebApi.Main.Models.Sponsor;

namespace WebApi.Repositories
{
	public class UserRepository : IUserRepository
	{
        private UserContext _contx { get; set; }
        private readonly ITimestampService timestamp;

        public UserRepository(UserContext context, ITimestampService timestampService)
        {
            _contx = context;
            timestamp = timestampService;
        }

        public async Task<long> RegisterUserAsync(UserRegistrationModel model, bool wasRegistered = false)
        {
            var country = "---";
            var city = "---";

            Location location = null;
            var user = new Main.Entities.User.User(model.Id);
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

            var uData = new Main.Entities.User.UserData
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
                UserRawDescription = model.Description,
                UserRealName = model.RealName,
            };

            var uSettings = new Main.Entities.User.Settings(model.Id, model.UsesOcean);
            var uStats = new Statistics(model.Id);

            user.LocationId = location.Id;
            user.UserName = model.UserName;

            await _contx.Users.AddAsync(user);
            await _contx.UserData.AddAsync(uData);
            await _contx.UsersSettings.AddAsync(uSettings);
            await _contx.UserLocations.AddAsync(location);
            await _contx.UserStatistics.AddAsync(uStats);
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
                var oceanStats = new Main.Entities.User.OceanStats(model.Id);
                var oceanPoints = new Main.Entities.User.OceanPoints(model.Id);

                await _contx.OceanPoints.AddAsync(oceanPoints);
                await _contx.OceanStats.AddAsync(oceanStats);
            }

            var invitation = await GetInvitationAsync(model.Id);

            if(invitation != null)
            {
                var invitor = await _contx.Users.Where(u => u.Id == invitation.InviterCredentials.UserId)
                    .Include(u => u.Settings)
                    .Include(u => u.Statistics)
                    .FirstOrDefaultAsync();

                invitor.InvitedUsersCount++;


                // TODO: CHECK IF INVITED VIA QR CODE
                invitor.InvitedUsersCount++;
                if (invitor.InvitedUsersCount == 6)
                    await GrantAchievementAsync(invitor.Id, 6);

                // Registered via referal credentials
                await GrantAchievementAsync(model.Id, 21);

                var bonus = invitor.PremiumExpirationDate != null ? 0.05f : 0f;
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

            await GrantAchievementAsync(model.Id, 1);

            return model.Id;
        }

        public async Task<UserInfo> GetUserInfoAsync(long id)
        {
            //Actualize premium information
            await CheckUserHasPremiumAsync(id);

            var userTags = await _contx.UserTags.Where(t => t.UserId == id)
                .Join(_contx.Tags,
                    ut => ut.TagId, t => t.Id,
                    (ut, t) => new
                    {
                        Name = t.Text,
                    }).ToListAsync();

            var userInfo = await _contx.Users.Where(u => u.Id == id)
                .Include(u => u.Data)
                .Include(u => u.Location)
                .Select(u => (UserInfo)u)
                .FirstOrDefaultAsync();

            userInfo.Tags = string.Join(", ", userTags);

            return userInfo;
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
                HasPremium = s.PremiumExpirationDate != null,
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
            if (currentUser.Settings.ShouldFilterUsersWithoutRealPhoto && currentUser.PremiumExpirationDate != null)
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

            var d = query
                .Select(u => new GetUserData((models.User.User)u, ""))
                .ToList();

            var data = await query.OrderBy(q => EF.Functions.Random())
                .Select(u => new GetUserData(u, ""))
                .Take(profileCount)
                .ToListAsync();

            //If user uses OCEAN+ functionality
            if (currentUser.Settings.UsesOcean)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    returnData.Add(await GetOceanMatchResult(currentUser, data[i], isRepeated));
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

        private async Task<GetUserData> AssembleProfileAsync(entities.User.User currentUser, GetUserData outputUser)
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

        private async Task<GetUserData> GetOceanMatchResult(entities.User.User currentUser, GetUserData managedUser, bool isRepeated)
        {
            var returnUser = await AssembleProfileAsync(currentUser, managedUser);

            //Pass if user does not use OCEAN+
            if (!managedUser.UsesOcean)
                return returnUser;

            var userActiveEffects = await GetUserActiveEffects(currentUser.Id);
            var deviation = 0.15f;
            var minDeviation = 0.05f;

            var valentineBonus = 1f;

            var hasActiveValentine = userActiveEffects.Any(e => e.Effect == Currency.TheValentine);

            var userHasDetectorOn = userActiveEffects.Any(e => e.Effect == Currency.TheDetector);

            if (hasActiveValentine)
                valentineBonus = 2;

            if (isRepeated)
            {
                deviation *= 1.5f;
                minDeviation *= 3.2f;
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

            var user2Points = await _contx.OceanPoints.Where(p => p.UserId == managedUser.UserId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var user2Stats = await _contx.OceanStats.Where(s => s.UserId == managedUser.UserId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var calculationResult = await OceanCalculator.Calculate(userPoints: userPoints, userStats: userStats, user2Points: user2Points, user2Stats: user2Stats, 
                valentineBonus: valentineBonus, deviation: deviation, minDeviation: minDeviation, important: important);

            // Ocean+ match counts only if there are 1 important param match or 3 secondary match
            if (calculationResult.ImportantMatches < 1 && calculationResult.SecondaryMatches < 3)
            {
                calculationResult.Bonus = "";
            }
            else
            {
                var cUserStats = await _contx.UserStatistics.Where(u => u.UserId == currentUser.Id)
                    .FirstOrDefaultAsync();

                var mUserStats = await _contx.UserStatistics.Where(u => u.UserId == managedUser.UserId)
                    .FirstOrDefaultAsync();

                cUserStats.HighSimilarityEncounters++;
                mUserStats.HighSimilarityEncounters++;

                switch (cUserStats.HighSimilarityEncounters)
                {
                    case 1:
                        await GrantAchievementAsync(cUserStats.UserId, 17);
                        break;
                    case 10:
                        await GrantAchievementAsync(cUserStats.UserId, 18);
                        break;
                    case 100:
                        await GrantAchievementAsync(cUserStats.UserId, 19);
                        break;
                    case 200:
                        await GrantAchievementAsync(cUserStats.UserId, 20);
                        break;
                }

                switch (mUserStats.HighSimilarityEncounters)
                {
                    case 1:
                        await GrantAchievementAsync(mUserStats.UserId, 17);
                        break;
                    case 10:
                        await GrantAchievementAsync(mUserStats.UserId, 18);
                        break;
                    case 100:
                        await GrantAchievementAsync(mUserStats.UserId, 19);
                        break;
                    case 200:
                        await GrantAchievementAsync(mUserStats.UserId, 20);
                        break;
                }

                await _contx.SaveChangesAsync();
            }

            //Add comment if user wants to see them
            if (currentUser.Settings.ShouldComment)
                returnUser.Comment = await GetRandomHintAsync(currentUser.Data.Language, HintType.Search);

            if (managedUser.HasPremium && managedUser.Nickname != "")
                calculationResult.Bonus += $"<b>{managedUser.Nickname}</b>\n";

            if (managedUser.IdentityType == IdentityConfirmationType.Partial)
                calculationResult.Bonus += $"☑️☑️☑️\n\n";
            else if (managedUser.IdentityType == IdentityConfirmationType.Full)
                calculationResult.Bonus += $"✅✅✅\n\n";

            if (userHasDetectorOn)
                calculationResult.Bonus += $"<b>OCEAN+ match!</b>\n<b>{calculationResult.Bonus}</b>";
            else
                calculationResult.Bonus += "<b>OCEAN+ match!</b>";

            returnUser.AddDescriptionUpwards(calculationResult.Bonus);

            return returnUser;
        }

        public async Task<Country> GetCountryAsync(long id)
        {
            var c = await _contx.Countries.Include(c => c.Cities).FirstOrDefaultAsync(c => c.Id == id);
            return c;
        }

        public async Task<long> AddFeedbackAsync(AddFeedback request)
        {
            var feedback = new entities.Report.Feedback
            {
                UserId = request.UserId,
                Reason = request.Reason,
                Text = request.Text,
                InsertedUtc = timestamp.GetUtcNow()
            };

            await _contx.Feedbacks.AddAsync(feedback);
            await _contx.SaveChangesAsync();

            return feedback.Id;
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

        public async Task AddUserReportAsync(SendUserReport request)
        {
            var reportedUser = await _contx.Users.Where(u => u.Id == request.ReportedUser)
                .FirstOrDefaultAsync();

            reportedUser.ReportCount++;

            if (reportedUser.ReportCount == 1)
                await GrantAchievementAsync(reportedUser.Id, 22);

            //Ban user if dailly report count is too high
            if (reportedUser.ReportCount >= 5)
                reportedUser.BanDate = timestamp.GetUtcNow();

            var report = new entities.Report.Report
            {
                SenderId = request.Sender,
                UserId = request.ReportedUser,
                Text = request.Text,
                Reason = request.Reason,
                InsertedUtc = timestamp.GetUtcNow()
            };

            await _contx.UserReports.AddAsync(report);

            await _contx.SaveChangesAsync();
        }

        public async Task AddAdventureReportAsync(SendAdventureReport request)
        {
            var reportedUser = await _contx.Adventures.Where(u => u.Id == request.Adventure)
                .Select(a => a.Creator)
                .FirstOrDefaultAsync();

            reportedUser.ReportCount++;

            //Ban user if dailly report count is too high
            if (reportedUser.ReportCount >= 5)
                reportedUser.BanDate = timestamp.GetUtcNow();

            var report = new entities.Report.Report
            {
                SenderId = request.Sender,
                AdventureId = request.Adventure,
                Text = request.Text,
                Reason = request.Reason,
                InsertedUtc = timestamp.GetUtcNow()
            };

            await _contx.UserReports.AddAsync(report);

            await _contx.SaveChangesAsync();
    }

        public async Task<List<entities.Report.Report>> GetMostRecentReports()
        {
            var reports = await _contx.UserReports.Where(ur => ur.AdminId == null)
                .ToListAsync();
            
            return reports;
        }

        public async Task<entities.Report.Report> GetUserReportByIdAsync(long id)
        {
            var reports = await _contx.UserReports.Where(r => r.Id == id)
                .FirstOrDefaultAsync();
            return reports;
        }

        public async Task<List<entities.Report.Report>> GetAllReportsOnUserAsync(long userId)
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

        public async Task<List<entities.Report.Report>> GetAllUserReportsBySenderAsync(long userId)
        {
            var reports = await _contx.UserReports.Where(u => u.SenderId == userId)
                .ToListAsync();

            return reports;
        }

        public async Task<byte> BanUserAsync(long userId)
        {
            var user = await _contx.Users.Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user.BanDate != null)
            {
                user.BanDate = timestamp.GetUtcNow();

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

            if (user.BanDate != null)
            {
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
                .Select(u => u.BanDate != null)
                .FirstOrDefaultAsync());
        }

        public async Task<bool> CheckUserIsDeleted(long userId)
        {
            return await _contx.Users.Where(u => u.Id == userId && u.DeleteDate != null)
                .AnyAsync();
        }

        public async Task<string> AddAchievementProgress(long userId, long achievementId, int progress)
        {
            var achievement = await _contx.UserAchievements
                .Where(a => a.UserId == userId && a.AchievementId == achievementId)
                .Include(a => a.Achievement)
                .FirstOrDefaultAsync();

            achievement.Progress += progress;
            _contx.UserAchievements.Update(achievement);
            await _contx.SaveChangesAsync();

            if (achievement.Progress >= achievement.Achievement.ConditionValue)
                return $"{Resources.ResourceManager.GetString("AchievementGrant_Message")}\n\n{achievement.Achievement.Description}\n{achievement.Achievement.Reward}p";

            return "";
        }

        public async Task<string> GrantAchievementAsync(long userId, long achievementId)
        {
            var achievement = await _contx.UserAchievements
                .Where(a => a.UserId == userId && a.AchievementId == achievementId && !a.IsAcquired)
                .Include(a => a.Achievement)
                .FirstOrDefaultAsync();

            var acquireMessage = $"{Resources.ResourceManager.GetString("AchievementGrant_Message")}\n\n{achievement.Achievement.Description}\n{achievement.Achievement.Reward}p";

            if (achievement == null)
                return "";
                //throw new Exception($"User have already acquired achievement #{achievementId} or it does not exist");

            achievement.IsAcquired = true;
            achievement.Progress = achievement.Achievement.ConditionValue;

            await TopUpPointBalance(userId, achievement.Achievement.Reward, "Achievement acquiring");

            await AddUserNotificationAsync(new UserNotification
            {
                UserId = userId,
                Section = Section.Neutral,
                Type = NotificationType.Other,
                Description = acquireMessage
            });

            _contx.UserAchievements.Update(achievement);
            await _contx.SaveChangesAsync();

            return acquireMessage;
        }

        public async Task<List<GetShortAchievement>> GetUserAchievements(long userId)
        {
            return await _contx.UserAchievements
                .Where(a => a.UserId == userId)
                .Select(a => new GetShortAchievement
                {
                    Id = a.AchievementId,
                    IsAcquired = a.IsAcquired,
                    Name = a.Achievement.Name
                }).ToListAsync();
        }

        public async Task<GetUserAchievement> GetSingleUserAchievement(long userId, long achievementId)
        {
            return await _contx.UserAchievements
                .Where(a => a.UserId == userId && a.AchievementId == achievementId)
                .Select(a => new GetUserAchievement
                {
                    Id = a.AchievementId,
                    IsAcquired = a.IsAcquired,
                    Progress = a.Progress,
                    Name = a.Achievement.Name,
                    ConditionValue = a.Achievement.ConditionValue,
                    Description = a.Achievement.Description,
                    Reward = a.Achievement.Reward
                }).FirstOrDefaultAsync();
        }

        public async Task GenerateUserAchievementList(long userId, AppLanguage localisationId, bool wasRegistered=false)
        {

            List<UserAchievement> userAchievements;
            
            // TODO: Remove
            if (wasRegistered)
            {
                userAchievements = await  _contx.UserAchievements
                    .Where(u => u.UserId == userId)
                    .ToListAsync();
                _contx.UserAchievements.RemoveRange(userAchievements);
            }

            userAchievements = new List<UserAchievement>();
            var sysAchievements = await _contx.Achievements.Where(a => a.Language == localisationId)
                .ToListAsync();

            sysAchievements.ForEach(a => userAchievements.Add(new UserAchievement(a.Id, userId, a.Language)));

            await _contx.UserAchievements.AddRangeAsync(userAchievements);
            await _contx.SaveChangesAsync();
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
                            await RegisterUserEncounter(new Main.Entities.User.RegisterEncounter { UserId = user1, EncounteredUserId = user2, Section = Section.RT });
                            await RegisterUserEncounter(new Main.Entities.User.RegisterEncounter { UserId = user2, EncounteredUserId = user1, Section = Section.RT });
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
                            await RegisterUserEncounter(new Main.Entities.User.RegisterEncounter { UserId = user1, EncounteredUserId = user2, Section = Section.RT});
                            await RegisterUserEncounter(new Main.Entities.User.RegisterEncounter { UserId = user2, EncounteredUserId = user1, Section = Section.RT });
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
                            await RegisterUserEncounter(new Main.Entities.User.RegisterEncounter { UserId = user1, EncounteredUserId = user2, Section = Section.RT});
                            await RegisterUserEncounter(new Main.Entities.User.RegisterEncounter { UserId = user2, EncounteredUserId = user1, Section = Section.RT });
                        }

                        return result;
                    }

                    await AddUserTrustProgressAsync(user1, 0.000005 * (double)userInfo1.BonusIndex);
                    await AddUserTrustProgressAsync(user2, 0.000005 * (double)userInfo2.BonusIndex);

                    await RegisterUserEncounter(new Main.Entities.User.RegisterEncounter { UserId = user1, EncounteredUserId = user2, Section = Section.RT });
                    await RegisterUserEncounter(new Main.Entities.User.RegisterEncounter { UserId = user2, EncounteredUserId = user1, Section = Section.RT });

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
            var time = timestamp.GetUtcNow();
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
            var time = timestamp.GetUtcNow();
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
                PointInTime = timestamp.GetUtcNow(),
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
            var timeNow = timestamp.GetUtcNow();

            var user = await _contx.Users.Where(u => u.Id == userId)
                .FirstOrDefaultAsync();
                
            if (user != null)
            {
                if ((user.PremiumExpirationDate < timeNow) || (user.PremiumExpirationDate == null))
                {
                    user.PremiumDuration = null;
                    user.PremiumExpirationDate = null;
                    //TODO: Notify user that his premium access has expired
                }


                await _contx.SaveChangesAsync(); 
                return user.PremiumExpirationDate != null;
            }
            return false;
        }

        public async Task<DateTime> GetPremiumExpirationDate(long userId)
        {

            var timeNow = timestamp.GetUtcNow();
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
            var timeNow = timestamp.GetUtcNow();
            var premiumFutureExpirationDate = timestamp.GetUtcNow().AddDays(dayDuration);

            var user = await _contx.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            var balance = await GetUserWalletBalance(userId);

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

        private async Task<Main.Entities.User.User> GetUserWithPremium(long userId, DateTime timeNow)
        {
            return await _contx.Users
                .Where(u => u.Id == userId && u.PremiumExpirationDate > timeNow)
                .AsNoTracking()
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

            if (model.AppLanguage != user.Data.Language)
            {
                await _contx.UserAchievements.Where(a => a.UserId == model.Id)
                    .ExecuteUpdateAsync(a => a.SetProperty(s => s.AchievementLanguage, s => model.AppLanguage));

                await _contx.UserTests.Where(a => a.UserId == model.Id)
                    .ExecuteUpdateAsync(a => a.SetProperty(s => s.TestLanguage, s => model.AppLanguage));
            }

            location.CountryId = model.CountryCode;
            location.CityCountryLang = model.CountryCode != null? model.AppLanguage : null;
            location.CityId = model.CityCode;
            location.CountryLang = model.CityCode != null? model.AppLanguage : null;

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

                if (user.DeleteDate != null)
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

        public async Task<GetRequests> GetUserRequests(long userId)
        {
            var requests = await _contx.Requests
                .Where(r => r.UserId == userId && r.Answer == null)
                .OrderByDescending(r => r.Type)
                .AsNoTracking()
                .Select(r => new GetRequest
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

            var model = new GetRequests(requests);

            var notification = await _contx.Notifications.Where(n => n.UserId == userId
             && n.Type == NotificationType.LikeNotification)
                .FirstOrDefaultAsync();

            if (notification != null)
            {
                model.Notification = notification.Description;

                _contx.Notifications.Remove(notification);
                await _contx.SaveChangesAsync();
            }

            return model;
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
            var notification = new UserNotification
            {
                UserId = model.UserId,
                Type = NotificationType.LikeNotification,
                Description = "<b>Someone had liked you</b>"
            };

            var request = new Request
            {
                UserId = model.UserId,
                SenderId = model.SenderId,
                IsMatch = model.IsMatch,
                Message = model.Message,
                Type = model.MessageType
            };

            var returnMessage = "";

            notification.Section = Section.Familiator;


            await RegisterUserEncounter(new Main.Entities.User.RegisterEncounter { UserId = (long)model.SenderId, EncounteredUserId = model.UserId, Section = Section.Familiator});


            //Register request
            await _contx.Requests.AddAsync(request);

            var requestsCount = await _contx.Notifications.Where(n => n.Type == NotificationType.Like)
                .AsNoTracking()
                .CountAsync();

            if (requestsCount > 3 && requestsCount < 7)
                notification.Description = "Your profile got some attention! See who has shown interest in you!";
            else if (requestsCount > 7 && requestsCount < 10)
                notification.Description = "Your attractiveness has not gone unnoticed! See who's interested in you";
            else if (requestsCount > 10)
                notification.Description = "Your profile is the center of attention! Check out who's showing interest in you!";

            //Register notification
            await AddUserNotificationAsync(notification);

            var userStats = await _contx.UserStatistics.Where(s => s.UserId == model.UserId)
                    .FirstOrDefaultAsync();

            var senderStats = await _contx.UserStatistics.Where(s => s.UserId == model.SenderId)
                .FirstOrDefaultAsync();

            if (!model.IsMatch)
            {
                senderStats.Likes++;

                switch (senderStats.Likes)
                {
                    case 10:
                        await GrantAchievementAsync(model.SenderId, 33);
                        break;
                    case 50:
                        await GrantAchievementAsync(model.SenderId, 34);
                        break;
                    case 100:
                        await GrantAchievementAsync(model.SenderId, 35);
                        break;
                    case 250:
                        await GrantAchievementAsync(model.SenderId, 36);
                        break;
                    default:
                        break;
                }

                userStats.LikesReceived++;

                switch (userStats.LikesReceived)
                {
                    case 1:
                        await GrantAchievementAsync(model.UserId, 14);
                        break;
                    case 50:
                        await GrantAchievementAsync(model.UserId, 16);
                        break;
                    case 100:
                        await GrantAchievementAsync(model.UserId, 15);
                        break;
                    default:
                        break;
                }

                await _contx.SaveChangesAsync();
            }
            else
            {
                userStats.Matches++;
                senderStats.Matches++;

                switch (userStats.Matches)
                {
                    case 10:
                        await GrantAchievementAsync(model.UserId, 29);
                        break;
                    case 50:
                        await GrantAchievementAsync(model.UserId, 30);
                        break;
                    case 100:
                        await GrantAchievementAsync(model.UserId, 31);
                        break;
                    case 250:
                        await GrantAchievementAsync(model.UserId, 32);
                        break;  
                    default:
                        break;
                }

                switch (senderStats.Matches)
                {
                    case 10:
                        await GrantAchievementAsync(model.SenderId, 29);
                        break;
                    case 50:
                        await GrantAchievementAsync(model.SenderId, 30);
                        break;
                    case 100:
                        await GrantAchievementAsync(model.SenderId, 31);
                        break;
                    case 250:
                        await GrantAchievementAsync(model.SenderId, 32);
                        break;
                    default:
                        break;
                }
            }

            return returnMessage;
        }

        //TODO: Make more informative and interesting
        public async Task<string> DeclineRequestAsync(long user, long encounteredUser)
        {
            var sim = await GetSimilarityBetweenUsersAsync(user, encounteredUser);

            var doesExist = await _contx.Requests.AnyAsync(r => r.UserId == user && r.SenderId == encounteredUser);

            //Encounter is not registered anywhere but here in that case
            await RegisterUserEncounter(new Main.Entities.User.RegisterEncounter
			{
                UserId = user,
                EncounteredUserId = encounteredUser,
                Section = Section.Familiator
            });

            var userStats = await _contx.UserStatistics.Where(s => s.UserId == user)
                .FirstOrDefaultAsync();
            
            if (doesExist)
            {
                userStats.DiscardedMatches++;
                switch (userStats.DiscardedMatches)
                {
                    case 10:
                        await GrantAchievementAsync(user, 11);
                        break;
                    case 50:
                        await GrantAchievementAsync(user, 12);
                        break;
                    case 200:
                        await GrantAchievementAsync(user, 13);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                userStats.DislikedProfiles++;
                switch (userStats.DislikedProfiles)
                {
                    case 10:
                        await GrantAchievementAsync(user, 7);
                        break;
                    case 50:
                        await GrantAchievementAsync(user, 8);
                        break;
                    case 100:
                        await GrantAchievementAsync(user, 9);
                        break;
                    case 250:
                        await GrantAchievementAsync(user, 10);
                        break;
                    default:
                        break;
                }
            }

            await _contx.SaveChangesAsync();

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

        private async Task<string> AcceptUserRequestAsync(long userId, long requesSenderId)
        {
            var returnMessage = "";
            var request = new Request
            {
                SenderId = userId,
                UserId = requesSenderId,
                IsMatch = true
            };

            var notification = new UserNotification
            {
                Description = "<b>Someone had liked you</b>",
                UserId = requesSenderId,
                Section = Section.Requester,
                Type = NotificationType.LikeNotification
            };

            var userStats = await _contx.UserStatistics.Where(s => s.UserId == userId)
                .FirstOrDefaultAsync();

            userStats.Matches++;

            var senderStats = await _contx.UserStatistics.Where(s => s.UserId == requesSenderId)
                .FirstOrDefaultAsync();

            senderStats.Matches++;

            // TODO: Create related achievements and implement check
            //switch (userStats.Match)
            //{
            //    case 5:
            //    default:
            //        break;
            //}

            //TODO: Same for sender

            if (new Random().Next(0, 2) == 0)
            {
                var senderUserName = await _contx.Users.Where(d => d.Id == userId)
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
                var receiverUserName = await _contx.Users
                    .Where(d => d.Id == requesSenderId)
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

            var requestsCount = await _contx.Notifications.Where(n => n.Type == NotificationType.Like)
                .AsNoTracking()
                .CountAsync();

            if (requestsCount > 3 && requestsCount < 7)
                notification.Description = "Your profile got some attention! See who has shown interest in you!";
            else if (requestsCount > 7 && requestsCount < 10)
                notification.Description = "Your attractiveness has not gone unnoticed! See who's interested in you";
            else if (requestsCount > 10)
                notification.Description = "Your profile is the center of attention! Check out who's showing interest in you!";

            //Register notification
            await AddUserNotificationAsync(notification);

            await _contx.Requests.AddAsync(request);
            await _contx.SaveChangesAsync();

            return returnMessage;
        }

        public async Task<string> AnswerUserRequestAsync(long requestId, RequestAnswer reaction)
        {
            var request = await _contx.Requests.Where(r => r.Id == requestId)
                .FirstOrDefaultAsync();

            request.Answer = reaction;
            request.AnsweredTimeStamp = timestamp.GetUtcNow();
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

        public async Task SetAllBusyStatusToFalse()
        {
            var sql = "update users set \"IsBusy\" = False ";

            await _contx.Database.ExecuteSqlRawAsync(sql);
        }

        public async Task AssignAdminRightsAsync(List<long> userIds)
        {
            var role = Role.Admin.ToLowerString();
            var sql = $"UPDATE users SET \"IsAdmin\" = True WHERE \"Id\" IN ({string.Join(",", userIds)});";

            sql += string.Join(";", userIds.Select(id =>
                $"INSERT INTO user_roles (\"UserId\", \"Role\")" +
                $"SELECT {id}, '{role}' " +
                $"WHERE NOT EXISTS (" +
                $"SELECT 1 FROM user_roles WHERE \"UserId\" = {id} AND \"Role\" = '{role}'" +
                $")"
            ));

            await _contx.Database.ExecuteSqlRawAsync(sql);
        }

        public async Task AssignSponsorRightsAsync(List<long> userIds)
        {
            var role = Role.Sponsor.ToLowerString();
            var sql = $"UPDATE users SET \"IsSponsor\" = True WHERE \"Id\" IN ({string.Join(",", userIds)});";

            sql += string.Join(";", userIds.Select(id =>
                $"INSERT INTO user_roles (\"UserId\", \"Role\")" +
                $"SELECT {id}, '{role}' " +
                $"WHERE NOT EXISTS (" +
                $"SELECT 1 FROM user_roles WHERE \"UserId\" = {id} AND \"Role\" = '{role}'" +
                $")"
            ));

            await _contx.Database.ExecuteSqlRawAsync(sql);
        }

        public async Task RemoveAllEncountersAsync() // TODO: Remove in production
        {
            var sql = $"delete from encounters";

            await _contx.Database.ExecuteSqlRawAsync(sql);
        }

        public async Task RegisterUserEncounter(Main.Entities.User.RegisterEncounter model)
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

            await _contx.Encounters.AddAsync(new Encounter(model, timestamp.GetUtcNow()));
            
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

        public async Task<TrustLevel> GetUserTrustLevel(long userId)
        {
            return await _contx.TrustLevels
                .FindAsync(userId);
        }

        private async Task<long> AddUserTrustLevel(long userId)
        {
            await _contx.TrustLevels.AddAsync(TrustLevel.CreateDefaultTrustLevel(userId));
            await _contx.SaveChangesAsync();
            return userId;
        }

        public async Task<bool> UpdateUserNickname(long userId, string nickname)
        {
            var currentUser = await _contx.Users.FindAsync(userId);

            if (currentUser.PremiumExpirationDate != null)
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
                    InvitationTime = timestamp.GetUtcNow()
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
                    model.Type == NotificationType.VerificationRequest || 
                    model.Type == NotificationType.PremiumEnd ||
                    model.Type == NotificationType.LikeNotification ||
                    model.Type == NotificationType.ReferentialRegistration)
                {
                    var notification = await _contx.Notifications.Where(n => n.UserId == model.UserId && n.Type == model.Type)
                        .FirstOrDefaultAsync();

                    if (notification != null)
                        notification.Description = model.Description;
                    else
                        await _contx.Notifications.AddAsync(model);

                    await _contx.SaveChangesAsync();
                }

                else if (model.Type == NotificationType.AdventureParticipation)
                {
                    var notification = await _contx.Notifications.Where(n => n.UserId == model.UserId && n.Type == model.Type)
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
                    var notification = await _contx.Notifications.Where(n => n.UserId == model.UserId && n.Type == model.Type)
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
                .Select(a => $"\n\n{a.Achievement.Name}\n\n{a.Achievement.Description}\nProgress: {a.Progress} / {a.Achievement.ConditionValue}\nReward: {a.Achievement.Reward}p")
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

        public async Task<int> GetUserMaximumLanguageCountAsync(long userId)
        {
            var user = await _contx.Users.FindAsync(userId);

            if (user == null)
                return GetMaximumLanguageCount(null);

            return GetMaximumLanguageCount(user.PremiumExpirationDate != null);
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

                var stats = await _contx.UserStatistics.Where(s => s.UserId == model.UserId)
                    .FirstOrDefaultAsync();

                stats.TestsPassed++;
                switch (stats.TestsPassed)
                {
                    case 1:
                        await GrantAchievementAsync(model.UserId, 4);
                        break;
                    case 5:
                        await GrantAchievementAsync(model.UserId, 5);
                        break;
                    case 10:
                        await GrantAchievementAsync(model.UserId, 6);
                        break;
                    default:
                        break;
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

        public async Task<models.User.OceanPoints> GetOceanPoints(long userId)
        {
            var points = await _contx.OceanPoints
                    .Where(s => s.UserId == userId)
                    .FirstOrDefaultAsync();

            if (points == null)
            {
                points = new entities.User.OceanPoints(userId);
                await _contx.OceanPoints.AddAsync(points);
            }

            return (models.User.OceanPoints) points;
        }

        private async Task<models.User.OceanPoints> RecalculateSimilarityPercentage(PointsPayload model)
        {
            try
            {
                var points = await GetOceanPoints(model.UserId);

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
                userStats = new Main.Entities.User.OceanStats(model.UserId);
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
            return new RecalculationResult { Stats = (models.User.OceanStats)userStats, TestResult = result};
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
                        oceanStats = new Main.Entities.User.OceanStats(userId);
                        await _contx.OceanStats.AddAsync(oceanStats);
                        await _contx.SaveChangesAsync();
                    }

                    //Add ocean points, if they were not created when user was registered
                    if (oceanPoints == null)
                    {
                        oceanPoints = new entities.User.OceanPoints(userId);
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

                userTest.PassedOn = timestamp.GetUtcNow();
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

        public async Task UpdateAdventureTagsAsync(long adventureId, List<long> tags)
        {
            // Remove old tags related to adventure
            _contx.UserTags.RemoveRange(_contx.UserTags.Where(t => t.UserId == adventureId));

            var newTags = tags.Select(t => new AdventureTag(t, adventureId, TagType.Tags));

            await _contx.AdventureTags.AddRangeAsync(newTags);
            await _contx.SaveChangesAsync();
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
            if (currentUser.Settings.ShouldFilterUsersWithoutRealPhoto && currentUser.PremiumExpirationDate != null)
                query = query.Where(u => u.IdentityType != IdentityConfirmationType.None);

            //Add user search
            currentUser.TagSearchesCount++;

            var extractedTags = await AddTagsAsync(model.Tags, TagType.Tags);
            
            // TODO: Come up with some premium benifits
            if(currentUser.PremiumExpirationDate != null)
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
                outputUser = await GetOceanMatchResult(currentUser, user, false);
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
                            //await ActivateToggleEffectAsync(userId, effectId);
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

                await CreateUserBalance(userId, 0, timestamp.GetUtcNow());
                return false;
            }
            catch { return false; }
        }

        public async Task<DateTime?> ActivateDurableEffectAsync(long userId, Currency effectId)
        {
            try
            {
                entities.Effect.ActiveEffect effect;
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
                                    effect = new Main.Entities.Effect.TheValentine(userId, timestamp.GetUtcNow());
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
                            effect = new Main.Entities.Effect.TheDetector(userId, timestamp.GetUtcNow());
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

        private bool AtLeastOneIsNotZero(Main.Entities.User.OceanPoints points)
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
                var effectsToRemove = new List<Main.Entities.Effect.ActiveEffect>();
                var effectsToReturn = new List<GetActiveEffect>();
                var activeEffects = await _contx.ActiveEffects
                    .Where(e => e.UserId == userId)
                    .ToListAsync();

                foreach (var effect in activeEffects)
                {
                    if (effect.ExpirationTime.Value <= timestamp.GetUtcNow())
                        effectsToRemove.Add(effect);
                    else
                        effectsToReturn.Add(new GetActiveEffect((models.Effect.ActiveEffect)effect));
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

        public async Task<bool> SendTickRequestAsync(SendVerificationRequest request)
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
                    existingRequest.Media = request.Media;
                    existingRequest.MediaType = request.MediaType;
                    existingRequest.Gesture = request.Gesture;
                    existingRequest.ConfirmationType = request.ConfirmationType;
                    existingRequest.Status = VerificationRequestStatus.ToView;
                    existingRequest.User.IdentityType = IdentityConfirmationType.Awaiting;

                    _contx.TickRequests.Update(existingRequest);
                    _contx.Users.Update(existingRequest.User);
                    await _contx.SaveChangesAsync();
                    return true;
                }

                var user = await _contx.Users.Where(u => u.Id == request.UserId)
                    .FirstOrDefaultAsync();

                user.IdentityType = IdentityConfirmationType.Awaiting;

                var model = (VerificationRequest)request;

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

        public async Task<List<GetTestShortData>> GetTestDataByPropertyAsync(long userId, Enums.Enums.User.OceanStats param)
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
                .Select(t => new GetFullTestData {Id = t.Id, Name = t.Name, Description = t.Description, TestType = t.TestType})
                .FirstOrDefaultAsync();
        }

        public async Task<GetUserTest> GetUserTestAsync(long userId, long testId)
        {
            var test = await _contx.UserTests.Where(t => t.UserId == userId && t.TestId == testId)
                .Include(t => t.Test)
                .Include(t => t.Test.Questions)
                .Include(t => t.Test.Questions)
                .ThenInclude(t => t.Answers)
                .Include(t => t.Test.Results)
                .Include(t => t.Test.Scales)
                .FirstOrDefaultAsync();

            if (test == null)
                throw new NullReferenceException($"User {userId} does not have Test {testId}");

            return (GetUserTest)test;
        }

        public async Task<int> GetPossibleTestPassRangeAsync(long userId, long testId)
        {
            var test = await _contx.UserTests.Where(t => t.UserId == userId && t.TestId == testId)
                .Include(t => t.Test)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (test == null)
                throw new NullReferenceException($"User #{userId} does not have test #{testId} available");

            //If date of passing is equal to null => test had not been passed so far
            if (test.PassedOn == null)
                return 0;

            //Every test has its own passing range. If date of passing is out of it => Test can be passed again
            if ((timestamp.GetUtcNow() - test.PassedOn).Value.Days > test.Test.CanBePassedInDays)
                return 0;

            //Get number of days in which this test can be passed again
            var result = (test.Test.CanBePassedInDays - (timestamp.GetUtcNow() - test.PassedOn).Value.Days);
            return result?? 0;
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

            await _contx.UserTests.AddAsync(new Main.Entities.User.UserTest
            {
                UserId = userId,
                TestId = testId,
                TestType = test.TestType,
                Result = 0,
                TestLanguage = localisation
            });
                
            await _contx.SaveChangesAsync();
        }

        public async Task<List<GetTestShortData>> GetUserTestDataByPropertyAsync(long userId, Enums.Enums.User.OceanStats? param)
        {
            //Get users tests
            return await _contx.UserTests.Where(t => t.UserId == userId && t.TestType == param)
                .Include(t => t.Test)
                .Select(t => new GetTestShortData { Id = t.TestId, Name = t.Test.Name })
                .ToListAsync();
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
                stats = new Main.Entities.User.OceanStats(userId);
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

            if (sender.PremiumExpirationDate != null && sender.Nickname != "")
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

            var adventure = new entities.Adventure.Adventure
            {
                UserId = model.UserId,
                Name = model.Name,
                CountryId = model.CountryId,
                CityId = model.CityId,
                CountryLang = userLang,
                CityCountryLang = userLang,
                Description = model.Description,
                MediaType = model.MediaType,
                Media = model.Media,
                AutoReplyType = model.AutoReplyType,
                AutoReply = model.AutoReply,
                IsOffline = model.IsOffline,
                IsAwaiting = model.IsAwaiting,
                UniqueLink = Guid.NewGuid().ToString("N").Substring(0, 7).ToUpper(),
                Status = AdventureStatus.ToView
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

            adventure.Name = model.Name;
            adventure.Media = model.Media;
            adventure.MediaType = model.MediaType;
            adventure.CityId = model.CityId;
            adventure.CountryId = model.CountryId;
            adventure.Description = model.Description;
            adventure.AutoReplyType = model.AutoReplyType;
            adventure.AutoReply = model.AutoReply;
            adventure.Status = AdventureStatus.ToView;
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

            var userName = await _contx.Users.Where(u => u.Id == userId)
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

            adventure.DeleteDate = timestamp.GetUtcNow();

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
                .Include(a => a.Creator)
                .FirstOrDefaultAsync();

            attendee.Status = status;

            if (status == AdventureAttendeeStatus.Accepted)
            {
                var contact = string.IsNullOrEmpty(adventure.GroupLink) ? 
                    $"You may contact its creator @{adventure.Creator.UserName} and discuss details" : 
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

        public async Task<List<Models.Models.Adventure.Adventure>> GetUsersSubscribedAdventuresAsync(long userId)
        {
            var adventureIds = await _contx.AdventureAttendees.Where(a => a.UserId == userId)
                .Select(a => a.AdventureId)
                .ToListAsync();

            return await _contx.Adventures.Where(a => adventureIds.Contains(a.Id))
                .Select(a => (models.Adventure.Adventure)a)
                .ToListAsync();
        }

        public async Task<List<GetAdventure>> GetUserAdventuresAsync(long userId)
        {
            return await _contx.Adventures.Where(a => a.UserId == userId)
                .Where(a => a.DeleteDate == null)
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

            tickRequest.Status = VerificationRequestStatus.ToView;
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
                    Username = u.UserName,
                    UserRealName = u.Data.UserRealName,
                    HasPremium = u.PremiumExpirationDate != null,
                    IsBanned = u.BanDate != null,
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

        public async Task<entities.Adventure.Adventure> GetAdventureAsync(long id)
        {
            return await _contx.Adventures
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<entities.Adventure.Adventure> ResolveAdventure(ResolveAdventure model)
        {
			var adventure = await GetAdventureAsync(model.Id);

            if (!string.IsNullOrEmpty(model.Tags))
            {
                var tags = await AddTagsAsync(model.Tags, TagType.Tags);
                await UpdateAdventureTagsAsync(model.Id, tags);
            }

			if (adventure == null)
				throw new NullReferenceException("Adventure was not found");

			adventure.Status = model.Status;
			adventure.AdminId = model.AdminId;

            await _contx.SaveChangesAsync();

            return adventure;
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

            user.DeleteDate = timestamp.GetUtcNow();

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
            if (user.DeleteDate == null)
                return RestoreResult.Error;

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

            var query = _contx.Adventures.Where(q => q.DeleteDate == null && q.UserId != userId)
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
                AutoReplyType = q.AutoReplyType
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
                        .Select(t => new Main.Entities.Tag.TagRelative(t.TagId, t.TagType))
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

        // TODO: Finish up
        public async Task ProcessInterestsDataAsync(QuestionerPayload model)
        {
            var userId = await _contx.Users.Where(u => u.UserName == model.Username)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (userId == 0)
                throw new NullReferenceException("User with such username does not exist !");

            var userStats = await _contx.UserStatistics.Where(s => s.UserId == userId)
                .FirstOrDefaultAsync();

            // TODO: Achievements
            switch (userStats.QuestionerPasses)
            {
                default:
                    break;
            }
        }

        public async Task SetUserStoryAsync(SetStory model)
        {
            var userData = await _contx.UserData.Where(d => d.Id == model.UserId)
                .FirstOrDefaultAsync();

            if (userData == null)
                return;

            userData.UserStory = model.Story;

            await _contx.SaveChangesAsync();
        }

        public async Task RemoveUserStoryAsync(long userId)
        {
            var userData = await _contx.UserData.Where(d => d.Id == userId)
                .FirstOrDefaultAsync();

            if (userData == null)
                return;

            userData.UserStory = null;

            await _contx.SaveChangesAsync();
        }

        public async Task<models.User.User> GetUserAsync(long id)
        {
            return await _contx.Users.Where(u => u.Id == id)
                .AsNoTracking()
                .Select(u => (models.User.User)u)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Role>> GetUserRolesAsync(long userId)
        {
            return await _contx.UserRoles.Where(u => u.UserId == userId)
                .AsNoTracking()
                .Select(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<List<entities.Report.Feedback>> GetAllFeedbackAsync()
        {
            var feedbacks = await _contx.Feedbacks
                .Include(u => u.User)
                .ToListAsync();

            return feedbacks;
        }

        public async Task<List<entities.Report.Feedback>> GetRecentFeedbackAsync()
        {
            var feedbacks = await _contx.Feedbacks.Where(f => f.AdminId == null)
                .ToListAsync();

            return feedbacks;
        }

        public async Task<List<entities.Report.Report>> GetRecentReportsAsync()
        {
            var reports = await _contx.UserReports.Where(r => r.AdminId == null)
                .ToListAsync();

            return reports;
        }

        public async Task<int> CountRecentFeedbacksAsync()
        {
            var count = await _contx.Feedbacks.Where(f => f.AdminId == null)
                .CountAsync();

            return count;
        }

        public async Task<int> CountRecentReportsAsync()
        {
            var count = await _contx.UserReports.Where(f => f.AdminId == null)
                .CountAsync();

            return count;
        }

        public async Task<int> CountPendingVerificationRequestsAsync()
        {
            var count = await _contx.TickRequests.
                Where(r => r.Status == VerificationRequestStatus.ToView)
                .CountAsync();

            return count;
        }

        public async Task<int> CountPendingAdvertisementsAsync()
        {
            var count = await _contx.Advertisements.Where(a => a.AdminId == null)
                .CountAsync();

            return count;
        }

        public async Task<int> CountPendingAdventuresAsync()
        {
            var count = await _contx.Adventures.Where(a => a.AdminId == null)
                .CountAsync();

            return count;
        }

        public async Task<UserMedia> GetUserMediaAsync(long userId)
        {
            return await _contx.UserData.Where(d => d.Id == userId)
                .Select(d => new UserMedia
                {
                    UserId = d.Id,
                    Media = d.UserMedia,
                    MediaType = d.MediaType
                }).FirstOrDefaultAsync();
        }

        public async Task<AppLanguage> GetUserLanguageAsync(long userId)
        {
            return await _contx.UserData.Where(d => d.Id == userId)
                .Select(d => d.Language).FirstOrDefaultAsync();
        }

		public async Task<ICollection<entities.Adventure.Adventure>> GetPendingAdventuresAsync()
		{
            var adventures = _contx.Adventures.Where(a => a.Status == AdventureStatus.ToView || a.DeleteDate == null)
                .Take(3);

            // Set status = InProcess
            await adventures.ExecuteUpdateAsync(a => a.SetProperty(ad => ad.Status, AdventureStatus.InProcess));

            return await adventures.ToListAsync();
		}
	}
}
