using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.TestEntities
{
    public class UploadTestQuestion
    {
        public string Text { get; set; }
        public string Photo { get; set; }
        public List<UploadTestAnswer> Answers { get; set; }
    }
}
