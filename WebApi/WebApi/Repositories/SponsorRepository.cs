using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Enums.Enums.Sponsor;
using WebApi.Enums.Enums.Tag;
using WebApi.Interfaces;
using WebApi.Interfaces.Services;
using WebApi.Main.Entities.Admin;
using WebApi.Main.Entities.User;
using WebApi.Main.Models.Admin;
using WebApi.Main.Models.Sponsor;
using WebApi.Models.Models.Sponsor;
using WebApi.Models.Models.User;
using WebApi.Models.Utilities;
using entities = WebApi.Main.Entities.Sponsor;

namespace WebApi.Repositories
{
	public class SponsorRepository : ISponsorRepository
	{
        private UserContext _contx { get; set; }
        private readonly ITimestampService timestamp;

        public SponsorRepository(UserContext context, ITimestampService timestampService)
        {
            _contx = context;
            timestamp = timestampService;
        }

        public async Task<List<AdvertisementItem>> GetAdvertisementListAsync(long sponsorId)
        {
            var advertisements = await _contx.Advertisements.Where(a => a.UserId == sponsorId && a.Deleted == null)
                .AsNoTracking()
                .Select(a => (AdvertisementItem)a)
                .ToListAsync();

            return advertisements;
        }

        public async Task<entities.Advertisement> GetAdvertisementAsync(long advertisementId)
        {
            var advertisement = await _contx.Advertisements
                .FirstOrDefaultAsync(a => a.Id == advertisementId && a.Deleted == null);

            return advertisement;
        }

		public async Task<entities.Advertisement> ResolveAdvertisement(ResolveAdvertisement model)
		{
			var advertisement = await GetAdvertisementAsync(model.Id);

			if (advertisement == null)
				throw new NullReferenceException("Advertisement was not found");

			advertisement.Status = model.Status;
			advertisement.AdminId = model.AdminId;

			await _contx.SaveChangesAsync();

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

        public async Task DeleteAdvertisementAsync(long advertisementId)
        {
            var advertisement = await _contx.Advertisements.Where(a => a.Id == advertisementId && a.Deleted == null)
                .FirstOrDefaultAsync();

            advertisement.Deleted = timestamp.GetUtcNow();
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

        public async Task SetAdvertisementPriorityAsync(long advertisementId, AdvertisementPriority priority)
        {
            var advertisement = await _contx.Advertisements.Where(a => a.Id == advertisementId && a.Deleted == null)
                .FirstOrDefaultAsync();

            advertisement.Priority = priority;
            await _contx.SaveChangesAsync();
        }

        public async Task SwitchShowStatusAsync(long advertisementId)
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

		public async Task<ICollection<entities.Advertisement>> GetPendingAdvertisementsAsync()
		{
			var query = _contx.Advertisements.Where(a => a.Status == Enums.Enums.Advertisement.AdvertisementStatus.ToView && a.Deleted == null)
                .Take(3);

            var advertisements = await query.Include(a => a.Tags)
                .ThenInclude(a => a.Tag)
                .ToListAsync();

            // Set status = InProcess
            await query.ExecuteUpdateAsync(a => a.SetProperty(ad => ad.Status, Enums.Enums.Advertisement.AdvertisementStatus.InProcess));

            return advertisements;
		}

		public async Task UpdateTags(long advertisementId, List<long> tags)
		{
			_contx.AdvertisementTags.RemoveRange(_contx.AdvertisementTags.Where(t => t.AdvertisementId == advertisementId));

			var newTags = tags.Select(t => new AdvertisementTag(t, advertisementId, TagType.Tags));

			await _contx.AdvertisementTags.AddRangeAsync(newTags);

			await _contx.SaveChangesAsync();
		}
	}
}
