using System.ComponentModel.DataAnnotations;
using enums = WebApi.Enums.Enums.User;

namespace WebApi.Main.Models.User
{
    public class UserRole
    {
        [Key]
        public long UserId { get; set; }
        
        [Key]
        public Enums.Enums.Authentication.Role Role { get; set; }
    }
}
