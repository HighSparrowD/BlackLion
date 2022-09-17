using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.AdminEntities
{
    public class Admin
    {
        [Key]
        public long Id { get; set; }
        public bool IsEnabled { get; set; }
    }
}
