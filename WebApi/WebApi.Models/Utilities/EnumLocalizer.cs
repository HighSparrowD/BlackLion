using WebApi.Enums.Enums.General;
using WebApi.Enums.Enums.Report;
using WebApi.Enums.Enums.Sponsor;
using WebApi.Enums.Enums.User;
using WebApi.Models.App_GlobalResources;

namespace WebApi.Models.Utilities
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

        public static string GetLocalizedValue(PaymentCurrency language)
        {
            return Resources.ResourceManager.GetString("Currency_" + language.ToString()) ?? language.ToString();
        }

        public static string GetLocalizedValue(Currency effect)
        {
            return Resources.ResourceManager.GetString("Effect_" + effect.ToString()) ?? effect.ToString();
        }

        public static string GetLocalizedValue(AdvertisementPriority reason)
        {
            return Resources.ResourceManager.GetString("AdvertisementPriority_" + reason.ToString()) ?? reason.ToString();
        }
    }
}
