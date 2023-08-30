using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Entities.UserInfoEntities;
using WebApi.Entities.ReportEntities;
using WebApi.Entities.LocationEntities;
using WebApi.Entities.AchievementEntities;
using WebApi.Entities.UserActionEntities;
using WebApi.Entities.TestEntities;
using WebApi.Entities.EffectEntities;
using WebApi.Entities.AdventureEntities;
using WebApi.Enums;
using WebApi.Entities;
using OceanStats = WebApi.Enums.OceanStats;

namespace WebApi.Controllers
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
        public async Task<bool> CheckUserHasVisitedSection(long userId, Section section)
        {
            return await _repository.CheckUserHasVisitedSection(userId, section);
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
        public async Task<string> GetUserLanguagePrefs(long userId)
        {
            return await _repository.GetUserAppLanguage(userId);
        }

        [HttpGet("/UserInfo/{userId}")]
        public async Task<ActionResult<User>> GetUserInfo(long userId)
        {
            return Ok(await _repository.GetUserInfoAsync(userId));
        }

        [HttpPost("/UpdateUserProfile")]
        public async Task UpdateUserProfile(UpdateUserProfile model)
        {
            if (model.WasChanged)
                await _repository.UpdateUserAsync(model);
        }

        [HttpGet("/UpdateUserAppLanguage/{userId}/{appLanguage}")]
        public async Task<ActionResult<byte>> UpdateUserAppLanguage(long userId, AppLanguage appLanguage)
        {
            return await _repository.UpdateUserAppLanguageAsync(userId, appLanguage);
        }

        [HttpGet("/GetUserByUsername/{username}")]
        public async Task<ActionResult<User>> GetUserInfo(string username)
        {
            return await _repository.GetUserInfoByUsrnameAsync(username);
        }

        [HttpGet("/GetUserList")]
        public async Task<SearchResponse> GetUserList([FromQuery] long userId)
        {
            return await _repository.GetUsersAsync(userId);
        }

        [HttpGet("/GetUserList/FreeSearch/{userId}")]
        public async Task<SearchResponse> GetUserList3(long userId)
        {
            return await _repository.GetUsersAsync(userId, isFreeSearch: true);
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

        [HttpGet("/GetReportReasons/{localisationId}")]
        public List<string> GetReportReasons(int localisationId)
        {
            var reasons = new List<string>();

            reasons.Add(ReportReason.Spam.ToString());
            return reasons;
        }

        [HttpPost("/feedback")]
        public async Task<long> AddFeedback([FromBody] AddFeedback request)
        {
            return await _repository.AddFeedbackAsync(request);
        }

        [HttpPost("/report-user")]
        public async Task<long> AddUserReport([FromBody] SendUserReport report)
        {
            return await _repository.AddUserReportAsync(report);
        }

        [HttpPost("/report-adventure")]
        public async Task<long> ReportAdventure([FromBody] SendAdventureReport report)
        {
            return await _repository.AddAdventureReportAsync(report);
        }

        [HttpGet("/basic-info/{userId}")]
        public async Task<BasicUserInfo> AddUserReport([FromRoute] long userId)
        {
            return await _repository.GetUserBasicInfo(userId);
        }

        [HttpPost("/RegisterUser")]
        public async Task<long> AddUser(UserRegistrationModel model)
        {

            return await _repository.RegisterUserAsync(model);
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

        [HttpGet("/user-balance/{userId}")]
        public async Task<Balance> GetActiveUserWalletBalance(long userId)
        {
            return await _repository.GetUserWalletBalance(userId);
        }

        [HttpGet("/TopUpUserWalletBalance/{userId}/{points}/{description}")]
        public async Task<float> TopUpUserWalletBalance(long userId, int points, string description)
        {
            return await _repository.TopUpPointBalance(userId, points, description);
        }

        [HttpGet("/TopUpUserWalletPPBalance/{userId}/{points}/{description}")]
        public async Task<int> TopUpUserWalletPPBalance(long userId, int points, string description)
        {
            return await _repository.TopUpOPBalance(userId, points, description);
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
        public async Task<DateTime> GrantPremiumToUser(long userId, float cost, int dayDuration, Currency currency)
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

        [HttpGet("/SwhitchUserBusyStatus/{userId}/{section}")]
        public async Task<SwitchBusyStatusResponse> SwhitchUserBusyStatus([FromRoute] long userId, [FromRoute]Section section)
        {
            return await _repository.SwhitchUserBusyStatus(userId, section);
        }

        [HttpGet("/GetUserRequest/{requestId}")]
        public async Task<UserNotification> GetUserRequest(long requestId)
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
        public async Task<string> RegisterUserRequest(AddNotification request)
        {
            return await _repository.RegisterUserRequestAsync(request);
        }

        [HttpGet("/DeclineRequest/{user1}/{user2}")]
        public async Task<string> DeclineRequest(long user1, long user2)
        {
            return await _repository.DeclineRequestAsync(user1, user2);
        }

        [HttpDelete("/DeleteUserRequest/{requestId}")]
        public async Task<byte> DeleteUserRequest(long requestId)
        {
            return await _repository.DeleteUserRequest(requestId);
        }

        [HttpDelete("/DeleteUserRequests/{userId}")]
        public async Task<byte> DeleteUserRequests(long userId)
        {
            return await _repository.DeleteUserRequests(userId);
        }

        [HttpPost("/RegisterUserEncounter")]
        public async Task RegisterUserEncounter(RegisterEncounter model)
        {
            await _repository.RegisterUserEncounter(model);
        }

        [HttpGet("/profile-encounters/{userId}")]
        public async Task<List<Encounter>> GetUserProfileEncounters([FromRoute] long userId)
        {
            var familiatorEncounters = await _repository.GetUserEncounters(userId, Section.Familiator);
            familiatorEncounters.AddRange(await _repository.GetUserEncounters(userId, Section.Requester));

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

        [HttpGet("/GetRandomAchievements/{userId}")]
        public async Task<List<string>> GetRandomAchievements(long userId)
        {
            return await _repository.GetRandomAchievements(userId);
        }

        [HttpGet("/CalculateSimilarity/{param1}/{param2}")] //Remove in production. This method is internal
        public async Task<double> CalculateSimilarity(double param1, double param2)
        {
            return await _repository.CalculateSimilarityAsync(param1, param2);
        }

        [HttpGet("/GetUserMaximumLanguageCount/{userId}")]
        public async Task<int> GetUserMaximumLanguageCount(long userId)
        {
            return await _repository.GetUserMaximumLanguageCountAsync(userId);
        }

        [HttpGet("/GetUserNotificationsIds/{userId}")]
        public async Task<List<long>> GetUserNotificationsIds(long userId)
        {
            return await _repository.GetUserNotificationsIdsAsync(userId);
        }

        [HttpGet("/GetUserNotification/{notificationId}")]
        public async Task<UserNotification> GetUserNotification(long notificationId)
        {
            return await _repository.GetUserNotificationAsync(notificationId);
        }

        [HttpGet("/delete-notification/{notificationId}")]
        public async Task<byte> DeleteNotification(long notificationId)
        {
            return await _repository.DeleteNotificationAsync(notificationId);
        }

        [HttpGet("/ocean-points-amount/{userId}")]
        public async Task<int> GetUserPersonalityPointsAmount(long userId)
        {
            return await _repository.GetUserPersonalityPointsAmount(userId);
        }

        [HttpPost("/update-ocean-stats")]
        public async Task<bool> UpdateUserPersonalityStats(TestPayload model)
        {
            return await _repository.UpdateOceanStatsAsync(model);
        }

        [HttpPost("/update-ocean-points")]
        public async Task<bool> UpdateUserPersonalityPoints(PointsPayload model)
        {
            return await _repository.UpdateUserPersonalityPoints(model);
        }

        [HttpGet("/ocean-points/{userId}")]
        public async Task<OceanPoints> GetUserPersonalityPoints(long userId)
        {
            return await _repository.GetUserPersonalityPoints(userId);
        }

        [HttpGet("/switch-ocean-usage/{userId}")]
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
        public async Task<List<UserTag>> GetTags(long userId)
        {
            return await _repository.GetTags(userId);
        }

        [HttpPost("/GetUserByTags")]
        public async Task<SearchResponse> GetUserByTags(GetUserByTags model)
        {
            return await _repository.GetUserByTagsAsync(model);
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
        public async Task<IActionResult> SetAutoReplyText(long userId, string text)
        {
            await _repository.SetAutoReplyTextAsync(userId, text);
            return Ok();
        }

        [HttpGet("/SetAutoReplyVoice/{userId}/{voice}")]
        public async Task<IActionResult> SetAutoReplyVoice(long userId, string voice)
        {
            await _repository.SetAutoReplyVoiceAsync(userId, voice);
            return Ok();
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

        [HttpGet("/test-data-by-param/{userId}/{param}")]
        public async Task<List<GetTestShortData>> GetTestDataByProperty(long userId, OceanStats param)
        {
            return await _repository.GetTestDataByPropertyAsync(userId, param);
        }

        [HttpGet("/test-data-by-prop/{userId}/{param}")]
        public async Task<List<GetTestShortData>> GetUserTestDataByProperty(long userId, OceanStats param)
        {
            return await _repository.GetUserTestDataByPropertyAsync(userId, param);
        }

        [HttpGet("/test-data-by-id/{testId}/{localisation}")]
        public async Task<GetFullTestData> GetTestFullDataById(long testId, AppLanguage localisation)
        {
            return await _repository.GetTestFullDataByIdAsync(testId, localisation);
        }

        [HttpGet("/user-test/{userId}/{testId}")]
        public async Task<GetUserTest> GetUserTest(long userId, long testId)
        {
            return await _repository.GetUserTestAsync(userId, testId);
        }

        [HttpGet("/test-pass-range/{userId}/{testId}")]
        public async Task<int> GetPossibleTestPassRange(long userId, long testId)
        {
            return await _repository.GetPossibleTestPassRangeAsync(userId, testId);
        }

        [HttpGet("/purchase-test/{userId}/{testId}/{localisation}")]
        public async Task<bool> PurchaseTest(long userId, long testId, AppLanguage localisation)
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

        [HttpGet("/ocean-caps/{userId}")]
        public async Task<OceanCaps> GetUserPersonalityCaps(long userId)
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
        public async Task<DateTime?> ActivateDurableEffect(long userId, Currency effectId)
        {
            return await _repository.ActivateDurableEffectAsync(userId, effectId);
        }

        [HttpGet("/ActivateToggleEffect/{userId}/{effectId}/{user2Id?}")]
        public async Task<bool> ActivateToggleEffect(long userId, int effectId, long? user2Id = null)
        {
            return await _repository.ActivateToggleEffectAsync(userId, effectId, user2Id);
        }

        [HttpGet("/CheckEffectIsActive/{userId}/{effectId}")]
        public async Task<bool> CheckEffectIsActive(long userId, Currency effectId)
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
        public async Task<bool> PurchaseEffect(long userId, int effectId, float points, Currency currency, short count=1)
        {
            return await _repository.PurchaseEffectAsync(userId, effectId, points, currency, count);
        }

        [HttpGet("/PurchasePoints")]
        public async Task<IActionResult> PurchasePoints([FromQuery] long userId, [FromQuery]int cost, 
            [FromQuery] Currency currency, [FromQuery] int amount)
        {
            await _repository.PurchasePointsAsync(userId, cost, currency, amount);
            return Ok();
        }

        [HttpGet("/SetUserCurrency/{userId}/{currency}")]
        public async Task<ActionResult> SetUserCurrency(long userId, Currency currency)
        {
            await _repository.SetUserCurrencyAsync(userId, currency);

            return Ok();
        }

        [HttpGet("/GetRequestSender/{requestId}")]
        public async Task<ActionResult<GetUserData>> GetRequestSender(long requestId)
        {
            var sender = await _repository.GetRequestSenderAsync(requestId);

            if (ModelState.IsValid)
                return Ok(sender);

            return BadRequest();
        }

        [HttpGet("/PurchesPPForPoints/{userId}/{price}/{count?}")]
        public async Task<bool> PurchesPPForPoints(long userId, int price, short count=1)
        {
            return await _repository.PurchasePersonalityPointsAsync(userId, price, Currency.Points, count);
        }

        [HttpGet("/PurchesPPForRealMoney/{userId}/{price}/{currency}/{count?}")]
        public async Task<bool> PurchesPPForRealMoney(long userId, int price, Currency currency, short count = 1)
        {
            return await _repository.PurchasePersonalityPointsAsync(userId, price, currency, count);
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
        public async Task<string> RegisterAdventure([FromBody] ManageAdventure model)
        {
            return await _repository.RegisterAdventureAsync(model);
        }

        [HttpPost("/ChangeAdventure")]
        public async Task ChangeAdventure(ManageAdventure model)
        {
            await _repository.ChangeAdventureAsync(model);
        }

        [HttpPost("/adventure-group-id")]
        public async Task<SetGroupIdResult> AdventureGroupId([FromBody] SetGroupIdRequest request)
        {
            return await _repository.SetAdventureGroupIdAsync(request);
        }

        [HttpGet("/adventure-templates/{userId}")]
        public async Task<List<GetTemplateShort>> AdventureTenmplates([FromRoute]long userId)
        {
            return await _repository.GetAdventureTemplatesAsync(userId);
        }

        [HttpGet("/adventure-template/{id}")]
        public async Task<ManageTemplate> AdventureTenmplate([FromRoute] long id)
        {
            return await _repository.GetAdventureTemplateAsync(id);
        }

        [HttpPost("/SendAdventureRequestByCode")]
        public async Task<ParticipationRequestStatus> SendAdventureRequestByCode([FromBody] ParticipationRequest model)
        {
            return await _repository.SendAdventureRequestByCodeAsync(model);
        }

        [HttpDelete("/adventure")]
        public async Task<bool> DeleteAdventure([FromQuery] long id, [FromQuery] long userId)
        {
            return await _repository.DeleteAdventureAsync(id, userId);
        }

        [HttpGet("/adventure-request")]
        public async Task<ParticipationRequestStatus> SendAdventureRequest([FromQuery] long id, [FromQuery] long userId)
        {
            return await _repository.SendAdventureRequestAsync(id, userId);
        }

        //TODO: perhaps change return type to enum
        [HttpGet("/process-adventure-request/{id}/{userId}/{status}")]
        public async Task<bool> ProcessSubscriptionRequest(long id, long userId, AdventureAttendeeStatus status)
        {
            return await _repository.ProcessSubscriptionRequestAsync(id, userId, status);
        }

        [HttpGet("/adventure-attendees/{id}")]
        public async Task<List<AttendeeInfo>> GetAdventureAttendees(long id)
        {
            return await _repository.GetAdventureAttendeesAsync(id);
        }

        [HttpGet("/user-adventures")]
        public async Task<List<GetAdventure>> GetUsersAdventures([FromQuery] long userId)
        {
            return await _repository.GetUserAdventuresAsync(userId);
        }

        [HttpGet("/adventure/{id}")]
        public async Task<ManageAdventure> GetAdventure(long id)
        {
            return await _repository.GetAdventureAsync(id);
        }

        [HttpGet("/GetUsersSubscribedAdventures/{userId}")]
        public async Task<List<Adventure>> GetUsersSubscribedAdventures(long userId)
        {
            return await _repository.GetUsersSubscribedAdventuresAsync(userId);
        }

        [HttpGet("/GetSimilarityBetweenUsers/{userId1}/{userId2}")]
        public async Task<SimilarityBetweenUsers> GetSimilarityBetweenUsers(long userId1, long userId2)
        {
            return await _repository.GetSimilarityBetweenUsersAsync(userId1, userId2);
        }

        [HttpPost("/SaveTemplate")]
        public async Task<bool> SaveTemplate(ManageTemplate model)
        {
            return await _repository.SaveAdventureTemplateAsync(model);
        }

        [HttpDelete("/delete-template/{templateId}")]
        public async Task<DeleteResult> DeleteTemplate([FromRoute]long templateId, [FromServices] IUserRepository userRepo)
        {
            return await userRepo.DeleteAdventureTemplateAsync(templateId);
        }

        [HttpDelete("/delete-attendee/{adventureId}/{attendeeId}")]
        public async Task<DeleteResult> DeleteAttendee([FromRoute] long adventureId, [FromRoute] long attendeeId, [FromServices] IUserRepository userRepo)
        {
            return await userRepo.DeleteAdventureAttendeeAsync(adventureId, attendeeId);
        }

        [HttpGet("/get-user-media/{userId}")]
        public async Task<GetUserMedia> UserMedia([FromRoute] long userId, [FromServices] IUserRepository userRepo)
        {
            return await userRepo.GetUserMediaAsync(userId);
        }

        [HttpGet("/limitations/{userId}")]
        public async Task<GetLimitations> Limitations(long userId, [FromServices] IUserRepository userRepo)
        {
            return await userRepo.GetUserSearchLimitations(userId);
        }

        [HttpGet("/set-comment-status/{userId}")]
        public async Task SetCommentStatus([FromRoute]long userId, [FromServices] IUserRepository userRepo)
        {
            await userRepo.SwitchSearchCommentsVisibilityAsync(userId);
        }

        [HttpGet("/set-hint-status/{userId}")]
        public async Task SetHintStatus([FromRoute] long userId, [FromServices] IUserRepository userRepo)
        {
            await userRepo.SwitchHintsVisibilityAsync(userId);
        }

        [HttpGet("/user-partial-data/{userId}")]
        public async Task<UserPartialData> GetUserPartialData([FromRoute] long userId, [FromServices] IUserRepository userRepo)
        {
            return await userRepo.GetUserPartialData(userId);
        }

        [HttpGet("/report-reasons")]
        public List<GetLocalizedEnum> ReportReasons([FromServices] IUserRepository userRepo)
        {
            return userRepo.GetReportReasonsAsync();
        }

        [HttpGet("/payment-currencies")]
        public List<GetLocalizedEnum> PaymentCurrencies([FromServices] IUserRepository userRepo)
        {
            return userRepo.GetPaymentCurrencies();
        }

        [HttpGet("/genders")]
        public List<GetLocalizedEnum> GetGenders()
        {
            return _repository.GetGenders();
        }

        [HttpPost("/delete-user")]
        public async Task<DeleteResult> DeleteUser([FromBody] DeleteUserRequest request)
        {
            return await _repository.DeleteUserAsync(request);
        }

        [HttpGet("/restore-user")]
        public async Task<RestoreResult> RestoreUser([FromQuery] long userId)
        {
            return await _repository.RestoreUserAsync(userId);
        }

        [HttpGet("/get-adventures")]
        public async Task<AdventureSearchResponse> GetAdventures([FromQuery] long userId)
        {
            return await _repository.GetAdventuresAsync(userId);
        }
    }
}
