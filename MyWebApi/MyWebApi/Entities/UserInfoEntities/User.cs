using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using MyWebApi.Enums;
#nullable enable

namespace MyWebApi.Entities.UserInfoEntities
{
    public class User
    {
        [Key]
        public long UserId { get; set; }
        public long UserBaseInfoId { get; set; }
        public long UserDataInfoId { get; set; }
        public long UserPreferencesId { get; set; }
        public bool ShouldConsiderLanguages { get; set; }
        public bool IsBusy { get; set; }
        public bool IsBanned { get; set; }
        public DateTime? BanDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool HasPremium { get; set; }
        public bool HadReceivedReward { get; set; }
        public bool? IsFree { get; set; }
        public IdentityConfirmationType IdentityType { get; set; }
        public bool IncreasedFamiliarity { get; set; }
        public short? PremiumDuration { get; set; }
        public short ReportCount { get; set; }
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
        public virtual UserBaseInfo? UserBaseInfo { get; set; }
        public virtual UserDataInfo? UserDataInfo  { get; set; }
        public virtual UserPreferences? UserPreferences { get; set; }
        public virtual List<BlackList>? UserBlackList { get; set; }
        //public virtual UserTrustLevel? TrustLevel { get; set; }
        //public virtual List<Encounter>? UserEncounters { get; set; }

        public User(long userId)
        {
            UserId = userId;
            UserBaseInfoId = userId;
            UserDataInfoId = userId;
            UserPreferencesId = userId;
        }

        //public List<BaseTestModel>? UserSertificates { get; set; }

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

        public static User? CreateDummyUser()
        {
            return null; //new UserInfoModel { UserId = rn.Next(m, x), UserName = "NewUser"};
        }
    }
}
