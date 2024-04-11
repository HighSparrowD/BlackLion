using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Interfaces;
using WebApi.Interfaces.Services;
using WebApi.Models.Models.Admin;
using WebApi.Models.Models.Report;
using models = WebApi.Models.Models;

namespace WebApi.Services.Admin
{
    public class AdminService : IAdminService
    {
        private IServiceProvider _serviceProvider { get; set; }

        private IUserRepository _userRepo { get; set; }

        public AdminService(IServiceProvider serviceProvider, IUserRepository userRepo)
        {
            _serviceProvider = serviceProvider;
            _userRepo = userRepo;
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

        public async Task<TickRequest> GetTickRequestsAsync()
        {
            var request = await _userRepo.GetTickRequestAsync();

            return (TickRequest) request;
        }
    }
}
