using WebApi.Enums.Enums.Media;
using WebApi.Enums.Enums.Sponsor;

#nullable enable
namespace WebApi.Models.Models.Sponsor;

public class AdvertisementUpdate
{
    public int Id { get; set; }
    public long SponsorId { get; set; }
    public string? Text { get; set; }
    public string? TargetAudience { get; set; }
    public string? Media { get; set; }
    public AdvertisementPriority Priority { get; set; }
    public MediaType MediaType { get; set; }
}
