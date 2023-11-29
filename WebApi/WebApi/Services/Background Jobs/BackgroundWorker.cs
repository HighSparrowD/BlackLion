using Microsoft.Extensions.DependencyInjection;
using WebApi.Entities.UserActionEntities;
using Microsoft.Extensions.Hosting;
using WebApi.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Enums;
using WebApi.App_GlobalResources;

namespace WebApi.Services.Background
{
    public class BackgroundWorker : BackgroundService
    {
        private int BatchSize => 100;

        private readonly IServiceProvider _services;

        public BackgroundWorker(IServiceProvider serviceProvider)
        {
            _services = serviceProvider;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //await CorrectTimerAsync();
                await CorrectTimerFasterAsync();
                await Beat();
            }
        }

        //TODO: manage multipliers
        public async Task CorrectTimerAsync()
        {
            var now = DateTime.UtcNow;

            var differenceHours = 23 - now.Hour;
            var differenceMinutes = 59 - now.Minute;
            var differenceSeconds = 59 - now.Second;

            var timespan = new TimeSpan(differenceHours, differenceMinutes, differenceSeconds);

            Console.WriteLine($"UTC Time now is {now}");
            Console.WriteLine($"Correcting by timespan: {timespan}");
            await Task.Delay(timespan);
        }

        // Debugging method
        public async Task CorrectTimerFasterAsync()
        {
            await Task.Delay(10000);
        }

        public async Task Beat()
        {
            await using var scope = _services.CreateAsyncScope();

            var backgroundRepo = scope.ServiceProvider.GetRequiredService<IBackgroundRepository>();
            var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            var now = DateTime.UtcNow;

            var isMoreToTake = true;

            // Remove use streaks
            await backgroundRepo.RemoveStreakAsync();

            while (isMoreToTake)
            {
                var batch = await backgroundRepo.GetBatchToUpdate(BatchSize);
                isMoreToTake = batch.Count == 100;

                for (int i = 0; i < batch.Count; i++)
                {
                    var user = batch[i];

                    user.HadReceivedReward = false;
                    user.ReportCount = 0;
                    user.ProfileViewsCount = 0;
                    user.RTViewsCount = 0;
                    user.TagSearchesCount = 0;
                    user.AdventureSearchCount = 0;

                    //TODO: consider if has premium
                    user.MaxProfileViewsCount = 50;
                    user.MaxRTViewsCount = 25;
                    user.MaxTagSearchCount = 3;
                    user.MaxAdventureSearchCount = 15;

                    user.Settings.IsFree = null;

                    //Random achievements
                    var achievements = await userRepo.GetRandomAchievements(user.Id);

                    if (achievements.Count > 0)
                    {
                        await userRepo.AddUserNotificationAsync(new UserNotification
                        {
                            
                            Description = Resources.ResourceManager.GetString("RandomAchievements_Message") + string.Join(".", achievements),
                            UserId = user.Id,
                            Type = NotificationType.Other,
                            Section = Section.Neutral
                        });
                    }

                    if (user.PremiumExpirationDate != null)
                    {
                        var untillExpiration = user.PremiumExpirationDate.Value.Subtract(now);
                        if(untillExpiration.TotalHours < 24)
                        {
                            if(untillExpiration.TotalHours > 18)
                            {
                                //TODO: Enhance something else
                                //Enhance user stats
                                user.ShouldEnhance = true && user.PremiumDuration >= 30;
                            }
                            else
                            {
                                user.ShouldEnhance = false;
                                user.PremiumDuration = null;
                            }

                            //Notify user that his premium is about to expire
                            await userRepo.AddUserNotificationAsync(new UserNotification
                            {
                                Type = NotificationType.PremiumEnd,
                                UserId = user.Id,
                                Description = "Your premium access ends today !"
                            });
                        }
                    }

                    user.Statistics.UseStreak++;

                    switch (user.Statistics.UseStreak)
                    {   case 5:
                            await userRepo.GrantAchievementAsync(user.Id, 26);
                            break;
                        case 10:
                            await userRepo.GrantAchievementAsync(user.Id, 27);
                            break;
                        case 20:
                            await userRepo.GrantAchievementAsync(user.Id, 28);
                            break;
                    }

                    user.IsUpdated = true;
                    await backgroundRepo.SaveBatchChanges(batch);
                }
            }

            //Remove old transactions
            await backgroundRepo.DeleteOldTransactionsAsync();

            //Remove old encounters
            await backgroundRepo.DeleteOldEncountersAsync();

            //Remove old feedbacks
            await backgroundRepo.DeleteOldFeedbacksAsync();

            //Remove old reports
            await backgroundRepo.DeleteOldReportsAsync();

            // Remove old requests
            await backgroundRepo.DeleteOldRequestsAsync();
            
            //Remove old deleted users
            await backgroundRepo.DeleteOldUsersAsync();

            //Remove old deleted adventures and their reports
            await backgroundRepo.DeleteOldAdventuresWithReportsAsync();
        }
    }
}
