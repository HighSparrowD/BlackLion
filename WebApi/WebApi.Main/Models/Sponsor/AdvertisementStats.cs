using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using models = WebApi.Models.Models.Sponsor;

#nullable enable
namespace WebApi.Main.Models.Sponsor
{
    public class AdvertisementStats
    {
        [Key]
        public int Id { get; set; }

        public long SponsorId { get; set; }

        [ForeignKey("Advertisement")]
        public long AdvertisementId { get; set; }

		// Economic metrics
		public float Payback { get; set; }

        public float PricePerClick { get; set; }

        public float TotalPrice { get; set; }

        public float Income { get; set; }

		// Engagement metrics
		public int LinkClickCount { get; set; }

        public float PeoplePercentage { get; set; }

        public int ViewCount { get; set; }

        public int AverageStayInSeconds { get; set; }


        public string? TargetAudience { get; set; }

        public DateOnly Created { get; set; }

        public virtual Advertisement? Advertisement { get; set; }

        public static explicit operator AdvertisementStats?(models.AdvertisementStats? advertisement)
        {
            if (advertisement == null)
                return null;

            return new AdvertisementStats
            {
                Id = advertisement.Id,
                AdvertisementId = advertisement.AdvertisementId,
                AverageStayInSeconds = advertisement.AverageStayInSeconds,
                LinkClickCount = advertisement.ClickCount,
                Income = advertisement.Income,
                Payback = advertisement.Payback,
                PricePerClick = advertisement.PricePerClick,
                TargetAudience = advertisement.TargetAudience,
                TotalPrice = advertisement.TotalPrice,
                ViewCount = advertisement.ViewCount,
                Created = advertisement.Created
            };
        }

        public static implicit operator models.AdvertisementStats?(AdvertisementStats? advertisement)
        {
            if (advertisement == null)
                return null;

            return new models.AdvertisementStats
            {
                Id = advertisement.Id,
                AdvertisementId = advertisement.AdvertisementId,
                AverageStayInSeconds = advertisement.AverageStayInSeconds,
                ClickCount = advertisement.LinkClickCount,
                Income = advertisement.Income,
                Payback = advertisement.Payback,
                PricePerClick = advertisement.PricePerClick,
                TargetAudience = advertisement.TargetAudience,
                TotalPrice = advertisement.TotalPrice,
                ViewCount = advertisement.ViewCount,
                Created = advertisement.Created
            };
        }

        public static implicit operator models.AdvertisementEconomyStats?(AdvertisementStats? advertisement)
        {
            if (advertisement == null)
                return null;

            return new models.AdvertisementEconomyStats
            {
                Id = advertisement.Id,
                AdvertisementId = advertisement.AdvertisementId,
                Income = advertisement.Income,
                Payback = advertisement.Payback,
                PricePerClick = advertisement.PricePerClick,
                TotalPrice = advertisement.TotalPrice,
                Created = advertisement.Created.ToString("dd.MM")
            };
        }

		public static implicit operator models.AdvertisementEngagementStats?(AdvertisementStats? advertisement)
		{
			if (advertisement == null)
				return null;

			return new models.AdvertisementEngagementStats
			{
				Id = advertisement.Id,
				AdvertisementId = advertisement.AdvertisementId,
				AverageStayInSeconds = advertisement.AverageStayInSeconds,
                LinkClickCount = advertisement.LinkClickCount,
                PeoplePercentage = advertisement.PeoplePercentage,
                ViewCount = advertisement.ViewCount,
				Created = advertisement.Created.ToString("dd.MM")
			};
		}
	}
}
