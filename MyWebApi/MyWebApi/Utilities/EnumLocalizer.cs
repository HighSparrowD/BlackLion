using WebApi.App_GlobalResources;
using WebApi.Enums;

namespace WebApi.Utilities
{
    public static class EnumLocalizer
    {
        public static string GetLocalizedValue(ReportReason reason)
        {
            return Resources.ResourceManager.GetString("ReportReason_" + reason.ToString()) ?? reason.ToString();
        }

        public static string GetLocalizedValue(FeedbackReason reason)
        {
            return Resources.ResourceManager.GetString("FeedbackReason_" + reason.ToString()) ?? reason.ToString();
        }

        public static string GetLocalizedValue(UsageReason reason)
        {
            return Resources.ResourceManager.GetString("UsageReason_" + reason.ToString()) ?? reason.ToString();
        }

        public static string GetLocalizedValue(Gender gender)
        {
            return Resources.ResourceManager.GetString("Gender_" + gender.ToString()) ?? gender.ToString();
        }

        public static string GetLocalizedValue(CommunicationPreference preference)
        {
            return Resources.ResourceManager.GetString("CommunicationPreference_" + preference.ToString()) ?? preference.ToString();
        }

        public static string GetLocalizedValue(AppLanguage language)
        {
            return Resources.ResourceManager.GetString("AppLanguage_" + language.ToString()) ?? language.ToString();
        }
    }
}
