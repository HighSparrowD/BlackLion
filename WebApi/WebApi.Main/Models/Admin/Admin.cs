using System.ComponentModel.DataAnnotations;

namespace WebApi.Main.Models.Admin;

public class Admin
{
    [Key]
    public long Id { get; set; }
    public bool IsEnabled { get; set; }
}
