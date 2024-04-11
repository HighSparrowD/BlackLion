using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Enums.Enums.General;

namespace WebApi.Main.Entities.User;

public class Encounter
{
    [Key]
    public long Id { get; set; }
    [ForeignKey("User")]
    public long UserId { get; set; }
    [ForeignKey("EncounteredUser")]
    public long EncounteredUserId { get; set; }
    public Section Section { get; set; }
    public DateTime EncounterDate { get; set; }
    public virtual User User { get; set; }
    public virtual User EncounteredUser { get; set; }

    public Encounter()
    { }

    public Encounter(RegisterEncounter model)
    {
        UserId = model.UserId;
        EncounteredUserId = model.EncounteredUserId;
        Section = model.Section;
        EncounterDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
    }
}
