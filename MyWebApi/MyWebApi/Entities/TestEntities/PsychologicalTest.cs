using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.TestEntities
{
    public class PsychologicalTest
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual List<PsychologicalTestQuestion> Questions { get; set; }
    }
}
