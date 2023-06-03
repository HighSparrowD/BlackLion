using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebApi.Enums;

namespace WebApi.Entities.TestEntities
{
    public class TestResult
    {
        [Key]
        [NotNull]
        public long Id { get; set; }
        [NotNull]
        public long TestId { get; set; }
        [NotNull]
        public AppLanguage Language { get; set; }
        [NotNull]
        public int Score { get; set; }
        [NotNull]
        public string Result { get; set; }
        public string Tags { get; set; }
    }
}
