using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.General;

#nullable enable
namespace WebApi.Models.Models.Effect;

public class ActiveEffect
{
    [Key]
    public long Id { get; set; }
    public Currency Effect { get; set; }
    public long UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? ExpirationTime { get; set; }

    public ActiveEffect()
    {}

    public ActiveEffect(long userId)
    {
        UserId = userId;
    }
}
