using System.ComponentModel.DataAnnotations;
namespace WebApi.Enums.Enums.User;

public enum UsageReason : short
{
    [Display(Name = "UsageReason_Relationship")]
    Relationship = 1,
    [Display(Name = "UsageReason_Friendship")]
    Friendship = 2,
    //[Display(Name = "UsageReason_Sex")]
    //Sex = 2,
    [Display(Name = "UsageReason_Communication")]
    Communication = 3,
    [Display(Name = "UsageReason_NoMatter")]
    NoMatter = 4
}
