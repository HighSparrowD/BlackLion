using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.TestEntities
{
    public class IntellectualTest
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<IntellectualTestQuestion> Questions { get; set; }
    }
}
