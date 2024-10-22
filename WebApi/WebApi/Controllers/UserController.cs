﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Main.Entities.User;
using WebApi.Main.Entities.Location;
using WebApi.Main.Entities.Report;
using WebApi.Main.Entities.Achievement;
using RegisterEncounter = WebApi.Main.Entities.User.RegisterEncounter;
using WebApi.Models.Models.Test;
using WebApi.Models.Models.User;
using WebApi.Models.Models.Adventure;
using WebApi.Models.User;
using WebApi.Models.Models.Achievement;
using WebApi.Enums.Enums.Responses;
using WebApi.Enums.Enums.Adventure;
using WebApi.Enums.Enums.General;
using models = WebApi.Models.Models;
using WebApi.Enums.Enums.User;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private IUserRepository _repository;

        public UserController(ILogger<UserController> logger, IUserRepository rep)
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

        [HttpGet("/user-info/{userId}")]
        public async Task<ActionResult<UserInfo>> GetUserInfo(long userId)
        {
            return Ok(await _repository.GetUserInfoAsync(userId));
        }

        [HttpGet("/user-settings/{userId}")]
        public async Task<ActionResult<GetUserSettings>> GetUserSettings(long userId)
        {
            return Ok(await _repository.GetUserSettingsAsync(userId));
        }

        [HttpPost("/update-user")]
        public async Task UpdateUserProfile(UpdateUserProfile model)
        {
            await _repository.UpdateUserAsync(model);
        }

        [HttpGet("/UpdateUserAppLanguage/{userId}/{appLanguage}")]
        public async Task<ActionResult<byte>> UpdateUserAppLanguage(long userId, AppLanguage appLanguage)
        {
            return await _repository.UpdateUserAppLanguageAsync(userId, appLanguage);
        }

        [HttpGet("/user-list")]
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
        public Task<List<Models.Models.User.User>> GetBaseUserInfo()
        {
            throw new NotImplementedException();
        }

        [HttpGet("/GetCountry/{id}")]
        public async Task<Country> GetBaseUserInfo(long id)
        {
            return await _repository.GetCountryAsync(id);
        }

        [HttpPost("/feedback")]
        public async Task<long> AddFeedback([FromBody] AddFeedback request)
        {
            return await _repository.AddFeedbackAsync(request);
        }

        [HttpPost("/report-user")]
        public async Task<ActionResult> AddUserReport([FromBody] SendUserReport report)
        {
            await _repository.AddUserReportAsync(report);
            return NoContent();
        }

        [HttpPost("/report-adventure")]
        public async Task<ActionResult> ReportAdventure([FromBody] SendAdventureReport report)
        {
            await _repository.AddAdventureReportAsync(report);
            return NoContent();
        }

        [HttpGet("/basic-info/{userId}")]
        public async Task<BasicUserInfo> AddUserReport([FromRoute] long userId)
        {
            return await _repository.GetUserBasicInfo(userId);
        }

        [HttpPost("/user-register")]
        public async Task<long> AddUser(UserRegistrationModel model)
        {

            return await _repository.RegisterUserAsync(model);
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
            return await _repository.GrantAchievementAsync(userId, achievementId);
        }

        [HttpGet("/user-achievements/{userId}")]
        public async Task<List<GetShortAchievement>> GetUserAchievements([FromRoute] long userId)
        {
            return await _repository.GetUserAchievements(userId);
        }

        [HttpGet("/GetUserAchievementsAsAdmin/{userId}")]
        public async Task<List<UserAchievement>> GetUserAchievementsAsAdmin(long userId)
        {
            return await _repository.GetUserAchievementsAsAdmin(userId);
        }

        [HttpGet("/user-achievement/{userId}/{achievementId}")]
        public async Task<GetUserAchievement> GetSingleUserAchievement([FromRoute] long userId, [FromRoute] long achievementId)
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

        [HttpGet("/switch-busy-status/{userId}/{section}")]
        public async Task<SwitchBusyStatusResponse> SwhitchUserBusyStatus([FromRoute] long userId, [FromRoute] Section section)
        {
            return await _repository.SwhitchUserBusyStatus(userId, section);
        }

        [HttpGet("/GetUserRequest/{requestId}")]
        public async Task<UserNotification> GetUserRequest(long requestId)
        {
            return await _repository.GetUserRequest(requestId);
        }

        [HttpGet("/user-requests/{userId}")]
        public async Task<GetRequests> GetUserRequests(long userId)
        {
            return await _repository.GetUserRequests(userId);
        }

        [HttpGet("/CheckUserHasRequests/{userId}")]
        public async Task<bool> CheckUserHasRequests(long userId)
        {
            return await _repository.CheckUserHasRequests(userId);
        }

        [HttpPost("/user-request")]
        public async Task<string> RegisterUserRequest([FromBody] AddRequest request)
        {
            return await _repository.RegisterUserRequestAsync(request);
        }

        [HttpGet("/answer-user-request")]
        public async Task<string> AnswerUserRequest([FromQuery] long requestId, [FromQuery] RequestAnswer reaction)
        {
            return await _repository.AnswerUserRequestAsync(requestId, reaction);
        }

        [HttpGet("/decline-user-request")]
        public async Task<string> DeclineUserRequest([FromQuery] long userId, [FromQuery] long encounteredUser)
        {
            return await _repository.DeclineRequestAsync(userId, encounteredUser);
        }

        [HttpDelete("/DeleteUserRequest/{requestId}")]
        public async Task DeleteUserRequest(long requestId)
        {
            await _repository.DeleteUserRequest(requestId);
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
        public async Task<TrustLevel> GetUserTrustLevel(long userId)
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

        [HttpGet("/user-notifications/{userId}")]
        public async Task<List<UserNotification>> GetUserNotifications(long userId)
        {
            return await _repository.GetUserNotifications(userId);
        }

        [HttpGet("/GetRandomAchievements/{userId}")]
        public async Task<List<string>> GetRandomAchievements(long userId)
        {
            return await _repository.GetRandomAchievements(userId);
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
        public async Task<models.User.OceanPoints> GetOceanPoints(long userId)
        {
            return await _repository.GetOceanPoints(userId);
        }

        [HttpGet("/switch-ocean-usage/{userId}")]
        public async Task<bool> SwitchPersonalityUsage(long userId)
        {
            return await _repository.SwitchOceanUsageAsync(userId);
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
        public async Task<bool> SendTickRequest(SendVerificationRequest request)
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

        [HttpGet("/non-possest-test")]
        public async Task<List<GetTestShortData>> GetTestDataByProperty([FromQuery] long userId, [FromQuery] Enums.Enums.User.OceanStats param)
        {
            return await _repository.GetTestDataByPropertyAsync(userId, param);
        }

        [HttpGet("/test-data-by-prop")]
        public async Task<List<GetTestShortData>> GetUserTestDataByProperty([FromQuery] long userId, [FromQuery] Enums.Enums.User.OceanStats? param)
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

        [HttpGet("/purchase-test")]
        public async Task<IActionResult> PurchaseTest([FromQuery]long userId, [FromQuery] long testId, 
            [FromQuery] float cost, [FromQuery] Currency currency, [FromQuery] AppLanguage language)
        {
            await _repository.PurchaseTestAsync(userId, testId, cost, currency, language);
            return Ok();
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

        [HttpGet("/active-effects/{userId}")]
        public async Task<List<GetActiveEffect>> GetUserActiveEffects(long userId)
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
        public async Task<IActionResult> PurchasePoints([FromQuery] long userId, [FromQuery]float cost, 
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

            return Ok(sender);
        }

        [HttpGet("/PurchesPPForPoints/{userId}/{price}/{count?}")]
        public async Task<bool> PurchesPPForPoints(long userId, float price, short count=1)
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
        public async Task<ActionResult<ManageAdventure>> GetAdventure(long id)
        {
            var adventure = await _repository.GetAdventureAsync(id);

            return Ok((ManageAdventure)adventure);
        }

        [HttpGet("/GetUsersSubscribedAdventures/{userId}")]
        public async Task<List<Models.Models.Adventure.Adventure>> GetUsersSubscribedAdventures(long userId)
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

        // TODO: Finish up
        [HttpPost("/questioner-data")]
        public async Task<IActionResult> ReceiveQuestioner([FromBody] QuestionerPayload payload)
        {
            await _repository.ProcessInterestsDataAsync(payload);
            return Ok();
        }

        [HttpPost("/user-story")]
        public async Task<IActionResult> SetUserStory([FromBody] SetStory model)
        {
            await _repository.SetUserStoryAsync(model);
            return Ok();
        }

        [HttpDelete("/user-story")]
        public async Task<IActionResult> RemoveUserStory([FromQuery] long userId)
        {
            await _repository.RemoveUserStoryAsync(userId);
            return Ok();
        }

        [HttpGet("media/{userId}")]
        public async Task<ActionResult<models.User.UserMedia>> GetUserMedia([FromRoute] long userId)
        {
            var media = await _repository.GetUserMediaAsync(userId);
            return Ok(media);
        }

        [HttpGet("language/{userId}")]
        public async Task<ActionResult<AppLanguage>> GetUserLanguage([FromRoute] long userId)
        {
            var language = await _repository.GetUserLanguageAsync(userId);
            return Ok(language);
        }
    }
}
