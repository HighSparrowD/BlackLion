using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Entities.AchievementEntities;
using MyWebApi.Entities.AdminEntities;
using MyWebApi.Entities.DailyTaskEntities;
using MyWebApi.Entities.LocationEntities;
using MyWebApi.Entities.ReasonEntities;
using MyWebApi.Entities.ReportEntities;
using MyWebApi.Entities.SecondaryEntities;
using MyWebApi.Entities.TestEntities;
using MyWebApi.Entities.UserInfoEntities;
using MyWebApi.Enums;
using MyWebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebApi.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private UserContext _contx { get; set; }
        private IUserRepository _userRep { get; set; }

        public AdminRepository(UserContext context, IUserRepository userRepository)
        {
            _contx = context;
            _userRep = userRepository;
        }

        public async Task<long> UploadCities(List<City> cities)
        {
            cities.ForEach(async c => await _contx.CITIES.AddAsync(c));
            await _contx.SaveChangesAsync();
            return cities.Count;
        }

        public async Task<long> UploadCountries(List<Country> countries)
        {
            countries.ForEach(async c => 
            {
                if (!_contx.COUNTRIES.Contains(c))
                    await _contx.COUNTRIES.AddAsync(c);
                else
                    _contx.COUNTRIES.Update(c);
            });
            await _contx.SaveChangesAsync();

            return countries.Count;
        }

        public async Task<long> UploadLanguages(List<Language> langs)
        {
            langs.ForEach(async l => 
            {
                if (!_contx.LANGUAGES.Contains(l))
                    await _contx.LANGUAGES.AddAsync(l);
                else
                    _contx.LANGUAGES.Update(l);
            });
            await _contx.SaveChangesAsync();

            return langs.Count;
        }

        public async Task<long> UploadFeedbackReasons(List<FeedbackReason> reasons)
        {
            reasons.ForEach(async r => 
            {
                if (!_contx.FEEDBACK_REASONS.Contains(r))
                    await _contx.FEEDBACK_REASONS.AddAsync(r);
                else
                    _contx.FEEDBACK_REASONS.Update(r);
            });
            await _contx.SaveChangesAsync();
            return reasons.Count;
        }

        public async Task<List<Feedback>> GetFeedbacks()
        {
            var reports = await _contx.SYSTEM_FEEDBACKS.Include(r => r.User)
                .Include(r => r.Reason).ToListAsync();

            return reports;
        }

        public async Task<bool> CheckUserIsAdmin(long userId)
        {
            var admin = await _contx.SYSTEM_ADMINS.FindAsync(userId);
            if (admin == null) { return false; }

            return admin.IsEnabled;
        }

        public async Task<byte> SwitchAdminStatus(long userId)
        {
            var admin = await _contx.SYSTEM_ADMINS.Where(a => a.Id == userId).SingleOrDefaultAsync();
            if (admin == null) { return 0; }

            admin.IsEnabled = admin.IsEnabled ? false : true;
            _contx.Update(admin);
            await _contx.SaveChangesAsync();
            return 1;
        }

        public async Task<bool?> GetAdminStatus(long userId)
        {
            var admin = await _contx.SYSTEM_ADMINS.Where(a => a.Id == userId).SingleOrDefaultAsync();
            if (admin == null){ return null; }

            return admin.IsEnabled;
        }

        public async Task<long> DeleteUser(long userId)
        {
            try
            {
                var user = await _contx.SYSTEM_USERS.Where(u => u.UserId == userId).SingleOrDefaultAsync();
                var userBase = await _contx.SYSTEM_USERS_BASES.Where(u => u.Id == userId).SingleOrDefaultAsync();
                var userData = await _contx.SYSTEM_USERS_DATA.Where(u => u.Id == userId).SingleOrDefaultAsync();
                var userPrefs = await _contx.SYSTEM_USERS_PREFERENCES.Where(u => u.Id == userId).SingleOrDefaultAsync();
                var userLocation = await _contx.USER_LOCATIONS.Where(u => u.Id == userId).SingleOrDefaultAsync();
                var userAchievements = await _contx.USER_ACHIEVEMENTS.Where(u => u.UserBaseInfoId == userId).ToListAsync();
                var userPurchases = await _contx.USER_WALLET_PURCHASES.Where(u => u.UserId == userId).ToListAsync();
                var userBalances = await _contx.USER_WALLET_BALANCES.Where(u => u.UserId == userId).ToListAsync();
                var userNotifications = await _contx.USER_NOTIFICATIONS.Where(u => u.UserId == userId).ToListAsync();
                var userNotifications1 = await _contx.USER_NOTIFICATIONS.Where(u => u.UserId1 == userId).ToListAsync();
                var sponsorRatings = await _contx.SPONSOR_RATINGS.Where(u => u.UserId == userId).ToListAsync();
                var userTrustLevel = await _contx.USER_TRUST_LEVELS.Where(u => u.Id == userId).SingleOrDefaultAsync();
                var userInvitations = await _contx.USER_INVITATIONS.Where(u => u.InvitorCredentials.UserId == userId).ToListAsync();
                var userInvitationCreds = await _contx.USER_INVITATION_CREDENTIALS.Where(u => u.UserId == userId).SingleOrDefaultAsync();

                if (userInvitations.Count > 0)
                {
                    _contx.USER_INVITATIONS.RemoveRange(userInvitations);
                    await _contx.SaveChangesAsync();
                }

                if (userInvitationCreds != null)
                {
                    _contx.USER_INVITATION_CREDENTIALS.Remove(userInvitationCreds);
                    await _contx.SaveChangesAsync();
                }

                if (userLocation != null)
                {
                    _contx.USER_LOCATIONS.Remove(userLocation);
                    await _contx.SaveChangesAsync();
                }
                if (userAchievements.Count > 0)
                {
                    _contx.USER_ACHIEVEMENTS.RemoveRange(userAchievements);
                    await _contx.SaveChangesAsync();
                }
                if (userBalances.Count > 0)
                {
                    _contx.USER_WALLET_BALANCES.RemoveRange(userBalances);
                    await _contx.SaveChangesAsync();
                }
                if (userPurchases.Count > 0)
                {
                    _contx.USER_WALLET_PURCHASES.RemoveRange(userPurchases);
                    await _contx.SaveChangesAsync();
                }
                if (userNotifications.Count > 0)
                {
                    _contx.USER_NOTIFICATIONS.RemoveRange(userNotifications);
                    await _contx.SaveChangesAsync();
                }
                if (userNotifications1.Count > 0)
                {
                    _contx.USER_NOTIFICATIONS.RemoveRange(userNotifications1);
                    await _contx.SaveChangesAsync();
                }
                if (sponsorRatings.Count > 0)
                {
                    _contx.SPONSOR_RATINGS.RemoveRange(sponsorRatings);
                    await _contx.SaveChangesAsync();
                }
                if (userTrustLevel != null)
                {
                    _contx.USER_TRUST_LEVELS.Remove(userTrustLevel);
                    await _contx.SaveChangesAsync();
                }
                if (userBase != null)
                {
                    _contx.SYSTEM_USERS_BASES.Remove(userBase);
                    await _contx.SaveChangesAsync();
                }
                if (userPrefs != null)
                {
                    _contx.SYSTEM_USERS_PREFERENCES.Remove(userPrefs);
                    await _contx.SaveChangesAsync();
                }
                if (userData != null)
                {
                    _contx.SYSTEM_USERS_DATA.Remove(userData);
                    await _contx.SaveChangesAsync();
                }
                if (user != null)
                {
                    _contx.SYSTEM_USERS.Remove(user);
                    await _contx.SaveChangesAsync();
                }

                await _contx.SaveChangesAsync();

                return userId;
            }
            catch { return -1; }
        }

        public async Task<int> DeleteAllUsers()
        {
            try
            {
                var usersCount = await _contx.SYSTEM_USERS.CountAsync();

                var user = await _contx.SYSTEM_USERS.ToListAsync();
                var userBase = await _contx.SYSTEM_USERS_BASES.ToListAsync();
                var userData = await _contx.SYSTEM_USERS_DATA.ToListAsync();
                var userPrefs = await _contx.SYSTEM_USERS_PREFERENCES.ToListAsync();
                var userLocation = await _contx.USER_LOCATIONS.ToListAsync();
                var userAchievements = await _contx.USER_ACHIEVEMENTS.ToListAsync();
                var userPurchases = await _contx.USER_WALLET_PURCHASES.ToListAsync();
                var userBalances = await _contx.USER_WALLET_BALANCES.ToListAsync();
                var userNotifications = await _contx.USER_NOTIFICATIONS.ToListAsync();
                var userNotifications1 = await _contx.USER_NOTIFICATIONS.ToListAsync();
                var sponsorRatings = await _contx.SPONSOR_RATINGS.ToListAsync();

                _contx.USER_LOCATIONS.RemoveRange(userLocation);
                _contx.USER_ACHIEVEMENTS.RemoveRange(userAchievements);
                _contx.USER_WALLET_BALANCES.RemoveRange(userBalances);
                _contx.USER_WALLET_PURCHASES.RemoveRange(userPurchases);
                _contx.USER_NOTIFICATIONS.RemoveRange(userNotifications);
                _contx.USER_NOTIFICATIONS.RemoveRange(userNotifications1);
                _contx.SPONSOR_RATINGS.RemoveRange(sponsorRatings);
                _contx.SYSTEM_USERS_BASES.RemoveRange(userBase);
                _contx.SYSTEM_USERS_PREFERENCES.RemoveRange(userPrefs);
                _contx.SYSTEM_USERS_DATA.RemoveRange(userData);
                _contx.SYSTEM_USERS.RemoveRange(user);

                await _contx.SaveChangesAsync();

                return usersCount;
            }
            catch { return -1; }
        }

        //DO NOT use that method in production
        public async Task<byte> UploadAchievements(List<Achievement> achievements)
        {
            try
            {
                var ach = await _contx.SYSTEM_ACHIEVEMENTS.ToListAsync();

                if (ach != null)
                    _contx.SYSTEM_ACHIEVEMENTS.RemoveRange(ach);

                await _contx.SYSTEM_ACHIEVEMENTS.AddRangeAsync(achievements);
                await _contx.SaveChangesAsync();

                if (await UpdateUsersAchievements(achievements, shouldDeleteAll:true))
                    return 1;

                return 0;
            }
            catch { return 0; }
        }

        public async Task<byte> AddNewAchievements(List<Achievement> achievements)
        {
            try
            {
                await _contx.SYSTEM_ACHIEVEMENTS.AddRangeAsync(achievements);
                await _contx.SaveChangesAsync();

                if (await UpdateUsersAchievements(achievements))
                    return 1;

                return 0;
            }
            catch { return 0; }
        }

        //Used when new achievements added to DB
        private async Task<bool> UpdateUsersAchievements(List<Achievement> achievements, bool shouldDeleteAll=false)
        {
            try
            {
                if (shouldDeleteAll)
                {
                    _contx.USER_ACHIEVEMENTS.RemoveRange(await _contx.USER_ACHIEVEMENTS.ToListAsync());
                    await _contx.SaveChangesAsync();
                }

                await Task.Run(async() => {             
                    foreach (var achievement in achievements)
                    {
                        var users = await _contx.SYSTEM_USERS_DATA.ToListAsync();
                        foreach (var user in users)
                        {
                            if (achievement.ClassLocalisationId == user.LanguageId)
                            {
                                var a = new UserAchievement(achievement.Id, user.Id, achievement.ClassLocalisationId, achievement.Name, achievement.Description, achievement.Value, achievement.ClassLocalisationId);
                                _contx.USER_ACHIEVEMENTS.Add(a);
                                await _contx.SaveChangesAsync();
                            }
                        }
                    }
                });

                return true;
            }
            catch { return false; }
        }

        public async Task<byte> AddDailyTaskAsync(DailyTask model)
        {
            try
            {
                model.Id = (await _contx.DAILY_TASKS.CountAsync()) + 1;
                await _contx.DAILY_TASKS.AddAsync(model);
                await _contx.SaveChangesAsync();

                return 1;
            }
            catch { return 0; }
        }

        public async Task<List<TickRequest>> GetTickRequestsAsync()
        {
            return await _contx.tick_requests.Take(15)
                .ToListAsync();
        }

        public async Task<TickRequest> GetTickRequestAsync(Guid? requestId = null)
        {
            if (requestId != null)
            {
                //Returns only new, aborted or changed requests
                return await _contx.tick_requests.Where(r => r.Id == requestId && (r.State == 1 || r.State == 2 || r.State == 6))
                    .Include(r => r.User)
                    .SingleOrDefaultAsync();
            }

            //Return any request if id wasnt supplied. (Method is used on the frontend)
            var request = await _contx.tick_requests.Where(r => r.State == 1 || r.State == 2 || r.State == 6)
                .Include(r => r.User)
                .FirstOrDefaultAsync();

            if (request != null)
            {
                request.State = (short)SystemEnums.TickRequestStatus.InProcess;
                await _contx.SaveChangesAsync();
            }

            return request;
        }

        public async Task<bool> ResolveTickRequestAsync(Guid requestId, long adminId, bool isAccepted)
        {
            var request = await _contx.tick_requests.Where(r => r.Id == requestId && (r.State == 3))
                .Include(r => r.User)
                .SingleOrDefaultAsync();

            if (request == null)
                throw new NullReferenceException("Request was not found");

            if (isAccepted)
                request.State = (short)SystemEnums.TickRequestStatus.Accepted;
            else
                request.State = (short)SystemEnums.TickRequestStatus.Declined;

            request.AdminId = adminId;
            request.User.IsIdentityConfirmed = isAccepted;
            await _contx.SaveChangesAsync();

            if (isAccepted)
                await _userRep.AddUserNotificationAsync(new Entities.UserActionEntities.UserNotification
                {
                    Description = "Your tick request had been accepted :)",
                    UserId1 = request.UserId,
                    Severity = (short)SystemEnums.Severities.Urgent,
                    SectionId = (int)SystemEnums.Sections.Neutral,
                });
            else
                await _userRep.AddUserNotificationAsync(new Entities.UserActionEntities.UserNotification
                {
                    Description = "Sorry, your tick request had been denied.\nPlease contact the administration and try again",
                    UserId1 = request.UserId,
                    Severity = (short)SystemEnums.Severities.Urgent,
                    SectionId = (int)SystemEnums.Sections.Neutral,
                });

            return isAccepted;
        }

        public async Task<byte> UploadPsTestsAsync(List<UploadTest> tests)
        {
            try
            {
                long testId = 0;

                for (int i = 0; i < tests.Count; i++)
                {
                    var model = tests[i];
                    //Check if a version of test already exists
                    var existingTest = await _contx.tests.Where(t => t.Id == model.Id)
                        .SingleOrDefaultAsync();
                    if (existingTest != null)
                    {
                        if (existingTest.ClassLocalisationId == model.ClassLocalisationId)
                            throw new Exception("This version of test already exists");

                        testId = model.Id;
                    }
                    else
                        testId = await _contx.tests.CountAsync() + 1;

                    var lastQuestionId = await _contx.tests_questions.CountAsync();
                    var lastAnswerId = await _contx.tests_answers.CountAsync() + 1;

                    var questions = new List<TestQuestion>();
                    var answers = new List<TestAnswer>();
                    var test = new Test
                    {
                        Id = testId,
                        ClassLocalisationId = model.ClassLocalisationId,
                        Name = model.Name,
                        Description = model.Description,
                        TestType = model.TestType,
                        Price = model.Price
                    };

                    foreach (var question in model.Questions)
                    {
                        lastQuestionId++;

                        questions.Add(new TestQuestion
                        {
                            Id = lastQuestionId,
                            TestClassLocalisationId = test.ClassLocalisationId,
                            TestId = testId,
                            Text = question.Text
                        });

                        foreach (var answer in question.Answers)
                        {
                            lastAnswerId++;

                            answers.Add(new TestAnswer
                            {
                                Id = lastAnswerId,
                                Text = answer.Text,
                                Value = answer.Value,
                                IsCorrect = answer.IsCorrect,
                                TestQuestionId = lastQuestionId
                            });
                        }
                    }
                    await _contx.tests.AddAsync(test);
                    await _contx.tests_questions.AddRangeAsync(questions);
                    await _contx.tests_answers.AddRangeAsync(answers);
                    await _contx.SaveChangesAsync();
                }

                return 1;
            }
            catch { return 0; }
        }

        public async Task<string> GetNewNotificationsCountAsync(long adminId)
        {
            string returnData = "";

            returnData = $"Recent feedbacks: {(await _userRep.GetMostRecentFeedbacks()).Count}\nActive tick requests {(await _contx.tick_requests.Where(r => (r.State == 1 || r.State == 2 || r.State == 6) && r.AdminId == null).ToListAsync()).Count}";

            return returnData;
        }

        public async Task<string> GetUserPhotoAsync(long userId)
        {
            return await _contx.SYSTEM_USERS_BASES.Where(b => b.Id == userId)
                .Select(u => u.UserPhoto)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AbortTickRequestAsync(Guid requestId)
        {
            try
            {
                //Get request if it was marked as processed
                var request = await _contx.tick_requests.Where(r => r.Id == requestId && (r.State == 3))
                        .SingleOrDefaultAsync(); ;

                if (request == null)
                    throw new NullReferenceException($"Request {requestId} is not in process rigt now");

                request.State = (short)SystemEnums.TickRequestStatus.Aborted;
                await _contx.SaveChangesAsync();

                return true;
            }
            catch { return false; }
        }

        public async Task<bool> NotifyFailierTickRequestAsync(Guid requestId, long adminId)
        {
            try
            {
                var request = await _contx.tick_requests.Where(r => r.Id == requestId)
                        .SingleOrDefaultAsync(); ;

                if (request == null)
                    throw new NullReferenceException($"Request {requestId} does not exist");

                request.State = (short)SystemEnums.TickRequestStatus.Failed;
                request.AdminId = adminId;

                await _contx.SaveChangesAsync();

                return true;
            }
            catch { return false; }
        }

        public async Task<bool> CreateDecoyAsync(long? copyUserId = null, UserRegistrationModel model = null)
        {
            var rand = new Random();

            UserBaseInfo uBase = null;
            UserDataInfo uData = null;
            UserPreferences uPrefs= null;
            User m = null;
            Location location = null;

            if (model != null)
            {
                var langCount = await _userRep.GetUserMaximumLanguageCountAsync(model.Id);
                if (model.UserLanguages.Count > langCount)
                    throw new Exception($"This user cannot have more than {langCount} languages !");
                
                uBase = new UserBaseInfo(model.Id + rand.Next(8000), model.UserName, model.UserRealName, model.UserDescription, model.UserPhoto, model.IsPhotoReal);
                uData = new UserDataInfo
                {
                    Id = uBase.Id,
                    UserLanguages = model.UserLanguages,
                    ReasonId = model.ReasonId,
                    UserAge = model.UserAge,
                    UserGender = model.UserGender,
                    LanguageId = model.UserAppLanguageId,
                };
                uPrefs = new UserPreferences(uBase.Id, model.UserLanguagePreferences, model.UserLocationPreferences, model.AgePrefs, model.CommunicationPrefs, model.UserGenderPrefs, model.ShouldUserPersonalityFunc);
                uPrefs.ShouldFilterUsersWithoutRealPhoto = false;
                m = new User(uBase.Id)
                {
                    IsBusy = false,
                    IsDeleted = false,
                    IsBanned = false,
                    ShouldConsiderLanguages = false,
                    HasPremium = false,
                    HadReceivedReward = false,
                    DailyRewardPoint = 0,
                    BonusIndex = 1,
                    ProfileViewsCount = 0,
                    InvitedUsersCount = 0,
                    InvitedUsersBonus = 0,
                    TagSearchesCount = 0,
                    MaxProfileViewsCount = 50,
                    IsIdentityConfirmed = false,
                };

                if (model.UserCityCode != null && model.UserCountryCode != null)
                    location = new Location { Id = uBase.Id, CityId = (int)model.UserCityCode, CountryId = (int)model.UserCountryCode, CityCountryClassLocalisationId = model.UserAppLanguageId, CountryClassLocalisationId = model.UserAppLanguageId };
                else
                    location = new Location { Id = uBase.Id };

                uData.LocationId = location.Id;
            }
            else
            {
                uBase = await _contx.SYSTEM_USERS_BASES.Where(b => b.Id == copyUserId).AsNoTracking().SingleOrDefaultAsync();
                uData = await _contx.SYSTEM_USERS_DATA.Where(b => b.Id == copyUserId).AsNoTracking().SingleOrDefaultAsync();
                uPrefs = await _contx.SYSTEM_USERS_PREFERENCES.Where(b => b.Id == copyUserId).AsNoTracking().SingleOrDefaultAsync();
                m = await _contx.SYSTEM_USERS.Where(b => b.UserId == copyUserId).AsNoTracking().SingleOrDefaultAsync();
                location = await _contx.USER_LOCATIONS.Where(b => b.Id == copyUserId).AsNoTracking().SingleOrDefaultAsync();

                uBase.Id += rand.Next(8000);
                uData.Id = uBase.Id;
                uPrefs.Id = uBase.Id;
                m.UserId = uBase.Id;
                location.Id = uBase.Id;
            }

            await _userRep.RegisterUserAsync(m, uBase, uData, uPrefs, location);

            return true;
        }

        //public Task<long> UploadInTest(UploadInTest model)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
