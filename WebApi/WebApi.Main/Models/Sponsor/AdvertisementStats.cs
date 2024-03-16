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

        public int ViewCount { get; set; }

        public int AverageStayInSeconds { get; set; }

        public float Payback { get; set; }

        public float PricePerClick { get; set; }

        public float TotalPrice { get; set; }

        public float Income { get; set; }

        public int ClickCount { get; set; }

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
                ClickCount = advertisement.ClickCount,
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
                ClickCount = advertisement.ClickCount,
                Income = advertisement.Income,
                Payback = advertisement.Payback,
                PricePerClick = advertisement.PricePerClick,
                TargetAudience = advertisement.TargetAudience,
                TotalPrice = advertisement.TotalPrice,
                ViewCount = advertisement.ViewCount,
                Created = advertisement.Created
            };
        }

        public static implicit operator models.AdvertisementStatsShort?(AdvertisementStats? advertisement)
        {
            if (advertisement == null)
                return null;

            return new models.AdvertisementStatsShort
            {
                Id = advertisement.Id,
                AdvertisementId = advertisement.AdvertisementId,
                AverageStayInSeconds = advertisement.AverageStayInSeconds,
                ClickCount = advertisement.ClickCount,
                Income = advertisement.Income,
                Payback = advertisement.Payback,
                PricePerClick = advertisement.PricePerClick,
                TargetAudience = advertisement.TargetAudience,
                TotalPrice = advertisement.TotalPrice,
                ViewCount = advertisement.ViewCount,
                Created = advertisement.Created.ToString("dd.MM")
            };
        }
    }
}
