using System.Text.Json.Serialization;

namespace WebApi.Models.Models.Report;

public class GroupedFeedback
{
    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("feedbacks")]
    public List<Feedback> Feedbacks { get; set; }

    public GroupedFeedback(string username, List<Feedback> feedbacks)
    {
        Username = username;
        Feedbacks = feedbacks;
    }
}
