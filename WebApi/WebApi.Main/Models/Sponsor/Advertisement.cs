﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using WebApi.Enums.Enums.Advertisement;
using WebApi.Enums.Enums.Media;
using WebApi.Enums.Enums.Sponsor;
using WebApi.Main.Models.Sponsor;
using WebApi.Models.Models.Sponsor;
using models = WebApi.Models.Models.Sponsor;

#nullable enable
namespace WebApi.Main.Entities.Sponsor;

public class Advertisement
{
    [Key]
    public long Id { get; set; }
    [ForeignKey("User")]
    public long UserId { get; set; }
    public string? Name { get; set; }
    public string? Text { get; set; }
    public string? TargetAudience { get; set; }
    public string? Media { get; set; }
    public bool Show { get; set; }
    public bool Updated { get; set; }
    [MaybeNull]
    public DateTime? Deleted { get; set; }
    public AdvertisementPriority Priority { get; set; }
    public MediaType MediaType { get; set; }

    public AdvertisementStatus Status { get; set; }

    public long? AdminId { get; set; }

    public virtual User.User? User { get; set; }
    
    public virtual List<AdvertisementStats>? AdvertisementStats { get; set; }

    public virtual List<AdvertisementTag>? Tags { get; set; }

    public Advertisement()
    {}

    public Advertisement(models.AdvertisementNew model)
    {
        UserId = model.SponsorId;
        Name = model.Name;
        Text = model.Text;
        TargetAudience = model.TargetAudience;
        Media = model.Media;
        MediaType = model.MediaType;
        Show = true;
        Updated = true;
        Deleted = null;
        AdminId = null;
        Priority = model.Priority;
        Status = AdvertisementStatus.ToView;
    }

    public void Update(AdvertisementUpdate model)
    {
        Name = model.Name;
        Text = model.Text;
        TargetAudience = model.TargetAudience;
        Media = model.Media;
        MediaType = model.MediaType;
        Show = true;
        Updated = true;
        AdminId = null;
        Priority = model.Priority;
        Status = AdvertisementStatus.ToView;
    }

    public static explicit operator Advertisement?(models.Advertisement? advertisement)
    {
        if (advertisement == null)
            return null;

        return new Advertisement
        {
            Id = advertisement.Id,
            UserId = advertisement.UserId,
            Name = advertisement.Name,
            Text = advertisement.Text,
            Media = advertisement.Media,
            MediaType = advertisement.MediaType,
            Show = advertisement.Show,
            Priority = advertisement.Priority,
            TargetAudience = advertisement.TargetAudience,
            Updated = advertisement.Updated,
            AdminId = advertisement.AdminId,
            Status = advertisement.Status
        };
    }

    public static implicit operator models.Advertisement?(Advertisement? advertisement)
    {
        if (advertisement == null)
            return null;

        var model = new models.Advertisement
        {
            Id = advertisement.Id,
            UserId = advertisement.UserId,
            Name = advertisement.Name,
            Text = advertisement.Text,
            Media = advertisement.Media,
            MediaType = advertisement.MediaType,
            Show = advertisement.Show,
            Priority = advertisement.Priority,
            TargetAudience = advertisement.TargetAudience,
            Updated = advertisement.Updated,
            AdminId = advertisement.AdminId,
            Status = advertisement.Status
        };

        if (advertisement.Tags != null)
            model.Tags = advertisement.Tags.Select(t => t.Tag.Text).ToArray();

        return model;
    }

    public static implicit operator models.AdvertisementItem?(Advertisement? advertisement)
    {
        if (advertisement == null)
            return null;

        return new models.AdvertisementItem
        {
            Id = advertisement.Id,
            Name = advertisement.Name
        };
    }
}