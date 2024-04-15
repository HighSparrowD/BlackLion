using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebApi.Enums.Enums.General;
using models = WebApi.Models.Models.Test;

#nullable enable
namespace WebApi.Main.Entities.Test;

public class TestQuestion
{
    [Key]
    [NotNull]
    public long Id { get; set; }
    [NotNull]
    public long TestId { get; set; }
    [NotNull]
    public AppLanguage TestLanguage { get; set; }
    [NotNull]
    public string Text { get; set; } = default!;
    public string Photo { get; set; } = default!;
    // Each test has different number of scales.
    // This parameter represents a parameter to which the question is related to
    public string Scale { get; set; } = default!;
    public virtual List<TestAnswer> Answers { get; set; } = default!;

    public TestQuestion()
    {}

    public static explicit operator TestQuestion?(models.TestQuestion? question)
    {
        if (question == null)
            return null;

        return new TestQuestion
        {
            Id = question.Id,
            Scale = question.Scale,
            Photo = question.Photo,
            TestId = question.TestId,
            TestLanguage = question.TestLanguage,
            Text = question.Text,
            Answers = question.Answers.Select(a => (TestAnswer)a!).ToList()
        };
    }

    public static implicit operator models.TestQuestion?(TestQuestion? question)
    {
        if (question == null)
            return null;

        return new models.TestQuestion
        {
            Id = question.Id,
            Scale = question.Scale,
            Photo = question.Photo,
            TestId = question.TestId,
            TestLanguage = question.TestLanguage,
            Text = question.Text,
            Answers = question.Answers.Select(a => (models.TestAnswer)a!).ToList()
        };
    }
}
