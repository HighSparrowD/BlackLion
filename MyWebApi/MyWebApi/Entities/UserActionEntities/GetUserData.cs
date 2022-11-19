using MyWebApi.Entities.UserInfoEntities;
using System.Collections.Generic;
using System;
#nullable enable

namespace MyWebApi.Entities.UserActionEntities
{
    public class GetUserData
    {
        public long UserId { get; set; }
        public UserBaseInfo? UserBaseInfo { get; set; }
        public UserDataInfo? UserDataInfo { get; set; }
        public UserPreferences? UserPreferences { get; set; }

        public GetUserData(User userModel, string descriptionBonus = "")
        {
            UserId = userModel.UserId;
            UserBaseInfo = new UserBaseInfo(userModel.UserBaseInfo!);
            UserDataInfo = userModel.UserDataInfo;
            UserPreferences = userModel.UserPreferences;

            UserBaseInfo.UserDescription = $"{descriptionBonus}\n{UserBaseInfo.UserDescription}";
        }

        public void AddDescriptionBonus(string bonus)
        {
            UserBaseInfo!.UserDescription = $"{bonus}\n{UserBaseInfo.UserDescription}";
        }
    }
}
