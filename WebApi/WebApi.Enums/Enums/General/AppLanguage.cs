using System.ComponentModel.DataAnnotations;

namespace WebApi.Enums.Enums.General;

public enum AppLanguage : byte
{
    [Display(Name = "EN")]
    EN = 1,
    [Display(Name = "RU")]
    RU = 2,
    [Display(Name = "UK")]
    UK = 3
}
