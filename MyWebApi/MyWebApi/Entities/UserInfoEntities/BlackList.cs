using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApi.Entities.UserInfoEntities
{
    public class BlackList
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public long UserId1 { get; set; }
    }
}
