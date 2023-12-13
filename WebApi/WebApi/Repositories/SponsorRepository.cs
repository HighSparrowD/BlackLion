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
            var advertisements = await _contx.Advertisements.Where(a => a.SponsorId == sponsorId)
                .AsNoTracking()
                .Select(a => (AdvertisementItem)a)
                .ToListAsync();

            return advertisements;
        }

        public async Task<Models.Models.Sponsor.Advertisement> GetAdvertisementAsync(int advertisementId)
        {
            var advertisement = await _contx.Advertisements.Where(a => a.SponsorId == advertisementId && a.Deleted == null)
                .AsNoTracking()
                .Select(a => (Models.Models.Sponsor.Advertisement)a)
                .FirstOrDefaultAsync();

            return advertisement;
        }

        public async Task AddAdvertisementAsync(AdvertisementNew model)
        {
            var advertisement = new entities.Advertisement(model);

            await _contx.AddAsync(advertisement);
            await _contx.SaveChangesAsync();
        }

        public async Task UpdateAdvertisementAsync(AdvertisementUpdate model)
        {
            var advertisement = await _contx.Advertisements.Where(a => a.SponsorId == model.Id && a.Deleted == null)
                .FirstOrDefaultAsync();

            advertisement.Update(model);
            await _contx.SaveChangesAsync();
        }

        public async Task DeleteAdvertisementAsync(int advertisementId)
        {
            var advertisement = await _contx.Advertisements.Where(a => a.SponsorId == advertisementId && a.Deleted == null)
                .FirstOrDefaultAsync();

            advertisement.Deleted = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            await _contx.SaveChangesAsync();
        }

        public List<GetLocalizedEnum> GetPrioritiesAsync()
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
            var advertisement = await _contx.Advertisements.Where(a => a.SponsorId == advertisementId && a.Deleted == null)
                .FirstOrDefaultAsync();

            advertisement.Priority = priority;
            await _contx.SaveChangesAsync();
        }

        public async Task SwitchShowStatusAsync(int advertisementId)
        {
            var advertisement = await _contx.Advertisements.Where(a => a.SponsorId == advertisementId && a.Deleted == null)
                .FirstOrDefaultAsync();

            advertisement.Show = !advertisement.Show;
            await _contx.SaveChangesAsync();
        }
    }
}
