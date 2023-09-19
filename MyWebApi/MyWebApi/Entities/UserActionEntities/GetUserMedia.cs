using WebApi.Enums;

namespace WebApi.Entities.UserActionEntities
{
    public class GetUserMedia
    {
        public string Media { get; set; }
        public MediaType MediaType { get; set; }
    }
}
