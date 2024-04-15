using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.General;

namespace WebApi.Main.Entities.User;

public class Visit
{
    [Key]
    public long UserId { get; set; }
    [Key]
    public Section Section { get; set; }
}
