using System.ComponentModel.DataAnnotations;

namespace WebApi.Main.Entities.DailyReward;

public class DailyReward
{
    [Key]
    public long Id { get; set; }
    public int PointReward { get; set; }
    public short Index { get; set; }
}
