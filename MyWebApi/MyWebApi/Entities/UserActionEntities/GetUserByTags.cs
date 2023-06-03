using System.Collections.Generic;

namespace WebApi.Entities.UserActionEntities
{
    public class GetUserByTags
    {
        public long UserId { get; set; }
        public List<string> Tags { get; set; }
    }
}
