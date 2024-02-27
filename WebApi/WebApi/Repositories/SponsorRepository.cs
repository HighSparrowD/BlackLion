using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Enums.Enums.Sponsor;
using WebApi.Interfaces;
using WebApi.Models.Models.Sponsor;
using WebApi.Models.Models.User;
using WebApi.Models.Utilities;
using entities = WebApi.Main.Models.Sponsor;

namespace WebApi.Repositories
{
    public class SponsorRepository : ISponsorRepository
    {
        private UserContext _contx { get; set; }

        public SponsorRepository(UserContext context)
        {
            _contx = context;
        }

        public async Task<List<AdvertisementItem>> GetAdvertisementListAsync(int sponsorId)
        {
            var advertisements = await _contx.Advertisements.Where(a => a.UserId == sponsorId && a.Deleted == null)
                .AsNoTracking()
                .Select(a => (AdvertisementItem)a)
                .ToListAsync();

            return advertisements;
        }

        public async Task<Models.Models.Sponsor.Advertisement> GetAdvertisementAsync(int advertisementId)
        {
            var advertisement = await _contx.Advertisements.Where(a => a.Id == advertisementId && a.Deleted == null)
                .AsNoTracking()
                .Select(a => (Models.Models.Sponsor.Advertisement)a)
                .FirstOrDefaultAsync();

            return advertisement;
        }

        public async Task AddAdvertisementAsync(AdvertisementNew model)
        {
            var advertisement = new entities.Advertisement(model);
            await _contx.Advertisements.AddAsync(advertisement);

            var stats = new entities.AdvertisementStats
            {
                SponsorId = model.SponsorId,
                AdvertisementId = advertisement.Id
            };

            await _contx.AdvertisementStatistics.AddAsync(stats);
            await _contx.SaveChangesAsync();
        }

        public async Task UpdateAdvertisementAsync(AdvertisementUpdate model)
        {
            var advertisement = await _contx.Advertisements.Where(a => a.Id == model.Id && a.Deleted == null)
                .FirstOrDefaultAsync();

            advertisement.Update(model);
            await _contx.SaveChangesAsync();
        }

        public async Task DeleteAdvertisementAsync(int advertisementId)
        {
            var advertisement = await _contx.Advertisements.Where(a => a.Id == advertisementId && a.Deleted == null)
                .FirstOrDefaultAsync();

            advertisement.Deleted = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            await _contx.SaveChangesAsync();
        }

        public List<GetLocalizedEnum> GetPriorities()
        {
            var reasons = new List<GetLocalizedEnum>();

            foreach (var reason in Enum.GetValues(typeof(AdvertisementPriority)))
            {
                reasons.Add(new GetLocalizedEnum
                {
                    Id = (short)reason,
                    Name = EnumLocalizer.GetLocalizedValue((AdvertisementPriority)reason)
                });
            }

            return reasons;
        }

        public async Task SetAdvertisementPriorityAsync(int advertisementId, AdvertisementPriority priority)
        {
            var advertisement = await _contx.Advertisements.Where(a => a.Id == advertisementId && a.Deleted == null)
                .FirstOrDefaultAsync();

            advertisement.Priority = priority;
            await _contx.SaveChangesAsync();
        }

        public async Task SwitchShowStatusAsync(int advertisementId)
        {
            var advertisement = await _contx.Advertisements.Where(a => a.Id == advertisementId && a.Deleted == null)
                .FirstOrDefaultAsync();

            advertisement.Show = !advertisement.Show;
            await _contx.SaveChangesAsync();
        }

        public async Task<List<AdvertisementStats>> GetAdvertisementStatsAsync(long advertisementId)
        {
            var statistics = await _contx.AdvertisementStatistics.Where(a => a.AdvertisementId == advertisementId)
                .Select(a => (AdvertisementStats)a)
                .ToListAsync();

            return statistics;
        }

        public async Task<List<AdvertisementStats>> GetAllAdvertisementsStatsAsync(long userId)
        {
            var statistics = await _contx.AdvertisementStatistics.Where(a => a.SponsorId == userId)
                .Select(a => (AdvertisementStats)a)
                .ToListAsync();

            return statistics;
        }
    }
}
