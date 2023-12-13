namespace WebApi.Models.Models.User
{
    public class PointsPayload
    {
        public long UserId { get; set; }
        public int Balance { get; set; }
        public int? Openness { get; set; }
        public int? Conscientiousness { get; set; }
        public int? Extroversion { get; set; }
        public int? Agreeableness { get; set; }
        public int? Neuroticism { get; set; }
        public int? Nature { get; set; }
    }
}
