using System.Text.Json.Serialization;

namespace WebApi.Models.Models.Sponsor
{
	public class AdvertisementEngagementStats
	{
		[JsonPropertyName("id")]
		public int Id { get; set; }

		[JsonPropertyName("advertisementId")]
		public long AdvertisementId { get; set; }

		[JsonPropertyName("linkClickCount")]
		public int LinkClickCount { get; set; }

		[JsonPropertyName("peoplePercentage")]
		public float PeoplePercentage { get; set; }

		[JsonPropertyName("viewCount")]
		public int ViewCount { get; set; }

		[JsonPropertyName("averageStayInSeconds")]
		public int AverageStayInSeconds { get; set; }

		[JsonPropertyName("created")]
		public string Created { get; set; }
	}
}
