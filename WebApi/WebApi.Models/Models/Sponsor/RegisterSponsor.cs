﻿using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.General;

namespace WebApi.Models.Models.Sponsor;

public class RegisterSponsor
{
    public long Id { get; set; }
    public string Username { get; set; }
    public int Age { get; set; }
    public string CodeWord { get; set; }
    public int UserMaxAdCount { get; set; }
    public int UserMaxAdViewCount { get; set; }
    public int UserAppLanguage { get; set; }
    public int UserCountryId { get; set; }
    public int UserCityId { get; set; }
    public List<AppLanguage> Languages { get; set; }
    public List<string> LanguageLevels { get; set; }
    [MaxLength(255)]
    public string Tel { get; set; }
    [MaxLength(255)]
    public string Email { get; set; }
    [MaxLength(255)]
    public string Instagram { get; set; }
    [MaxLength(255)]
    public string Facebook { get; set; }
}
