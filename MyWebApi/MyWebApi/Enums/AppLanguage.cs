using System.ComponentModel.DataAnnotations;

namespace WebApi.Enums;

public enum AppLanguage : byte
{
    [Display(Name = "Eng")]
    Eng = 0,
    [Display(Name = "Rus")]
    Rus = 1,
    [Display(Name = "Ukr")]
    Ukr = 2
}
