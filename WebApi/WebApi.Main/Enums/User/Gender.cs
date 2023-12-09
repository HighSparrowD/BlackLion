using System.ComponentModel.DataAnnotations;

namespace WebApi.Main.Enums.User;

public enum Gender : byte
{
    [Display(Name = "Gender_Male")]
    Male = 1,
    [Display(Name = "Gender_Female")]
    Female = 2,
    [Display(Name = "Gender_NonBinary")]
    NonBinary = 3,
    [Display(Name = "Gender_RatherNotSay")]
    RatherNotSay = 4,
}
