using MyWebApi.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.UserInfoEntities
{
    public class UserTag
    {
        [Key]
        public long UserId { get; set; }
        [Key]
        public string Tag { get; set; }
        public TagType TagType { get; set; }

        public UserTag()
        {}

        public static List<UserTag> CreateTagList(long userId, string tagsSeparatedBy, string separator, TagType tagType)
        {
            var result = new List<UserTag>();
            var tags = tagsSeparatedBy.ToLower().Trim().Split(separator);

            foreach (var tag in tags)
            {
                result.Add(new UserTag
                {
                    UserId = userId,
                    Tag = tag,
                    TagType = tagType
                });
            }

            return result;
        }

        public static List<UserTag> CreateTagList (long userId, List<string> tagList, TagType tagType)
        {
            var result = new List<UserTag>();

            foreach (var tag in tagList)
            {
                result.Add(new UserTag
                {
                    UserId = userId,
                    Tag = tag,
                    TagType = tagType
                });
            }

            return result;
        }
    }
}
