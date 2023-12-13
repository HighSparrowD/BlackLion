using System.ComponentModel.DataAnnotations;

namespace WebApi.Main.Models.Sponsor;

public class SponsorRating
{
    [Key]
    public long Id { get; set; }
    public long SponsorId { get; set; }
    public long UserId { get; set; }
    public short Rating { get; set; }
    public string Comment { get; set; }
    public DateTime CommentTime { get; set; }
    public Sponsor Sponsor { get; set; }
    public User.User Commentator { get; set; }
}
