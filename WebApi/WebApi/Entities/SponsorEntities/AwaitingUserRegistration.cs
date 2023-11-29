namespace WebApi.Entities.SponsorEntities
{
    public class AwaitingUserRegistration
    {
        public string Username { get; set; }
        public string CodeWord { get; set; }
        public int UserMaxAdCount { get; set; }
        public int UserMaxAdViewCount { get; set; }
        public int UserAppLanguage { get; set; }
    }
}
