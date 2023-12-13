using System.Collections.Generic;

namespace WebApi.Models.Models.User
{
    public class GetUserByTags
    {
        public long UserId { get; set; }
        public string Tags { get; set; }
    }
}
