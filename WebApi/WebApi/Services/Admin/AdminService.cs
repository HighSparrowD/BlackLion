using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Enums.Enums.Adventure;
using WebApi.Enums.Enums.Advertisement;
using WebApi.Enums.Enums.General;
using WebApi.Enums.Enums.Notification;
using WebApi.Enums.Enums.User;
using WebApi.Interfaces;
using WebApi.Interfaces.Services;
using WebApi.Main.Entities.Admin;
using WebApi.Main.Entities.User;
using WebApi.Main.Models.Admin;
using WebApi.Models.Models.Admin;
using WebApi.Models.Models.Adventure;
using WebApi.Models.Models.Report;
using WebApi.Models.Models.Sponsor;
using models = WebApi.Models.Models;

namespace WebApi.Services.Admin
{
	public class AdminService : IAdminService
	{
        private IServiceProvider _serviceProvider { get; set; }

        private IUserRepository _userRepo { get; set; }

        private ISponsorRepository _sponsorRepo { get; set; }

        private IAdminRepository _adminRepo { get; set; }

        public AdminService(IServiceProvider serviceProvider, IUserRepository userRepo, IAdminRepository adminRepo, ISponsorRepository sponsorRepo)
        {
            _serviceProvider = serviceProvider;
            _userRepo = userRepo;
            _adminRepo = adminRepo;
            _sponsorRepo = sponsorRepo;
        }

        public async Task StartInDebug(List<long> userIds)
        {
            var userRepo = _serviceProvider.GetRequiredService<IUserRepository>();

            await userRepo.RemoveAllEncountersAsync();
            await userRepo.SetAllBusyStatusToFalse();
            await userRepo.AssignAdminRightsAsync(userIds);
            await userRepo.AssignSponsorRightsAsync(userIds);
        }

        public async Task<List<GroupedFeedback>> GetGroupedFeedbackAsync()
        {            
            var feedbacks = await _userRepo.GetAllFeedbackAsync();

            return feedbacks.GroupBy(f => f.User.UserName)
                .Select(f => new GroupedFeedback(f.Key, f.Select(s => (Feedback)s).ToList()))
                .ToList();
        }

        public async Task<List<Feedback>> GetRecentFeedbackAsync()
        {
            var feedBacks = await _userRepo.GetRecentFeedbackAsync();

            return feedBacks.Select(f => (Feedback)f)
                .ToList();
        }

        public async Task<List<Report>> GetRecentReportsAsync()
        {
            var reports = await _userRepo.GetRecentReportsAsync();

            return reports.Select(f => (Report)f)
                .ToList();
        }

        public async Task<List<models.Admin.VerificationRequest>> GetVerificationRequestsAsync()
        {
            var request = await _adminRepo.GetVerificationRequestAsync();

            return request.Select(r => (models.Admin.VerificationRequest)r).ToList();
        }

		public async Task ResolveVerificationRequestAsync(ResolveVerificationRequest model)
		{
			var request = await _adminRepo.ResolveVerificationRequest(model);

			if (model.Status == VerificationRequestStatus.Approved)
			{
				await _userRepo.AddUserNotificationAsync(new UserNotification
				{
					Description = $"Your identity confirmation request had been approved :)\n{model.Comment}",
					UserId = request.UserId,
					Type = NotificationType.VerificationRequest,
					Section = Section.Neutral,
				});
			}
			else if (model.Status == VerificationRequestStatus.ToView)
			{
				await _userRepo.AddUserNotificationAsync(new UserNotification
				{
					Description = $"Sorry, your identity confirmation request had been denied.\n{model.Comment}",
					UserId = request.UserId,
					Type = NotificationType.VerificationRequest,
					Section = Section.Neutral,
				});
			}
		}

		public async Task<RecentUpdates> GetRecentUpdatesAsync()
        {
            var updates = new RecentUpdates
            {
                PendingAdventureCount = await _userRepo.CountPendingAdventuresAsync(),
                PendingAdvertisementCount = await _userRepo.CountPendingAdvertisementsAsync(),
                RecentFeedbackCount = await _userRepo.CountRecentFeedbacksAsync(),
                RecentReportCount = await _userRepo.CountRecentReportsAsync(),
                VerificationRequestCount = await _userRepo.CountPendingVerificationRequestsAsync()
            };

            return updates;
        }

		public async Task<ICollection<Advertisement>> GetPendingtAdvertisementsAsync()
		{
			var request = await _sponsorRepo.GetPendingAdvertisementsAsync();

			return request.Select(r => (models.Sponsor.Advertisement)r).ToList();
		}

		public async Task<Advertisement> ResolveAdvertisementsAsync(ResolveAdvertisement model)
		{
			var advertisement = await _sponsorRepo.ResolveAdvertisement(model);

			if (!string.IsNullOrEmpty(model.Tags))
			{
				var tags = await _userRepo.AddTagsAsync(model.Tags, Enums.Enums.Tag.TagType.Tags);
				await _sponsorRepo.UpdateTags(model.Id, tags);
			}

			if (model.Status == AdvertisementStatus.Approved)
			{
				await _userRepo.AddUserNotificationAsync(new UserNotification
				{
					Description = $"Your advertisement had been approved :)\n{model.Comment}",
					UserId = advertisement.UserId,
					Type = NotificationType.Other,
					Section = Section.Neutral,
				});
			}
			else if (model.Status == AdvertisementStatus.ToView)
			{
				await _userRepo.AddUserNotificationAsync(new UserNotification
				{
					Description = $"Sorry, your advertisement had been denied.\n\n{model.Comment}",
					UserId = advertisement.UserId,
					Type = NotificationType.Other,
					Section = Section.Neutral,
				});
			}

			return advertisement;
		}

		public async Task<ICollection<Adventure>> GetPendingAdventuresAsync()
		{
			var request = await _userRepo.GetPendingAdventuresAsync();

			return request.Select(r => (models.Adventure.Adventure)r).ToList();
		}

		public async Task<Adventure> ResolveAdventureAsync(ResolveAdventure model)
		{
			var adventure = await _userRepo.ResolveAdventure(model);

			if (model.Status == AdventureStatus.Approved)
			{
				await _userRepo.AddUserNotificationAsync(new UserNotification
				{
					Description = $"Your adventure had been approved :)\n{model.Comment}",
					UserId = adventure.UserId,
					Type = NotificationType.Other,
					Section = Section.Neutral,
				});
			}
			else if (model.Status == AdventureStatus.ToView)
			{
				await _userRepo.AddUserNotificationAsync(new UserNotification
				{
					Description = $"Sorry, your adventure had been denied.\n{model.Comment}",
					UserId = adventure.UserId,
					Type = NotificationType.Other,
					Section = Section.Neutral,
				});
			}

			return (Adventure)adventure;
		}
	}
}
