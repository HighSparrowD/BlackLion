using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebApi.Entities.TestEntities
{
    public class TestAnswer
    {
        [Key]
        [NotNull]
        public long Id { get; set; }
        [NotNull]
        public string Text { get; set; }
        [NotNull]
        public double Value { get; set; }
        [NotNull]
        public long TestQuestionId { get; set; }
        public string Tags { get; set; }
        public bool IsCorrect { get; set; }
    }
}
