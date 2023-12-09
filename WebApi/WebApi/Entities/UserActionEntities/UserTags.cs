using System.Text.Json.Serialization;
using WebApi.Main.Enums.Tag;

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
