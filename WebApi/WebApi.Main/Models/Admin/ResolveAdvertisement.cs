using System.Text.Json.Serialization;
using WebApi.Enums.Enums.Advertisement;

#nullable enable
namespace WebApi.Main.Models.Admin;

public class ResolveAdvertisement
{
	[JsonPropertyName("id")]
	public long Id { get; set; }

	[JsonPropertyName("adminId")]
	public long AdminId { get; set; }

	[JsonPropertyName("status")]
	public AdvertisementStatus Status { get; set; }

	[JsonPropertyName("comment")]
	public string? Comment { get; set; }
}
