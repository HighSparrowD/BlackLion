using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.General;
using WebApi.Main.Entities.Admin;

namespace WebApi.Main.Entities.Achievement;

public class Achievement
{
    [Key]
    public int Id { get; set; }
    [Key]
    public AppLanguage Language { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int ConditionValue { get; set; }
    public int Reward { get; set; }

    public Achievement()
    { }

    public Achievement(UploadAchievement model)
    {
        Id = model.Id;
        Language = model.Language;
        Name = model.Name;
        Description = model.Description;
        ConditionValue = model.ConditionValue;
        Reward = model.Reward;
    }
}
