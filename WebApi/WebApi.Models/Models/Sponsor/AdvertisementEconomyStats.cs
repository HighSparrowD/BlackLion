﻿using System.Text.Json.Serialization;

namespace WebApi.Models.Models.Sponsor
{
	public class AdvertisementEconomyStats
	{
		[JsonPropertyName("id")]
		public int Id { get; set; }

		[JsonPropertyName("advertisementId")]
		public long AdvertisementId { get; set; }

		[JsonPropertyName("payback")]
		public float Payback { get; set; }

		[JsonPropertyName("pricePerClick")]
		public float PricePerClick { get; set; }

		[JsonPropertyName("totalPrice")]
		public float TotalPrice { get; set; }

		[JsonPropertyName("income")]
		public float Income { get; set; }

		[JsonPropertyName("created")]
		public string Created { get; set; }
	}
}
