namespace WebApi.Models.Models.Test
{
    public class TestAnswer
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public double Value { get; set; }
        public long TestQuestionId { get; set; }
        public List<long> Tags { get; set; }
    }
}
