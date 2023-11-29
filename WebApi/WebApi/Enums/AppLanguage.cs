using System.ComponentModel.DataAnnotations;

namespace WebApi.Enums;

public enum AppLanguage : byte
{
    [Display(Name = "EN")]
    EN = 0,
    [Display(Name = "RU")]
    RU = 1,
    [Display(Name = "UK")]
    UK = 2
}
