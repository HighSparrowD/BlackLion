using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.General;

namespace WebApi.Main.Entities.Sponsor;

public class SponsorLanguage
{
    [Key]
    public long Id { get; set; }
    public long SponsorId { get; set; }
    public AppLanguage Lang { get; set; }
    public short Level { get; set; }
    public virtual Language.Language Language { get; set; }
}
