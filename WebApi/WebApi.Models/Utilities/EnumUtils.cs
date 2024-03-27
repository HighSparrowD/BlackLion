using WebApi.Enums.Enums.User;

namespace WebApi.Models.Utilities
{
    public static class EnumUtils
    {
        public static string ToLowerString(this UserRole userRole)
        {
            return userRole.ToString().ToLower();
        }
    }
}
