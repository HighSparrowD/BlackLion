using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using WebApi.Enums;
using WebApi.Entities.LocationEntities;
#nullable enable

namespace WebApi.Entities.UserInfoEntities
{
    public class User
    {
        [Key]
        public long Id { get; set; }
        public long DataId { get; set; }
        public long SettingsId { get; set; }
        public long LocationId { get; set; }
        public bool IsBusy { get; set; }
        public bool IsBanned { get; set; }
        public DateTime? BanDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool HasPremium { get; set; }
        public bool HadReceivedReward { get; set; }
        public short? PremiumDuration { get; set; }
        public IdentityConfirmationType IdentityType { get; set; }
        public bool ShouldEnhance { get; set; }
        public short ReportCount { get; set; } //Daily report count
        public short DailyRewardPoint { get; set; } 
        public double BonusIndex { get; set; }
        public int InvitedUsersCount { get; set; }
        public double InvitedUsersBonus { get; set; }
        public string? Nickname { get; set; }
        public long? ParentId { get; set; }
        public int ProfileViewsCount { get; set; }
        public int RTViewsCount { get; set; }
        public int MaxProfileViewsCount { get; set; }
        public int MaxRTViewsCount { get; set; }
        public int MaxTagSearchCount { get; set; }
        public int TagSearchesCount { get; set; }
        public short? Currency { get; set; }
        public DateTime? PremiumExpirationDate{ get; set; }
        public string? EnteredPromoCodes { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsDecoy { get; set; }

        public virtual UserData? Data  { get; set; }
        public virtual UserSettings? UserSettings  { get; set; }
        public virtual Location? Location  { get; set; }
        public virtual List<BlackList>? UserBlackList { get; set; }
        public virtual List<UserTag>? Tags { get; set; }
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

            IsBusy = false;
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

            MaxProfileViewsCount = 50;
            MaxRTViewsCount = 25;
            MaxTagSearchCount = 3;

            IsUpdated = true;

            IsDecoy = isDecoy;
        }

        public string GenerateUserDescription(string? name, int age, string? country, string? city, string? description)
        {
            return $"{name}, {age},\n({country} - {city})\n\n{description}";
        }

        public bool CheckIfHasEncountered(List<Encounter> encounters, long userId)
        {
            bool result = encounters.Where(e => e.UserId == userId)
                .SingleOrDefault() != null 
                || 
                encounters
                .Where(e => e.EncounteredUserId == userId)
                .SingleOrDefault() != null;
            return result;
        }

        public static List<int>? CalculateAgeList(int userAge, int c)
        {
            switch (c)
            {
                case 0:
                    return Enumerable.Range(userAge + 5, 6).ToList();
                case 1:
                    return Enumerable.Range(userAge + 2, 4).ToList();
                case 2:
                    return Enumerable.Range(userAge - 2, 4).ToList();
                case 3:
                    return Enumerable.Range(userAge - 6, 4).ToList();
                case 4:
                    return Enumerable.Range(userAge - 10, 6).ToList();
                case 5:
                    return Enumerable.Range(0, 100).ToList();
                default:
                    return null;
            }
        }
    }
}
