using System.Collections.Generic;

namespace MyWebApi.Entities
{
    public class LogicalTestModel : BaseTestModel
    {
        public List<Dictionary<string, List<string>>> Questions { get; set; }
        public List<int> CorrectAnswers { get; set; } //TODO: Think about another way of checking if the answer is correct
        public List<string> Photos { get; set; } // -> Optional
    }
}
