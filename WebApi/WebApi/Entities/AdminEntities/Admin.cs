using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.AdminEntities
{
    public class Admin
    {
        [Key]
        public long Id { get; set; }
        public bool IsEnabled { get; set; }
    }
}
