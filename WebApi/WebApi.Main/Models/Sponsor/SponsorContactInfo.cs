﻿using System.ComponentModel.DataAnnotations;

namespace WebApi.Main.Entities.Sponsor;

public class SponsorContactInfo
{
    [Key]
    public long Id { get; set; }
    public long SponsorId { get; set; }
    [MaxLength(255)]
    public string Tel { get; set; }
    [MaxLength(255)]
    public string Email { get; set; }
    [MaxLength(255)]
    public string Instagram { get; set; }
    [MaxLength(255)]
    public string Facebook { get; set; }
}
