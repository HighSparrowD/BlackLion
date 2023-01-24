using System.Collections.Generic;

namespace MyWebApi.Entities.UserActionEntities
{
    public class GetUserTags
    {
        public List<UserTags> FullTags { get; set; }
        public List<string> Tags { get; set; }
    }
}
