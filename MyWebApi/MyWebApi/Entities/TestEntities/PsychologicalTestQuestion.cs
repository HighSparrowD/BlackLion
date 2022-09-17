using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.TestEntities
{
    public class PsychologicalTestQuestion
    {
        [Key]
        public long Id { get; set; }
        public long PsychologicalTestId { get; set; }
        public string Text { get; set; }
        public virtual List<PsychologicalTestAnswer> Answers { get; set; }
    }
}
