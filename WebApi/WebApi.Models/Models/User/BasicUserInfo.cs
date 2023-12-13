using System.Text.Json.Serialization;

namespace WebApi.Models.Models.User
{
    public class BasicUserInfo
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("userRealName")]
        public string UserRealName { get; set; }
        [JsonPropertyName("isBanned")]
        public bool IsBanned { get; set; }
        [JsonPropertyName("isBusy")]
        public bool IsBusy { get; set; }
        [JsonPropertyName("hasPremium")]
        public bool HasPremium { get; set; }
        [JsonPropertyName("isFree")]
        public bool? IsFree { get; set; }
        [JsonPropertyName("limitations")]
        public GetLimitations Limitations { get; set; }
    }
}
