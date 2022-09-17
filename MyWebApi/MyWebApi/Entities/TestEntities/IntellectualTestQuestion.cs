using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.TestEntities
{
    public class IntellectualTestQuestion
    {
        [Key]
        public long Id { get; set; }
        public long IntellectualTestId { get; set; }
        public string Text { get; set; }
        public virtual List<IntellectualTestAnswer> Answers { get; set; }
    }
}
