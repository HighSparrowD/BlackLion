using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Entities.AchievementEntities;
using WebApi.Entities.AdminEntities;
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
using WebApi.Entities.SystemEntitires;
using Microsoft.OpenApi.Validations;

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
                if (dbCities.Any(c => c.Id == city.Id && c.CountryLang == city.Lang))
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
                if (dbCountries.Any(c => c.Id == country.Id && c.Lang == country.Lang))
                    continue;

                await _contx.Countries.AddAsync(new Country
                {
                    Id = country.Id,
                    CountryName = country.CountryName.ToLower(),
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
                if (dbLangs.Any(c => c.Id == lang.Id && c.Lang == lang.Lang))
                    continue;

                await _contx.Languages.AddAsync(new Language
                {
                    Id = lang.Id,
                    LanguageName = lang.LanguageName.ToLower(),
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

        public async Task AddAchievementsAsync(List<UploadAchievement> achievements)
        {
            var existingAchievement = await _contx.Achievements.Select(a => new { Id = a.Id, Lang = a.Language})
                .ToListAsync();

            var achievementList = new List<Achievement>();

            foreach (var ach in achievements)
            {
                if (!existingAchievement.Contains(new { Id = ach.Id, Lang = ach.Language }) )
                    achievementList.Add(new Achievement(ach));
            }

            await _contx.AddRangeAsync(achievementList);
            await _contx.SaveChangesAsync();

            await UpdateUserAchievementsAsync(achievementList);
        }

        //Used when new achievements added to DB
        private async Task UpdateUserAchievementsAsync(List<Achievement> achievements, bool shouldDeleteAll=false)
        {
            if (shouldDeleteAll)
            {
                _contx.UserAchievements.RemoveRange(await _contx.UserAchievements.ToListAsync());
                await _contx.SaveChangesAsync();
            }
         
            foreach (var achievement in achievements)
            {
                var users = await _contx.UserData.Select(u => new {Id = u.Id, Lang = u.Language})
                    .ToListAsync();

                foreach (var user in users)
                {
                    if (achievement.Language == user.Lang)
                    {
                        var a = new UserAchievement(achievement.Id, user.Id, achievement.Language);
                        await _contx.UserAchievements.AddAsync(a);
                        await _contx.SaveChangesAsync();
                    }
                }
            }
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
                    UserId = request.UserId,
                    Type = NotificationType.TickRequest,
                    Section = Section.Neutral,
                });
            else
                await _userRep.AddUserNotificationAsync(new Entities.UserActionEntities.UserNotification
                {
                    Description = $"Sorry, your identity confirmation request had been denied.\n{model.Comment}",
                    UserId = request.UserId,
                    Type = NotificationType.TickRequest,
                    Section = Section.Neutral,
                });

            return model.IsAccepted;
        }

        public async Task<byte> UploadTestsAsync(List<UploadTest> tests)
        {
            try
            {
                long testId = 0;

                for (int i = 0; i < tests.Count; i++)
                {
                    var model = tests[i];

                    //Check if a version of the test already exists
                    var existingTest = await _contx.Tests.Where(t => t.Id == model.Id)
                        .FirstOrDefaultAsync();

                    if (existingTest != null)
                    {
                        if (existingTest.Language == model.Language)
                            //Continue if test version already exists.
                            //It allows to remove the necessity of constant change of the source file in tools
                            continue;
                    }

                    testId = model.Id;

                    var test = new Test
                    {
                        Id = testId,
                        Language = model.Language,
                        Name = model.Name,
                        Description = model.Description,
                        TestType = model.TestType,
                        Price = model.Price,
                        CanBePassedInDays = model.CanBePassedInDays
                    };

                    await _contx.Tests.AddAsync(test);

                    foreach (var question in model.Questions)
                    {
                        var q = new TestQuestion
                        {
                            TestLanguage = test.Language,
                            TestId = testId,
                            Scale = question.Scale,
                            Text = question.Text,
                            Photo = question.Photo
                        };

                        await _contx.TestsQuestions.AddAsync(q);

                        foreach (var answer in question.Answers)
                        {
                            _contx.TestsAnswers.Add(new TestAnswer
                            {
                                Text = answer.Text,
                                Value = answer.Value,
                                TestQuestionId = q.Id,
                                Tags = string.IsNullOrEmpty(answer.Tags)? null : await _userRep.AddTagsAsync(answer.Tags, TagType.Tests)
                            });
                        }
                    }

                    foreach (var result in model.Results)
                    {
                        await _contx.TestsResults.AddAsync(new TestResult
                        {
                            Result = result.Result,
                            Score = result.Score,
                            TestId = test.Id,
                            TestLanguage = test.Language,
                            Tags = string.IsNullOrEmpty(result.Tags) ? null : await _userRep.AddTagsAsync(result.Tags, TagType.Tests)
                        });
                    }

                    foreach (var scale in model.Scales)
                    {
                        await _contx.TestsScales.AddAsync(new TestScale
                        {
                            TestId = test.Id,
                            TestLanguage = test.Language,
                            Scale = scale.Scale,
                            MinValue = scale.MinValue
                        });
                    }

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
