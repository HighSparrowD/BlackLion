using System.Collections.Generic;

namespace WebApi.Entities.UserActionEntities
{
    public class GetUserByTags
    {
        public long UserId { get; set; }
        public string Tags { get; set; }
    }
}
