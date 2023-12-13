using System.Text.Json.Serialization;
using WebApi.Enums.Enums.User;
#nullable enable

namespace WebApi.Models.Models.User;

public class GetUserData
{
    [JsonPropertyName("userId")]
    public long UserId { get; set; }
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
    [JsonPropertyName("cityId")]
    public int? CityId { get; set; }
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }
    [JsonPropertyName("hasPremium")]
    public bool HasPremium { get; set; }
    [JsonPropertyName("usesOcean")]
    public bool UsesOcean { get; set; }
    [JsonPropertyName("identityType")]
    public IdentityConfirmationType IdentityType { get; set; }
    [JsonPropertyName("userData")]
    public UserData UserData { get; set; }

    public GetUserData(User userModel, string descriptionBonus = "")
    {
        UserId = userModel.Id;
        Nickname = userModel.Nickname;
        HasPremium = userModel.HasPremium;
        UsesOcean = userModel.Settings!.UsesOcean;
        IdentityType = userModel.IdentityType;
        UserData = userModel.Data!;

        if (userModel.Location != null)
            CityId = userModel.Location.CityId;

        UserData.UserDescription = $"{descriptionBonus}\n{UserData.UserDescription}";
    }

    public void AddDescriptionUpwards(string bonus)
    {
        UserData!.UserDescription = $"{bonus}\n{UserData.UserDescription}";
    }

    public void AddDescriptionBonusDownwards(string bonus)
    {
        UserData!.UserDescription = $"{UserData.UserDescription}\n{bonus}";
    }
}
