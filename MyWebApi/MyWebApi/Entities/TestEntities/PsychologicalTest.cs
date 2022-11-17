using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MyWebApi.Entities.TestEntities
{
    public class PsychologicalTest
    {
        [Key]
        [NotNull]
        public long Id { get; set; }
        [Key]
        [NotNull]
        public int ClassLocalisationId { get; set; }
        [NotNull]
        public string Name { get; set; }
        [NotNull]
        public string Description { get; set; }
        [NotNull]
        public virtual List<PsychologicalTestQuestion> Questions { get; set; }
    }
}
