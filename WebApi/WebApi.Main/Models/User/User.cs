using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Enums.Enums.User;
using WebApi.Models.Models.User;
using models = WebApi.Models.Models.User;
#nullable enable

namespace WebApi.Main.Entities.User;

public class User
{
    [Key]
    public long Id { get; set; }

    public string? UserName { get; set; }

    [ForeignKey("Data")]
    public long DataId { get; set; }

    [ForeignKey("Settings")]
    public long SettingsId { get; set; }

    [ForeignKey("Location")]
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
    public virtual Statistics? Statistics { get; set; }
    public virtual List<BlackList>? BlackList { get; set; }
    public virtual List<UserTag>? Tags { get; set; }
    public virtual List<Encounter>? Encounters { get; set; }
    public virtual List<UserNotification>? Notifications { get; set; }
    public virtual List<Request>? Requests { get; set; }
    public virtual List<UserRole>? UserRoles { get; set; }
    //public virtual UserTrustLevel? TrustLevel { get; set; }
    //public virtual List<Encounter>? UserEncounters { get; set; }

    public User()
    { }

    public User(long userId, bool isDecoy = false)
    {
        Id = userId;
        DataId = userId;
        SettingsId = userId;
        LocationId = userId;

        IsBusy = true; // At the moment of registration user is still inside Registration module, thus true
        HadReceivedReward = false;
        IdentityType = IdentityConfirmationType.None;
        ShouldEnhance = false;
        ReportCount = 0;
        DailyRewardPoint = 1;
        BonusIndex = 0;

        InvitedUsersCount = 0;
        InvitedUsersBonus = 0;

        ProfileViewsCount = 0;
        RTViewsCount = 0;
        TagSearchesCount = 0;
        AdventureSearchCount = 0;

        MaxProfileViewsCount = 50;
        MaxRTViewsCount = 25;
        MaxTagSearchCount = 3;
        MaxAdventureSearchCount = 15;

        IsUpdated = true;
        IsDecoy = isDecoy;

        IsAdmin = false;

        IsSponsor = false;
    }

    public string GenerateUserDescription(string? name, int age, string? country, string? city, string? description)
    {
        return $"{name}, {age},\n({country} - {city})\n\n{description}";
    }

    public static explicit operator User? (models.User user)
    {
        if (user == null)
            return null;

        return new User
        {
            Id = user.Id,
            UserName = user.UserName,
            DataId = user.DataId,
            SettingsId = user.SettingsId,
            Data = (UserData?)user.Data,
            Settings = (Settings?)user.Settings,
            AdventureSearchCount= user.AdventureSearchCount,
            MaxAdventureSearchCount = user.MaxAdventureSearchCount,
            MaxTagSearchCount = user.MaxTagSearchCount,
            TagSearchesCount = user.TagSearchesCount,
            BanDate = user.BanDate,
            BonusIndex = user.BonusIndex,
            DailyRewardPoint = user.DailyRewardPoint,
            DeleteDate = user.DeleteDate,
            EnteredPromoCodes = user.EnteredPromoCodes,
            HadReceivedReward = user.HadReceivedReward,
            IdentityType = user.IdentityType,
            InvitedUsersBonus = user.InvitedUsersBonus,
            InvitedUsersCount = user.InvitedUsersCount,
            IsBusy = user.IsBusy,
            IsDecoy = user.IsDecoy,
            IsUpdated = user.IsUpdated,
            Location = (Location.Location?)user.Location,
            MaxProfileViewsCount = user.MaxProfileViewsCount,
            MaxRTViewsCount = user.MaxRTViewsCount,
            Nickname = user.Nickname,
            RTViewsCount = user.RTViewsCount,
            LocationId = user.LocationId,
            ReportCount = user.ReportCount,
            ParentId = user.ParentId,
            ProfileViewsCount = user.ProfileViewsCount,
            PremiumExpirationDate = user.PremiumExpirationDate,
            PremiumDuration = user.PremiumDuration,
            ShouldEnhance = user.ShouldEnhance,
            IsAdmin = user.IsAdmin,
            IsSponsor = user.IsSponsor
        };
    }

    public static implicit operator models.User? (User? user)
    {
        if (user == null)
            return null;

        return new models.User
        {
            Id = user.Id,
            UserName = user.UserName,
            DataId = user.DataId,
            SettingsId = user.SettingsId,
            Data = (models.UserData?)user.Data,
            Settings = (models.Settings?)user.Settings,
            AdventureSearchCount = user.AdventureSearchCount,
            MaxAdventureSearchCount = user.MaxAdventureSearchCount,
            MaxTagSearchCount = user.MaxTagSearchCount,
            TagSearchesCount = user.TagSearchesCount,
            BanDate = user.BanDate,
            BonusIndex = user.BonusIndex,
            DailyRewardPoint = user.DailyRewardPoint,
            DeleteDate = user.DeleteDate,
            EnteredPromoCodes = user.EnteredPromoCodes,
            HadReceivedReward = user.HadReceivedReward,
            IdentityType = user.IdentityType,
            InvitedUsersBonus = user.InvitedUsersBonus,
            InvitedUsersCount = user.InvitedUsersCount,
            IsBusy = user.IsBusy,
            IsDecoy = user.IsDecoy,
            IsUpdated = user.IsUpdated,
            Location = user.Location,
            MaxProfileViewsCount = user.MaxProfileViewsCount,
            MaxRTViewsCount = user.MaxRTViewsCount,
            Nickname = user.Nickname,
            RTViewsCount = user.RTViewsCount,
            LocationId = user.LocationId,
            ReportCount = user.ReportCount,
            ParentId = user.ParentId,
            ProfileViewsCount = user.ProfileViewsCount,
            PremiumExpirationDate = user.PremiumExpirationDate,
            PremiumDuration = user.PremiumDuration,
            ShouldEnhance = user.ShouldEnhance,
            IsAdmin = user.IsAdmin,
            IsSponsor = user.IsSponsor
        };
    }

    public static implicit operator models.UserInfo?(User user)
    {
        if (user == null || user.Data == null || user.Location == null)
            return null;

        return new UserInfo
        {
            Id = user.Data.Id,
            Username = user.UserName,
            AgePrefs = user.Data.AgePrefs,
            Text = user.Data.AutoReplyText,
            Voice = user.Data.AutoReplyVoice,
            Age = user.Data.UserAge,
            Description = user.Data.UserDescription,
            RawDescription = user.Data.UserRawDescription,
            CommunicationPrefs = user.Data.CommunicationPrefs,
            Language = user.Data.Language,
            LanguagePreferences = user.Data.LanguagePreferences,
            LocationPreferences = user.Data.LocationPreferences,
            MediaType = user.Data.MediaType,
            Media = user.Data.UserMedia,
            Reason = user.Data.Reason,
            Gender = user.Data.UserGender,
            GenderPrefs = user.Data.UserGenderPrefs,
            Languages = user.Data.UserLanguages,
            RealName = user.Data.UserRealName,
            City = user.Location.CityId,
            Country = user.Location.CountryId,
            CityLang = user.Data.Language,
            CountryLang = user.Data.Language,
            IdentityType = user.IdentityType,
            HasPremium = user.PremiumExpirationDate != null
        };
    }
}
