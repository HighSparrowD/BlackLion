using System.Text.Json.Serialization;
using WebApi.Enums.Enums.Tag;

namespace WebApi.Models.Models.User
{
    public class UserTags
    {
        [JsonPropertyName("tag")]
        public string Tag { get; set; }
        [JsonPropertyName("tagType")]
        public TagType TagType { get; set; }
    }
}
