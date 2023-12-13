using WebApi.Enums.Enums.Media;

namespace WebApi.Models.Models.User
{
    public class GetUserMedia
    {
        public string Media { get; set; }
        public MediaType MediaType { get; set; }
    }
}
