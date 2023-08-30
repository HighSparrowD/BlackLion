using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebApi.Enums;
using OceanStats = WebApi.Enums.OceanStats;

namespace WebApi.Entities.TestEntities
{
    public class Test
    {
        [Key]
        [NotNull]
        public long Id { get; set; }
        [Key]
        [NotNull]
        public AppLanguage Language { get; set; }
        [NotNull]
        public string Name { get; set; }
        [NotNull]
        public string Description { get; set; }
        [NotNull]
        public OceanStats TestType { get; set; }
        [NotNull]
        public int Price { get; set; }
        [NotNull]
        public int CanBePassedInDays { get; set; }
        [NotNull]
        public virtual List<TestQuestion> Questions { get; set; }
        [NotNull]
        public virtual List<TestResult> Results { get; set; }
        [NotNull]
        public virtual List<TestScale> Scales { get; set; }
    }
}
