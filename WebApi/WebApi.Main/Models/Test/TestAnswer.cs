using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using models = WebApi.Models.Models.Test;

#nullable enable
namespace WebApi.Main.Models.Test;

public class TestAnswer
{
    [Key]
    [NotNull]
    public long Id { get; set; }
    [NotNull]
    public string Text { get; set; } = default!;
    [NotNull]
    public double Value { get; set; }
    [NotNull]
    public long TestQuestionId { get; set; }
    public List<long>? Tags { get; set; }

    public TestAnswer()
    {}

    public static explicit operator TestAnswer?(models.TestAnswer? answer)
    {
        if (answer == null)
            return null;

        return new TestAnswer
        {
            Id = answer.Id,
            Tags = answer.Tags,
            TestQuestionId = answer.TestQuestionId,
            Text = answer.Text,
            Value = answer.Value
        };
    }

    public static implicit operator models.TestAnswer?(TestAnswer? answer)
    {
        if (answer == null)
            return null;

        return new models.TestAnswer
        {
            Id = answer.Id,
            Tags = answer.Tags,
            TestQuestionId = answer.TestQuestionId,
            Text = answer.Text,
            Value = answer.Value
        };
    }
}
