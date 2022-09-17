using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.UserInfoEntities
{
    public class FriendModel
    {
        [Key]
        public long UserId { get; set; }
    }
}
