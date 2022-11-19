using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MyWebApi.Entities.TestEntities
{
    public class TestQuestion
    {   [Key]
        [NotNull]
        public long Id { get; set; }
        [NotNull]
        public long TestId { get; set; }
        [NotNull]
        public int TestClassLocalisationId { get; set; }
        [NotNull]
        public string Text { get; set; }
        [NotNull]
        public virtual List<TestAnswer> Answers { get; set; }
    }
}
