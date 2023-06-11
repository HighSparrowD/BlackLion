using System.ComponentModel.DataAnnotations;
namespace WebApi.Enums;

public enum UsageReason : byte
{
    [Display(Name = "UsageReason_Relationship")]
    Relationship = 0,
    [Display(Name = "UsageReason_Friendship")]
    Friendship = 1,
    //[Display(Name = "UsageReason_Sex")]
    //Sex = 2,
    [Display(Name = "UsageReason_NoMatter")]
    NoMatter = 3
}
