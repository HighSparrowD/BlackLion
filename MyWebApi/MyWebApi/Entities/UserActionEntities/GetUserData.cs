using WebApi.Entities.UserInfoEntities;
using System.Text.Json.Serialization;
#nullable enable

namespace WebApi.Entities.UserActionEntities
{
    public class GetUserData
    {
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("comment")]
        public string? Comment { get; set; }
        [JsonPropertyName("cityId")]
        public int? CityId { get; set; }
        [JsonPropertyName("userDataInfo")]
        public UserData UserDataInfo { get; set; }

        public GetUserData(User userModel, string descriptionBonus = "")
        {
            UserId = userModel.Id;
            UserDataInfo = userModel.Data!;

            if (userModel.Location != null)
                CityId = userModel.Location.CityId;

            UserDataInfo.UserDescription = $"{descriptionBonus}\n{UserDataInfo.UserDescription}";
        }

        public void AddDescriptionBonus(string bonus)
        {
            UserDataInfo!.UserDescription = $"{bonus}\n{UserDataInfo.UserDescription}";
        }

        public void AddDescriptionBonusDownwards(string bonus)
        {
            UserDataInfo!.UserDescription = $"{UserDataInfo.UserDescription}\n{bonus}";
        }
    }
}
