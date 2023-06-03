using System.Collections.Generic;

namespace WebApi.Entities.UserActionEntities
{
    public class GetUserTags
    {
        public List<UserTags> FullTags { get; set; }
        public List<string> Tags { get; set; }
    }
}
