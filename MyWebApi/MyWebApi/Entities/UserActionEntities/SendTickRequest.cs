using WebApi.Enums;

namespace WebApi.Entities.UserActionEntities
{
    #nullable enable
    public class SendTickRequest
    { 
        public long UserId { get; set; }
        public string? Photo { get; set; }
        public string? Video { get; set; }
        public string? Circle { get; set; }
        public string? Gesture { get; set; }
        public IdentityConfirmationType Type { get; set; }
    }
}
