using System.ComponentModel.DataAnnotations;

namespace WebApi.Enums.Enums.Report;

//TODO: Fill-up
public enum FeedbackReason : byte
{
    [Display(Name = "Suggestion")]
    Suggestion = 1,
    [Display(Name = "SuggestHints")]
    SuggestHints = 2,
}
