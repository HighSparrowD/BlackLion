using System.ComponentModel.DataAnnotations;

#nullable enable
namespace WebApi.Main.Models.Sponsor;

public class Ad
{
    [Key]
    public long Id { get; set; }
    public long SponsorId { get; set; }
    public string? Text { get; set; }
    public string? Description { get; set; }
    public string? Photo { get; set; }
    public string? Video { get; set; }

    public static string TrancateDescription(string text, int leng)
    {
        string description = "";

        foreach (char c in text)
        {
            if (description.Length + 1 <= leng)
            {
                description += c;
            }
        }

        return description;
    }
}
