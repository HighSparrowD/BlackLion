using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.TestEntities
{
    public class UploadPsychologicalTestQuestion
    {
        public string Text { get; set; }
        public List<UploadPsychologicalTestAnswer> Answers { get; set; }
    }
}
