using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.AchievementEntities
{
    public class Achievement
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Key]
        public int ClassLocalisationId { get; set; }
        public int SectionId { get; set; }
        public int ConditionValue { get; set; }
        public int Value { get; set; }
    }
}
