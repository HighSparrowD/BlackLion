using WebApi.App_GlobalResources;
using WebApi.Enums;

namespace WebApi.Utilities
{
    public static class EnumLocalizer
    {
        public static string GetLocalizedValue(ReportReason genericValue)
        {
            return Resources.ResourceManager.GetString("ReportReason_" + genericValue.ToString()) ?? genericValue.ToString();
        }

        public static string GetLocalizedValue(UsageReason genericValue)
        {
            return Resources.ResourceManager.GetString("UsageReason_" + genericValue.ToString()) ?? genericValue.ToString();
        }
    }
}
