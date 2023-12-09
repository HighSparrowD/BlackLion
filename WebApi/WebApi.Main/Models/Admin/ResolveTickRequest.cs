using System;

namespace WebApi.Main.Models.Admin;

public class ResolveTickRequest
{
    public long Id { get; set; }
    public long AdminId { get; set; }
    public bool IsAccepted { get; set; }
    public string Comment { get; set; }
}
