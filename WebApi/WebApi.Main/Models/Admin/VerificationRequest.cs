using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Enums.Enums.Media;
using WebApi.Enums.Enums.User;
using WebApi.Models.Models.User;
using models = WebApi.Models.Models.Admin;

namespace WebApi.Main.Entities.Admin;

#nullable enable
public class VerificationRequest
{
    [Key]
    public long Id { get; set; }

    [ForeignKey("User")]
    public long UserId { get; set; }

    public long? AdminId { get; set; }

    public VerificationRequestStatus? State { get; set; }

    public string Media { get; set; } = default!;

    public MediaType MediaType { get; set; }

    public string? Gesture { get; set; }

    public IdentityConfirmationType ConfirmationType { get; set; }

    public virtual User.User? User { get; set; }

    public static explicit operator VerificationRequest?(models.VerificationRequest? request)
    {
        if (request == null)
            return null;

        return new VerificationRequest
        {
            Id = request.Id,
            AdminId = request.AdminId,
            UserId = request.UserId,
            State = request.State,
            Media = request.Media,
            MediaType = request.MediaType,
            Gesture = request.Gesture,
            ConfirmationType = request.ConfirmationType,
        };
    }

    public static explicit operator VerificationRequest?(SendVerificationRequest? request)
    {
        if (request == null)
            return null;

        return new VerificationRequest
        {
            UserId = request.UserId,
            Media = request.Media,
            MediaType = request.MediaType,
            Gesture = request.Gesture,
            State = null,
            AdminId = null,
            ConfirmationType = request.ConfirmationType,
        };
    }

    public static implicit operator models.VerificationRequest?(VerificationRequest? request)
    {
        if (request == null)
            return null;

        return new models.VerificationRequest
        {
            Id = request.Id,
            AdminId = request.AdminId,
            UserId = request.UserId,
            State = request.State,
            Media = request.Media,
            MediaType = request.MediaType,
            Gesture = request.Gesture,
            ConfirmationType = request.ConfirmationType
        };
    }
}
