#nullable enable
namespace WebApi.Models.Models.Sponsor
{
    public class AdvertisementStats
    {
        public int Id { get; set; }

        public long AdvertisementId { get; set; }

        public int ViewCount { get; set; }

        public int AverageStayInSeconds { get; set; }

        public float Payback { get; set; }

        public float PricePerClick { get; set; }

        public float TotalPrice { get; set; }

        public float Income { get; set; }

        public float ClickCount { get; set; }

        public string? TargetAudience { get; set; }

        public DateOnly Created { get; set; }
    }
}
