using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using WebApi.Enums.Enums.Media;
using WebApi.Enums.Enums.Sponsor;
using WebApi.Models.Models.Sponsor;
using models = WebApi.Models.Models.Sponsor;

#nullable enable
namespace WebApi.Main.Models.Sponsor;

public class Advertisement
{
    [Key]
    public long Id { get; set; }
    [ForeignKey("User")]
    public long UserId { get; set; }
    public string? Text { get; set; }
    public string? TargetAudience { get; set; }
    public string? Media { get; set; }
    public bool Show { get; set; }
    public bool Updated { get; set; }
    [MaybeNull]
    public DateTime? Deleted { get; set; }
    public AdvertisementPriority Priority { get; set; }
    public MediaType MediaType { get; set; }

    public User.User? User { get; set; }

    public Advertisement()
    {}

    public Advertisement(models.AdvertisementNew model)
    {
        UserId = model.SponsorId;
        Text = model.Text;
        TargetAudience = model.TargetAudience;
        Media = model.Media;
        MediaType = model.MediaType;
        Show = true;
        Updated = true;
        Deleted = null;
        Priority = model.Priority;
    }

    public void Update(AdvertisementUpdate model)
    {
        Text = model.Text;
        TargetAudience = model.TargetAudience;
        Media = model.Media;
        MediaType = model.MediaType;
        Show = true;
        Updated = true;
        Priority = model.Priority;
    }

    //public static string TruncateDescription(string text, int leng)
    //{
    //    string description = "";

    //    foreach (char c in text)
    //    {
    //        if (description.Length + 1 <= leng)
    //        {
    //            description += c;
    //        }
    //    }

    //    return description;
    //}

    public static explicit operator Advertisement?(models.Advertisement? advertisement)
    {
        if (advertisement == null)
            return null;

        return new Advertisement
        {
            Id = advertisement.Id,
            UserId = advertisement.UserId,
            Text = advertisement.Text,
            Media = advertisement.Media,
            MediaType = advertisement.MediaType,
            Priority = advertisement.Priority,
            TargetAudience = advertisement.TargetAudience,
            Updated = advertisement.Updated
        };
    }

    public static implicit operator models.Advertisement?(Advertisement? advertisement)
    {
        if (advertisement == null)
            return null;

        return new models.Advertisement
        {
            Id = advertisement.Id,
            UserId = advertisement.UserId,
            Text = advertisement.Text,
            Media = advertisement.Media,
            MediaType = advertisement.MediaType,
            Priority = advertisement.Priority,
            TargetAudience = advertisement.TargetAudience,
            Updated = advertisement.Updated
        };
    }

    public static implicit operator models.AdvertisementItem?(Advertisement? advertisement)
    {
        if (advertisement == null)
            return null;

        return new models.AdvertisementItem
        {
            Id = advertisement.Id,
            Text = advertisement.Text
        };
    }
}