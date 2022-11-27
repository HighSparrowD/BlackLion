using System.Collections.Generic;

namespace MyWebApi.Entities.UserActionEntities
{
    public class GetUserByTags
    {
        public long UserId { get; set; }
        public List<string> Tags { get; set; }
    }
}
