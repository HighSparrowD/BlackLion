using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebApi.Entities.AdventureEntities
{
    public class AdventureSearchResponse
    {
        [JsonPropertyName("adventures")]
        public List<GetAdventureSearch> Adventures { get; set; }

        public AdventureSearchResponse()
        {}

        public AdventureSearchResponse(List<GetAdventureSearch> adventures)
        {
            Adventures = adventures;
        }
    }
}
