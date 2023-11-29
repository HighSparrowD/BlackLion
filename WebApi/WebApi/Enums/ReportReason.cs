using System.ComponentModel.DataAnnotations;

namespace WebApi.Enums;

//TODO: Fill-Up 
public enum ReportReason : short
{
    [Display(Name = "ReportReason_TestReason")]
    TestReason = 1,
    [Display(Name = "ReportReason_Spam")]
    Spam = 2
}

//public override string ToString()
//{
//    var displayAttribite = typeof(ReportReason)
//        .GetField(this.ToString())
//        .GetCustomAttributes(typeof(DisplayAttribute), false)
//        .FirstOrDefault() as DisplayAttribute;

//    if (displayAttribite != null)
//    {
//        var resourceManager = new ResourceManager(typeof(Resources));
//        var localizedDisplayName = resourceManager.GetString(displayAttribite.Name);
//        if (!string.IsNullOrEmpty(localizedDisplayName))
//        {
//            return localizedDisplayName;
//        }
//    }

//    //Fall back to initial functionality if string is not present in resources
//    return this.ToString();
//}
