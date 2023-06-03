using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.TestEntities
{
    public class UploadTestAnswer
    {
        public string Text { get; set; }
        public double Value { get; set; }
        public bool IsCorrect{ get; set; }
        public string Tags { get; set; }
    }
}
