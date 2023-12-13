using System.ComponentModel.DataAnnotations;

namespace WebApi.Enums.Enums.User;

public enum CommunicationPreference : byte
{
    [Display(Name = "Online")]
    Online = 1,
    [Display(Name = "Offline")]
    Offline = 2,
    [Display(Name = "NoMatter")]
    NoMatter = 3
}
