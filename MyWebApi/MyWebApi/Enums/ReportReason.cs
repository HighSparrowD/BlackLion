using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Enums;

//TODO: Fill-Up 
public enum ReportReason : short
{
    [Display(Name = "ReportReason_TestReason")]
    TestReason = 1,
    [Display(Name = "ReportReason_Spam")]
    Spam = 2
}
