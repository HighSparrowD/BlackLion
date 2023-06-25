using System.ComponentModel.DataAnnotations;

namespace WebApi.Enums;

//TODO: Fill-up
public enum FeedbackReason : byte
{
    [Display(Name = "Testing")]
    Testing = 1,
    [Display(Name = "Suggestion")]
    Suggestion = 2,
}
