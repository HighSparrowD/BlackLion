using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.UserInfoEntities
{
    public class FriendModel
    {
        [Key]
        public long UserId { get; set; }
    }
}
