using WebApi.Enums;
using System.Text.Json.Serialization;

namespace WebApi.Entities.UserActionEntities
{
    public class SendAdventureReport
    {
        [JsonPropertyName("sender")]
        public long Sender { get; set; }
        [JsonPropertyName("adventure")]
        public long Adventure { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("reason")]
        public ReportReason Reason { get; set; }
    }
}
