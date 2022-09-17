using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.TestEntities
{
    public class PsychologicalTestAnswer
    {
        [Key]
        public long Id { get; set; }
        public long PsychologicalTestQuestionId  { get; set; }
        public string Text { get; set; }
    }
}
