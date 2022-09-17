using System.Collections.Generic;

namespace MyWebApi.Entities
{
    public class PsychologicalTestModel : BaseTestModel
    {
        public List<Dictionary<string, List<string>>> Questions { get; set; }
        public List<string> Photos { get; set; } // -> Optional
    }
}
