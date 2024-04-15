using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.General;

namespace WebApi.Main.Entities.User;

public class Transaction
{
    [Key]
    public long Id { get; set; }
    public long UserId { get; set; }
    public DateTime PointInTime { get; set; }
    public float Amount { get; set; }
    public string Description { get; set; }
    public Currency Currency { get; set; }
}
