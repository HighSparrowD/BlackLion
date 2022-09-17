using System.ComponentModel.DataAnnotations;
#nullable enable

namespace MyWebApi.Entities.UserInfoEntities
{
    public class UserBaseInfo
    {
        [Key]
        public long Id { get; set; }
        public string? UserName { get; set; }
        public string? UserRealName { get; set; }
        public string? UserDescription { get; set; }
        public string? UserPhoto { get; set; }

        public UserBaseInfo(long id, string? userName, string? userRealName, string? userDescription, string? userPhoto)
        {
            Id = id;
            UserName = userName;
            UserRealName = userRealName;
            UserDescription = userDescription;
            UserPhoto = userPhoto;
        }
    }
}
