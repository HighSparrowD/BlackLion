using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebApi.Entities.UserActionEntities
{
    public class GetUserTags
    {
        [JsonPropertyName("fullTags")]
        public List<UserTags> FullTags { get; set; }
        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }
    }
}
