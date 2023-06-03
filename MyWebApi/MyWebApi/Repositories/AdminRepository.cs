using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Entities.AchievementEntities;
using WebApi.Entities.AdminEntities;
using WebApi.Entities.DailyTaskEntities;
using WebApi.Entities.LocationEntities;
using WebApi.Entities.ReasonEntities;
using WebApi.Entities.ReportEntities;
using WebApi.Entities.SecondaryEntities;
using WebApi.Entities.TestEntities;
using WebApi.Entities.UserActionEntities;
using WebApi.Entities.UserInfoEntities;
using WebApi.Enums;
using WebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Repositories
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
            cities.ForEach(async c => await _contx.cities.AddAsync(c));
            await _contx.SaveChangesAsync();
            return cities.Count;
        }

        public async Task<long> UploadCountries(List<Country> countries)
        {
            countries.ForEach(async c => 
            {
                if (!_contx.countries.Contains(c))
                    await _contx.countries.AddAsync(c);
                else
                    _contx.countries.Update(c);
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
                var user = await _contx.users.Where(u => u.Id == userId).SingleOrDefaultAsync();
                var userData = await _contx.users_settings.Where(u => u.Id == userId).SingleOrDefaultAsync();
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
                if (userLocation != null)
                {
                    _contx.USER_LOCATIONS.Remove(userLocation);
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
                if (userData != null)
                {
                    _contx.users_settings.Remove(userData);
                    await _contx.SaveChangesAsync();
                }
                if (user != null)
                {
                    _contx.users.Remove(user);
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
                var usersCount = await _contx.users.CountAsync();

                var user = await _contx.users.ToListAsync();
                var userData = await _contx.users_settings.ToListAsync();
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
                _contx.users_settings.RemoveRange(userData);
                _contx.users.RemoveRange(user);

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
                        var users = await _contx.users_data.ToListAsync();
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
                //Returns only new, aborted, failed or changed requests
                return await _contx.tick_requests.Where(r => r.Id == requestId && (r.State == TickRequestStatus.Added 
                || r.State == TickRequestStatus.Changed 
                || r.State == TickRequestStatus.Aborted 
                || r.State == TickRequestStatus.Failed))
                    .Include(r => r.User)
                    .SingleOrDefaultAsync();
            }

            //Return any request if id wasnt supplied. (Method is used on the frontend)
            var request = await _contx.tick_requests.Where(r => r.State == TickRequestStatus.Added || r.State == TickRequestStatus.Changed || r.State == TickRequestStatus.Aborted)
                .Include(r => r.User)
                .FirstOrDefaultAsync();

            if (request != null)
            {
                request.State = TickRequestStatus.InProcess;
                await _contx.SaveChangesAsync();
            }

            return request;
        }

        public async Task<bool> ResolveTickRequestAsync(ResolveTickRequest model)
        {
            var request = await _contx.tick_requests.Where(r => r.Id == model.Id && (r.State == TickRequestStatus.InProcess))
                .Include(r => r.User)
                .SingleOrDefaultAsync();

            if (request == null)
                throw new NullReferenceException("Request was not found");

            if (model.IsAccepted)
                request.State = TickRequestStatus.Accepted;
            else
                request.State = TickRequestStatus.Declined;

            request.AdminId = model.AdminId;
            request.User.IdentityType = request.Type;

            await _contx.SaveChangesAsync();

            if (model.IsAccepted)
                await _userRep.AddUserNotificationAsync(new Entities.UserActionEntities.UserNotification
                {
                    Description = $"Your identity confirmation had been accepted :)\n{model.Comment}",
                    UserId1 = request.UserId,
                    Severity = Severities.Urgent,
                    Section = Sections.Neutral,
                });
            else
                await _userRep.AddUserNotificationAsync(new Entities.UserActionEntities.UserNotification
                {
                    Description = $"Sorry, your identity confirmation request had been denied.\n{model.Comment}",
                    UserId1 = request.UserId,
                    Severity = Severities.Urgent,
                    Section = Sections.Neutral,
                });

            return model.IsAccepted;
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
                            //Continue if test version already exists.
                            //It allows to avoid constantly changing source file in tools
                            continue;

                        testId = model.Id;
                    }
                    else
                        testId = await _contx.tests.CountAsync() + 1;

                    var lastQuestionId = await _contx.tests_questions.CountAsync();
                    var lastAnswerId = await _contx.tests_answers.CountAsync();
                    var lastResultId = await _contx.tests_results.CountAsync();

                    var results = new List<TestResult>();
                    var questions = new List<TestQuestion>();
                    var answers = new List<TestAnswer>();
                    var test = new Test
                    {
                        Id = testId,
                        ClassLocalisationId = model.ClassLocalisationId,
                        Name = model.Name,
                        Description = model.Description,
                        TestType = model.TestType,
                        Price = model.Price,
                        CanBePassedInDays = model.CanBePassedInDays
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
                                TestQuestionId = lastQuestionId,
                                Tags = answer.Tags
                            });
                        }
                    }

                    foreach (var result in model.Results)
                    {
                        lastResultId++;

                        results.Add(new TestResult
                        {
                            Id = lastResultId,
                            Result = result.Result,
                            Score = result.Score,
                            TestId = test.Id,
                            TestClassLocalisationId = test.ClassLocalisationId,
                            Tags = result.Tags
                        });
                    }

                    await _contx.tests.AddAsync(test);
                    await _contx.tests_questions.AddRangeAsync(questions);
                    await _contx.tests_answers.AddRangeAsync(answers);
                    await _contx.tests_results.AddRangeAsync(results);
                    await _contx.SaveChangesAsync();
                }

                return 1;
            }
            catch { return 0; }
        }

        public async Task<string> GetNewNotificationsCountAsync(long adminId)
        {
            string returnData = "";

            var recentFeedbacks = await _userRep.GetMostRecentFeedbacks();
            var tickRequests = await _contx.tick_requests
                .Where(r => (r.State == TickRequestStatus.Added || 
                r.State == TickRequestStatus.Changed || 
                r.State == TickRequestStatus.Aborted || 
                r.State == TickRequestStatus.Failed) && 
                r.AdminId == null)
                .ToListAsync();


            var bannedUsers = await GetRecentlyBannedUsersAsync();

            returnData = $"Recent feedbacks: {recentFeedbacks.Count}\nActive tick requests {tickRequests.Count}\nRecently banned users{bannedUsers.Count}";

            return returnData;
        }

        public async Task<bool> AbortTickRequestAsync(Guid requestId)
        {
            try
            {
                //Get request if it was marked as processed
                var request = await _contx.tick_requests.Where(r => r.Id == requestId && (r.State == TickRequestStatus.InProcess))
                        .SingleOrDefaultAsync(); ;

                if (request == null)
                    throw new NullReferenceException($"Request {requestId} is not in process rigt now");

                request.State = TickRequestStatus.Aborted;
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

                request.State = TickRequestStatus.Failed;
                request.AdminId = adminId;

                await _contx.SaveChangesAsync();

                return true;
            }
            catch { return false; }
        }

        public async Task<List<long>> GetRecentlyBannedUsersAsync()
        {
            return await _contx.users.Where(u => u.IsBanned && u.BanDate != null)
                .Select(u => u.Id)
                .ToListAsync();
        }

        //public Task<long> UploadInTest(UploadInTest model)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
