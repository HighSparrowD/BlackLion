namespace MyWebApi.Entities.TestEntities
{
    public class IntellectualTestAnswer
    {
        public long Id { get; set; }
        public long IntellectualTestQuestionId { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }
}
