using MyWebApi.App_GlobalResources;
using MyWebApi.Enums;
using System;

namespace MyWebApi.Utilities
{
    public static class EnumLocalizer
    {
        public static string GetLocalizedValue(ReportReason genericValue)
        {
            return Resources.ResourceManager.GetString("ReportReason_" + genericValue.ToString()) ?? genericValue.ToString();
        }
    }
}
