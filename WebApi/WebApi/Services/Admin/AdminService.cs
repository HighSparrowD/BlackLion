using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Interfaces;
using WebApi.Interfaces.Services;

namespace WebApi.Services.Admin
{
    public class AdminService : IAdminService
    {
        private IServiceProvider _serviceProvider { get; set; }

        public AdminService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartInDebug(List<long> userIds)
        {
            var userRepo = _serviceProvider.GetRequiredService<IUserRepository>();

            await userRepo.RemoveAllEncountersAsync();
            await userRepo.SetAllBusyStatusToFalse();
            await userRepo.AssignAdminRightsAsync(userIds);
            await userRepo.AssignSponsorRightsAsync(userIds);
        }
    }
}
