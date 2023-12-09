using System.ComponentModel.DataAnnotations;
using WebApi.Main.Enums.General;

namespace WebApi.Main.Models.User;

public class Visit
{
    [Key]
    public long UserId { get; set; }
    [Key]
    public Section Section { get; set; }
}
