using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Main.Models.Admin;

public class AdminErrorLog
{
    [Key]
    public Guid Id { get; set; }
    public long? ThrownByUser { get; set; }
    public string Description { get; set; } = string.Empty;
    public int SectionId { get; set; }
    public DateTime Time { get; set; }
}
