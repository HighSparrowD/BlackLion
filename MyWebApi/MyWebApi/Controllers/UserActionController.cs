using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyWebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWebApi.Entities.UserInfoEntities;
using MyWebApi.Entities.ReportEntities;
using MyWebApi.Entities.LocationEntities;
using MyWebApi.Entities.ReasonEntities;
using MyWebApi.Entities.AchievementEntities;
using MyWebApi.Entities.UserActionEntities;
using MyWebApi.Entities.SponsorEntities;
using MyWebApi.Entities.DailyTaskEntities;
using MyWebApi.Entities.TestEntities;
using static MyWebApi.Enums.SystemEnums;
using MyWebApi.Entities.AdminEntities;
using MyWebApi.Entities.EffectEntities;
using MyWebApi.Entities.AdventureEntities;
using MyWebApi.Enums;

namespace MyWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserActionController : ControllerBase
    {
        //private User currentUser = Entities.UserInfoEntities.User.CreateDummyUser(); //TODO: relocate to an application 
        private readonly ILogger<UserActionController> _logger;
        private IUserRepository _repository;

        public UserActionController(ILogger<UserActionController> logger, IUserRepository rep)
        {
            _logger = logger;
            _repository = rep;
        }

        [HttpGet("/CheckUserExists/{userId}")]
        public async Task<bool> CheckUserExists(long userId)
        {
            return await _repository.CheckUserExists(userId);
        }

        [HttpGet("/CheckUserIsRegistered/{userId}")]
        public async Task<bool> CheckUserIsRegistered(long userId)
        {
            return await _repository.CheckUserIsRegistered(userId);
        }

        [HttpGet("/CheckUserHasVisitedSection/{userId}/{sectionId}")]
        public async Task<bool> CheckUserHasVisitedSection(long userId, int sectionId)
        {
            return await _repository.CheckUserHasVisitedSection(userId, sectionId);
        }

        [HttpGet("/CheckUserIsDeleted/{userId}")]
        public async Task<bool> CheckUserIsDeleted(long userId)
        {
            return await _repository.CheckUserIsDeleted(userId);
        }

        [HttpGet("/CheckUserIsBanned/{userId}")]
        public async Task<bool> CheckUserIsBanned(long userId)
        {
            return await _repository.CheckUserIsBanned(userId);
        }

        [HttpGet("/CheckUsersAreCombinableRT/{user1}/{user2}")]
        public async Task<bool> CheckUsersAreCombinableRT(long user1, long user2)
        {
            return await _repository.CheckUsersAreCombinableRT(user1, user2);
        }

        [HttpGet("/GetUserAppLanguage/{userId}")]
        public async Task<int> GetUserLanguagePrefs(long userId)
        {
            return await _repository.GetUserAppLanguage(userId);
        }

        [HttpGet("/UserInfo/{userId}")]
        public async Task<ActionResult<User>> GetUserInfo(long userId)
        {
            return Ok(await _repository.GetUserInfoAsync(userId));
        }

        [HttpPost("/UpdateUserProfile")]
        public async Task<byte> UpdateUserProfile(UpdateUserProfile model)
        {
            if (model.WasChanged)
            {
                var langCount = await GetUserMaximumLanguageCount(model.Id);
                if (model.UserLanguages.Count > langCount)
                    throw new Exception($"This user cannot have more than {langCount} languages !");

                Location location = new Location { Id = model.Id};

                if(model.UserCityCode != null && model.UserCountryCode != null)
                {
                    location.CityId = (int)model.UserCityCode;
                    location.CityCountryClassLocalisationId = model.UserAppLanguageId;
                    location.CountryId = (int)model.UserCountryCode;
                    location.CountryClassLocalisationId = model.UserAppLanguageId;
                }

                var uBase = new UserBaseInfo(model.Id, model.UserName, model.UserRealName, "", model.UserPhoto, model.IsPhotoReal);
                uBase.UserRawDescription = model.UserDescription;
                var uData = new UserDataInfo
                {
                    Id = model.Id,
                    UserLanguages = model.UserLanguages,
                    ReasonId = model.ReasonId,
                    UserAge = model.UserAge,
                    UserGender = model.UserGender,
                    LanguageId = model.UserAppLanguageId,
                    LocationId = location.Id,
                };
                var uPrefs = new UserPreferences(model.Id, model.UserLanguagePreferences, model.UserLocationPreferences, model.AgePrefs, model.CommunicationPrefs, model.UserGenderPrefs, model.ShouldUserPersonalityFunc);

                if ((await _repository.UpdateUserAppLanguageAsync(model.Id, model.UserAppLanguageId)) == 1)
                    if (location== null || (await _repository.UpdateUserLocationAsync(location)) == 1)
                        if ((await _repository.UpdateUserDataAsync(uData)) == 1)
                            if ((await _repository.UpdateUserBaseAsync(uBase)) == 1)
                                if ((await _repository.UpdateUserPreferencesAsync(uPrefs)) == 1)
                                    return 1;
            return 0;
            }

            return 1;
        }

        [HttpGet("/UpdateUserAppLanguage/{userId}/{appLanguage}")]
        public async Task<ActionResult<byte>> UpdateUserAppLanguage(long userId, int appLanguage)
        {
            return await _repository.UpdateUserAppLanguageAsync(userId, appLanguage);
        }

        [HttpPost("/UpdateUserBase")]
        public async Task<ActionResult<byte>> UpdateUserBase(UserBaseInfo user)
        {
            return await _repository.UpdateUserBaseAsync(user);
        }

        [HttpPost("/UpdateUserData")]
        public async Task<ActionResult<byte>> UpdateUserBase(UserDataInfo user)
        {
            return await _repository.UpdateUserDataAsync(user);
        }

        [HttpPost("/UpdateUserPreferences")]
        public async Task<ActionResult<byte>> UpdateUserBase(UserPreferences user)
        {
            return await _repository.UpdateUserPreferencesAsync(user);
        }

        [HttpPost("/UpdateUserLocation")]
        public async Task<ActionResult<byte>> UpdateUserLocation(Location location)
        {
            return await _repository.UpdateUserLocationAsync(location);
        }

        [HttpGet("/GetUserByUsername/{username}")]
        public async Task<ActionResult<User>> GetUserInfo(string username)
        {
            return await _repository.GetUserInfoByUsrnameAsync(username);
        }

        [HttpGet("/GetUserBaseInfo/{userId}")]
        public async Task<ActionResult<UserBaseInfo>> UserBaseInfo(long userId)
        {
            return await _repository.GetUserBaseInfoAsync(userId);
        }

        [HttpGet("/GetUserList/{userId}")]
        public async Task<List<GetUserData>> GetUserList(long userId)
        {
            return await _repository.GetUsersAsync(userId);
        }

        [HttpGet("/GetUserList/TurnOffP/{userId}")]
        public async Task<List<GetUserData>> GetUserList2(long userId)
        {
            return await _repository.GetUsersAsync(userId, turnOffPersonalityFunc: true);
        }

        [HttpGet("/GetUserList/FreeSearch/{userId}")]
        public async Task<List<GetUserData>> GetUserList3(long userId)
        {
            return await _repository.GetUsersAsync(userId, isFreeSearch: true);
        }

        [HttpGet("/GetFriends")]
        public async Task<IEnumerable<FriendModel>> GetFriendsList()
        {
            return await _repository.GetFriendsAsync();
        }

        [HttpGet("/GetFriendInfo/{userId}")]
        public async Task<User> GetFriendInfo(long userId)
        {
            return await _repository.GetFriendInfoAsync(userId);
        }

        [HttpPost("/AddFriend/{userId}")]
        public async Task<long> AddFriendUser(long userId)
        {
            var actionId = _repository.AddFriendUserAsync(userId);
            return await actionId;
        }

        [HttpGet("/GetBaseUserInfo")]
        public Task<List<User>> GetBaseUserInfo()
        {
            throw new NotImplementedException();
        }



        [HttpGet("/GetCountry/{id}")]
        public async Task<Country> GetBaseUserInfo(long id)
        {
            return await _repository.GetCountryAsync(id);
        }

        [HttpGet("/GetFeedbackReasons/{localisationId}")]
        public async Task<List<FeedbackReason>> GetFeedbackReasons(int localisationId)
        {
            return await _repository.GetFeedbackReasonsAsync(localisationId);
        }

        [HttpGet("/GetReportReasons/{localisationId}")]
        public async Task<List<ReportReason>> GetReportReasons(int localisationId)
        {
            return await _repository.GetReportReasonsAsync(localisationId);
        }


        [HttpPost("/AddFeedback")]
        public async Task<long> AddFeedback(Feedback report)
        {
            report.InsertedUtc = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            return await _repository.AddFeedbackAsync(report);
        }

        [HttpPost("/AddUserReport")]
        public async Task<long> AddUserReport(Report report)
        {
            report.InsertedUtc = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            return await _repository.AddUserReportAsync(report);
        }


        [HttpPost("/RegisterUser")]
        public async Task<long> AddUser(UserRegistrationModel model)
        {
            var langCount = await GetUserMaximumLanguageCount(model.Id);
            if (model.UserLanguages.Count > langCount)
                throw new Exception($"This user cannot have more than {langCount} languages !");

            Location location = null;
            var uBase = new UserBaseInfo(model.Id, model.UserName, model.UserRealName, model.UserDescription, model.UserPhoto, model.IsPhotoReal);
            var uData = new UserDataInfo
            {
                Id = model.Id,
                UserLanguages = model.UserLanguages,
                ReasonId = model.ReasonId,
                UserAge = model.UserAge,
                UserGender = model.UserGender,
                LanguageId = model.UserAppLanguageId,
            };
            var uPrefs = new UserPreferences(model.Id, model.UserLanguagePreferences, model.UserLocationPreferences, model.AgePrefs, model.CommunicationPrefs, model.UserGenderPrefs, model.ShouldUserPersonalityFunc);
            uPrefs.ShouldFilterUsersWithoutRealPhoto = false;
            var m = new User(model.Id)
            {
                IsBusy = false,
                IsDeleted = false,
                IsBanned = false,
                ShouldConsiderLanguages = false,
                HasPremium = false,
                HadReceivedReward = false,
                IncreasedFamiliarity = true,
                DailyRewardPoint = 0,
                BonusIndex = 1,
                ProfileViewsCount = 0,
                InvitedUsersCount = 0,
                InvitedUsersBonus = 0,
                TagSearchesCount = 0,
                MaxProfileViewsCount = 50,
                IsIdentityConfirmed = false,
                EnteredPromoCodes = model.Promo
            };

            if (model.UserCityCode != null && model.UserCountryCode != null)
                location = new Location { Id = model.Id, CityId = (int)model.UserCityCode, CountryId = (int)model.UserCountryCode, CityCountryClassLocalisationId = model.UserAppLanguageId, CountryClassLocalisationId = model.UserAppLanguageId};
            else
                location = new Location { Id = model.Id };

            uData.LocationId = location.Id;

            var id = await _repository.RegisterUserAsync(m, uBase, uData, uPrefs, location);
            return id;
        }

        [HttpGet("/ReRegisterUser/{userId}")]
        public async Task<long> ReRegisterUser(long userId)
        {
            return await _repository.ReRegisterUser(userId);
        }

        [HttpGet("/GetRecentFeedbacks")]
        public async Task<List<Feedback>> GetRecentFeedbacks()
        {
            return await _repository.GetMostRecentFeedbacks();
        }

        [HttpGet("/GetMostRecentReports")]
        public async Task<List<Report>> GetMostRecentReports()
        {
            return await _repository.GetMostRecentReports();
        }

        [HttpGet("/GetFeedbackById/{id}")]
        public async Task<Feedback> GetFeedbackById(long id)
        {
            return await _repository.GetFeedbackById(id);
        }

        [HttpGet("/GetSingleUserReportById/{id}")]
        public async Task<Report> GetSingleUserReportByIdAsync(long id)
        {
            return await _repository.GetSingleUserReportByIdAsync(id);
        }

        [HttpGet("/GetAllReportsOnUser/{id}")]
        public async Task<List<Report>> GetAllReportsOnUser(long id)
        {
            return await _repository.GetAllReportsOnUserAsync(id);
        }

        [HttpGet("/GetAllUserReports/{id}")]
        public async Task<List<Report>> GetAllUserReports(long id)
        {
            return await _repository.GetAllUserReportsAsync(id);
        }

        [HttpGet("/GetAllUsersIds")]
        public async Task<List<long>> GetAllUsers()
        {
            return await _repository.GetAllUsersAsync();
        }

        [HttpGet("/GetUsersRecentFeedbacks/{userId}")]
        public async Task<List<Feedback>> GetUsersRecentFeedbacks(long userId)
        {
            return await _repository.GetMostRecentFeedbacksByUserId(userId);
        }

        [HttpGet("/AddUserToBlackList/{userId}/{bannedUserId}")]
        public async Task<bool> AddUserToBlackList(long userId, long bannedUserId)
        {
            return await _repository.AddUserToBlackListAsync(userId, bannedUserId);
        }

        [HttpDelete("/RemoveUserFromBlackList/{userId}/{bannedUserId}")]
        public async Task<bool> RemoveUserFromBlackList(long userId, long bannedUserId)
        {
            return await _repository.RemoveUserFromBlackListAsync(userId, bannedUserId);
        }

        [HttpGet("/DeleteUser/{userId}")]
        public async Task<byte> DeleteUser(long userId)
        {
            return await _repository.RemoveUserAsync(userId);
        }

        [HttpGet("/BanUser/{userId}")]
        public async Task<byte> BanUser(long userId)
        {
            return await _repository.BanUserAsync(userId);
        }

        [HttpGet("/UnbanUser/{userId}")]
        public async Task<byte> UnbanUser(long userId)
        {
            return await _repository.UnbanUserAsync(userId);
        }

        [HttpGet("/AddAchievementProgress/{userId}/{achievementId}/{progress}")]
        public async Task<string> AddAchievementProgress(long userId, long achievementId, int progress)
        {
            return await _repository.AddAchievementProgress(userId, achievementId, progress);
        }

        [HttpGet("/GrantAchievementToUser/{userId}/{achievementId}")]
        public async Task<string> GrantAchievementToUser(long userId, long achievementId)
        {
            return await _repository.GrantAchievementToUser(userId, achievementId);
        }

        [HttpGet("/GetUserAchievements/{userId}")]
        public async Task<List<UserAchievement>> GetUserAchievements(long userId)
        {
            return await _repository.GetUserAchievements(userId);
        }

        [HttpGet("/GetUserAchievementsAsAdmin/{userId}")]
        public async Task<List<UserAchievement>> GetUserAchievementsAsAdmin(long userId)
        {
            return await _repository.GetUserAchievementsAsAdmin(userId);
        }

        [HttpGet("/GetSingleUserAchievement/{userId}/{achievementId}")]
        public async Task<UserAchievement> GetSingleUserAchievement(long userId, long achievementId)
        {
            return await _repository.GetSingleUserAchievement(userId, achievementId);
        }

        [HttpGet("/SetUserRtLanguagePrefs/{userId}/{shouldBeConsidered}")]
        public async Task<bool> SetUserRtLanguagePrefs(long userId, bool shouldBeConsidered)
        {
            return await _repository.SetUserRtLanguagePrefs(userId, shouldBeConsidered);
        }

        [HttpGet("/GetActiveUserWalletBalance/{userId}")]
        public async Task<Balance> GetActiveUserWalletBalance(long userId)
        {
            return await _repository.GetUserWalletBalance(userId);
        }

        [HttpGet("/TopUpUserWalletBalance/{userId}/{points}/{description}")]
        public async Task<int> GetActiveUserWalletBalance(long userId, int points, string description)
        {
            return await _repository.TopUpUserWalletPointsBalance(userId, points, description);
        }

        [HttpGet("/TopUpUserWalletPPBalance/{userId}/{points}/{description}")]
        public async Task<int> TopUpUserWalletPPBalance(long userId, int points, string description)
        {
            return await _repository.TopUpUserWalletPPBalance(userId, points, description);
        }

        [HttpGet("/CheckUserHasPremium/{userId}")]
        public async Task<bool> CheckUserHasPremium(long userId)
        {
            return await _repository.CheckUserHasPremiumAsync(userId);
        }

        [HttpGet("/CheckBalanceIsSufficient/{userId}/{cost}")]
        public async Task<bool> CheckBalanceIsSufficient(long userId, int cost)
        {
            return await _repository.CheckBalanceIsSufficient(userId, cost);
        }

        [HttpGet("/GetPremiumExpirationDate/{userId}")]
        public async Task<DateTime> GetPremiumExpirationDate(long userId)
        {
            return await _repository.GetPremiumExpirationDate(userId);
        }

        [HttpGet("/GrantPremiumToUser/{userId}/{cost}/{dayDuration}/{currency}")]
        public async Task<DateTime> GrantPremiumToUser(long userId, int cost, int dayDuration, short currency)
        {
            return await _repository.GrantPremiumToUser(userId, cost, dayDuration, currency);
        }

        [HttpGet("/CheckUserIsBusy/{userId}")]
        public async Task<bool> CheckUserIsBusy(long userId)
        {
            return await _repository.CheckUserIsBusy(userId);
        }

        [HttpGet("/SetDebugProperties")]
        public async Task<bool> SetDebugProperties() // TODO: remove in production
        {
            return await _repository.SetDebugProperties();
        }

        [HttpGet("/SwhitchUserBusyStatus/{userId}")]
        public async Task<bool> SwhitchUserBusyStatus(long userId)
        {
            return await _repository.SwhitchUserBusyStatus(userId);
        }

        [HttpGet("/GetUserRequest/{requestId}")]
        public async Task<UserNotification> GetUserRequest(Guid requestId)
        {
            return await _repository.GetUserRequest(requestId);
        }

        [HttpGet("/GetUserRequests/{userId}")]
        public async Task<List<UserNotification>> GetUserRequests(long userId)
        {
            return await _repository.GetUserRequests(userId);
        }

        [HttpGet("/CheckUserHasRequests/{userId}")]
        public async Task<bool> CheckUserHasRequests(long userId)
        {
            return await _repository.CheckUserHasRequests(userId);
        }

        [HttpPost("/RegisterUserRequest")]
        public async Task<string> RegisterUserRequest(UserNotification request)
        {
            return await _repository.RegisterUserRequest(request);
        }

        [HttpDelete("/DeleteUserRequest/{requestId}")]
        public async Task<byte> DeleteUserRequest(Guid requestId)
        {
            return await _repository.DeleteUserRequest(requestId);
        }

        [HttpDelete("/DeleteUserRequests/{userId}")]
        public async Task<byte> DeleteUserRequests(long userId)
        {
            return await _repository.DeleteUserRequests(userId);
        }

        [HttpPost("/RegisterUserEncounter")]
        public async Task<Guid?> RegisterUserEncounter(Encounter model)
        {
            return await _repository.RegisterUserEncounter(model);
        }

        [HttpGet("/GetUserEncounter/{encounterId}")]
        public async Task<Encounter> GetUserEncounter(long encounterId)
        {
            return await _repository.GetUserEncounter(encounterId);
        }

        [HttpGet("/GetUserEncounters/{userId}/{sectionId}")]
        public async Task<List<Encounter>> GetUserEncounters(long userId, int sectionId)
        {
            return await _repository.GetUserEncounters(userId, sectionId);
        }

        [HttpGet("/GetUserProfileEncounters/{userId}")]
        public async Task<List<Encounter>> GetUserProfileEncounters(long userId)
        {
            var familiatorEncounters = await _repository.GetUserEncounters(userId, (int)Sections.Familiator);
            familiatorEncounters.AddRange(await _repository.GetUserEncounters(userId, (int)Sections.Requester));

            return familiatorEncounters;
        }

        [HttpGet("/GetUserTrustLevel/{userId}")]
        public async Task<UserTrustLevel> GetUserTrustLevel(long userId)
        {
            return await _repository.GetUserTrustLevel(userId);
        }

        [HttpGet("/AddUserTrustProgressAsync/{userId}/{progress}")]
        public async Task<int> GetUserEncounters(long userId, double progress)
        {
            return await _repository.AddUserTrustProgressAsync(userId, progress);
        }

        [HttpGet("/UpdateUserTrustLevelAsync/{userId}/{sectionId}")]
        public async Task<int> UpdateUserTrustLevelAsync(long userId, int level)
        {
            return await _repository.UpdateUserTrustLevelAsync(userId, level);
        }

        [HttpGet("/GetOnlineEventList/{userId}")]
        public async Task<List<Event>> GetOnlineEventList(long userId)
        {
            return await _repository.GetEventList(userId, true);
        }

        [HttpGet("/GetOfflineEventList/{userId}")]
        public async Task<List<Event>> GetOfflineEventList(long userId)
        {
            return await _repository.GetEventList(userId, false);
        }

        [HttpGet("/UpdateUserNickname/{userId}/{nickname}")]
        public async Task<bool> UpdateUserNickname(long userId, string nickname)
        {
            return await _repository.UpdateUserNickname(userId, nickname);
        }

        [HttpGet("/GetUserNickname/{userId}")]
        public async Task<string> GetUserNickname(long userId)
        {
            return await _repository.GetUserNickname(userId);
        }

        [HttpGet("/CheckUserCanClaimReward/{userId}")]
        public async Task<bool> CheckUserCanClaimReward(long userId)
        {
            return await _repository.CheckUserCanClaimReward(userId);
        }

        [HttpGet("/ClaimDailyReward/{userId}")]
        public async Task<string> ClaimDailyReward(long userId)
        {
            return await _repository.ClaimDailyReward(userId);
        }

        [HttpGet("/GenerateInvitationCredentials/{userId}")]
        public async Task<InvitationCredentials> GenerateInvitationCredentials(long userId)
        {
            return await _repository.GenerateInvitationCredentialsAsync(userId);
        }

        [HttpGet("/GetInvitationCredentialsByUserId/{userId}")]
        public async Task<InvitationCredentials> GetInvitationCredentialsByUserId(long userId)
        {
            return await _repository.GetInvitationCredentialsByUserIdAsync(userId);
        }

        [HttpGet("/GetInvitationLink/{userId}")]
        public async Task<string> GetUserInvitationLinkAsync(long userId)
        {
            return await _repository.GetUserInvitationLinkAsync(userId);
        }

        [HttpGet("/GetQRCode/{userId}")]
        public async Task<string> GetQRCode(long userId)
        {
            return await _repository.GetQRCode(userId);
        }

        [HttpGet("/InviteUser/{invitationId}/{userId}")]
        public async Task<bool> InviteUser(Guid invitationId, long userId)
        {
            return await _repository.InviteUserAsync(invitationId, userId);
        }

        [HttpGet("/CheckUserHasNotifications/{userId}")]
        public async Task<bool> CheckUserHasNotifications(long userId)
        {
            return await _repository.CheckUserHasNotificationsAsync(userId);
        }

        [HttpGet("/GetUserNotifications/{userId}")]
        public async Task<List<UserNotification>> GetUserNotifications(long userId)
        {
            return await _repository.GetUserNotifications(userId);
        }

        [HttpGet("/DeleteUserNotification/{userId}/{notificationId}")]
        public async Task<bool> DeleteUserNotification(Guid notificationId)
        {
            return await _repository.DeleteUserNotification(notificationId);
        }

        [HttpGet("/GetRandomAchievements/{userId}")]
        public async Task<List<UserAchievement>> GetRandomAchievements(long userId)
        {
            return await _repository.GetRandomAchievements(userId);
        }

        [HttpGet("/CalculateSimilarity/{param1}/{param2}")] //Remove in production. This method is internal
        public async Task<double> CalculateSimilarity(double param1, double param2)
        {
            return await _repository.CalculateSimilarityAsync(param1, param2);
        }

        [HttpGet("/GetUserDailyTaskById/{userId}/{taskId}")]
        public async Task<UserDailyTask> GetUserDailyTaskById(long userId, long taskId)
        {
            return await _repository.GetUserDailyTaskByIdAsync(userId, taskId);
        }

        [HttpGet("/GetDailyTaskById/{taskId}")]
        public async Task<DailyTask> GetDailyTaskById(long taskId)
        {
            return await _repository.GetDailyTaskByIdAsync(taskId);
        }

        [HttpGet("/UpdateUserDailyTaskProgress/{userId}/{taskId}/{progress}")]
        public async Task<int> UpdateUserDailyTaskProgress(long userId, long taskId, int progress)
        {
            return await _repository.UpdateUserDailyTaskProgressAsync(userId, taskId, progress);
        }

        [HttpGet("/GiveDailyTaskRewardToUser/{userId}/{taskId}")]
        public async Task<int> GiveDailyTaskRewardToUser(long userId, long taskId)
        {
            return await _repository.GiveDailyTaskRewardToUserAsync(userId, taskId);
        }

        [HttpGet("/CheckUserHasTasksInSection/{userId}/{sectionId}")]
        public async Task<bool> CheckUserHasTasksInSection(long userId, int sectionId)
        {
            return await _repository.CheckUserHasTasksInSectionAsync(userId, sectionId);
        }

        [HttpGet("/GenerateUserDailyTaskList/{userId}")]
        public async Task<byte> GenerateUserDailyTaskList(long userId)
        {
            return await _repository.GenerateUserDailyTaskListAsync(userId);
        }

        [HttpGet("/ShowDailyTaskProgress/{userId}/{taskId}")]
        public async Task<string> ShowDailyTaskProgress(long userId, long taskId)
        {
            return await _repository.ShowDailyTaskProgressAsync(userId, taskId);
        }

        [HttpGet("/GetUserMaximumLanguageCount/{userId}")]
        public async Task<int> GetUserMaximumLanguageCount(long userId)
        {
            return await _repository.GetUserMaximumLanguageCountAsync(userId);
        }

        [HttpGet("/GetUserNotificationsIds/{userId}")]
        public async Task<List<Guid>> GetUserNotificationsIds(long userId)
        {
            return await _repository.GetUserNotificationsIdsAsync(userId);
        }

        [HttpGet("/GetUserNotification/{notificationId}")]
        public async Task<UserNotification> GetUserNotification(Guid notificationId)
        {
            return await _repository.GetUserNotificationAsync(notificationId);
        }

        [HttpGet("/SendNotificationConfirmationCode/{notificationId}")]
        public async Task<byte> SendNotificationConfirmationCode(Guid notificationId)
        {
            return await _repository.SendNotificationConfirmationCodeAsync(notificationId);
        }

        [HttpGet("/GetUserPersonalityPointsAmount/{userId}")]
        public async Task<int> GetUserPersonalityPointsAmount(long userId)
        {
            return await _repository.GetUserPersonalityPointsAmount(userId);
        }

        [HttpPost("/UpdateUserPersonalityStats")]
        public async Task<bool> UpdateUserPersonalityStats(TestPayload model)
        {
            return await _repository.UpdateUserPersonalityStats(model);
        }

        [HttpPost("/UpdateUserPersonalityPoints")]
        public async Task<bool> UpdateUserPersonalityPoints(PointsPayload model)
        {
            return await _repository.UpdateUserPersonalityPoints(model);
        }

        [HttpGet("/GetUserPersonalityPoints/{userId}")]
        public async Task<UserPersonalityPoints> GetUserPersonalityPoints(long userId)
        {
            return await _repository.GetUserPersonalityPoints(userId);
        }

        [HttpGet("/GetUserPersonalityStats/{userId}")]
        public async Task<UserPersonalityStats> GetUserPersonalityStats(long userId)
        {
            return await _repository.GetUserPersonalityStats(userId);
        }

        [HttpGet("/SwitchPersonalityUsage/{userId}")]
        public async Task<bool> SwitchPersonalityUsage(long userId)
        {
            return await _repository.SwitchPersonalityUsage(userId);
        }

        [HttpPost("/UpdateTags")]
        public async Task<bool> UpdateTags(UpdateTags model)
        {
            return await _repository.UpdateTags(model);
        }

        [HttpGet("/GetTags/{userId}")]
        public async Task<List<string>> UpdateTags(long userId)
        {
            return await _repository.GetTags(userId);
        }

        [HttpPost("/GetUserByTags")]
        public async Task<User> GetUserByTags(GetUserByTags model)
        {
            return await _repository.GetUserListByTagsAsync(model);
        }

        [HttpGet("/GetMaxTagCount/{userId}")]
        public async Task<int> GetMaxTagCount(long userId)
        {
            if (await _repository.CheckUserHasPremiumAsync(userId))
            {
                return 50;
            }
            return 25;
        }

        [HttpGet("/CheckUserUsesPersonality/{userId}")]
        public async Task<bool?> CheckUserUsesPersonality(long userId)
        {
            return await _repository.CheckUserUsesPersonality(userId);
        }

        [HttpGet("/GetBlackList/{userId}")]
        public async Task<List<BlackList>> GetBlackList(long userId)
        {
            return await _repository.GetBlackList(userId);
        }

        [HttpGet("/CheckEncounteredUserIsInBlackList/{userId}/{encounteredUser}")]
        public async Task<bool> CheckEncounteredUserIsInBlackList(long userId, long encounteredUser)
        {
            return await _repository.CheckEncounteredUserIsInBlackList(userId, encounteredUser);
        }

        [HttpGet("/RetreiveCommonLanguages/{user1Id}/{user2Id}/{localisationId}")]
        public async Task<string> RetreiveCommonLanguages(long user1Id, long user2Id, int localisationId)
        {
            return await _repository.RetreiveCommonLanguagesAsync(user1Id, user2Id, localisationId);
        }

        [HttpGet("/SetAutoReplyText/{userId}/{text}")]
        public async Task<bool> SetAutoReplyText(long userId, string text)
        {
            return await _repository.SetAutoReplyTextAsync(userId, text);
        }

        [HttpGet("/SetAutoReplyVoice/{userId}/{voice}")]
        public async Task<bool> SetAutoReplyVoice(long userId, string voice)
        {
            return await _repository.SetAutoReplyVoiceAsync(userId, voice);
        }

        [HttpGet("/GetActiveAutoReply/{userId}")]
        public async Task<ActiveAutoReply> GetActiveAutoReply(long userId)
        {
            return await _repository.GetActiveAutoReplyAsync(userId);
        }

        [HttpPost("/SendTickRequest")]
        public async Task<bool> SendTickRequest(SendTickRequest request)
        {
            return await _repository.SendTickRequestAsync(request);
        }

        [HttpGet("/SwitchUserFilteringByPhoto/{userId}")]
        public async Task<bool> SwitchUserFilteringByPhoto(long userId)
        {
            return await _repository.SwitchUserFilteringByPhotoAsync(userId);
        }

        [HttpGet("/GetUserFilteringByPhotoStatus/{userId}")]
        public async Task<bool> GetUserFilteringByPhotoStatus(long userId)
        {
            return await _repository.GetUserFilteringByPhotoStatusAsync(userId);
        }

        [HttpGet("/GetTestDataByProperty/{userId}/{param}")]
        public async Task<List<GetTestShortData>> GetTestDataByProperty(long userId, short param)
        {
            return await _repository.GetTestDataByPropertyAsync(userId, param);
        }

        [HttpGet("/GetUserTestDataByProperty/{userId}/{param}")]
        public async Task<List<GetTestShortData>> GetUserTestDataByProperty(long userId, short param)
        {
            return await _repository.GetUserTestDataByPropertyAsync(userId, param);
        }

        [HttpGet("/GetTestFullDataById/{testId}/{localisation}")]
        public async Task<GetFullTestData> GetTestFullDataById(long testId, int localisation)
        {
            return await _repository.GetTestFullDataByIdAsync(testId, localisation);
        }

        [HttpGet("/GetUserTest/{userId}/{testId}")]
        public async Task<GetUserTest> GetUserTest(long userId, long testId)
        {
            return await _repository.GetUserTestAsync(userId, testId);
        }

        [HttpGet("/GetPossibleTestPassRange/{userId}/{testId}")]
        public async Task<int> GetPossibleTestPassRange(long userId, long testId)
        {
            return await _repository.GetPossibleTestPassRangeAsync(userId, testId);
        }

        [HttpGet("/PurchaseTest/{userId}/{testId}/{localisation}")]
        public async Task<bool> PurchaseTest(long userId, long testId, int localisation)
        {
            return await _repository.PurchaseTestAsync(userId, testId, localisation);
        }

        [HttpGet("/CheckTickRequestStatus/{userId}")]
        public async Task<string> CheckTickRequestStatus(long userId)
        {
            return await _repository.CheckTickRequestStatusÀsync(userId);
        }

        [HttpGet("/SetUserFreeSearchParam/{userId}/{freeSearch}")]
        public async Task<bool> SetUserFreeSearchParam(long userId, bool freeSearch)
        {
            return await _repository.SetUserFreeSearchParamAsync(userId, freeSearch);
        }

        [HttpGet("/SwitchUserFreeSearchParam/{userId}")]
        public async Task<bool> SwitchUserFreeSearchParam(long userId)
        {
            return await _repository.SwitchUserFreeSearchParamAsync(userId);
        }

        [HttpGet("/CheckUserHaveChosenFreeParam/{userId}")]
        public async Task<bool> CheckUserHaveChosenFreeParam(long userId)
        {
            return await _repository.CheckUserHaveChosenFreeParamAsync(userId);
        }

        [HttpGet("/CheckShouldTurnOffPersonality/{userId}")]
        public async Task<bool> CheckShouldTurnOffPersonality(long userId)
        {
            return await _repository.CheckShouldTurnOffPersonalityAsync(userId);
        }

        [HttpGet("/GetUserPersonalityCaps/{userId}")]
        public async Task<PersonalityCaps> GetUserPersonalityCaps(long userId)
        {
            return await _repository.GetUserPersonalityCapsAsync(userId);
        }

        [HttpGet("/GetUserActiveEffects/{userId}")]
        public async Task<List<ActiveEffect>> GetUserActiveEffects(long userId)
        {
            return await _repository.GetUserActiveEffects(userId);
        }

        [HttpGet("/CheckUserHasEffect/{userId}/{effectId}")]
        public async Task<bool> CheckUserHasEffect(long userId, int effectId)
        {
            return await _repository.CheckUserHasEffectAsync(userId, effectId);
        }

        [HttpGet("/ActivateDurableEffect/{userId}/{effectId}")]
        public async Task<DateTime?> ActivateDurableEffect(long userId, int effectId)
        {
            return await _repository.ActivateDurableEffectAsync(userId, effectId);
        }

        [HttpGet("/ActivateToggleEffect/{userId}/{effectId}/{user2Id?}")]
        public async Task<bool> ActivateToggleEffect(long userId, int effectId, long? user2Id = null)
        {
            return await _repository.ActivateToggleEffectAsync(userId, effectId, user2Id);
        }

        [HttpGet("/CheckEffectIsActive/{userId}/{effectId}")]
        public async Task<bool> CheckEffectIsActive(long userId, int effectId)
        {
            return await _repository.CheckEffectIsActiveAsync(userId, effectId);
        }

        [HttpGet("/SwitchUserRTLanguageConsideration/{userId}")]
        public async Task<bool> SwitchUserRTLanguageConsideration(long userId)
        {
            return await _repository.SwitchUserRTLanguageConsiderationAsync(userId);
        }

        [HttpGet("/CheckEffectIsActive/{userId}")]
        public async Task<bool> GetUserRTLanguageConsideration(long userId)
        {
            return await _repository.GetUserRTLanguageConsiderationAsync(userId);
        }

        [HttpGet("/PurchaseEffect/{userId}/{effectId}/{points}/{currency}/{count?}")]
        public async Task<bool> PurchaseEffect(long userId, int effectId, int points, short currency, short count=1)
        {
            return await _repository.PurchaseEffectAsync(userId, effectId, points, currency, count);
        }

        [HttpGet("/SetUserCurrency/{userId}/{currency}")]
        public async Task<ActionResult> SetUserCurrency(long userId, short currency)
        {
            await _repository.SetUserCurrencyAsync(userId, currency);

            if (ModelState.IsValid)
                return Ok();

            return BadRequest();
        }

        [HttpGet("/GetRequestSender/{requestId}")]
        public async Task<ActionResult<GetUserData>> GetRequestSender(Guid requestId)
        {
            var sender = await _repository.GetRequestSenderAsync(requestId);

            if (ModelState.IsValid)
                return Ok(sender);

            return BadRequest();
        }

        [HttpGet("/PurchesPPForPoints/{userId}/{price}/{count?}")]
        public async Task<bool> PurchesPPForPoints(long userId, int price, short count=1)
        {
            return await _repository.PurchasePersonalityPointsAsync(userId, price, (short)Currencies.Points, count);
        }

        [HttpGet("/PurchesPPForRealMoney/{userId}/{price}/{count?}")]
        public async Task<bool> PurchesPPForRealMoney(long userId, int price, short count = 1)
        {
            return await _repository.PurchasePersonalityPointsAsync(userId, price, (short)Currencies.RealMoney, count);
        }

        [HttpGet("/CheckPromoIsCorrect/{userId}/{promo}/{isActivatedBeforeRegistration}")]
        public async Task<bool> CheckPromoIsCorrect(long userId, string promo, bool isActivatedBeforeRegistration)
        {
            return await _repository.CheckPromoIsCorrectAsync(userId, promo, isActivatedBeforeRegistration);
        }

        [HttpGet("/GetUserIncreasedFamiliarity/{userId}")]
        public async Task<bool> GetUserIncreasedFamiliarity(long userId)
        {
            return await _repository.GetUserIncreasedFamiliarityAsync(userId);
        }

        [HttpGet("/SwitchIncreasedFamiliarity/{userId}")]
        public async Task<bool> SwitchIncreasedFamiliarity(long userId)
        {
            return await _repository.SwitchIncreasedFamiliarityAsync(userId);
        }

        //Adventures
        [HttpPost("/RegisterAdventure")]
        public async Task<Guid> RegisterAdventure(Adventure model)
        {
            return await _repository.RegisterAdventureAsync(model);
        }

        [HttpPost("/ChangeAdventure")]
        public async Task<bool> ChangeAdventure(ChangeAdventure model)
        {
            return await _repository.ChangeAdventureAsync(model);
        }

        [HttpDelete("/DeleteAdventure/{id}/{userId}")]
        public async Task<bool> DeleteAdventure(Guid id, long userId)
        {
            return await _repository.DeleteAdventureAsync(id, userId);
        }

        [HttpGet("/SubscribeOnAdventure/{id}/{userId}")]
        public async Task<bool> SubscribeOnAdventure(Guid id, long userId)
        {
            return await _repository.SubscribeOnAdventureAsync(id, userId);
        }

        [HttpGet("/ProcessSubscriptionRequest/{id}/{userId}/{status}")]
        public async Task<bool> ProcessSubscriptionRequest(Guid id, long userId, AdventureRequestStatus status)
        {
            return await _repository.ProcessSubscriptionRequestAsync(id, userId, status);
        }

        [HttpGet("/GetAdventureAttendees/{id}")]
        public async Task<List<AttendeeInfo>> GetAdventureAttendees(Guid id)
        {
            return await _repository.GetAdventureAttendeesAsync(id);
        }
    }
}
