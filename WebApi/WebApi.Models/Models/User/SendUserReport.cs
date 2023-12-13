using System.Text.Json.Serialization;
using WebApi.Enums.Enums.Report;

namespace WebApi.Models.Models.User
{
    public class SendUserReport
    {
        [JsonPropertyName("sender")]
        public long Sender { get; set; }
        [JsonPropertyName("reportedUser")]
        public long ReportedUser { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("reason")]
        public ReportReason Reason { get; set; }
    }
}
