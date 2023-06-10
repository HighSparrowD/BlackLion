using WebApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.HintEntities
{
    public class Hint
    {
        [Key]
        public int Id { get; set; }
        [Key]
        public AppLanguage Localization { get; set; }
        public Section? Section { get; set; }
        public HintType Type { get; set; }
        public string Text { get; set; }
    }
}
