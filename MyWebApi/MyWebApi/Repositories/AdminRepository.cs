using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Entities.AchievementEntities;
using WebApi.Entities.AdminEntities;
using WebApi.Entities.DailyTaskEntities;
using WebApi.Entities.LocationEntities;
using WebApi.Entities.ReportEntities;
using WebApi.Entities.SecondaryEntities;
using WebApi.Entities.TestEntities;
using WebApi.Enums;
using WebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;

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

        public async Task<long> UploadCities(List<UpdateCity> cities)
        {
            var dbCities = await _contx.Cities.ToListAsync();

            foreach (var city in cities)
            {
                if (dbCities.Any(c => c.Id == city.Id))
                    continue;

                await _contx.Cities.AddAsync(new City
                {
                    Id = city.Id,
                    CityName = city.CityName.ToLower(),
                    CountryId = city.CountryId,
                    CountryLang = city.Lang
                });
            }

            await _contx.SaveChangesAsync();
            return cities.Count;
        }

        public async Task<long> UploadCountries(List<UpdateCountry> countries)
        {
            var dbCountries = await _contx.Countries.ToListAsync();

            foreach (var country in countries)
            {
                if (dbCountries.Any(c => c.Id == country.Id))
                    continue;

                await _contx.Countries.AddAsync(new Country
                {
                    Id = country.Id,
                    CountryName = country.CountryName,
                    Priority = country.Priority,
                    Lang = country.Lang
                });
            }

            await _contx.SaveChangesAsync();
            return countries.Count;
        }

        public async Task<long> UploadLanguages(List<UpdateLanguage> langs)
        {
            var dbLangs = await _contx.Languages.ToListAsync();

            foreach (var lang in langs)
            {
                if (dbLangs.Any(c => c.Id == lang.Id))
                    continue;

                await _contx.Languages.AddAsync(new Language
                {
                    Id = lang.Id,
                    LanguageName = lang.LanguageName,
                    LanguageNameNative = lang.LanguageNameNative,
                    Priority = lang.Priority,
                    Lang = lang.Lang
                });
            }

            await _contx.SaveChangesAsync();
            return langs.Count;
        }

        public async Task<List<Feedback>> GetFeedbacks()
        {
            var reports = await _contx.Feedbacks.Include(r => r.User)
                .Include(r => r.Reason).ToListAsync();

            return reports;
        }

        public async Task<bool> CheckUserIsAdmin(long userId)
        {
            var admin = await _contx.Admins.FindAsync(userId);
            if (admin == null) { return false; }

            return admin.IsEnabled;
        }

        public async Task<byte> SwitchAdminStatus(long userId)
        {
            var admin = await _contx.Admins.Where(a => a.Id == userId).SingleOrDefaultAsync();
            if (admin == null) { return 0; }

            admin.IsEnabled = admin.IsEnabled ? false : true;
            _contx.Update(admin);
            await _contx.SaveChangesAsync();
            return 1;
        }

        public async Task<bool?> GetAdminStatus(long userId)
        {
            var admin = await _contx.Admins.Where(a => a.Id == userId).SingleOrDefaultAsync();
            if (admin == null){ return null; }

            return admin.IsEnabled;
        }

        public async Task<long> DeleteUser(long userId)
        {
            try
            {
                var user = await _contx.Users.Where(u => u.Id == userId).SingleOrDefaultAsync();
                var userData = await _contx.UsersSettings.Where(u => u.Id == userId).SingleOrDefaultAsync();
                var userLocation = await _contx.UserLocations.Where(u => u.Id == userId).SingleOrDefaultAsync();
                var userAchievements = await _contx.UserAchievements.Where(u => u.UserId == userId).ToListAsync();
                var userPurchases = await _contx.Transaction.Where(u => u.UserId == userId).ToListAsync();
                var userBalances = await _contx.Balances.Where(u => u.UserId == userId).ToListAsync();
                var userNotifications = await _contx.Notifications.Where(u => u.SenderId == userId).ToListAsync();
                var userNotifications1 = await _contx.Notifications.Where(u => u.ReceiverId == userId).ToListAsync();
                var sponsorRatings = await _contx.SponsorRatings.Where(u => u.UserId == userId).ToListAsync();
                var userTrustLevel = await _contx.TrustLevels.Where(u => u.Id == userId).SingleOrDefaultAsync();
                var userInvitations = await _contx.Invitations.Where(u => u.InviterCredentials.UserId == userId).ToListAsync();
                var userInvitationCreds = await _contx.InvitationCredentials.Where(u => u.UserId == userId).SingleOrDefaultAsync();

                if (userInvitations.Count > 0)
                {
                    _contx.Invitations.RemoveRange(userInvitations);
                    await _contx.SaveChangesAsync();
                }

                if (userInvitationCreds != null)
                {
                    _contx.InvitationCredentials.Remove(userInvitationCreds);
                    await _contx.SaveChangesAsync();
                }
                if (userAchievements.Count > 0)
                {
                    _contx.UserAchievements.RemoveRange(userAchievements);
                    await _contx.SaveChangesAsync();
                }
                if (userBalances.Count > 0)
                {
                    _contx.Balances.RemoveRange(userBalances);
                    await _contx.SaveChangesAsync();
                }
                if (userPurchases.Count > 0)
                {
                    _contx.Transaction.RemoveRange(userPurchases);
                    await _contx.SaveChangesAsync();
                }
                if (userNotifications.Count > 0)
                {
                    _contx.Notifications.RemoveRange(userNotifications);
                    await _contx.SaveChangesAsync();
                }
                if (userNotifications1.Count > 0)
                {
                    _contx.Notifications.RemoveRange(userNotifications1);
                    await _contx.SaveChangesAsync();
                }
                if (userLocation != null)
                {
                    _contx.UserLocations.Remove(userLocation);
                    await _contx.SaveChangesAsync();
                }
                if (sponsorRatings.Count > 0)
                {
                    _contx.SponsorRatings.RemoveRange(sponsorRatings);
                    await _contx.SaveChangesAsync();
                }
                if (userTrustLevel != null)
                {
                    _contx.TrustLevels.Remove(userTrustLevel);
                    await _contx.SaveChangesAsync();
                }
                if (userData != null)
                {
                    _contx.UsersSettings.Remove(userData);
                    await _contx.SaveChangesAsync();
                }
                if (user != null)
                {
                    _contx.Users.Remove(user);
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
                var usersCount = await _contx.Users.CountAsync();

                var user = await _contx.Users.ToListAsync();
                var userData = await _contx.UsersSettings.ToListAsync();
                var userLocation = await _contx.UserLocations.ToListAsync();
                var userAchievements = await _contx.UserAchievements.ToListAsync();
                var userPurchases = await _contx.Transaction.ToListAsync();
                var userBalances = await _contx.Balances.ToListAsync();
                var userNotifications = await _contx.Notifications.ToListAsync();
                var userNotifications1 = await _contx.Notifications.ToListAsync();
                var sponsorRatings = await _contx.SponsorRatings.ToListAsync();

                _contx.UserLocations.RemoveRange(userLocation);
                _contx.UserAchievements.RemoveRange(userAchievements);
                _contx.Balances.RemoveRange(userBalances);
                _contx.Transaction.RemoveRange(userPurchases);
                _contx.Notifications.RemoveRange(userNotifications);
                _contx.Notifications.RemoveRange(userNotifications1);
                _contx.SponsorRatings.RemoveRange(sponsorRatings);
                _contx.UsersSettings.RemoveRange(userData);
                _contx.Users.RemoveRange(user);

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
                var ach = await _contx.Achievements.ToListAsync();

                if (ach != null)
                    _contx.Achievements.RemoveRange(ach);

                await _contx.Achievements.AddRangeAsync(achievements);
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
                await _contx.Achievements.AddRangeAsync(achievements);
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
                    _contx.UserAchievements.RemoveRange(await _contx.UserAchievements.ToListAsync());
                    await _contx.SaveChangesAsync();
                }

                await Task.Run(async() => {             
                    foreach (var achievement in achievements)
                    {
                        var users = await _contx.UsersData.ToListAsync();
                        foreach (var user in users)
                        {
                            if (achievement.Language == user.Language)
                            {
                                var a = new UserAchievement(achievement.Id, user.Id, achievement.Language, achievement.Name, achievement.Description, achievement.Value, achievement.Language);
                                _contx.UserAchievements.Add(a);
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
                model.Id = (await _contx.DailyTasks.CountAsync()) + 1;
                await _contx.DailyTasks.AddAsync(model);
                await _contx.SaveChangesAsync();

                return 1;
            }
            catch { return 0; }
        }

        public async Task<List<TickRequest>> GetTickRequestsAsync()
        {
            return await _contx.TickRequests.Take(15)
                .ToListAsync();
        }

        public async Task<TickRequest> GetTickRequestAsync(long? requestId = null)
        {
            if (requestId != null)
            {
                //Returns only new, aborted, failed or changed requests
                return await _contx.TickRequests.Where(r => r.Id == requestId && (r.State == TickRequestStatus.Added 
                || r.State == TickRequestStatus.Changed 
                || r.State == TickRequestStatus.Aborted 
                || r.State == TickRequestStatus.Failed))
                    .Include(r => r.User)
                    .SingleOrDefaultAsync();
            }

            //Return any request if id wasnt supplied. (Method is used on the frontend)
            var request = await _contx.TickRequests.Where(r => r.State == TickRequestStatus.Added || r.State == TickRequestStatus.Changed || r.State == TickRequestStatus.Aborted)
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
            var request = await _contx.TickRequests.Where(r => r.Id == model.Id && (r.State == TickRequestStatus.InProcess))
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
                    ReceiverId = request.UserId,
                    Severity = Severities.Urgent,
                    Section = Section.Neutral,
                });
            else
                await _userRep.AddUserNotificationAsync(new Entities.UserActionEntities.UserNotification
                {
                    Description = $"Sorry, your identity confirmation request had been denied.\n{model.Comment}",
                    ReceiverId = request.UserId,
                    Severity = Severities.Urgent,
                    Section = Section.Neutral,
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
                    var existingTest = await _contx.Tests.Where(t => t.Id == model.Id)
                        .SingleOrDefaultAsync();

                    if (existingTest != null)
                    {
                        if (existingTest.Language == model.ClassLocalisationId)
                            //Continue if test version already exists.
                            //It allows to avoid constantly changing source file in tools
                            continue;

                        testId = model.Id;
                    }
                    else
                        testId = await _contx.Tests.CountAsync() + 1;

                    var lastQuestionId = await _contx.TestsQuestions.CountAsync();
                    var lastAnswerId = await _contx.TestsAnswers.CountAsync();
                    var lastResultId = await _contx.TestsResults.CountAsync();

                    var results = new List<TestResult>();
                    var questions = new List<TestQuestion>();
                    var answers = new List<TestAnswer>();
                    var test = new Test
                    {
                        Id = testId,
                        Language = model.ClassLocalisationId,
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
                            Language = test.Language,
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
                            Language = test.Language,
                            Tags = result.Tags
                        });
                    }

                    await _contx.Tests.AddAsync(test);
                    await _contx.TestsQuestions.AddRangeAsync(questions);
                    await _contx.TestsAnswers.AddRangeAsync(answers);
                    await _contx.TestsResults.AddRangeAsync(results);
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
            var tickRequests = await _contx.TickRequests
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

        public async Task<bool> AbortTickRequestAsync(long requestId)
        {
            try
            {
                //Get request if it was marked as processed
                var request = await _contx.TickRequests.Where(r => r.Id == requestId && (r.State == TickRequestStatus.InProcess))
                        .SingleOrDefaultAsync(); ;

                if (request == null)
                    throw new NullReferenceException($"Request {requestId} is not in process rigt now");

                request.State = TickRequestStatus.Aborted;
                await _contx.SaveChangesAsync();

                return true;
            }
            catch { return false; }
        }

        public async Task<bool> NotifyFailierTickRequestAsync(long requestId, long adminId)
        {
            try
            {
                var request = await _contx.TickRequests.Where(r => r.Id == requestId)
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
            return await _contx.Users.Where(u => u.IsBanned && u.BanDate != null)
                .Select(u => u.Id)
                .ToListAsync();
        }

        //public Task<long> UploadInTest(UploadInTest model)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
