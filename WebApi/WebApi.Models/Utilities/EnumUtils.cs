using WebApi.Enums.Enums.Authentication;

namespace WebApi.Models.Utilities
{
    public static class EnumUtils
    {
        public static string ToLowerString<T>(this T userRole) where T : Enum
        {
            return userRole.ToString().ToLower();
        }
    }
}
