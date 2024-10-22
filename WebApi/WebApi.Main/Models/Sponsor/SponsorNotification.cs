﻿using System.ComponentModel.DataAnnotations;

namespace WebApi.Main.Entities.Sponsor;

public class SponsorNotification
{
    [Key]
    public long Id { get; set; }
    public long SponsorId { get; set; }
    public short NotificationReason { get; set; }
    public string Description { get; set; }
    public virtual Sponsor Reciever { get; set; }
}
