using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.UserInfoEntities
{
    public class Visit
    {
        [Key]
        public long UserId { get; set; }
        [Key]
        public int SectionId { get; set; }
    }
}
