using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyWebApi.Entities.UserActionEntities;
using Microsoft.Extensions.Hosting;
using MyWebApi.Data;
using MyWebApi.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using MyWebApi.Enums;

namespace MyWebApi.Services.Background
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
                await CorrectTimerAsync();
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

            Console.WriteLine($"Correcting by timestamp {timespan}");
            await Task.Delay(timespan);
        }

        public async Task Beat()
        {
            await using var scope = _services.CreateAsyncScope();

            var backgroundRepo = scope.ServiceProvider.GetRequiredService<IBackgroundRepository>();
            var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            var now = DateTime.UtcNow;

            var isMoreToTake = true;

            while (isMoreToTake)
            {
                var batch = await backgroundRepo.GetBatchToUpdate(BatchSize);
                isMoreToTake = batch.Count == 100;

                for (int i = 0; i < batch.Count; i++)
                {
                    var user = batch[i];

                    user.HadReceivedReward = false;
                    user.IsFree = false;
                    user.ReportCount = 0;
                    user.ProfileViewsCount = 0;
                    user.RTViewsCount = 0;
                    user.TagSearchesCount = 0;

                    //TODO: consider if has premium
                    user.MaxProfileViewsCount = 50;
                    user.MaxRTViewsCount = 25;
                    user.MaxTagSearchCount = 3;

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
                                Severity = Severities.Urgent,
                                UserId1 = user.UserId,
                                Description = "Your premium access ends today !"
                            });
                        }
                    }

                    user.IsUpdated = true;
                    await backgroundRepo.SaveBatchChanges(batch);
                }
            }
        }
    }
}
