using WebApi.Enums;

namespace WebApi.Entities.UserInfoEntities
{
    public class UserPartialData
    {
        public long Id { get; set; }
        public string Media { get; set; }
        public bool IsPhoto { get; set; }
        public AppLanguage AppLanguage { get; set; }
    }
}
