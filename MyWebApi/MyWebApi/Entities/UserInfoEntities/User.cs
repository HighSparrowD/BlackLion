using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApi.Enums;
using WebApi.Entities.LocationEntities;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Entities.UserActionEntities;
#nullable enable

namespace WebApi.Entities.UserInfoEntities
{
    public class User
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey("Data")]
        public long DataId { get; set; }
        [ForeignKey("Settings")]
        public long SettingsId { get; set; }
        [ForeignKey("Location")]
        public long LocationId { get; set; }
        public bool IsBusy { get; set; }
        public bool IsBanned { get; set; }
        public bool IsDeleted { get; set; }
        public bool HasPremium { get; set; }
        public DateTime? BanDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public DateTime? PremiumExpirationDate{ get; set; }
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

        public virtual UserData? Data  { get; set; }
        public virtual Settings? Settings  { get; set; }
        public virtual Location? Location  { get; set; }
        public virtual List<BlackList>? BlackList { get; set; }
        public virtual List<UserTag>? Tags { get; set; }
        public virtual List<Encounter>? Encounters { get; set; }
        public virtual List<UserNotification>? Notifications { get; set; }
        public virtual List<Request>? Requests { get; set; }
        //public virtual UserTrustLevel? TrustLevel { get; set; }
        //public virtual List<Encounter>? UserEncounters { get; set; }

        public User()
        {}

        public User(long userId, bool isDecoy=false)
        {
            Id = userId;
            DataId = userId;
            SettingsId = userId;
            LocationId = userId;

            IsBusy = true; // At the moment of registration user is still inside Registration module, thus true
            IsBanned = false;
            IsDeleted = false;
            HasPremium = false;
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
        }

        public string GenerateUserDescription(string? name, int age, string? country, string? city, string? description)
        {
            return $"{name}, {age},\n({country} - {city})\n\n{description}";
        }
    }
}
