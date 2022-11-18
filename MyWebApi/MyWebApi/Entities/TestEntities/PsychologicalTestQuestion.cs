using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MyWebApi.Entities.TestEntities
{
    public class PsychologicalTestQuestion
    {   [Key]
        [NotNull]
        public long Id { get; set; }
        [NotNull]
        public long PsychologicalTestId { get; set; }
        [NotNull]
        public int PsychologicalTestClassLocalisationId { get; set; }
        [NotNull]
        public string Text { get; set; }
        [NotNull]
        public virtual List<PsychologicalTestAnswer> Answers { get; set; }
    }
}
