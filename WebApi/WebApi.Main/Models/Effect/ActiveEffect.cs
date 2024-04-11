using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.General;
using models = WebApi.Models.Models.Effect;

#nullable enable
namespace WebApi.Main.Entities.Effect;

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

    public static explicit operator ActiveEffect?(models.ActiveEffect? effect)
    {
        if (effect == null)
            return null;

        return new ActiveEffect
        {
            Id = effect.Id,
            UserId = effect.UserId,
            Effect = effect.Effect,
            ExpirationTime = effect.ExpirationTime,
            Name = effect.Name
        };
    }

    public static implicit operator models.ActiveEffect?(ActiveEffect? effect)
    {
        if (effect == null)
            return null;

        return new models.ActiveEffect
        {
            Id = effect.Id,
            UserId = effect.UserId,
            Effect = effect.Effect,
            ExpirationTime = effect.ExpirationTime,
            Name = effect.Name
        };
    }
}
