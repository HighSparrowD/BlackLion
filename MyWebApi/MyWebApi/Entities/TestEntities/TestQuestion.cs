using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebApi.Enums;

namespace WebApi.Entities.TestEntities
{
    public class TestQuestion
    {   [Key]
        [NotNull]
        public long Id { get; set; }
        [NotNull]
        public long TestId { get; set; }
        [NotNull]
        public AppLanguage TestLanguage { get; set; }
        [NotNull]
        public string Text { get; set; }
        public string Photo { get; set; }
        // Each test has different number of scales.
        // This parameter represents, to which the question is related to
        public string Scale { get; set; }
        public virtual List<TestAnswer> Answers { get; set; }
    }
}
