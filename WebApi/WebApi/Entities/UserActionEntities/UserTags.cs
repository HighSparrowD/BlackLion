using System.Text.Json.Serialization;
using WebApi.Enums;

namespace WebApi.Entities.UserActionEntities
{
    public class UserTags
    {
        [JsonPropertyName("tag")]
        public string Tag { get; set; }
        [JsonPropertyName("tagType")]
        public TagType TagType { get; set; }
    }
}
