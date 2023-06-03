using System.ComponentModel.DataAnnotations;
using WebApi.Enums;

namespace WebApi.Entities.AchievementEntities
{
    public class Achievement
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Key]
        public AppLanguage Language { get; set; }
        public int SectionId { get; set; }
        public int ConditionValue { get; set; }
        public int Value { get; set; }
    }
}
