using System.ComponentModel.DataAnnotations;
using WebApi.Enums;

namespace WebApi.Entities.UserInfoEntities
{
    public class Visit
    {
        [Key]
        public long UserId { get; set; }
        [Key]
        public Section Section { get; set; }
    }
}
