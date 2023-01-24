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
        public string? UserRawDescription { get; set; }
        public string? UserMedia { get; set; }
        public bool IsPhotoReal { get; set; }
        public bool IsMediaPhoto { get; set; }

        public UserBaseInfo()
        {}

        public UserBaseInfo(long id, string? userName, string? userRealName, string? userDescription, string? userPhoto, bool isPhotoReal, bool isMediaPhoto)
        {
            Id = id;
            UserName = userName;
            UserRealName = userRealName;
            UserDescription = userDescription;
            UserMedia = userPhoto;
            IsPhotoReal = isPhotoReal;
            IsMediaPhoto = isMediaPhoto;
        }

        public UserBaseInfo(UserBaseInfo model)
        {
            Id = model.Id;
            UserName = model.UserName;
            UserRealName = model.UserRealName;
            UserDescription = model.UserDescription;
            UserMedia = model.UserMedia;
            IsPhotoReal = model.IsPhotoReal;
        }

        public string GenerateUserDescription(string? name, int age, string? country, string? city, string? description)
        {
            return $"{name}, {age},\n({country} - {city})\n\n{description}";
        }
    }
}
