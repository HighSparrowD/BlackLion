using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Enums.Enums.User;
using models = WebApi.Models.Models.Admin;

namespace WebApi.Main.Entities.Admin;

#nullable enable
public class TickRequest
{
    [Key]
    public long Id { get; set; }

    [ForeignKey("User")]
    public long UserId { get; set; }

    public long? AdminId { get; set; }

    public TickRequestStatus? State { get; set; }

    public string? Photo { get; set; }

    public string? Video { get; set; }

    public string? Circle { get; set; }

    public string? Gesture { get; set; }

    public IdentityConfirmationType Type { get; set; }

    public virtual User.User? User { get; set; }

    public static explicit operator TickRequest?(models.TickRequest? request)
    {
        if (request == null)
            return null;

        return new TickRequest
        {
            Id = request.Id,
            AdminId = request.AdminId,
            UserId = request.UserId,
            State = request.State,
            Circle = request.Circle,
            Gesture = request.Gesture,
            Photo = request.Photo,
            Type = request.Type,
            Video = request.Video,
        };
    }

    public static implicit operator models.TickRequest?(TickRequest? request)
    {
        if (request == null)
            return null;

        return new models.TickRequest
        {
            Id = request.Id,
            AdminId = request.AdminId,
            UserId = request.UserId,
            State = request.State,
            Circle = request.Circle,
            Gesture = request.Gesture,
            Photo = request.Photo,
            Type = request.Type,
            Video = request.Video,
        };
    }
}
