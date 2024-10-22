﻿#nullable enable
using System.Text.Json.Serialization;

namespace WebApi.Models.Models.Sponsor
{
    public class AdvertisementStats
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("advertisementId")]
        public long AdvertisementId { get; set; }

        [JsonPropertyName("viewCount")]
        public int ViewCount { get; set; }

        [JsonPropertyName("averageStayInSeconds")]
        public int AverageStayInSeconds { get; set; }

        [JsonPropertyName("payback")]
        public float Payback { get; set; }

        [JsonPropertyName("pricePerClick")]
        public float PricePerClick { get; set; }

        [JsonPropertyName("totalPrice")]
        public float TotalPrice { get; set; }

        [JsonPropertyName("income")]
        public float Income { get; set; }

        [JsonPropertyName("clickCount")]
        public int ClickCount { get; set; }

        [JsonPropertyName("targetAudience")]
        public string? TargetAudience { get; set; }

        [JsonPropertyName("created")]
        public DateOnly Created { get; set; }
    }
}
