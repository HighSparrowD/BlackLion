using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.TestEntities
{
    public class UploadPsychologicalTest
    {
        public long Id { get; set; }
        public int ClassLocalisationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<UploadPsychologicalTestQuestion> Questions { get; set; }
    }
}
