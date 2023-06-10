using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.UserInfoEntities
{
    public class Visit
    {
        [Key]
        public long UserId { get; set; }
        [Key]
        public int SectionId { get; set; }
    }
}
