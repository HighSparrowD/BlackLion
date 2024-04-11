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
using entities = WebApi.Main.Entities.Sponsor;

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

        public async Task<Advertisement> AddAdvertisementAsync(AdvertisementNew model)
        {
            var advertisement = new entities.Advertisement(model);
            await _contx.Advertisements.AddAsync(advertisement);

            await _contx.SaveChangesAsync();

            return (Advertisement)advertisement;
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

        public async Task<List<AdvertisementEconomyStats>> GetAdvertisementEconomyStatsAsync(long userId, AdvertisementStatsRequest searchModel, long? addId = null)
        {
            var query = _contx.AdvertisementStatistics.Where(a => a.SponsorId == userId);
            query = GetTimedQuery(query, searchModel);

            if(addId == null)
            {
                var result = await query.GroupBy(a => a.Created)
                    .Select(a => new
                    {
                        Created = a.Key.ToString("dd.MM"),
                        AdvertisementId = a.Select(s => s.AdvertisementId).FirstOrDefault(),
                        Income = a.Sum(s => s.Income),
                        Payback = a.Sum(s => s.Payback),
                        PricePerClick = a.Sum(s => s.PricePerClick),
                        TotalPrice = a.Sum(s => s.TotalPrice),
                        EntityCount = a.Count()
                    })
                    .Select(a => new AdvertisementEconomyStats
                    {
                        Created = a.Created,
                        AdvertisementId = a.AdvertisementId,
                        Income = a.Income,
                        Payback = a.TotalPrice - a.Income,
                        PricePerClick = a.PricePerClick / a.EntityCount,
                        TotalPrice = a.TotalPrice

                    }).ToListAsync();

                return result;
            }


            return await query.Select(a => (AdvertisementEconomyStats)a)
                .ToListAsync();
        }

		public async Task<List<AdvertisementEngagementStats>> GetAdvertisementEngagementStatsAsync(long userId, AdvertisementStatsRequest searchModel, long? addId = null)
		{
			var query = _contx.AdvertisementStatistics.Where(a => a.SponsorId == userId);

			query = GetTimedQuery(query, searchModel);

            if (addId == null)
            {
                var result = await query.GroupBy(a => a.Created)
                    .Select(a => new
                    {
                        Created = a.Key.ToString("dd.MM"),
                        AdvertisementId = a.Select(s => s.AdvertisementId).FirstOrDefault(),
                        StayInSeconds = a.Sum(s => s.AverageStayInSeconds),
                        LinkClickCount = a.Sum(s => s.LinkClickCount),
                        PeoplePercentage = a.Sum(s => s.PeoplePercentage),
                        ViewCount = a.Sum(s => s.ViewCount),
                        EntityCount = a.Count()
                    })
                    .Select(a => new AdvertisementEngagementStats
                    {
                        Created = a.Created,
                        AdvertisementId = a.AdvertisementId,
                        AverageStayInSeconds = a.StayInSeconds / a.EntityCount,
                        PeoplePercentage = a.PeoplePercentage / a.EntityCount,
                        LinkClickCount = a.LinkClickCount,
                        ViewCount = a.ViewCount
                    }).ToListAsync();

                return result;
            }

            return await query.Select(a => (AdvertisementEngagementStats)a)
				.ToListAsync();
		}

		private IQueryable<entities.AdvertisementStats> GetTimedQuery(IQueryable<entities.AdvertisementStats> query, 
            AdvertisementStatsRequest searchModel)
        {
            return query.Where(a => a.Created >= searchModel.From && a.Created <= searchModel.To);
        }
	}
}
