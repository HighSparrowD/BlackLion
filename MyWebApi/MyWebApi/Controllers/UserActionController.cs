using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyWebApi.Core;
using MyWebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWebApi.Data;
using MyWebApi.Entities.UserInfoEntities;
using MyWebApi.Entities.ReportEntities;
using MyWebApi.Entities.LocationEntities;
using MyWebApi.Entities.ReasonEntities;
using MyWebApi.Entities.AchievementEntities;
using MyWebApi.Entities.UserActionEntities;
using MyWebApi.Entities.SponsorEntities;

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

        [HttpGet("/GetUserLanguagePrefs/{userId}")]
        public async Task<int> GetUserLanguagePrefs(long userId)
        {
            return await _repository.GetUserAppLanguage(userId);
        }

        [HttpGet("/UserInfo/{userId}")]
        public async Task<ActionResult<User>> GetUserInfo(long userId)
        {
            return await _repository.GetUserInfoAsync(userId);
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
        public async Task<List<User>> GetUserList(long userId)
        {
            return await _repository.GetUsersAsync(userId);
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
            var location = new Location { Id = model.Id, CityId = model.UserCityCode, CountryId = model.UserCountryCode };
            var uBase = new UserBaseInfo(model.Id, model.UserName, model.UserRealName, model.UserDescription, model.UserPhoto);
            var uData = new UserDataInfo
            {
                Id = model.Id,
                UserLanguages = model.UserLanguages,
                ReasonId = model.ReasonId,
                UserAge = model.UserAge,
                UserGender = model.UserGender,
                LanguageId = model.UserAppLanguageId,
                LocationId = location.Id
            };
            var uPrefs = new UserPreferences(model.Id, model.UserLanguagePreferences, model.UserLocationPreferences, Entities.UserInfoEntities.User.CalculateAgeList(model.UserAge, model.AgePrefs), model.CommunicationPrefs, model.UserGenderPrefs);
            var m = new User(model.Id);
            m.IsBusy = false;
            m.IsDeleted = false;
            m.IsBanned = false;
            m.ShouldConsiderLanguages = false;
            m.HasPremium = false;

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
        public async Task<long> AddUser(long userId, long bannedUserId)
        {
            return await _repository.AddUserToBlackListAsync(userId, bannedUserId);
        }

        [HttpDelete("/RemoveUserFromBlackList/{userId}/{bannedUserId}")]
        public async Task<long> RemoveUserFromBlackList(long userId, long bannedUserId)
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
            return await _repository.GetUserWalletBalance(userId, DateTime.Now);
        }

        [HttpGet("/TopUpUserWalletBalance/{userId}/{points}/{description}")]
        public async Task<int> GetActiveUserWalletBalance(long userId, int points, string description)
        {
            return await _repository.TopUpUserWalletBalance(userId, points, description);
        }

        [HttpGet("/CheckUserHasPremium/{userId}")]
        public async Task<bool> CheckUserHasPremium(long userId)
        {
            return await _repository.CheckUserHasPremium(userId);
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

        [HttpGet("/GrantPremiumToUser/{userId}/{cost}/{dayDuration}")]
        public async Task<DateTime> GrantPremiumToUser(long userId, int cost, int dayDuration)
        {
            return await _repository.GrantPremiumToUser(userId, cost, dayDuration);
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
        public async Task<long> RegisterUserRequest(UserNotification request)
        {
            return await _repository.RegisterUserRequest(request);
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
        public async Task<long> RegisterUserRequest(Encounter model)
        {
            return await _repository.RegisterUserEncounter(model);
        }

        [HttpGet("/GetUserEncounter/{userId}/{encounterId}/{sectionId}")]
        public async Task<Encounter> GetUserEncounter(long userId, long encounterId, int sectionId)
        {
            return await _repository.GetUserEncounter(userId, encounterId, sectionId);
        }

        [HttpGet("/GetUserEncounters/{userId}/{sectionId}")]
        public async Task<List<Encounter>> GetUserEncounters(long userId, int sectionId)
        {
            return await _repository.GetUserEncounters(userId, sectionId);
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

        [HttpGet("/DeleteUserNotification/{notificationId}")]
        public async Task<bool> DeleteUserNotification(long notificationId)
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
    }
}
