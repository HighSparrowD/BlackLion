using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.User;
#nullable enable

namespace WebApi.Models.Models.User;

public class User
{
    [Key]
    public long Id { get; set; }

    public string? UserName { get; set; }
    public long DataId { get; set; }
    public long SettingsId { get; set; }
    public long LocationId { get; set; }
    public bool IsBusy { get; set; }
    public DateTime? BanDate { get; set; }
    public DateTime? DeleteDate { get; set; }
    public DateTime? PremiumExpirationDate { get; set; }
    public bool HadReceivedReward { get; set; }
    public short? PremiumDuration { get; set; }
    public IdentityConfirmationType IdentityType { get; set; }
    public bool ShouldEnhance { get; set; }
    public short ReportCount { get; set; } // How many times this user was reported today
    public short DailyRewardPoint { get; set; }
    public float BonusIndex { get; set; }
    public int InvitedUsersCount { get; set; }
    public float InvitedUsersBonus { get; set; }
    public string? Nickname { get; set; }
    public long? ParentId { get; set; }
    public int ProfileViewsCount { get; set; }
    public int RTViewsCount { get; set; }
    public int AdventureSearchCount { get; set; }
    public int MaxProfileViewsCount { get; set; }
    public int MaxRTViewsCount { get; set; }
    public int MaxTagSearchCount { get; set; }
    public int MaxAdventureSearchCount { get; set; }
    public int TagSearchesCount { get; set; }
    public string? EnteredPromoCodes { get; set; }
    public bool IsUpdated { get; set; }
    
    public bool IsDecoy { get; set; }
    
    public bool IsAdmin { get; set; }

    public bool IsSponsor { get; set; }

    public virtual UserData? Data { get; set; }
    public virtual Settings? Settings { get; set; }
    public virtual Location.Location? Location { get; set; }

    public User()
    { }
}
